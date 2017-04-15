﻿using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetUserClientServer
    {
        public List<int> UserServerIDs { get; set; }
        public List<string> UserIDs { get; set; }
        public List<int> ServerIDs { get; set; }
        public List<int> ClientIDs { get; set; }
    }
}
