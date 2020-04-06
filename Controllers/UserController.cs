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
            if (verifiedUser != null)
            {
                return Ok(verifiedUser);
            }
            return BadRequest("Incorrect Username or Password");

        }

        [Route("Register")]
        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            
            UserModel response = _service.Register(user);
            if (response == null)
            {
                return BadRequest("Username is already taken");
            }
            return Ok(response);

        }



        [Route("SignUpForEvent/{id}")]
        [Authorize(Roles = "Volunteer", AuthenticationSchemes = "OAuth")]
        [HttpGet]
        public ActionResult SignUpForEvent(int id)
        {
            int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
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
  



