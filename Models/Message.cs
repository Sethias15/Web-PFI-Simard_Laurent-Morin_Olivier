using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class Message
    {
        public Message(int senderId, int receiverId, string message)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            TextMessage = message;
        }
        public Message()
        {
            SenderId = -1;
            ReceiverId = -1;
            TextMessage = "";
        }
        #region Data Members
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string TextMessage { get; set; }
        #endregion
    }
}