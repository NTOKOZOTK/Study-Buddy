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
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Windows;

namespace Study_Buddy.Controllers
{
    public class StudentsController : ApplicationBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Students
        public ActionResult Index(int? id)
        {
            if (id != null)
            {
                var paid = db.Students.Where(x => x.Stud_Id == id).FirstOrDefault();
                paid.Status = "Paid";
                db.SaveChanges();
            }

            var username = User.Identity.GetUserName();
            var filter = (from x in db.Students
                          where x.Email == username
                          select x);
            return View(filter.ToList());
        }

        
       
        public ActionResult ManageStudent()
        {
           
            return View(db.Students.ToList());
        }
        public ActionResult landingS()
        {
            return View();
        }
        public ActionResult landingA()
        {
            return View();
        }
        public ActionResult PaymentButton()
        {
            return View();
        }
        public ActionResult MakePayment(int id)
        {
            //var Paid = db.Students.Where(x => x.Stud_Id == id).FirstOrDefault();
            //Paid.Status = "Paid";
            //db.SaveChanges();
            //return RedirectToAction("PaymentButton");

            string total = "150";
            return Redirect(PaymentLink(total, "Tutorial Fee Payment", id));
        }

        public string PaymentLink(string totalCost, string paymentSubjetc, int stud_Id)
        {
            
            string paymentMode = ConfigurationManager.AppSettings["PaymentMode"], site, merchantId, merchantKey, returnUrl;

            site = "https://sandbox.payfast.co.za/eng/process?";
            merchantId = "10022900";
            merchantKey = "qq34viiias2on";
            returnUrl = "https://localhost:44305/Students/Index/";

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("merchant_id=" + HttpUtility.HtmlEncode(merchantId));
            stringBuilder.Append("&merchant_key=" + HttpUtility.HtmlEncode(merchantKey));
            stringBuilder.Append("&return_url= " + HttpUtility.HtmlEncode("https://localhost:44305/Students/Index/" + stud_Id));

            string amount = totalCost;
            amount = amount.Replace(",", ".");
            stringBuilder.Append("&amount=" + HttpUtility.HtmlEncode(amount));
            stringBuilder.Append("&item_name=" + HttpUtility.HtmlEncode(paymentSubjetc));
            stringBuilder.Append("&email_confirmation=" + HttpUtility.HtmlEncode("1"));
            stringBuilder.Append("&confirmation_address=" + HttpUtility.HtmlEncode(ConfigurationManager.AppSettings["PF_ConfirmationAddress"]));

            return (site + stringBuilder);
        }

        public ActionResult RejectStudent(int id,Booking booking)
        {
            string username = User.Identity.GetUserName();
            string name = db.Tutors.Where(x => x.Email == username).Select(x => x.Tutor_Name).FirstOrDefault();
            var decline = db.Students.Where(x => x.Stud_Id == id).FirstOrDefault();
            decline.Status = "Rejected";
            if (decline.Status == "Rejected")
            {
                var bookings = ( from x in db.Bookings
                               where x.Stud_name == decline.Stud_Name
                               select x).FirstOrDefault();
                bookings.Student_status = "Rejected";
                db.Bookings.Add(booking);
                db.SaveChanges();

            }
            SendEmail(decline.Email, decline.Stud_Name, decline.Status, name);
            db.SaveChanges();

           

            return RedirectToAction("ManageStudent");
        }
        public ActionResult ApproveStudent(int id, Booking booking)
        {
            string username = User.Identity.GetUserName();
            string name = db.Tutors.Where(x => x.Email == username).Select(x => x.Tutor_Name).FirstOrDefault();
            var decline = db.Students.Where(x => x.Stud_Id == id).FirstOrDefault();
            decline.Status = "Approved";
            if (decline.Status == "Approved")
            {
                var bookings = (from x in db.Bookings
                                where x.Stud_name == decline.Stud_Name
                                select x).FirstOrDefault();
                bookings.Student_status = "Approved";
                db.Bookings.Add(booking);
                db.SaveChanges();

            }
            SendEmail(decline.Email, decline.Stud_Name, decline.Status,name);
            db.SaveChanges();
            return RedirectToAction("ManageStudent");
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create

        [HttpGet]
        public ActionResult Create()
        {
            Student student = new Student();
            var user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            try
            {
                student.Stud_Number = Convert.ToInt32(user.Email.Substring(0, 8));
                student.Stud_Name = user.LastName;
                student.Email = user.Email;

                
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please Use Only Dut Prescribed Emails !");
            }
            return View(student);
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( int id,[Bind(Include = "Stud_Id,Stud_Number,Stud_Name,Email,Study_Time,Study_Date,pur_booking,Level_Of_Study,Meeting_Place,Status,Module,Faculty,Department,Tutor_Id")] Student student)
        {
            Session["id"]= id;
            if (ModelState.IsValid)
            {
                student.Status = "Pending";
                student.Payment_Status = "Not Paid";
                student.Email = User.Identity.GetUserName();
                db.Students.Add(student);
                db.SaveChanges();
            }
           

            return RedirectToAction("Booking");
        }
        public ActionResult Booking(Booking booking)
        {
            int id = Convert.ToInt32(Session["id"]);
            var username = User.Identity.GetUserName();
            if (ModelState.IsValid)
            {
                var tutor_infor = db.Tutors.Where(x => x.Tutor_Id == id).FirstOrDefault();
                booking.Tutor_email = tutor_infor.Email;
                booking.Tutor_name = tutor_infor.Tutor_Name;
                booking.Book_time = tutor_infor.availableTime;
                booking.Module = tutor_infor.Module;
                var stud_name = (from x in db.Users
                                 where x.Email == username
                                 select x.LastName).FirstOrDefault();
                booking.Stud_name = stud_name;
                booking.Student_status = "Pending";
                booking.Stud_email = User.Identity.GetUserName();
                db.Bookings.Add(booking);
                db.SaveChanges();
                //var tut_info = (from x in db.Bookings
                //                where x.Booking_Id == id
                //                select x.Tutor_email);
                Send(booking.Tutor_email, booking.Stud_name, booking.Tutor_name, booking.Book_time, booking.Module);
            }
            return RedirectToAction("BookingIndex");
        }
        public ActionResult BookingIndex()
        {

            var userId = User.Identity.GetUserName();
            var filter = (from x in db.Bookings
                          where x.Stud_email == userId
                          select x);
            return View(filter.ToList());
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Stud_Id,Stud_Number,Stud_Name,Email,Study_Time,Study_Date,pur_booking,Level_Of_Study,Meeting_Place,Status,Module,Faculty,Department")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("ManageStudent");
        }
        // GET: Students/Delete/5
        public ActionResult DeleteBooking(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Students Booking Delete/Delete/5
        [HttpPost, ActionName("DeleteBooking")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBooking(int id)
        {
            Booking booking= db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("BookingIndex");
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
        public void SendEmail(string Email, string Stud_Name, string Status,string name )
        {
            var fromEmail = new MailAddress("21345309@dut4life.ac.za", "Study Buddy");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "Dut910214"; // Replace with actual password

            string subject = "";
            string body = "";

            if (Status == "Approved")
            {
                subject = "Tutorial Application Approval";
                body = "Greetings " + Stud_Name + "<br/><br/>We are excited to inform you that" +
                    " your application for a tutorial session has been Approved by tutor "+""+name+ "<br/><br/> Kind Regards<br/><br/> Study Buddy ";

            }
            else if (Status == "Rejected")
            {
                subject = "Tutorial Application Rejection";
                body = "Greetings " + Stud_Name + "<br/><br/>We regret to inform you that your tutorial application" +
                    " session has been rejected by tutor "+""+name+" due to no payment" +
                    " or inavailability of time scheduled you selected<br/><br/> Kind Regards<br/><br/> Study Buddy ";
            }


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
        [NonAction]
        public void Send(string Email,string Stud_Name, string Tut_name, string Book_Time, string Module)
        {
            var fromEmail = new MailAddress("21345309@dut4life.ac.za", "Study Buddy");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "Dut910214"; // Replace with actual password

            string subject = "";
            string body = "";

           
                subject = "Tutorial Booking";
                body = "Greetings " + Tut_name + "<br/><br/>You have been booked" +
                    " for today tutorial session scheduled between " + Book_Time +" time slot, by student " + "" + Stud_Name + " for "+Module+" module."+ " <br/><br/> Please login to the system to manage this applicant. " + "<br/><br/> Kind Regards<br/> Study Buddy ";

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
