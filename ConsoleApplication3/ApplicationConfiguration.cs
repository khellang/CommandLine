namespace ConsoleApplication3
{
    public class ApplicationConfiguration
    {
        public ApplicationConfiguration()
        {
            HandleErrors = true;
        }

        internal bool HandleErrors { get; set; }
    }
}