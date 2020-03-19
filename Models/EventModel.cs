using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ExampleAPI.Models
{
    public class EventModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public UserModel Creator { get; set; }


    }
}
