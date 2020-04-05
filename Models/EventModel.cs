using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace ExampleAPI.Models
{
    // represents an event
    public class EventModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Foreign key that points to the UserModel that created this event
        public int CreatorId { get; set; }
        public UserModel Creator { get; set; }

        [JsonIgnore]
        public List<UserEvent> Volunteers { get; set; }

        [NotMapped]
        public List<UserModel> Users { get; set; }


    }
}
