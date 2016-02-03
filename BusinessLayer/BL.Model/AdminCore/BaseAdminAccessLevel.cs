﻿using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.AdminCore
{
    public class BaseAdminAccessLevel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}