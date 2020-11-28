using SitePlugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mcv2.Model
{
    public class McvUser : IMcvUser
    {
        public System.Guid Guid { get; }
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
            Guid = Guid.NewGuid();
            Id = userId;
        }
        [Newtonsoft.Json.JsonConstructor]
        private McvUser(Guid guid, string id)
        {
            Guid = guid;
            Id = id;
        }
        public static string Serialize(McvUser user)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(user, new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
            });
        }
        public static McvUser? Deserialize(string serializedUser)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(serializedUser, new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
            }) as McvUser;
        }
        public static bool HasChanged(McvUser user)
        {
            return user.Nickname != null || user.IsNgUser || user.IsSiteNgUser;
        }
    }
}
