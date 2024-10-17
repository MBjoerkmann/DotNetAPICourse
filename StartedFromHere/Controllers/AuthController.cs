using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using DotnetAPI.Helpers;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }


        [AllowAnonymous]
        [HttpPost("MakeRegistration")]
        public IActionResult Register(UserForRegistrationDto userForRegistrationDto)
        {
            if (userForRegistrationDto.Password == userForRegistrationDto.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email from TutorialAppSchema.Auth where Email = '"
                 + userForRegistrationDto.Email + "'";

                IEnumerable<string> ExistingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (ExistingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }
                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistrationDto.Password, passwordSalt);

                    string sqlAddAuth = @"insert into TutorialAppSchema.Auth ([Email],
                    [PasswordHash],
                    [PassWordSalt] ) 
                    VALUES ('" + userForRegistrationDto.Email +
                    "', @PasswordHash , @PasswordSalt)";

                    List<SqlParameter> parametersRegisterUser = new List<SqlParameter>();
                    _authHelper.InsertParametersIntoList(parametersRegisterUser, "@PasswordHash", SqlDbType.VarBinary, passwordHash);
                    _authHelper.InsertParametersIntoList(parametersRegisterUser, "@PasswordSalt", SqlDbType.VarBinary, passwordSalt);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, parametersRegisterUser))
                    {
                        string sqlAddUser = @"
                            INSERT INTO TutorialAppSchema.Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender],
                                [Active]
                                
                            ) VALUES ('" + userForRegistrationDto.FirstName +
                                "', '" + userForRegistrationDto.LastName +
                                "', '" + userForRegistrationDto.Email +
                                "', '" + userForRegistrationDto.Gender +
                                "', '" + userForRegistrationDto.Active +
                            "')";

                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("New user was not created");
                    }
                    throw new Exception("failed to register user");
                }
                throw new Exception("User already exists");
            }
            throw new Exception("Password do not match");

        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForloginDto userForloginDto)
        {

            string sqlForHashAndSalt = @"Select [Email],
                                    [PasswordHash],
                                    [PassWordSalt] FROM TutorialAppSchema.Auth
                                    where Email = '" + userForloginDto.Email + "'";

            UserForLoginConfirmationDto userForconfirmation = _dapper
                .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForloginDto.Password, userForconfirmation.PasswordSalt);
            //if (passwordHash == userForconfirmation.PasswordHash) won't work because of "==" weirdness with byte arrays
            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForconfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Incorrect password");
                }
            }
            string sqlUserId = "SELECT userId from TutorialAppSchema.Users where Email = '" + userForloginDto.Email + "'" ;

            int userId = _dapper.LoadDataSingle<int>(sqlUserId);

            return Ok(new Dictionary<string, string> {
                { "token", _authHelper.CreateToken(userId) },
            });
        }


        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";
            string userIdSql = @"SELECT UserId 
                                    FROM TutorialAppSchema.Users 
                                    WHERE UserId = " + userId;

            int userIdDB = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userIdDB)}
                });

        }


    }
}