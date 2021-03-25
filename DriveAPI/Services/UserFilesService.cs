﻿using DriveAPI.Services.Interfaces;
using DriveAPI.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Services
{
    public class UserFilesService : IUserFilesService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public UserFilesService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public async Task<byte[]> GetBytesOfFile(string relativeFilePath)
        {
            var filePath = Path.Combine(
                    _env.ContentRootPath, _configuration["UsersFilesPath"], relativeFilePath);

            if (!File.Exists(filePath)) return null;

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        public async Task<string> SaveFile(IFormFile formFile, string parentFolder)
        {
            if(formFile.Length > 0 && FileNameUtility.FileFolderNameIsValid(name: formFile.FileName))
            {
                var relativePath = Path.Combine(parentFolder, formFile.FileName);
                var filePath = Path.Combine(
                    _env.ContentRootPath, _configuration["UsersFilesPath"], relativePath);

                var parentFolderPath = Path.Combine(
                    _env.ContentRootPath, _configuration["UsersFilesPath"], parentFolder);

                if (!Directory.Exists(parentFolderPath))
                {
                    Directory.CreateDirectory(parentFolderPath);
                }

                using(var stream = File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                // Returns the relative path
                return relativePath;
            }

            return null;
        }

        public Task<string> CreateFolder(string folderName, string parentFolder)
        {
            if (FileNameUtility.FileFolderNameIsValid(name: folderName))
            {
                var relativePath = Path.Combine(parentFolder, folderName);
                var folderPath = Path.Combine(
                    _env.ContentRootPath, _configuration["UsersFilesPath"], relativePath);

                // To indicates that this method does not create this folder
                if (Directory.Exists(folderPath)) return null;

                Directory.CreateDirectory(folderPath);

                // Returns the relative path
                return Task.FromResult(relativePath);
            }

            return null;
        }

        public Task<bool> UpdateFile()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFile(string relativeFilePath)
        {
            var filePath = Path.Combine(
                    _env.ContentRootPath, _configuration["UsersFilesPath"], relativeFilePath);

            if (!File.Exists(filePath)) return Task.FromResult(false);

            File.Delete(filePath);

            return Task.FromResult(true);
        }

        public Task<bool> DeleteFolder(string relativeFolderPath)
        {
            var folderPath = Path.Combine(
                    _env.ContentRootPath, _configuration["UsersFilesPath"], relativeFolderPath);

            if (!Directory.Exists(folderPath)) return Task.FromResult(false);

            Directory.Delete(folderPath, true);

            return Task.FromResult(true);
        }
    }
}
