using System;
using System.Collections.Generic;

namespace DMS_WebAPI.Models
{
    // Models returned by AccountController actions.

    public class DocumentsViewModel:BaseViewModels
    {
        public List<DocumentViewModel> Documents { get; set; }
    }

    public class DocumentViewModel:BaseViewModels
    {
        public int Id { get; set; }
    }
}
