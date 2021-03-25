using DriveAPI.Models;
using DriveAPI.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Authenticates a user in the system.
        /// </summary>
        /// <param name="request">The request with the user information</param>
        /// <returns>An AuthenticatedUser with its token, if the request is correct,
        /// otherwise returns null.</returns>
        Task<AuthenticatedUser> AuthenticateUser(AuthenticationRequest request);

        /// <summary>
        /// Registers a user in the system.
        /// </summary>
        /// <param name="request">The request with the user information</param>
        /// <returns>An AuthenticatedUser with its token, if the request is correct,
        /// otherwise returns null.</returns>
        Task<AuthenticatedUser> RegisterUser(RegisterUserRequest request);

        Task<bool> IsEmailAlreadyRegistered(string email);

        Task<bool> IsUsernameAlreadyRegistered(string username);

        Task<bool> DoesUserExist(int id);

        /// <summary>
        /// Edit a user according to the data in the request
        /// </summary>
        /// <param name="id">The user Id</param>
        /// <param name="request">A request with the new data to replace in the user</param>
        /// <returns>If the changes were applied and saved successfully</returns>
        Task<bool> EditUser(int id, EditUserRequest request);
    }
}
