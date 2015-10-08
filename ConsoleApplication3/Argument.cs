using System.Reflection;

namespace ConsoleApplication3
{
    internal class Argument<TResult>
    {
        public Argument(ApplicationConfiguration<TResult> config, string name, PropertyInfo property)
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