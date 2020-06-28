using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FindTaskHelperWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FindTaskHelperWebApplication.Controllers
{
    public class LoginController : Controller
    {
        private HomeController _HomeController = new HomeController();
        private UserController _UserController = new UserController();
        private EmailController _EmailController = new EmailController();
        private ICurrentSession _CurrentSession;

        public LoginController(ICurrentSession currentSession)
        {
            _CurrentSession = currentSession;
        }
        public IActionResult LoginIndex(string message="")
        {
            ViewData["message"] = message;
            return View("Views/Login/Login.cshtml");
        }
        
        public IActionResult ValidateCredentials(Login login)
        {
            _CurrentSession.SetContextAccessor(new HttpContextAccessor());
            RegisterController registerController = new RegisterController();
            string username = login.UsernameField;
            string password = login.PasswordField;
            bool register = login.RegisterButton;
           
            if (register)
                return registerController.RegisterIndex();
            User user = _UserController.ReturnUserWithGivenUsername(username);
            if (user == null)
                return LoginIndex(message: "username");
            else if (!Decrypt(user.Password).Equals(password))
            {
                return LoginIndex(message: "password");
            }
            ViewData["message"] = "loggedIn";
            _CurrentSession.SetUsername(username);
            ViewData["username"] = username;
            return _HomeController.IndexLoggedIn(username,message: "loggedIn");
        }
        public IActionResult ForgotPassword(Login login)
        {
            string newPassword = RandomString(8);
            User user = _UserController.ReturnUserWithGivenEmail(login.EmailFieldForForgotPassword);
            if(user == null)
            {
                return LoginIndex("fail-to-find-email");
            }
            user.Password = Encrypt(newPassword);
            _UserController.UpdateUserInformationWithGivenUsername(user.Username, user);
            Email email = new Email(login.EmailFieldForForgotPassword, user.Username, "New Password Request",
                                    _EmailController.ForgotPasswordMessage(newPassword));
            _EmailController.SendMail(email);
            return LoginIndex("check-mailbox");
        }
        public int UpdatePassword(string username, string password)
        {
            password = Encrypt(password);
            User user = _UserController.ReturnUserWithGivenUsername(username);
            user.Password = password;
            return _UserController.UpdateUserInformationWithGivenUsername(username, user);
        }
        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
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
        private string RandomString(int size, bool lowerCase=true)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

    }
}
