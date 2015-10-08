using System.Collections.Generic;

namespace ConsoleApplication3
{
    public static class Program
    {
        public static int Main(string[] stringArgs)
        {
            var config = new ApplicationConfiguration<int>();

            return Application.Run<int>(config, stringArgs, app =>
            {
                app.AddCommand<Pull>("command", cmd =>
                {
                    cmd.MapOption("b|boolean", x => x.Boolean);
                    cmd.MapOption("s|string", x => x.String);
                    cmd.MapOption("i|integer", x => x.Integer);
                    cmd.MapOption("d|double", x => x.Double);
                    cmd.MapOption("string-list", x => x.StringList);
                    cmd.MapOption("integer-list", x => x.IntegerList);

                    return args => 0;
                });
            });
        }

        private class Pull
        {
            public bool Boolean { get; set; }

            public string String { get; set; }

            public int Integer { get; set; }

            public double Double { get; set; }

            public IEnumerable<string> StringList { get; set; }

            public IEnumerable<int> IntegerList { get; set; }
        }
    }
}