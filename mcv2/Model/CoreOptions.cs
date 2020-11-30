using Common;

namespace mcv2.Model
{
    class CoreOptions : DynamicOptionsBase, ICoreOptions
    {
        public string PluginDirPath { get => GetValue(); set => SetValue(value); }
        public string SettingsDirPath { get => GetValue(); set => SetValue(value); }
        public bool IsAutoUpdateCheck { get => GetValue(); set => SetValue(value); }
        public double MainViewPositionX { get => GetValue(); set => SetValue(value); }
        public double MainViewPositionY { get => GetValue(); set => SetValue(value); }

        protected override void Init()
        {
            Dict.Add(nameof(PluginDirPath), new Item { DefaultValue = "plugins", Predicate = c => true, Serializer = c => (string)c, Deserializer = s => s });
            Dict.Add(nameof(SettingsDirPath), new Item { DefaultValue = "settings", Predicate = c => true, Serializer = c => (string)c, Deserializer = s => s });
            Dict.Add(nameof(IsAutoUpdateCheck), new Item { DefaultValue = true, Predicate = b => true, Serializer = b => b.ToString(), Deserializer = s => bool.Parse(s) });
            Dict.Add(nameof(MainViewPositionX), new Item { DefaultValue = 0, Predicate = b => true, Serializer = b => b.ToString(), Deserializer = s => double.Parse(s) });
            Dict.Add(nameof(MainViewPositionY), new Item { DefaultValue = 0, Predicate = b => true, Serializer = b => b.ToString(), Deserializer = s => double.Parse(s) });

        }
    }


}
