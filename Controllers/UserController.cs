using ExampleAPI.Models;
using Microsoft.AspNetCore.Mvc;
using ExampleAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

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
        public ActionResult<List<UserModel>> Get(){

            return Ok(_service.GetUsers());
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("User is Invalid");
            }

            UserModel response = _service.Login(user);
            if(response != null)
            {
                return Ok(response);
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

            // Generate random salt
            UserModel response = _service.Register(user);
            return Ok(user);

        }



        

        
    }
}