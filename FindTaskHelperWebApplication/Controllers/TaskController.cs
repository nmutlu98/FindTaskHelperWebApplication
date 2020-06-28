using FindTaskHelperWebApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FindTaskHelperWebApplication.Models.User;

namespace FindTaskHelperWebApplication.Controllers
{
    public class TaskController : Controller
    {
        NpgsqlConnection _Connection = new DatabaseConnection().GetDatabaseConnection();
        private static int _CurrentTaskId = 0;
        private UserController _UserController = new UserController();
        private HomeController _HomeController = new HomeController();
        

        public int CurrentTaskId
        {
            get
            {
                return _CurrentTaskId;
            }
            set
            {
                _CurrentTaskId = value;
            }
        }

        public TaskController()
        {
            _Connection.Open();
            string sqlQuery = "SELECT id from tasks";
            NpgsqlCommand command = new NpgsqlCommand(sqlQuery, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (_CurrentTaskId < (int)reader[0])
                    _CurrentTaskId = (int)reader[0];
            }
            _CurrentTaskId++;
            _Connection.Close();
        }
        public IActionResult AddNewAd(string username)
        {
            ViewData["username"] = username;
            return View("Views/Task/AddNewAdView.cshtml");
        }
        [HttpPost]
        public IActionResult ProcessNewAdData(AddNewAd task)
        {
            task.CreationDate = DateTime.UtcNow.Date.ToString("dd/MM/yyyy");
            task.PaymentAmountField = task.PaymentAmountField.ToString() + " "+ task.Currency;
            task.LocationField = task.Country + " " + task.State + " " + task.City;
            AddNewTask(task); 
            int taskID = _CurrentTaskId - 1;
            int numberOfRowsAffected = _UserController.AddNewAdToUserWithGivenUsername(task.TaskOwnerUsername, taskID);
            if (numberOfRowsAffected == 0)
            {
                ViewData["username"] = task.TaskOwnerUsername;
                ViewData["message"] = "An error has occured.";
                return AddNewAd(task.TaskOwnerUsername);
            }
            else
            {
                return _HomeController.MyAds(task.TaskOwnerUsername); // will be updated to My ads page when it is ready
            }

        }

        public int AddNewTask(Models.Task task)
        {
            if (task.JobCategory == null)
                task.JobCategory = "";
            return AddNewTask(task.TaskOwnerUsername, task.CreationDate, task.Title, task.Description,
                             task.Location, task.Address, task.TaskPhotoUrl, task.IsAssignedToSomeone,
                             task.AssignedToUserWithThisUsername, task.PaymentAmount, task.IsCompleted, task.JobCategory);
        }

        public int AddNewTask(AddNewAd task)
        {
            if (task.Category.Equals("Other"))
            {
                if (task.OtherCategory != null)
                    task.Category = task.OtherCategory;
            }
            return AddNewTask(task.TaskOwnerUsername, task.CreationDate, task.TitleField, task.DescriptionField,
                             task.LocationField, task.AddressField, task.TaskPhotoUrl, false,
                             "", task.PaymentAmountField, false, task.Category);
        }
        public Models.Task ReturnTaskWithGivenTaskId(int taskId)
        {
            _Connection.Open();

            string query = $"SELECT * FROM tasks WHERE id=\'{taskId}\'";
            NpgsqlCommand command = new NpgsqlCommand(query, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Models.Task ad = new Models.Task(true, taskId, (string)reader[0], ((DateTime)reader[1]).ToString("dd/MM/yyyy"), (string)reader[2],
                            (string)reader[3], (string)reader[4], (string)reader[5], (string)reader[6], (string)reader[12],
                            (bool)reader[7], (string)reader[8], (bool)reader[9], (string)reader[11]);
                _Connection.Close();
                return ad;
            }

            _Connection.Close();
            return null;
        }
        public int UpdateTaskWithGivenTaskId(FindTaskHelperWebApplication.Models.Task task)
        {
            int taskId = task.TaskID;
            _Connection.Open();

            string sqlCommand = $"UPDATE tasks " +
                                $"SET taskownerusername =\'{task.TaskOwnerUsername}\', creationdate =\'{task.CreationDate}\', title =\'{task.Title}\', " +
                                $"description =\'{task.Description}\', location =\'{task.Location}\', address =\'{task.Address}\', taskphotourl =\'{task.TaskPhotoUrl}\', " +
                                $"isassignedtosomeone =\'{task.IsAssignedToSomeone}\', " +
                                $"assignedtouserwithusername =\'{task.AssignedToUserWithThisUsername}\', iscompleted =\'{task.IsCompleted}\', paymentamount =\'{task.PaymentAmount}\', jobcategory=\'{task.JobCategory}\'" +
                                $"WHERE id=\'{taskId}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();

            _Connection.Close();

            return numberOfRowsAffected;
        }
        public IActionResult ListMyAds(string username, string message="")
        {
            int[] ads = _UserController.ReturnUserWithGivenUsername(username).Ads;
            List<Tuple<Models.Task, bool>> adsBelongToCurrentUser = new List<Tuple<Models.Task, bool>>();
            for(int i = 0; i<ads.Length; i++)
            {
                if (ads[i] != -1)
                {
                    Models.Task ad = ReturnTaskWithGivenTaskId(ads[i]);
                    if (ad != null) {
                        bool isTaskerRated = IsTaskerRated(ad.TaskID);
                        int index = ad.PaymentAmount.IndexOf(" ");
                        ad.Currency = ad.PaymentAmount.Substring(index+1);
                        ad.PaymentAmount = ad.PaymentAmount.Substring(0,ad.PaymentAmount.Length-index-1);
                        index = ad.Location.IndexOf(" ");
                        ad.Country = ad.Location.Substring(0, index);
                        int secondIndex = ad.Location.IndexOf(" ", index + 1);
                        ad.State = ad.Location.Substring(index + 1, secondIndex - index);
                        int thirdIndex = ad.Location.IndexOf(" ", secondIndex + 1);
                        ad.City = ad.Location.Substring(secondIndex + 1);
                        adsBelongToCurrentUser.Add(new Tuple<Models.Task, bool>(ad, isTaskerRated));
                    }
                }
            }
            ViewData["message"] = message;
            ViewData["username"] = username;
            ViewData["ads"] = adsBelongToCurrentUser;
            return View("Views/Task/MyAdsView.cshtml");
        }
        public int AssignTaskToUserWithGivenUsername(int taskId, string username)
        {
            _Connection.Open();

            string sqlCommand = $"UPDATE tasks SET isassignedtosomeone={true}, assignedtouserwithusername=\'{username}\' WHERE id=\'{taskId}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();

            _Connection.Close();
            return numberOfRowsAffected;

        }
        public int AssignTaskToUserWithGivenUsername(string username, Models.Task task)
        {
            UserController userController = new UserController();
            User user = userController.ReturnUserWithGivenUsername(username);
            if (user != null)
            {
                int affectedRows = userController.AddNewTaskToUserWithGivenUsername(username, task.TaskID);
                if (affectedRows < 1)
                {
                    Console.WriteLine("There is no user with given username! ");
                    return -1;
                }
                else
                {
                    task.AssignedToUserWithThisUsername = username;
                    task.IsCompleted = true;
                    return UpdateTaskWithGivenTaskId(task);
                }

            }

            return -1;
        }
        public IActionResult ListMyTasks(string username)
        {
            int[] tasks = _UserController.ReturnUserWithGivenUsername(username).Tasks;
            List<Tuple<Models.Task, bool>> tasksBelongToCurrentUser = new List<Tuple<Models.Task, bool>>();
            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i] != -1)
                {
                    Models.Task ad = ReturnTaskWithGivenTaskId(tasks[i]);
                    if (ad != null) 
                    {
                        bool isOwnerRated = IsTaskOwnerRated(ad.TaskID);
                        tasksBelongToCurrentUser.Add(new Tuple<Models.Task, bool>(ad, isOwnerRated));
                    }
                }
            }
            ViewData["username"] = username;
            ViewData["tasks"] = tasksBelongToCurrentUser;
            return View("Views/Task/MyTasks.cshtml");
        }
        
        [HttpPost]
        public IActionResult LoadUpdatedMyAds(Models.Task ad, string message="")
        {
            UpdateTaskWithGivenTaskId(ad);
            ad = ReturnTaskWithGivenTaskId(ad.TaskID);

            if (!ad.IsAssignedToSomeone)
            {
                EmailController emailController = new EmailController();
                string mailBody = emailController.TheJobIsUpdatedMessage(ad.TaskOwnerUsername, ad.TaskID);
                User tasker = _UserController.ReturnUserWithGivenUsername(ad.AssignedToUserWithThisUsername);
                Email email = new Email(tasker.ContactInfo.Email, tasker.Username, "The Task " + ad.Title + " is updated by the owner.", mailBody);
                emailController.SendMail(email);
            }
            
            return _HomeController.MyAds(ad.TaskOwnerUsername, message);
        }
        public int AddNewTask(string ownerUsername, string creationDate, string title, string description,
                             string location, string address, string taskphotourl, bool isAssignedToSomeone,
                             string isAssignedToUserWithUsername, string paymentAmount, bool isCompleted, string jobCategory)
        {
            _Connection.Open();

            string sqlCommand = $"INSERT INTO tasks(taskownerusername, creationdate, title, description," +
                                $"location, address, taskphotourl, isassignedtosomeone, assignedtouserwithusername, iscompleted, id, jobcategory, paymentamount)" +
                                $"VALUES(\'{ownerUsername}\', \'{creationDate}\', \'{title}\', \'{description}\', \'{location}\', \'{address}\', \'{taskphotourl}\'," +
                                $" \'{isAssignedToSomeone}\', \'{isAssignedToUserWithUsername}\', \'{isCompleted}\', \'{_CurrentTaskId}\', \'{jobCategory}\', \'{paymentAmount}\' )";
            int taskId = CurrentTaskId;
            _CurrentTaskId++;
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();
            _Connection.Close();

            _Connection.Open();
            string sqlCommandForRatings = $"INSERT INTO ratings(taskid, taskownerstatus, taskerstatus) VALUES('{taskId}', false, false)";
            command = new NpgsqlCommand(sqlCommandForRatings, _Connection);
            command.ExecuteNonQuery();
            _Connection.Close();

            return numberOfRowsAffected;
        }

        public long CountTasks(bool active=false)
        {
            _Connection.Open();

            string query = "SELECT COUNT(*) from tasks";
            if (active)
                query += " WHERE isassignedtosomeone=\'false\'";
            NpgsqlCommand command = new NpgsqlCommand(query, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                long number = (System.Int64)reader[0];
                _Connection.Close();
                return number;
            }
            _Connection.Close();
            return -1;
        }
        public int DeleteTaskWithGivenTaskId(int taskId)
        {
            _Connection.Open();
            string sqlCommand = $"DELETE FROM tasks WHERE id={taskId}";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();
            _Connection.Close();
            return numberOfRowsAffected;
        }

        public int MarkTaskWithGivenTaskIdAsCompleted(int taskId)
        {
            _Connection.Open();

            string sqlCommand = $"UPDATE tasks SET iscompleted={true} WHERE id=\'{taskId}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();

            _Connection.Close();
            return numberOfRowsAffected;

        }
        public int MarkTaskWithGivenTaskIdAsActive(int taskId)
        {
            _Connection.Open();

            string sqlCommand = $"UPDATE tasks SET isassignedtosomeone={false}, assignedtouserwithusername=\'\' WHERE id=\'{taskId}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();

            _Connection.Close();
            return numberOfRowsAffected;

        }
        
        public List<Tuple<Models.Task,ContactInformationAttribute, double, string>> GetAllActiveAds(string location="", string title = "")
        {
            _Connection.Open();

            string query;
            List<Models.Task> listOfActiveTasks = new List<Models.Task>();
            if (location.Equals("") && title.Equals(""))
                query = "SELECT * from tasks WHERE isassignedtosomeone=false";
            else if (location.Equals("") && !title.Equals(""))
            {
                title = title.ToLower();
                query = $"SELECT * FROM tasks WHERE isassignedtosomeone=false AND LOWER(title) LIKE \'%{title.ToLower()}%\'";
            }
            else if (!location.Equals("") && title.Equals("")) 
                query = $"SELECT * FROM tasks WHERE LOWER(location) LIKE \'%{location.ToLower()}%\' AND isassignedtosomeone=false";
        
            else 
            {
                title = title.ToLower();
                query = $"SELECT * FROM tasks WHERE LOWER(title) LIKE '%{title.ToLower()}%' AND LOWER(location) LIKE \'%{location.ToLower()}%\' AND isassignedtosomeone=false";
            }
            NpgsqlCommand command = new NpgsqlCommand(query, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Models.Task ad = new Models.Task(true, (int)reader[10], (string)reader[0], ((DateTime)reader[1]).ToString("dd/MM/yyyy"),
                (string)reader[2], (string)reader[3], (string)reader[4], (string)reader[5], (string)reader[6], (string)reader[12], (bool)reader[7],
               (string)reader[8], (bool)reader[9], (string)reader[11]);
                listOfActiveTasks.Add(ad);
            }

            _Connection.Close();
            List<Tuple<Models.Task, ContactInformationAttribute, double, string>> activeAdsWithAdOwnerContactInfos = new List<Tuple<Models.Task, ContactInformationAttribute, double, string>>();
            foreach (var ad in listOfActiveTasks)
            {
                string taskOwnerUsername = ad.TaskOwnerUsername;
                User taskOwner = _UserController.ReturnUserWithGivenUsername(taskOwnerUsername);
                activeAdsWithAdOwnerContactInfos.Add(new Tuple<Models.Task, ContactInformationAttribute, double, string>(ad, taskOwner.ContactInfo, taskOwner.Rating, taskOwner.PhotoUrl));
            }
            return activeAdsWithAdOwnerContactInfos;
        }
        public List<Tuple<Models.Task, ContactInformationAttribute, double, string>> ArrangeOrderOfAds(List<Tuple<Models.Task, ContactInformationAttribute, double, string>> activeAds, string[] preferredTypes)
        {
            int start = 0;
            int end = activeAds.Count - 1;
            Tuple<Models.Task, ContactInformationAttribute, double, string>[] ads = activeAds.ToArray();
            while (start < end)
            {
                while (preferredTypes.Contains(ads[start].Item1.JobCategory) && start < end)
                {
                    start++;
                }
                while (!preferredTypes.Contains(ads[end].Item1.JobCategory) && start < end)
                {
                    end--;
                }
                if(start < end)
                {
                    Tuple<Models.Task, ContactInformationAttribute, double, string> temp = ads[start];
                    ads[start] = ads[end];
                    ads[end] = temp;
                    start++;
                    end--;
                }


            }
            return ads.ToList();
        }
        public bool IsTaskerRated(int taskId)
        {
            _Connection.Open();
            string sqlQuery = $"SELECT taskerstatus FROM ratings WHERE taskid='{taskId}'";
            NpgsqlCommand command = new NpgsqlCommand(sqlQuery, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                bool res = (bool)reader[0];
                _Connection.Close();
                return res;
            }
            _Connection.Close();
            return false;
        }
        public bool IsTaskOwnerRated(int taskId)
        {
            _Connection.Open();
            string sqlQuery = $"SELECT taskownerstatus FROM ratings WHERE taskid='{taskId}'";
            NpgsqlCommand command = new NpgsqlCommand(sqlQuery, _Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) { 
                bool res = (bool)reader[0];
                _Connection.Close();
                return res;
            }
            _Connection.Close();
            return false;
        }
    }
}
