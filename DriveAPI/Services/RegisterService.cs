using DriveAPI.Database;
using DriveAPI.Models;
using DriveAPI.Models.Requests;
using DriveAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MimeTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Services
{
    public class RegisterService : IRegisterService
    {
        private DriveDbContext _context;
        private IUserFilesService _userFilesService;

        public RegisterService(DriveDbContext context, IUserFilesService userFilesService)
        {
            _context = context;
            _userFilesService = userFilesService;
        }

        public async Task<Register> AddFile(int userId, int? parentFolder, AddFileRequest request)
        {
            string relativeFilePath;

            if (parentFolder.HasValue)
            {
                var folder = await _context.Register.Where(
                    register => register.Id == parentFolder.Value).AsNoTracking().FirstOrDefaultAsync();

                if (folder == null) return null;

                relativeFilePath = await _userFilesService.SaveFile(
                    formFile: request.File,
                    relativeParentFolderPath: folder.PathOrUrl,
                    parentFolderId: folder.Id
                );
            }
            else
            {
                // IMPORTANT: When the parentFolder is null, the relativeParentFolderPath will be the user id
                // and the parentFolderId (only for the relativeFilePath) will be 0.
                // For example: If the user id is 5, the file path will be "5/0-fileName.extension"
                relativeFilePath = await _userFilesService.SaveFile(
                    formFile: request.File,
                    relativeParentFolderPath: userId.ToString(),
                    parentFolderId: 0
                );
            }

            if (relativeFilePath == null) return null;

            var fileToAdd = _context.Register.Add(new Register
            {
                Name = request.File.FileName,
                PathOrUrl = relativeFilePath,
                FileExtension = Path.GetExtension(path: relativeFilePath),
                Size = (int?)request.File.Length,
                UploadDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                IsFolder = false,
                ParentFolder = parentFolder,
                Author = userId,
            });

            var entriesWritten = await _context.SaveChangesAsync();

            if (entriesWritten > 0) return fileToAdd.Entity;

            return null;
        }

        public async Task<Register> AddFolder(int userId, int? parentFolder, AddFolderRequest request)
        {
            string relativeFolderPath;

            if (parentFolder.HasValue)
            {
                var folder = await _context.Register.Where(
                    register => register.Id == parentFolder.Value && register.Author == userId).AsNoTracking().FirstOrDefaultAsync();

                if (folder == null) return null;

                relativeFolderPath = await _userFilesService.CreateFolder(
                    folderName: request.Name,
                    relativeParentFolderPath: folder.PathOrUrl,
                    parentFolderId: folder.Id
                );
            }
            else
            {
                // IMPORTANT: When the parentFolder is null, the relativeParentFolderPath will be the user id
                // and the parentFolderId (only for the relativeFilePath) will be 0.
                // For example: If the user id is 5, the new folder's path will be "5/0-newFolder/"
                relativeFolderPath = await _userFilesService.CreateFolder(
                    folderName: request.Name,
                    relativeParentFolderPath: userId.ToString(),
                    parentFolderId: 0
                );
            }

            if (relativeFolderPath == null) return null;

            var folderToAdd = _context.Register.Add(new Register
            {
                Name = request.Name,
                PathOrUrl = relativeFolderPath,
                UploadDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                IsFolder = true,
                ParentFolder = parentFolder,
                Author = userId,
            });

            var entriesWritten = await _context.SaveChangesAsync();

            if (entriesWritten > 0) return folderToAdd.Entity;

            return null;
        }

        public async Task<Register> DeleteRegister(int userId, int registerId)
        {
            var registerToDelete = await _context.Register.Where(
                register => register.Id == registerId && register.Author == userId
            ).FirstOrDefaultAsync();

            if (registerToDelete == null) return null;

            if (registerToDelete.IsFolder)
            {
                // Delete a folder.
                var folderDeleted = await _userFilesService.DeleteFolder(registerToDelete.PathOrUrl);

                if (!folderDeleted) return null;

                await DeleteSubRegisterOfARegister(registerToDelete);
            }
            else
            {
                // Delete a file.
                var fileDeleted = await _userFilesService.DeleteFile(registerToDelete.PathOrUrl);

                if (!fileDeleted) return null;

                _context.Register.Remove(registerToDelete);
            }

            var entriesWritten = await _context.SaveChangesAsync();

            // To prevent a possible object cycle when serialize this object in the controller's response
            registerToDelete.InverseParentFolderNavigation = null;

            if (entriesWritten > 0) return registerToDelete;

            return null;
        }

        public async Task<Register> ChangeRegisterName(int userId, ChangeRegisterNameRequest request)
        {
            var registerToChange = await _context.Register.Where(
                register => register.Id == request.RegisterId && register.Author == userId
            ).FirstOrDefaultAsync();

            if (registerToChange == null) return null;

            var newRelativePath = GenerateNewRelativePathForChangeName(registerToChange, request.NewName);

            if (registerToChange.IsFolder)
            {
                //Rename the folder with the service.
                var folderChanged = await _userFilesService.MoveOrRenameFolder(registerToChange.PathOrUrl, newRelativePath);

                if (!folderChanged) return null;

                // Change the name of the register
                registerToChange.Name = request.NewName;
                //Replace the relative path of the register and its subregisters (recursively)
                await ChangeRelativePathOfSubRegisters(
                    register: registerToChange,
                    oldRelativePath: registerToChange.PathOrUrl,
                    newRelativePath: newRelativePath
                );
            }
            else
            {
                // Rename the file with the service.
                var fileChanged = await _userFilesService.MoveOrRenameFile(registerToChange.PathOrUrl, newRelativePath);

                if (!fileChanged) return null;

                // Change the name and the path of the register.
                registerToChange.Name = request.NewName;
                registerToChange.PathOrUrl = registerToChange.PathOrUrl.Replace(
                    registerToChange.PathOrUrl, newRelativePath
                );

                _context.Register.Update(registerToChange);
            }

            var entriesWritten = await _context.SaveChangesAsync();

            // To prevent a possible object cycle when serialize this object in the controller's response
            registerToChange.InverseParentFolderNavigation = null;

            if (entriesWritten > 0) return registerToChange;

            return null;
        }

        public async Task<IEnumerable<Register>> GetSubRegistersFromFolder(int userId, int? folderId)
        {
            var subRegisters = await _context.Register.Where(
                    r => r.ParentFolder == folderId && r.Author == userId
                ).AsNoTracking().ToArrayAsync();

            if (subRegisters == null) return null;

            return subRegisters;
        }

        public async Task<Register> GetRegister(int userId, int registerId)
        {
            var register = await _context.Register.Where(
                r => r.Id == registerId && r.Author == userId
            ).AsNoTracking().FirstOrDefaultAsync();

            return register;
        }

        public async Task<FileRegister> GetFile(int userId, int registerId)
        {
            var register = await _context.Register.Where(
                r => r.Id == registerId && r.Author == userId && !r.IsFolder
            ).AsNoTracking().FirstOrDefaultAsync();

            if (register == null) return null;

            var bytesOfFile = await _userFilesService.GetBytesOfFile(relativeFilePath: register.PathOrUrl);

            if (bytesOfFile == null) return null;

            return new FileRegister
            {
                Name = register.Name,
                ExtensionFile = register.FileExtension,
                MimeType = MimeTypeMap.GetMimeType(register.FileExtension.ToLower()),
                Bytes = bytesOfFile,
            };
        }

        public async Task<bool> DoesFolderBelongToUser(int userId, int folderId)
        {
            return await _context.Register.AnyAsync(
                register => register.Id == folderId && register.Author == userId && register.IsFolder
            );
        }

        public async Task<bool> DoesRegisterBelongToUser(int userId, int registerId)
        {
            return await _context.Register.AnyAsync(
                register => register.Id == registerId && register.Author == userId
            );
        }

        public async Task<bool> DoesFileOrFolderAlreadyExist(int userId, string name, int? parentFolder)
        {
            return await _context.Register.AnyAsync(
                register => register.Name == name &&
                    register.Author == userId &&
                    register.ParentFolder == parentFolder
            );
        }

        public async Task<int?> GetParentFolderForRegister(int registerId)
        {
            var register = await _context.Register.Where(r => r.Id == registerId)
                .AsNoTracking().FirstOrDefaultAsync();

            if (register == null) return null;

            return register.ParentFolder;
        }

        /// <summary>
        /// Deletes the register specified and its sub registers (folders and files) recursively
        /// </summary>
        /// <param name="register">The register to delete</param>
        /// <returns></returns>
        private async Task DeleteSubRegisterOfARegister(Register register)
        {
            var subRegisters = await _context.Register.Where(r=> r.ParentFolder == register.Id).ToArrayAsync();

            if (subRegisters.Any())
            {
                foreach(var subRegister in subRegisters)
                {
                    await DeleteSubRegisterOfARegister(subRegister);
                }
            }

            _context.Register.Remove(register);
        }

        private async Task ChangeRelativePathOfSubRegisters(Register register, string oldRelativePath, string newRelativePath)
        {
            var subRegisters = await _context.Register.Where(r => r.ParentFolder == register.Id).ToArrayAsync();

            if (subRegisters.Any())
            {
                foreach (var subRegister in subRegisters)
                {
                    await ChangeRelativePathOfSubRegisters(subRegister, oldRelativePath, newRelativePath);
                }
            }

            register.PathOrUrl = register.PathOrUrl.Replace(oldRelativePath, newRelativePath);

            _context.Register.Update(register);
        }

        private string GenerateNewRelativePathForChangeName(Register registerToChange, string newName)
        {
            // 0 for user's root folder
            int parentFolderId = registerToChange.ParentFolder ?? 0;
            return registerToChange.PathOrUrl.Replace(
                $"{parentFolderId}-{registerToChange.Name}",
                $"{parentFolderId}-{newName}"
            );
        }
    }
}
