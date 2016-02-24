using System;
using System.Collections.Generic;
using Ninject;

namespace BL.Logic.DependencyInjection
{
    public interface IResolver
    {
        T Get<T>(Type type = null, IEnumerable<KeyValuePair<string, object>> args = null);
        T TryGet<T>(Type type = null, IEnumerable<KeyValuePair<string, object>> args = null);
        IEnumerable<T> GetAll<T>(Type type = null, IEnumerable<KeyValuePair<string, object>> args = null);

        IKernel Kernel { get; set; }
    }
}