using MVCPAP.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCPAP.Models;

namespace MVCPAP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            VideoBll videoBll = new VideoBll();

            List<Models.Video> videos = new List<Models.Video>();

            if (Session["userId"] != null)
            {
                string usernameString = Session["userId"].ToString();

                User user = new User();
                user.username = usernameString.Split('-')[1];
                user.discriminator = int.Parse(usernameString.Split('-')[0]);

                videos = videoBll.GetRecomendedVideos(user);
            }


            return View(videos);
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

        public ActionResult profile(string id)
        {
            if (!id.Contains("-"))
                Response.Redirect("PageNotFound...");
            int discriminator = int.Parse(id.Split('-')[0]);
            string username = id.Split('-')[1];
            //  SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=PAP1;Integrated Security=True");

            //  SqlDataAdapter dataAdapter = new SqlDataAdapter(
            //"SELECT * FROM [User] Where username='" + username + "' And discriminator=" + discriminator, connection);
            //  DataSet dataSet = new DataSet();
            //  dataAdapter.Fill(dataSet);

            //  if(dataSet.Tables[0].Rows.Count<=0)
            //  {
            //      throw new Exception("User not found");
            //  }

            //  _User user = new _User();
            //  user.email = dataSet.Tables[0].Rows[0]["email"].ToString();
            //  user.username = username;
            //  user.discriminator = discriminator;
            //  user.profilePicture = dataSet.Tables[0].Rows[0]["profilePicture"].ToString();
            //  user.password = dataSet.Tables[0].Rows[0]["password"].ToString();

            UserBll userBll = new UserBll();
            try
            {
                ViewBag.user = userBll.GetUserById(id);
            }
            catch (Exception ex)
            {
                return RedirectToAction("NotFound", "Home", new { message = ex.Message });
            }


            // SqlDataAdapter dataAdapter2 = new SqlDataAdapter(
            //"SELECT * FROM Video Where username='" + username + "' And discriminator='" + discriminator + "'", connection);
            // DataSet dataSet2 = new DataSet();
            // dataAdapter2.Fill(dataSet2);

            // List<_Video> Videos = new List<_Video>();

            // foreach (DataRow row in dataSet2.Tables[0].Rows)
            // {
            //     _Video V = new _Video();
            //     V.id = int.Parse(row["id"].ToString());
            //     V.title = row["title"].ToString();
            //     V.username = row["username"].ToString();
            //     V.discriminator = int.Parse(row["discriminator"].ToString());
            //     V.videoFile = row["videoFile"].ToString();
            //     V.thumbnail = row["thumbnail"].ToString();
            //     Videos.Add(V);
            // }
            VideoBll videoBll = new VideoBll();
            ViewBag.Videos = videoBll.GetVideosByUserId(id);

            return View();
        }

        public ActionResult Search(FormCollection collection, string id)
        {

            if (Request.QueryString.Get("page") != null)
                ViewBag.page = int.Parse(Request.QueryString.Get("page"));

            ViewBag.Title = "Search for " + id;
            ViewBag.Search = id;
            id = id.ToLower();
            // SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=PAP1;Integrated Security=True");

            // SqlDataAdapter dataAdapter = new SqlDataAdapter(
            //"SELECT * FROM Video Where lower(title) Like '%" + id + "%'"
            //+ "Order By (Case When lower(title)='" + id + "' Then 1 When lower(title)='" + id + "%' Then 2 Else 3 End)"

            //, connection);
            // DataSet dataSet = new DataSet();
            // dataAdapter.Fill(dataSet);

            // List<_Video> Videos = new List<_Video>();

            // foreach (DataRow row in dataSet.Tables[0].Rows)
            // {
            //     _Video V = new _Video();
            //     V.id = int.Parse(row["id"].ToString());
            //     V.title = row["title"].ToString();
            //     V.username = row["username"].ToString();
            //     V.discriminator = int.Parse(row["discriminator"].ToString());
            //     V.videoFile = row["videoFile"].ToString();
            //     V.thumbnail = row["thumbnail"].ToString();
            //     Videos.Add(V);
            // }


            VideoBll videoBll = new VideoBll();
            ViewBag.Videos = videoBll.GetVideosSearchByTitle(id);

            return View();
        }

        public ActionResult NotFound(string id)
        {
            ViewBag.title = "Page not found";
            ViewBag.info = id;
            return View();
        }

        public ActionResult Video(int id)
        {
            ViewBag.id = id;
            VideoBll videoBll = new VideoBll();
            Models.VideoData video = videoBll.GetVideoDataById(id);

            if (id == null || id < 0 || video is null)
            {
                return RedirectToAction("NotFound", "Home", new { message = "Could not find that video" });
            }

            ViewBag.loggedin = 0;
            string usernameString = "0000-user";
            if (Session["userId"] != null)
            {
                usernameString = Session["userId"].ToString();
                ViewBag.loggedin = 1;
            }

            int discriminator = int.Parse(usernameString.Split('-')[0]);
            string username = usernameString.Split('-')[1];

            if (video.upvotes.Where(x => x.username == username && x.discriminator == discriminator).Count() >= 1)
            {
                video.Upvoted = true;
                ViewBag.upvoted = 1;
            }
            else
            {
                video.Upvoted = false;
                ViewBag.upvoted = 0;
            }




            //CommentBll commentBll = new CommentBll();
            //List<Models.Comment> comments = commentBll.GetCommentsByVideoId(int.Parse(id));
            //ViewBag.Comments = commentBll.GetCommentsByVideoId(int.Parse(id));

            //Models.VideoComments videoComments = new Models.VideoComments();
            //videoComments.video = video;
            //videoComments.comments = comments;



            return View(video);
        }

        public ActionResult UpdateComments(string id)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=PAP1;Integrated Security=True");
            SqlDataAdapter dataAdapter2 = new SqlDataAdapter("SELECT * FROM Comment Where video=" + id + " Order By createdAt", connection);
            DataSet dataSet2 = new DataSet();
            dataAdapter2.Fill(dataSet2);

            List<_Comment> Comments = new List<_Comment>();

            foreach (DataRow row2 in dataSet2.Tables[0].Rows)
            {
                _Comment C = new _Comment();
                C.id = int.Parse(row2["id"].ToString());
                C.comment = row2["comment"].ToString();
                C.username = row2["username"].ToString();
                C.discriminator = int.Parse(row2["discriminator"].ToString());
                C.createdAt = DateTime.Parse(row2["createdAt"].ToString());
                Comments.Add(C);
            }

            return PartialView("VideoComments", Comments);
        }

        //[HttpPost]
        public ActionResult PostComment(int id, string id2)
        {
            if (Session["userId"] == null)
                throw new Exception("uiuiui");
            //return new ContentResult() { Content = "Not Logged in" };

            string usernameString = Session["userId"].ToString();
            //String text = comment.comment;
            int discriminator = int.Parse(usernameString.Split('-')[0]);
            string username = usernameString.Split('-')[1];

            //int id = comment.video;

            CommentBll commentBll = new CommentBll();
            int CommentId = commentBll.CreateComment(id, usernameString, id2);
            List<MVCPAP.Models.Comment> comments = commentBll.GetCommentsByVideoId(id);

            VideoBll videoBll = new VideoBll();
            //return View("Video", videoBll.GetVideoDataById(id));
            return new ContentResult() { Content = CommentId.ToString() };

        }


        [HttpPost]
        public ActionResult DeleteComment(int id)
        {


            if (Session["userId"] == null)
                //return new ContentResult() { Content = "Not Logged in" };
                throw new Exception("Not loged in");

            CommentBll commentBll = new CommentBll();
            //List<Models.Comment> comments = commentBll.GetCommentsByVideoId(id2);

            //Models.Comment comment = comments.Where(x => x.id == id).FirstOrDefault();

            Models.Comment comment = commentBll.GetCommentById(id);

            if (comment == null)
                //return new ContentResult() { Content = "Comment not found" };
                throw new Exception("Comment does not exist");

            string usernameString = Session["userId"].ToString();
            //String text = Request.QueryString.Get("text");

            int discriminator = int.Parse(usernameString.Split('-')[0]);
            string username = usernameString.Split('-')[1];

            if (comment.user.username != username || comment.user.discriminator != discriminator)
                //return new ContentResult() { Content = "User does not has permission" };
                throw new Exception("Not your comment");

            commentBll.DeleteCommentById(id);

            //comments.Remove(comment);

            return new ContentResult() { Content = "Done" };
            //return PartialView("CommentSection", comments);
            //return RedirectToAction("Video", "Home", id);
        }

        public ActionResult EditComment(int id, string id2)
        {
            CommentBll commentBll = new CommentBll();

            if (Session["userId"] == null)
                return new ContentResult() { Content = "false" };

            Models.Comment comment = commentBll.GetCommentById(id);

            if (comment == null)
                return new ContentResult() { Content = "false" };

            string usernameString = Session["userId"].ToString();

            int discriminator = int.Parse(usernameString.Split('-')[0]);
            string username = usernameString.Split('-')[1];

            if (comment.username != username || comment.discriminator != discriminator)
                return new ContentResult() { Content = "false" };

            commentBll.EditCommentById(id, id2);

            return new ContentResult() { Content = "true" };
        }

        public ActionResult UpvoteVideo(int id)
        {

            if (Session["userId"] is null)
            {
                return new ContentResult() { Content = "Not logged in" };
            }
            Models.User user = new Models.User();
            string usernameString = Session["userId"].ToString();
            user.discriminator = int.Parse(usernameString.Split('-')[0]);
            user.username = usernameString.Split('-')[1];

            Models.Video video = new Models.Video();
            video.id = id;

            VideoBll videoBll = new VideoBll();

            if (video.upvotes.Where(x => x.username == user.username && x.discriminator == user.discriminator).Count() == 0)
                videoBll.CreateVideoUpvote(user, video);
            else
                return new ContentResult() { Content = "false" };
            return new ContentResult() { Content = "true" };
        }

        public ActionResult DownvoteVideo(int id)
        {
            if (Session["userId"] is null)
            {
                return new ContentResult() { Content = "Not logged in" };
            }
            Models.User user = new Models.User();
            string usernameString = Session["userId"].ToString();
            user.discriminator = int.Parse(usernameString.Split('-')[0]);
            user.username = usernameString.Split('-')[1];

            Models.Video video = new Models.Video();
            video.id = id;

            VideoBll videoBll = new VideoBll();


            if (videoBll.GetVideoUpvotesByVideoId(id).Where(x => x.username == user.username && x.discriminator == user.discriminator).Count() >= 1)
                videoBll.DeleteVideoUpvote(user, video);
            else
                return new ContentResult() { Content = "false" };
            return new ContentResult() { Content = "true" };
        }

        public class _Video
        {
            public int id { get; set; }
            public String title { get; set; }
            public String username { get; set; }
            public int discriminator { get; set; }
            public String videoFile { get; set; }
            public String thumbnail { get; set; }
        }

        public class _User
        {
            public String username { get; set; }
            public int discriminator { get; set; }
            public String email { get; set; }
            public String profilePicture { get; set; }
            public String password { get; set; }
        }

        public class _Comment
        {
            public int id { get; set; }
            public string comment { get; set; }
            public string username { get; set; }
            public int discriminator { get; set; }
            public int upvotes { get; set; }
            public int downvotes { get; set; }
            public DateTime createdAt { get; set; }
        }


        public ActionResult SearchPage(string id)
        {
            if (Request.QueryString.Get("page") != null)
                ViewBag.page = int.Parse(Request.QueryString.Get("page"));
            ViewBag.Search = id;
            ViewBag.title = "Search Result";
            VideoBll videoBll = new VideoBll();
            //ViewBag.Videos = videoBll.GetVideosSearchByTitle(id);
            if (id == null)
            {
                return RedirectToAction("NotFound", "Home", new { message = "Search query not specified - Invalid URL" });
            }

            return View(videoBll.GetVideosSearchByTitle(id.ToLower()));
        }

        public ActionResult VideoPage(int id)
        {
            VideoBll videoBll = new VideoBll();
            Models.Video video = videoBll.GetVideoById(id);
            ViewBag.title = video.title;

            return View(video);
        }

        public ActionResult ProfilePage(string id)
        {

            id.Replace("#", "-");

            if (!id.Contains("-"))
                return RedirectToAction("NotFound", "Home", new { message = "User not found - Invalid URL" });

            int discriminator = int.Parse(id.Split('-')[0]);
            string username = id.Split('-')[1];

            UserBll userBll = new UserBll();
            Models.User user;
            try
            {
                user = userBll.GetUserById(id);
            }
            catch (Exception ex)
            {
                return RedirectToAction("NotFound", "Home", new { message = ex.Message });
            }

            return View(user);
        }

        public ActionResult Return(string id)
        {
            List<string> returnTo = new List<string>();

            if (Session["return"] != null)
                returnTo = Session["return"].ToString().Split('/').ToList();
            else
            {
                returnTo.Add("Home");
                returnTo.Add("Index");

            }

            Session["return"] = null;

            returnTo.RemoveAll(x => x == "");

            if (returnTo.Count == 0)
            {
                returnTo.Add("Home");
            }
            if (returnTo.Count == 1)
            {
                returnTo.Add("Index");
            }
            if (returnTo.Count == 2)
            {
                returnTo.Add("");
            }

            return RedirectToAction(returnTo[1], returnTo[0], new { id = returnTo[2] });
        }
    }
}