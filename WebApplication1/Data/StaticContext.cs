using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Contract;
using WebApplication1.Entities;

namespace WebApplication1.Data
{
    public class StaticContext
    {
        public static List<UserDTO> Users = new List<UserDTO>();

        static StaticContext()
        {
            Users.AddRange(new List<UserDTO> {
                    new UserDTO { Id = 1, UserId = "kmehta", firstName = "kanad", lastName = "Mehta", status = ActiveStatus.Away,Password="HAHAHA"},
                    new UserDTO { Id = 1, UserId = "bbhamare", firstName = "Bhushan", lastName = "bHamare", status = ActiveStatus.Away,Password="HAHAHA"}
                });
        }

        internal void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
