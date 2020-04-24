﻿using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.Models;
using Microsoft.Marketplace.SaasKit.Models;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using Microsoft.Marketplace.SaasKit.Web.Services;
using System;
using System.Net;
using System.Net.Mail;

namespace Microsoft.Marketplace.SaasKit.Web.Helpers
{
    public class EmailHelper
    {

        public static void SendEmail(SubscriptionResultExtension Subscription, IApplicationConfigRepository applicationConfigRepository, IEmailTemplateRepository emailTemplateRepository, IPlanEventsMappingRepository planEventsMappingRepository, IEventsRepository eventsRepository, string planEvent = "success", SubscriptionStatusEnum oldValue = SubscriptionStatusEnum.PendingFulfillmentStart, string newValue = null)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls13 | SecurityProtocolType.Tls;

            MailMessage mail = new MailMessage();
            string FromMail = applicationConfigRepository.GetValuefromApplicationConfig("SMTPFromEmail");
            string password = applicationConfigRepository.GetValuefromApplicationConfig("SMTPPassword");
            string username = applicationConfigRepository.GetValuefromApplicationConfig("SMTPUserName");
            string Subject = string.Empty;
            bool smtpSsl = bool.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPSslEnabled"));
            mail.From = new MailAddress(FromMail);

            string body = TemplateService.ProcessTemplate(Subscription, emailTemplateRepository, applicationConfigRepository, planEvent, oldValue, newValue);
            mail.Body = body;
            mail.IsBodyHtml = true;

            int eventID = eventsRepository.GetEventID(Subscription.EventName);

            string toReceipents = string.Empty;

            bool CustomerToCopy = false;
            bool isActive = false;
            var eventrep = planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID);
            if (eventrep != null)
            {
                CustomerToCopy = eventrep.CopytoCustomer ?? false;
                isActive = eventrep.Isactive;
            }

            if (isActive)
            {
                if (CustomerToCopy && planEvent.ToLower() == "success")
                {
                    toReceipents = Subscription.CustomerEmailAddress;
                    if (string.IsNullOrEmpty(toReceipents))
                    {
                        throw new Exception(" Error while sending an email, please check the configuration. ");
                    }
                    Subject = emailTemplateRepository.GetSubject(Subscription.SaasSubscriptionStatus.ToString());
                    mail.Subject = Subject;
                    mail.To.Add(toReceipents);
                    SmtpClient copy = new SmtpClient();
                    copy.Host = applicationConfigRepository.GetValuefromApplicationConfig("SMTPHost");
                    copy.Port = int.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPPort"));
                    copy.UseDefaultCredentials = false;
                    copy.Credentials = new NetworkCredential(
                        username, password);
                    copy.EnableSsl = smtpSsl;
                    copy.Send(mail);
                }

                mail.To.Clear();


                if (CustomerToCopy && planEvent.ToLower() == "failure")
                {
                    toReceipents = Subscription.CustomerEmailAddress;
                    if (string.IsNullOrEmpty(toReceipents))
                    {
                        throw new Exception(" Error while sending an email, please check the configuration. ");
                    }
                    Subject = emailTemplateRepository.GetSubject(planEvent);
                    mail.Subject = Subject;
                    mail.To.Add(toReceipents);
                    SmtpClient copy = new SmtpClient();
                    copy.Host = applicationConfigRepository.GetValuefromApplicationConfig("SMTPHost");
                    copy.Port = int.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPPort"));
                    copy.UseDefaultCredentials = false;
                    copy.Credentials = new NetworkCredential(
                        username, password);
                    copy.EnableSsl = smtpSsl;
                    copy.Send(mail);
                }

                if (planEvent.ToLower() == "success")
                {
                    toReceipents = (planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID).SuccessStateEmails
                  );
                    if (string.IsNullOrEmpty(toReceipents))
                    {
                        throw new Exception(" Error while sending an email, please check the configuration. ");
                    }
                    Subject = emailTemplateRepository.GetSubject(Subscription.SaasSubscriptionStatus.ToString());
                    mail.Subject = Subject;
                    if (!string.IsNullOrEmpty(toReceipents))
                    {
                        string[] ToEmails = toReceipents.Split(';');

                        foreach (string Multimailid in ToEmails)
                        {
                            mail.To.Add(new MailAddress(Multimailid));
                        }

                        if (!string.IsNullOrEmpty(emailTemplateRepository.GetCCRecipients(Subscription.SaasSubscriptionStatus.ToString())))
                        {
                            string[] CcEmails = (emailTemplateRepository.GetCCRecipients(Subscription.SaasSubscriptionStatus.ToString())).Split(';');
                            foreach (string Multimailid in CcEmails)
                            {
                                mail.CC.Add(new MailAddress(Multimailid));
                            }
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

                }
                if (planEvent.ToLower() == "failure")
                {
                    toReceipents = (planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID).FailureStateEmails
                    );
                    if (string.IsNullOrEmpty(toReceipents))
                    {
                        throw new Exception(" Error while sending an email, please check the configuration. ");
                    }
                    Subject = emailTemplateRepository.GetSubject(planEvent);
                    mail.Subject = Subject;
                    if (!string.IsNullOrEmpty(toReceipents))
                    {
                        string[] ToEmails = toReceipents.Split(';');

                        foreach (string Multimailid in ToEmails)
                        {
                            mail.To.Add(new MailAddress(Multimailid));
                        }

                        if (!string.IsNullOrEmpty(emailTemplateRepository.GetCCRecipients(planEvent)))
                        {
                            string[] CcEmails = (emailTemplateRepository.GetCCRecipients(planEvent)).Split(';');
                            foreach (string Multimailid in CcEmails)
                            {
                                mail.CC.Add(new MailAddress(Multimailid));
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(emailTemplateRepository.GetBccRecipients(planEvent)))
                    {
                        string[] BccEmails = (emailTemplateRepository.GetBccRecipients(planEvent)).Split(';');
                        foreach (string Multimailid in BccEmails)
                        {
                            mail.Bcc.Add(new MailAddress(Multimailid));
                        }
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
}
