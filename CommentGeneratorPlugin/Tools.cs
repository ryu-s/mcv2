using Common.Wpf;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommentGeneratorPlugin
{
    static class Tools
    {
        public static long ToUnixTime(DateTime dateTime)
        {
            // 時刻をUTCに変換
            dateTime = dateTime.ToUniversalTime();

            // unix epochからの経過秒数を求める
            return (long)dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
        public static (string? name, string? comment) GetData(ISiteMessage message)
        {
            string? name;
            string? comment;

            if (message is YouTubeLiveSitePlugin.IYouTubeLiveComment ytComment)
            {
                comment = ytComment.CommentItems.ToText();
                name = ytComment.NameItems.ToText();
            }
            else if (message is YouTubeLiveSitePlugin.IYouTubeLiveSuperchat superchat)
            {
                comment = superchat.CommentItems.ToText();
                name = superchat.NameItems.ToText();
            }
            else if (message is NicoSitePlugin.INicoComment nicoComment)
            {
                comment = nicoComment.Text;
                name = nicoComment.UserName;
            }
            else if (message is NicoSitePlugin.INicoItem nicoItem)
            {
                comment = nicoItem.Text;
                name = null;
            }
            else if (message is TwitchSitePlugin.ITwitchComment twComment)
            {
                comment = twComment.CommentItems.ToText();
                name = twComment.DisplayName;
            }
            else if (message is TwicasSitePlugin.ITwicasComment casComment)
            {
                comment = casComment.CommentItems.ToText();
                name = casComment.UserName;
            }
            else if (message is TwicasSitePlugin.ITwicasKiitos casKiitos)
            {
                comment = casKiitos.CommentItems.ToText();
                name = casKiitos.UserName;
            }
            else if (message is TwicasSitePlugin.ITwicasItem casItem)
            {
                comment = casItem.CommentItems.ToText();
                name = casItem.UserName;
            }
            else if (message is WhowatchSitePlugin.IWhowatchComment wwComment)
            {
                comment = wwComment.Comment;
                name = wwComment.UserName;
            }
            else if (message is WhowatchSitePlugin.IWhowatchItem wwItem)
            {
                comment = wwItem.Comment;
                name = wwItem.UserName;
            }
            else if (message is OpenrecSitePlugin.IOpenrecComment opComment)
            {
                comment = opComment.MessageItems.ToText();
                name = opComment.NameItems.ToText();
            }
            else if (message is MirrativSitePlugin.IMirrativComment mrComment)
            {
                comment = mrComment.Text;
                name = mrComment.UserName;
            }
            else if (message is MirrativSitePlugin.IMirrativJoinRoom mrJoin)
            {
                comment = mrJoin.Text;
                name = mrJoin.UserName;
            }
            else if (message is MirrativSitePlugin.IMirrativItem mrItem)
            {
                comment = mrItem.Text;
                name = mrItem.UserName;
            }
            else if (message is LineLiveSitePlugin.ILineLiveComment llComment)
            {
                comment = llComment.Text;
                name = llComment.DisplayName;
            }
            else if (message is PeriscopeSitePlugin.IPeriscopeComment psComment)
            {
                comment = psComment.Text;
                name = psComment.DisplayName;
            }
            else if (message is ShowRoomSitePlugin.IShowRoomComment srComment)
            {
                comment = srComment.Text;
                name = srComment.UserName;
            }
            else if (message is MildomSitePlugin.IMildomComment mildomComment)
            {
                comment = mildomComment.CommentItems.ToText();
                name = mildomComment.UserName;
            }
            else
            {
                name = null;
                comment = null;
            }
            return (name, comment);
        }
    }
}
