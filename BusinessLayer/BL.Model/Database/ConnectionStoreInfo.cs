using System;

namespace BL.Model.Database
{
    public class ConnectionStoreInfo
    {
        public DateTime LastUsege { get; set; }
        public object ConnectionObject { get; set; }
    }
}