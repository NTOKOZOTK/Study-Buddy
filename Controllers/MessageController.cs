using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using Study_Buddy.Models;
using Study_Buddy.ViewModels;


namespace Study_Buddy.Controllers
{
    public class MessageController : ApplicationBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [HttpPost]
        [Authorize]
        public ActionResult PostMessage(MessageReplyViewModel vm)
        {
            var username = User.Identity.Name;
            string fullName = "";
            int msgid = 0;
            if (!string.IsNullOrEmpty(username))
            {
                var user = db.Users.SingleOrDefault(u => u.UserName == username);
                fullName = string.Concat(new string[] { user.FirstName, " ", user.LastName });
            }
            Message messagetoPost = new Message();
            if (vm.Message.Subject != string.Empty && vm.Message.MessageToPost != string.Empty)
            {
                messagetoPost.DatePosted = DateTime.Now;
                messagetoPost.Subject = vm.Message.Subject;
                messagetoPost.MessageToPost = vm.Message.MessageToPost;
                messagetoPost.From = fullName;

                db.Messages.Add(messagetoPost);
                db.SaveChanges();
                msgid = messagetoPost.Id;
            }

            return RedirectToAction("Index", "NoticeBoard", new { Id = msgid });
        }

        public ActionResult Create()
        {
            MessageReplyViewModel vm = new MessageReplyViewModel();

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ReplyMessage(MessageReplyViewModel vm, int messageId)
        {
            var username = User.Identity.Name;
            string fullName = "";
            if (!string.IsNullOrEmpty(username))
            {
                var user = db.Users.SingleOrDefault(u => u.UserName == username);
                fullName = string.Concat(new string[] { user.FirstName, " ", user.LastName });
            }
            if (vm.Reply.ReplyMessage != null)
            {
                //var email = (from e in db.TBLStuds
                //             where e.TeamName == setTeam.TeamName
                //             select e.EmailAddress).Single();
                //SendEmail(email, setTeam.TeamName, setTeam.TournamentType);
                Reply _reply = new Reply();
                _reply.ReplyDateTime = DateTime.Now;
                _reply.MessageId = messageId;
                _reply.ReplyFrom = fullName;
                _reply.ReplyMessage = vm.Reply.ReplyMessage;
                db.Replies.Add(_reply);
                db.SaveChanges();



            }


            //reply to the message owner          - using email template

            var messageOwner = db.Messages.Where(x => x.Id == messageId).Select(s => s.From).FirstOrDefault();
            var users = from user in db.Users
                        orderby user.FirstName
                        select new
                        {
                            FullName = user.FirstName + " " + user.LastName,
                            UserEmail = user.Email
                        };

            var uemail = users.Where(x => x.FullName == messageOwner).Select(s => s.UserEmail).Single();

            var fromEmail = new MailAddress("21345309@dut4life.ac.za", "Study Buddy");

            var toEmail = new MailAddress(uemail);
            var fromEmailPassword = "Dut910214"; // Replace with actual password

            string subject = "Reply for your message :" + db.Messages.Where(i => i.Id == messageId).Select(s => s.Subject).FirstOrDefault();
            string body = vm.Reply.ReplyMessage;

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
            //SendGridMessage replyMessage = new SendGridMessage();
            //replyMessage.From = new MailAddress(username);
            //replyMessage.Subject = "Reply for your message :" + db.Messages.Where(i=>i.Id==messageId).Select(s=>s.Subject).FirstOrDefault();
            //replyMessage.Text = vm.Reply.ReplyMessage;


            //replyMessage.AddTo(uemail);


            //var credentials = new NetworkCredential("YOUR SENDGRID USERNAME", "PASSWORD");
            //var transportweb = new Web(credentials);
            //transportweb.DeliverAsync(replyMessage);
            return RedirectToAction("Index", "NoticeBoard", new { Id = messageId });

        }

       

        [HttpPost]
        [Authorize]
        public ActionResult DeleteMessage(int messageId)
        {
            Message _messageToDelete = db.Messages.Find(messageId);
            db.Messages.Remove(_messageToDelete);
            db.SaveChanges();

            // also delete the replies related to the message
            var _repliesToDelete = db.Replies.Where(i => i.MessageId == messageId).ToList();
            if (_repliesToDelete != null)
            {
                foreach (var rep in _repliesToDelete)
                {
                    db.Replies.Remove(rep);
                    db.SaveChanges();
                    }
            }


            return RedirectToAction("Index", "NoticeBoard");
        }
    }
}
