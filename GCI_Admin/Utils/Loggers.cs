using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class Loggers
    {
        private static string _methodName = string.Empty;
        private static string PrepareErrorMessage(string methodName, Exception exception)
        {
            try
            {
                var message = $"{methodName} - Line No:{GetErrorLineNumber(exception)} - {exception.Message}";
                return message;
            }
            catch (Exception ex)
            {
                return $"{methodName} - Line No: - {ex.Message}";
            }
        }

        public static void DoLogs(string errMsg)
        {
            try
            {
                DateTime currtime = DateTime.Now;
                errMsg = errMsg + currtime;

                string appPath = Path.GetDirectoryName("C:\\GCI");
                appPath = appPath + "\\AdminErrors\\" + DateTime.Now.ToString("yyyyMMdd");
                if (!Directory.Exists(appPath))
                    Directory.CreateDirectory(appPath);
                appPath = appPath + "\\errorlog.txt";
                using (StreamWriter sw = File.AppendText(appPath))
                {
                    sw.WriteLine(errMsg);
                }
            }
            catch (Exception ex)
            {
                DoLogs(ex.Message);
            }
        }

        public static void LogMethodsErrorDetails(string method, Exception exception, int hasMode, int mode)
        {
            try
            {
                _methodName = hasMode == 1 ? $"{method}({mode})" : method;

                DoLogs(PrepareErrorMessage(_methodName, exception));
            }
            catch (Exception ex)
            {
                var reflectedType = MethodBase.GetCurrentMethod()?.ReflectedType;
                if (reflectedType != null)
                    DoLogs(PrepareErrorMessage(reflectedType.Name, ex));
            }
        }

        private static string GetErrorLineNumber(Exception ex)
        {
            try
            {
                var line = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                return line.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

}
