using ryu_s.BrowserCookie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace SitePlugin
{
    public abstract class SitePluginbase : ISitePlugin
    {
        public void Connect(ConnectionId connectionId, string input, IBrowserProfile2 browser)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(ConnectionId connectionId)
        {
            throw new NotImplementedException();
        }

        public ISiteOptions2 GetSiteOptions()
        {
            throw new NotImplementedException();
        }

        public bool IsValidInput(string input)
        {
            throw new NotImplementedException();
        }

        public void SetSiteOptions(ISiteOptions2 siteOptions)
        {
            throw new NotImplementedException();
        }
    }
    public interface ISiteOptions2
    {

    }
    public interface ISitePlugin
    {
        void SetSiteOptions(ISiteOptions2 siteOptions);
        ISiteOptions2 GetSiteOptions();
        /// <summary>
        /// inputがこのサイトの入力値に適合しているか
        /// 形式が正しいかを判定するだけで、実際に存在するかは関知しない
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool IsValidInput(string input);
        //string LoggedInName(IBrowserProfile2 browser);
        void Connect(ConnectionId connectionId, string input, IBrowserProfile2 browser);
        void Disconnect(ConnectionId connectionId);
    }
    public interface ISiteContext
    {
        /// <summary>
        /// 
        /// </summary>
        SitePluginId Guid { get; }
        string DisplayName { get; }
        SiteType SiteType { get; }
        void SaveOptions(string path, IIo io);
        void LoadOptions(string path, IIo io);
        void Init();
        void Save();
        ICommentProvider2 CreateCommentProvider();
        /// <summary>
        /// inputがこのサイトの入力値に適合しているか
        /// 形式が正しいかを判定するだけで、実際に存在するかは関知しない
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool IsValidInput(string input);
    }
}

