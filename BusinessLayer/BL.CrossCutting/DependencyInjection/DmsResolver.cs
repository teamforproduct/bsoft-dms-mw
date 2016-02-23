using System;

namespace BL.Logic.DependencyInjection
{
    public class DmsResolver
    {
        private static readonly Lazy<NinjectResolver> _lazyNinjectResolver =
           new Lazy<NinjectResolver>(() => new NinjectResolver());

        private static IResolver _resolver;
        public static IResolver Current
        {
            get { return _resolver ?? _lazyNinjectResolver.Value; }
            set { _resolver = value; }
        }
    }
}