using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ChatManager.Models
{
    public class FriendshipRepository : Repository<Friendship>
    {
        public Friendship GetByTargetId(int targetId, int? userId = null)
        {
            if (userId == null)
                userId = OnlineUsers.GetSessionUser().Id;

            Friendship friendship = ToList().Where(f => 
            ((f.UserId == userId) && (f.TargetUserId == targetId)) ||
            ((f.UserId == targetId) && (f.TargetUserId == userId))).FirstOrDefault();

            return friendship;
        }

        public List<Friendship> GetByUserId(int? userId = null)
        {
            if (userId == null)
                userId = OnlineUsers.GetSessionUser().Id;

            return ToList().Where(f => f.UserId == userId || f.TargetUserId == userId).ToList();
        }
        
        public void DeleteByUser(int userId)
        {
            foreach(Friendship fs in GetByUserId(userId))
            {
                Delete(fs.Id);
            }
        }
    }
}