// Services/PasswordHasherService.cs
using PFA_TEMPLATE.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace PFA_TEMPLATE.Services
{
    public class PasswordHasherService : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return HasherProgram.HashPassword(password);
        }
    }
}