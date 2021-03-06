﻿using ClientLogIn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var Id = _userManager.GetUserId(HttpContext.User);
            var returnList = _context.Users.Where(s => s.Id.ToString() != Id).ToList();
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
            ViewModel v = new ViewModel();
            List<Role> roles = _context.Roles.ToList();
            ViewBag.selectrole = new SelectList(roles, "Name", "Name" );
            return View(v); 
        }
        [Authorize(Roles = "SysAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(ViewModel viewModel)
        {
            var userExist = await _userManager.FindByNameAsync(viewModel.user.UserName);
  
            if(userExist != null)
            {

                ViewBag.usrExist = "User already exists, login";
            }
            else
            {
                var createUser = await _userManager.CreateAsync(viewModel.user, viewModel.user.PasswordHash);
                var user = await _userManager.FindByNameAsync(viewModel.user.UserName);
                var resultat = await _userManager.AddToRoleAsync(user, viewModel.role.Name);
                if (createUser.Succeeded)
                {
                    if (resultat.Succeeded)
                    {
                        ViewBag.successMsg = "User Successfully created";
                    }
                    else
                    {
                        ViewBag.failMsg = "User misslyckad";
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    return RedirectToAction("Create");
                }
            }
            
            return RedirectToAction("GetAllUsers");
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

            var editUsr = await _userManager.GetUserAsync(HttpContext.User);
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
            return RedirectToAction("AdminProfile");
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


  

   
        public IActionResult AdminProfile()
        {
            ViewModel v = new ViewModel();
            ViewBag.Msg = User.Identity.Name;

          var Userid = _userManager.GetUserId(HttpContext.User);
           
           
           
          v.user  = _userManager.FindByIdAsync(Userid).Result;
          return View(v);
            

           
            
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
