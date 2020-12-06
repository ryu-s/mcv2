using mcv2;
using System;
using System.Threading.Tasks;

namespace SitePlugin
{
    public sealed class CommentProviderId
    {
        private Guid _guid;
        public CommentProviderId()
        {
            _guid = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return obj is CommentProviderId id &&
                   _guid.Equals(id._guid);
        }

        public override int GetHashCode()
        {
            return -2045414129 + _guid.GetHashCode();
        }
    }
    /// <summary>
    /// SitePluginの利用者
    /// </summary>
    public interface ISitePluginHost
    {
        //void SetUserData(string userId, bool isSiteNgUser);
        /// <summary>
        /// SitePluginで発生したエラーを通知する
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="raw"></param>
        void SetError(Exception ex, string message, string raw);
        /// <summary>
        /// 取得したコメントや入室メッセージなどを渡す
        /// </summary>
        /// <param name="cpId"></param>
        /// <param name="message"></param>
        /// <param name="metadata"></param>
        void SetMessage(CommentProviderId cpId, ISiteMessage message, IMessageMetadata2 metadata);
        /// <summary>
        /// 配信のタイトルや視聴者数等を渡す
        /// </summary>
        /// <param name="cpId"></param>
        /// <param name="metadata"></param>
        void SetMetadata(CommentProviderId cpId, IMetadata metadata);
    }
    /// <summary>
    /// 配信サイト毎の設定値
    /// </summary>
    public interface ISiteOptions { }
    /// <summary>
    /// SitePluginの本体
    /// </summary>
    public interface IMcvSitePlugin
    {
        IMcvCommentProvider CreateCommentProvider();
    }
    /// <summary>
    /// コメントを取得する際に必要な情報。配信サイト毎に異なる。
    /// URLやCookie、ツイキャスのプライベート配信のパスワード
    /// </summary>
    public interface IConnectOptions { }
    /// <summary>
    /// コメントを取得したり、投稿してくれるやつ
    /// </summary>
    public interface IMcvCommentProvider
    {
        /// <summary>
        /// 各CommentProviderを識別するためのID
        /// </summary>
        CommentProviderId Id { get; }
        /// <summary>
        /// 設定値をセットする
        /// </summary>
        /// <param name="siteOptions"></param>
        void SetOptions(ISiteOptions siteOptions);
        /// <summary>
        /// コメントの取得を開始する
        /// </summary>
        /// <param name="connectOptions"></param>
        /// <returns></returns>
        Task ConnectAsync(IConnectOptions connectOptions);
        /// <summary>
        /// コメントを投稿する
        /// </summary>
        /// <param name="commentData"></param>
        /// <returns></returns>
        Task PostCommentAsync(ICommentDataToPost commentData);
    }

}
