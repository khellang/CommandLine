namespace ConsoleApplication3
{
    public interface IConfigurable<TResult>
    {
        ApplicationConfiguration<TResult> Config { get; }
    }
}