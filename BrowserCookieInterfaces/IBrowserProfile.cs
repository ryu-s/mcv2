using System;
using System.Collections.Generic;
using System.Net;

namespace ryu_s.BrowserCookie
{
    public interface IBrowserProfile2
    {
        Guid Id { get; }
        string Path { get; }
        string ProfileName { get; }
        BrowserType Type { get; }
        Cookie GetCookie(string domain, string name);
        List<Cookie> GetCookieCollection(string domain);
    }
}
