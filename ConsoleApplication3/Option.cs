using System.Diagnostics;
using System.Reflection;

namespace ConsoleApplication3
{
    [DebuggerDisplay("{Name, nq}")]
    internal class Option<TResult>
    {
        public Option(ApplicationConfiguration<TResult> config, string name, PropertyInfo property)
        {
            Config = config;
            Name = name;
            Property = property;
        }

        public ApplicationConfiguration<TResult> Config { get; }

        public string Name { get; }

        public PropertyInfo Property { get; }
    }
}