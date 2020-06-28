using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindTaskHelperWebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using FindTaskHelperWebApplication.Models;
using System.Security.Cryptography;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;

namespace FindTaskHelperWebApplication.Controllers.Tests
{
    [TestClass()]
    public class ProfileControllerTests
    {
        private User user;
        [TestInitialize]
        public void InitTest()
        {
            string username = "Mock-User-1";
            string password = "123";
            string name = "Mock";
            string surname = "User";
            string email = "mock@gmail.com";
            string address = "Mock Address";
            string phoneNumber = "123456789";
            Models.User.ContactInformationAttribute contactInfo = new Models.User.ContactInformationAttribute(email, phoneNumber, address);

            UserController userController = new UserController();
            userController.AddNewUser(username, Encrypt(password), name, surname, contactInfo);
            user = userController.ReturnUserWithGivenUsername(username);

        }
        [TestCleanup]
        public void CleanMockUser()
        {
            UserController userController = new UserController();
            userController.DeleteUserAccountWithGivenUsername(user.Username);
        }
        [TestMethod()]
        public void ProfileIndexTest()
        {
            var mockCurrentSession = new Mock<ICurrentSession>();
            mockCurrentSession.Setup(x => x.SetUsername(user.Username)).Returns(true);
            ProfileController profileController = new ProfileController(mockCurrentSession.Object);
            var result = profileController.ProfileIndex(user.Username) as ViewResult;
            Assert.AreEqual("Views/Profile/ProfileView.cshtml", result.ViewName);
            Assert.AreEqual(user.Name, result.ViewData["name"]);
            Assert.AreEqual(user.Surname, result.ViewData["surname"]);
            Assert.AreEqual(user.ContactInfo.Email, result.ViewData["email"]);
            Assert.AreEqual(user.ContactInfo.PhoneNumber, result.ViewData["phone"]);
            Assert.AreEqual(user.ContactInfo.Address, result.ViewData["address"]);

        }

        [TestMethod()]
        public void ProcessProfileDataTest()
        {
            Profile profile = new Profile();
            profile.UsernameField = user.Username;
            profile.NameField = user.Name;
            profile.SurnameField = user.Surname;
            profile.EmailField = user.ContactInfo.Email;
            profile.AddressField = user.ContactInfo.Address;
            profile.PhoneField = user.ContactInfo.PhoneNumber;
            profile.DeleteButton = "Save";

            var mockCurrentSession = new Mock<ICurrentSession>();
            mockCurrentSession.Setup(x => x.SetUsername(user.Username)).Returns(true);
            ProfileController profileController = new ProfileController(mockCurrentSession.Object);
            var result = profileController.ProcessProfileData(profile) as ViewResult;
            Assert.AreEqual("Views/Profile/ProfileView.cshtml", result.ViewName);

            profile.DeleteButton = "Delete";
            result = profileController.ProcessProfileData(profile) as ViewResult;
            Assert.AreEqual("Views/Home/Index.cshtml", result.ViewName);

        }

        [TestMethod()]
        public void DeleteUserProfileTestWhenUserHasAnAdTakenByATasker()
        {
            AddNewAd newAdForm = new AddNewAd();
            newAdForm.TaskOwnerUsername = user.Username;
            newAdForm.TitleField = "Mock Title";
            newAdForm.DescriptionField = "Mock Description";
            newAdForm.LocationField = "Bursa";
            newAdForm.AddressField = "Mock Address";
            newAdForm.Currency = "TL";
            newAdForm.Category = "Grocery Shopping";
            newAdForm.PaymentAmountField = "20";
            TaskController taskController = new TaskController();
            UserController userController = new UserController();
            taskController.ProcessNewAdData(newAdForm);
            user = userController.ReturnUserWithGivenUsername(newAdForm.TaskOwnerUsername);

            string usernameForTasker = "Mock-User-2";
            string passwordForTasker = "123";
            string nameForTasker = "Mock2";
            string surnameForTasker = "User";
            string emailForTasker = "mock2@gmail.com";
            string addressForTasker = "Mock Address";
            string phoneNumberForTasker = "123456789";
            Models.User.ContactInformationAttribute contactInfoForTasker = new Models.User.ContactInformationAttribute(emailForTasker, phoneNumberForTasker, addressForTasker);
            userController.AddNewUser(usernameForTasker, passwordForTasker, nameForTasker, surnameForTasker, contactInfoForTasker);
            User tasker = userController.ReturnUserWithGivenUsername(usernameForTasker);

            UpdateAdd assignTaskForm = new UpdateAdd();
            assignTaskForm.TaskIdField = user.Ads[1];
            assignTaskForm.Tasker = usernameForTasker;

            var mockCurrentSession = new Mock<ICurrentSession>();
            mockCurrentSession.Setup(x => x.SetUsername(user.Username)).Returns(true);
            ProfileController profileController = new ProfileController(mockCurrentSession.Object);
            Profile userProfile = new Profile();
            userProfile.UsernameField = user.Username;
            profileController.DeleteUserProfile(userProfile);
            tasker = userController.ReturnUserWithGivenUsername(usernameForTasker);

            Assert.AreEqual(null, taskController.ReturnTaskWithGivenTaskId(assignTaskForm.TaskIdField));
            Assert.AreEqual(false, tasker.Tasks.Contains(assignTaskForm.TaskIdField));

            userController.DeleteUserAccountWithGivenUsername(usernameForTasker);

        
        }
        [TestMethod()]
        public void DeleteUserProfileTestWhenUserIsATaskerWithActiveTasks()
        {
            AddNewAd newAdForm = new AddNewAd();
            newAdForm.TaskOwnerUsername = user.Username;
            newAdForm.TitleField = "Mock Title";
            newAdForm.DescriptionField = "Mock Description";
            newAdForm.LocationField = "Bursa";
            newAdForm.AddressField = "Mock Address";
            newAdForm.Currency = "TL";
            newAdForm.Category = "Grocery Shopping";
            newAdForm.PaymentAmountField = "20";
            TaskController taskController = new TaskController();
            UserController userController = new UserController();
            taskController.ProcessNewAdData(newAdForm);
            user = userController.ReturnUserWithGivenUsername(newAdForm.TaskOwnerUsername);

            string usernameForTasker = "Mock-User-2";
            string passwordForTasker = "123";
            string nameForTasker = "Mock2";
            string surnameForTasker = "User";
            string emailForTasker = "mock2@gmail.com";
            string addressForTasker = "Mock Address";
            string phoneNumberForTasker = "123456789";
            Models.User.ContactInformationAttribute contactInfoForTasker = new Models.User.ContactInformationAttribute(emailForTasker, phoneNumberForTasker, addressForTasker);
            userController.AddNewUser(usernameForTasker, passwordForTasker, nameForTasker, surnameForTasker, contactInfoForTasker);
            User tasker = userController.ReturnUserWithGivenUsername(usernameForTasker);

            UpdateAdd assignTaskForm = new UpdateAdd();
            assignTaskForm.TaskIdField = user.Ads[1];
            assignTaskForm.Tasker = usernameForTasker;

            var mockCurrentSession = new Mock<ICurrentSession>();
            mockCurrentSession.Setup(x => x.SetUsername(user.Username)).Returns(true);
            ProfileController profileController = new ProfileController(mockCurrentSession.Object);
            Profile userProfile = new Profile();
            userProfile.UsernameField = usernameForTasker;
            profileController.DeleteUserProfile(userProfile);
            user = userController.ReturnUserWithGivenUsername(user.Username);

            Models.Task task = taskController.ReturnTaskWithGivenTaskId(assignTaskForm.TaskIdField);
            Assert.AreNotEqual(null, task);
            Assert.AreEqual(false, task.IsAssignedToSomeone);
            Assert.AreEqual("", task.AssignedToUserWithThisUsername);
            Assert.AreEqual(true, user.Ads.Contains(assignTaskForm.TaskIdField));
            
            taskController.DeleteTaskWithGivenTaskId(assignTaskForm.TaskIdField);
        }
        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

    }
}