﻿using Common;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    internal class TwicasSiteOptions : DynamicOptionsBase, ITwicasSiteOptions
    {
        //コメント取得インターバル
        public int CommentRetrieveIntervalSec { get => GetValue(); set => SetValue(value); }
        //キートス
        public Color KiitosBackColor { get => GetValue(); set => SetValue(value); }
        public Color KiitosForeColor { get => GetValue(); set => SetValue(value); }

        public Color ItemBackColor { get => GetValue(); set => SetValue(value); }
        public Color ItemForeColor { get => GetValue(); set => SetValue(value); }

        public bool IsAutoSetNickname { get => GetValue(); set => SetValue(value); }
        public bool IsShowItem { get => GetValue(); set => SetValue(value); }
        protected override void Init()
        {
            Dict.Add(nameof(CommentRetrieveIntervalSec), new Item { DefaultValue = 1, Predicate = f => f > 0, Serializer = f => f.ToString(), Deserializer = s => int.Parse(s) });
            Dict.Add(nameof(KiitosBackColor), new Item { DefaultValue = ColorFromArgb("#FFFF0000"), Predicate = c => true, Serializer = c => ColorToArgb(c), Deserializer = s => ColorFromArgb(s) });
            Dict.Add(nameof(KiitosForeColor), new Item { DefaultValue = ColorFromArgb("#FFFFFFFF"), Predicate = c => true, Serializer = c => ColorToArgb(c), Deserializer = s => ColorFromArgb(s) });
            Dict.Add(nameof(ItemBackColor), new Item { DefaultValue = ColorFromArgb("#FFFF0000"), Predicate = c => true, Serializer = c => ColorToArgb(c), Deserializer = s => ColorFromArgb(s) });
            Dict.Add(nameof(ItemForeColor), new Item { DefaultValue = ColorFromArgb("#FFFFFFFF"), Predicate = c => true, Serializer = c => ColorToArgb(c), Deserializer = s => ColorFromArgb(s) });
            Dict.Add(nameof(IsAutoSetNickname), new Item { DefaultValue = false, Predicate = b => true, Serializer = b => b.ToString(), Deserializer = s => bool.Parse(s) });
            Dict.Add(nameof(IsShowItem), new Item { DefaultValue = true, Predicate = b => true, Serializer = b => b.ToString(), Deserializer = s => bool.Parse(s) });
        }
        public void Set(ITwicasSiteOptions changedOptions)
        {
            var props = typeof(ITwicasSiteOptions).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.SetMethod is null) continue;
                prop.SetValue(this, prop.GetValue(changedOptions));
            }
        }
        #region Converters
        private FontFamily FontFamilyFromString(string str)
        {
            return new FontFamily(str);
        }
        private string FontFamilyToString(FontFamily family)
        {
            return family.FamilyNames.Values.First();
        }
        private FontStyle FontStyleFromString(string str)
        {
            return (FontStyle)new FontStyleConverter().ConvertFromString(str);
        }
        private string FontStyleToString(FontStyle style)
        {
            return new FontStyleConverter().ConvertToString(style);
        }
        private FontWeight FontWeightFromString(string str)
        {
            return (FontWeight)new FontWeightConverter().ConvertFromString(str);
        }
        private string FontWeightToString(FontWeight weight)
        {
            return new FontWeightConverter().ConvertToString(weight);
        }
        private bool IsValidArgb(string argb)
        {
            const string pattern = "#(?<a>[0-9a-fA-F]{2})(?<r>[0-9a-fA-F]{2})(?<g>[0-9a-fA-F]{2})(?<b>[0-9a-fA-F]{2})";
            return System.Text.RegularExpressions.Regex.IsMatch(argb, pattern);
        }
        private Color ColorFromArgb(string argb)
        {
            if (argb == null)
                throw new ArgumentNullException("argb");
            var pattern = "#(?<a>[0-9a-fA-F]{2})(?<r>[0-9a-fA-F]{2})(?<g>[0-9a-fA-F]{2})(?<b>[0-9a-fA-F]{2})";
            var match = System.Text.RegularExpressions.Regex.Match(argb, pattern, System.Text.RegularExpressions.RegexOptions.Compiled);

            if (!match.Success)
            {
                throw new ArgumentException("形式が不正");
            }
            else
            {
                var a = byte.Parse(match.Groups["a"].Value, System.Globalization.NumberStyles.HexNumber);
                var r = byte.Parse(match.Groups["r"].Value, System.Globalization.NumberStyles.HexNumber);
                var g = byte.Parse(match.Groups["g"].Value, System.Globalization.NumberStyles.HexNumber);
                var b = byte.Parse(match.Groups["b"].Value, System.Globalization.NumberStyles.HexNumber);
                return Color.FromArgb(a, r, g, b);
            }
        }
        private string ColorToArgb(Color color)
        {
            var argb = color.ToString();
            return argb;
        }
        #endregion
    }
}
