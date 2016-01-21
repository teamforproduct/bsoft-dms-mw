using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BL.CrossCutting.Extensions;
using Ninject;
using Ninject.Parameters;

namespace BL.CrossCutting.DependencyInjection
{
    public class NinjectResolver:IResolver
    {
        public IKernel Kernel { get; set; }

        public NinjectResolver()
        {
            var kernel = new StandardKernel();
            var appAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x=>x.FullName.ToUpper().StartsWith("BL.") || x.FullName.ToUpper().StartsWith("DMS"));
            kernel.Load(appAssemblies);
            Kernel = kernel;
        }

        public T Get<T>(Type type = null, IEnumerable<KeyValuePair<string, object>> args = null)
        {
            var @params = args
                .IfNullThenEmpty()
                .Select(o => new ConstructorArgument(o.Key, o.Value) as IParameter)
                .ToArray();
            return type == null
                ? Kernel.Get<T>(@params)
                : (T)Kernel.Get(type, @params);
        }

        public T TryGet<T>(Type type = null, IEnumerable<KeyValuePair<string, object>> args = null)
        {
            var @params = args
                .IfNullThenEmpty()
                .Select(o => new ConstructorArgument(o.Key, o.Value) as IParameter)
                .ToArray();
            return type == null
                ? Kernel.TryGet<T>(@params)
                : (T)Kernel.TryGet(type, @params);
        }

        public IEnumerable<T> GetAll<T>(Type type = null, IEnumerable<KeyValuePair<string, object>> args = null)
        {
            var @params = args
                .IfNullThenEmpty()
                .Select(o => new ConstructorArgument(o.Key, o.Value) as IParameter)
                .ToArray();
            return type == null
                ? Kernel.GetAll<T>(@params)
                : Kernel.GetAll(type, @params).Cast<T>();
        }
    }
}