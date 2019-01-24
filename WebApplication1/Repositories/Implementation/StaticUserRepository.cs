using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Extentions;
using WebApplication1.Models;
using WebApplication1.Repositories.Contracts;

namespace WebApplication1.Repositories.Implementation
{
    public class StaticUserRepository : IUserRepository
    {

        private StaticContext _context;
        public List<string> ErrorMessages { get; set; }
        public bool IsSuccessStatus { get; set; }
        public StaticUserRepository()
        {
        }

        public void Initialize()
        {
            IsSuccessStatus = false;
            ErrorMessages = new List<string>();
            _context = new StaticContext();
        }

        public IQueryable<User> GetAllAsync()
        {
            var allUsers = StaticContext.Users;

            IsSuccessStatus = !allUsers.Any();

            return allUsers.Select(s => s.ToEntity()).AsQueryable();
        }
        public async Task<string> GetAvatar(int id, int? width)
        {
            var filePath = Path.Combine(Path.GetFullPath($"c:\\UserPrilePictures\\{id}"));
            filePath = ImageHelper.ResizeImg(filePath, width, width);
            IsSuccessStatus = true;
            return filePath;
        }
        public async Task<User> GetByIdAsync(int id)
        {
            var allUsers = StaticContext.Users;

            var user = allUsers.Find(u => u.Id == id).ToEntity();

            IsSuccessStatus = user != null;

            return user;
        }

        public void AddAsync(User entity)
        {
            try
            {
                StaticContext.Users.Add(entity.ToContract());
                IsSuccessStatus = true;
            }
            catch (Exception ex)
            {
                ErrorMessages.Add(ex.Message);
                IsSuccessStatus = false;
            }
        }
        public async Task SavePostedFileAsync(int id, IFormFile postedFile, string extension)
        {
            using (Stream stream = postedFile.OpenReadStream())
            {
                if (postedFile.Length > 0)
                {
                    var filePath = Path.Combine(Path.GetFullPath($"c:\\UserPrilePictures\\{id}\\"));
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    using (var fstream = new FileStream(filePath + "\\original.png", FileMode.Create))
                    {
                        await postedFile.CopyToAsync(fstream);
                        IsSuccessStatus = true;
                    }
                }
            }
        }
        public void UpdateAsync(User entity)
        {
            DeleteAsync(entity.Id);
            if (IsSuccessStatus)
                StaticContext.Users.Add(entity.ToContract());
        }
        public async Task<bool> UpdateUserCredentialsAsync(long userId, CredentialUpdateInputParam input)
        {
            var allUsers = StaticContext.Users;

            var user = allUsers.Find(u => u.Id == userId).ToEntity();

            IsSuccessStatus = user != null && input.OldPassword == user.Password;
            if (IsSuccessStatus)
                user.Password = input.NewPassword;
            return IsSuccessStatus;
        }

        public void DeleteAsync(User entity)
        {
            DeleteAsync(entity.Id);
        }
        public void DeleteAsync(int id)
        {
            IsSuccessStatus = StaticContext.Users.RemoveAll(s => s.Id == id) > 0;
        }
        public void Dispose()
        {
            if (_context != null)
                _context.Dispose();
        }
    }
}
