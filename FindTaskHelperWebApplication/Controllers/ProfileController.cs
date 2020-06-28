using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FindTaskHelperWebApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace FindTaskHelperWebApplication.Controllers
{
    public class ProfileController : Controller
    {
        private ICurrentSession _CurrentSession;
        private UserController _UserController = new UserController();
        private HomeController _HomeController = new HomeController();
        private TaskController _TaskController = new TaskController();
        private EmailController _EmailControler = new EmailController();
        private NpgsqlConnection _Connection = new DatabaseConnection().GetDatabaseConnection();

        public ProfileController(ICurrentSession currentSession)
        {
            _CurrentSession = currentSession;
        }
        public IActionResult ProfileIndex(string username, string message="")
        {
            User user = _UserController.ReturnUserWithGivenUsername(username);
            ViewData["message"] = message;
            ViewData["username"] = user.Username;
            ViewData["name"] = user.Name;
            ViewData["surname"] = user.Surname;
            ViewData["email"] = user.ContactInfo.Email;
            ViewData["phone"] = user.ContactInfo.PhoneNumber;
            ViewData["address"] = user.ContactInfo.Address;
            ViewData["password"] = user.Password;
            ViewData["photoUrl"] = user.PhotoUrl;
            ViewData["rating"] = user.Rating;
            return View("Views/Profile/ProfileView.cshtml");
        }
        [HttpPost]
        public IActionResult ProcessProfileData(Profile profile)
        {
            if (profile.DeleteButton.Equals("Save"))
                return SaveNewProfileData(profile);
            else
                return DeleteUserProfile(profile);

        }
        public IActionResult SaveNewProfileData(Profile profile)
        {
            IFormFile profilePhoto = profile.Photo;
            if (profilePhoto != null)
            {
                var path = "";
                var imageName = DateTime.Now.Millisecond + profilePhoto.FileName;
                if (profilePhoto != null || profilePhoto.Length != 0)
                {
                    User currentUser = _UserController.ReturnUserWithGivenUsername(profile.UsernameField);
                    string oldPhotoPath = currentUser.PhotoUrl;
                    oldPhotoPath = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot/profilePhotos",
                                oldPhotoPath);
                    if (oldPhotoPath != null && !oldPhotoPath.Equals("") && System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                    path = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot/profilePhotos",
                                imageName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        profilePhoto.CopyTo(stream);
                    }
                }
                profile.PhotoUrlField = imageName;
            }
            else
            {
                User currentUser = _UserController.ReturnUserWithGivenUsername(profile.UsernameField);
                profile.PhotoUrlField = currentUser.PhotoUrl;
            }

            _Connection.Open();

            string sqlQueryForEmail = $"SELECT email from users WHERE username!=\'{profile.UsernameField}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlQueryForEmail, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            { 
                if (((string)reader[0]).Equals(profile.EmailField))
                {
                    return ProfileIndex(profile.UsernameField, "mail-not-unique");
                }
            }
            _Connection.Close();

            _UserController.UpdateProfileData(profile.UsernameField, profile.NameField, profile.SurnameField, profile.EmailField, profile.PhoneField,
                profile.AddressField, profile.PhotoUrlField);
            return ProfileIndex(profile.UsernameField, "success");
        }
        public IActionResult ChangePassword(Profile profile)
        {
            LoginController loginController = new LoginController(_CurrentSession);
            if (!profile.PasswordField.Equals(profile.PasswordConfirmationField))
                return ProfileIndex(profile.UsernameField, "no-match");
            int numberOfRowsAffected =loginController.UpdatePassword(profile.UsernameField, profile.PasswordField);
            if(numberOfRowsAffected == 0)
                return ProfileIndex(profile.UsernameField, "fail");
            else
                return ProfileIndex(profile.UsernameField, "success");
        }

        public IActionResult DeleteUserProfile(Profile profile)
        {
            int[] ads = _UserController.ReturnUserWithGivenUsername(profile.UsernameField).Ads;
            int[] tasks = _UserController.ReturnUserWithGivenUsername(profile.UsernameField).Tasks;
            for(int i = 0; i<ads.Length; i++)
            {
                
                if (ads[i] != -1) 
                {
                    Models.Task ad =_TaskController.ReturnTaskWithGivenTaskId(ads[i]);
                    if (ad.IsAssignedToSomeone)
                    {
                        User tasker = _UserController.ReturnUserWithGivenUsername(ad.AssignedToUserWithThisUsername);
                        _UserController.RemoveTaskFromUserWithGivenUsername(tasker.Username, ads[i]);
                        Email cancellation = new Email(tasker.ContactInfo.Email, tasker.Username, "The job is cancelled by the owner.",
                                            _EmailControler.TheJobIsCancelledMessage(ad.TaskOwnerUsername, ad.TaskID));
                        _EmailControler.SendMail(cancellation);
                    }
                    _TaskController.DeleteTaskWithGivenTaskId(ads[i]);
                }

            }
            for(int i = 0; i < tasks.Length; i++)
            {
                if(tasks[i] != -1) 
                {
                    Models.Task task = _TaskController.ReturnTaskWithGivenTaskId(tasks[i]);
                    User owner = _UserController.ReturnUserWithGivenUsername(task.TaskOwnerUsername);
                    Email cancellation = new Email(owner.ContactInfo.Email, owner.Username, "The tasker quitted the job!",
                                        _EmailControler.TaskerQuittedMessage(owner.Username, task.TaskID, task.AssignedToUserWithThisUsername));
                    _EmailControler.SendMail(cancellation);
                    _TaskController.MarkTaskWithGivenTaskIdAsActive(tasks[i]);
                }
            }
            int numberOfRowsAffected = _UserController.DeleteUserAccountWithGivenUsername(profile.UsernameField);
            if (numberOfRowsAffected == 1)
            {
                return _HomeController.Index();
            }
            else
            {
                ViewData["message"] = "An error has occured.";
                return ProfileIndex(profile.UsernameField);
            }
        }
    }
}
