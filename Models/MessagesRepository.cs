using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class MessagesRepository : Repository<Message>
    {
        public void DeleteMessagesByUser(int userId) 
        {
            List<Message> messages = ToList().Where(m => m.SenderId == userId || m.ReceiverId == userId).ToList();
            foreach (Message message in messages)
            {
                Delete(message.Id);
            }
        }
    }
}