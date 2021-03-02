using ClientLogIn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;


namespace ClientLogIn.Controllers
{    [Authorize(Roles = "SysAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly MyContext _context;

        public UserController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            MyContext context,
            RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [Authorize(Roles= "SysAdmin")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {

            var returnList = _context.Users.ToList();
            return View(returnList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            return View(user);
        }
        [Authorize(Roles = "SysAdmin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(); 
        }
        [Authorize(Roles = "SysAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            var userExist = await _userManager.FindByNameAsync(user.UserName);
            if(userExist != null)
            {
                ViewBag.usrExist = "User already exists, login";
            }
            else
            {
                var createUser = await _userManager.CreateAsync(user, user.PasswordHash);
                var addToRole = await _userManager.AddToRoleAsync(user, "SysAdmin");
                
                if (createUser.Succeeded)
                {
                    ViewBag.successMsg = "User Successfully created";
                }
            }
            
            return RedirectToAction(nameof(GetAllUsers));
        }

        [Authorize(Roles = "SysAdmin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id, User user)
        {
            var editUsr = await _userManager.FindByIdAsync(user.Id.ToString());
            return View(editUsr);
        }
        [Authorize(Roles = "SysAdmin")]
        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            var editUsr = await _userManager.FindByIdAsync(user.Id.ToString());
            editUsr.Name = user.Name;
            editUsr.StreetNo = user.StreetNo;
            editUsr.City = user.City;
            editUsr.ZipCode = user.ZipCode;
            editUsr.Email = user.Email;
            editUsr.PhoneNumber = user.PhoneNumber;
            var newUsr = await _userManager.UpdateAsync(editUsr);
            if (newUsr.Succeeded)
            {
                ViewBag.SuccMsg = "User successfully updated.";
            }
            else
            {
                ViewBag.Error = "Something went wrong.";
            }
            return RedirectToAction(nameof(GetAllUsers));
        }

        [Authorize(Roles = "SysAdmin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id, User user)
        {
            var usr = await _userManager.FindByIdAsync(user.Id.ToString());
            return View(usr);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, User user)
        {
            var usr = await _userManager.FindByIdAsync(user.Id.ToString());
            var del = await _userManager.DeleteAsync(usr);
            if (del.Succeeded)
            {
                ViewBag.deleteMsg = "user deleted.";
            }
            return RedirectToAction(nameof(GetAllUsers));
        }


        //[HttpGet]
        //public IActionResult Login()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginModel loginModel)
        //{
        //    var s = await _userManager.FindByNameAsync("TestTest");
        //    await _userManager.AddToRoleAsync(s,"Employee");

        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByNameAsync(loginModel.Username);
        //        await _userManager.AddToRoleAsync(user, "SysAdmin");
        //        if(user != null)
        //        {
        //            var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, false);
        //            if (result.Succeeded)
        //            {

        //                var roleList = await _userManager.GetRolesAsync(user);

        //                foreach(var item in roleList)
        //                {
        //                    if(item == "SysAdmin")
        //                    {
        //                        return RedirectToAction("AdminProfile", "User");
        //                    } else
        //                    {
        //                        return RedirectToAction("Index", "Profile");
        //                    }
        //                }

                        
        //            }
        //        }

               
        //        ModelState.AddModelError("", "Invalid user login details");
        //    }
        //    return View(loginModel);
        //}

   
        public IActionResult AdminProfile()
        {
            ViewBag.Msg = User.Identity.Name;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Login");
        }


        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(Role role)
        {
          
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("GetAllUsers", "User");
            }
            return View(role);
        }
        

    }
}
