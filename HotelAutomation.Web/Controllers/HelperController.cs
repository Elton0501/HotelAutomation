using DataBase;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    public class HelperController : Controller
    {
        public bool SendEmail(EmailTemplateModel model)
        {
            bool result = false;
            try
            {
                var context = new ApplicationDbContext();
                string subject = model.Subject;
                TempData["subject"] = model.Subject;
                string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "EmailTemp" + ".cshtml");
                var url = model.Message;
                body = body.Replace("@ViewBag.templateBodyContent", url);
                body = body.Replace("@ViewBag.subject", subject);
                body = body.ToString();
                result = SendEmail(model.Subject, body, model.Destination);
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        private bool SendEmail(string subject, string body, string sendemail)
        {
            bool result = false;
            try
            {
                var mail = ConfigurationManager.AppSettings["email"];
                var pass = ConfigurationManager.AppSettings["pass"];
                var port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                var stp = ConfigurationManager.AppSettings["smtp"];
                var ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["ssl"]);
                StringBuilder sb = new StringBuilder();
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(ConfigurationManager.AppSettings["email"].ToString());
                msg.To.Add(new MailAddress(sendemail));
                msg.Subject = subject;
                sb.Append(body);
                msg.Body = sb.ToString();

                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(stp, port);
                smtp.Credentials = new NetworkCredential(mail, pass);
                smtp.EnableSsl = ssl;
                smtp.Send(msg);
                smtp.Dispose();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public bool templateEmail(string email, string subject, string head, string body, string phone, string address)
        {
            bool result = false;
            try
            {
                var mail = ConfigurationManager.AppSettings["email"];
                var pass = ConfigurationManager.AppSettings["pass"];
                var port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                var stp = ConfigurationManager.AppSettings["smtp"];
                var ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["ssl"]);

                string FilePath = HostingEnvironment.MapPath("~/EmailTemplate/") + "mailtemp" + ".html";
                StreamReader reader = new StreamReader(FilePath);
                string textMail = reader.ReadToEnd();
                reader.Close();

                textMail = textMail.Replace("[head]", head.Trim());
                textMail = textMail.Replace("[content]", body.Trim());

                textMail = textMail.Replace("[phone]", phone.Trim());
                textMail = textMail.Replace("[contact]", phone.Trim());
                textMail = textMail.Replace("[address]", address.Trim());

                MailMessage message = new MailMessage();
                message.From = new MailAddress(mail);
                message.To.Add(email);
                message.Subject = subject;
                message.Body = textMail;

                message.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(stp, port);
                smtp.Credentials = new NetworkCredential(mail, pass);
                smtp.EnableSsl = ssl;
                smtp.Send(message);
                smtp.Dispose();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        [HttpPost]
        public string Encrypt(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
    }
}