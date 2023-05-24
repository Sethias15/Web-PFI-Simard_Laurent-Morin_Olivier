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
        private int CurrentTarget
        {
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
        private List<Message> GetConversation()
        {
            List<Message> allMessage = DB.Messages.ToList();
            List<Message> conversation = new List<Message>();
            User currentUser = OnlineUsers.GetSessionUser();
            User targetFriend = DB.Users.Get(CurrentTarget);
            if (targetFriend != null)
            {
                foreach (Message message in allMessage)
                {
                    if ((message.SenderId == currentUser.Id || message.SenderId == targetFriend.Id) && (message.ReceiverId == currentUser.Id || message.ReceiverId == targetFriend.Id))
                    {
                        conversation.Add(message);
                    }
                }
            }

            return conversation;
        }

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
        public ActionResult GetMessages(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || DB.Messages.HasChanged || DB.Friendships.HasChanged)
            {
                if(CurrentTarget != 0)
                {
                    ViewBag.CurrentUser = OnlineUsers.GetSessionUser();
                    Friendship fs = DB.Friendships.GetByTargetId(CurrentTarget, ViewBag.CurrentUser.Id);
                    if (fs == null || !fs.Accepted)
                    {
                        CurrentTarget = 0;
                    }
                }

                ViewBag.TargetFriend = CurrentTarget;
                ViewBag.CurrentConversation = GetConversation();
                return PartialView();
            }
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
        [OnlineUsers.UserAccess]
        public ActionResult Send(string message)
        {
            if (CurrentTarget > 0)
            {
                User currentUser = OnlineUsers.GetSessionUser();
                DB.Messages.Add(new Message(currentUser.Id, CurrentTarget, message));
                OnlineUsers.AddNotification(CurrentTarget, $"Vous avez reçu un message de {currentUser.GetFullName()}: {message}");
            }
            return null;
        }
        [OnlineUsers.UserAccess]
        public ActionResult Delete(int id)
        {
            DB.Messages.Delete(id);
            return null;
        }
        [OnlineUsers.UserAccess]
        public ActionResult Update(int id, string message)
        {
            Message oldMessage = DB.Messages.Get(id);
            DB.Messages.Update(new Message(oldMessage.Id, oldMessage.SenderId, oldMessage.ReceiverId, message, oldMessage.SendDate));
            return null;
        }
        #region AdminChat
        [OnlineUsers.AdminAccess]
        public ActionResult AdminChat()
        {
            return View();
        }

        [OnlineUsers.AdminAccess(false)] // RefreshTimout = false otherwise periodical refresh with lead to never timed out session
        public ActionResult GetAdminMessagesStatus(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Messages.HasChanged)
            {
                return PartialView();
            }
            return null;
        }

        public ActionResult DeleteAdminMessage(int id)
        {
            DB.Messages.Delete(id);
            return null;
        }
        #endregion
    }
}