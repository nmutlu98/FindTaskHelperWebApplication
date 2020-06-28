using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FindTaskHelperWebApplication.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;

namespace FindTaskHelperWebApplication.Controllers
{
    public class UserController : Controller
    {
        private NpgsqlConnection _Connection = new DatabaseConnection().GetDatabaseConnection();

        public int AddNewUser(string username, string password, string name, string surname, User.ContactInformationAttribute contactInfo)
        {
            _Connection.Open();

            string queryString = $"INSERT INTO users (username, name, surname, email, phone, address, tasks, ads, rating, photo, password, ratingnumber, jobtypes)"
                + $"VALUES(\'{username}\',\'{name}\',\'{surname}\',\'{contactInfo.Email}\',\'{contactInfo.PhoneNumber}\',\'{contactInfo.Address}\'," + "\'{-1}\' , \'{-1}\'" + $", 5.0,\'-1\',\'{password}\', 1, ARRAY[\'-1\'])";
            NpgsqlCommand command = new NpgsqlCommand(queryString, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();

            _Connection.Close();
            return numberOfRowsAffected;

        }
        public User ReturnUserWithGivenUsername(string username)
        {
            _Connection.Open();
            string queryString = $"SELECT * FROM users WHERE username=\'{username}\'";
            NpgsqlCommand command = new NpgsqlCommand(queryString, _Connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                User userWithUsername = new User(dataReader[0].ToString(), dataReader[10].ToString(), dataReader[1].ToString(), dataReader[2].ToString(),
                            new User.ContactInformationAttribute(dataReader[3].ToString(), dataReader[4].ToString(), dataReader[5].ToString()),
                            (int[])dataReader[6], (int[])dataReader[7], (double)dataReader[8], dataReader[9].ToString(), (int)dataReader[11], (string[])dataReader[12]);
                _Connection.Close();
                return userWithUsername;

            }
            _Connection.Close();
            return null;
        }
        public User ReturnUserWithGivenEmail(string email)
        {
            _Connection.Open();
            string queryString = $"SELECT * FROM users WHERE email=\'{email}\'";
            NpgsqlCommand command = new NpgsqlCommand(queryString, _Connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                User user = new User(dataReader[0].ToString(), dataReader[10].ToString(), dataReader[1].ToString(), dataReader[2].ToString(),
                            new User.ContactInformationAttribute(dataReader[3].ToString(), dataReader[4].ToString(), dataReader[5].ToString()),
                            (int[])dataReader[6], (int[])dataReader[7], (double)dataReader[8], dataReader[9].ToString(), (int)dataReader[11], (string[])dataReader[12]);
                _Connection.Close();
                return user;

            }
            _Connection.Close();
            return null;
        }
        public int UpdateUserInformationWithGivenUsername(string username, User user)
        {
            _Connection.Open();
            string query = $"UPDATE users SET username=\'{user.Username}\', name=\'{user.Name}\', surname=\'{user.Surname}\'," +
                           $"email=\'{user.ContactInfo.Email}\', phone=\'{user.ContactInfo.PhoneNumber}\', address=\'{user.ContactInfo.Address}\'," +
                           $"rating=\'{user.Rating}\', photo=\'{user.PhotoUrl}\', password=\'{user.Password}\', ratingnumber={user.RatingNumber} WHERE username=\'{username}\'";
            NpgsqlCommand command = new NpgsqlCommand(query, _Connection);
            int numberOfAffectedRows = command.ExecuteNonQuery();
            _Connection.Close();
            return numberOfAffectedRows;

        }

        public int AddNewAdToUserWithGivenUsername(string username, int taskID)
        {
            _Connection.Open();

            string sqlCommand = $"UPDATE users SET ads=array_append(ads, \'{taskID}\') WHERE username=\'{username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();

            _Connection.Close();
            return numberOfRowsAffected;

        }
        public int RemoveAdFromUserWithGivenUsername(string username, int taskID)
        {
            _Connection.Open();

            string sqlCommand = $"UPDATE users SET ads=array_remove(ads, \'{taskID}\') WHERE username=\'{username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();

            _Connection.Close();
            return numberOfRowsAffected;

        }
        public int AddNewTaskToUserWithGivenUsername(string username, int taskID)
        {
            _Connection.Open();

            string sqlCommand = $"UPDATE users SET tasks=array_append(tasks, \'{taskID}\') WHERE username=\'{username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();

            _Connection.Close();
            return numberOfRowsAffected;
        }
        public int RemoveTaskFromUserWithGivenUsername(string username, int taskID)
        {
            _Connection.Open();

            string sqlCommand = $"UPDATE users SET tasks=array_remove(tasks, \'{taskID}\') WHERE username=\'{username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            int numberOfRowsAffected = command.ExecuteNonQuery();

            _Connection.Close();
            return numberOfRowsAffected;
        }

        public void UpdateProfileData(string username, string name, string surname,
                            string email, string phone, string address, string photoUrl){
            
            _Connection.Open();

            string sqlCommand = $"UPDATE users SET name=\'{name}\', surname=\'{surname}\'," +
                $"email=\'{email}\', phone=\'{phone}\', address=\'{address}\', photo=\'{photoUrl}\' WHERE username=\'{username}\'";
            NpgsqlCommand command = new NpgsqlCommand(sqlCommand, _Connection);
            command.ExecuteNonQuery();

            _Connection.Close();

        }

        public int UpdateUserRating(double rate, string username)
        {
            User user = ReturnUserWithGivenUsername(username);
            double newRate = Math.Ceiling((user.Rating * user.RatingNumber + rate) / (user.RatingNumber + 1));
            user.Rating = newRate;
            user.RatingNumber++;
            return UpdateUserInformationWithGivenUsername(user.Username, user);
        }

        public int DeleteUserAccountWithGivenUsername(string username)
        {
            _Connection.Open();
            string commandString = $"DELETE FROM users WHERE username=\'{username}\'";
            Console.Write("String:"+commandString);
            NpgsqlCommand command = new NpgsqlCommand(commandString, _Connection);
            int numberOfAffectedRows = command.ExecuteNonQuery();
            _Connection.Close();
            return numberOfAffectedRows;

        }
        public long CountUsers()
        {
            _Connection.Open();

            string query = "SELECT COUNT(*) from users";
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
        public void AddPreferredTaskTypes(string username, string[] preferredTypes)
        {
            _Connection.Open();
            for(int i = 0; i<preferredTypes.Length; i++)
            {
                string commandToAppend = $"UPDATE users SET jobtypes=array_append(jobtypes,\'{preferredTypes[i]}\') WHERE username=\'{username}\'";
                NpgsqlCommand command = new NpgsqlCommand(commandToAppend, _Connection);
                command.ExecuteNonQuery();
            }
            _Connection.Close();
        }
    }
}
