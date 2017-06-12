using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using System.Collections.Generic;

namespace DMS_WebAPI.Utilities
{
    public class UserContextsItem : AuthContext, IContextItem
    {
        private readonly Dictionary<string, IClientContext> _cacheContexts = new Dictionary<string, IClientContext>();

        public Dictionary<string, IClientContext> ClientContexts
        {
            get
            {
                return _cacheContexts;
            }
        }

        public string Key { get; set; }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            DefaultContext ctx = obj as DefaultContext;
            return ctx != null && string.Equals(this.Key, ctx.Key);
        }

    }
}