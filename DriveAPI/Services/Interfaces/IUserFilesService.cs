using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Services.Interfaces
{
    public interface IUserFilesService
    {
        /// <summary>
        /// Gets a file from the system as a byte array.
        /// </summary>
        /// <param name="relativeFilePath">The relative where the file is located</param>
        /// <returns>A byte array, null if an error occurred reading the file</returns>
        public Task<byte[]> GetBytesOfFile(string relativeFilePath);

        /// <summary>
        /// Saves a file in the specified parent folder and returns the relative path where was saved.
        /// 
        /// Note about parentFolderId: It is necessary to prevent equals sub paths,
        /// in this implementation is necessary because, when a folder is moved, update the relative paths
        /// of the subregisters could be fail if there are equals sub paths.
        /// 
        /// </summary>
        /// <param name="formFile">The file to save</param>
        /// <param name="relativeParentFolderPath">The relative path of the folder where the file will be saved</param>
        /// <param name="parentFolderId">The id of the parent folder to attach as a prefix of the relative path of the file</param>
        /// <returns>If it was success, returns the relative path where the file was saved,
        /// otherwise returns null</returns>
        Task<string> SaveFile(IFormFile formFile, string relativeParentFolderPath, int parentFolderId);

        /// <summary>
        /// Creates a folder in the specified parent folder and returns the relative path where was created.
        /// 
        /// Note about parentFolderId: It is necessary to prevent equals sub paths,
        /// in this implementation is necessary because, when a folder is moved, update the relative paths
        /// of the subregisters could be fail if there are equals sub paths.
        /// </summary>
        /// <param name="folderName">The name of the folder to create</param>
        /// <param name="relativeParentFolderPath">The relative path of the folder where the new folder will be created</param>
        /// <param name="parentFolderId">The id of the parent folder to attach as a prefix of the relative path of the folder</param>
        /// <returns>If it was success, returns the relative path where the folder was created,
        /// otherwise returns null</returns>
        Task<string> CreateFolder(string folderName, string relativeParentFolderPath, int parentFolderId);

        /// <summary>
        /// Moves a file to a folder. If the origin folder (present in oldRelativeFilePath) is
        /// equals to the destiny folder (present in the newRelativeFilePath), but with different
        /// file name, this will result in a change of file name.
        /// </summary>
        /// <param name="oldRelativeFilePath">The path of the origin file</param>
        /// <param name="newRelativeFilePath">The new path where will be located</param>
        /// <returns>If the operation was successful</returns>
        Task<bool> MoveOrRenameFile(string oldRelativeFilePath, string newRelativeFilePath);

        /// <summary>
        /// Moves a folder to another folder. If the origin folder (present in oldRelativeFolderPath) is
        /// equals to the destiny folder (present in the newRelativeFolderPath), but with different
        /// name, this will result in a change of folder name.
        /// </summary>
        /// <param name="oldRelativeFolderPath">The path of the origin folder</param>
        /// <param name="newRelativeFolderPath">The new path where will be located</param>
        /// <returns>If the operation was successful</returns>
        Task<bool> MoveOrRenameFolder(string oldRelativeFolderPath, string newRelativeFolderPath);

        /// <summary>
        /// Deletes a file from the system
        /// </summary>
        /// <param name="relativeFilePath">The relative path where the file is located</param>
        /// <returns>If the file was successfully deleted</returns>
        Task<bool> DeleteFile(string relativeFilePath);

        /// <summary>
        /// Deletes a folder and all its content in the system
        /// </summary>
        /// <param name="relativeFolderPath">The relative path where the folder is located</param>
        /// <returns>If the folder was successfully deleted</returns>
        Task<bool> DeleteFolder(string relativeFolderPath);
    }
}
