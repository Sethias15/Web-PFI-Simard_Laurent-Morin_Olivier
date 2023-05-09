using ChatManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatManager.Controllers
{
    public class FriendshipsController : Controller
    {
        //Correct procedures ?
        private string SearchText
        {
            get
            {
                if (Session["SearchText"] == null)
                {
                    Session["SearchText"] = "";
                }
                return Session["SearchText"].ToString();
            }
            set
            {
                Session["SearchText"] = value;
            }
        }
        private bool FilterNotFriend
        {
            get
            {
                if (Session["FilterNotFriend"] == null)
                {
                    Session["FilterNotFriend"] = true;
                }
                return (bool)Session["FilterNotFriend"];
            }
            set
            {
                Session["FilterNotFriend"] = value;
            }
        }
        private bool FilterRequest
        {
            get
            {
                if (Session["FilterRequest"] == null)
                {
                    Session["FilterRequest"] = true;
                }
                return (bool)Session["FilterRequest"];
            }
            set
            {
                Session["FilterRequest"] = value;
            }
        }
        private bool FilterPending
        {
            get
            {
                if (Session["FilterPending"] == null)
                {
                    Session["FilterPending"] = true;
                }
                return (bool)Session["FilterPending"];
            }
            set
            {
                Session["FilterPending"] = value;
            }
        }
        private bool FilterFriend
        {
            get
            {
                if (Session["FilterFriend"] == null)
                {
                    Session["FilterFriend"] = true;
                }
                return (bool)Session["FilterFriend"];
            }
            set
            {
                Session["FilterFriend"] = value;
            }
        }
        private bool FilterRefused
        {
            get
            {
                if (Session["FilterRefused"] == null)
                {
                    Session["FilterRefused"] = true;
                }
                return (bool)Session["FilterRefused"];
            }
            set
            {
                Session["FilterRefused"] = value;
            }
        }
        private bool FilterBlocked
        {
            get
            {
                if (Session["FilterBlocked"] == null)
                {
                    Session["FilterBlocked"] = true;
                }
                return (bool)Session["FilterBlocked"];
            }
            set
            {
                Session["FilterBlocked"] = value;
            }
        }

        // GET: Friendships
        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {
            ViewBag.FilterNotFriend = FilterNotFriend;
            ViewBag.FilterRequest = FilterRequest;
            ViewBag.FilterPending = FilterPending;
            ViewBag.FilterFriend = FilterFriend;
            ViewBag.FilterRefused = FilterRefused;
            ViewBag.FilterBlocked = FilterBlocked;
            ViewBag.LoggedUsersId = new List<int>(OnlineUsers.ConnectedUsersId);
            return View();
        }
        public ActionResult Search(string text)
        {
            SearchText = text;
            return null;
        }
        public ActionResult SetFilterNotFriend(bool check)
        {
            FilterNotFriend = check;
            return null;
        }
        public ActionResult SetFilterRequest(bool check)
        {
            FilterRequest = check;
            return null;
        }
        public ActionResult SetFilterPending(bool check)
        {
            FilterPending = check;
            return null;
        }
        public ActionResult SetFilterFriend(bool check)
        {
            FilterFriend = check;
            return null;
        }
        public ActionResult SetFilterRefused(bool check)
        {
            FilterRefused = check;
            return null;
        }
        public ActionResult SetFilterBlocked(bool check)
        {
            FilterBlocked = check;
            return null;
        }
        /*private List<User> FilterUsers()
        {
            switch ()
            {
                case x:
                    break;
                default:
                    break;
            }
        }*/
        [OnlineUsers.UserAccess(false)] // RefreshTimout = false otherwise periodical refresh with lead to never timed out session
        public ActionResult GetFriendShipsStatus(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged())
            {
                ViewBag.LoggedUsersId = new List<int>(OnlineUsers.ConnectedUsersId);
                return PartialView();
            }
            return null;
        }
    }
}