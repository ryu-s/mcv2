﻿using Common;

namespace mcv2.MainViewPlugin
{
    internal class TwitchSiteOptions : DynamicOptionsBase, ITwitchSiteOptions
    {
        /// <summary>
        /// @コテハンを自動登録するか
        /// </summary>
        public bool NeedAutoSubNickname { get => GetValue(); set => SetValue(value); }
        public string NeedAutoSubNicknameStr { get => GetValue(); set => SetValue(value); }
        protected override void Init()
        {
            Dict.Add(nameof(NeedAutoSubNickname), new Item { DefaultValue = false, Predicate = b => true, Serializer = b => b.ToString(), Deserializer = s => bool.Parse(s) });
            Dict.Add(nameof(NeedAutoSubNicknameStr), new Item { DefaultValue = "@|＠", Predicate = s => !string.IsNullOrEmpty(s), Serializer = s => s, Deserializer = s => s });
        }
        internal TwitchSiteOptions Clone()
        {
            return (TwitchSiteOptions)this.MemberwiseClone();
        }
        internal void Set(TwitchSiteOptions changedOptions)
        {
            foreach (var src in changedOptions.Dict)
            {
                var v = src.Value;
                SetValue(v.Value, src.Key);
            }
        }
        internal void Set(ITwitchSiteOptions changedOptions)
        {
            var props = typeof(ITwitchSiteOptions).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.SetMethod is null) continue;
                prop.SetValue(this, prop.GetValue(changedOptions));
            }
        }
    }
}
