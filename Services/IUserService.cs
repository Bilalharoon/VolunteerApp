using ExampleAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
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
using ExampleAPI.Helpers;

namespace ExampleAPI.Services
{
    public interface IUserService
    {
        UserModel Register(UserModel user);
        UserModel Login(UserModel user);
        List<UserModel> GetUsers();
        UserModel GetUserById(int id);
        UserEvent VolunteerForEvent(int eventId, int userId);
        
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

        private string GenerateToken(UserModel user)
        {
            // store information about the user
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())

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
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: signingCredentials
                );

            // turn token into string
            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenJson;
        }

        public UserModel Login(UserModel user)
        {
            // Get user with same username and password
            var verifiedUser = _context.Users.AsEnumerable().SingleOrDefault(x => x.Username.ToUpper() == user.Username.ToUpper() && VerifyHash(x.Password, user.Password));

            if (verifiedUser == null)
            {
                throw new HttpResponseException("Username or Password is incorrect");
            }

            if (user.RefreshToken != verifiedUser.RefreshToken)
            {
                throw new HttpResponseException("Refresh Token is incorrect");
            }

            // authentication successful so generate jwt token
            string token = GenerateToken(verifiedUser);
            verifiedUser.Token = token;
            verifiedUser.Password = null;
            return verifiedUser;
            

        }

        public UserModel Register(UserModel user)
        {
            //check that username is not already taken
            var checkUser = _context.Users.SingleOrDefault(u => u.Username.ToUpper() == user.Username.ToUpper());
            if (checkUser != null)
            {
                throw new HttpResponseException("Username is taken");
            }
            // Generate random salt
            byte[] salt = new byte[128 / 8];

            // Populate salt
            using (var rng = RandomNumberGenerator.Create())
            {
                
                rng.GetBytes(salt);
            }

            // Hash the password 
            user.Password = Hash(user.Password, salt);

            // Create a refresh token
            user.RefreshToken = GenerateRefreshToken(user);
            // Save the user to the database
            _context.Add(user);
            _context.SaveChanges();

            //create a token
            user.Token = GenerateToken(user);
            
            return user;
        }

        // Method to hash passwords
        // format: hash:salt
        private string Hash(string password, byte[] salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
              password: password,
              salt: salt,
              prf: KeyDerivationPrf.HMACSHA256,
              iterationCount: 10000,
              numBytesRequested: 256 / 8));
            return hashed + ":" + Convert.ToBase64String(salt);
        }

        // verify a password by hashing the password inputed by the user with the same salt
        // and checking if they are the same
        private bool VerifyHash(string hash, string plainText)
        {
            // seperate hash from salt
            string[] SplithashedPass = hash.Split(":");

            // convert salt to bytes
            byte[] salt = Convert.FromBase64String(SplithashedPass[1]);


            // hash the entered password with the same salt
            string userPassHash = Hash(plainText, salt);

            // check if the passwords match
            return hash == userPassHash;
        }

        // Get all Users
        public List<UserModel> GetUsers()
        {
            var users = _context.Users.ToList();

            // remove the passwords before sending
            users.ForEach(u => u.Password = null);

            return users;
        }

        // Volunteer for an event.
        public UserEvent VolunteerForEvent(int eventId, int UserId)
        {
            // get the users and events from the Id
            UserModel user = _context.Users.FirstOrDefault(u => u.Id == UserId);
            EventModel eventModel = _context.Events.FirstOrDefault(e => e.Id == eventId);

            
            // Create the volunteer object
            UserEvent volunteer = new UserEvent { Users = user, Events = eventModel };

            // save the volunteer
            _context.UserEvents.Add(volunteer);

           
            _context.SaveChanges();

            return volunteer;
        }



        public UserModel GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            user.Password = null;
            user.Token = null;

            return user;
        }

        public string GenerateRefreshToken(UserModel user)
        {
            var token = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(token);
            return Convert.ToBase64String(token);


        }

    }
}
