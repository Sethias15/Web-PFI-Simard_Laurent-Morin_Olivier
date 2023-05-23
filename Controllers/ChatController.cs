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
                ViewBag.FilteredFriends = FilterFriends();
                return PartialView();
            }
            return null;
        }

        [OnlineUsers.AdminAccess]
        public ActionResult AdminChat()
        {
            return View();
        }

    }
}