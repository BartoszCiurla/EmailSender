using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMailer.Factory;
using FluentMailer.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceTest
{
    [TestClass]
    public class SendEmailServiceTest
    {
        [TestMethod]
        public void SendEmail()
        {
            IFluentMailer _fluentMailer = FluentMailerFactory.Create();
            _fluentMailer.CreateMessage()               
            .WithViewBody("<html><body>Test message</body></html>")
            .WithReceiver("itsgoodtoride@gmail.com")
            .WithSubject("Mail subject")
            .Send();
        }
    }
}
