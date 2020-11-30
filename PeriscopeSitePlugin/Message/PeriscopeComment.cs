using PeriscopeSitePlugin;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PeriscopeSitePlugin
{
    internal class PeriscopeComment : MessageBase2, IPeriscopeComment
    {
        public override SiteType SiteType { get; } = SiteType.Periscope;
        public PeriscopeMessageType PeriscopeMessageType { get; } = PeriscopeMessageType.Comment;
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime PostedAt { get; set; }
        public string Text { get; }
        public string DisplayName { get; }
        public PeriscopeComment(Kind1Type1 kind1Type1) : base(kind1Type1.Raw)
        {
            Id = kind1Type1.Uuid;
            UserId = kind1Type1.UserId;
            Text = kind1Type1.Body;
            DisplayName = kind1Type1.DisplayName;
            //timestampの長さが10,13,19の場合を確認済み。全て10にして処理する
            var timestampStr = kind1Type1.Timestamp.ToString();
            var timestamp10Str = new string(timestampStr.Take(10).ToArray());
            PostedAt = DateTimeFromUnixTimestampSec(long.Parse(timestamp10Str)).ToLocalTime();
        }
        static readonly DateTime BaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime DateTimeFromUnixTimestampSec(long sec)
        {
            return BaseTime.AddSeconds(sec);
        }
    }
}
