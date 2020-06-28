using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindTaskHelperWebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using FindTaskHelperWebApplication.Models;
using Microsoft.AspNetCore.Http;

namespace FindTaskHelperWebApplication.Controllers.Tests
{
    [TestClass()]
    public class UpdateAddControllerTests
    {
        private User taskOwner;
        private User tasker;
        private Models.Task ad;
        [TestInitialize]
        public void InitTest()
        {
            UserController userController = new UserController();
            TaskController taskController = new TaskController();

            string username = "Mock-User-1";
            string name = "Mock";
            string surname = "User";
            string password = "123";
            User.ContactInformationAttribute contactInfo = new User.ContactInformationAttribute("erdincmutlu1972@gmail.com", "05398376436", "Rumelifeneri Mahallesi");
            userController.AddNewUser(username, password, name, surname, contactInfo);
            taskOwner = userController.ReturnUserWithGivenUsername(username);

            string usernameForTasker = "Mock-User-2";
            string nameForTasker = "Mock2";
            string surnameForTasker = "User";
            string passwordForTasker = "123";
            User.ContactInformationAttribute contactInfoForTasker = new User.ContactInformationAttribute("necla-mutlu1998@hotmail.com", "05398376436", "Rumelifeneri Mahallesi");
            userController.AddNewUser(usernameForTasker, passwordForTasker, nameForTasker, surnameForTasker, contactInfoForTasker);
            tasker = userController.ReturnUserWithGivenUsername(usernameForTasker);

            AddNewAd newAdForm = new AddNewAd();
            newAdForm.TaskOwnerUsername = username;
            newAdForm.TitleField = "Mock Task 1";
            newAdForm.DescriptionField = "Mock Test for Update ad controller";
            newAdForm.LocationField = "Bursa";
            newAdForm.AddressField = "Rumelifeneri";
            newAdForm.PaymentAmountField = "20";
            newAdForm.Category = "Mock";
            newAdForm.Currency = "TL";
            taskController.ProcessNewAdData(newAdForm);
            taskOwner = userController.ReturnUserWithGivenUsername(username);
            ad = taskController.ReturnTaskWithGivenTaskId(taskOwner.Ads[1]);
        }
        [TestCleanup]
        public void CleanCreatedUsersAndTasks()
        {
            TaskController taskController = new TaskController();
            UserController userController = new UserController();

            taskController.DeleteTaskWithGivenTaskId(ad.TaskID);
            userController.DeleteUserAccountWithGivenUsername(taskOwner.Username);
            userController.DeleteUserAccountWithGivenUsername(tasker.Username);

        }
        
        [TestMethod()]
        public void TestAssignAd()
        {
            UpdateAdd newForm = new UpdateAdd();
            newForm.TaskIdField = ad.TaskID;
            newForm.Tasker = tasker.Username;

            UpdateAddController updateAddController = new UpdateAddController();
            updateAddController.AssignAd(newForm);

            TaskController taskController = new TaskController();
            ad = taskController.ReturnTaskWithGivenTaskId(ad.TaskID);

            Assert.AreEqual(true, ad.IsAssignedToSomeone);
            Assert.AreEqual(tasker.Username, ad.AssignedToUserWithThisUsername);
        }

        [TestMethod()]
        public void RateTaskOwnerTest()
        {
            int rate = 3;
            UserController userController = new UserController();
            UpdateAdd updateAdForm = new UpdateAdd();
            updateAdForm.Rate = rate;
            updateAdForm.TaskOwner = taskOwner.Username;
            updateAdForm.Tasker = tasker.Username;

            UpdateAddController updateAddController = new UpdateAddController();
            updateAddController.RateTaskOwner(updateAdForm);
            taskOwner = userController.ReturnUserWithGivenUsername(taskOwner.Username);

            Assert.AreEqual(4, taskOwner.Rating);
        }

        [TestMethod()]
        public void RateTaskerTest()
        {
            int rate = 3;
            UserController userController = new UserController();
            UpdateAdd updateAdForm = new UpdateAdd();
            updateAdForm.Rate = rate;
            updateAdForm.Tasker = tasker.Username;
            updateAdForm.TaskOwner = taskOwner.Username;

            UpdateAddController updateAddController = new UpdateAddController();
            updateAddController.RateTasker(updateAdForm);
            tasker = userController.ReturnUserWithGivenUsername(tasker.Username);

            Assert.AreEqual(4, tasker.Rating);
        }

        [TestMethod()]
        public void QuitTaskTest()
        {
            UpdateAdd newForm = new UpdateAdd();
            TaskController taskController = new TaskController();
            newForm.TaskIdField = ad.TaskID;
            newForm.Tasker = tasker.Username;

            UpdateAddController updateAddController = new UpdateAddController();
            updateAddController.AssignAd(newForm);
            UpdateAdd quitForm = new UpdateAdd();
            quitForm.TaskIdField = ad.TaskID;
            quitForm.Tasker = tasker.Username;
            quitForm.TaskOwner = taskOwner.Username;
            updateAddController.QuitTask(quitForm);
            ad = taskController.ReturnTaskWithGivenTaskId(ad.TaskID);

            Assert.AreEqual(false, ad.IsAssignedToSomeone);
            Assert.AreEqual("", ad.AssignedToUserWithThisUsername);
            
            
        }

    }
}