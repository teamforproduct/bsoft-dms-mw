using System.Runtime.Serialization;

namespace BL.Model.Users
{
    public class ChangeLogin
    {
        /// <summary>
        /// Новый логин
        /// </summary>
        public string NewEmail { get; set; }


        /// <summary>
        /// Новый логин
        /// </summary>
        [IgnoreDataMember]
        public string NewUserName { get; set; }
    }
}
