﻿using System;
using System.Collections.Generic;
using System.Net;

namespace ryu_s.BrowserCookie
{
    public class UnknownProfile : IBrowserProfile2
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Path { get; }

        public string ProfileName { get; }

        public BrowserType Type { get; }

        public Cookie GetCookie(string domain, string name)
        {
            return new Cookie("", "", "", "");
        }

        public List<Cookie> GetCookieCollection(string domain)
        {
            return new List<Cookie>();
        }
        public UnknownProfile()
        {
            Path = null;
            ProfileName = null;
            Type = BrowserType.Unknown;
        }
    }
    public class UnknownManager : IBrowserManager
    {
        public BrowserType Type { get; }

        public List<IBrowserProfile2> GetProfiles()
        {
            return new List<IBrowserProfile2>()
            {
                new UnknownProfile(),
            };
        }
        public UnknownManager()
        {
            Type = BrowserType.Unknown;
        }
    }
}
