using Microsoft.AspNetCore.Mvc;
using PasswordManeger.Data;
using PasswordManeger.Models;
using PasswordManeger.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;

namespace PasswordManeger.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly byte[] _Key;
        private readonly byte[] _IV;

        public DashBoardController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;

            _Key = Convert.FromBase64String(configuration["Encryption:Key"]);
            _IV = Convert.FromBase64String(configuration["Encryption:IV"]);
        }

        public IActionResult DashBoard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var passwords = _context.UserPasswords
                .Where(p => p.UserId == userId)
                .ToList();

            // расшифровываем перед выводом
            foreach (var p in passwords)
            {
                p.Password = Decrypt(Convert.FromBase64String(p.Password), _Key, _IV);
            }

            var model = new DashBoardViewModel
            {
                Passwords = passwords,
                NewPassword = new AddPasswordViewModel()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var encryptedBytes = Encryp(model.Password, _Key, _IV);

                var password = new UserPasswords
                {
                    ServiceName = model.ServiceName,
                    Password = Convert.ToBase64String(encryptedBytes), 
                    Link = model.Link,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                _context.UserPasswords.Add(password);
                _context.SaveChanges();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var passwords = _context.UserPasswords
                .Where(p => p.UserId == currentUserId)
                .ToList();

            foreach (var p in passwords)
            {
                p.Password = Decrypt(Convert.FromBase64String(p.Password), _Key, _IV);
            }

            var vm = new DashBoardViewModel
            {
                Passwords = passwords,
                NewPassword = new AddPasswordViewModel()
            };

            return View("DashBoard", vm);
        }

        public async Task<IActionResult> DeletePassword(int id)
        {
            var password = _context.UserPasswords.FirstOrDefault(p => p.Id == id);
            if (password != null)
            {
                _context.UserPasswords.Remove(password);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("DashBoard");
        }

        static byte[] Encryp(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
