using System;
using Course.Core.Entities;
using Course.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
	{
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;


        public AuthController(IAuthService authService,UserManager<AppUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;

        }
        [HttpGet("users")]
        public async Task<IActionResult> CreateUser()
        {
            AppUser user = new AppUser
            {
                FullName = "Elmar Qarayev",
                UserName = "elm111",
            };

            await _userManager.CreateAsync(user, "elmar123");
            return Ok(user.Id);
        }
    }
}

