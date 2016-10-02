using System;
using System.Collections.Generic;
using System.Diagnostics;
using Model;
using NLog;
using Quartz;
using Service;
using Service.Utility;

namespace Task.Job
{
    public class SendEmailsJob<T>:IJob where T:EmailAddress
    {
        private readonly IReadCsvService<T> _readCsvService;
        private readonly ISendEmailService<T> _sendEmailService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SendEmailsJob(IReadCsvService<T> csvService,ISendEmailService<T> sendEmailService)
        {
            _readCsvService = csvService;
            _sendEmailService = sendEmailService;
        }

        public static void Shedule(IScheduler scheduler)
        {
            IJobDetail jobDetail =
                JobBuilder.Create<SendEmailsJob<T>>().WithIdentity("sendEmailsJob", "Group1").Build();

            ITrigger trigger =
            TriggerBuilder.Create()
                .WithIdentity("sendEmailsTrigger", "Group1")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever()).Build();
            scheduler.ScheduleJob(jobDetail, trigger);
        }

        public void Execute(IJobExecutionContext context)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Logger.Info($"Send emails job start");
            ValidateExecute(_readCsvService.ReadCsv());

            stopWatch.Stop();
            Logger.Info($"Send emails job end. RunTime {GetElapsedTime(stopWatch.Elapsed)}");
            //Parallel.ForEach(models, ValidateExecute);
        }

        private string GetElapsedTime(TimeSpan timeSpan)
        {
            return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds/10:00}";
        }

        private void ValidateExecute(IEnumerable<T> models)
        {
            foreach (var model in models)
            {
                try
                {                   
                    _sendEmailService.Send(model);                    
                }
                catch (Exception exception)
                {
                    Logger.Error(exception.Message);
                }
                
            }         
        }
    }
}
