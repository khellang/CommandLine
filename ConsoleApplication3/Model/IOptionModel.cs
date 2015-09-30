using System.Reflection;

namespace ConsoleApplication3
{
    internal interface IOptionModel
    {
        string Name { get; }

        PropertyInfo Property { get; }
    }
}