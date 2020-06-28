using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindTaskHelperWebApplication.Models
{
    public class UpdateAdd
    {
        private string _TitleField;
        private string _DescriptionField;
        private string _LocationField;
        private string _Country;
        private string _State;
        private string _City;
        private string _AddressField;
        private string _PaymentAmountField;
        private string _Currency;
        private int _TaskIdField = -1;
        private string _Tasker = "";
        private int _Rate = 0;
        private string _TaskOwner = "";
        private string _CategoryField;
        public string TitleField
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
        public int TaskIdField
        {
            get
            {
                return _TaskIdField;
            }
            set
            {
                _TaskIdField = value;
            }
        } 
        public string Tasker
        {
            get
            {
                return _Tasker;
            }
            set
            {
                _Tasker = value;

            }
        }
        public int Rate
        {
            get
            {
                return _Rate;
            }
            set
            {
                _Rate = value;
            }
        }
        public string TaskOwner
        {
            get
            {
                return _TaskOwner;
            }
            set
            {
                _TaskOwner = value;
            }
        }
        public string CategoryField
        {
            get
            {
                return _CategoryField;
            }
            set
            {
                _CategoryField = value;
            }
        }
    }
}
