﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using DotEnv.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

namespace Concert.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly string SendGridSecret;
        private readonly string SenderEmail;

        public EmailSender(IConfiguration _config)
        {
            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            new EnvLoader().Load();
            var envVarReader = new EnvReader();

            if (envName == SD.ENVIRONMENT_DEVELOPMENT)
            {
                SendGridSecret = envVarReader["SendGrid_SecretKey"];
                SenderEmail = envVarReader["SendGrid_SenderEmail"];
            }
            else if (envName == SD.ENVIRONMENT_PRODUCTION)
            {
                SendGridSecret = Environment.GetEnvironmentVariable("SendGrid_SecretKey");
                SenderEmail = Environment.GetEnvironmentVariable("SendGrid_SenderEmail");
            }
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(SendGridSecret);
            var from = new EmailAddress(SenderEmail, "Concert");
            var to = new EmailAddress(email);
            var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

            /// @todo Logic to send an email
            return client.SendEmailAsync(message);
        }
    }
}
