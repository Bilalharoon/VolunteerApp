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

        public string Password { get; set; }

        public string Role { get; set; } = Roles.Volunteer;
        
        /// <summary>
        /// The long-term token that is used to refresh other tokens 
        /// </summary>
        public string AccessToken { get; set; }


        [NotMapped]
        public string Token { get; set; }

        [JsonIgnore]
        public virtual List<UserEvent> UserEvents { get; set; }

        [NotMapped]
        public List<EventModel> Events => UserEvents.Select(userEvent => userEvent.Events).ToList();

    }
}
