using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBikesWebpage;
using MVCBikesWebpage.Models;
using System.IO;
using System.Net.Mail;
using CaptchaMvc.Attributes;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Interface;

namespace MVCBikesWebpage.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "FirstName, LastName, Email, Comments")]  ContactUsModel contact)
        {
            if (ModelState.IsValid)
            {
                bool valid = true;
                if (!this.IsCaptchaValid("Captcha is not valid !"))
                {
                    ViewBag.CaptchaError = "Captcha Is not valid !";
                    valid = false;
                }
                if (valid)
                {
                    try
                    {
                        SmtpClient smtp = new SmtpClient();
                        MailMessage msg = new MailMessage();
                        msg.Subject = "Contact Us on " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now;
                        msg.From = new MailAddress("compscimilestone@gmail.com");
                        msg.To.Add(new MailAddress("compscimilestone@gmail.com"));
                        msg.IsBodyHtml = true;
                        msg.Body += "First Name: " + contact.FirstName + "<br>";
                        msg.Body += "Last Name: " + contact.LastName + "<br>";
                        msg.Body += "Email: " + contact.Email + "<br>";
                        msg.Body += "Comments: " + contact.Comments;
                        msg.ReplyTo = new MailAddress(contact.Email);

                        //Add a Reply-to the message having the inputted user's email
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Credentials = new NetworkCredential("compscimilestone@gmail.com", "somepassword");
                        smtp.Send(msg);
                        msg.Dispose();
                        return RedirectToAction("Success");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Error");
                    }
                }

            }
            return View();
        }
    }
}