using System;
using System.Globalization;

namespace ConsoleApplication3
{
    public class ApplicationConfiguration<TResult>
    {
        public ApplicationConfiguration()
        {
            CultureInfo = CultureInfo.InvariantCulture;
            ErrorHandler = DefaultErrorHandler;
            StringComparer = StringComparer.Ordinal;
            HandleErrors = true;
        }

        public CultureInfo CultureInfo { get; set; }

        public Func<Exception, TResult> ErrorHandler { get; set; }

        public StringComparer StringComparer { get; set; }

        internal bool HandleErrors { get; set; }

        private TResult DefaultErrorHandler(Exception exception)
        {
            if (HandleErrors)
            {
                Console.Error.WriteLine($"error: {exception.Message.ToLower(CultureInfo)}");
                Environment.Exit(1);
            }

            return default(TResult);
        }
    }
}