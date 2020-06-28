using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindTaskHelperWebApplication.Models
{
    public class AddNewAd
    {
        private string _TaskOwnerUsername;
        private string _CreationDate;
        private string _TitleField;
        private string _DescriptionField;
        private string _Country;
        private string _State;
        private string _City;
        private string _LocationField;
        private string _AddressField;
        private string _TaskPhotoUrlField;
        private string _PaymentAmountField;
        private string _Category;
        private string _OtherCategory;
        private string _Currency;

        public string TaskOwnerUsername
        {
            get
            {
                return _TaskOwnerUsername;
            }
            set
            {
                _TaskOwnerUsername = value;
            }
        }
        public string CreationDate
        {
            get
            {
                return _CreationDate;
            }
            set
            {
                _CreationDate = value;
            }
        }
        public String TitleField
        {
            get
            {
                return _TitleField;
            }
            set
            {
                _TitleField = value;
            }
        }
        public string Country
        {
            get
            {
                return _Country;
            }
            set
            {
                _Country = value;
            }
        }
        public string State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
            }
        }
        public string City
        {
            get
            {
                return _City;
            }
            set
            {
                _City = value;
            }
        }
        public string LocationField
        {
            get
            {
                return _LocationField;
            }
            set
            {
                _LocationField = value;
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

        public string DescriptionField
        {
            get
            {
                return _DescriptionField;
            }
            set
            {
                _DescriptionField = value;
            }
        }

        public string TaskPhotoUrl
        {
            get
            {
                return _TaskPhotoUrlField;
            }
            set
            {
                _TaskPhotoUrlField = value;
            }
        }

        public string PaymentAmountField
        {
            get
            {
                return _PaymentAmountField;
            }
            set
            {
                _PaymentAmountField = value;
            }
        }
        public string Category
        {
            get
            {
                return _Category;
            }
            set
            {
                _Category = value;
            }
        }
        public string OtherCategory
        {
            get
            {
                return _OtherCategory;
            }
            set
            {
                _OtherCategory = value;
            }
        }
        public string Currency
        {
            get
            {
                return _Currency;
            }
            set
            {
                _Currency = value;
            }
        }

    }
}
