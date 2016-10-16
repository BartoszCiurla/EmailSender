using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Model;
using NLog;
using Quartz;
using Service;
using Service.Utility;

namespace Task.Job
{
    public class SendEmailsJob<T> : IJob where T : EmailAddress
    {
        private readonly IReadCsvService<T> _readCsvService;
        private readonly ISendEmailService<T> _sendEmailService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string JobCacheDataKey = "CurrentRow";
        private const string JobName = "sendEmailsJob";
        private const string JobGroup = "group1";
        private const string TriggerName = "sendEmailsTrigger";
        private static MemoryCache o_Cache = MemoryCache.Default;

        public SendEmailsJob(IReadCsvService<T> csvService, ISendEmailService<T> sendEmailService)
        {
            _readCsvService = csvService;
            _sendEmailService = sendEmailService;
        }

        public static void Shedule(IScheduler scheduler,int intervalInSeconds)
        {
            IJobDetail jobDetail =
                JobBuilder.Create<SendEmailsJob<T>>().WithIdentity(JobName,JobGroup).Build();

            ITrigger trigger =
            TriggerBuilder.Create()
                .WithIdentity(TriggerName,JobGroup)
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(intervalInSeconds).RepeatForever()).Build();

            AddCacheItem(0);

            scheduler.ScheduleJob(jobDetail, trigger);
        }

        private static int GetJobdataFromCache()
        {
            int lastReadRows;
            int.TryParse(o_Cache.Get(JobCacheDataKey).ToString(),out lastReadRows);
            return lastReadRows;
        }

        private static void SetJobDataInCache(int readRows)
        {
            int lastReadRows = GetJobdataFromCache();
            if (readRows > 0)
            {
                o_Cache.Remove(JobCacheDataKey);
                AddCacheItem(lastReadRows + readRows);
            }            
        }

        private static void AddCacheItem(int readRows)
        {
            o_Cache.Add(new CacheItem(JobCacheDataKey, readRows), new CacheItemPolicy());
        }

        public void Execute(IJobExecutionContext context)
        {           
            Logger.Info($"Send emails job start");
            string runTime = ExecuteWithRunTime();                       
            Logger.Info($"Send emails job end. RunTime {runTime}");
        }

        private string ExecuteWithRunTime()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            int readRows = GetJobdataFromCache();
            var models = _readCsvService.ReadCsv(readRows).ToArray();

            Parallel.ForEach(models, ValidateExecute);
            SetJobDataInCache(models.Length);

            stopWatch.Stop();
            return ElapsedTime.GetElapsedTime(stopWatch.Elapsed);
        }
       

        private void ValidateExecute(T model)
        {
            try
            {
                ModelValidator.Validate(model);
                _sendEmailService.Send(model);
            }
            catch (Exception exception)
            {
                Logger.Error($"{exception.Message} for {model}");
            }
        }
    }
}

