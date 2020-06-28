using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindTaskHelperWebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using FindTaskHelperWebApplication.Models;
using System.Linq;
using Newtonsoft.Json.Bson;
using Microsoft.AspNetCore.Routing.Constraints;

namespace FindTaskHelperWebApplication.Controllers.Tests
{
    [TestClass()]
    public class TaskControllerTests
    {
        private NpgsqlConnection _Connection = (new DatabaseConnection()).GetDatabaseConnection();
        private User user;
        private User mockTasker;
        private AddNewAd newAdForm;
        private int mockTaskID;
        
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
            userController.AddNewUser(username, password, name, surname, contactInfo);
            user = userController.ReturnUserWithGivenUsername(username);

            newAdForm = new AddNewAd();
            newAdForm.TaskOwnerUsername = user.Username;
            newAdForm.TitleField = "Mock Title";
            newAdForm.DescriptionField = "Mock Description";
            newAdForm.LocationField = "Bursa";
            newAdForm.AddressField = "Mock Address";
            newAdForm.Currency = "TL";
            newAdForm.Category = "Other";
            newAdForm.OtherCategory = "Mock";
            newAdForm.PaymentAmountField = "20";

            string usernameForTasker = "Mock-User-2";
            string passwordForTasker = "123";
            string nameForTasker = "Mock2";
            string surnameForTasker = "User";
            string emailForTasker = "mock2@gmail.com";
            string addressForTasker = "Mock Address";
            string phoneNumberForTasker = "123456789";
            Models.User.ContactInformationAttribute contactInfoForTasker = new Models.User.ContactInformationAttribute(emailForTasker, phoneNumberForTasker, addressForTasker);

            userController.AddNewUser(usernameForTasker, passwordForTasker, nameForTasker, surnameForTasker, contactInfoForTasker);
            mockTasker = userController.ReturnUserWithGivenUsername(usernameForTasker);
            
        }
        [TestCleanup]
        public void CleanMockUserAndTasks()
        {
            UserController userController = new UserController();
            userController.DeleteUserAccountWithGivenUsername(user.Username);
            userController.DeleteUserAccountWithGivenUsername(mockTasker.Username);
            
            _Connection.Open();
            string sql = "DELETE FROM tasks WHERE jobcategory='Mock'";
            NpgsqlCommand command = new NpgsqlCommand(sql, _Connection);
            command.ExecuteNonQuery();
            _Connection.Close();

        }
        [TestMethod]
        public void TestAddNewAd()
        {
            TaskController taskController = new TaskController();
            var result = taskController.AddNewAd(user.Username) as ViewResult;
            var viewData = result.ViewData;

            Assert.AreEqual("Views/Task/AddNewAdView.cshtml", result.ViewName);
            Assert.AreEqual(user.Username, viewData["username"]);

        }
        [TestMethod]
        public void TestProcessNewAdData()
        {
            TaskController taskController = new TaskController();
            taskController.ProcessNewAdData(newAdForm);

            UserController userController = new UserController();
            user = userController.ReturnUserWithGivenUsername(user.Username);
            if (1 < user.Ads.Length)
            {
                Assert.IsNotNull(user.Ads[1]);
                mockTaskID = user.Ads[1];
            }
            else
                Assert.Fail();


        }
        [TestMethod]
        public void TestListMyAds()
        {
            TaskController taskController = new TaskController();
            var result = taskController.ListMyAds(user.Username) as ViewResult;
            Models.Task[] viewDataAds = (Models.Task[])result.ViewData["ads"];

            for(int i = 1; i<user.Ads.Length; i++)
            {
                Assert.AreEqual(user.Ads[i], viewDataAds[i-1].TaskID);
            }
            Assert.AreEqual(result.ViewName, "Views/Task/MyAdsView.cshtml");
            Assert.AreEqual(user.Username, result.ViewData["username"]);
            
        }
        [TestMethod]
        public void TestAssignTaskToUserWithGivenUsername()
        {
            UserController userController = new UserController();
            TaskController taskController = new TaskController();

            taskController.ProcessNewAdData(newAdForm);
            user = userController.ReturnUserWithGivenUsername(user.Username);
            if (user.Ads.Length <= 1)
                Assert.Fail();
            else
            {
                int mockTaskID = user.Ads[1];
                taskController.AssignTaskToUserWithGivenUsername(mockTaskID, mockTasker.Username);
                mockTasker = userController.ReturnUserWithGivenUsername(mockTasker.Username);
                Models.Task mockTask = taskController.ReturnTaskWithGivenTaskId(mockTaskID);
                Assert.AreEqual(true, mockTask.IsAssignedToSomeone);
                Assert.AreEqual(mockTasker.Username, mockTask.AssignedToUserWithThisUsername);
            }
        }
        [TestMethod]
        public void TestListMyTasks()
        {
            TaskController taskController = new TaskController();
            
            var result = taskController.ListMyTasks(user.Username) as ViewResult;
            Models.Task[] viewDataTasks = (Models.Task[])result.ViewData["tasks"];
            for (int i = 1; i < user.Tasks.Length; i++)
            {
                Assert.AreEqual(user.Tasks[i], viewDataTasks[i - 1].TaskID);
            }

            Assert.AreEqual(result.ViewName, "Views/Task/MyTasks.cshtml");
            Assert.AreEqual(user.Username, result.ViewData["username"]);

        }
        [TestMethod]
        public void TestReturnTaskWithGivenId()
        {
            UserController userController = new UserController();
            TaskController taskController = new TaskController();
            taskController.ProcessNewAdData(newAdForm);
            user = userController.ReturnUserWithGivenUsername(user.Username);
            mockTaskID = user.Ads[1];
            Models.Task task = taskController.ReturnTaskWithGivenTaskId(mockTaskID);
            Assert.AreEqual(user.Username, task.TaskOwnerUsername);
            Assert.AreEqual(newAdForm.TitleField, task.Title);
        }
        [TestMethod]
        public void TestGetAllActiveAdsWithoutAnyParameter()
        {
            TaskController taskController = new TaskController();
            List<Tuple<Models.Task, Models.User.ContactInformationAttribute, double, string>> activeAds = taskController.GetAllActiveAds();

            _Connection.Open();
            string sqlQuery = "SELECT COUNT(*) FROM tasks WHERE isassignedtosomeone=\'false\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlQuery, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                if (activeAds.Count == 0)
                    Assert.AreEqual(reader[0],(System.Int64) 0);
                else
                    Assert.AreEqual((System.Int64)reader[0], (System.Int64)activeAds.Count);
            }
            _Connection.Close();
        }
        [TestMethod]
        public void TestGetAllActiveAdsWithLocation()
        {
            string location = "İstanbul";

            TaskController taskController = new TaskController();
            int numberOfActivesInIstanbul = taskController.GetAllActiveAds(location: location).Count;

            _Connection.Open();
            string query = $"SELECT COUNT(*) FROM tasks WHERE location=\'{location}\' AND isassignedtosomeone=\'false\'";
            NpgsqlCommand command = new NpgsqlCommand(query, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Assert.AreEqual((System.Int64)reader[0], (System.Int64)numberOfActivesInIstanbul);
            }
            _Connection.Close();
        }
        [TestMethod]
        public void TestGetAllActiveAdsWithTitle()
        {
            string titlePart = "dog";

            TaskController taskController = new TaskController();
            int numberOfActivesWithDogInTitle = taskController.GetAllActiveAds(title: titlePart).Count;

            _Connection.Open();
            string query = $"SELECT COUNT(*) FROM tasks WHERE LOWER(Title) LIKE '%{titlePart}%' AND isassignedtosomeone=\'false\'";
            NpgsqlCommand command = new NpgsqlCommand(query, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Assert.AreEqual((System.Int64)reader[0], (System.Int64)numberOfActivesWithDogInTitle);
            }
            _Connection.Close();
        }
        [TestMethod]
        public void TestGetAllActiveAdsWithTitleAndLocation()
        {
            string titlePart = "dog";
            string location = "İstanbul";

            TaskController taskController = new TaskController();
            int numberOfActives = taskController.GetAllActiveAds(location: location, title: titlePart).Count;

            _Connection.Open();
            string query = $"SELECT COUNT(*) FROM tasks WHERE LOWER(Title) LIKE '%{titlePart}%' AND location='{location}' AND isassignedtosomeone=\'false\'";
            NpgsqlCommand command = new NpgsqlCommand(query, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Assert.AreEqual((System.Int64)reader[0], (System.Int64)numberOfActives);
            }
            _Connection.Close();
        }
        [TestMethod]
        public void TestUpdateTaskWithGivenTaskId()
        {
            TaskController taskController = new TaskController();
            UserController userController = new UserController();
            taskController.ProcessNewAdData(newAdForm);
            user = userController.ReturnUserWithGivenUsername(user.Username);
            mockTaskID = user.Ads[1];
            Models.Task task = taskController.ReturnTaskWithGivenTaskId(mockTaskID);
            task.Title = "Mock change";
            int numberOfRows = taskController.UpdateTaskWithGivenTaskId(task);
            Assert.AreEqual(1, numberOfRows);
        }
        [TestMethod]
        public void DeleteTaskWithGivenId()
        {
            TaskController taskController = new TaskController();
            UserController userController = new UserController();
            taskController.ProcessNewAdData(newAdForm);
            user = userController.ReturnUserWithGivenUsername(user.Username);
            mockTaskID = user.Ads[1];
            int numberOfRows = taskController.DeleteTaskWithGivenTaskId(mockTaskID);
            Assert.AreEqual(1, numberOfRows);
        }
    }
}