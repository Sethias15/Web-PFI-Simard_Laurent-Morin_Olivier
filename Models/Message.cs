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
            SendDate = DateTime.Now;
        }
        public Message(int id, int senderId, int receiverId, string message, DateTime dateTime)
        {
            Id = id;
            SenderId = senderId;
            ReceiverId = receiverId;
            TextMessage = message;
            SendDate = dateTime;
        }
        public Message()
        {
            SendDate = DateTime.Now;
        }
        #region Data Members
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string TextMessage { get; set; }
        public DateTime SendDate { get; set; }
        #endregion
    }
}