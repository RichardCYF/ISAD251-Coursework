using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using ISAD251_Coursework.Models;

namespace ISAD251_Coursework.Controllers.API
{
    public class UsersController : ApiController
    {
        static byte[] GenerateHash(byte[] Text, byte[] salt)
        {
            HashAlgorithm algo = new SHA256Managed();
            byte[] outputBytes =
              new byte[Text.Length + salt.Length];

            for (int i = 0; i < Text.Length; i++)
            {
                outputBytes[i] = Text[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                outputBytes[Text.Length + i] = salt[i];
            }
            return algo.ComputeHash(outputBytes);
        }
        static byte[] GenerateSalt()
        {
            var RNG = new RNGCryptoServiceProvider();
            byte[] output = new byte[32];
            RNG.GetNonZeroBytes(output);
            return output;
        }

        private Entities db = new Entities();
        // GET: api/Users/
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser()
        {
            String APIKey = System.Web.HttpContext.Current.Request.QueryString["APIKey"];
            User user = db.Users.Where(obj => obj.APIKey.Equals(APIKey)).FirstOrDefault();
            if (user != null && APIKey != "" && APIKey != null)
            {
                user.APIKey = null;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return Ok("APIKey nullified.");
            }
            return Ok("Input error.");
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usr = db.Users.Where(obj => obj.Username.Equals(user.Username)).FirstOrDefault();
            if (usr != null && user.Password != "" && user.Password != null)
            {
                byte[] Salt = Convert.FromBase64String(usr.Salt);
                String TempPassword = user.Password;
                byte[] InputPasswordHash = GenerateHash(Encoding.ASCII.GetBytes(TempPassword), Salt);
                byte[] PasswordHash = Convert.FromBase64String(usr.Password);
                if (System.Collections.StructuralComparisons.StructuralEqualityComparer.Equals(PasswordHash, InputPasswordHash))
                {
                    usr.APIKey = Convert.ToBase64String(GenerateSalt()).Replace("=","Z").Replace("\\","A").Replace("/","D").Replace("+","");
                    db.Entry(usr).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok(usr.APIKey);
                }
            }
            return BadRequest("Invalid Username or Password.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }
    }
}