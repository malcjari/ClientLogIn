using ClientLogIn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientLogIn.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly MyContext _context;
        private readonly ILogger<LoginController> _logger;

        public LoginController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            MyContext context,
            RoleManager<Role> roleManager,
            ILogger<LoginController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {

                _logger.LogError(e.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            _logger.LogInformation($"Login Attempt, User: {loginModel.Username}");
            try
            {
                if (ModelState.IsValid)
                {


                    var user = await _userManager.FindByNameAsync(loginModel.Username);

                    if (user != null)
                    {
                        var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, false);
                        if (result.Succeeded)
                        {

                            var roleList = await _userManager.GetRolesAsync(user);

                            foreach (var item in roleList)
                            {
                                if (item == "SysAdmin")
                                {
                                    return RedirectToAction("AdminProfile", "User");
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Profile");
                                }
                            }


                        }
                    }


                    ModelState.AddModelError("", "Invalid user login details");
                }
                return View(loginModel);

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

            }
            return View();
        }
            

        public async Task<IActionResult> Logout()
        {

            try
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Login");
            }
            catch (Exception e)
            {

                _logger.LogError(e.Message);
            }
            return View();
        }
    }
}
