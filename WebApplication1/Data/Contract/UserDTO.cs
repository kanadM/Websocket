using System;
using WebApplication1.Entities;

namespace WebApplication1.Data.Contract
{
    public class UserDTO
    {
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
        internal User ToEntity()
        {
            return new User
            {
                Id = Id,
                UserId = UserId,
                firstName = firstName,
                lastName = lastName,
                profilePic = profilePic,
                facebookId= facebookId,
                instagramId= instagramId,
                twiterId= twiterId,
                status = status,
                Password= Password
            };
        }
    }
}