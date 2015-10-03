using System;
using ConsoleApplication3.Model;

namespace ConsoleApplication3
{
    public static class Application
    {
        public static TResult Run<TResult>(ApplicationConfiguration<TResult> config, string[] args, Action<IApplicationModelBuilder<TResult>> build)
        {
            return Create(config, build).Run(args);
        }

        internal static IApplicationModel<TResult> Create<TResult>(ApplicationConfiguration<TResult> config, Action<IApplicationModelBuilder<TResult>> build)
        {
            var builder = new ApplicationModelBuilder<TResult>(config);

            build(builder);

            return builder.Build();
        }
    }
}