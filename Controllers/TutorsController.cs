using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Study_Buddy.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Net.Mail;

namespace Study_Buddy.Controllers
{
    public class TutorsController : ApplicationBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tutors
        public ActionResult Index()
        {
            var username = User.Identity.GetUserName();
            var filter = (from x in db.Tutors
                          where x.Email == username
                          select x);
            return View(filter.ToList());
        }
        public ActionResult IndexA()
        {
            return View(db.Tutors.ToList());
        }
        public ActionResult ListofTutors()
        {
            
            return View(db.Tutors.ToList());
        }
        public ActionResult LandingT()
        {
            return View();
        }
        public ActionResult RejectTutor(int id)
        {
            var decline = db.Tutors.Where(x => x.Tutor_Id == id).FirstOrDefault();
            decline.Status = "Rejected";
            db.SaveChanges();
            return RedirectToAction("IndexA");
        }
        public ActionResult ApproveTutor(int id)
        {
            var decline = db.Tutors.Where(x => x.Tutor_Id == id).FirstOrDefault();
            decline.Status = "Approved";
            db.SaveChanges();
            return RedirectToAction("IndexA");
        }
        public ActionResult Available(int id)
        {
            var avail = db.Tutors.Where(x => x.Tutor_Id == id).FirstOrDefault();
            avail.Availability = "Available";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult NotAvailable(int id)
        {
            var NotAvailable = db.Tutors.Where(x => x.Tutor_Id == id).FirstOrDefault();
            NotAvailable.Availability = "Not Available";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Tutors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tutor tutor = db.Tutors.Find(id);
            if (tutor == null)
            {
                return HttpNotFound();
            }
            return View(tutor);
        }

        public ActionResult ApplicationSuccessPage()
        {
            return View();
        }

        // GET: Tutors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tutors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Tutor_Id,Tutor_Name,Email,Level_Of_Study,Status,Binary,Level_2_tut,Faculty,Department,Module,Availability,availableTime")] Tutor tutor, HttpPostedFileBase File )
        {
            if (ModelState.IsValid)
            {
                tutor.Status = "Untouched";
                tutor.Availability = "Not Specified";
                if (File != null)
                {
                    tutor.Binary = new byte[File.ContentLength];
                    File.InputStream.Read(tutor.Binary, 0, File.ContentLength);

                }
                SendEmail(tutor.Email, tutor.Tutor_Name);
                db.Tutors.Add(tutor);
                db.SaveChanges();
                
            }
            return RedirectToAction("ApplicationSuccessPage", "Tutors");


        }
       
        public ActionResult Download(int id)
        {
                string getName = db.Tutors.Where(x => x.Tutor_Id == id)
                            .Select(x => x.Tutor_Name).FirstOrDefault();
          
                byte[] getFile = db.Tutors.Where(x=>x.Tutor_Id==id)
                            .Select(x=>x.Binary).FirstOrDefault();

            return File(getFile, "application/pdf", getName+"_Transcript.pdf");
        }
          
        // GET: Tutors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tutor tutor = db.Tutors.Find(id);
            if (tutor == null)
            {
                return HttpNotFound();
            }
            return View(tutor);
        }

        // POST: Tutors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Tutor_Id,Tutor_Name,Email,Level_Of_Study,Status,Binary,Level_2_tut,Faculty,Department,Module,Availability,availableTime")] Tutor tutor)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(tutor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tutor);
        }

        // GET: Tutors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tutor tutor = db.Tutors.Find(id);
            if (tutor == null)
            {
                return HttpNotFound();
            }
            return View(tutor);
        }

        // POST: Tutors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tutor tutor = db.Tutors.Find(id);
            db.Tutors.Remove(tutor);
            db.SaveChanges();
            return RedirectToAction("IndexA");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [NonAction]
        public void SendEmail(string Email, string Name)
        {
            var fromEmail = new MailAddress("21345309@dut4life.ac.za", "Study Buddy");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "Dut910214"; // Replace with actual password

            string subject = "";
            string body = "";
           
                subject = "Application Confirmation";
                body = "Greetings " + Name + 
                "<br/><br/>We are excited to tell you that your application to become a tutor has been received"
                +"<br/>"+"kindly sign up to be able to track your application status. " +"<br/><br/> Kind Regards<br/><br/> Study Buddy ";

            var smtp = new SmtpClient
            {
                Host = "smtp.office365.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
            ViewBag.Message = "Success";
        }

    }
}
