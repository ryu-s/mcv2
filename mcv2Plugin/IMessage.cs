using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcv2
{
    public interface IMessage
    {
        //string Raw{ get; }//実装したいけど結構大変な作業になりそう...
        //JsonConvert.SerializeObject()とかすればできそうな気もするけど、Rawもそのオブジェクトに含まれているからできないんじゃないかな
        //自身のSerializeした状態を持つ状況っておかしい？
        //それ自体に持たせなくても必要な時に第三者がSerializeすれば良くね？
        //{"type":"request","data":{"type":"connection","connection":"add",...}}
        //{"type":"response","reqid":"...","data":{"type":"connection","connection":"error",...}}
        //{"type":"request","data":{"type":"sitemessage",...}}
        //みたいな形式にしたいから単純にSerializeするのは不可能かも。
        //なんでこの形式にしたいのかと言うと分かりやすさ3割。自己満7割。
        //Rustとか他の言語でも実装することを考えると汎用的なシリアライズ化は必須。
    }
}
