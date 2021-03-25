using ClientLogIn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace ClientLogIn.Controllers
{    [Authorize(Roles = "SysAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly MyContext _context;
        private readonly ILogger<UserController> _logger;


        public UserController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            MyContext context,
            RoleManager<Role> roleManager,
            ILogger<UserController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        

        [Authorize(Roles= "SysAdmin")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            if (TempData["CreateSuccessMsg"] != null)
            {
                ViewBag.CreateSuccess = TempData["CreateSuccessMsg"];
            }

            try
            {
                var Id = _userManager.GetUserId(HttpContext.User);
                var returnList = _context.Users.Where(s => s.Id.ToString() != Id).ToList();
                return View(returnList);
            }
            catch (Exception e)
            {

                _logger.LogError(e.Message);
                RedirectToAction("AdminProfile");
            }
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {

                var user = await _userManager.FindByIdAsync(id.ToString());
                return View(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                RedirectToAction("GetAllUsers");
            }
            return View();
        }
        [Authorize(Roles = "SysAdmin")]
        [HttpGet]
        public IActionResult Create()
        {
            try
            {

                if(TempData["ExistingUserMsg"] != null)
                {
                    ViewBag.ExistingUser = TempData["ExistingUserMsg"];
                }

                if (TempData["CreateFailedMsg"] != null)
                {
                    ViewBag.CreateFailed = TempData["CreateFailedMsg"];
                }


                ViewModel v = new ViewModel();
                List<Role> roles = _context.Roles.ToList();
                ViewBag.selectrole = new SelectList(roles, "Name", "Name");
                return View(v);
            }
            catch (Exception e )
            {
                _logger.LogError(e.Message);
                RedirectToAction("GetAllUsers");
            }
            return View();
        }
        [Authorize(Roles = "SysAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(ViewModel viewModel)
        {
            try
            {
                var userExist = await _userManager.FindByNameAsync(viewModel.user.UserName);

                if (userExist != null)
                {

                    TempData["ExistingUserMsg"] = "Användaren existerar redan!";
                    return RedirectToAction("Create");
                }
                else
                {
                    var createUser = await _userManager.CreateAsync(viewModel.user, viewModel.user.PasswordHash);

                    
                    if (createUser.Succeeded)
                    {
                        var user = await _userManager.FindByNameAsync(viewModel.user.UserName);
                        var resultat = await _userManager.AddToRoleAsync(user, viewModel.role.Name);

                        TempData["CreateSuccessMsg"] = "Ny användare skapad!";
                        return RedirectToAction("GetAllUsers");

                        //if (resultat.Succeeded)
                        //{
                            
                        //}
                        //else
                        //{
                            
                        //}
                    }
                    else
                    {
                        List<string> tempList = new List<string>();
                        foreach (var error in createUser.Errors)
                        {
                            tempList.Add(error.Description);
                        }
                        TempData["CreateFailedMsg"] = tempList;
                        return RedirectToAction("Create");
                    }
                }

                return RedirectToAction("GetAllUsers");
            }
            catch (Exception e)
            {

                _logger.LogError(e.Message);
                RedirectToAction("Create");
            }
            return View();
        }

        [Authorize(Roles = "SysAdmin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id, User user)
        {
            try
            {
                var editUsr = await _userManager.FindByIdAsync(user.Id.ToString());
                return View(editUsr);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return View();
        }
        [Authorize(Roles = "SysAdmin")]
        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            try
            {

                var editUsr = await _userManager.GetUserAsync(HttpContext.User);

                editUsr.UserName = user.UserName;
                editUsr.Name = user.Name;
                editUsr.StreetNo = user.StreetNo;
                editUsr.City = user.City;
            
                editUsr.ZipCode = user.ZipCode;
                if(user.ZipCode.ToString().Length > 5 || user.ZipCode.ToString().Length < 5)
                {

                    TempData["editFailedMsg"] = "Ogiltigt postnummer!";
                    return RedirectToAction("AdminProfile");
                }

                editUsr.Email = user.Email;
                editUsr.PhoneNumber = user.PhoneNumber;
                var newUsr = await _userManager.UpdateAsync(editUsr);

                if (newUsr.Succeeded)
                {
                    TempData["editSuccessMsg"] = "Lyckad uppdatering av användare!";
                }
                else
                {
                    TempData["editFailedMsg"] = "Användarnamnet existrerar redan!";
                }

                return RedirectToAction("AdminProfile");
            }
            catch (Exception e)
            {

                _logger.LogError(e.Message);
                RedirectToAction("AdminProfile");
            }
            return View();
        }

        

        [Authorize(Roles = "SysAdmin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id, User user)
        {
            try
            {
                var usr = await _userManager.FindByIdAsync(user.Id.ToString());
                return View(usr);
            }
            catch (Exception e)
            {

                _logger.LogError(e.Message);
                RedirectToAction("GetAllUsers");
            }
            return View();
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, User user)
        {
            try
            {
                var usr = await _userManager.FindByIdAsync(user.Id.ToString());
                var del = await _userManager.DeleteAsync(usr);
                if (del.Succeeded)
                {
                    ViewBag.deleteMsg = "user deleted.";
                }
                return RedirectToAction(nameof(GetAllUsers));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                RedirectToAction("GetAllUsers");
            }
            return View();
        }


  

   
        public IActionResult AdminProfile()
        {
            try
            {
                if(TempData["feedbackMsg"] != null)
                {
                    ViewBag.Error = TempData["feedbackMsg"];
                }
                if (TempData["successMsg"] != null)
                {
                    ViewBag.Success = TempData["successMsg"];
                }
                if (TempData["editFailedMsg"] != null)
                {
                    ViewBag.EditError = TempData["editFailedMsg"];
                }
                if (TempData["editSuccessMsg"] != null)
                {
                    ViewBag.EditSuccess = TempData["editSuccessMsg"];
                }


                ViewModel v = new ViewModel();
                ViewBag.Msg = User.Identity.Name;

                var Userid = _userManager.GetUserId(HttpContext.User);



                v.user = _userManager.FindByIdAsync(Userid).Result;
                return View(v);
            }
            catch (Exception e)
            {

                _logger.LogError(e.Message);
                
            }

            return RedirectToAction("Login", "Login");



        }


        //Metod för ändring av lösenord
        [Authorize(Roles = "SysAdmin")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ViewModel viewModel)
        {
            
            try
            {

                User user = await _userManager.FindByIdAsync(viewModel.user.Id.ToString());

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, token, viewModel.newPassword);

                if (result.Succeeded)
                {

                    TempData["successMsg"] = "success";
                    return RedirectToAction("AdminProfile");                   
                }
                else
                {
                    List<string> tempList = new List<string>();
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("failedpwd", error.Description);
                        tempList.Add(error.Description);
                    }


                    TempData["feedbackMsg"] = tempList;


                }

                return RedirectToAction("AdminProfile");

            }
            catch (Exception e)
            {

                _logger.LogError(e.Message);

            }

            return RedirectToAction("AdminProfile");
        }


        public IActionResult CreateRole()
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
        public async Task<IActionResult> CreateRole(Role role)
        {
            try
            {

                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("GetAllUsers", "User");
                }
                return View(role);
            }
            catch (Exception e)
            {

                _logger.LogError(e.Message);
            }
            return View();
        }
        

    }
}
