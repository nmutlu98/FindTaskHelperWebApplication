using FindTaskHelperWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FindTaskHelperWebApplication.Controllers
{
    public class RegisterController : Controller
    {
        private NpgsqlConnection _Connection = new DatabaseConnection().GetDatabaseConnection();
        private UserController _UserController = new UserController();
        private EmailController _EmailController = new EmailController();
        public ActionResult RegisterIndex(){
            return View("Views/Register/RegisterView.cshtml");
        }

        [HttpPost]
        public ActionResult GetRegisterFormInputs(Register register)
        {
            _Connection.Open();

            string sqlQuery = "SELECT username from users";
            NpgsqlCommand command = new NpgsqlCommand(sqlQuery, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if((string) reader[0] == register.UsernameField)
                {
                    ViewData["message"] = "fail";
                    return View("Views/Register/RegisterView.cshtml");
                }
            }

            _Connection.Close();
            _Connection.Open();

            string sqlQueryForEmail = "SELECT email from users";
            command = new NpgsqlCommand(sqlQueryForEmail, _Connection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                if ((string)reader[0] == register.EmailField)
                {
                    ViewData["message"] = "mail-not-unique";
                    return View("Views/Register/RegisterView.cshtml");
                }
            }

            _Connection.Close();

            _UserController.AddNewUser(register.UsernameField, Encrypt(register.PasswordField), register.NameField, register.SurnameField,
                                        new User.ContactInformationAttribute(register.EmailField, register.PhoneNumberField, register.AddressField));
            ViewData["message"] = "success";
            Email registrationSuccess = new Email(register.EmailField, register.UsernameField,  "Welcome to Tasker!", _EmailController.PrepareWelcomeMessage(register.UsernameField));
            _EmailController.SendMail(registrationSuccess);
            List<string> preferredJobs = new List<string>();
            if (register.GroceryCheckBox.Equals("true"))
                preferredJobs.Add("Grocery Shopping");
            if (register.BabySittingCheckBox.Equals("true"))
                preferredJobs.Add("Baby Sitting");
            if (register.DogWalkingCheckBox.Equals("true"))
                preferredJobs.Add("Dog Walking");
            if (register.FurnitureCheckBox.Equals("true"))
                preferredJobs.Add("Furniture Set Up");
            if (register.GardeningCheckBox.Equals("true"))
                preferredJobs.Add("Gardening");
            if (register.PlumbingCheckBox.Equals("true"))
                preferredJobs.Add("Plumbing");
            if (register.ShippingCheckBox.Equals("true"))
                preferredJobs.Add("Shipping");
            if (register.WebDesignCheckBox.Equals("true"))
                preferredJobs.Add("Web Design");
            if (register.TeachingCheckBox.Equals("true"))
                preferredJobs.Add("Teaching");
            string[] preferredJobTypes = preferredJobs.ToArray();
            _UserController.AddPreferredTaskTypes(register.UsernameField, preferredJobTypes);
            return View("Views/Register/RegisterView.cshtml");

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
