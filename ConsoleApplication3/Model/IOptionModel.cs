using System.Reflection;

namespace ConsoleApplication3.Model
{
    internal interface IOptionModel
    {
        string Name { get; }

        PropertyInfo Property { get; }
    }
}