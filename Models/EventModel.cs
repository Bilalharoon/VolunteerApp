using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace ExampleAPI.Models
{
    public class EventModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }


        public int CreatorId { get; set; }
        public UserModel Creator { get; set; }


    }
}
