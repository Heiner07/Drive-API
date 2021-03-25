using DriveAPI.Database;
using DriveAPI.Models;
using DriveAPI.Models.Requests;
using DriveAPI.Services.Interfaces;
using DriveAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DriveAPI.Services
{
    public class UserService : IUserService
    {
        private readonly DriveDbContext _context;
        private readonly JWTSettings _jwtSettings;

        public UserService(DriveDbContext context, IOptions<JWTSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;

        }

        public async Task<AuthenticatedUser> AuthenticateUser(AuthenticationRequest request)
        {
            string encryptedPassword = EncrypterUtility.StringToSHA256String(value: request.Password);

            // Find a user by the Email or Username
            var userFound = await _context.UserDrive.Where(
                user => (user.Email == request.EmailOrUsername && user.Password == encryptedPassword) ||
                        (user.Username == request.EmailOrUsername && user.Password == encryptedPassword)
            ).AsNoTracking().FirstOrDefaultAsync();

            if (userFound == null) return null;

            var authenticatedUser = new AuthenticatedUser
            {
                Email = userFound.Email,
                Username = userFound.Username,
                Name = userFound.Name,
                Lastname = userFound.Lastname,
                Token = GenerateToken(user: userFound),
            };

            return authenticatedUser;
        }

        public async Task<AuthenticatedUser> RegisterUser(RegisterUserRequest request) {
            string encryptedPassword = EncrypterUtility.StringToSHA256String(value: request.Password);

            var userToRegister = new UserDrive
            {
                Name = request.Name,
                Lastname = request.Lastname,
                Username = request.Username,
                Email = request.Email,
                Password = encryptedPassword,
            };

            _context.UserDrive.Add(userToRegister);
            var entriesWritten = await _context.SaveChangesAsync();
            
            if(entriesWritten > 0)
            {
                return await AuthenticateUser(request: new AuthenticationRequest
                    {
                        EmailOrUsername = userToRegister.Email,
                        Password = request.Password,
                    }
                );
            }

            return null;
        }

        public async Task<bool> IsEmailAlreadyRegistered(string email)
        {
            return await _context.UserDrive.AnyAsync(user => user.Email == email);
        }

        public async Task<bool> IsUsernameAlreadyRegistered(string username)
        {
            return await _context.UserDrive.AnyAsync(user => user.Username == username);
        }

        public async Task<bool> DoesUserExist(int id)
        {
            return await _context.UserDrive.AnyAsync(user => user.Id == id);
        }

        public async Task<bool> EditUser(int id, EditUserRequest request)
        {
            var userToEdit = await _context.UserDrive.Where(user => user.Id == id).FirstOrDefaultAsync();

            if (userToEdit == null) return false;

            string encryptedPassword = EncrypterUtility.StringToSHA256String(value: request.Password);

            userToEdit.Name = request.Name;
            userToEdit.Lastname = request.Lastname;
            userToEdit.Password = encryptedPassword;

            _context.UserDrive.Update(userToEdit);
            
            var entriesWritten = await _context.SaveChangesAsync();
            
            if (entriesWritten > 0) return true;

            return false;
        }

        /// <summary>
        /// Generates a JWT based on a user
        /// </summary>
        /// <param name="user">User to generate the token</param>
        /// <returns>A string representing the JWT</returns>
        private string GenerateToken(UserDrive user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtkey = Encoding.ASCII.GetBytes(_jwtSettings.JWTKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Username", user.Username),
                }),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwtkey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            return jwtTokenHandler.WriteToken(
                token: jwtTokenHandler.CreateToken(tokenDescriptor: tokenDescriptor)
            );
        }
    }
}
