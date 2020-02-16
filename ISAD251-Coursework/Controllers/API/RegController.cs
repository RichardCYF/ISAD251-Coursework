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
    public class RegController : ApiController
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

        // GET: api/Reg
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Reg/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Reg/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Reg
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            byte[] Salt = GenerateSalt();
            user.Salt = Convert.ToBase64String(Salt);
            String TempPassword = user.Password;
            byte[] PasswordHash = GenerateHash(Encoding.ASCII.GetBytes(TempPassword), Salt);
            user.Password = Convert.ToBase64String(PasswordHash);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE: api/Reg/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
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