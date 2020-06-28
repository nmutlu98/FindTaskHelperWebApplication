using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindTaskHelperWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace FindTaskHelperWebApplication.Controllers
{
    public class UpdateAddController : Controller
    {
        private NpgsqlConnection _Connection = (new DatabaseConnection()).GetDatabaseConnection();
        private TaskController _TaskController = new TaskController();
        private HomeController _HomeController = new HomeController();
        private UserController _UserController = new UserController();
        private EmailController _EmailController = new EmailController();
        public IActionResult ProcessUpdatedAdInformation(UpdateAdd updatedAdInformation)
        {
            int paymentAmount = 0;
            bool result = int.TryParse(updatedAdInformation.PaymentAmountField, out paymentAmount);
           
            Models.Task oldTaskInfo = _TaskController.ReturnTaskWithGivenTaskId(updatedAdInformation.TaskIdField);
            updatedAdInformation.LocationField = updatedAdInformation.Country + " " + updatedAdInformation.State + " " + updatedAdInformation.City;
            updatedAdInformation.PaymentAmountField = updatedAdInformation.PaymentAmountField + " " + updatedAdInformation.Currency;
            Models.Task ad = new Models.Task(true, updatedAdInformation.TaskIdField, oldTaskInfo.TaskOwnerUsername,
                oldTaskInfo.CreationDate, updatedAdInformation.TitleField, updatedAdInformation.DescriptionField,
                oldTaskInfo.Location, updatedAdInformation.AddressField, oldTaskInfo.TaskPhotoUrl,
                updatedAdInformation.PaymentAmountField, oldTaskInfo.IsAssignedToSomeone, oldTaskInfo.AssignedToUserWithThisUsername,
                oldTaskInfo.IsCompleted, updatedAdInformation.CategoryField);
            if (!result)
            {
                return _TaskController.LoadUpdatedMyAds(ad, "fail-payment");
            }
            return _TaskController.LoadUpdatedMyAds(ad, "success");
        }
        public IActionResult DeleteAd(UpdateAdd formWithOnlyTaskId)
        {
            Models.Task taskWillBeDeleted = _TaskController.ReturnTaskWithGivenTaskId(formWithOnlyTaskId.TaskIdField);
            User tasker = _UserController.ReturnUserWithGivenUsername(taskWillBeDeleted.AssignedToUserWithThisUsername);
            if(tasker != null) { 
                Email infoMailToTasker = new Email(tasker.ContactInfo.Email, tasker.Username, "The job is cancelled by the owner!",
                                                   _EmailController.TheJobIsCancelledMessage(taskWillBeDeleted.TaskOwnerUsername, taskWillBeDeleted.TaskID));
                _EmailController.SendMail(infoMailToTasker);
                _UserController.RemoveTaskFromUserWithGivenUsername(tasker.Username, taskWillBeDeleted.TaskID);
            }
            _UserController.RemoveAdFromUserWithGivenUsername(taskWillBeDeleted.TaskOwnerUsername, taskWillBeDeleted.TaskID);
            _TaskController.DeleteTaskWithGivenTaskId(formWithOnlyTaskId.TaskIdField);
            return _HomeController.MyAds(taskWillBeDeleted.TaskOwnerUsername);
        }
        public IActionResult AssignAd(UpdateAdd formWithTaskIdAndTasker)
        {
            int taskId = formWithTaskIdAndTasker.TaskIdField;
            string tasker = formWithTaskIdAndTasker.Tasker;
            Models.Task task = _TaskController.ReturnTaskWithGivenTaskId(taskId);
            User owner = _UserController.ReturnUserWithGivenUsername(task.TaskOwnerUsername);
            User taskTaker = _UserController.ReturnUserWithGivenUsername(tasker);
            _UserController.AddNewTaskToUserWithGivenUsername(tasker, taskId);
            _TaskController.AssignTaskToUserWithGivenUsername(taskId, tasker);
            Email emailToOwner = new Email(owner.ContactInfo.Email, owner.Username, "A tasker is accepted the job!",
                                           _EmailController.PrepareATaskerFoundMessage(task.Title, task.TaskOwnerUsername, tasker));
            _EmailController.SendMail(emailToOwner);
            Email emailToTasker = new Email(taskTaker.ContactInfo.Email, tasker, "The task " + task.Title + " is assigned to you!",
                                            _EmailController.PrepareTaskIsAssignedToYouMessage(task.Title, owner.Username, taskId));
            _EmailController.SendMail(emailToTasker);
            return _HomeController.MyTasks(tasker);
        }
        public IActionResult RateTaskOwner(UpdateAdd formWithRateAndTaskOwner)
        {
            int taskId = formWithRateAndTaskOwner.TaskIdField;
            _Connection.Open();
            string sqlCommand = $"UPDATE ratings SET taskownerstatus='true' WHERE taskid='{taskId}'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            command.ExecuteNonQuery();
            _Connection.Close();

            _UserController.UpdateUserRating(formWithRateAndTaskOwner.Rate, formWithRateAndTaskOwner.TaskOwner);
            return _HomeController.MyTasks(formWithRateAndTaskOwner.Tasker);
        }
        public IActionResult RateTasker(UpdateAdd formWithRateAndTasker)
        {
            int taskId = formWithRateAndTasker.TaskIdField;
            _Connection.Open();
            string sqlCommand = $"UPDATE ratings SET taskerstatus='true' WHERE taskid='{taskId}'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            command.ExecuteNonQuery();
            _Connection.Close();

            _UserController.UpdateUserRating(formWithRateAndTasker.Rate, formWithRateAndTasker.Tasker);
            return _HomeController.MyAds(formWithRateAndTasker.TaskOwner);
        }
        public IActionResult QuitTask(UpdateAdd formWithTaskIdAndTasker)
        {
            int taskId = formWithTaskIdAndTasker.TaskIdField;
            Models.Task task = _TaskController.ReturnTaskWithGivenTaskId(taskId);
            User ownerOfTask = _UserController.ReturnUserWithGivenUsername(task.TaskOwnerUsername);
            string taskerUsername = formWithTaskIdAndTasker.Tasker;
            Email emailToOwner = new Email(ownerOfTask.ContactInfo.Email, ownerOfTask.Username, "The tasker quitted from the job!",
                                            _EmailController.TaskerQuittedMessage(ownerOfTask.Username, taskId, taskerUsername));
             _TaskController.MarkTaskWithGivenTaskIdAsActive(taskId);
            _EmailController.SendMail(emailToOwner);
             _UserController.RemoveTaskFromUserWithGivenUsername(taskerUsername, taskId);
            return _HomeController.MyTasks(taskerUsername);
        }
        public IActionResult MarkAsCompleted(UpdateAdd formForCompletion)
        {
            string currentUser = formForCompletion.Tasker;
            string adOwner = formForCompletion.TaskOwner;
            int relatedTaskId = formForCompletion.TaskIdField;
            User owner = _UserController.ReturnUserWithGivenUsername(adOwner);
            Email emailToOwner = new Email(owner.ContactInfo.Email, adOwner, "You job is completed!",
                                           _EmailController.JobCompletedMessage(adOwner, relatedTaskId, currentUser));
            _EmailController.SendMail(emailToOwner);
            _TaskController.MarkTaskWithGivenTaskIdAsCompleted(relatedTaskId);
            return _HomeController.MyTasks(currentUser);
        }
        public IActionResult FilterAds(UpdateAdd filters)
        {
            string location = filters.LocationField;
            string potentialTitle = filters.TitleField;
            string loggedInUser = filters.Tasker;
            if (location == null)
                location = "";
            if (potentialTitle == null)
                potentialTitle = "";
            if (loggedInUser == null)
                loggedInUser = "";
            if (!loggedInUser.Equals("")) {
                return _HomeController.IndexLoggedIn(username: loggedInUser, location: location, potentialTitle: potentialTitle);
            }
            else {
                return _HomeController.Index(location: location, potentialTitle: potentialTitle);
            }

        }
    }
}
