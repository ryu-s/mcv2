using System;
using SitePlugin;
using Common;
using System.Xml.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.Diagnostics;
using System.Linq;
using System.Web;
namespace Common
{
    public interface ILogger
    {
        List<ILogData> GetLogData();
        void LogException(Exception ex, string message = "", string rawData = "");
        void LogUnknownData(string rawData, string message = "");

    }
    public interface ILogData
    {
    }
    public class UnknownData : ILogData
    {
        public UnknownData(string rawData, string message)
        {
            RawData = rawData;
            Message = message;
        }

        public string RawData { get; }
        public string Message { get; }
        public string Timestamp { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }
    public class ExceptionData : ILogData
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="rawData"></param>
        public ExceptionData(Exception ex, string message, string rawData)
        {
            Ex = new ExceptionInfo(ex);
            Message = message;
            RawData = rawData;
        }
        public ExceptionData() { }
        public ExceptionInfo Ex { get; set; }
        public string Message { get; }
        public string RawData { get; }
    }
    [Serializable]
    public class ExceptionInfo
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Timestamp { get; set; }
        public ExceptionInfo[] InnerError { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        //public string Raw
        //{
        //    get
        //    {
        //        var name = $"\"{Name}\"";
        //        var message = $"\"{Message}\"";
        //        var stackTrace = $"\"{HttpUtility.JavaScriptStringEncode(StackTrace)}\"";
        //        var timestamp = $"\"{Timestamp}\"";
        //        string innerException;
        //        var a = InnerError?.Select(x => x.Raw);
        //        if (a != null)
        //        {
        //            innerException = $"[{string.Join(",", a)}]";
        //        }
        //        else
        //        {
        //            innerException = "[]";
        //        }
        //        string properties;
        //        var list = new List<string>();
        //        foreach (var kv in Properties.Select(kv => kv))
        //        {
        //            var k = kv.Key;
        //            var v = kv.Value;
        //            list.Add($"\"{k}\":\"{HttpUtility.JavaScriptStringEncode(v)}\"");
        //        }
        //        properties = $"{{{string.Join(",", list)}}}";
        //        var s = $"{{\"name\":{name},\"message\":{message},\"stack_trace\":{stackTrace},\"timestamp\":{timestamp},\"innerExceptions\":{innerException},\"properties\":{properties}}}";
        //        return s;
        //    }
        //}
        private ExceptionInfo()
        {
            Timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
        public ExceptionInfo(Exception ex) : this()
        {
            Name = ex.GetType().FullName;
            Message = ex.Message;
            StackTrace = ex.StackTrace;
            SetProperties(ex);

            if (ex.InnerException != null)
            {
                InnerError = new ExceptionInfo[1];
                InnerError[0] = new ExceptionInfo(ex.InnerException);
            }
        }
        public ExceptionInfo(WebException ex) : this((Exception)ex)
        {
            Properties.Add(nameof(ex.Status), ex.Status.ToString());
            if (ex.Response is HttpWebResponse http)
            {
                Properties.Add(nameof(http.StatusCode), http.StatusCode.ToString());
                using (var sr = new System.IO.StreamReader(http.GetResponseStream()))
                {
                    var s = sr.ReadToEnd();
                    Properties.Add("Response", s.Replace("\"", "\\\""));
                }
            }
        }
        private void SetProperties(Exception ex)
        {
            try
            {
                var properties = ex.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var get = property.GetGetMethod();
                        var name = property.Name;
                        var s = (string)get.Invoke(ex, null);
                        Properties.Add(name, s);
                    }
                }
            }
            catch (Exception ex1)
            {
                Debug.WriteLine(ex1.Message);
            }
        }
        public ExceptionInfo(AggregateException ex) : this()
        {
            Name = ex.GetType().FullName;
            Message = ex.Message;
            StackTrace = ex.StackTrace;
            var innerCount = ex.InnerExceptions.Count;
            InnerError = new ExceptionInfo[innerCount];
            for (int i = 0; i < innerCount; i++)
            {
                InnerError[i] = new ExceptionInfo(ex.InnerExceptions[i]);
            }
        }
    }
    public class LoggerTest2 : ILogger
    {
        readonly List<ILogData> _data = new List<ILogData>();
        public static List<ILogData> Parse(string json)
        {
            throw new NotImplementedException();
        }
        public static string Parse(List<ILogData> data)
        {
            var sb = new StringBuilder();
            foreach (var n in data)
            {
                sb.Append($"{{\"type\":\"{n.GetType()}\",\"data\":");
                sb.Append(JsonConvert.SerializeObject(n));
                sb.Append("},");
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
        public List<ILogData> GetLogData()
        {
            return _data;
        }

        public void LogException(Exception ex, string message = "", string rawData = "")
        {
            _data.Add(new ExceptionData(ex, message, rawData));
        }

        public void LogUnknownData(string rawData, string message = "")
        {
            _data.Add(new UnknownData(rawData, message));

        }
    }
}
