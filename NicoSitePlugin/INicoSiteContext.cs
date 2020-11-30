﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SitePlugin;
namespace NicoSitePlugin
{
    public interface INicoSiteContext2 : ISiteContext
    {
        INicoCommentProvider2 CreateNicoCommentProvider();
        INicoSiteOptions GetNicoSiteOptions();
    }
}
