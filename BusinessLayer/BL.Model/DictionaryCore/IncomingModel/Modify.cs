using BL.Model.Extensions;
using System;
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
        //TODO [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// адреса контрагентов
    /// </summary>
    public class ModifyAgentAddress : AddAgentAddress
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
        //[Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Компании"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyAgentClientCompany : AddAgentClientCompany
    {
        /// <summary>
        /// ID
        /// </summary>
        //TODO [Required]
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
        //TODO [Required]
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
    /// Контрагент - физическое лицо
    /// </summary>
    public class ModifyAgentPerson : AddAgentPerson
    {
        /// <summary>
        /// ID
        /// </summary>
        // TODO
        //[Required]
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
        //TODO [Required]
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
        //TODO [Required]
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
        //TODO [Required]
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
        //TODO [Required]
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
        /// //TODO [Required]
        public int Id { get; set; }

    }


    public class ModifyStandartSendList : AddStandartSendList
    {
        //TODO [Required]
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
        //TODO [Required]
        public int Id { get; set; }
    }

    public class ModifyTag : AddTag
    {
        //TODO [Required]
        public int Id { get; set; }

    }

    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Компании"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyDictionaryAgentUser: AddDictionaryAgentUser
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }
    }
}
