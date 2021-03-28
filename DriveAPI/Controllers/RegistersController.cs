using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveAPI.Database;
using DriveAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using DriveAPI.Utilities;
using DriveAPI.Constants;
using DriveAPI.Models.Requests;

namespace DriveAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RegistersController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        public RegistersController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        // GET: api/Registers
        [HttpGet("FromFolder/{id}")]
        public async Task<ActionResult<IEnumerable<Register>>> GetRegistersFromFolder(int id)
        {
            var userIdString = JWTUtility.GetUserId(User);
            if (userIdString == null) return BadRequest();

            var userId = int.Parse(userIdString);

            int? registerId;
            if (id == 0) { registerId = null; } else { registerId = id; }

            // Check is the register does belong to the user
            // If the register is null, the folder is the user's root folder
            if (registerId.HasValue && !await _registerService.DoesRegisterBelongToUser(userId, registerId.Value))
                return BadRequest();

            var subRegisters = await _registerService.GetSubRegistersFromFolder(userId, folderId: registerId);

            if (subRegisters == null) return NotFound();

            return Ok(subRegisters);
        }

        // GET: api/Registers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegister(int id)
        {
            var userIdString = JWTUtility.GetUserId(User);
            if (userIdString == null) return BadRequest();

            var userId = int.Parse(userIdString);

            // Check is the register does belong to the user
            if (!await _registerService.DoesRegisterBelongToUser(userId, registerId: id))
                return BadRequest();

            var register = await _registerService.GetRegister(userId, registerId: id);

            if (register == null) return NotFound();

            return Ok(register);
        }

        // GET: api/Registers/DownloadRegister/5
        [HttpGet("DownloadRegister/{id}")]
        public async Task<IActionResult> DownloadRegister(int id)
        {
            var userIdString = JWTUtility.GetUserId(User);
            if (userIdString == null) return BadRequest();

            var userId = int.Parse(userIdString);

            // Check is the register does belong to the user
            if (!await _registerService.DoesRegisterBelongToUser(userId, registerId: id))
                return BadRequest();

            // IMPORTANT: At this time, only file downloads are allowed.
            // ToDo: Download a complete folder.
            var fileToSend = await _registerService.GetFile(userId, registerId: id);

            if (fileToSend == null) return BadRequest(Texts.ERROR_PREPARING_ELEMENT_TO_DOWNLOAD);

            return File(
                fileContents: fileToSend.Bytes,
                contentType: fileToSend.MimeType,
                fileDownloadName: fileToSend.Name
            );
        }

        // PUT: api/Registers/ChangeName
        [HttpPut("ChangeName")]
        public async Task<IActionResult> ChangeRegisterName(ChangeRegisterNameRequest request)
        {
            var userIdString = JWTUtility.GetUserId(User);
            if (userIdString == null) return BadRequest();

            var userId = int.Parse(userIdString);

            // Check if the register does belong to the user.
            if (!await _registerService.DoesRegisterBelongToUser(userId, registerId: request.RegisterId))
                return BadRequest();

            // Check if the new name is valid.
            if (!FileNameUtility.FileFolderNameIsValid(request.NewName))
                return BadRequest(Texts.INVALID_FILE_NAME);

            // Check if the new name already exists in the parent folder.
            var parentFolderId = await _registerService.GetParentFolderForRegister(request.RegisterId);
            if (await _registerService.DoesFileOrFolderAlreadyExist(userId, name: request.NewName, parentFolder: parentFolderId))
                return BadRequest(Texts.FILE_FOLDER_ALREADY_EXISTS);

            var registerChanged = await _registerService.ChangeRegisterName(userId, request);

            if(registerChanged == null) return StatusCode(statusCode: 500, value: Texts.ERROR_MODIFYING_REGISTER);

            return Ok(registerChanged);
        }

        // PUT: api/Registers/Move
        [HttpPut("Move")]
        public async Task<IActionResult> MoveRegister(MoveRegisterRequest request)
        {
            var userIdString = JWTUtility.GetUserId(User);
            if (userIdString == null) return BadRequest();

            var userId = int.Parse(userIdString);

            // Check if the register and destiny folder belong to the user.
            if (!await _registerService.DoesRegisterBelongToUser(userId, registerId: request.RegisterId) ||
                (request.DestinyFolderId.HasValue &&
                    !await _registerService.DoesFolderBelongToUser(userId, folderId: request.DestinyFolderId.Value)))
                return BadRequest();
            
            // Check if the destiny folder is a sub folder of the origin folder
            if (request.DestinyFolderId.HasValue &&
                await _registerService.IsDestinyFolderASubFolderOfFolderToMove(folderToMoveId: request.RegisterId, destinyFolderId: request.DestinyFolderId.Value))
                return BadRequest(Texts.DESTINY_FOLDER_IS_SUB_FOLDER_OF_ORIGIN_FOLDER);

            var nameOfRegister = await _registerService.GetNameOfRegister(registerId: request.RegisterId);

            if(nameOfRegister == null) return StatusCode(statusCode: 500, value: Texts.ERROR_MOVING_REGISTER);

            // Check if the name of the register to move already exists in the destiny folder.
            if (await _registerService.DoesFileOrFolderAlreadyExist(userId, name: nameOfRegister, parentFolder: request.DestinyFolderId))
                return BadRequest(Texts.FILE_FOLDER_ALREADY_EXISTS_IN_DESTINY);

            var registerChanged = await _registerService.MoveRegister(userId, request);

            if (registerChanged == null) return StatusCode(statusCode: 500, value: Texts.ERROR_MOVING_REGISTER);

            return Ok(registerChanged);
        }

        // POST: api/Registers/AddFile
        [HttpPost("AddFile")]
        public async Task<IActionResult> AddFile([FromForm] AddFileRequest request)
        {
            var userIdString = JWTUtility.GetUserId(User);
            if (userIdString == null) return BadRequest();

            var userId = int.Parse(userIdString);

            if (request.ParentFolder != null)
            {
                // Check if the parent folder does belong to the user
                if (!await _registerService.DoesFolderBelongToUser(
                        userId, folderId: request.ParentFolder.Value))
                    return BadRequest();
            }

            // Check if the filename is valid
            if (!FileNameUtility.FileFolderNameIsValid(request.File.FileName))
                return BadRequest(Texts.INVALID_FILE_NAME);

            // Check if the file does not exist
            if (await _registerService.DoesFileOrFolderAlreadyExist(userId, name: request.File.FileName, parentFolder: request.ParentFolder))
                return BadRequest(Texts.FILE_FOLDER_ALREADY_EXISTS);

            var fileAdded = await _registerService.AddFile(userId, parentFolder: request.ParentFolder, request);

            if (fileAdded == null) return StatusCode(statusCode: 500, value: Texts.ERROR_SAVING_FILE);
            
            return Ok(fileAdded);
        }

        // POST: api/Registers/AddFolder
        [HttpPost("AddFolder")]
        public async Task<IActionResult> AddFolder([FromBody] AddFolderRequest request)
        {
            var userIdString = JWTUtility.GetUserId(User);
            if (userIdString == null) return BadRequest();

            var userId = int.Parse(userIdString);

            if (request.ParentFolder != null)
            {
                // Check if the parent folder does belong to the user
                if (!await _registerService.DoesFolderBelongToUser(
                        userId, folderId: request.ParentFolder.Value))
                    return BadRequest();
            }

            // Check if the folder name is valid
            if (!FileNameUtility.FileFolderNameIsValid(request.Name))
                return BadRequest(Texts.INVALID_FILE_NAME);

            // Check if the folder does not exist
            if (await _registerService.DoesFileOrFolderAlreadyExist(userId, name: request.Name, parentFolder: request.ParentFolder))
                return BadRequest(Texts.FILE_FOLDER_ALREADY_EXISTS);

            var folderAdded = await _registerService.AddFolder(userId, parentFolder: request.ParentFolder, request);

            if (folderAdded == null) return StatusCode(statusCode: 500, value: Texts.ERROR_CREATING_FOLDER);

            return Ok(folderAdded);
        }

        // DELETE: api/Registers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegister(int id)
        {
            var userIdString = JWTUtility.GetUserId(User);
            if (userIdString == null) return BadRequest();

            var userId = int.Parse(userIdString);

            // Check if the register with that Id belongs to the user.
            if (!await _registerService.DoesRegisterBelongToUser(userId, registerId: id))
                return BadRequest();

            var registerDeleted = await _registerService.DeleteRegister(userId, registerId: id);

            if(registerDeleted == null) return StatusCode(statusCode: 500, value: Texts.ERROR_DELETING_ELEMENT);

            return Ok(registerDeleted);
        }
    }
}
