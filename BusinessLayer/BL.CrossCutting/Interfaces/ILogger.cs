using System;

namespace BL.CrossCutting.Interfaces
{
    public interface ILogger
    {
        void Trace(IContext ctx, string message, params object[] args);
        void Information(IContext ctx, string message);
        void Warning(IContext ctx, string message, params object[] args);
        void Error(IContext ctx, string message, params object[] args);
        void Error(IContext ctx, Exception exception, string message = null, params object[] args);
        void Fatal(IContext ctx, string message, params object[] args);
        void Fatal(IContext ctx, Exception exception, string message = null, params object[] args);
    }
}