﻿namespace PFA_TEMPLATE.ViewModels
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
    }
}
