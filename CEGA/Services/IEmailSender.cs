﻿namespace CEGA.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string html);
    }
}
