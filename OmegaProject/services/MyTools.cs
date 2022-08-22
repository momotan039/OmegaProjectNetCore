using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Security.Cryptography;
using System.Text;

namespace OmegaProject.services
{
    public partial class MyTools 
    {

        public static string GenerateHashedPassword()
        {
            var x = SHA256.Create().ComputeHash(Encoding.Default.GetBytes("MomoTan Best Programmer"));
            return Convert.ToBase64String(x);
        }

        public  static string CreateHashedPassword(string pass)
        {
            var x = SHA256.Create().ComputeHash(Encoding.Default.GetBytes(pass));
            return Convert.ToBase64String(x);
        }
    }
}
