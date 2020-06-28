using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindTaskHelperWebApplication.Models
{
    public class Email
    {
        private string _ToUser;
        private string _Username;
        private string _Message;
        private const string _From = "taskertheapp@gmail.com";
        private string _Subject;
        public string ToUser
        {
            get
            {
                return _ToUser;
            }
            set
            {
                _ToUser = value;
            }
        }
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
            }
        }
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }
        public string From
        {
            get
            {
                return _From;
            }
        }

        public string Subject
        {
            get
            {
                return _Subject;
            }
            set
            {
                _Subject = value;
            }
        }
        public Email(string toUser, string username, string subject, string message)
        {
            _ToUser = toUser;
            _Username = username;
            _Subject = subject;
            _Message = message;
        }
        
    }

}
