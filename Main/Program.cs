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
            
            HostFactory.Run(x =>
            {
                x.Service<EmailSenderEngine>(s =>
                {
                    s.ConstructUsing(name => new EmailSenderEngine());
                    s.WhenStarted(ese => ese.RunEmailSenderFor<Person, PersonRazorTemplate>(path));
                    s.WhenStopped(ese => ese.StopEmailSender());
                });
            });
        }
       
    }
}
