using ChatManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatManager.Controllers
{
    public class ChatController : Controller
    {
        private int CurrentTarget {
            get
            {
                if (Session["CurrentTarget"] == null)
                {
                    Session["CurrentTarget"] = 0;
                }
                return (int)Session["CurrentTarget"];
            }
            set
            {
                Session["CurrentTarget"] = value;
            }
        }
        private List<User> FilterFriends()
        {
            List<User> allUsers = new List<User>(DB.Users.SortedUsers());
            List<User> userToShow = new List<User>();
            User currentUser = OnlineUsers.GetSessionUser();
            allUsers.Remove(currentUser);
            List<Friendship> friendshipsWithCurrentUser = DB.Friendships.ToList().FindAll(f => f.UserId == currentUser.Id || f.TargetUserId == currentUser.Id);
            foreach (User user in allUsers)
            {
                if (!user.Blocked)
                {
                    if (friendshipsWithCurrentUser.Exists(f => (user.Id == f.TargetUserId || f.UserId == user.Id) && f.Accepted))
                    {
                        userToShow.Add(user);
                    }
                }
            }
            return userToShow;
        }
        //public ActionResult SetCurrentTarget(int userId)
        //{
        //}

        // GET: Chat
        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {
            return View();
        }

        [OnlineUsers.UserAccess]
        public ActionResult GetFriendsList(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || DB.Friendships.HasChanged)
            {
                ViewBag.TargetFriend = CurrentTarget;
                ViewBag.FilteredFriends = FilterFriends();
                return PartialView();
            }
            return null;
        }
        [OnlineUsers.UserAccess]
        public ActionResult GetMessages()
        {
            return null;
        }
        public ActionResult SetCurrentTarget(int userId = 0)
        {
            CurrentTarget = userId;
            return null;
        }

        [OnlineUsers.UserAccess]
        public ActionResult IsTargetTyping()
        {
            return null;
        }
    }
}