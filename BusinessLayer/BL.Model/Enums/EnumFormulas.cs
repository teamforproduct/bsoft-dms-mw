using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Enums
{
    public enum EnumFormulas
    {
        /// <summary>
        /// ИД журнала
        /// </summary>
        RegistrationJournalId = 1,
        /// <summary>
        /// Индекс журнала регистрации
        /// </summary>
        RegistrationJournalIndex = 2,

        /// <summary>
        /// Индекс подразделения журнала регистрации
        /// </summary>
        RegistrationJournalDepartmentCode = 3,

        /// <summary>
        /// Номер полный инициативного документа (собственный)	берется головной документ пакета
        /// </summary>
        InitiativeRegistrationFullNumber = 4,
        /// <summary>
        /// Префикс инициативного документа (собственный)
        /// </summary>
        InitiativeRegistrationNumberPrefix = 5,
        /// <summary>
        /// Суффикс инициативного документа (собственный)
        /// </summary>
        InitiativeRegistrationNumberSuffix = 6,
        /// <summary>
        /// Номер порядковый инициативного документа (собственный)
        /// </summary>
        InitiativeRegistrationNumber = 7,
        /// <summary>
        /// Номер инициативного документа (корреспондента) SenderNumber
        /// </summary>
        InitiativeRegistrationSenderNumber = 8,

        /// <summary>
        /// Индекс подразделения - исполнитель	определить подразденление по исполнителю
        /// </summary>
        ExecutorPositionDepartmentCode = 9,
        /// <summary>
        /// Индекс подразделения - подписавший	определить подразделение подписанта
        /// </summary>
        SubscriptionsPositionDepartmentCode = 10,
        /// <summary>
        /// Индекс подразделения - регистратор	регистрирующее подразденение, реквизит в документе (подразделение регистратора-пользователя) Брать из курент позитион
        /// </summary>
        CurrentPositionDepartmentCode = 11,

        /// <summary>
        /// Цикличность годовая, месячная... (за счет формата: "YYYY" "MM")
        /// </summary>
        Date = 12,
        /// <summary>
        /// Первая буква н/ф корреспондента	из рассылки по документу берется последний контагент
        /// </summary>
        DocumentSendListLastAgentExternalFirstSymbolName = 13,
        /// <summary>
        /// Порядковый № обращения корреспондента	по связанным документам количество входящих от этого контрагента (создать документ в дополнении: пакеты)
        /// </summary>
        OrdinalNumberDocumentLinkForCorrespondent = 14
    }
}
