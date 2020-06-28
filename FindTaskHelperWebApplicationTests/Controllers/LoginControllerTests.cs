using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindTaskHelperWebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using FindTaskHelperWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.IO;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FindTaskHelperWebApplication.Controllers.Tests
{
    [TestClass()]
    public class LoginControllerTests
    {
        private User user;
        private string _Username;
        private string _Password;

        [TestInitialize]
        public void InitTest()
        {
            string username = "Mock-User-1";
            _Username = username;
            string password = "123";
            _Password = password;
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
        public void LoginIndexTest()
        {
            var mockCurrentSession = new Mock<ICurrentSession>();
            mockCurrentSession.Setup(x => x.SetUsername(user.Username)).Returns(true);
            LoginController loginController = new LoginController(mockCurrentSession.Object);
            var result = loginController.LoginIndex() as ViewResult;
            Assert.AreEqual("Views/Login/Login.cshtml", result.ViewName);
        }

        [TestMethod()]
        public void ValidateCredentialsTestWithCorrectCredentials()
        {
            var mockCurrentSession = new Mock<ICurrentSession>();
            mockCurrentSession.Setup(x => x.SetUsername(user.Username)).Returns(true);
            LoginController loginController = new LoginController(mockCurrentSession.Object);
            Login login = new Login();
            login.UsernameField = _Username;
            login.PasswordField = _Password;

            var result = loginController.ValidateCredentials(login) as ViewResult;
            Assert.AreEqual("Views/Home/IndexLoggedIn.cshtml", result.ViewName);
            Assert.AreEqual("loggedIn", result.ViewData["message"]);

        }
        [TestMethod()]
        public void ValidateCredentialsTestWithCorrectUsernameIncorrectPassword()
        {
            string inCorrectPassword = "456";
            var mockCurrentSession = new Mock<ICurrentSession>();
            mockCurrentSession.Setup(x => x.SetUsername(user.Username)).Returns(true);
            LoginController loginController = new LoginController(mockCurrentSession.Object);
            Login login = new Login();
            login.UsernameField = _Username;
            login.PasswordField = inCorrectPassword;

            var result = loginController.ValidateCredentials(login) as ViewResult;
            Assert.AreEqual("Views/Login/Login.cshtml", result.ViewName);
            Assert.AreEqual("password", result.ViewData["message"]);
        }
        [TestMethod()]
        public void ValidateCredentialsTestWithIncorrectUsername()
        {
            string incorrectUsername = "Mocky User";
            var mockCurrentSession = new Mock<ICurrentSession>();
            mockCurrentSession.Setup(x => x.SetUsername(user.Username)).Returns(true);
            LoginController loginController = new LoginController(mockCurrentSession.Object);
            Login login = new Login();
            login.UsernameField = incorrectUsername;
            login.PasswordField = _Password;

            var result = loginController.ValidateCredentials(login) as ViewResult;
            Assert.AreEqual("Views/Login/Login.cshtml", result.ViewName);
            Assert.AreEqual("username", result.ViewData["message"]);
        }

        [TestMethod()]
        public void ForgotPasswordTestWithCorrectMail()
        {
            Login forgotPasswordForm = new Login();
            forgotPasswordForm.EmailFieldForForgotPassword = user.ContactInfo.Email;

            var mockCurrentSession = new Mock<ICurrentSession>();
            mockCurrentSession.Setup(x => x.SetUsername(user.Username)).Returns(true);
            

            LoginController loginController = new LoginController(mockCurrentSession.Object);
            var result = loginController.ForgotPassword(forgotPasswordForm) as ViewResult;
            Assert.AreEqual("Views/Login/Login.cshtml", result.ViewName);
            Assert.AreEqual("check-mailbox", result.ViewData["message"]);


        }
        [TestMethod()]
        public void ForgotPasswordTestWithIncorrectMail()
        {
            string incorrectMail = "mock2@gmail.com";
            Login forgotPasswordForm = new Login();
            forgotPasswordForm.EmailFieldForForgotPassword = incorrectMail;

            var mockCurrentSession = new Mock<ICurrentSession>();
            mockCurrentSession.Setup(x => x.SetUsername(user.Username)).Returns(true);


            LoginController loginController = new LoginController(mockCurrentSession.Object);
            var result = loginController.ForgotPassword(forgotPasswordForm) as ViewResult;
            Assert.AreEqual("Views/Login/Login.cshtml", result.ViewName);
            Assert.AreEqual("fail-to-find-email", result.ViewData["message"]);
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

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
    
}