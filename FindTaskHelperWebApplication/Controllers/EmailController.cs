using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FindTaskHelperWebApplication.Models;
using System.Net.Mail;
using System.Net;

namespace FindTaskHelperWebApplication.Controllers
{
    public class EmailController : Controller
    {
        private UserController _UserController = new UserController();
        private TaskController _TaskController = new TaskController();
        public string PrepareWelcomeMessage(string newUserUsername) {

            string mailBody = "Welcome to Tasker! \n" +
                "Hello " + newUserUsername + "\n" +
                "Welcome to Tasker! \n" +
                "May taskers be with you!";
                
            return mailBody;
        }
        public string PrepareATaskerFoundMessage(string taskTitle, string owner, string tasker)
        {
            User taskerUser = _UserController.ReturnUserWithGivenUsername(tasker);
            string mailBody = "A tasker accepted the job!\n" +
                              taskTitle + " is accepted by " + tasker +
                              "\nRating: "+taskerUser.Rating+
                              "\nContact Information: \n" +
                              "Email: " + taskerUser.ContactInfo.Email +
                              "\nPhone: " + taskerUser.ContactInfo.PhoneNumber;
                              

            return mailBody;
        }
        public string PrepareTaskIsAssignedToYouMessage(string taskTitle, string owner, int taskID)
        {
            Models.Task task = _TaskController.ReturnTaskWithGivenTaskId(taskID);
            User ownerUser = _UserController.ReturnUserWithGivenUsername(owner);
            string mailBody = taskTitle + " is assigned to you!\n" +
                              "Task Owner Information: \n" +
                              "Rating: " + ownerUser.Rating +
                              "\nContact Information: \n" +
                              "Email: " + ownerUser.ContactInfo.Email +
                              "\nPhone: " + ownerUser.ContactInfo.PhoneNumber +
                              "\nTask Information: \n" +
                              "\nTask Title: " + task.Title +
                              "\nTask Description: " + task.Description +
                              "\nAddress: " + task.Address+" ,"+task.Location;
                                
            return mailBody;
        }
        public string TheJobIsCancelledMessage(string owner, int taskID)
        {
            Models.Task task = _TaskController.ReturnTaskWithGivenTaskId(taskID);
            User ownerUser = _UserController.ReturnUserWithGivenUsername(owner);
            string mailBody = owner + " cancelled the task " + task.Title + ".\n" +
                              "You may search for new tasks that may be suitable for you!";
            return mailBody;
        }
        public string TheJobIsUpdatedMessage(string owner, int taskID)
        {
            Models.Task task = _TaskController.ReturnTaskWithGivenTaskId(taskID);
            User ownerUser = _UserController.ReturnUserWithGivenUsername(owner);
            string mailBody = owner + " updated the task " + task.Title + ".\n" +
                              "You may check new details in My Tasks page.";
            return mailBody;
        }
        public string TaskerQuittedMessage(string owner, int taskID, string tasker)
        {
            Models.Task task = _TaskController.ReturnTaskWithGivenTaskId(taskID);
            User ownerUser = _UserController.ReturnUserWithGivenUsername(owner);
            string mailBody = tasker + " quitted from the task " + task.Title + ".\n" +
                              "Your task is marken as active again and will be displayed to new taskers!";
            return mailBody;
        }
        public string JobCompletedMessage(string owner, int taskID, string tasker)
        {
            Models.Task task = _TaskController.ReturnTaskWithGivenTaskId(taskID);
            User ownerUser = _UserController.ReturnUserWithGivenUsername(owner);
            string mailBody = tasker + " completed the task " + task.Title + ".\n" +
                              "You can now rate the tasker from My Ads section.";
            return mailBody;
        }
        public string ForgotPasswordMessage(string password)
        {
            string mailBody = " Dear user,\n " +
                              "Your new password is: " + password;
            return mailBody;
        }
        public void SendMail(Email email)
        {
            string username = email.Username;
            string toUser = email.ToUser;
            string from = email.From;
            string subject = email.Subject;
            string message = email.Message;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(from);
                mail.To.Add(toUser);
                mail.Subject = subject;
                mail.Body = message;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(from, "ABleeminho23.");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                
            }
            catch (Exception ex)
            {
              Console.WriteLine("Error"+ex.ToString());
            }

        }
    }
}
