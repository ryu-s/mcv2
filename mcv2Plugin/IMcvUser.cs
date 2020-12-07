using SitePlugin;
using System.Collections.Generic;

namespace mcv2
{
    public interface IMcvUser
    {
        /// <summary>
        /// 名前。保存はされない
        /// </summary>
        IEnumerable<IMessagePart> Name { get; }
        /// <summary>
        /// コメビュ独自で付けた名前
        /// </summary>
        string? Nickname { get; }
        /// <summary>
        /// ユーザーID
        /// </summary>
        string Id { get; }
        /// <summary>
        /// コメビュ独自のNGユーザーか
        /// </summary>
        bool IsNgUser { get; }
        /// <summary>
        /// 配信サイト上で設定されたNGユーザーか
        /// </summary>
        bool IsSiteNgUser { get; }
        SitePluginId SiteId { get; }

        void Update(IMcvUserDiff diff);
    }
    /// <summary>
    /// ユーザー情報の変更前と後の差分
    /// </summary>
    public interface IMcvUserDiff
    {
        SitePluginId SiteId { get; }
        string UserId { get; }
        IEnumerable<IMessagePart>? Name { get; }
        string? Nickname { get; }
        bool? IsNgUser { get; }
        bool? IsSiteNgUser { get; }
    }
}
