using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Services.Interfaces
{
    public interface IUserFilesService
    {
        public Task<byte[]> GetBytesOfFile(string relativeFilePath);

        /// <summary>
        /// Saves a file in the specified parent folder and returns the relative path where was saved
        /// </summary>
        /// <param name="formFile">The file to save</param>
        /// <param name="parentFolder">The relative path of the folder where the file will be saved</param>
        /// <returns>If it was success, returns the relative path where the file was saved,
        /// otherwise returns null</returns>
        Task<string> SaveFile(IFormFile formFile, string parentFolder);

        /// <summary>
        /// Creates a folder in the specified parent folder and returns the relative path where was created
        /// </summary>
        /// <param name="folderName">The name of the folder to create</param>
        /// <param name="parentFolder">The relative path of the folder where the new folder will be created</param>
        /// <returns>If it was success, returns the relative path where the folder was created,
        /// otherwise returns null</returns>
        Task<string> CreateFolder(string folderName, string parentFolder);

        Task<bool> UpdateFile();
        Task<bool> DeleteFile(string relativeFilePath);
        Task<bool> DeleteFolder(string relativeFolderPath);
    }
}
