using ClientLogIn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ClientLogIn.Controllers
{
    [Authorize]
    public class Profile : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly MyContext _context;
        public List<WorkShift> list = new List<WorkShift>();
        private readonly ILogger<Profile> ilogger;


        public Profile(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            MyContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime iDate, int id)
        {
            try
            {
                ViewModel viewModel = new ViewModel();

                //Om ett datum skickas med i metoden körs detta
                if (iDate.Year != 0001)
                {
                    //Slänger in valt datum i DateMapper som plockar ut relevanta värden till viewModellen
                    viewModel.dayData.FirstDayOfWeek = DateMapper(iDate, viewModel);
                    viewModel.dayData.FullDate = iDate;


                }
                else
                {
                    //Om inget datum har valts, körs denna med dagens datum.Detta är default

                    //Slänger in valt datum i DateMapper som plockar ut relevanta värden till viewModellen
                    DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    viewModel.dayData.FullDate = date;
                    viewModel.dayData.FirstDayOfWeek = DateMapper(date, viewModel);
                }

                var activeUser = await _userManager.GetUserAsync(HttpContext.User);
                var isSysAdmin = await _userManager.IsInRoleAsync(activeUser, "SysAdmin");

                if (isSysAdmin)
                {

                    viewModel.user = await _userManager.FindByIdAsync(id.ToString());

                    viewModel.WorkShiftList = _context.WorkShifts.Where(ap => ap.Date.Month == viewModel.dayData.Month).ToList();

                    viewModel.WorkShiftList = viewModel.WorkShiftList.Where(ap => ap.UserId == viewModel.user.Id).ToList();
                }
                else
                {

                    viewModel.user = activeUser;

                    //Hämtar och Sorterar WorkShift efer aktiv månad
                    viewModel.WorkShiftList = _context.WorkShifts.Where(ap => ap.Date.Month == viewModel.dayData.Month).ToList();

                    viewModel.WorkShiftList = viewModel.WorkShiftList.Where(ap => ap.UserId == activeUser.Id).ToList();
                }


                //ViewBag.userId = viewModel.user.Id;

                ViewBag.userId = viewModel.user.Id;
                ViewBag.count = 0;


                //Initierar och skapar select listor för formuläret

                var shifts = _context.ShiftTypes.ToList();
                var tasks = _context.Tasks.ToList();
                SelectList shiftList = new SelectList(shifts, "Id", "Name");
                SelectList taskList = new SelectList(tasks, "Id", "Name");




                ViewBag.shiftList = shiftList;
                ViewBag.taskList = taskList;

                return View(viewModel);

            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }


        // Metod för ändnring av användarprofil
        [HttpPost]
        public async Task<IActionResult> Editemployee(User user)
        {
            try
            {
                User _user = new User();
                _user = await _userManager.FindByIdAsync(user.Id.ToString());
                if (_user != null)
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

                return RedirectToAction("Index", new { id = user.Id });


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();

        }



        //Metod för ändring av lösenord
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

                    return RedirectToAction("Index", "Profile");


                }
                else
                {

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("failedpwd", error.Description);
                    }

                    ViewBag.changePwdFailed = "Password Successfully NOT Changed";



                    ModelState.AddModelError("success", "Password Successfully NOT Changed");
                }

                return RedirectToAction("Index", "Profile", new { id = user.Id });

            }
            catch (Exception e)
            {

                Console.WriteLine(e);

            }
            return View();
        }


        //Metod för ändring av epost
        [HttpPost]
        public async Task<IActionResult> ResetEmail(int id, string newEmail)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                var emailExist = await _userManager.FindByEmailAsync(newEmail);



                if (emailExist != null)
                {
                    return RedirectToAction("Index", "Profile");
                }

                var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

                var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Profile");
                }

                return RedirectToAction("Index", "Profile");

            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }
        [Authorize(Roles = "SysAdmin")]
        public IActionResult Delete(int id)
        {
            try
            {
                WorkShift w = _context.WorkShifts.Find(id);
                _context.Remove(w);
                _context.SaveChanges();



                return RedirectToAction("Index", new { id = w.UserId });
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }


        [Authorize(Roles = "SysAdmin")]
        [HttpPost]
        public IActionResult Add(WorkShift WorkShift)
        {
            try
            {
                _context.WorkShifts.Add(WorkShift);
                _context.SaveChanges();
                return RedirectToAction("Index", new { id = WorkShift.UserId });
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }

        public int DateMapper(DateTime date, ViewModel viewModel)
        {
            try
            {
                //Hämtar månadens första dag
                string day = date.DayOfWeek.ToString();
                //Hämtar antalet dagar i månaden
                viewModel.dayData.Days = DateTime.DaysInMonth(date.Year, date.Month);
                //Hämtar månadens namn i en sträng för användning i kalenderns header
                viewModel.dayData.MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(date.Month).ToUpper();
                //Hämtar månadens nr för att sortera WorkShift efter detta numret och visa rätt pass för rätt månad
                viewModel.dayData.Month = date.Month;
                DateTime todaysDate = DateTime.Now;
                viewModel.dayData.TodaysDate = todaysDate.Day;


                int number = 0;


                //Gör om månadens första dag till ett värde som gör att kalenderna synkas rätt och startar på rätt ställe
                switch (day)
                {
                    case "Monday":
                        number = 0;
                        break;
                    case "Tuesday":
                        number = 1;
                        break;
                    case "Wenesday":
                        number = 2;
                        break;
                    case "Thursday":
                        number = 3;
                        break;
                    case "Friday":
                        number = 4;
                        break;
                    case "Saturday":
                        number = 5;
                        break;
                    case "Sunday":
                        number = 6;
                        break;
                }

                return number;
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
               
            }
            return View();
        }
    
    }
}
