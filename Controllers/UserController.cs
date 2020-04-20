using ExampleAPI.Models;
using Microsoft.AspNetCore.Mvc;
using ExampleAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Security.Claims;
using System;

namespace ExampleAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _service;

        public UserController(IUserService userService)
        {
            _service = userService;
        }

        [Authorize(AuthenticationSchemes = "OAuth", Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<UserModel>> Get() {

            return Ok(_service.GetUsers());
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<UserModel> Get(int id)
        {
            return Ok(_service.GetUserById(id));
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest("User is Invalid");
            }

           
            var verifiedUser = _service.Login(user);
            return Ok(verifiedUser);
        
            
            

        }


        [Route("Register")]
        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            
            var userResponse = _service.Register(user);
            
            return Ok(userResponse);

        }


        // TODO: change to post on /Event
        [Route("SignUpForEvent/{id}")]
        [Authorize(Roles = "Volunteer", AuthenticationSchemes = "OAuth")]
        [HttpGet]
        public ActionResult SignUpForEvent(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            UserEvent vol = _service.VolunteerForEvent(id, userId);
            
            if(vol != null)
            {
                
                return Ok(vol);
            }
            else
            {
                return BadRequest();
            }
        }


    }
}
  



