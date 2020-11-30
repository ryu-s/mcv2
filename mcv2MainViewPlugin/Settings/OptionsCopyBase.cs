namespace mcv2.MainViewPlugin
{
    abstract class OptionsCopyBase<T>
    {
        public void Set(T changedOptions)
        {
            var props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.SetMethod is null) continue;
                prop.SetValue(this, prop.GetValue(changedOptions));
            }
        }
    }
}
