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
    }
}
