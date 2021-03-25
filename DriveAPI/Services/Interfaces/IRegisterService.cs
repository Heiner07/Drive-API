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
        Task<Register> GetRegister(int userId, int registerId);
        Task<FileRegister> GetFile(int userId, int registerId);
        Task<IEnumerable<Register>> GetAllRegisters(int userId);

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
        Task<Register> EditRegister(int userId, Register request);
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
    }
}
