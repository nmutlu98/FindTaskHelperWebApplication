using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FindTaskHelperWebApplication.Models;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.EntityFrameworkCore.Internal;
using static FindTaskHelperWebApplication.Models.User;
using Microsoft.AspNetCore.Http;

namespace FindTaskHelperWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private CurrentSession _CurrentSession;
        public HomeController()
        {
            _CurrentSession = new CurrentSession();
            _CurrentSession.SetContextAccessor(new HttpContextAccessor());
        }
        
        public IActionResult Index(string location = "", string potentialTitle = "", string message="")
        {
            UserController userController = new UserController();
            TaskController taskController = new TaskController();
            List<Tuple<Models.Task, ContactInformationAttribute, double, string>> activeAdsWithAdOwnerContactInfos = taskController.GetAllActiveAds(location, potentialTitle);
            ViewData["members"] = userController.CountUsers();
            ViewData["tasks"] = taskController.CountTasks();
            ViewData["numberOfActiveTasks"] = activeAdsWithAdOwnerContactInfos.Count;
            ViewData["activeAds"] = activeAdsWithAdOwnerContactInfos;
            ViewData["message"] = message;
            return View("Views/Home/Index.cshtml");
        }
        public IActionResult IndexLoggedIn(string username, string location="", string potentialTitle="", string message="")
        {
            UserController userController = new UserController();
            TaskController taskController = new TaskController();
            
            if (username == null)
                username = _CurrentSession.GetUsername();
            User currentUser = userController.ReturnUserWithGivenUsername(username);
            List<Tuple<Models.Task, ContactInformationAttribute, double, string>> activeAdsWithAdOwnerContactInfos = taskController.ArrangeOrderOfAds(taskController.GetAllActiveAds(location, potentialTitle), currentUser.PreferredJobCategories);
            ViewData["members"] = userController.CountUsers();
            ViewData["tasks"] = taskController.CountTasks();
            ViewData["numberOfActiveTasks"] = activeAdsWithAdOwnerContactInfos.Count;
            ViewData["username"] = username;
            ViewData["activeAds"] = activeAdsWithAdOwnerContactInfos;
            ViewData["message"] = message;
            return View("Views/Home/IndexLoggedIn.cshtml");
        }
        public IActionResult Login()
        {
            LoginController loginController = new LoginController(_CurrentSession);
            return loginController.LoginIndex();
        }
        public IActionResult Register()
        {
            RegisterController registerController = new RegisterController();
            return registerController.RegisterIndex();
        }
        public IActionResult Profile()
        {
            ProfileController profileController = new ProfileController(_CurrentSession);
            string username = _CurrentSession.GetUsername();
            return profileController.ProfileIndex(username);
        }
        public IActionResult MyAds(string username, string message="")
        {
            if (username == null)
                username = _CurrentSession.GetUsername();
            TaskController taskController = new TaskController();
            return taskController.ListMyAds(username, message);
        }
        public IActionResult MyTasks(string username)
        {
            if (username == null)
                username = _CurrentSession.GetUsername();
            TaskController taskController = new TaskController();
            return taskController.ListMyTasks(username);
        }
        public IActionResult NewAd()
        {
            TaskController taskController = new TaskController();
           string username = _CurrentSession.GetUsername();
            return taskController.AddNewAd(username);
        }
        public IActionResult Privacy()
        {
            ViewData["username"] = _CurrentSession.GetUsername();
            return View("/Views/Privacy/Privacy.cshtml");
        }
        public IActionResult TermsOfService()
        {
            ViewData["username"] = _CurrentSession.GetUsername();
            return View("/Views/TermsOfService/termsOfServices.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
