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

        public void RunEmailSenderFor<TModel, TRazorTemplate>(string path) where TModel : EmailAddress
        {
            _container = BuildContainer<TModel, TRazorTemplate>(path);
            _scheduler = _container.Resolve<IScheduler>();
            _scheduler.Start();
            SendEmailsJob<TModel>.Shedule(_scheduler);
        }

        public void StopEmailSender()
        {
            _scheduler.Shutdown(true);
        }

        private IContainer BuildContainer<TModel, TRazorTemplate>(string path) where TModel : EmailAddress
        {
            var builder = new ContainerBuilder();
            builder.Register(a => FluentMailerFactory.Create()).As<IFluentMailer>();
            builder.RegisterType<TRazorTemplate>().As<IRazorTemplate<TModel>>();
            builder.Register(a => new CsvReader(File.OpenText(path), new CsvConfiguration { HasHeaderRecord = false }))
                .As<ICsvReader>();
            builder.RegisterType<ReadCsvService<TModel>>().As<IReadCsvService<TModel>>();
            builder.RegisterType<SendEmailService<TModel>>().As<ISendEmailService<TModel>>();
            builder.RegisterType<SendEmailsJob<TModel>>().AsSelf();

            builder.RegisterModule(new QuartzAutofacFactoryModule());
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(SendEmailsJob<TModel>).Assembly));

            return builder.Build();
        }
    }
}
