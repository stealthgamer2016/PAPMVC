using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MVCPAP.Models;

namespace MVCPAP.Business
{
    public class VideoBll
    {// o video data tem join de outras tabelas(como os upvotes)

        public Models.Video GetVideoById(int id)
        {

            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(
         "SELECT Top(1) * FROM Video Where id=" + id, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            if (dataSet.Tables.Count <= 0)
                return null;

            if (dataSet.Tables[0].Rows.Count <= 0)
                return null;

            DataRow row = dataSet.Tables[0].Rows[0];

            UserBll userBll = new UserBll();
            CommentBll commentBll = new CommentBll();

            Models.Video video = new Models.Video();
            video.id = id;
            video.title = row["title"].ToString();
            video.description = row["description"].ToString();
            video.username = row["username"].ToString();
            video.discriminator = int.Parse(row["discriminator"].ToString());
            video.videoFile = row["videoFile"].ToString();
            video.thumbnail = row["thumbnail"].ToString();
            video.publishedAt = DateTime.Parse(row["publishedAt"].ToString());
            video.User = userBll.GetUserById(video.discriminator+"-"+video.username);
            video.comments = commentBll.GetCommentsByVideoId(video.id);
            video.upvotes = GetVideoUpvotesByVideoId(video.id);
            video.views = GetVideoViewsByVideoId(video.id);

            return video;
        }

        public List<Models.Video> GetVideosSearchByTitle(string search)
        {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
           "SELECT * FROM Video Where lower(title) Like '%" + search + "%'"
           + "Order By (Case When lower(title)='" + search + "' Then 1 When lower(title)='" + search + "%' Then 2 Else 3 End)", connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            List<Models.Video> Videos = new List<Models.Video>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Models.Video video = new Models.Video();
                video.id = int.Parse(row["id"].ToString());
                video.title = row["title"].ToString();
                video.description = row["description"].ToString();
                video.username = row["username"].ToString();
                video.discriminator = int.Parse(row["discriminator"].ToString());
                video.videoFile = row["videoFile"].ToString();
                video.thumbnail = row["thumbnail"].ToString();
                video.publishedAt = DateTime.Parse(row["publishedAt"].ToString());
                Videos.Add(video);
            }

            return Videos;
        }

        public List<Models.Video> GetVideosByUserId(string userId)
        {
            int discriminator = int.Parse(userId.Split('-')[0]);
            string username = userId.Split('-')[1];

            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
           "SELECT * FROM Video Where username='" + username + "' And discriminator='" + discriminator + "'", connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            List<Models.Video> Videos = new List<Models.Video>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Models.Video video = new Models.Video();

                video.id = int.Parse(row["id"].ToString());
                video.title = row["title"].ToString();
                video.description = row["description"].ToString();
                video.username = row["username"].ToString();
                video.discriminator = int.Parse(row["discriminator"].ToString());
                video.videoFile = row["videoFile"].ToString();
                video.thumbnail = row["thumbnail"].ToString();
                video.publishedAt = DateTime.Parse(row["publishedAt"].ToString());

                Videos.Add(video);
            }

            return Videos;
        }

        public Models.VideoData GetVideoDataById(int id)
        {

            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(
         " SELECT Top(1) " +
         " v.id, v.title, v.description, v.username, v.discriminator, v.videoFile, v.thumbnail, v.publishedAt, COUNT(vu.username)'upVotes' " +
         " FROM Video v " +
         " left Join VideoUpvotes vu on vu.video = v.id " +
         " Where v.id = " + id +
         " Group by v.id, v.title, v.description, v.username, v.discriminator, v.videoFile, v.thumbnail, v.publishedAt ", connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            if (dataSet.Tables.Count <= 0)
                return null;

            if (dataSet.Tables[0].Rows.Count <= 0)
                return null;

            DataRow row = dataSet.Tables[0].Rows[0];

            Models.VideoData video = new Models.VideoData();
            video.id = id;
            video.title = row["title"].ToString();
            video.description = row["description"].ToString();
            video.username = row["username"].ToString();
            video.discriminator = int.Parse(row["discriminator"].ToString());
            video.videoFile = row["videoFile"].ToString();
            video.thumbnail = row["thumbnail"].ToString();
            video.publishedAt = DateTime.Parse(row["publishedAt"].ToString());
            video.upVotes = int.Parse(row["upVotes"].ToString());

            return video;
        }

        public List<Models.User> GetVideoUpvotesByVideoId(int id)
        {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
           "Select * " +
           " From VideoUpvotes" +
           " Where video = "+id, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            List<Models.User> users = new List<Models.User>();
            UserBll userBll = new UserBll();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Models.User user = new Models.User();

                user.username = row["username"].ToString();
                user.discriminator = int.Parse(row["discriminator"].ToString());
                //user = userBll.GetUserById(user.discriminator+"-"+user.username);

                users.Add(user);
            }

            return users;
        }

        public List<Models.User> GetVideoViewsByVideoId(int id)
        {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
           "Select * " +
           " From VideoView" +
           " Where video = " + id, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            List<Models.User> users = new List<Models.User>();
            UserBll userBll = new UserBll();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Models.User user = new Models.User();

                user.username = row["username"].ToString();
                user.discriminator = int.Parse(row["discriminator"].ToString());
                //user = userBll.GetUserById(user.discriminator + "-" + user.username);

                users.Add(user);
            }

            return users;
        }

        public Models.Video CreateVideo(Models.User user, Models.Video video)
        {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT Video(title,description,username,discriminator,publishedAt,videoFile,thumbnail) Values (@title,@description,@username,@disc,@date,@file,@thumbnail)";
            command.Parameters.AddWithValue("title", video.title);
            command.Parameters.AddWithValue("username", user.username);
            command.Parameters.AddWithValue("disc", user.discriminator);
            command.Parameters.AddWithValue("date", DateTime.Now);
            command.Parameters.AddWithValue("file", video.videoFile);
            command.Parameters.AddWithValue("thumbnail", video.thumbnail);
            command.Parameters.AddWithValue("description", video.description);
            //connection.Open();
            //command.ExecuteNonQuery();

            //connection.Close();

            string com= "INSERT Video(title,description,username,discriminator,publishedAt,videoFile,thumbnail) Output inserted.id Values (@title,@description,@username,@disc,@date,@file,@thumbnail)";
            //com = com.Replace("@title", video.title);
            //com = com.Replace("@username", user.username);
            //com = com.Replace("@disc", user.discriminator.ToString());
            //com = com.Replace("@date", DateTime.Now.ToString());
            //com = com.Replace("@file", video.videoFile);
            //com = com.Replace("@thumbnail", video.thumbnail);
            //com = com.Replace("@description", video.description);


            SqlDataAdapter dataAdapter = new SqlDataAdapter(com, connection);
            //dataAdapter.InsertCommand = command;
            //dataAdapter.InsertCommand.CommandText = com;
            //dataAdapter.InsertCommand.CommandType = CommandType.Text;
            dataAdapter.SelectCommand.Parameters.AddWithValue("title", video.title);
            dataAdapter.SelectCommand.Parameters.AddWithValue("username", user.username);
            dataAdapter.SelectCommand.Parameters.AddWithValue("disc", user.discriminator);
            dataAdapter.SelectCommand.Parameters.AddWithValue("date", DateTime.Now);
            dataAdapter.SelectCommand.Parameters.AddWithValue("file", video.videoFile);
            dataAdapter.SelectCommand.Parameters.AddWithValue("thumbnail", video.thumbnail);
            dataAdapter.SelectCommand.Parameters.AddWithValue("description", video.description);
            //dataAdapter.InsertCommand.Connection = connection;
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            DataRow row = dataSet.Tables[0].Rows[0];

            video.id= int.Parse(row["id"].ToString());


            return video;
        }

        public void CreateVideoUpvote(Models.User user, Models.Video video)
        {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT VideoUpvotes(username,discriminator,video) Values (@username,@discriminator,@video)";
            command.Parameters.AddWithValue("video", video.id);
            command.Parameters.AddWithValue("username", user.username);
            command.Parameters.AddWithValue("discriminator", user.discriminator);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void DeleteVideoUpvote(Models.User user, Models.Video video) {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "Delete VideoUpvotes Where video=@video And username=@username And discriminator=@discriminator";
            command.Parameters.AddWithValue("video", video.id);
            command.Parameters.AddWithValue("username", user.username);
            command.Parameters.AddWithValue("discriminator", user.discriminator);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void CreateVideoView(Models.User user, Models.Video video) {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT VideoView(video,username,discriminator) Values (@video,@username,@disc)";
            command.Parameters.AddWithValue("video", video.id);
            command.Parameters.AddWithValue("username", user.username);
            command.Parameters.AddWithValue("disc", user.discriminator);
            connection.Open();
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}