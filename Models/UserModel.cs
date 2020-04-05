using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Models
{
    public static class Roles
    {
        public static string Organizer { get; set; } = "Organizer";
        public static string Volunteer { get; set; } = "Volunteer";
        public static string Admin { get; set; } = "Admin";


    }
    public class UserModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
        public string Role { get; set; } = Roles.Volunteer;

        [NotMapped]
        [JsonIgnore]
        public string Token { get; set; }

        
        [JsonIgnore]
        public List<UserEvent> Events { get; set; }

    }
}
