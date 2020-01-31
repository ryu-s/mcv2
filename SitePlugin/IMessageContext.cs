using System;

namespace SitePlugin
{
    public interface IMessageContext2
    {
        //以下の3つを持たせる必要がある。
        //Message本体
        //色、フォント, IsInitialComment, IsFirstCommentなどの本メッセージに関連する値
        //コピー、URLを開く、AfterCommentAdded()といったコマンド、動作




        ISiteMessage Message { get; }
        IMessageMetadata2 Metadata { get; }
    }
}
