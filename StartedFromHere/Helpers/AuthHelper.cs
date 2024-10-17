using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
        public AuthHelper(IConfiguration config)
        {
            _config = config;
        }


        /// <summary>
        /// Create a Password Hash based on a password and a salt.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                        Convert.ToBase64String(passwordSalt);

            byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 256 / 8
            );
            return passwordHash;
        }


        /// <summary>
        /// Create a JWT token for a user based on their userID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[]
            {
                new Claim("userId", userId.ToString()),
            };
            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString != null ? tokenKeyString : ""));

            SigningCredentials credentials = new SigningCredentials(
                tokenKey,
                SecurityAlgorithms.HmacSha512Signature
                );
            
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
        /// <summary>
        /// Insert a value into a list of parameters to avoid sql-injection
        /// </summary>
        /// <param name="listOfSqlParameters"></param>
        /// <param name="parameterName"></param>
        /// <param name="type"></param>
        /// <param name="ParameterValue"></param>
        public void InsertParametersIntoList(List<SqlParameter> listOfSqlParameters, string parameterName, SqlDbType type, object? ParameterValue)
        {
            SqlParameter parameter = new SqlParameter(parameterName, type);
            parameter.Value = ParameterValue;
            listOfSqlParameters.Add(parameter);
        }
    }

}