using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindTaskHelperWebApplication.Models
{
    public class Login
    {
        private string _UsernameField;
        private string _PasswordField;
        private string _EmailFieldForForgotPassword;
        private bool _RegisterButton;

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
        public string PasswordField
        {
            get
            {
                return _PasswordField;
            }
            set
            {
                _PasswordField =  value;
            }
        }
        public bool RegisterButton
        {
            get
            {
                return _RegisterButton;
            }
            set
            {
                _RegisterButton = value;
            }
        }
        public string EmailFieldForForgotPassword
        {
            get
            {
                return _EmailFieldForForgotPassword;
            }
            set
            {
                _EmailFieldForForgotPassword = value;
            }
        }
    }
}
