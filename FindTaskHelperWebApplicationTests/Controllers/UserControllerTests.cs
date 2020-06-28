using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindTaskHelperWebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using FindTaskHelperWebApplication.Models;
using Npgsql;
using System.Linq;

namespace FindTaskHelperWebApplication.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests
    {
        private NpgsqlConnection _Connection = (new DatabaseConnection()).GetDatabaseConnection();
        private string _Username;
        private string _Password;
        private string _Name;
        private string _Surname;
        private string _Email;
        private string _Address;
        private string _PhoneNumber;
        private FindTaskHelperWebApplication.Models.User.ContactInformationAttribute _ContactInfo;

        [TestInitialize]
        public void InitTest()
        {
            _Username = "Mock-User-1";
            _Password = "123";
            _Name = "Mock";
            _Surname = "User";
            _Email = "mock@gmail.com";
            _Address = "Mock Address";
            _PhoneNumber = "123456789";
            _ContactInfo = new Models.User.ContactInformationAttribute(_Email, _PhoneNumber, _Address);
        }
        [TestCleanup]
        public void CleanCreatedUsers()
        {
            _Connection.Open();
            string sql = $"DELETE FROM users WHERE username='{_Username}'";
            NpgsqlCommand command = new NpgsqlCommand(sql, _Connection);
            command.ExecuteNonQuery();
            _Connection.Close();
        }
        [TestMethod()]
        public void TestAddNewUser()
        {
            UserController userController = new UserController();
            int numberOfRowsAffected = userController.AddNewUser(_Username, _Password, _Name, _Surname, _ContactInfo);
            Assert.AreEqual(1, numberOfRowsAffected);
        }
        [TestMethod]
        public void TestReturnUserWithGivenUsernameWithAnExistingUser()
        {
            UserController userController = new UserController();
            userController.AddNewUser(_Username, _Password, _Name, _Surname, _ContactInfo);
            User user = userController.ReturnUserWithGivenUsername(_Username);

            _Connection.Open();
             Assert.AreEqual(_Username, user.Username);
             Assert.AreEqual(_Name, user.Name);
             Assert.AreEqual(_Surname, user.Surname);
             Assert.AreEqual(_Email, user.ContactInfo.Email);
             Assert.AreEqual(_PhoneNumber, user.ContactInfo.PhoneNumber);
             Assert.AreEqual(_Address, user.ContactInfo.Address);
            
            _Connection.Close();

        }
        [TestMethod]
        public void TestReturnUserWithGivenUsernameWithNoneExistingUser()
        {
            string username = "Ayla";
            
            _Connection.Open();
            UserController userController = new UserController();
            User user = userController.ReturnUserWithGivenUsername(username);
            Assert.AreEqual(null, user);
            _Connection.Close();

        }
        [TestMethod]
        public void TestReturnUserWithGivenExistingEmail()
        {
            UserController userController = new UserController();
            userController.AddNewUser(_Username, _Password, _Name, _Surname, _ContactInfo);
            User user = userController.ReturnUserWithGivenEmail(_Email);
            Assert.AreEqual(_Username, user.Username);
        }
        [TestMethod]
        public void TestReturnUserWithGivenNonExistingEmail()
        {
            string email = "invalid-mail@gmail.com";

            UserController userController = new UserController();
            User user = userController.ReturnUserWithGivenEmail(email);
            Assert.AreEqual(null, user);
        }
        [TestMethod]
        public void TestAddNewAdToUserWithGivenUsername()
        {
            int taskId = 115;

            UserController userController = new UserController();
            userController.AddNewUser(_Username, _Password, _Name, _Surname, _ContactInfo);
            userController.AddNewAdToUserWithGivenUsername(_Username, taskId);

            _Connection.Open();
            string sqlCommand = $"SELECT ads FROM users WHERE username=\'{_Username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                int[] ads = (int[])reader[0];
                Assert.AreEqual(true, ads.Contains(taskId));
            }
            _Connection.Close();
        }
        [TestMethod]
        public void TestAddNewTaskToUserWithGivenUsername()
        {
            int taskId = 115;

            UserController userController = new UserController();
            userController.AddNewUser(_Username, _Password, _Name, _Surname, _ContactInfo);
            userController.AddNewTaskToUserWithGivenUsername(_Username, taskId);

            _Connection.Open();
            string sqlCommand = $"SELECT tasks FROM users WHERE username=\'{_Username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                int[] tasks = (int[])reader[0];
                Assert.AreEqual(true, tasks.Contains(taskId));
            }
            _Connection.Close();
        }
        [TestMethod]
        public void TestRemoveAdFromUserWithGivenUsername()
        {
            int taskId = 115;

            UserController userController = new UserController();
            userController.AddNewUser(_Username, _Password, _Name, _Surname, _ContactInfo);
            userController.AddNewAdToUserWithGivenUsername(_Username, taskId);
            userController.RemoveAdFromUserWithGivenUsername(_Username, taskId);

            _Connection.Open();
            string sqlCommand = $"SELECT ads FROM users WHERE username=\'{_Username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                int[] ads = (int[])reader[0];
                Assert.AreEqual(false, ads.Contains(taskId));
            }
            _Connection.Close();
        }
        [TestMethod]
        public void TestRemoveTaskFromUserWithGivenUsername()
        {
            int taskId = 115;

            UserController userController = new UserController();
            userController.AddNewUser(_Username, _Password, _Name, _Surname, _ContactInfo);
            userController.AddNewTaskToUserWithGivenUsername(_Username, taskId);
            userController.RemoveTaskFromUserWithGivenUsername(_Username, taskId);

            _Connection.Open();
            string sqlCommand = $"SELECT tasks FROM users WHERE username=\'{_Username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                int[] tasks = (int[])reader[0];
                Assert.AreEqual(false, tasks.Contains(taskId));
            }
            _Connection.Close();
        }
        [TestMethod]
        public void TestUpdateUserRatingWithRate()
        {
            double rate = 1;

            UserController userController = new UserController();
            userController.AddNewUser(_Username, _Password, _Name, _Surname, _ContactInfo);
            userController.UpdateUserRating(rate, _Username);

            User user = userController.ReturnUserWithGivenUsername(_Username);
            Assert.AreEqual(3, user.Rating);
            Assert.AreEqual(2, user.RatingNumber);
        }
        [TestMethod]
        public void TestAddPreferredTaskTypes()
        {
            string[] preferredTaskTypes = { "Grocery Shopping", "Software", "Web Design" };

            UserController userController = new UserController();
            userController.AddNewUser(_Username, _Password, _Name, _Surname, _ContactInfo);
            userController.AddPreferredTaskTypes(_Username, preferredTaskTypes);

            _Connection.Open();
            string sqlQuery = $"SELECT jobtypes FROM users WHERE username=\'{_Username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlQuery, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string[] jobtypes = (string[])reader[0];
                for (int i = 0; i < preferredTaskTypes.Length; i++)
                    Assert.AreEqual(true, jobtypes.Contains(preferredTaskTypes[i]));
            }
            _Connection.Close();
        }
        [TestMethod]
        public void TestDeleteUserAccountWithGivenUsername()
        {
            UserController userController = new UserController();
            userController.AddNewUser(_Username, _Password, _Name, _Surname, _ContactInfo);
            userController.DeleteUserAccountWithGivenUsername(_Username);

            _Connection.Open();
            string sql = $"SELECT * FROM users WHERE username=\'{_Username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sql, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Assert.Equals(reader[0], null);
            }
            _Connection.Close();
        }
    }
}