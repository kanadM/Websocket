using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WebApplication1.Data.Contract;

namespace WebApplication1.Entities
{
    public class User
    {
        public User()
        {

        }
        public int Id { get; set; }
        public string UserId { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }
        public string profilePic { get; set; }
        public string facebookId { get; set; }
        public string instagramId { get; set; }
        public string twiterId { get; set; }
        public ActiveStatus status { get; set; }
        public string Password { get; set; }
        public WebSocket connection { get; set; }
        internal UserDTO ToContract()
        {
            return new UserDTO
            {
                Id = Id,
                UserId = UserId,
                firstName = firstName,
                lastName = lastName,
                profilePic = profilePic,
                facebookId = facebookId,
                instagramId = instagramId,
                twiterId = twiterId,
                status = status,
                Password= Password
            };
        }
    }
}
