using System.IO;
using Autofac;
using Autofac.Extras.Quartz;
using CsvHelper;
using CsvHelper.Configuration;
using FluentMailer.Factory;
using FluentMailer.Interfaces;
using Model;
using Quartz;
using Service;
using Task.Job;
using View.RazorTemplate;

namespace Main
{
    public class EmailSenderEngine
    {
        private static IContainer _container;
        private static IScheduler _scheduler;

        public void RunEmailSenderFor<TModel, TRazorTemplate>(string filePath, int recordsToTake, int sendEmailIntervalInSeconds) where TModel : EmailAddress
        {
            _container = BuildContainer<TModel, TRazorTemplate>(filePath, recordsToTake);
            _scheduler = _container.Resolve<IScheduler>();
            _scheduler.Start();
            SendEmailsJob<TModel>.Shedule(_scheduler, sendEmailIntervalInSeconds);
        }

        private IContainer BuildContainer<TModel, TRazorTemplate>(string filePath, int recordsToTake) where TModel : EmailAddress
        {
            var builder = new ContainerBuilder();
            builder.Register(a => FluentMailerFactory.Create()).As<IFluentMailer>();

            builder.RegisterType<TRazorTemplate>().As<IRazorTemplate<TModel>>();

            builder.Register(a => new CsvReader(File.OpenText(filePath), new CsvConfiguration { HasHeaderRecord = false }))
                .As<ICsvReader>();     

            builder.RegisterType<ReadCsvService<TModel>>()
                .As<IReadCsvService<TModel>>()
                .WithParameter("recordsToTake", recordsToTake);
                   

            builder.RegisterType<SendEmailService<TModel>>().As<ISendEmailService<TModel>>();

            builder.RegisterType<SendEmailsJob<TModel>>().AsSelf();

            builder.RegisterModule(new QuartzAutofacFactoryModule());
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(SendEmailsJob<TModel>).Assembly));

            return builder.Build();
        }

        public void StopEmailSender()
        {
            _scheduler.Shutdown(true);
        }
    }
}
