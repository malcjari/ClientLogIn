﻿using ClientLogIn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace ClientLogIn.Controllers
{

    
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
        
        public async Task<IActionResult> IndexAsync(DateTime iDate, int id)
        {



            ViewModel viewModel = new ViewModel();
            //Skapar alla WorkShift och lägger i viewModelListan

            InitWorkShift(list);
            viewModel.WorkShiftList = list;


            //viewModel.WorkShift.UserId = Int32.Parse(_userManager.GetUserId(HttpContext.User));
            //viewModel.user = await _userManager.FindByIdAsync(id.ToString());

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
                //Om inget datum har valts, körs denna med dagens datum. Detta är default

                //Slänger in valt datum i DateMapper som plockar ut relevanta värden till viewModellen
                DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                viewModel.dayData.FullDate = date;
                viewModel.dayData.FirstDayOfWeek = DateMapper(date, viewModel);
            }

            //Sorterar WorkShift efer aktiv månad
            viewModel.WorkShiftList = viewModel.WorkShiftList.Where(ap => ap.Date.Month == viewModel.dayData.Month).ToList();



            //Initierar och skapar select listor för formuläret
            Dictionary<int, string> shiftDict = InitShiftList();
            SelectList shiftList = new SelectList(shiftDict, "Key", "Value");
            Dictionary<int, string> taskDict = InitTaskList();
            SelectList taskList = new SelectList(taskDict, "Key", "Value");



            User u = new User();

            u.Name = "wasim";
            u.StreetNo = "Siriusgatan 199";
            u.City = "gotham";
            u.ZipCode = 41525;
            u.PhoneNumber = "070666049494";
            u.Email = "Wasim_ajaja@homtial.cn";
            viewModel.user = u;



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


            ViewBag.shiftList = shiftList;
            ViewBag.taskList = taskList;

            return View(viewModel);
        }
        // Metod för ändnring av användarprofil
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

            return View();
        }


        public IActionResult Delete(int id)
        {
            //list.Remove(list.Where(ws => ws.Id == id).FirstOrDefault());

            
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Add(WorkShift WorkShift)
        {

            //_context.WorkShifts.Add(WorkShift)
            //_context.SaveChanges();
            return RedirectToAction("Index");
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
                case "Sundday":
                    number = 6;
                    break;
            }

            return number;
        }

        public List<WorkShift> WorkShiftFromMonth(int month, List<WorkShift> list)
        {


            return list;
        }

        public void InitWorkShift(List<WorkShift> list)
        {
            DateTime date1 = new DateTime(2020, 2, 1);
            DateTime date2 = new DateTime(2020, 2, 02);
            DateTime date3 = new DateTime(2020, 2, 07);
            DateTime date4 = new DateTime(2020, 2, 06);
            DateTime date5 = new DateTime(2020, 2, 22);
            DateTime date6 = new DateTime(2020, 2, 10);



            WorkShift arb1 = new WorkShift(1, date1, 1, "Reception");
            WorkShift arb2 = new WorkShift(2, date2, 1, "Cleaning");
            WorkShift arb3 = new WorkShift(3, date3, 2, "Reception");
            WorkShift arb4 = new WorkShift(4, date4, 2, "Preperations");
            WorkShift arb5 = new WorkShift(5, date5, 3, "Preperations");
            WorkShift arb6 = new WorkShift(6, date6, 3, "Guard-duty");
            WorkShift arb7 = new WorkShift(7, date1, 2, "Guard-duty");

            DateTime date11 = new DateTime(2020, 3, 1);
            DateTime date12 = new DateTime(2020, 3, 03);
            DateTime date13 = new DateTime(2020, 3, 12);
            DateTime date14 = new DateTime(2020, 3, 01);
            DateTime date15 = new DateTime(2020, 3, 01);
            DateTime date16 = new DateTime(2020, 3, 24);


            WorkShift arb11 = new WorkShift(11, date11, 1, "Reception");
            WorkShift arb12 = new WorkShift(12, date12, 1, "Cleaning");
            WorkShift arb13 = new WorkShift(13, date13, 2, "Reception");
            WorkShift arb14 = new WorkShift(14, date14, 2, "Preperations");
            WorkShift arb15 = new WorkShift(15, date15, 3, "Preperations");
            WorkShift arb16 = new WorkShift(16, date16, 3, "Guard-duty");
            WorkShift arb17 = new WorkShift(17, date16, 2, "Guard-duty");

            list.Add(arb1);
            list.Add(arb2);
            list.Add(arb3);
            list.Add(arb4);
            list.Add(arb5);
            list.Add(arb6);
            list.Add(arb7);
            list.Add(arb11);
            list.Add(arb12);
            list.Add(arb13);
            list.Add(arb14);
            list.Add(arb15);
            list.Add(arb16);
            list.Add(arb17);
        }

        public Dictionary<int, string> InitShiftList()
        {
            Dictionary<int, string> returnList = new Dictionary<int, string>();

            returnList.Add(1, "Day");
            returnList.Add(2, "Evening");
            returnList.Add(3, "Night");

            return returnList;
        }

        public Dictionary<int, string> InitTaskList()
        {
            Dictionary<int, string> returnList = new Dictionary<int, string>();

            returnList.Add(1, "Reception");
            returnList.Add(2, "Cleaning");

            return returnList;
        }
    }
}
