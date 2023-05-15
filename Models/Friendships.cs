using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ChatManager.Models
{
    public class Friendship
    {
        public Friendship()
        {
            Accepted = false;
            Declined = false;
        }
        public Friendship(int userId, int targerUserId)
        {
            UserId = userId;
            TargetUserId = targerUserId;
            Accepted = false;
            Declined = false;
        }
        public Friendship(int id, int userId, int targerUserId, bool accepted, bool declined)
        {
            Id = id;
            UserId = userId;
            TargetUserId = targerUserId;
            Accepted = accepted;
            Declined = declined;
        }
        public User Clone()
        {
            return JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(this));
        }
        #region Data Members
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TargetUserId { get; set; }
        public bool Accepted { get; set; }
        public bool Declined { get; set; }
        #endregion
    }
}