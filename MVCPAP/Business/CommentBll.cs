using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MVCPAP.Business
{
    public class CommentBll
    {
        public List<Models.Comment> GetCommentsByVideoId(int id) {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlDataAdapter dataAdapter2 = new SqlDataAdapter(
               "SELECT * FROM Comment Where video=" + id
/*               + " Order By upvotes Desc"*/

               , connection);
            DataSet dataSet2 = new DataSet();
            dataAdapter2.Fill(dataSet2);

            List<Models.Comment> Comments = new List<Models.Comment>();

            foreach (DataRow row in dataSet2.Tables[0].Rows)
            {
                Models.Video video = new Models.Video();
                video.id = int.Parse(row["video"].ToString());

                Models.User user = new Models.User();
                video.username = row["username"].ToString();
                video.discriminator = int.Parse(row["discriminator"].ToString());


                Models.Comment comment = new Models.Comment();
                comment.id = int.Parse(row["id"].ToString());
                comment.comment = row["comment"].ToString();
                comment.video = video;
                comment.user = user;
                comment.username = row["username"].ToString();
                comment.discriminator = int.Parse(row["discriminator"].ToString());
                //comment.createdAt = DateTime.Parse(row["createdAt"].ToString());
                Comments.Add(comment);
            }
            return Comments;
        }

        public int CreateComment(int videoId,string userId,string text) {
            int discriminator = int.Parse(userId.Split('-')[0]);
            string username = userId.Split('-')[1];

            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            //SqlCommand command = new SqlCommand();
            //command.Connection = connection;
            //command.CommandType = CommandType.Text;
            //command.CommandText = "INSERT Comment(comment,username,discriminator,createdAt,video) Values (@text,@username,@discriminator,@date,@video)";
            //command.Parameters.AddWithValue("text", text);
            //command.Parameters.AddWithValue("username", username);
            //command.Parameters.AddWithValue("discriminator", discriminator);
            //command.Parameters.AddWithValue("date", DateTime.Now);
            //command.Parameters.AddWithValue("video", videoId);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
            "INSERT Comment(comment,username,discriminator,video) Output inserted.id Values ('"+text+"','"+username+"',"+discriminator+","+videoId+")", connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            DataRow row = dataSet.Tables[0].Rows[0];

            return int.Parse(row["id"].ToString());



            //try
            //{
            //connection.Open();
            //command.ExecuteNonQuery();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //connection.Close();
            //}


        }

        public void DeleteCommentById(int id) {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "Delete Comment Where id=@id";
            command.Parameters.AddWithValue("id", id);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void EditCommentById(int id, string text) {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "Update Comment Set comment=@text Where id=@id";
            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("text", text);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        public Models.Comment GetCommentById(int id) {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.Connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(
            "SELECT * FROM Comment Where id=" + id, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);


            if (dataSet.Tables.Count <= 0)
                return null;

            if (dataSet.Tables[0].Rows.Count <= 0)
                return null;

            DataRow row = dataSet.Tables[0].Rows[0];

            Models.User user = new Models.User();
            user.username= row["username"].ToString();
            user.discriminator= int.Parse(row["discriminator"].ToString());


            Models.Comment comment = new Models.Comment();
            comment.id = int.Parse(row["id"].ToString());
            comment.comment = row["comment"].ToString();
            comment.user = user;
            comment.username = row["username"].ToString();
            comment.discriminator = int.Parse(row["discriminator"].ToString());
            //comment.createdAt = DateTime.Parse(row["createdAt"].ToString());

            return comment;
        }
    }
}