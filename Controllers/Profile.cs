using ClientLogIn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private string errorMessage;
        public List<WorkShift> list = new List<WorkShift>();

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
            ViewModel viewModel = new ViewModel();

            var activeUser = await _userManager.GetUserAsync(HttpContext.User);
            var isSysAdmin = await _userManager.IsInRoleAsync(activeUser, "SysAdmin");

            if (isSysAdmin)
            {

                viewModel.user = await _userManager.FindByIdAsync(id.ToString());

            }
            else
            {
                viewModel.user = activeUser;
            }


            ViewBag.userId = 2;


            ViewBag.count = 0;


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

            //Hämtar och Sorterar WorkShift efer aktiv månad
            viewModel.WorkShiftList = _context.WorkShifts.Where(ap => ap.Date.Month == viewModel.dayData.Month).ToList();



            //Initierar och skapar select listor för formuläret

            //var shifts = _context.ShiftTypes.ToList();
            //var tasks = _context.Tasks.ToList();
            //SelectList shiftList = new SelectList(shifts, "Id", "Name");
            //SelectList taskList = new SelectList(tasks, "Id", "Name");






            //var Userid = _userManager.GetUserId(HttpContext.User);
            //if (Userid == null)
            //{
            //    RedirectToAction("Loginsidan");
            //}
            //else
            //{
            //    User _user = _userManager.FindByIdAsync(Userid).Result;
            //    return View(_user);
            //}


            //ViewBag.shiftList = shiftList;
            //ViewBag.taskList = taskList;

            return View(viewModel);
        }



        // Metod för ändnring av användarprofil
        [HttpPost]
        public async Task<IActionResult> Editemployee(User user)
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

            return RedirectToAction("Index", new { id =user.Id} );
        }





        //Metod för ändring av lösenord
        [HttpPost]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {

            User user = await _userManager.FindByIdAsync(id.ToString());

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Profile");

            } else
            {
                errorMessage = "Ändring av lösenordet misslyckades!";
            }

            return RedirectToAction("Index", "Profile");
        }



        //Metod för ändring av epost
        [HttpPost]
        public async Task<IActionResult> ResetEmail(int id,string newEmail)
        {

            var user = await _userManager.FindByIdAsync(id.ToString());

            var emailExist = await _userManager.FindByEmailAsync(newEmail);



            if(emailExist != null)
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

        //[Authorize(Roles = "SysAdmin")]
        public IActionResult Delete(int id)
        {
            WorkShift w = _context.WorkShifts.Find(id);
            _context.Remove(w);
            _context.SaveChanges();
            //list.Remove(list.Where(ws => ws.Id == id).FirstOrDefault());


            return RedirectToAction("Index");
        }


        //[Authorize(Roles = "SysAdmin")]
        [HttpPost]
        public IActionResult Add(WorkShift WorkShift)
        {
            _context.WorkShifts.Add(WorkShift);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public int DateMapper(DateTime date, ViewModel viewModel)
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

    }
}
