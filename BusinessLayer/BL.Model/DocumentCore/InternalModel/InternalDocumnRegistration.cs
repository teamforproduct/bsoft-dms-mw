using System;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumnRegistration
    {
        /// <summary>
        /// ИД журнала
        /// </summary>
        public int? RegistrationJournalId { get; set; }
        /// <summary>
        /// Индекс журнала регистрации
        /// </summary>
        public string RegistrationJournalIndex { get; set; }
        /// <summary>
        /// Индекс подразделения журнала регистрации
        /// </summary>
        public string RegistrationJournalDepartmentCode { get; set; }

        /// <summary>
        /// Номер полный инициативного документа (собственный)	берется головной документ пакета
        /// </summary>
        public string InitiativeRegistrationFullNumber { get; set; }

        /// <summary>
        /// Префикс инициативного документа (собственный)
        /// </summary>
        public string InitiativeRegistrationNumberPrefix { get; set; }

        /// <summary>
        /// Суффикс инициативного документа (собственный)
        /// </summary>
        public string InitiativeRegistrationNumberSuffix { get; set; }
        /// <summary>
        /// Номер порядковый инициативного документа (собственный)
        /// </summary>
        public int? InitiativeRegistrationNumber { get; set; }
        /// <summary>
        /// Номер инициативного документа (корреспондента) SenderNumber
        /// </summary>
        public string InitiativeRegistrationSenderNumber { get; set; }

        /// <summary>
        /// Индекс подразделения - исполнитель	определить подразденление по исполнителю
        /// </summary>
        public string ExecutorPositionDepartmentCode { get; set; }

        /// <summary>
        /// Цикличность годовая, месячная... (за счет формата: "YYYY" "MM")
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Индекс подразделения - подписавший	определить подразделение подписанта
        /// </summary>
        public string SubscriptionsPositionDepartmentCode { get; set; }

        /// <summary>
        /// Индекс подразделения - регистратор	регистрирующее подразденение, реквизит в документе (подразделение регистратора-пользователя) Брать из курент позитион
        /// </summary>
        public string CurrentPositionDepartmentCode { get; set; }

        /// <summary>
        /// Первая буква н/ф корреспондента	из рассылки по документу берется последний контагент
        /// </summary>
        public string DocumentSendListLastAgentExternalFirstSymbolName { get; set; }

        /// <summary>
        /// Порядковый № обращения корреспондента	по связанным документам количество входящих от этого контрагента (создать документ в дополнении: пакеты)
        /// </summary>
        public int OrdinalNumberDocumentLinkForCorrespondent { get; set; }

    }
}
