using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeLiveIF
{
    public interface UserType
    {
        string TypeName { get; }
    }
    public class NormalUser : UserType
    {
        public string TypeName { get; } = "";
    }
    public class Moderator : UserType
    {
        public string TypeName { get; } = "モデレーター";
        public string TestInfo { get; set; }
    }
    public class Member : UserType
    {
        public string TypeName { get; } = "メンバー";
        public string Long { get; set; }
        public string TestInfo { get; set; }
    }
}
