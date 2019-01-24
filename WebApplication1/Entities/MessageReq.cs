using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Entities
{
    public class MessageReq
    {
        public MessageReq()
        {
            To = Message = From = string.Empty;
        }
        public string To { get; set; }
        public string Message { get; set; }
        public string From { get; set; }
        public DateTime SentDateTime { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        internal byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }
    }
}
