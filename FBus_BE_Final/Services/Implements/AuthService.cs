using FBus_BE.DTOs.AuthDTOs;
using FBus_BE.Enums;
using FBus_BE.Models;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FBus_BE.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly FbusMainContext _context;

        public AuthService(FbusMainContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponse> Authenticate(string idToken)
        {
            GoogleJsonWebSignature.Payload? payload = await GetPayload(idToken);
            if (payload != null)
            {
                Account? account = await _context.Accounts.FirstOrDefaultAsync(account => account.Email == payload.Email);
                if (account != null)
                {
                    payload.Subject = Hash(payload.Subject);
                    string? issuer, audience;

                    switch (account.Status)
                    {
                        case (byte)AccountStatusEnum.Unsigned:
                            account.Password = payload.Subject;
                            account.Status = (byte)AccountStatusEnum.Active;
                            await _context.SaveChangesAsync();
                            if (account.Password == payload.Subject)
                            {
                                issuer = _configuration["JWT:Issuer"];
                                audience = _configuration["JWT:Audience"];
                                SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

                                SigningCredentials signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                                JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                                    issuer,
                                    audience,
                                    new List<Claim> {
                                        new Claim("Id", account.Id.ToString()),
                                        new Claim("Role", account.Role)
                                    }, null,
                                    DateTime.Now.AddMinutes(30), signingCredentials);
                                string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                                return new AuthResponse()
                                {
                                    AccessToken = token,
                                    Role = account.Role,
                                    Code = account.Code,
                                    Name = payload.Name,
                                    Picture = payload.Picture
                                };
                            }
                            break;
                        case (byte)AccountStatusEnum.Active:
                            if (account.Password == payload.Subject)
                            {
                                issuer = _configuration["JWT:Issuer"];
                                audience = _configuration["JWT:Audience"];
                                SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

                                SigningCredentials signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                                JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(issuer, audience, new List<Claim> { new Claim("Id", account.Id.ToString()), new Claim("Role", account.Role) }, null, DateTime.Now.AddDays(30), signingCredentials);
                                string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                                return new AuthResponse()
                                {
                                    AccessToken = token,
                                    Role = account.Role,
                                    Code = account.Code,
                                    Name = payload.Name,
                                    Picture = payload.Picture
                                };
                            }
                            break;
                        case (byte)AccountStatusEnum.Inactive:
                            break;
                    }
                }
            }
            return null;
        }

        private async Task<GoogleJsonWebSignature.Payload?> GetPayload(string idToken)
        {
            try
            {
                GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new string[]
                    {
                        "319062689013-fku6m54vf3arbhrnoiij84qb0e852o28.apps.googleusercontent.com"
                    }
                };
                return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch
            {
                return null;
            }
        }

        private string Hash(string text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
