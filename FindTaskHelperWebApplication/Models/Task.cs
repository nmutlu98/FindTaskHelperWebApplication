using FindTaskHelperWebApplication.Controllers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindTaskHelperWebApplication.Models
{
    public class Task
    {
        private int _TaskId;
        private string _TaskOwnerUsername;
        private string _CreationDate;
        private string _Title;
        private string _Description;
        private string _Location;
        private string _Country;
        private string _State;
        private string _City;
        private string _Address;
        private string _TaskPhotoUrl;
        private string _PaymentAmount;
        private string _Currency;
        private bool _IsAssignedToSomeone;
        private string _AssignedToUserWithThisUsername;
        private bool _IsCompleted;
        private string _JobCategory;
        private TaskController _TaskController = new TaskController();

        public int TaskID
        {
            get
            {
                return _TaskId;
            }
            set
            {
                _TaskId = value;
            }
        }

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

        public String Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }

        public string Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
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

        public string Address
        {
            get
            {
                return _Address;
            }
            set
            {
                _Address = value;
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }
        
        public string TaskPhotoUrl
        {
            get
            {
                return _TaskPhotoUrl;
            }
            set
            {
                _TaskPhotoUrl = value;
            }
        }

        public string PaymentAmount
        {
            get
            {
                return _PaymentAmount;
            }
            set
            {
                _PaymentAmount = value;
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
        public bool IsAssignedToSomeone
        {
            get
            {
                return _IsAssignedToSomeone;
            }
            set
            {
                _IsAssignedToSomeone = value;
            }
        }

        public string AssignedToUserWithThisUsername
        {
            get
            {
                return _AssignedToUserWithThisUsername;
            }
            set
            {
                _AssignedToUserWithThisUsername = value;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return _IsCompleted;
            }
            set
            {
                _IsCompleted = value;
            }
        }
        public string JobCategory
        {
            get
            {
                return _JobCategory;
            }
            set
            {
                _JobCategory = value;
            }
        }

        public Task(bool alreadyExistingTask, int taskId, string taskOwnerUsername, string creationDate, string title, string description, string location,
            string address, string taskPhotoUrl, string paymentAmount, bool isAssignedToSomeone, string assignedToUserWithThisUsername, bool isCompleted, string jobCategory)
        {
            if (alreadyExistingTask)
            {
                this._TaskId = taskId;
                this._TaskOwnerUsername = taskOwnerUsername;
                this._CreationDate = creationDate;
                this._Title = title;
                this._Description = description;
                this._Location = location;
                this._Address = address;
                this._TaskPhotoUrl = taskPhotoUrl;
                this._PaymentAmount = paymentAmount;
                this._IsAssignedToSomeone = isAssignedToSomeone;
                this._AssignedToUserWithThisUsername = assignedToUserWithThisUsername;
                this._IsCompleted = isCompleted;
                this.JobCategory = jobCategory;
            }
            else
            {
                this._TaskId = _TaskController.CurrentTaskId;
                this._TaskOwnerUsername = taskOwnerUsername;
                this._CreationDate = creationDate;
                this._Title = title;
                this._Description = description;
                this._Location = location;
                this._Address = address;
                this._TaskPhotoUrl = taskPhotoUrl;
                this._PaymentAmount = paymentAmount;
                this._IsAssignedToSomeone = isAssignedToSomeone;
                this._AssignedToUserWithThisUsername = assignedToUserWithThisUsername;
                this._IsCompleted = isCompleted;
                this.JobCategory = jobCategory;
            }
        }
            

    }
}
