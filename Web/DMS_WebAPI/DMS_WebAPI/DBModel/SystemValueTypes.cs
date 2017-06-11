using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{

    public class SystemValueTypes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }

    }
}
