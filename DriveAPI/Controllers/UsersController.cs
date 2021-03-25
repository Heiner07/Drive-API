using DriveAPI.Constants;
using DriveAPI.Database;
using DriveAPI.Models.Requests;
using DriveAPI.Services.Interfaces;
using DriveAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DriveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/<UsersController>/Login
        [HttpGet("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            var authenticatedUser = await _userService.AuthenticateUser(request: authenticationRequest);
            
            if (authenticatedUser == null) return NotFound();

            return Ok(authenticatedUser);
        }

        // POST api/<UsersController>/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerRequest)
        {
            if (await _userService.IsEmailAlreadyRegistered(email: registerRequest.Email))
                return BadRequest(Texts.EMAIL_REPEATED);

            if(await _userService.IsUsernameAlreadyRegistered(username: registerRequest.Username))
                return BadRequest(Texts.USERNAME_REPEATED);

            var userRegistered = await _userService.RegisterUser(request: registerRequest);

            if (userRegistered == null) return StatusCode(statusCode: 500, value: Texts.ERROR_REGISTERING_THE_USER);

            return Ok(userRegistered);
        }

        // PUT api/<UsersController>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EditUserRequest request)
        {
            var userIdString = JWTUtility.GetUserId(User);
            if (userIdString == null) return BadRequest();

            var userId = int.Parse(userIdString);

            if (!await _userService.DoesUserExist(id: userId))
                return NotFound();

            if (await _userService.EditUser(id: userId, request: request))
                return Ok();

            return StatusCode(statusCode: 500, value: Texts.ERROR_EDITING_THE_USER);
        }
    }
}
