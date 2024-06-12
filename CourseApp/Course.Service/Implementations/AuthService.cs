using System;
using Course.Core.Entities;
using Course.Service.Dtos.UserDtos;
using Course.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Course.Service.Implementations
{
	public class AuthService:IAuthService
	{
        private readonly UserManager<AppUser> _userManager;

        public AuthService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public string Login(UserLoginDto loginDto)
        {
            throw new NotImplementedException();
        }
    }
}

