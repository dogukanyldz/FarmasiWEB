using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Farmasi.Models;

namespace Web.Farmasi.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

     
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> SignIn(User user)
        {
            var existUser = await _userManager.FindByEmailAsync(user.Email);
            if (existUser == null)
                return NotFound("User is Not Found");

            var result = await _signInManager.PasswordSignInAsync(existUser.UserName, user.Password,false,false);

            if (!result.Succeeded)
                return BadRequest();
            

            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            var existUser = await _userManager.FindByEmailAsync(user.Email);

            if (existUser != null)
                return BadRequest();

            var appUser = new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            var result = await _userManager.CreateAsync(appUser, user.Password);

            if (!result.Succeeded)
                return BadRequest();
            

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();


            return RedirectToAction("Index");
        }
    }
}
