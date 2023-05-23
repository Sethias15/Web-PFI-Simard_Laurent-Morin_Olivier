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
            List<User> allUsers;
            //Utiliser SortedUser pour la recherche texte
            if (SearchText == null)
            {
                allUsers = new List<User>(DB.Users.SortedUsers());
            }
            else
            {
                allUsers = DB.Users.SortedUsers().Where(u => (u.FirstName.ToLower() + u.LastName.ToLower()).Contains(SearchText.ToLower())).ToList();
            }

            List<User> userToShow = new List<User>();
            User currentUser = OnlineUsers.GetSessionUser();
            allUsers.Remove(currentUser);
            List<Friendship> friendshipsWithCurrentUser = DB.Friendships.ToList().FindAll(f => f.UserId == currentUser.Id || f.TargetUserId == currentUser.Id);
            foreach (User user in allUsers)
            {
                if (FilterNotFriend && !friendshipsWithCurrentUser.Exists(f => user.Id == f.TargetUserId || f.UserId == user.Id))
                {
                    userToShow.Add(user);
                }
                else if (FilterRequest && friendshipsWithCurrentUser.Exists(f => (user.Id == f.UserId) && !f.Accepted && !f.Declined))
                {
                    userToShow.Add(user);
                }
                else if (FilterPending && friendshipsWithCurrentUser.Exists(f => (user.Id == f.TargetUserId) && !f.Accepted && !f.Declined))
                {
                    userToShow.Add(user);
                }
                else if (FilterFriend && friendshipsWithCurrentUser.Exists(f => (user.Id == f.TargetUserId || f.UserId == user.Id) && f.Accepted))
                {
                    userToShow.Add(user);
                }
                else if (FilterRefused && friendshipsWithCurrentUser.Exists(f => (user.Id == f.TargetUserId || f.UserId == user.Id) && f.Declined))
                {
                    userToShow.Add(user);
                }

                if (user.Blocked)
                {
                    //Si l'usager avait deja une relation d'amitier avec l'usager actuelle avant de se faire bloquer
                    //On la retire pour "changer l'utilisateur de categorie" et donc eviter les doublons
                    if (userToShow.Contains(user))
                    {
                        userToShow.Remove(user);
                    }
                    if (FilterBlocked)
                    {
                        userToShow.Add(user);
                    }
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
        public ActionResult SendFriendshipRequest(int id) //Aucune relation ou demande après refus
        {
            User user = OnlineUsers.GetSessionUser();
            Friendship fs = DB.Friendships.GetByTargetId(id);
            if (fs == null)
                DB.Friendships.Add(new Friendship(user.Id, id));
            else
                DB.Friendships.Update(new Friendship(fs.Id, fs.TargetUserId, fs.UserId, false, false));
            OnlineUsers.AddNotification(id, $"Vous avez reçu une demande d'amitié de {user.GetFullName()}");
            //ajoute "Accepted": false, "Declined": false
            return null;
        }
        public ActionResult RemoveFriendshipRequest(int id) //Annuler une demande
        {
            User user = OnlineUsers.GetSessionUser();
            Friendship fs = DB.Friendships.Get(id);
            DB.Friendships.Delete(id);
            //retire la relation du fichier
            OnlineUsers.AddNotification(fs.TargetUserId, $"{user.GetFullName()} a retiré sa demande d'amitié");
            return null;
        }
        public ActionResult AcceptFriendshipRequest(int id) //Accepter une demande
        {
            User user = OnlineUsers.GetSessionUser();
            Friendship fs = DB.Friendships.Get(id);
            DB.Friendships.Update(new Friendship(id, fs.UserId, fs.TargetUserId, true, fs.Declined));
            //change "Accepted": true, "Declined": false
            OnlineUsers.AddNotification(fs.UserId, $"{user.GetFullName()} a accepté votre demande d'amitié");
            return null;
        }
        public ActionResult DeclineFriendshipRequest(int id) //Refuser une demande
        {
            User user = OnlineUsers.GetSessionUser();
            Friendship fs = DB.Friendships.Get(id);
            DB.Friendships.Update(new Friendship(id, fs.UserId, fs.TargetUserId, fs.Accepted, true));
            //change "Accepted": false, "Declined": true
            OnlineUsers.AddNotification(fs.UserId, $"{user.GetFullName()} a décliné votre demande d'amitié");
            return null;
        }
        public ActionResult RemoveFriendship(int id) //Retirer une amitié déjà existante
        {
            User user = OnlineUsers.GetSessionUser();
            Friendship fs = DB.Friendships.Get(id);
            DB.Friendships.Delete(id);
            OnlineUsers.AddNotification(user.Id == fs.UserId ? fs.TargetUserId : fs.UserId, $"{user.GetFullName()} s'est désisté de votre amitié");
            //retire la relation du fichier
            //différente fonction pour conservation messages ?
            return null;
        }
    }
}