using BL.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BL.Model.DocumentCore.InternalModel
{
    public interface IDocumentEventAccessSet
    {
        IEnumerable<InternalDocumentEventAccess> Accesses{ get; set; }
        IEnumerable<InternalDocumentEventAccessGroup> AccessGroups { get; set; }
    }
}
