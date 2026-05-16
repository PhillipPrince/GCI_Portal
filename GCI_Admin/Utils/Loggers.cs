using System;
using System.IO;
using System.Reflection;

namespace Utils
{
    public static class Loggers
    {
        private static readonly object _lock = new object();
        private static string _methodName = string.Empty;

        // =========================
        // ERROR MESSAGE FORMATTER
        // =========================
        private static string PrepareErrorMessage(string methodName, Exception exception)
        {
            try
            {
                return $"{methodName} - Line No:{GetErrorLineNumber(exception)} - {exception.Message}";
            }
            catch (Exception ex)
            {
                return $"{methodName} - Line No: - {ex.Message}";
            }
        }

        // =========================
        // DAILY LOG PATH (ADMIN)
        // =========================
        private static string GetAdminLogPath(string fileName)
        {
            string basePath = @"C:\GCI\GCI_Logs";

            string folder = Path.Combine(basePath, DateTime.Now.ToString("yyyyMMdd"));

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return Path.Combine(folder, fileName);
        }

        // =========================
        // ADMIN ERROR LOG
        // =========================
        public static void DoLogs(string errMsg)
        {
            try
            {
                lock (_lock)
                {
                    string path = GetAdminLogPath("AdminErrorLogs.log");

                    errMsg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {errMsg}";

                    File.AppendAllText(path, errMsg + Environment.NewLine);
                }
            }
            catch
            {
                // avoid recursive crash
            }
        }

        // =========================
        // ADMIN EVENT LOG
        // =========================
        public static void EventLogs(string errMsg)
        {
            try
            {
                lock (_lock)
                {
                    string path = GetAdminLogPath("AdminEventLogs.log");

                    errMsg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {errMsg}";

                    File.AppendAllText(path, errMsg + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                DoLogs(ex.Message);
            }
        }

        // =========================
        // METHOD ERROR LOGGING
        // =========================
        public static void LogMethodsErrorDetails(string method, Exception exception)
        {
            try
            {
                _methodName = method;
                DoLogs(PrepareErrorMessage(_methodName, exception));
            }
            catch (Exception ex)
            {
                var reflectedType = MethodBase.GetCurrentMethod()?.ReflectedType;
                if (reflectedType != null)
                    DoLogs(PrepareErrorMessage(reflectedType.Name, ex));
            }
        }

        // =========================
        // GET LINE NUMBER
        // =========================
        private static string GetErrorLineNumber(Exception ex)
        {
            try
            {
                var line = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                return line.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}