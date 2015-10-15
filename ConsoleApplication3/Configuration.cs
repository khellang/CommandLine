using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication3
{
    public class Configuration<TResult>
    {
        public Configuration()
        {
            HandleErrors = true;
            ErrorHandler = DefaultErrorHandler;
            ArgumentActivator = DefaultActivator;
            StringComparer = StringComparer.Ordinal;
            CultureInfo = CultureInfo.InvariantCulture;
            ResponseFileReader = DefaultResponseFileReader;
            ResponseFileEncoding = Encoding.UTF8;
            Conventions = new ConfigurationConventions();
        }

        public CultureInfo CultureInfo { get; set; }

        public StringComparer StringComparer { get; set; }

        public Func<Type, object> ArgumentActivator { get; set; }

        public Func<Exception, TResult> ErrorHandler { get; set; }

        public Func<string, IEnumerable<string>> ResponseFileReader { get; set; }

        public Encoding ResponseFileEncoding { get; set; }

        public ConfigurationConventions Conventions { get; }

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

        public IEnumerable<string> DefaultResponseFileReader(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllLines(path, ResponseFileEncoding).SelectMany(x => x.Split(' '));
            }

            throw new FileNotFoundException($"Could not find response file '{path}'");
        }
    }
}