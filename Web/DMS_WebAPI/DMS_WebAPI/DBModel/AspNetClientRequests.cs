using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClientRequests
    {
        public int Id { get; set; }

        [MaxLength(200)]
        [Index("IX_Code", IsUnique = true)]
        public string ClientCode { get; set; }

        [MaxLength(2000)]
        public string ClientName { get; set; }



        [MaxLength(10)]
        public string Language { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        [MaxLength(256)]
        public string FirstName { get; set; }

        [MaxLength(256)]
        public string LastName { get; set; }

        [MaxLength(256)]
        public string MiddleName { get; set; }


        [MaxLength(2000)]
        public string PhoneNumber { get; set; }

    }
}
