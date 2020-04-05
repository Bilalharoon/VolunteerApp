using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Models
{
    public class UserEvent
    {
        public int Id { get; set; }

        public UserModel Users { get; set; }
        public int UsersId { get; set; }    // Foreign key that points to user

        public EventModel Events { get; set; }
        public int EventsId { get; set; }   // Foreign key that points to event

        
    }
}
