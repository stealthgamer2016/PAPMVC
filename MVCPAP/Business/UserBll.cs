using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MVCPAP.Models;

namespace MVCPAP.Business
{
    public class UserBll
    {
        public Models.User GetUserById(string userId)
        {
            int discriminator = int.Parse(userId.Split('-')[0]);
            string username = userId.Split('-')[1];

            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
          "SELECT * FROM [User] Where username='" + username + "' And discriminator=" + discriminator, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count <= 0)
            {
                return null;
            }

            Models.User user = new Models.User();

            if (dataSet.Tables.Count <= 0)
                return null;

            if (dataSet.Tables[0].Rows.Count <= 0)
                return null;

            DataRow row = dataSet.Tables[0].Rows[0];


            user.email = row["email"].ToString();
            user.username = username;
            user.discriminator = discriminator;
            user.profilePicture = row["profilePicture"].ToString();
            user.password = row["password"].ToString();
            user.createdAt = DateTime.Parse(row["createdAt"].ToString());
            user.Followers = GetFollowersByUserId(user.discriminator + "-" + user.username);
            user.Following = GetFollowingByUserId(user.discriminator + "-" + user.username);


            return user;
        }

        public List<Models.User> GetFollowersByUserId(string userId)
        {
            int discriminator = int.Parse(userId.Split('-')[0]);
            string username = userId.Split('-')[1];

            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
          "SELECT * FROM Follow Where followedUsername='" + username + "' And followedDiscriminator=" + discriminator, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            List<Models.User> users = new List<Models.User>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Models.User user = new Models.User();
                user.discriminator = int.Parse(row["followerDiscriminator"].ToString());
                user.username = row["followerUsername"].ToString();

                users.Add(user);
            }

            return users;

        }

        public List<Models.User> GetFollowingByUserId(string userId)
        {
            int discriminator = int.Parse(userId.Split('-')[0]);
            string username = userId.Split('-')[1];

            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
          "SELECT * FROM Follow Where followerUsername='" + username + "' And followerDiscriminator=" + discriminator, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            List<Models.User> users = new List<Models.User>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Models.User user = new Models.User();
                user.discriminator = int.Parse(row["followedDiscriminator"].ToString());
                user.username = row["followedUsername"].ToString();

                users.Add(user);
            }

            return users;

        }

        public string CreateUser(Models.User user)
        {

            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT [User](discriminator,username,email,password,profilePicture,createdAt,firstName,Lastname)" +
                " Values (@discriminator,@username,@email,@password,@profilePicture,@date,@firstName,@lastName)";
            command.Parameters.AddWithValue("discriminator", user.discriminator);
            command.Parameters.AddWithValue("username", user.username);
            command.Parameters.AddWithValue("email", user.email);
            command.Parameters.AddWithValue("password", user.password);
            command.Parameters.AddWithValue("profilePicture", "none");
            command.Parameters.AddWithValue("date", DateTime.Now);
            command.Parameters.AddWithValue("firstName", "none");
            command.Parameters.AddWithValue("lastName", "none");
            try
            {
                connection.Open();
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return user.discriminator + "-" + user.username;

        }

        public string UpdateUsernameByUser(Models.User user, string NewUsername)
        {
            Models.User user1 = GetUserById(user.discriminator + "-" + NewUsername);

            if (user1 != null)// se ja existir um user com esse username. nao mudar o username e devolver o username antigo
            {
                return user.discriminator + "-" + user.username;
            }

            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "Update [User] Set username= @new Where username= @username And discriminator=@discriminator";
            command.Parameters.AddWithValue("discriminator", user.discriminator);
            command.Parameters.AddWithValue("username", user.username);
            command.Parameters.AddWithValue("new", NewUsername);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            user.username = NewUsername;
            return user.discriminator + "-" + user.username;
        }

        public List<Models.User> GetAllUsersLogin()
        {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
          "SELECT discriminator,username,email,password FROM [User]", connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            if (dataSet.Tables.Count <= 0)
                return null;

            if (dataSet.Tables[0].Rows.Count <= 0)
            {
                return null;
            }

            List<Models.User> users = new List<Models.User>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Models.User user = new Models.User();
                user.email = row["email"].ToString();
                user.username = row["username"].ToString();
                user.discriminator = int.Parse(row["discriminator"].ToString());
                user.password = row["password"].ToString();
                users.Add(user);
            }

            return users;
        }

        public Models.User SignInUser(Models.User user)
        {
            user.Valid = false;

            if (user.email == null || user.username == null || user.password == null)
            {
                user.LoginErrorMessage = "Missing credentials";
                return user;
            }

            List<Models.User> users = GetAllUsersLogin();

            //verify email
            List<Models.User> VerifyEmail = users.Where(x => x.email == user.email).ToList();
            if (VerifyEmail.Count>=1)
            {
                user.LoginErrorMessage = "Email already in use";
                return user;
            }

            //set unique dicriminator
            var exclude = new HashSet<int>() { 0 };
            foreach (var _user in users.Where(x => x.username == user.username))
            {
                exclude.Add(_user.discriminator);
            }
            var range = Enumerable.Range(1, 9999).Where(i => !exclude.Contains(i));

            var rand = new System.Random();
            int index = rand.Next(0, 9999 - exclude.Count);
            user.discriminator = range.ElementAt(index);

            //verify username
            List<Models.User> VerifyUsername = users.Where(x => x.username == user.username && x.discriminator == user.discriminator).ToList();
            if (VerifyUsername.Count>=1)
            {
                user.LoginErrorMessage = "Username already in use";
                return user;
            }

            CreateUser(user);
            user.Valid = true;


            return user;
        }

        public Models.User LogInUser(Models.User user)
        {
            user.Valid = false;

            if (user.email == null || user.password == null)
            {
                user.LoginErrorMessage = "Missing credentials";
                return user;
            }

            List<Models.User> users = GetAllUsersLogin();

            //verify user
            List<Models.User> Verify = users.Where(x => x.email == user.email && x.password == user.password).ToList();
            if (Verify.Count != 1)
            {
                user.LoginErrorMessage = "Incorect email or password";
                return user;
            }
            Verify[0].Valid = true;

            return Verify[0];
        }
    }
}