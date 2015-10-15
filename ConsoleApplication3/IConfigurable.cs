namespace ConsoleApplication3
{
    public interface IConfigurable<TResult>
    {
        Configuration<TResult> Config { get; }
    }
}