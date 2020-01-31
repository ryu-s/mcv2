using Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTests
{
    [Serializable]
    public class ParseException : Exception
    {
        public string Raw { get; }
        public ParseException() { }
        public ParseException(string raw)
        {
            Raw = raw;
        }
        public ParseException(string raw, Exception inner) : base("", inner)
        {
            Raw = raw;
        }

        protected ParseException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
    [TestFixture]
    class LoggerTests
    {
        [Test]
        public void Test0()
        {
            var logger = new Common.LoggerTest2();
            logger.LogException(new Exception());
            var s = LoggerTest2.Parse(logger.GetLogData());
            Assert.AreEqual("{\"type\":\"Common.ExceptionData\",\"data\":{\"Ex\":{\"Name\":\"System.Exception\",\"Message\":\"種類 'System.Exception' の例外がスローされました。\",\"StackTrace\":null,\"Timestamp\":\"2020/10/28 04:17:43\",\"InnerError\":null,\"Properties\":{\"Message\":\"種類 'System.Exception' の例外がスローされました。\",\"StackTrace\":null,\"HelpLink\":null,\"Source\":null}},\"Message\":\"\",\"RawData\":\"\"}},\r\n", s);
        }
        [Test]
        public void Test1()
        {
            var logger = new Common.LoggerTest2();
            logger.LogUnknownData("rawdata", "message");
            var s = LoggerTest2.Parse(logger.GetLogData());
            Assert.AreEqual("{\"type\":\"Common.UnknownData\",\"data\":{\"RawData\":\"rawdata\",\"Message\":\"message\",\"Timestamp\":\"2020/10/28 04:18:34\"}},\r\n", s);
        }
        [Test]
        public void Test()
        {
            var logger = new Common.LoggerTest2();
            logger.LogException(new ParseException("{\"is_running\":1,\"gift_event_id\":\"39\",\"t\":65}", new Exception("exception message")), "parse", "{\"created_at\":1600952336,\"is_emomo_wipe_enabled\":\"0\",\"t\":53}");
        }
    }
}
