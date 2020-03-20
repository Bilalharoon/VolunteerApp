using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleAPI.Models;
using ExampleAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        IEventService _service;
        public EventController(IEventService eventService)
        {
            _service = eventService;
        }
        
        // GET: Event
        [HttpGet]
        public ActionResult<List<EventModel>> Get()
        {
            var events = _service.GetEvents();

            if (events != null)
            {
                return Ok(events);
            }
            else
            {
                return BadRequest();
            }
        }

        // GET: Event/5
        [HttpGet("{id}", Name = "Get")]
        public ActionResult<EventModel> Get(int id)
        {
            EventModel e = _service.GetEvent(id);

            if (e != null)
            {
                return e;
            }
            else
            {
                return BadRequest();
            }
        }

        [Route("GetByUser/{id}")]
        [HttpGet]
        public ActionResult<List<EventModel>> GetByUser(int id)
        {
            var result = _service.GetEventsByUser(id);

            if (result != null)
            {
                return result;
            }
            else
            {
                return BadRequest(@"User does not exist or has no events ¯\_(ツ)_/¯");
            }
        }
        // POST: Event
        [HttpPost]
        [Authorize(AuthenticationSchemes="OAuth",Roles="Organizer, Admin")]
        public ActionResult Post([FromBody] EventModel value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Event");
            }
            _service.CreateEvent(value);
            return CreatedAtAction(nameof(Get), new { Id = value.Id }, value);
        }

        // PUT: api/Event/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
