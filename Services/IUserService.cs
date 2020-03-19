using ExampleAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAPI.Services
{
    public interface IUserService
    {
        UserModel Register(UserModel user);
        UserModel Login(UserModel user);
        List<UserModel> GetUsers();
    }

    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public UserService(IConfiguration configuration, ApplicationDbContext applicationDbContext)
        {
            _config = configuration;
            _context = applicationDbContext;
        }
        string GenerateToken(UserModel user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)


            };

            // get the key from appsettings.json
            var keyString = _config.GetSection("key").Value;

            // Turn the secret key into a string of bytes
            var secretBytes = Encoding.UTF8.GetBytes(keyString);

            // Turn bytes into signiture to verify token
            var key = new SymmetricSecurityKey(secretBytes);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the Token with the credentials
            var token = new JwtSecurityToken(
                    issuer: "localhost",
                    audience: "localhost",
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: signingCredentials
                );

            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenJson;
        }

        public UserModel Login( UserModel user)
        {
            var verifiedUser = _context.Users.AsEnumerable().SingleOrDefault(x => x.Username == user.Username && VerifyHash(x.Password, user.Password));

            // authentication successful so generate jwt token
            verifiedUser.Token = GenerateToken(verifiedUser);

            _context.Users.Update(verifiedUser);
            _context.SaveChanges();
            // remove password before returning
            verifiedUser.Password = null;

            return verifiedUser;
        }

        public UserModel Register(UserModel user)
        {
            

            // Generate random salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            user.Password = Hash(user.Password, salt);
            _context.Add(user);
            _context.SaveChanges();

            // remove password before returning
            user.Password = null;
            return user;
        }

        string Hash(string password, byte[] salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
              password: password,
              salt: salt,
              prf: KeyDerivationPrf.HMACSHA256,
              iterationCount: 10000,
              numBytesRequested: 256 / 8));
            return hashed + ":" + Convert.ToBase64String(salt);
        }

        bool VerifyHash(string hash, string plainText)
        {
            // seperate hash from salt
            string[] SplithashedPass = hash.Split(":");

            // convert salt to bytes
            byte[] salt = Convert.FromBase64String(SplithashedPass[1]);


            // hash the entered password with the same salt
            string userPassHash = Hash(plainText, salt);

            // check if the passwords match
            if (hash == userPassHash)
            {
                return true;
            }

            return false;
        }

        public List<UserModel> GetUsers()
        {
            var users = _context.Users.ToList();

            users.ForEach(u => u.Password = null);

            return users;
        }
    }
}
