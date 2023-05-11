using ChatManager.Models;
using Newtonsoft.Json.Linq;
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
        private List<User> FilterUsers()
        {
            //Utiliser SortedUser pour la recherche texte
            List<User> allUsers = new List<User>(DB.Users.ToList());
            List<User> userToShow = new List<User>();
            User currentUser = OnlineUsers.GetSessionUser();
            allUsers.Remove(currentUser);
            List<Friendship> friendshipsWithCurrentUser = DB.Friendships.ToList().FindAll(f => f.UserId == currentUser.Id);
            foreach (User user in allUsers)
            {
                if (FilterNotFriend && !friendshipsWithCurrentUser.Exists(f => user.Id == f.TargetUserId))
                {
                    userToShow.Add(user);
                }
                else if (FilterRequest && false)
                {
                    userToShow.Add(user);
                }
                else if (FilterPending && false)
                {
                    userToShow.Add(user);
                }
                else if (FilterFriend && false)
                {
                    userToShow.Add(user);
                }
                else if (FilterRefused && false)
                {
                    userToShow.Add(user);
                }
                else if (FilterBlocked && false)
                {
                    userToShow.Add(user);
                }
            }
            return userToShow;
        }

 

        [OnlineUsers.UserAccess(false)] // RefreshTimout = false otherwise periodical refresh with lead to never timed out session
        public ActionResult GetFriendShipsStatus(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || DB.Friendships.HasChanged)
            {
                ViewBag.LoggedUsersId = new List<int>(OnlineUsers.ConnectedUsersId);
                ViewBag.FilteredUser = FilterUsers();
                return PartialView();
            }
            return null;
        }
        public ActionResult SendFriendshipRequest(int targetId) //Aucune relation ou demande après refus
        {
            int userId = OnlineUsers.GetSessionUser().Id;
            if (DB.Friendships.GetByTargetId(targetId) == null)
            {
                DB.Friendships.Add(new Friendship(userId, targetId));
            }
            //ajoute "Accepted": false, "Declined": false
            return null;
        }
        public ActionResult RemoveFriendshipRequest(int friendshipId) //Annuler une demande
        {
            /*Session["FilterRequest"]
            DB.Users.
            DB.Friendships.Get()
            DB.Friendships.Delete()*/
            //retire la relation du fichier
            return null;
        }
        public ActionResult AcceptFriendshipRequest(int friendshipId) //Accepter une demande
        {
            //ajoute "Accepted": true, "Declined": false
            return null;
        }
        public ActionResult DeclineFriendshipRequest(int friendshipId) //Refuser une demande
        {
            //ajoute "Accepted": false, "Declined": true
            return null;
        }
        public ActionResult RemoveFriendship(int friendshipId) //Retirer une amitié déjà existante
        {
            //retire la relation du fichier
            //différente fonction pour conservation messages ?
            return null;
        }
    }
}