using DriveAPI.Database;
using DriveAPI.Models;
using DriveAPI.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Services.Interfaces
{
    public interface IRegisterService
    {
        /// <summary>
        /// Get a register of the user based on the id.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="registerId">The register id</param>
        /// <returns>Returns the register, null if a error occurred</returns>
        Task<Register> GetRegister(int userId, int registerId);

        /// <summary>
        /// Get a file of the system, returns a FileRegister object with the necessary data
        /// to send to the user.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="registerId">The register id that represents the file</param>
        /// <returns>A FileRegister object with the file data, null if an error occurred</returns>
        Task<FileRegister> GetFile(int userId, int registerId);

        /// <summary>
        /// Get all the subregisters (files and folders) from a folder of the user.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="folderId">The register id that represents the folder, if it's null, it is the user's root folder</param>
        /// <returns>A list of subregisters, null if an error occurred</returns>
        Task<IEnumerable<Register>> GetSubRegistersFromFolder(int userId, int? folderId);

        /// <summary>
        /// Creates a file in the parent folder specified and add a register to the system for control.
        /// If the parent folder is null, the user folder root (user id) will be the parent.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="parentFolder">The Id of the parent folder where the new file will be created</param>
        /// <param name="request">The request with the data to create the file</param>
        /// <returns>The Register added to the database, null if an error occurred</returns>
        Task<Register> AddFile(int userId, int? parentFolder, AddFileRequest request);

        /// <summary>
        /// Creates a folder in the parent folder specified and add a register to the system for control.
        /// If the parent folder is null, the user folder root (user id) will be the parent.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="parentFolder">The Id of the parent folder where the new folder will be created</param>
        /// <param name="request">The request with the data to create the folder</param>
        /// <returns>The Register added to the database, null if an error occurred</returns>
        Task<Register> AddFolder(int userId, int? parentFolder, AddFolderRequest request);
        
        Task<Register> ChangeRegisterName(int userId, ChangeRegisterNameRequest request);

        /// <summary>
        /// Deletes a register of the user. If the register is a folder,
        /// all the content inside will be deleted too.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="registerId">The register id to delete</param>
        /// <returns>The register deleted, null if an error occurred</returns>
        Task<Register> DeleteRegister(int userId, int registerId);

        Task<bool> DoesFolderBelongToUser(int userId, int folderId);

        Task<bool> DoesRegisterBelongToUser(int userId, int registerId);

        /// <summary>
        /// Determines whether a file or folder with the value of the
        /// name argument exists in the provided parent folder
        /// </summary>
        /// <param name="userId">The user Id to ensure that parent folder belongs to the user</param>
        /// <param name="name">The name to search</param>
        /// <param name="parentFolder">The folder where to search</param>
        /// <returns>If a file or folder with the name argument exists</returns>
        Task<bool> DoesFileOrFolderAlreadyExist(int userId, string name, int? parentFolder);

        Task<int?> GetParentFolderForRegister(int registerId);
    }
}
