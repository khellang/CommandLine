using System;
using System.Globalization;

namespace ConsoleApplication3
{
    public class ApplicationConfiguration
    {
    }

    public class ApplicationConfiguration<TResult> : ApplicationConfiguration
    {
        public ApplicationConfiguration()
        {
            HandleErrors = true;
            ErrorHandler = DefaultErrorHandler;
            ArgumentActivator = DefaultActivator;
            StringComparer = StringComparer.Ordinal;
            CultureInfo = CultureInfo.InvariantCulture;
            Conventions = new ApplicationConfigurationConventions();
        }

        public CultureInfo CultureInfo { get; set; }

        public StringComparer StringComparer { get; set; }

        public Func<Type, object> ArgumentActivator { get; set; }

        public Func<Exception, TResult> ErrorHandler { get; set; }

        public ApplicationConfigurationConventions Conventions { get; protected set; }

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

        private static object DefaultActivator(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}