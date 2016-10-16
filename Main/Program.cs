using System.Configuration;
using Model;
using Topshelf;
using View.RazorTemplate;

namespace Main
{
    class Program
    {
       
        static void Main()
        {
            string path = ConfigurationManager.AppSettings["Path"];
            int recordsToTake = int.Parse(ConfigurationManager.AppSettings["RecordsToTake"]);
            int sendEmailIntervalInSeconds = int.Parse(ConfigurationManager.AppSettings["SendEmailIntervalInSeconds"]);

            HostFactory.Run(x =>
            {
                x.Service<EmailSenderEngine>(s =>
                {
                    s.ConstructUsing(name => new EmailSenderEngine());
                    s.WhenStarted(ese => ese.RunEmailSenderFor<Person, PersonRazorTemplate>(path,recordsToTake,sendEmailIntervalInSeconds));
                    s.WhenStopped(ese => ese.StopEmailSender());
                });
            });
        }
       
    }
}
