using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindTaskHelperWebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using FindTaskHelperWebApplication.Models;

namespace FindTaskHelperWebApplication.Controllers.Tests
{
    [TestClass]
    public class HomeControllerTests
    {
        private NpgsqlConnection _Connection = new DatabaseConnection().GetDatabaseConnection();
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
            userController.AddNewUser(username, password, name, surname, contactInfo);
            user = userController.ReturnUserWithGivenUsername(username);
        }
        [TestCleanup]
        public void CleanMockUserAndTasks()
        {
            UserController userController = new UserController();
            userController.DeleteUserAccountWithGivenUsername(user.Username);
        }
        [TestMethod]
        public void TestIndexView()
        {
            var homeController = new HomeController();
            var result = homeController.Index() as ViewResult;
            Assert.AreEqual("Views/Home/Index.cshtml", result.ViewName);
        }
        [TestMethod]
        public void TestIndexViewData()
        {
            _Connection.Open();

            //test ViewData["members"]
            string queryForUsers = "SELECT COUNT(*) from users";
            NpgsqlCommand commandForUsers = new NpgsqlCommand(queryForUsers, _Connection);
            NpgsqlDataReader readerForUsers = commandForUsers.ExecuteReader();
            if (readerForUsers.Read())
            {
                long expectedNumberOfMembers = (System.Int64)readerForUsers[0];
                _Connection.Close();
                var homeController = new HomeController();
                var result = homeController.Index() as ViewResult;
                var viewData = result.ViewData;
                Assert.AreEqual(expectedNumberOfMembers, viewData["members"]);
            }

            //test ViewData["tasks"]
            _Connection.Open();
            string queryForTasks = "SELECT COUNT(*) from tasks";
            NpgsqlCommand commandForTasks = new NpgsqlCommand(queryForTasks, _Connection);
            NpgsqlDataReader readerForTasks = commandForTasks.ExecuteReader();
            if (readerForTasks.Read())
            {
                long expectedNumberOfTasks = (System.Int64)readerForTasks[0];
                _Connection.Close();
                var homeController = new HomeController();
                var result = homeController.Index() as ViewResult;
                var viewData = result.ViewData;
                Assert.AreEqual(expectedNumberOfTasks, viewData["tasks"]);
            }

            //test ViewData["numberOfActiveTasks"]
            _Connection.Open();

            string queryForActives = "SELECT COUNT(*) from tasks WHERE isassignedtosomeone=\'false\'";
            NpgsqlCommand commandForActives = new NpgsqlCommand(queryForActives, _Connection);
            NpgsqlDataReader readerForActives = commandForActives.ExecuteReader();
            if (readerForActives.Read())
            {
                long expectedNumberOfActives = (System.Int64)readerForActives[0];
                _Connection.Close();
                var homeController = new HomeController();
                var result = homeController.Index() as ViewResult;
                var viewData = result.ViewData;
                Assert.AreEqual((int)expectedNumberOfActives, (int)viewData["numberOfActiveTasks"]);
            }

        }
        
        [TestMethod]
        public void TestIndexLoggedIn()
        {
            var homeController = new HomeController();
            var result = homeController.IndexLoggedIn(user.Username) as ViewResult;
            Assert.AreEqual("Views/Home/IndexLoggedIn.cshtml", result.ViewName);
        }
        [TestMethod]
        public void TestIndexLoggedInViewData()
        {
            var homeController = new HomeController();
            var result = homeController.IndexLoggedIn(user.Username) as ViewResult;
            var viewData = result.ViewData;
            _Connection.Open();

            //test ViewData["members"]
            string queryForUsers = "SELECT COUNT(*) from users";
            NpgsqlCommand commandForUsers = new NpgsqlCommand(queryForUsers, _Connection);
            NpgsqlDataReader reader = commandForUsers.ExecuteReader();
            if (reader.Read())
            {
                long expectedNumberOfMembers = (System.Int64)reader[0];
                _Connection.Close();
                Assert.AreEqual(expectedNumberOfMembers, viewData["members"]);
            }

            //test ViewData["tasks"]
            _Connection.Open();
            string queryForTasks = "SELECT COUNT(*) from tasks";
            NpgsqlCommand commandForTasks = new NpgsqlCommand(queryForTasks, _Connection);
            NpgsqlDataReader readerForTasks = commandForTasks.ExecuteReader();
            if (readerForTasks.Read())
            {
                long expectedNumberOfMembers = (System.Int64)reader[0];
                _Connection.Close();
                Assert.AreEqual(expectedNumberOfMembers, viewData["tasks"]);
            }

            //test ViewData["numberOfActiveTasks"]
            _Connection.Open();

            string queryForActives = "SELECT COUNT(*) from tasks WHERE isassignedtosomeone=\'false\'";
            NpgsqlCommand commandForActives = new NpgsqlCommand(queryForActives, _Connection);
            NpgsqlDataReader readerForActives = commandForActives.ExecuteReader();
            if (readerForActives.Read())
            {
                long expectedNumberOfActives = (System.Int64)readerForActives[0];
                _Connection.Close();
                if(expectedNumberOfActives == 0)
                {
                    Assert.AreEqual(0, viewData["numberOfActiveTasks"]);
                }
                else
                {
                    Assert.AreEqual((int)expectedNumberOfActives, (int)viewData["numberOfActiveTasks"]);
                }
                    
                
            }

            //test ViewData["username"]
            Assert.AreEqual(user.Username, viewData["username"]);
        }

        [TestMethod]
        public void TestLogin()
        {
            var homeController = new HomeController();
            var result = homeController.Login() as ViewResult;
            Assert.AreEqual("Views/Login/Login.cshtml", result.ViewName);
        }
        [TestMethod]
        public void TestRegister()
        {
            var homeController = new HomeController();
            var result = homeController.Register() as ViewResult;
            Assert.AreEqual("Views/Register/RegisterView.cshtml", result.ViewName);
        }
        public void TestProfile()
        {
            var homeController = new HomeController();
            var result = homeController.Profile() as ViewResult;
            Assert.AreEqual("Views/Profile/ProfileView.cshtml", result.ViewName);
        }
    }
}