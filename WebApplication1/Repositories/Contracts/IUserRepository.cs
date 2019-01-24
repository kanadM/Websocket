using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data.Contract;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Repositories.Contracts
{
    public interface IUserRepository : IRepository<User>,IDisposable
    {
        Task<bool> UpdateUserCredentialsAsync(long userId, CredentialUpdateInputParam input);
        Task SavePostedFileAsync(int id, IFormFile postedFile, string extension);
        Task<string> GetAvatar(int id, int? width);


        //Task<Invitations> CreateUserInvitationAsync(long tierId, InvitationInputParam invite);
        //IEnumerable<ConnectedAppsAndSitesInfo> GetConnectedApps(long tierId, long userId);
        //Task<User> EnableTwoFactorAuth(long tierid, long id, TwoFactorAuthInputParam input);
    }
}
