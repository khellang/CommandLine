﻿using System.Collections.Generic;

namespace ConsoleApplication3
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var config = new Configuration<int>();

            return Application.Run<Pull, int>(config, args, app =>
            {
                app.AddCommand<Pull>("command", cmd =>
                {
                    cmd.Argument("path", x => x.Path);

                    cmd.Option("b|boolean", x => x.Boolean);
                    cmd.Option("s|string", x => x.String);
                    cmd.Option("i|integer", x => x.Integer);
                    cmd.Option("d|double", x => x.Double);
                    cmd.Option("string-list", x => x.StringList);
                    cmd.Option("integer-list", x => x.IntegerList);

                    return NoOp;
                });

                return NoOp;
            });
        }

        private static int NoOp<T>(T args)
        {
            return 0;
        }

        private class Pull
        {
            public string Path { get; set; }

            public bool Boolean { get; set; }

            public string String { get; set; }

            public int Integer { get; set; }

            public double Double { get; set; }

            public IEnumerable<string> StringList { get; set; }

            public IEnumerable<int> IntegerList { get; set; }
        }
    }
}