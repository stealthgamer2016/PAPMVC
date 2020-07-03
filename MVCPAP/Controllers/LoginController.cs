using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCPAP.Models;
using MVCPAP.Business;

namespace MVCPAP.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Autherize(MVCPAP.Models.User loginUser)
        {

            UserBll Bll = new UserBll();
            loginUser = Bll.LogInUser(loginUser);
            if (loginUser.Valid)
            {
                Session["userId"] = loginUser.discriminator + "-" + loginUser.username;

                return RedirectToAction("Return","Home",new { id=""});
            }
            else
            {
                return View("LoginPage", loginUser);
            }
        }

        public ActionResult Signin(MVCPAP.Models.User loginUser)
        {

            UserBll Bll = new UserBll();

            loginUser = Bll.SignInUser(loginUser);
            if (loginUser.Valid)
            {
                Session["userId"] = loginUser.discriminator + "-" + loginUser.username;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("SigninPage", loginUser);
            }
        }

        [HttpPost]
        public ActionResult Logout(string returnTo)
        {
            //Session.Abandon();
            Session["userId"] = null;
            Session["return"] = returnTo;

            return RedirectToAction("Return", "Home");
        }

        public ActionResult LoginPage(string returnTo)
        {
            Session["return"] = returnTo;
            return View();
        }

        public ActionResult SigninPage()
        {
            return View();
        }



    }
}