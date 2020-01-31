using SitePlugin;
using System.Collections.Generic;

namespace mcv2.Model
{
    public class McvUser : IMcvUser
    {
        public System.Guid Guid { get; } = System.Guid.NewGuid();
        public string? Nickname { get; set; }
        public string Id { get; }
        public bool IsNgUser { get; set; }
        public bool IsSiteNgUser { get; set; }
        public IEnumerable<IMessagePart> Name { get; set; }//Nameはどのサイトでも常にあるからnullableにしたくない
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">ユーザーIDが無いサイトはユーザー名をこれに充てる</param>
        public McvUser(string userId)
        {
            Id = userId;
        }
    }
}
