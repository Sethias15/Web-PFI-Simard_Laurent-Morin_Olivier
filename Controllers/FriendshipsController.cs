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
        // GET: Friendships
        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {
            return View();
        }
        [OnlineUsers.AdminAccess(false)] // RefreshTimout = false otherwise periodical refresh with lead to never timed out session
        public ActionResult GetFriendShipsStatus(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged())
            {
                //ViewBag.LoggedUsersId = new List<int>(OnlineUsers.ConnectedUsersId);
                return PartialView(DB.Users.ToList());
            }
            return null;
        }
    }
}