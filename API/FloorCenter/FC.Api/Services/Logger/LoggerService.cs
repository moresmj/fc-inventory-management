using FC.Api.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Logger
{
    public class LoggerService : ILoggerService
    {
        private readonly AppSettings _appSettings;

        public LoggerService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

        }

        public void WriteLogs(string controller, string action, string exceptionType, string logErrorMessage, string jwtToken)
        {
            var configSettings = string.Empty;
            var logFilePrefix = string.Empty;
            var currentYear = DateTime.Now.Year.ToString();
            var currentMonth = DateTime.Now.Month.ToString("D2");
            var currentDay = DateTime.Now.Day.ToString("D2");


            
            var logPath = InitializeLoggerPath(_appSettings.LogPath, currentYear, currentMonth, currentDay);
            var logfileName = logFilePrefix + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            var getPath = Path.Combine(logPath, logfileName);

            if (!File.Exists(getPath))
            {
                using (var stream = new FileStream(getPath, FileMode.Create)) { }
            }
            try
            {
                using (StreamWriter writer = File.AppendText(getPath))
                {
                    Log(controller, action,  exceptionType, logErrorMessage, jwtToken, writer);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void Log(string controller, string action,string exceptionType, string logErrorMessage, string jwtToken, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine("--------------------------------------------------------------");
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("Controller : {0}", controller);
                txtWriter.WriteLine("Action : {0}", action);
                txtWriter.WriteLine("Exception Type : {0}", exceptionType);
                txtWriter.WriteLine("Log Message : {0}", logErrorMessage);
                txtWriter.WriteLine("CurrentSession JWT : {0}", jwtToken);
                txtWriter.WriteLine("--------------------------------------------------------------");
            }
            catch
            {
            }
        }

        private string InitializeLoggerPath(string configSettings, string currentYear, string currentMonth, string currentDay)
        {
            var rootPath = Path.Combine(configSettings);
            //Rooth Path Creation (wwwroot) if does not exists
            Directory.CreateDirectory(rootPath);

            var logPath = Path.Combine(rootPath, "logs");
            //Log Path Creation (wwwroot) if does not exists
            Directory.CreateDirectory(logPath);

            var yearPath = Path.Combine(logPath, currentYear);
            Directory.CreateDirectory(yearPath);

            var monthPath = Path.Combine(yearPath, currentMonth);
            Directory.CreateDirectory(monthPath);

            return monthPath;
        }
    }
}
