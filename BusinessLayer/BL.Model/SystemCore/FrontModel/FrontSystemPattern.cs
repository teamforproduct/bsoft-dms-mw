using BL.Model.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Model.SystemCore.FrontModel
{

    public class FrontSystemPattern : ListItem
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
