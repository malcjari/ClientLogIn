using ClientLogIn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ClientLogIn.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly MyContext _context;

        public HomeController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            MyContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }



        public ActionResult Index()
        {
            //User u = new User();

            //u.Name = "wasim";
            //u.StreetNo = "Siriusgatan 199";
            //u.City = "gotham";
            //u.ZipCode = 41525;
            //u.PhoneNumber = "070666049494";
            //u.Email = "Wasim_ajaja@homtial.cn";

            var Userid = _userManager.GetUserId(HttpContext.User);
            if (Userid == null)
            {
                RedirectToAction("Loginsidan");
            }
            else
            {
                User _user = _userManager.FindByIdAsync(Userid).Result;
                return View(_user);
            }
            return View();
        }

        public async Task<IActionResult> EditemployeeAsync(int id)
        {
            User user = await _userManager.FindByNameAsync(id.ToString());
            if (user != null) 
            {
                return View(user);
            }
            else
            {
                RedirectToAction("Index");
            }
            return View();
        }[HttpPost]

        public async Task<IActionResult> Editemployee (User user)
        {
            User _user = new User();
            _user = await _userManager.FindByIdAsync(user.Id.ToString());
            if (_user!=null)
            {
                _user.Name = user.Name;
                _user.StreetNo = user.StreetNo;
                _user.City = user.City;
                _user.ZipCode = user.ZipCode;
                _user.PhoneNumber = user.PhoneNumber;
                _user.Email = user.Email;

                var resultat = await _userManager.UpdateAsync(_user);
                
                if (resultat.Succeeded)
                {
                    RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in resultat.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            
            }
            
            

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            
        }
    }
}
//var user = await _userManager.FindByNameAsync(Id.ToString());
//var Anvandare = _userManager.FindByIdAsync(Id.ToString());
////var anvandare = await _userManager.FindByIdAsync(Id.ToString());
//return View(anvandare);
//var anvandare = await _context.Users.FirstOrDefaultAsync(m => m.Id == Id);
//Anvandare =   _context.Users.Where(s => s.Id == Id).ToList()