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

        private UserModel _creator;
        public virtual UserModel Creator { get { _creator.Password = null; return _creator; } set { this._creator = value; } }

        [JsonIgnore]
        public virtual List<UserEvent> Volunteers { get; set; } = new List<UserEvent>();

        [NotMapped]
        public List<UserModel> Users => Volunteers.Select(v => v.Users).ToList();


    }
}
