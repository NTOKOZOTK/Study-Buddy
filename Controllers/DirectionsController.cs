using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Study_Buddy.Models;

namespace Study_Buddy.Controllers
{
    public class DirectionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Directions
        public ActionResult Index()
        {
            return View(db.Directions.ToList());
        }
        public ActionResult TutorDirections()
        {
            return View(db.Directions.ToList());
        }
        // GET: Directions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Directions directions = db.Directions.Find(id);
            if (directions == null)
            {
                return HttpNotFound();
            }
            return View(directions);
        }

        // GET: Directions/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult GetDirections(int id)
        {
            //Email user = new Email();
            var dir = (from e in db.Directions
                       where e.Id == id
                       select e.Location).FirstOrDefault();
            TempData["user"] = dir;
            return View();
        }

        // POST: Directions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,StudNumber,MeetingPoint,Email,Location")] Directions directions)
        {
            if (ModelState.IsValid)
            {
                db.Directions.Add(directions);
                db.SaveChanges();
                return RedirectToAction("TutorDirections");
            }

            return View(directions);
        }

        // GET: Directions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Directions directions = db.Directions.Find(id);
            if (directions == null)
            {
                return HttpNotFound();
            }
            return View(directions);
        }

        // POST: Directions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StudNumber,MeetingPoint,Email,Location")] Directions directions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(directions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TutorDirections");
            }
            return View(directions);
        }

        // GET: Directions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Directions directions = db.Directions.Find(id);
            if (directions == null)
            {
                return HttpNotFound();
            }
            return View(directions);
        }

        // POST: Directions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Directions directions = db.Directions.Find(id);
            db.Directions.Remove(directions);
            db.SaveChanges();
            return RedirectToAction("TutorDirections");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
