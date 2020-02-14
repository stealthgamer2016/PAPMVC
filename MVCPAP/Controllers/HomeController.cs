using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPAP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Search(FormCollection collection, string id)
        {
            ViewBag.Title = "Search for " + id;
            ViewBag.Search = id;


            SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=PAP1;Integrated Security=True");

            DataTable dt_Query = new DataTable();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "usp_GetVideosSearch";
            command.Parameters.AddWithValue("amount",3);
            if (id is null)
                id = "ok";

            command.Parameters.AddWithValue("query", id);
            string txt="";
            try
            {
                connection.Open();
                using (SqlDataReader rdr = command.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        txt = rdr["username"].ToString();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                connection.Close();
            }

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
           "SELECT * FROM Video", connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);


            ViewBag.Data = "Data:"+txt;

            List<Video> Videos = new List<Video>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Video V = new Video();
                V.title = row["title"].ToString();
                V.username = row["username"].ToString();
                V.discriminator = int.Parse(row["discriminator"].ToString());
                V.videoFile = row["videoFile"].ToString();
                V.thumbnail = row["thumbnail"].ToString();
                Videos.Add(V);
            }

            ViewBag.Videos = Videos;

            return View();
        }

        public class Video
        {
            public String title { get; set; }
            public String username { get; set; }
            public int discriminator { get; set; }
            public String videoFile { get; set; }
            public String thumbnail { get; set; }
        }
    }
}