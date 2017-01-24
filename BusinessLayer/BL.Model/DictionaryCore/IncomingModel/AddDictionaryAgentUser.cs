using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Компании"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class AddDictionaryAgentUser
    {
         
        /// <summary>
        /// Айдишник веб-юзера
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Осносной адрес пользователя, на который высылается письмо с приглашением
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

    }
}
