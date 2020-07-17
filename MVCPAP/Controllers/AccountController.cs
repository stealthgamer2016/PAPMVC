using MVCPAP.Business;
using MVCPAP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPAP.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            if(Session["userId"] is null)
            {
                return RedirectToAction("NotFound","Home",new { id="User not logged in" });
            }

            UserBll userBll = new UserBll();
            MVCPAP.Models.User user = userBll.GetUserById(Session["userId"].ToString());

            VideoBll videoBll = new VideoBll();
            ViewBag.videos = videoBll.GetVideosByUserId(Session["userId"].ToString());

            return View(user);
        }

        public ActionResult UploadVideo()
        {
            ViewBag.title = "Upload Video";

            return View();
        }

        public ActionResult ChangeUsername(MVCPAP.Models.User user)
        {
            ViewBag.title = "Change Username";

            if(Session["userId"]==null)
            {
                return RedirectToAction("NotFound", "Home", new { id = "User not logged in" });
            }

            user.username = Session["userId"].ToString().Split('-')[1];
            user.discriminator = int.Parse(Session["userId"].ToString().Split('-')[0]);
            return View(user);
        }

        [HttpPost]
        public ActionResult UpdateUsername(string newusername) {
            if (Session["userId"] is null)
            {
                return RedirectToAction("NotFound", "Home", new { id = "User not logged in" });
            }

            UserBll userBll = new UserBll();
            Models.User me = new Models.User();
            me.discriminator = int.Parse(Session["userId"].ToString().Split('-')[0]);
            me.username = Session["userId"].ToString().Split('-')[1];
            Session["userId"] = userBll.UpdateUsernameByUser(me,newusername);
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult PostVideo(HttpPostedFileBase file, Models.Video video, string tags)
        {
            Models.User user = new Models.User();

            user.username = Session["userId"].ToString().Split('-')[1];
            user.discriminator = int.Parse(Session["userId"].ToString().Split('-')[0]);


            foreach (string tagtext in tags.Split(' ').ToList())
            {
                Models.Tag tag = new Tag();
                tag.text = tagtext;
                video.tags.Add(tag);
            }

            //video.title = Request.QueryString.Get("title");
            //video.description = Request.QueryString.Get("description");
            video.thumbnail = "defaultthumbnail.png";


            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("../videos"),
                    Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                    video.videoFile = file.FileName;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    video.VideoUploadError = "ERROR:" + ex.Message.ToString();
                    return new ContentResult() { Content = "ERROR:" + ex.Message.ToString() };
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
                video.VideoUploadError= "You have not specified a file.";
                return new ContentResult() { Content = "You have not specified a file." };
            }


            VideoBll videoBll = new VideoBll();
            video = videoBll.CreateVideo(user, video);
            return RedirectToAction("VideoPage","Home",new { id=video.id });
            return new ContentResult() { Content = "Video posted" };
        }

        public ActionResult UploadVideoPage()
        {
            ViewBag.title = "Upload Video";

            return View();
        }
    }
}