using System;
using BL.CrossCutting.Interfaces;

namespace BL.CrossCutting.Logging
{
    public class Logger :ILogger
    {
        public void Trace(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Debug(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Information(string message, params object[] args)
        {

        }

        public void Warning(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception exception, string message = null, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Fatal(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Fatal(Exception exception, string message = null, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}