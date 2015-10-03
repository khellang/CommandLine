using System.Reflection;

namespace ConsoleApplication3.Model
{
    internal interface IOptionModel<TResult>
    {
        string Name { get; }

        PropertyInfo Property { get; }

        ApplicationConfiguration<TResult> Config { get; }
    }
}