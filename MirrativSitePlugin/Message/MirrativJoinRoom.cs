﻿using SitePlugin;
using System;
using System.Collections.Generic;

namespace MirrativSitePlugin
{
    internal class MirrativJoinRoom : MessageBase2, IMirrativJoinRoom
    {
        public override SiteType SiteType { get; } = SiteType.Mirrativ;
        public MirrativMessageType MirrativMessageType { get; } = MirrativMessageType.JoinRoom;
        public string Text { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime PostedAt { get; }
        public IMessageImage UserIcon { get; set; }
        public int OnlineViewerNum { get; set; }
        public MirrativJoinRoom(Message commentData, string raw) : base(raw)
        {
            UserId = commentData.UserId;
            Id = commentData.Id;
            Text = commentData.Comment;
            UserName = commentData.Username;
            UserIcon = null;
            PostedAt = SitePluginCommon.Utils.UnixtimeToDateTime(commentData.CreatedAt);
        }
    }
}
