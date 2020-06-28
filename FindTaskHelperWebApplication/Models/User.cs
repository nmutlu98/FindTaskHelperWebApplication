using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace FindTaskHelperWebApplication.Models
{
    public class User
    {
        public class ContactInformationAttribute : Attribute
        {
            private string _Email;
            private string _PhoneNumber;
            private string _Address;

            public string Email
            {
                get
                {
                    return _Email;
                }
                set
                {
                    _Email = value;
                }
            }
            public string PhoneNumber
            {
                get
                {
                    return _PhoneNumber;
                }
                set
                {
                    _PhoneNumber = value;
                }
            }
    public string Address {
                get
                {
                    return _Address;
                }
                set
                {
                    _Address = value;
                }
            }

            public ContactInformationAttribute(string email, string phoneNumber, string address)
            {
                this._Email= email;
                this._PhoneNumber = phoneNumber;
                this._Address = address;
            }

        }

        private const int INITIAL_CAPACITY = 20;
        private string _Username;
        private string _Password;
        private string _Name;
        private string _Surname;
        private ContactInformationAttribute _ContactInfo;
        private int[] _Tasks;
        private int[] _Ads;
        private double _Rating;
        private string _PhotoUrl;
        private int _RatingNumber;
        private string[] _PreferredJobCategories;
        public string Username {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
            }
        }
        public string Password {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }
        public string Name {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public string Surname {
            get
            {
                return _Surname;
            }
            set
            {
                _Surname = value;
            }
        }
        public ContactInformationAttribute ContactInfo {
            get
            {
                return _ContactInfo;
            }
            set
            {
                _ContactInfo = ContactInfo;
            }
        }
        public int[] Tasks
        {
            get
            {
                return _Tasks;
            }
            set
            {
                _Tasks = value;
            }
        }
        public int[] Ads {
            get
            {
                return _Ads;
            }
            set
            {
                _Ads = value;
            }
        }
        public double Rating {
            get
            {
                return _Rating;
            }
            set
            {
                _Rating = value;
            }
        }
        public string PhotoUrl
        {
            get
            {
                return _PhotoUrl;
            }
            set
            {
                _PhotoUrl = value;
            }
        }

        public int RatingNumber
        {
            get
            {
                return _RatingNumber;
            }
            set
            {
                _RatingNumber = value;
            }
        } 
        public string[] PreferredJobCategories
        {
            get
            {
                return _PreferredJobCategories;
            }
            set
            {
                _PreferredJobCategories = value;
            }
        }
        public User(string username, string password, string name, string surname, ContactInformationAttribute contactInfo, int[] tasks, int[] ads, double rating, string photoUrl, int ratingNumber, string[] preferredJobCategories)
        {
            this._Username = username;
            this._Password = password;
            this._Name = name;
            this._Surname = surname;
            this._ContactInfo = contactInfo;
            this._Tasks = tasks;
            this._Ads = ads;
            this._Rating = rating;
            this._PhotoUrl = photoUrl;
            this._RatingNumber = ratingNumber;
            this._PreferredJobCategories = preferredJobCategories;

        }

    }
}
