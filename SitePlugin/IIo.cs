﻿using System;
using System.Threading.Tasks;
namespace SitePlugin
{
    public interface IIo//Commonに移したい
    {
        string? ReadFile(string path);
        Task<string?> ReadFileAsync(string path);
        void WriteFile(string path, string s);
        Task WriteFileAsync(string path, string s);
    }
}
