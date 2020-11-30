﻿using SitePlugin;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NicoSitePlugin
{
    internal interface INicoCommentProviderInternal2
    {
        void BeforeConnect();
        void AfterDisconnected();
        Task ConnectAsync(string input, CookieContainer cc);
        void Disconnect();
        event EventHandler<IMessageContext2> MessageReceived;
        event EventHandler<IMetadata> MetadataUpdated;
        event EventHandler<ConnectedEventArgs> Connected;
        bool IsValidInput(string input);
        Task PostCommentAsync(string comment, string mail);
        void SetMessage(string raw);
    }
}
