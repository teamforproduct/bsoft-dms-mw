using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования
    /// </summary>
    public class ModifyAddressType : AddAddressType
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// адреса контрагентов
    /// </summary>
    public class ModifyAgentAddress : AddAgentAddress
    {
        public ModifyAgentAddress() { }

        public ModifyAgentAddress(ModifyUserAddress model)
        {
            Id = model.Id;
            AddressTypeId = model.AddressTypeId;
            PostCode = model.PostCode;
            Address = model.Address;
            IsActive = model.IsActive;
            Description = model.Description;
        }

        /// <summary>
        /// ИД 
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// адреса контрагентов
    /// </summary>
    public class ModifyUserAddress : BaseAgentAddress
    {
        /// <summary>
        /// ИД 
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// адреса контрагентов
    /// </summary>
    public class ModifyAgentContact : AddAgentContact
    {
        public ModifyAgentContact() { }

        public ModifyAgentContact(ModifyUserContact model)
        {
            Id = model.Id;
            ContactTypeId = model.ContactTypeId;
            Value = model.Value;
            IsActive = model.IsActive;
            IsConfirmed = model.IsConfirmed;
            Description = model.Description;
        }

        /// <summary>
        /// ИД 
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// адреса контрагентов
    /// </summary>
    public class ModifyUserContact : BaseAgentContact
    {
        /// <summary>
        /// ИД 
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    public class ModifyAgentBank : AddAgentBank
    {
        /// <summary>
        /// Ид
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Компании"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyOrg : AddOrg
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// Контрагент - юридическое лицо
    /// </summary>
    public class ModifyAgentCompany : AddAgentCompany
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    public class ModifyAgentContactPerson : AddAgentContactPerson
    {
        /// <summary>
        /// Ид
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class ModifyAgentEmployee : AddAgentEmployee
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// Для корректировки параметров из профиля
    /// </summary>
    public class ModifyAgentUser : AddAgentPeople
    {
        /// <summary>
        /// ИД аватарки, если она была загружена
        /// </summary>
        public int? ImageId { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        [IgnoreDataMember]
        public string PostedFileData { get; set; }

        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        [Required]
        public int LanguageId { get; set; }
    }

    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class ModifyAgentPerson : AddAgentPerson
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    public class ModifyAgentPeoplePassport : AddAgentPeoplePassport
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }
    }



    /// <summary>
    /// Типы контактов
    /// </summary>
    public class ModifyContactType : AddContactType
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// Модель для добавления/редактирования отдела в штатном расписании
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyDepartment : AddDepartment
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// Модель для добавления/редактирования
    /// </summary>
    public class ModifyDocumentType : AddDocumentType
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }
    }

    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Штатное расписание"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyPosition : AddPosition
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }


    }

    /// <summary>
    /// При назначении сотрудника на должность нужно указать следующие парметры:
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyPositionExecutor : AddPositionExecutor
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }


    }

    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Журнал регистрации"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyRegistrationJournal : AddRegistrationJournal
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

    }


    public class ModifyStandartSendList : AddStandartSendList
    {
        [Required]
        public int Id { get; set; }
    }

    /// <summary>
    /// Содержание типового списка рассылки
    /// </summary>
    public class ModifyStandartSendListContent : AddStandartSendListContent
    {
        /// <summary>
        /// ИД
        /// </summary>
        [Required]
        public int Id { get; set; }
    }

    public class ModifyTag : AddTag
    {
        [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Компании"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyDictionaryAgentUser : AddDictionaryAgentUser
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }
    }

    public class ModifyCustomDictionaryType : AddCustomDictionaryType
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

    }

    public class ModifyCustomDictionary : AddCustomDictionary
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }
    }
}
