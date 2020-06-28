using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace FindTaskHelperWebApplication.Models
{
    public class Profile
    {
        private string _UsernameField;
        private string _NameField;
        private string _SurnameField;
        private string _EmailField;
        private string _PasswordField;
        private string _PasswordConfirmationField;
        private string _OldPasswordField;
        private string _PhoneField;
        private string _AddressField;
        private string _PhotoUrlField;
        private IFormFile _Photo;
        private string _DeleteButton;
        public string UsernameField
        {
            get
            {
                return _UsernameField;
            }
            set
            {
                _UsernameField = value;
            }
        }
        public string NameField
        {
            get
            {
                return _NameField;
            }
            set
            {
                _NameField = value;
            }
        }
        public string SurnameField
        {
            get
            {
                return _SurnameField;
            }
            set
            {
                _SurnameField = value;
            }
        }
        public string OldPasswordField
        {
            get
            {
                return _OldPasswordField;
            }
            set
            {
                _OldPasswordField = value;
            }
        }
        public string PasswordField
        {
            get
            {
                return _PasswordField;
            }
            set
            {
                _PasswordField = value;
            }
        }
        public string PasswordConfirmationField
        {
            get
            {
                return _PasswordConfirmationField;
            }
            set
            {
                _PasswordConfirmationField = value;
            }
        }
        public string EmailField
        {
            get
            {
                return _EmailField;
            }
            set
            {
                _EmailField = value;
            }
        }
        public string PhoneField
        {
            get
            {
                return _PhoneField;
            }
            set
            {
                _PhoneField = value;
            }
        }
        public string AddressField
        {
            get
            {
                return _AddressField;
            }
            set
            {
                _AddressField = value;
            }
        }
        public string PhotoUrlField
        {
            get
            {
                return _PhotoUrlField;
            }
            set
            {
                _PhotoUrlField = value;
            }
        }
        public IFormFile Photo
        {
            get
            {
                return _Photo;
            }
            set
            {
                _Photo = value;
            }
        }
        public string DeleteButton
        {
            get
            {
                return _DeleteButton;
            }
            set
            {
                _DeleteButton = value;
            }
        }
    }
    
}
