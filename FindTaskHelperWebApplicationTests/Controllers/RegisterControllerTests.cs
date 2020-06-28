using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindTaskHelperWebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using FindTaskHelperWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindTaskHelperWebApplication.Controllers.Tests
{
    [TestClass()]
    public class RegisterControllerTests
    {
        private string _UsernameField;
        private string _NameField;
        private string _SurnameField;
        private string _EmailField;
        private string _PasswordField;
        private string _PhoneNumberField;
        private string _AddressField;
        [TestInitialize]
        public void InitTest()
        {
            _UsernameField = "Mock-User";
            _NameField = "Mock";
            _SurnameField = "User";
            _EmailField = "mock@gmail.com";
            _PasswordField = "123";
            _PhoneNumberField = "456789";
            _AddressField = "Mock adress;";
        }
        [TestCleanup]
        public void CleanCreatedUser()
        {
            UserController userController = new UserController();
            userController.DeleteUserAccountWithGivenUsername(_UsernameField);
        }
        [TestMethod()]
        public void RegisterIndexTest()
        {
            RegisterController registerController = new RegisterController();
            var result = registerController.RegisterIndex() as ViewResult;
            Assert.AreEqual("Views/Register/RegisterView.cshtml", result.ViewName);
        }

        [TestMethod()]
        public void GetRegisterFormInputsTestWithUniqueEmailAndUniqueUsername()
        {
            Register registerForm = new Register();
            registerForm.UsernameField = _UsernameField;
            registerForm.NameField = _NameField;
            registerForm.SurnameField = _SurnameField;
            registerForm.PasswordField = _PasswordField;
            registerForm.EmailField = _EmailField;
            registerForm.PhoneNumberField = _PhoneNumberField;
            registerForm.AddressField = _AddressField;
            registerForm.BabySittingCheckBox = "false";
            registerForm.DogWalkingCheckBox = "false";
            registerForm.FurnitureCheckBox = "false";
            registerForm.GardeningCheckBox = "false";
            registerForm.GroceryCheckBox = "false";
            registerForm.PlumbingCheckBox = "false";
            registerForm.ShippingCheckBox = "false";
            registerForm.SoftwareCheckBox = "false";
            registerForm.TeachingCheckBox = "false";
            registerForm.WebDesignCheckBox = "false";

            RegisterController registerController = new RegisterController();
            var result = registerController.GetRegisterFormInputs(registerForm) as ViewResult;

            Assert.AreEqual("Views/Register/RegisterView.cshtml", result.ViewName);
            Assert.AreEqual("success", result.ViewData["message"]);
        }
        [TestMethod()]
        public void GetRegisterFormInputsTestWithNonUniqueEmailAndUniqueUsername()
        {
            UserController userController = new UserController();
            userController.AddNewUser(_UsernameField, _PasswordField, _NameField, _SurnameField, new User.ContactInformationAttribute("mockEmail2@gmail.com", _PhoneNumberField, _AddressField));
            Register registerForm = new Register();
            registerForm.UsernameField = _UsernameField;
            registerForm.NameField = _NameField;
            registerForm.SurnameField = _SurnameField;
            registerForm.PasswordField = _PasswordField;
            registerForm.EmailField = _EmailField;
            registerForm.PhoneNumberField = _PhoneNumberField;
            registerForm.AddressField = _AddressField;

            RegisterController registerController = new RegisterController();
            var result = registerController.GetRegisterFormInputs(registerForm) as ViewResult;

            Assert.AreEqual("Views/Register/RegisterView.cshtml", result.ViewName);
            Assert.AreEqual("fail", result.ViewData["message"]);
        }
        [TestMethod()]
        public void GetRegisterFormInputsTestWithUniqueEmailAndNonUniqueUsername()
        {
            UserController userController = new UserController();
            string newUsername = "Mock-User-2";
            userController.AddNewUser(newUsername, _PasswordField, _NameField, _SurnameField, new User.ContactInformationAttribute(_EmailField, _PhoneNumberField, _AddressField));
            Register registerForm = new Register();
            registerForm.UsernameField = _UsernameField;
            registerForm.NameField = _NameField;
            registerForm.SurnameField = _SurnameField;
            registerForm.PasswordField = _PasswordField;
            registerForm.EmailField = _EmailField;
            registerForm.PhoneNumberField = _PhoneNumberField;
            registerForm.AddressField = _AddressField;

            RegisterController registerController = new RegisterController();
            var result = registerController.GetRegisterFormInputs(registerForm) as ViewResult;

            Assert.AreEqual("Views/Register/RegisterView.cshtml", result.ViewName);
            Assert.AreEqual("mail-not-unique", result.ViewData["message"]);

            userController.DeleteUserAccountWithGivenUsername(newUsername);
        }
    }
}