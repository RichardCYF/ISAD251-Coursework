﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ISAD251_Coursework.Models;

namespace ISAD251_Coursework.Controllers
{
    public class OrdersController : ApiController
    {
        private Entities db = new Entities();

        // GET: api/Orders
        public IQueryable<Order> GetOrders()
        {
            String APIKey = System.Web.HttpContext.Current.Request.QueryString["APIKey"];
            var usr = db.Users.Where(obj => obj.APIKey.Equals(APIKey)).FirstOrDefault();
            if (usr != null)
            {
                if (usr.IsAdmin) {
                    return db.Orders;
                }
                else {
                    return db.Orders.Where(a => a.UserId.Equals(usr.Id));
                }
            }

            return null;
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Input data error.");
            }
            String APIKey = System.Web.HttpContext.Current.Request.QueryString["APIKey"];
            var usr = db.Users.Where(obj => obj.APIKey.Equals(APIKey)).FirstOrDefault();
            if (usr != null)
            {
                order.OrderTime= DateTime.Now;
                order.UserId=usr.Id;
                db.Orders.Add(order);
                db.SaveChanges();

                return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
            }
            else {return BadRequest("Please login first."); }
            
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}