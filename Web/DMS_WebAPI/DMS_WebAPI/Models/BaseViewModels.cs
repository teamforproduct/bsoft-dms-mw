using System;
using System.Collections.Generic;

namespace DMS_WebAPI.Models
{
    // Models returned by AccountController actions.

    public class BaseViewModels
    {
        public bool Success { get; set; } = true;

        public string Msg { get; set; } = string.Empty;
    }
}
