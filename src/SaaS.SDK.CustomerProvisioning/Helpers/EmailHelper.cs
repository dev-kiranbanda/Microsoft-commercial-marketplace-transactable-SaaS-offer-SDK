﻿using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.Services;
using Microsoft.Marketplace.SaasKit.Models;
using System.Net;
using System.Net.Mail;
//using SendGrid;
//using SendGrid.Helpers.Mail;
namespace Microsoft.Marketplace.SaasKit.Client.Helpers
{
    public class EmailHelper
    {

        public static void SendEmail(SubscriptionResult Subscription, IApplicationConfigRepository applicationConfigRepository, IEmailTemplateRepository emailTemplateRepository)
        {
            MailMessage mail = new MailMessage();
            string FromMail = applicationConfigRepository.GetValuefromApplicationConfig("SMTPFromEmail");
            string password = applicationConfigRepository.GetValuefromApplicationConfig("SMTPPassword");
            string username = applicationConfigRepository.GetValuefromApplicationConfig("SMTPUserName");
            string Subject = emailTemplateRepository.GetSubject(Subscription.SaasSubscriptionStatus.ToString());
            bool smtpSsl = bool.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPSslEnabled"));
            mail.From = new MailAddress(FromMail);

            mail.Subject = Subject;

            string body = TemplateService.ProcessTemplate(Subscription, emailTemplateRepository, applicationConfigRepository);
            mail.Body = body;
            mail.IsBodyHtml = true;
            var result = emailTemplateRepository.GetToRecipients(Subscription.SaasSubscriptionStatus.ToString());
            if (!string.IsNullOrEmpty(emailTemplateRepository.GetToRecipients(Subscription.SaasSubscriptionStatus.ToString())))
            {
                string[] ToEmails = (emailTemplateRepository.GetToRecipients(Subscription.SaasSubscriptionStatus.ToString())).Split(';');
                foreach (string Multimailid in ToEmails)
                {
                    mail.To.Add(new MailAddress(Multimailid));
                }
            }


            if (!string.IsNullOrEmpty(emailTemplateRepository.GetCCRecipients(Subscription.SaasSubscriptionStatus.ToString())))
            {
                string[] CcEmails = (emailTemplateRepository.GetCCRecipients(Subscription.SaasSubscriptionStatus.ToString())).Split(';');
                foreach (string Multimailid in CcEmails)
                {
                    mail.CC.Add(new MailAddress(Multimailid));
                }
            }

            if (!string.IsNullOrEmpty(emailTemplateRepository.GetBccRecipients(Subscription.SaasSubscriptionStatus.ToString())))
            {
                string[] BccEmails = (emailTemplateRepository.GetBccRecipients(Subscription.SaasSubscriptionStatus.ToString())).Split(';');
                foreach (string Multimailid in BccEmails)
                {
                    mail.Bcc.Add(new MailAddress(Multimailid));
                }
            }
            SmtpClient smtp = new SmtpClient();
            smtp.Host = applicationConfigRepository.GetValuefromApplicationConfig("SMTPHost");
            smtp.Port = int.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPPort"));
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(
                username, password);
            smtp.EnableSsl = smtpSsl;
            smtp.Send(mail);
        }
    }
}
