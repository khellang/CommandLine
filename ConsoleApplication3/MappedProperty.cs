using System.Diagnostics;
using System.Reflection;

namespace ConsoleApplication3
{
    [DebuggerDisplay("{Name, nq}")]
    internal class MappedProperty<TResult>
    {
        public MappedProperty(Configuration<TResult> config, string name, PropertyInfo property)
        {
            Config = config;
            Name = name;
            Property = property;
        }

        public Configuration<TResult> Config { get; }

        public string Name { get; }

        public PropertyInfo Property { get; }
    }
}