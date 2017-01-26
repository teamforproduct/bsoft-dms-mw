using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Model.SystemCore.FrontModel
{
    public partial class FrontSystemModules
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }
    }
}
