using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.System;
using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DatabaseContext
{
    public static class DmsDbImportData
    {

        private static int IdSequence = 0;

        #region [+] Languages ...

        public static List<AdminLanguages> GetAdminLanguages()
        {
            var items = new List<AdminLanguages>();

            items.Add(new AdminLanguages { Id = 1, Code = "ru", Name = "Русский", IsDefault = true });
            items.Add(new AdminLanguages { Id = 2, Code = "en", Name = "English", IsDefault = false });

            return items;
        }

        private static void AddALV(List<AdminLanguageValues> list, string label, string EngLang, string RusLang)
        {
            list.Add(new AdminLanguageValues()
            {
                Id = ++IdSequence,
                LanguageId = 1,
                Label = label,
                Value = RusLang
            });
            list.Add(new AdminLanguageValues()
            {
                Id = ++IdSequence,
                LanguageId = 2,
                Label = label,
                Value = EngLang
            });
        }

        public static List<AdminLanguageValues> GetAdminLanguageValues()
        {
            IdSequence = 0;

            var list = new List<AdminLanguageValues>();

            AddALV(list, "##l@DictionaryDocumentDirections:Incoming@l##", "Incoming", "Входящий");
            AddALV(list, "##l@DictionaryDocumentDirections:Internal@l##", "Internal", "Собственный");
            AddALV(list, "##l@DictionaryDocumentDirections:Outcoming@l##", "Outcoming", "Иcходящий");
            AddALV(list, "##l@DictionaryImportanceEventTypes:AdditionalEvents@l##", "Secondary events", "Второстепенные события");
            AddALV(list, "##l@DictionaryImportanceEventTypes:DocumentMoovement@l##", "Facts movement documents", "Факты движения документов");
            AddALV(list, "##l@DictionaryImportanceEventTypes:ImportantEvents@l##", "Important events", "Важные события");
            AddALV(list, "##l@DictionaryImportanceEventTypes:Internal@l##", "Own notes", "Собственные примечания");
            AddALV(list, "##l@DictionaryImportanceEventTypes:Message@l##", "Messages", "Сообщения");
            AddALV(list, "##l@DictionaryImportanceEventTypes:PaperMoovement@l##", "Paper movement", "Движение БН");
            AddALV(list, "##l@DictionarySendTypes:SendForConsideration@l##", "For consideration", "Для рассмотрения");
            AddALV(list, "##l@DictionarySendTypes:SendForControl@l##", "In control", "На контроль(отв.исп.)");
            AddALV(list, "##l@DictionarySendTypes:SendForExecution@l##", "For execution", "Соисполнителю");
            AddALV(list, "##l@DictionarySendTypes:SendForInformation@l##", "For information", "Для сведения");
            AddALV(list, "##l@DictionarySendTypes:SendForInformationExternal@l##", "For information external agents", "Для сведения внешнему агенту");
            AddALV(list, "##l@DictionarySendTypes:SendForResponsibleExecution@l##", "ResponsibleExecution", "Исполненителю(отв.исп.)");
            AddALV(list, "##l@DictionarySendTypes:SendForSigning@l##", "For signing", "На подпись");
            AddALV(list, "##l@DictionarySendTypes:SendForVisaing@l##", "For visaing", "На визирование");
            AddALV(list, "##l@DictionarySendTypes:SendForАgreement@l##", "For agreement", "На согласование");
            AddALV(list, "##l@DictionarySendTypes:SendForАpproval@l##", "For approval", "На утверждение");
            AddALV(list, "##l@DictionarySubordinationTypes:Execution@l##", "Execution", "Исполнение");
            AddALV(list, "##l@DictionarySubordinationTypes:Informing@l##", "Informing", "Информирование");
            AddALV(list, "##l@DictionarySubscriptionStates:No@l##", "No", "Нет");
            AddALV(list, "##l@DictionarySubscriptionStates:Sign@l##", "Sign", "Подпись");
            AddALV(list, "##l@DictionarySubscriptionStates:Violated@l##", "Violated", "Нарушена");
            AddALV(list, "##l@DictionarySubscriptionStates:Visa@l##", "Visa", "Виза");
            AddALV(list, "##l@DictionarySubscriptionStates:Аgreement@l##", "Аgreement", "Согласование");
            AddALV(list, "##l@DictionarySubscriptionStates:Аpproval@l##", "Аpproval", "Утверждение");

            AddALV(list, "##l@DmsExceptions:AccessIsDenied@l##", "Access is Denied!", "Отказано в доступе!");
            AddALV(list, "##l@DmsExceptions:CannotAccessToFile@l##", "Cannot access to user file!", "Файл пользователя не доступен!");
            AddALV(list, "##l@DmsExceptions:CannotSaveFile@l##", "Error when save user file!", "Ошибка при сохранения файла пользователя!");
            AddALV(list, "##l@DmsExceptions:ClientIsNotFound@l##", "Client not found", "Клиент не найден");
            AddALV(list, "##l@DmsExceptions:ClientNameAlreadyExists@l##", "Client Name already exists", "Имя клиента уже существует");
            AddALV(list, "##l@DmsExceptions:ClientVerificationCodeIncorrect@l##", "Verification code is invalid", "Проверочный код неверен");
            AddALV(list, "##l@DmsExceptions:CommandNotDefinedError@l##", "The desired commands not found", "Команда не найдена");
            AddALV(list, "##l@DmsExceptions:CouldNotChangeAttributeLaunchPlan@l##", "Couldn\"t change attribute LaunchPlan", "Невозможно изменить атрибут LaunchPlan");
            AddALV(list, "##l@DmsExceptions:CouldNotChangeFavourite@l##", "Couldn\"t change attribute Favourite", "Невозможно изменить атрибут Favourite");
            AddALV(list, "##l@DmsExceptions:CouldNotChangeIsInWork@l##", "Couldn\"t change attribute IsInWork", "Невозможно изменить атрибут IsInWork");
            AddALV(list, "##l@DmsExceptions:CouldNotPerformThisOperation@l##", "Could Not Perform This Operation!", "Операция не выполнена!");
            AddALV(list, "##l@DmsExceptions:CryptographicError@l##", "Encryption Error", "Ошибка шифрования");
            AddALV(list, "##l@DmsExceptions:DatabaseError@l##", "An error occurred while accessing the database!", "Ошибка при обращении к базе данных!");
            AddALV(list, "##l@DmsExceptions:DatabaseIsNotFound@l##", "Database not found", "База данных не найдена");
            AddALV(list, "##l@DmsExceptions:DatabaseIsNotSet@l##", "The database is not set", "База данных не установлена");

            AddALV(list, "##l@DmsExceptions:AdminRecordNotUnique@l##", "Setting record should be unique!", "Настроечная запись должена быть уникальна!");
            AddALV(list, "##l@DmsExceptions:AdminRecordCouldNotBeAdded@l##", "You could not add this setting data!", "Вы не можете добавить настроечные данные");
            AddALV(list, "##l@DmsExceptions:AdminRecordCouldNotBeDeleted@l##", "You could not delete from this dictionary data!", "Вы не можете удалить настроечные данные");
            AddALV(list, "##l@DmsExceptions:AdminRecordWasNotFound@l##", "Dictionary record was not found!", "Элемент справочника не найден!");

            AddALV(list, "##l@DmsExceptions:DictionaryAddressTypeCodeNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryAddressTypeNameNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryContactTypeCodeNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryContactTypeNameNotUnique@l##", "", "");

            AddALV(list, "##l@DmsExceptions:DictionaryAgentBankMFOCodeNotUnique@l##", "Bank \"{0}\" MFO \"{1}\" сode should be unique!", "Банк \"{0}\" c МФО \"{1}\" уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentCompanyOKPOCodeNotUnique@l##", "Company \"{0}\" OKPO сode should be unique!", "Юридическое лицо с указанным ОКПО уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentCompanyTaxCodeNotUnique@l##", "Company \"{0}\" tax сode should be unique!", "Юридическое лицо с указанным ИНН уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentCompanyVATCodeNotUnique@l##", "Company \"{0}\" VAT сode should be unique!", "Юридическое лицо с указанным номером свидетельства НДС уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentEmployeePassportNotUnique@l##", "Employee \"{0}\" passport should be unique!", "Сотрудник с указанными паспортными данными уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentEmployeePersonnelNumberNotUnique@l##", "Employee \"{0}\" personnel number should be unique!", "Сотрудник с указанным табельным номером уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentEmployeeTaxCodeNotUnique@l##", "Employee \"{0}\" tax code should be unique!", "Сотрудник с указанным ИНН уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentNameNotUnique@l##", "Agent name \"{0}\" should be unique!", "Агент \"{0}\" уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentPersonPassportNotUnique@l##", "Person \"{0}\" passport should be unique!", "Физлицо с указанными паспортными данными уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentPersonTaxCodeNotUnique@l##", "Person \"{0}\" tax code should be unique!", "Физлицо с указанным ИНН уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentContactTypeNotUnique@l##", "Agent contact type should be unique!", "Контакт с указанным типом уже есть у этого агента");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentContactNotUnique@l##", "Agent contact should be unique!", "Указанный контакт уже есть у этого агента");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentAddressTypeNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentAccountNumberNotUnique@l##", "", "");
            
            AddALV(list, "##l@DmsExceptions:DictionaryCostomDictionaryNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryCostomDictionaryTypeNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionarysdDepartmentNotBeSubordinated@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryDepartmentNameNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryDocumentSubjectNameNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryDocumentTypeNameNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryRegistrationJournalNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryStandartSendListNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryStandartSendListContentNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryTagNotUnique@l##", "", "");



            AddALV(list, "##l@DmsExceptions:DictionaryRecordCouldNotBeAdded@l##", "You could not add this dictionary data!", "Вы не можете добавить данные в этот справочник");
            AddALV(list, "##l@DmsExceptions:DictionaryRecordCouldNotBeDeleted@l##", "You could not delete from this dictionary data!", "Вы не можете удалить данные из этого справочника");
            AddALV(list, "##l@DmsExceptions:DictionaryRecordNotUnique@l##", "Dictionary record should be unique!", "Элемент справочника должен быть уникален!");
            AddALV(list, "##l@DmsExceptions:DictionaryRecordWasNotFound@l##", "Dictionary record was not found!", "Элемент справочника не найден!");
            AddALV(list, "##l@DmsExceptions:DictionarySystemRecordCouldNotBeDeleted@l##", "Dictionary system record was not deleted!", "Невозможно удалить предустановленные записи справочника");
            AddALV(list, "##l@DmsExceptions:DictionaryTagNotFoundOrUserHasNoAccess@l##", "User could not access this tag!", "Пользователь не имеет доступа к этому тегу!");
            AddALV(list, "##l@DmsExceptions:DocumentCannotBeModifiedOrDeleted@l##", "Document cannot be Modified or Deleted!", "Документ не может быть изменен или удален!");
            AddALV(list, "##l@DmsExceptions:DocumentCouldNotBeRegistered@l##", "Document registration has non been successfull! Try again!", "Регистрационный документ не была успешной! Попробуй еще раз!");
            AddALV(list, "##l@DmsExceptions:DocumentFileWasChangedExternally@l##", "The document file has been modified from the outside", "Файл документа был изменен извне");
            AddALV(list, "##l@DmsExceptions:DocumentHasAlreadyHasLink@l##", "Document has already has link!", "Документ уже имеет ссылку!");
            AddALV(list, "##l@DmsExceptions:DocumentHasAlredyBeenRegistered@l##", "Document has already been registered!", "Документ уже зарегистрирован!");
            AddALV(list, "##l@DmsExceptions:DocumentNotFoundOrUserHasNoAccess@l##", "User could not access this document!", "Документ не доступен!");
            AddALV(list, "##l@DmsExceptions:DocumentRestrictedSendListDoesNotMatchTheTemplate@l##", "Document Restricted SendList does not match the template", "Разрешающий список рассылок для документа не соответствует шаблону");
            AddALV(list, "##l@DmsExceptions:DocumentRestrictedSendListDuplication@l##", "Duplicate Entry DocumentRestrictSendList", "Дублирование записей в разрешающем списке рассылке для документа");
            AddALV(list, "##l@DmsExceptions:DocumentSendListDoesNotMatchTheTemplate@l##", "Document SendList does not match the template", "Список рассылок для документа не соответствует шаблону");
            AddALV(list, "##l@DmsExceptions:DocumentSendListNotFoundInDocumentRestrictedSendList@l##", "DocumentSendList not found in DocumentRestrictedSendList", "Получатель не найден в разрешающем списке рассылок для документа");
            AddALV(list, "##l@DmsExceptions:EncryptionCertificatePrivateKeyСanNotBeExported@l##", "The private key can not be exported", "Приватный ключ нельзя экспортировать");
            AddALV(list, "##l@DmsExceptions:EncryptionCertificateWasNotFound@l##", "The certificate was not found", "Сертификат не был найден");
            AddALV(list, "##l@DmsExceptions:EventNotFoundOrUserHasNoAccess@l##", "User could not access this event!", "Пользователь не имеет доступа к этому событию!");
            AddALV(list, "##l@DmsExceptions:ExecutorAgentForPositionIsNotDefined@l##", "Executor agent for position is not defined!", "Исполнитель для должности не определен!");
            AddALV(list, "##l@DmsExceptions:IncomingModelIsNotvalid@l##", "Incoming Model is not valid!", "Входящая модель недействительна! Проверь REQUIRED поля");
            AddALV(list, "##l@DmsExceptions:LicenceExceededNumberOfConnectedUsers@l##", "You have exceeded the allowed number of connected users", "Превышено разрешенное количество подключенных пользователей");
            AddALV(list, "##l@DmsExceptions:LicenceExceededNumberOfRegisteredUsers@l##", "You have exceeded the allowed number of registered users", "Превышено разрешенное количество зарегистрированных пользователей");
            AddALV(list, "##l@DmsExceptions:LicenceExpired@l##", "Licence expired", "Срок лицензии истек");
            AddALV(list, "##l@DmsExceptions:LicenceInformationError@l##", "The licence is not valid", "Лицензия недействительна");
            AddALV(list, "##l@DmsExceptions:NeedInformationAboutCorrespondent@l##", "Need information about correspondent!", "Нужна информация о корреспонденте!");
            AddALV(list, "##l@DmsExceptions:NotFilledWithAdditionalRequiredAttributes@l##", "Not filled with additional required attributes!", "Не заполнены обязательные дополнительные атрибуты!");
            AddALV(list, "##l@DmsExceptions:PaperListNotFoundOrUserHasNoAccess@l##", "Paper list not found or user has no access", "Список бумага не найдена или пользователь не имеет доступа");
            AddALV(list, "##l@DmsExceptions:PaperNotFoundOrUserHasNoAccess@l##", "Paper not found or user has no access", "Бумага не найдена или пользователь не имеет доступа");
            AddALV(list, "##l@DmsExceptions:PlanPointHasAlredyBeenLaunched@l##", "Plan Point has already been Launched!", "Пункт плана уже запущен!");
            AddALV(list, "##l@DmsExceptions:RecordNotUnique@l##", "Record is not Unique", "Запись не уникальна");
            AddALV(list, "##l@DmsExceptions:TaskNotFoundOrUserHasNoAccess@l##", "Task not found", "Task не найден");
            AddALV(list, "##l@DmsExceptions:TemplateDocumentIsNotvalid@l##", "The document template is not valid", "Шаблон документа не корректен");
            AddALV(list, "##l@DmsExceptions:TemplateDocumentNotFoundOrUserHasNoAccess@l##", "User could not access this template document!", "Пользователь не имеет доступ к этот шаблону документа!");
            AddALV(list, "##l@DmsExceptions:UnknownDocumentFile@l##", "Could not find appropriate document file!", "Не удалось найти соответствующий файл документа!");
            AddALV(list, "##l@DmsExceptions:UserFileNotExists@l##", "User file does not exists on Filestore!", "Пользовательский файл не существует в файловом хранилище");
            AddALV(list, "##l@DmsExceptions:UserHasNoAccessToDocument@l##", "User could not access this document!", "Пользователь не может получить доступ к этот документ!");
            AddALV(list, "##l@DmsExceptions:UserNameAlreadyExists@l##", "User Name already exists", "Имя пользователя уже существует");
            AddALV(list, "##l@DmsExceptions:UserNameIsNotDefined@l##", "Employee for the current user could not be defined!", "Сотрудник для текущего пользователя не может быть определен!");
            AddALV(list, "##l@DmsExceptions:UserPositionIsNotDefined@l##", "Position for the current user could not be defined!", "Позиция для текущего пользователя не может быть определена!");
            AddALV(list, "##l@DmsExceptions:UserUnauthorized@l##", "Authorization has been denied for this request.", "Пользователь не авторизован");
            AddALV(list, "##l@DmsExceptions:WaitHasAlreadyClosed@l##", "Wait has already closed!", "Ожидание уже закрыто!");
            AddALV(list, "##l@DmsExceptions:WaitNotFoundOrUserHasNoAccess@l##", "User could not access this wait!", "Пользователь не имеет доступа к этим ожиданиям!");
            AddALV(list, "##l@DmsExceptions:WrongDocumentSendListEntry@l##", "Plan item is wrong.", "Некорректный пункт плана");
            AddALV(list, "##l@DmsExceptions:WrongParameterTypeError@l##", "Parameter type commands is incorrect!", "Тип параметра комманды указан неверно!");
            AddALV(list, "##l@DmsExceptions:WrongParametervalueError@l##", "Parameters commands incorrect!", "Параметры комманды неверные!");


            //pss 23.09.2016 Выявил DmsExceptions которые не имели перевода 
            //TODO Требуется локализация (перевод ошибок)
            AddALV(list, "##l@DmsExceptions:ControlerHasAlreadyBeenDefined@l##", "", "");
            AddALV(list, "##l@DmsExceptions:CouldNotModifyTemplateDocument@l##", "", "");
            AddALV(list, "##l@DmsExceptions:CouldNotPerformOperationWithPaper@l##", "", "");
            AddALV(list, "##l@DmsExceptions:EncryptionCertificateHasExpired@l##", "", "");
            AddALV(list, "##l@DmsExceptions:NobodyIsChosen@l##", "", "");
            AddALV(list, "##l@DmsExceptions:ResponsibleExecutorHasAlreadyBeenDefined@l##", "", "");
            AddALV(list, "##l@DmsExceptions:ResponsibleExecutorIsNotDefined@l##", "", "");
            AddALV(list, "##l@DmsExceptions:SigningTypeNotAllowed@l##", "", "");
            AddALV(list, "##l@DmsExceptions:SubordinationHasBeenViolated@l##", "", "");
            AddALV(list, "##l@DmsExceptions:TargetIsNotDefined@l##", "", "");
            AddALV(list, "##l@DmsExceptions:TaskIsNotDefined@l##", "", "");

            // после добавления переводов можно обновить их в базе api/v2/Languages/RefreshLanguageValues

            return list;
        }

        #endregion

        #region [+] SystemObjects ...

        public static List<SystemObjects> GetSystemObjects()
        {
            var items = new List<SystemObjects>();

            items.Add(GetSystemObjects(EnumObjects.Documents, "Документы"));
            items.Add(GetSystemObjects(EnumObjects.DocumentAccesses, "Документы - доступы"));
            items.Add(GetSystemObjects(EnumObjects.DocumentRestrictedSendLists, "Документы - ограничения рассылки"));
            items.Add(GetSystemObjects(EnumObjects.DocumentSendLists, "Документы - план работы"));
            items.Add(GetSystemObjects(EnumObjects.DocumentFiles, "Документы - файлы"));
            items.Add(GetSystemObjects(EnumObjects.DocumentLinks, "Документы - связи"));
            items.Add(GetSystemObjects(EnumObjects.DocumentSendListStages, "Документы - этапы плана работ"));
            items.Add(GetSystemObjects(EnumObjects.DocumentEvents, "Документы - события"));
            items.Add(GetSystemObjects(EnumObjects.DocumentWaits, "Документы - ожидания"));
            items.Add(GetSystemObjects(EnumObjects.DocumentSubscriptions, "Документы - подписи"));
            items.Add(GetSystemObjects(EnumObjects.DocumentTasks, "Документы - задачи"));
            items.Add(GetSystemObjects(EnumObjects.DocumentPapers, "Документы - бумажные носители"));
            items.Add(GetSystemObjects(EnumObjects.DocumentPaperEvents, "Документы - события по бумажным носителям"));
            items.Add(GetSystemObjects(EnumObjects.DocumentPaperLists, "Документы - реестры передачи бумажных носителей"));
            items.Add(GetSystemObjects(EnumObjects.DocumentSavedFilters, "Документы - сохраненные фильтры"));
            items.Add(GetSystemObjects(EnumObjects.DocumentTags, "Документы - тэги"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDocumentType, "Типы документов"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAddressType, "Типы адресов"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDocumentSubjects, "Тематики документов"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryRegistrationJournals, "Журналы регистрации"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryContactType, "Типы контактов"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgents, "Контрагенты"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryContacts, "Контакты"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentAddresses, "Адреса"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentPersons, "Физические лица"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDepartments, "Структура предприятия"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositions, "Штатное расписание"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentEmployees, "Сотрудники"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentCompanies, "Юридические лица"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentBanks, "Контрагенты - банки"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentAccounts, "Расчетные счета"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryStandartSendListContent, "Типовые списки рассылки (содержание)"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryStandartSendLists, "Типовые списки рассылки"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentClientCompanies, "Компании"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositionExecutorTypes, "Типы исполнителей"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositionExecutors, "Исполнители должности"));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocument, "Шаблоны документов"));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentSendList, "Списки рассылки в шаблонах"));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentRestrictedSendList, "Ограничительные списки рассылки в шаблонах"));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentTask, "Задачи в шаблонах"));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentAttachedFiles, "Прикрепленные к шаблонам файлы"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryTag, "Теги"));
            items.Add(GetSystemObjects(EnumObjects.CustomDictionaryTypes, "Типы пользовательских словарей"));
            items.Add(GetSystemObjects(EnumObjects.CustomDictionaries, "Пользовательские словари"));
            items.Add(GetSystemObjects(EnumObjects.Properties, "Динамические аттрибуты"));
            items.Add(GetSystemObjects(EnumObjects.PropertyLinks, "Связи динамических аттрибутов с объектами системы"));
            items.Add(GetSystemObjects(EnumObjects.PropertyValues, "Значения динамических аттрибутов"));

            items.Add(GetSystemObjects(EnumObjects.EncryptionCertificates, "Хранилище сертификатов"));
            items.Add(GetSystemObjects(EnumObjects.EncryptionCertificateTypes, "Типы сертификатов"));

            items.Add(GetSystemObjects(EnumObjects.AdminRoles, "Роли"));
            items.Add(GetSystemObjects(EnumObjects.AdminPositionRoles, "Роли"));
            items.Add(GetSystemObjects(EnumObjects.AdminUserRoles, "Роли"));

            return items;
        }

        private static SystemObjects GetSystemObjects(EnumObjects id, string description)
        {
            return new SystemObjects()
            {
                Id = (int)id,
                Code = id.ToString(),
                Description = description
            };
        }
        #endregion

        #region [+] SystemActions ...
        public static List<SystemActions> GetSystemActions()
        {
            var items = new List<SystemActions>();

            #region OLD
            //items.Add(new SystemActions { Id = 100001, ObjectId = 100, Code = "AddDocument", API = "", Description = "Создать документ по шаблону", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            //items.Add(new SystemActions { Id = 100002, ObjectId = 100, Code = "CopyDocument", API = "", Description = "Создать документ копированием", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            //items.Add(new SystemActions { Id = 100003, ObjectId = 100, Code = "ModifyDocument", API = "", Description = "Изменить документ", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            //items.Add(new SystemActions { Id = 100004, ObjectId = 100, Code = "DeleteDocument", API = "", Description = "Удалить проект", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            //items.Add(new SystemActions { Id = 100005, ObjectId = 100, Code = "LaunchPlan", API = "", Description = "Запустить выполнение плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            //items.Add(new SystemActions { Id = 100006, ObjectId = 100, Code = "AddDocumentSendListItem", API = "", Description = "Добавить пункт плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            //items.Add(new SystemActions { Id = 100007, ObjectId = 100, Code = "StopPlan", API = "", Description = "Остановить выполнение плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            //items.Add(new SystemActions { Id = 100008, ObjectId = 100, Code = "ChangeExecutor", API = "", Description = "Передать управление", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            //items.Add(new SystemActions { Id = 100009, ObjectId = 100, Code = "RegisterDocument", API = "", Description = "Зарегистрировать проект", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            //items.Add(new SystemActions { Id = 100010, ObjectId = 100, Code = "MarkDocumentEventAsRead", API = "", Description = "Отметить прочтение событий по документу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            //items.Add(new SystemActions { Id = 100011, ObjectId = 100, Code = "SendForInformation", API = "", Description = "Направить для сведения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            //items.Add(new SystemActions { Id = 100012, ObjectId = 100, Code = "SendForConsideration", API = "", Description = "Направить для рассмотрения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            //items.Add(new SystemActions { Id = 100015, ObjectId = 100, Code = "SendForInformationExternal", API = "", Description = "Направить для сведения внешнему агенту", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            //items.Add(new SystemActions { Id = 100021, ObjectId = 112, Code = "ControlOn", API = "", Description = "Взять на контроль", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100023, ObjectId = 112, Code = "ControlChange", API = "", Description = "Изменить параметры контроля", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100024, ObjectId = 112, Code = "SendForExecutionChange", API = "", Description = "Изменить параметры направлен для исполнения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100026, ObjectId = 112, Code = "SendForResponsibleExecutionChange", API = "", Description = "Изменить параметры направлен для отв.исполнения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100027, ObjectId = 112, Code = "ControlTargetChange", API = "", Description = "Изменить параметры контроля для исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100029, ObjectId = 112, Code = "ControlOff", API = "", Description = "Снять с контроля", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100030, ObjectId = 100, Code = "SendForControl", API = "", Description = "Направить для контроля", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100031, ObjectId = 100, Code = "SendForResponsibleExecution", API = "", Description = "Направить для отв.исполнения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100032, ObjectId = 100, Code = "SendForExecution", API = "", Description = "Направить для исполнения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100035, ObjectId = 112, Code = "MarkExecution", API = "", Description = "Отметить исполнение", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100037, ObjectId = 112, Code = "AcceptResult", API = "", Description = "Принять результат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100038, ObjectId = 112, Code = "RejectResult", API = "", Description = "Отклонить результат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            //items.Add(new SystemActions { Id = 100041, ObjectId = 100, Code = "SendForVisaing", API = "", Description = "Направить для визирования", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100042, ObjectId = 100, Code = "SendForАgreement", API = "", Description = "Направить для согласование", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100043, ObjectId = 100, Code = "SendForАpproval", API = "", Description = "Направить для утверждения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100044, ObjectId = 100, Code = "SendForSigning", API = "", Description = "Направить для подписи", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100046, ObjectId = 113, Code = "WithdrawVisaing", API = "", Description = "Отозвать с визирования", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100047, ObjectId = 113, Code = "WithdrawАgreement", API = "", Description = "Отозвать с согласования", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100048, ObjectId = 113, Code = "WithdrawАpproval", API = "", Description = "Отозвать с утверждения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100049, ObjectId = 113, Code = "WithdrawSigning", API = "", Description = "Отозвать с подписи", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100051, ObjectId = 113, Code = "AffixVisaing", API = "", Description = "Завизировать", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100052, ObjectId = 113, Code = "AffixАgreement", API = "", Description = "Согласовать", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100053, ObjectId = 113, Code = "AffixАpproval", API = "", Description = "Утвердить", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100054, ObjectId = 113, Code = "AffixSigning", API = "", Description = "Подписать", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100055, ObjectId = 113, Code = "SelfAffixSigning", API = "", Description = "Самоподписание", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100056, ObjectId = 113, Code = "RejectVisaing", API = "", Description = "Отказать в визирования", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100057, ObjectId = 113, Code = "RejectАgreement", API = "", Description = "Отказать в согласование", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100058, ObjectId = 113, Code = "RejectАpproval", API = "", Description = "Отказать в утверждения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100059, ObjectId = 113, Code = "RejectSigning", API = "", Description = "Отказать в подписи", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            //items.Add(new SystemActions { Id = 100081, ObjectId = 111, Code = "SendMessage", API = "", Description = "Направить сообщение участникам рабочей группы", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            //items.Add(new SystemActions { Id = 100083, ObjectId = 111, Code = "AddNote", API = "", Description = "Добавить примечание", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            //items.Add(new SystemActions { Id = 100085, ObjectId = 100, Code = "ReportRegistrationCardDocument", API = "", Description = "Регистрационная карточка", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Отчеты" });
            //items.Add(new SystemActions { Id = 100091, ObjectId = 100, Code = "AddFavourite", API = "", Description = "Добавить в избранное", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Дополнительно" });
            //items.Add(new SystemActions { Id = 100093, ObjectId = 100, Code = "DeleteFavourite", API = "", Description = "Удалить из избранного", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Дополнительно" });
            //items.Add(new SystemActions { Id = 100095, ObjectId = 100, Code = "FinishWork", API = "", Description = "Закончить работу с документом", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Дополнительно" });
            //items.Add(new SystemActions { Id = 100097, ObjectId = 100, Code = "StartWork", API = "", Description = "Возобновить работу с документом", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Дополнительно" });
            //items.Add(new SystemActions { Id = 100099, ObjectId = 100, Code = "ChangePosition", API = "", Description = "Поменять должность в документе", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Администратор" });
            //items.Add(new SystemActions { Id = 102001, ObjectId = 102, Code = "AddDocumentRestrictedSendList", API = "", Description = "Добавить ограничение рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 102002, ObjectId = 102, Code = "AddByStandartSendListDocumentRestrictedSendList", API = "", Description = "Добавить ограничения рассылки по стандартному списку", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 102009, ObjectId = 102, Code = "DeleteDocumentRestrictedSendList", API = "", Description = "Удалить ограничение рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 103001, ObjectId = 106, Code = "AddDocumentSendList", API = "", Description = "Добавить пункт плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 103002, ObjectId = 106, Code = "AddByStandartSendListDocumentSendList", API = "", Description = "Добавить план работы по стандартному списку", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 103005, ObjectId = 103, Code = "ModifyDocumentSendList", API = "", Description = "Изменить пункт плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 103009, ObjectId = 103, Code = "DeleteDocumentSendList", API = "", Description = "Удалить пункт плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 103011, ObjectId = 106, Code = "AddDocumentSendListStage", API = "", Description = "Добавить этап плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 103019, ObjectId = 106, Code = "DeleteDocumentSendListStage", API = "", Description = "Удалить этап плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 103021, ObjectId = 103, Code = "LaunchDocumentSendListItem", API = "", Description = "Запустить пункт плана на исполнение", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 104001, ObjectId = 104, Code = "AddDocumentFile", API = "", Description = "Добавить файл", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 104005, ObjectId = 104, Code = "ModifyDocumentFile", API = "", Description = "Изменить файл", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 104009, ObjectId = 104, Code = "DeleteDocumentFile", API = "", Description = "Удалить файл", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 104010, ObjectId = 104, Code = "AddDocumentFileUseMainNameFile", API = "", Description = "Добавить версию файла к файлу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 104011, ObjectId = 104, Code = "AcceptDocumentFile", API = "", Description = "Файл принят", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 104012, ObjectId = 104, Code = "RejectDocumentFile", API = "", Description = "Файл не принят", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 104013, ObjectId = 104, Code = "RenameDocumentFile", API = "", Description = "Переименовать файл", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 104014, ObjectId = 104, Code = "DeleteDocumentFileVersion", API = "", Description = "Удалить версию файл", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 104015, ObjectId = 104, Code = "DeleteDocumentFileVersionRecord", API = "", Description = "Удалить запись о версим файла", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 104016, ObjectId = 104, Code = "AcceptMainVersionDocumentFile", API = "", Description = "Сделать основной версией", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 105001, ObjectId = 105, Code = "AddDocumentLink", API = "", Description = "Добавить связь между документами", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 105009, ObjectId = 105, Code = "DeleteDocumentLink", API = "", Description = "Удалить связь между документами", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 115001, ObjectId = 115, Code = "AddDocumentTask", API = "", Description = "Добавить задачу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 115005, ObjectId = 115, Code = "ModifyDocumentTask", API = "", Description = "Изменить задачу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 115009, ObjectId = 115, Code = "DeleteDocumentTask", API = "", Description = "Удалить задачу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 121001, ObjectId = 121, Code = "AddDocumentPaper", API = "", Description = "Добавить бумажный носитель", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            //items.Add(new SystemActions { Id = 121005, ObjectId = 121, Code = "ModifyDocumentPaper", API = "", Description = "Изменить бумажный носитель", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            //items.Add(new SystemActions { Id = 121007, ObjectId = 121, Code = "MarkOwnerDocumentPaper", API = "", Description = "Отметить нахождение бумажного носителя у себя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            //items.Add(new SystemActions { Id = 121008, ObjectId = 121, Code = "MarkСorruptionDocumentPaper", API = "", Description = "Отметить порчу бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            //items.Add(new SystemActions { Id = 121009, ObjectId = 121, Code = "DeleteDocumentPaper", API = "", Description = "Удалить бумажный носитель", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            //items.Add(new SystemActions { Id = 122001, ObjectId = 122, Code = "PlanDocumentPaperEvent", API = "", Description = "Планировать движение бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            //items.Add(new SystemActions { Id = 122009, ObjectId = 122, Code = "CancelPlanDocumentPaperEvent", API = "", Description = "Отменить планирование движения бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            //items.Add(new SystemActions { Id = 122011, ObjectId = 122, Code = "SendDocumentPaperEvent", API = "", Description = "Отметить передачу бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            //items.Add(new SystemActions { Id = 122019, ObjectId = 122, Code = "CancelSendDocumentPaperEvent", API = "", Description = "Отменить передачу бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            //items.Add(new SystemActions { Id = 122021, ObjectId = 122, Code = "RecieveDocumentPaperEvent", API = "", Description = "Отметить прием бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            //items.Add(new SystemActions { Id = 123001, ObjectId = 123, Code = "AddDocumentPaperList", API = "", Description = "Добавить реестр бумажных носителей", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Реестры бумажных носителей" });
            //items.Add(new SystemActions { Id = 123005, ObjectId = 123, Code = "ModifyDocumentPaperList", API = "", Description = "Изменить реестр бумажных носителей", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Реестры бумажных носителей" });
            //items.Add(new SystemActions { Id = 123009, ObjectId = 123, Code = "DeleteDocumentPaperList", API = "", Description = "Удалить реестр бумажных носителей", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Реестры бумажных носителей" });
            //items.Add(new SystemActions { Id = 191001, ObjectId = 191, Code = "AddSavedFilter", API = "", Description = "Добавить сохраненный фильтр", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 191005, ObjectId = 191, Code = "ModifySavedFilter", API = "", Description = "Изменить сохраненный фильтр", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 191009, ObjectId = 191, Code = "DeleteSavedFilter", API = "", Description = "Удалить сохраненный фильтр", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 192005, ObjectId = 192, Code = "ModifyDocumentTags", API = "", Description = "Изменить тэги по документу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 201001, ObjectId = 201, Code = "AddDocumentType", API = "", Description = "Добавить тип документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 201005, ObjectId = 201, Code = "ModifyDocumentType", API = "", Description = "Изменить тип документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 201009, ObjectId = 201, Code = "DeleteDocumentType", API = "", Description = "Удалить тип документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 202001, ObjectId = 202, Code = "AddAddressType", API = "", Description = "Добавить тип адреса", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 202005, ObjectId = 202, Code = "ModifyAddressType", API = "", Description = "Изменить тип адреса", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 202009, ObjectId = 202, Code = "DeleteAddressType", API = "", Description = "Удалить тип адреса", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 203001, ObjectId = 203, Code = "AddDocumentSubject", API = "", Description = "Добавить тематику", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 203005, ObjectId = 203, Code = "ModifyDocumentSubject", API = "", Description = "Изменить тематику", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 203009, ObjectId = 203, Code = "DeleteDocumentSubject", API = "", Description = "Удалить тематику", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 204001, ObjectId = 204, Code = "AddRegistrationJournal", API = "", Description = "Добавить журнал регистрации", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 204005, ObjectId = 204, Code = "ModifyRegistrationJournal", API = "", Description = "Изменить журнал регистрации", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 204009, ObjectId = 204, Code = "DeleteRegistrationJournal", API = "", Description = "Удалить журнал регистрации", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 205001, ObjectId = 205, Code = "AddContactType", API = "", Description = "Добавить тип контакта", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 205005, ObjectId = 205, Code = "ModifyContactType", API = "", Description = "Изменить тип контакта", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 205009, ObjectId = 205, Code = "DeleteContactType", API = "", Description = "Удалить тип контакта", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 206001, ObjectId = 206, Code = "AddAgent", API = "", Description = "Добавить контрагента", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 206005, ObjectId = 206, Code = "ModifyAgent", API = "", Description = "Изменить контрагента", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 206009, ObjectId = 206, Code = "DeleteAgent", API = "", Description = "Удалить контрагента", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 207001, ObjectId = 207, Code = "AddContact", API = "", Description = "Добавить контакт", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 207005, ObjectId = 207, Code = "ModifyContact", API = "", Description = "Изменить контакт", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 207009, ObjectId = 207, Code = "DeleteContact", API = "", Description = "Удалить контакт", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 208001, ObjectId = 208, Code = "AddAddress", API = "", Description = "Добавить адрес", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 208005, ObjectId = 208, Code = "ModifyAddress", API = "", Description = "Изменить адрес", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 208009, ObjectId = 208, Code = "DeleteAddress", API = "", Description = "Удалить адрес", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 209001, ObjectId = 209, Code = "AddAgentPerson", API = "", Description = "Добавить физическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 209005, ObjectId = 209, Code = "ModifyAgentPerson", API = "", Description = "Изменить физическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 209009, ObjectId = 209, Code = "DeleteAgentPerson", API = "", Description = "Удалить физическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 210001, ObjectId = 210, Code = "AddDepartment", API = "", Description = "Добавить подразделение", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 210005, ObjectId = 210, Code = "ModifyDepartment", API = "", Description = "Изменить подразделение", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 210009, ObjectId = 210, Code = "DeleteDepartment", API = "", Description = "Удалить подразделение", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 211001, ObjectId = 211, Code = "AddPosition", API = "", Description = "Добавить должность", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 211005, ObjectId = 211, Code = "ModifyPosition", API = "", Description = "Изменить должность", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 211009, ObjectId = 211, Code = "DeletePosition", API = "", Description = "Удалить должность", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 212001, ObjectId = 212, Code = "AddAgentEmployee", API = "", Description = "Добавить сотрудника", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 212005, ObjectId = 212, Code = "ModifyAgentEmployee", API = "", Description = "Изменить сотрудника", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 212009, ObjectId = 212, Code = "DeleteAgentEmployee", API = "", Description = "Удалить сотрудника", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 213001, ObjectId = 213, Code = "AddAgentCompany", API = "", Description = "Добавить юридическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 213005, ObjectId = 213, Code = "ModifyAgentCompany", API = "", Description = "Изменить юридическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 213009, ObjectId = 213, Code = "DeleteAgentCompany", API = "", Description = "Удалить юридическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 214001, ObjectId = 214, Code = "AddAgentBank", API = "", Description = "Добавить банк", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 214005, ObjectId = 214, Code = "ModifyAgentBank", API = "", Description = "Изменить банк", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 214009, ObjectId = 214, Code = "DeleteAgentBank", API = "", Description = "Удалить банк", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 215001, ObjectId = 215, Code = "AddAgentAccount", API = "", Description = "Добавить расчетный счет", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 215005, ObjectId = 215, Code = "ModifyAgentAccount", API = "", Description = "Изменить расчетный счет", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 215009, ObjectId = 215, Code = "DeleteAgentAccount", API = "", Description = "Удалить расчетный счет", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 216001, ObjectId = 216, Code = "AddStandartSendListContent", API = "", Description = "Добавить содержание типового списка рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 216005, ObjectId = 216, Code = "ModifyStandartSendListContent", API = "", Description = "Изменить содержание типового списка рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 216009, ObjectId = 216, Code = "DeleteStandartSendListContent", API = "", Description = "Удалить содержание типового списка рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 217001, ObjectId = 217, Code = "AddStandartSendList", API = "", Description = "Добавить типовой список рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 217005, ObjectId = 217, Code = "ModifyStandartSendList", API = "", Description = "Изменить типовой список рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 217009, ObjectId = 217, Code = "DeleteStandartSendList", API = "", Description = "Удалить типовой список рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 218001, ObjectId = 218, Code = "AddCompany", API = "", Description = "Добавить компанию", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 218005, ObjectId = 218, Code = "ModifyCompany", API = "", Description = "Изменить компанию", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 218009, ObjectId = 218, Code = "DeleteCompany", API = "", Description = "Удалить компанию", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 219001, ObjectId = 219, Code = "AddExecutorType", API = "", Description = "Добавить тип исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 219005, ObjectId = 219, Code = "ModifyExecutorType", API = "", Description = "Изменить тип исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 219009, ObjectId = 219, Code = "DeleteExecutorType", API = "", Description = "Удалить тип исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 220001, ObjectId = 220, Code = "AddExecutor", API = "", Description = "Добавить исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 220005, ObjectId = 220, Code = "ModifyExecutor", API = "", Description = "Изменить исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 220009, ObjectId = 220, Code = "DeleteExecutor", API = "", Description = "Удалить исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 251001, ObjectId = 251, Code = "AddTemplateDocument", API = "", Description = "Добавить шаблон документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 251005, ObjectId = 251, Code = "ModifyTemplateDocument", API = "", Description = "Изменить шаблон документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 251009, ObjectId = 251, Code = "DeleteTemplateDocument", API = "", Description = "Удалить шаблон документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 252001, ObjectId = 252, Code = "AddTemplateDocumentSendList", API = "", Description = "Добавить список рассылки в шаблон", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 252005, ObjectId = 252, Code = "ModifyTemplateDocumentSendList", API = "", Description = "Изменить список рассылки в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 252009, ObjectId = 252, Code = "DeleteTemplateDocumentSendList", API = "", Description = "Удалить список рассылки в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 253001, ObjectId = 253, Code = "AddTemplateDocumentRestrictedSendList", API = "", Description = "Добавить ограничительный список рассылки в шаблон", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 253005, ObjectId = 253, Code = "ModifyTemplateDocumentRestrictedSendList", API = "", Description = "Изменить ограничительный список рассылки в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 253009, ObjectId = 253, Code = "DeleteTemplateDocumentRestrictedSendList", API = "", Description = "Удалить ограничительный список рассылки в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 254001, ObjectId = 254, Code = "AddTemplateDocumentTask", API = "", Description = "Добавить задачу в шаблон", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 254005, ObjectId = 254, Code = "ModifyTemplateDocumentTask", API = "", Description = "Изменить задачу в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 254009, ObjectId = 254, Code = "DeleteTemplateDocumentTask", API = "", Description = "Удалить задачу в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 255001, ObjectId = 255, Code = "AddTemplateAttachedFile", API = "", Description = "Добавить файл в шаблон", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 255005, ObjectId = 255, Code = "ModifyTemplateAttachedFile", API = "", Description = "Изменить файл в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 255009, ObjectId = 255, Code = "DeleteTemplateAttachedFile", API = "", Description = "Удалить файл в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 291001, ObjectId = 291, Code = "AddTag", API = "", Description = "Добавить тэг", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 291005, ObjectId = 291, Code = "ModifyTag", API = "", Description = "Изменить тэг", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 291009, ObjectId = 291, Code = "DeleteTag", API = "", Description = "Удалить тэг", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 301001, ObjectId = 301, Code = "AddCustomDictionaryType", API = "", Description = "Добавить тип пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 301005, ObjectId = 301, Code = "ModifyCustomDictionaryType", API = "", Description = "Изменить тип пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 301009, ObjectId = 301, Code = "DeleteCustomDictionaryType", API = "", Description = "Удалить тип пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 302001, ObjectId = 302, Code = "AddCustomDictionary", API = "", Description = "Добавить запись пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 302005, ObjectId = 302, Code = "ModifyCustomDictionary", API = "", Description = "Изменить запись пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 302009, ObjectId = 302, Code = "DeleteCustomDictionary", API = "", Description = "Удалить запись пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 311001, ObjectId = 311, Code = "AddProperty", API = "", Description = "Добавить динамический аттрибут", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 311005, ObjectId = 311, Code = "ModifyProperty", API = "", Description = "Изменить динамический аттрибут", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 311009, ObjectId = 311, Code = "DeleteProperty", API = "", Description = "Удалить динамический аттрибут", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 312001, ObjectId = 312, Code = "AddPropertyLink", API = "", Description = "Добавить связь динамических аттрибутов", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 312005, ObjectId = 312, Code = "ModifyPropertyLink", API = "", Description = "Изменить связь динамических аттрибутов", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 312009, ObjectId = 312, Code = "DeletePropertyLink", API = "", Description = "Удалить связь динамических аттрибутов", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            //items.Add(new SystemActions { Id = 313005, ObjectId = 313, Code = "ModifyPropertyValues", API = "", Description = "Изменить значение динамических аттрибутов", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            #endregion

            items.Add(GetSysAct(EnumDocumentActions.AddDocument, EnumObjects.Documents, "Создать документ по шаблону", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.CopyDocument, EnumObjects.Documents, "Создать документ копированием", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocument, EnumObjects.Documents, "Изменить документ", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocument, EnumObjects.Documents, "Удалить проект", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.LaunchPlan, EnumObjects.Documents, "Запустить выполнение плана", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentSendListItem, EnumObjects.Documents, "Добавить пункт плана", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.StopPlan, EnumObjects.Documents, "Остановить выполнение плана", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.ChangeExecutor, EnumObjects.Documents, "Передать управление", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.RegisterDocument, EnumObjects.Documents, "Зарегистрировать проект", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.MarkDocumentEventAsRead, EnumObjects.Documents, "Отметить прочтение событий по документу", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.SendForInformation, EnumObjects.Documents, "Направить для сведения", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.SendForConsideration, EnumObjects.Documents, "Направить для рассмотрения", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.SendForInformationExternal, EnumObjects.Documents, "Направить для сведения внешнему агенту", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.SendForControl, EnumObjects.Documents, "Направить для контроля", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForResponsibleExecution, EnumObjects.Documents, "Направить для отв.исполнения", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForExecution, EnumObjects.Documents, "Направить для исполнения", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForVisaing, EnumObjects.Documents, "Направить для визирования", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.SendForАgreement, EnumObjects.Documents, "Направить для согласование", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.SendForАpproval, EnumObjects.Documents, "Направить для утверждения", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.SendForSigning, EnumObjects.Documents, "Направить для подписи", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.ReportRegistrationCardDocument, EnumObjects.Documents, "Регистрационная карточка", "Отчеты"));
            items.Add(GetSysAct(EnumDocumentActions.AddFavourite, EnumObjects.Documents, "Добавить в избранное", "Дополнительно"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteFavourite, EnumObjects.Documents, "Удалить из избранного", "Дополнительно"));
            items.Add(GetSysAct(EnumDocumentActions.FinishWork, EnumObjects.Documents, "Закончить работу с документом", "Дополнительно"));
            items.Add(GetSysAct(EnumDocumentActions.StartWork, EnumObjects.Documents, "Возобновить работу с документом", "Дополнительно"));
            items.Add(GetSysAct(EnumDocumentActions.ChangePosition, EnumObjects.Documents, "Поменять должность в документе", "Администратор"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists, "Добавить ограничение рассылки"));
            items.Add(GetSysAct(EnumDocumentActions.AddByStandartSendListDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists, "Добавить ограничения рассылки по стандартному списку"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists, "Удалить ограничение рассылки"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentSendList, EnumObjects.DocumentSendLists, "Изменить пункт плана"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentSendList, EnumObjects.DocumentSendLists, "Удалить пункт плана"));
            items.Add(GetSysAct(EnumDocumentActions.LaunchDocumentSendListItem, EnumObjects.DocumentSendLists, "Запустить пункт плана на исполнение"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentFile, EnumObjects.DocumentFiles, "Добавить файл"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentFile, EnumObjects.DocumentFiles, "Изменить файл"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentFile, EnumObjects.DocumentFiles, "Удалить файл"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentFileUseMainNameFile, EnumObjects.DocumentFiles, "Добавить версию файла к файлу"));
            items.Add(GetSysAct(EnumDocumentActions.AcceptDocumentFile, EnumObjects.DocumentFiles, "Файл принят"));
            items.Add(GetSysAct(EnumDocumentActions.RejectDocumentFile, EnumObjects.DocumentFiles, "Файл не принят"));
            items.Add(GetSysAct(EnumDocumentActions.RenameDocumentFile, EnumObjects.DocumentFiles, "Переименовать файл"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentFileVersion, EnumObjects.DocumentFiles, "Удалить версию файл"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentFileVersionRecord, EnumObjects.DocumentFiles, "Удалить запись о версим файла"));
            items.Add(GetSysAct(EnumDocumentActions.AcceptMainVersionDocumentFile, EnumObjects.DocumentFiles, "Сделать основной версией"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentLink, EnumObjects.DocumentLinks, "Добавить связь между документами"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentLink, EnumObjects.DocumentLinks, "Удалить связь между документами"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentSendList, EnumObjects.DocumentSendListStages, "Добавить пункт плана"));
            items.Add(GetSysAct(EnumDocumentActions.AddByStandartSendListDocumentSendList, EnumObjects.DocumentSendListStages, "Добавить план работы по стандартному списку"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentSendListStage, EnumObjects.DocumentSendListStages, "Добавить этап плана"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentSendListStage, EnumObjects.DocumentSendListStages, "Удалить этап плана"));
            items.Add(GetSysAct(EnumDocumentActions.SendMessage, EnumObjects.DocumentEvents, "Направить сообщение участникам рабочей группы", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.AddNote, EnumObjects.DocumentEvents, "Добавить примечание", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.ControlOn, EnumObjects.DocumentWaits, "Взять на контроль", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.ControlChange, EnumObjects.DocumentWaits, "Изменить параметры контроля", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForExecutionChange, EnumObjects.DocumentWaits, "Изменить параметры направлен для исполнения", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForResponsibleExecutionChange, EnumObjects.DocumentWaits, "Изменить параметры направлен для отв.исполнения", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.ControlTargetChange, EnumObjects.DocumentWaits, "Изменить параметры контроля для исполнителя", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.ControlOff, EnumObjects.DocumentWaits, "Снять с контроля", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.MarkExecution, EnumObjects.DocumentWaits, "Отметить исполнение", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.AcceptResult, EnumObjects.DocumentWaits, "Принять результат", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.RejectResult, EnumObjects.DocumentWaits, "Отклонить результат", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawVisaing, EnumObjects.DocumentSubscriptions, "Отозвать с визирования", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawАgreement, EnumObjects.DocumentSubscriptions, "Отозвать с согласования", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawАpproval, EnumObjects.DocumentSubscriptions, "Отозвать с утверждения", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawSigning, EnumObjects.DocumentSubscriptions, "Отозвать с подписи", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixVisaing, EnumObjects.DocumentSubscriptions, "Завизировать", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixАgreement, EnumObjects.DocumentSubscriptions, "Согласовать", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixАpproval, EnumObjects.DocumentSubscriptions, "Утвердить", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixSigning, EnumObjects.DocumentSubscriptions, "Подписать", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.SelfAffixSigning, EnumObjects.DocumentSubscriptions, "Самоподписание", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectVisaing, EnumObjects.DocumentSubscriptions, "Отказать в визирования", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectАgreement, EnumObjects.DocumentSubscriptions, "Отказать в согласование", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectАpproval, EnumObjects.DocumentSubscriptions, "Отказать в утверждения", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectSigning, EnumObjects.DocumentSubscriptions, "Отказать в подписи", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentTask, EnumObjects.DocumentTasks, "Добавить задачу"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentTask, EnumObjects.DocumentTasks, "Изменить задачу"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentTask, EnumObjects.DocumentTasks, "Удалить задачу"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentPaper, EnumObjects.DocumentPapers, "Добавить бумажный носитель", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentPaper, EnumObjects.DocumentPapers, "Изменить бумажный носитель", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.MarkOwnerDocumentPaper, EnumObjects.DocumentPapers, "Отметить нахождение бумажного носителя у себя", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.MarkСorruptionDocumentPaper, EnumObjects.DocumentPapers, "Отметить порчу бумажного носителя", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentPaper, EnumObjects.DocumentPapers, "Удалить бумажный носитель", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.PlanDocumentPaperEvent, EnumObjects.DocumentPaperEvents, "Планировать движение бумажного носителя", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.CancelPlanDocumentPaperEvent, EnumObjects.DocumentPaperEvents, "Отменить планирование движения бумажного носителя", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.SendDocumentPaperEvent, EnumObjects.DocumentPaperEvents, "Отметить передачу бумажного носителя", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.CancelSendDocumentPaperEvent, EnumObjects.DocumentPaperEvents, "Отменить передачу бумажного носителя", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.RecieveDocumentPaperEvent, EnumObjects.DocumentPaperEvents, "Отметить прием бумажного носителя", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentPaperList, EnumObjects.DocumentPaperLists, "Добавить реестр бумажных носителей", "Реестры бумажных носителей"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentPaperList, EnumObjects.DocumentPaperLists, "Изменить реестр бумажных носителей", "Реестры бумажных носителей"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentPaperList, EnumObjects.DocumentPaperLists, "Удалить реестр бумажных носителей", "Реестры бумажных носителей"));
            items.Add(GetSysAct(EnumDocumentActions.AddSavedFilter, EnumObjects.DocumentSavedFilters, "Добавить сохраненный фильтр"));
            items.Add(GetSysAct(EnumDocumentActions.ModifySavedFilter, EnumObjects.DocumentSavedFilters, "Изменить сохраненный фильтр"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteSavedFilter, EnumObjects.DocumentSavedFilters, "Удалить сохраненный фильтр"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentTags, EnumObjects.DocumentTags, "Изменить тэги по документу"));

            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocument, EnumObjects.TemplateDocument, "Добавить шаблон документа"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocument, EnumObjects.TemplateDocument, "Изменить шаблон документа"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocument, EnumObjects.TemplateDocument, "Удалить шаблон документа"));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocumentSendList, EnumObjects.TemplateDocumentSendList, "Добавить список рассылки в шаблон"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocumentSendList, EnumObjects.TemplateDocumentSendList, "Изменить список рассылки в шаблоне"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocumentSendList, EnumObjects.TemplateDocumentSendList, "Удалить список рассылки в шаблоне"));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocumentRestrictedSendList, EnumObjects.TemplateDocumentRestrictedSendList, "Добавить ограничительный список рассылки в шаблон"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocumentRestrictedSendList, EnumObjects.TemplateDocumentRestrictedSendList, "Изменить ограничительный список рассылки в шаблоне"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocumentRestrictedSendList, EnumObjects.TemplateDocumentRestrictedSendList, "Удалить ограничительный список рассылки в шаблоне"));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocumentTask, EnumObjects.TemplateDocumentTask, "Добавить задачу в шаблон"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocumentTask, EnumObjects.TemplateDocumentTask, "Изменить задачу в шаблоне"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocumentTask, EnumObjects.TemplateDocumentTask, "Удалить задачу в шаблоне"));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateAttachedFile, EnumObjects.TemplateDocumentAttachedFiles, "Добавить файл в шаблон"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateAttachedFile, EnumObjects.TemplateDocumentAttachedFiles, "Изменить файл в шаблоне"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateAttachedFile, EnumObjects.TemplateDocumentAttachedFiles, "Удалить файл в шаблоне"));

            items.Add(GetSysAct(EnumDictionaryActions.AddDocumentType, EnumObjects.DictionaryDocumentType, "Добавить тип документа"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyDocumentType, EnumObjects.DictionaryDocumentType, "Изменить тип документа"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteDocumentType, EnumObjects.DictionaryDocumentType, "Удалить тип документа"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAddressType, EnumObjects.DictionaryAddressType, "Добавить тип адреса"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAddressType, EnumObjects.DictionaryAddressType, "Изменить тип адреса"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAddressType, EnumObjects.DictionaryAddressType, "Удалить тип адреса"));
            items.Add(GetSysAct(EnumDictionaryActions.AddDocumentSubject, EnumObjects.DictionaryDocumentSubjects, "Добавить тематику"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyDocumentSubject, EnumObjects.DictionaryDocumentSubjects, "Изменить тематику"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteDocumentSubject, EnumObjects.DictionaryDocumentSubjects, "Удалить тематику"));
            items.Add(GetSysAct(EnumDictionaryActions.AddRegistrationJournal, EnumObjects.DictionaryRegistrationJournals, "Добавить журнал регистрации"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyRegistrationJournal, EnumObjects.DictionaryRegistrationJournals, "Изменить журнал регистрации"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteRegistrationJournal, EnumObjects.DictionaryRegistrationJournals, "Удалить журнал регистрации"));
            items.Add(GetSysAct(EnumDictionaryActions.AddContactType, EnumObjects.DictionaryContactType, "Добавить тип контакта"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyContactType, EnumObjects.DictionaryContactType, "Изменить тип контакта"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteContactType, EnumObjects.DictionaryContactType, "Удалить тип контакта"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgent, EnumObjects.DictionaryAgents, "Добавить контрагента"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgent, EnumObjects.DictionaryAgents, "Изменить контрагента"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgent, EnumObjects.DictionaryAgents, "Удалить контрагента"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentContact, EnumObjects.DictionaryContacts, "Добавить контакт"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentContact, EnumObjects.DictionaryContacts, "Изменить контакт"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentContact, EnumObjects.DictionaryContacts, "Удалить контакт"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentAddress, EnumObjects.DictionaryAgentAddresses, "Добавить адрес"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentAddress, EnumObjects.DictionaryAgentAddresses, "Изменить адрес"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentAddress, EnumObjects.DictionaryAgentAddresses, "Удалить адрес"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentPerson, EnumObjects.DictionaryAgentPersons, "Добавить физическое лицо"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentPerson, EnumObjects.DictionaryAgentPersons, "Изменить физическое лицо"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentPerson, EnumObjects.DictionaryAgentPersons, "Удалить физическое лицо"));
            items.Add(GetSysAct(EnumDictionaryActions.AddDepartment, EnumObjects.DictionaryDepartments, "Добавить подразделение"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyDepartment, EnumObjects.DictionaryDepartments, "Изменить подразделение"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteDepartment, EnumObjects.DictionaryDepartments, "Удалить подразделение"));
            items.Add(GetSysAct(EnumDictionaryActions.AddPosition, EnumObjects.DictionaryPositions, "Добавить должность"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyPosition, EnumObjects.DictionaryPositions, "Изменить должность"));
            items.Add(GetSysAct(EnumDictionaryActions.DeletePosition, EnumObjects.DictionaryPositions, "Удалить должность"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentEmployee, EnumObjects.DictionaryAgentEmployees, "Добавить сотрудника"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentEmployee, EnumObjects.DictionaryAgentEmployees, "Изменить сотрудника"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentEmployee, EnumObjects.DictionaryAgentEmployees, "Удалить сотрудника"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentCompany, EnumObjects.DictionaryAgentCompanies, "Добавить юридическое лицо"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentCompany, EnumObjects.DictionaryAgentCompanies, "Изменить юридическое лицо"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentCompany, EnumObjects.DictionaryAgentCompanies, "Удалить юридическое лицо"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentBank, EnumObjects.DictionaryAgentBanks, "Добавить банк"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentBank, EnumObjects.DictionaryAgentBanks, "Изменить банк"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentBank, EnumObjects.DictionaryAgentBanks, "Удалить банк"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentAccount, EnumObjects.DictionaryAgentAccounts, "Добавить расчетный счет"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentAccount, EnumObjects.DictionaryAgentAccounts, "Изменить расчетный счет"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentAccount, EnumObjects.DictionaryAgentAccounts, "Удалить расчетный счет"));
            items.Add(GetSysAct(EnumDictionaryActions.AddStandartSendListContent, EnumObjects.DictionaryStandartSendListContent, "Добавить содержание типового списка рассылки"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyStandartSendListContent, EnumObjects.DictionaryStandartSendListContent, "Изменить содержание типового списка рассылки"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteStandartSendListContent, EnumObjects.DictionaryStandartSendListContent, "Удалить содержание типового списка рассылки"));
            items.Add(GetSysAct(EnumDictionaryActions.AddStandartSendList, EnumObjects.DictionaryStandartSendLists, "Добавить типовой список рассылки"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyStandartSendList, EnumObjects.DictionaryStandartSendLists, "Изменить типовой список рассылки"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteStandartSendList, EnumObjects.DictionaryStandartSendLists, "Удалить типовой список рассылки"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentClientCompany, EnumObjects.DictionaryAgentClientCompanies, "Добавить компанию"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentClientCompany, EnumObjects.DictionaryAgentClientCompanies, "Изменить компанию"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentClientCompany, EnumObjects.DictionaryAgentClientCompanies, "Удалить компанию"));
            items.Add(GetSysAct(EnumDictionaryActions.AddExecutorType, EnumObjects.DictionaryPositionExecutorTypes, "Добавить тип исполнителя"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyExecutorType, EnumObjects.DictionaryPositionExecutorTypes, "Изменить тип исполнителя"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteExecutorType, EnumObjects.DictionaryPositionExecutorTypes, "Удалить тип исполнителя"));
            items.Add(GetSysAct(EnumDictionaryActions.AddExecutor, EnumObjects.DictionaryPositionExecutors, "Добавить исполнителя"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyExecutor, EnumObjects.DictionaryPositionExecutors, "Изменить исполнителя"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteExecutor, EnumObjects.DictionaryPositionExecutors, "Удалить исполнителя"));

            items.Add(GetSysAct(EnumDictionaryActions.AddTag, EnumObjects.DictionaryTag, "Добавить тэг"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyTag, EnumObjects.DictionaryTag, "Изменить тэг"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteTag, EnumObjects.DictionaryTag, "Удалить тэг"));
            items.Add(GetSysAct(EnumDictionaryActions.AddCustomDictionaryType, EnumObjects.CustomDictionaryTypes, "Добавить тип пользовательского словаря"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyCustomDictionaryType, EnumObjects.CustomDictionaryTypes, "Изменить тип пользовательского словаря"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteCustomDictionaryType, EnumObjects.CustomDictionaryTypes, "Удалить тип пользовательского словаря"));
            items.Add(GetSysAct(EnumDictionaryActions.AddCustomDictionary, EnumObjects.CustomDictionaries, "Добавить запись пользовательского словаря"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyCustomDictionary, EnumObjects.CustomDictionaries, "Изменить запись пользовательского словаря"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteCustomDictionary, EnumObjects.CustomDictionaries, "Удалить запись пользовательского словаря"));
            items.Add(GetSysAct(EnumPropertyAction.AddProperty, EnumObjects.Properties, "Добавить динамический аттрибут"));
            items.Add(GetSysAct(EnumPropertyAction.ModifyProperty, EnumObjects.Properties, "Изменить динамический аттрибут"));
            items.Add(GetSysAct(EnumPropertyAction.DeleteProperty, EnumObjects.Properties, "Удалить динамический аттрибут"));
            items.Add(GetSysAct(EnumPropertyAction.AddPropertyLink, EnumObjects.PropertyLinks, "Добавить связь динамических аттрибутов"));
            items.Add(GetSysAct(EnumPropertyAction.ModifyPropertyLink, EnumObjects.PropertyLinks, "Изменить связь динамических аттрибутов"));
            items.Add(GetSysAct(EnumPropertyAction.DeletePropertyLink, EnumObjects.PropertyLinks, "Удалить связь динамических аттрибутов"));
            items.Add(GetSysAct(EnumPropertyAction.ModifyPropertyValues, EnumObjects.PropertyValues, "Изменить значение динамических аттрибутов"));



            items.Add(GetSysAct(EnumEncryptionActions.AddEncryptionCertificate, EnumObjects.EncryptionCertificates, "Добавить сертификат"));
            items.Add(GetSysAct(EnumEncryptionActions.ModifyEncryptionCertificate, EnumObjects.EncryptionCertificates, "Изменить сертификат"));
            items.Add(GetSysAct(EnumEncryptionActions.VerifyPdf, EnumObjects.EncryptionCertificates, "Проверка Pdf"));
            items.Add(GetSysAct(EnumEncryptionActions.DeleteEncryptionCertificate, EnumObjects.EncryptionCertificates, "Удалить сертификат"));

            items.Add(GetSysAct(EnumAdminActions.AddRole, EnumObjects.AdminRoles, "Добавить роль"));
            items.Add(GetSysAct(EnumAdminActions.ModifyRole, EnumObjects.AdminRoles, "Изменить роль"));
            items.Add(GetSysAct(EnumAdminActions.DeleteRole, EnumObjects.AdminRoles, "Удалить роль"));

            items.Add(GetSysAct(EnumAdminActions.AddPositionRole, EnumObjects.AdminPositionRoles, "Добавить роль для должности"));
            items.Add(GetSysAct(EnumAdminActions.ModifyPositionRole, EnumObjects.AdminPositionRoles, "Изменить роль для должности"));
            items.Add(GetSysAct(EnumAdminActions.DeletePositionRole, EnumObjects.AdminPositionRoles, "Удалить роль для должности"));

            items.Add(GetSysAct(EnumAdminActions.AddUserRole, EnumObjects.AdminUserRoles, "Добавить роль для пользователя"));
            items.Add(GetSysAct(EnumAdminActions.ModifyUserRole, EnumObjects.AdminUserRoles, "Изменить роль для пользователя"));
            items.Add(GetSysAct(EnumAdminActions.DeleteUserRole, EnumObjects.AdminUserRoles, "Удалить роль для пользователя"));

            items.Add(GetSysAct(EnumAdminActions.SetSubordination, EnumObjects.AdminSubordination, "Управление правилами рассылки"));

            return items;
        }

        private static SystemActions GetSysAct(EnumAdminActions id, EnumObjects objId, string description, string category = null, bool isGrantable = false, bool isGrantableByRecordId = false, bool isVisible = false, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSysAct(EnumEncryptionActions id, EnumObjects objId, string description, string category = null, bool isGrantable = false, bool isGrantableByRecordId = false, bool isVisible = false, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSysAct(EnumPropertyAction id, EnumObjects objId, string description, string category = null, bool isGrantable = false, bool isGrantableByRecordId = false, bool isVisible = false, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSysAct(EnumDictionaryActions id, EnumObjects objId, string description, string category = null, bool isGrantable = false, bool isGrantableByRecordId = false, bool isVisible = false, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSysAct(EnumDocumentActions id, EnumObjects objId, string description, string category = null, bool isGrantable = false, bool isGrantableByRecordId = false, bool isVisible = false, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSystemAction(int id, string code, EnumObjects objId, string description, string category = null, bool isGrantable = false, bool isGrantableByRecordId = false, bool isVisible = false, int? grantId = null)
        {
            return new SystemActions()
            {
                Id = id,
                ObjectId = (int)objId,
                Code = code,
                Description = description,
                API = "",
                IsGrantable = isGrantable,
                IsGrantableByRecordId = isGrantableByRecordId,
                IsVisible = isVisible,
                GrantId = grantId,
                Category = category
            };
        }
        #endregion

        public static List<AdminAccessLevels> GetAdminAccessLevels()
        {
            var items = new List<AdminAccessLevels>();

            items.Add(new AdminAccessLevels { Id = 10, Code = null, Name = "Только лично", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new AdminAccessLevels { Id = 20, Code = null, Name = "Лично+референты", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new AdminAccessLevels { Id = 30, Code = null, Name = "Лично+референты+ИО", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<SystemUIElements> GetSystemUIElements()
        {
            var items = new List<SystemUIElements>();

            items.Add(new SystemUIElements { Id = 1, ActionId = 100003, Code = "GeneralInfo", TypeCode = "display_only_text", Description = "Общая информация", Label = null, Hint = null, ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "GeneralInfo", ValueDescriptionFieldCode = "GeneralInfo", Format = null });
            items.Add(new SystemUIElements { Id = 2, ActionId = 100003, Code = "DocumentSubject", TypeCode = "select", Description = "Тематика документа", Label = "Тематика документа", Hint = "Выберите из словаря тематику документа", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "DictionaryDocumentSubjects", SelectFilter = null, SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "DocumentSubjectId", ValueDescriptionFieldCode = "DocumentSubjectName", Format = null });
            items.Add(new SystemUIElements { Id = 3, ActionId = 100003, Code = "Description", TypeCode = "textarea", Description = "Краткое содержание", Label = "Краткое содержание", Hint = "Введите краткое содержание", ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "Description", ValueDescriptionFieldCode = "Description", Format = null });
            items.Add(new SystemUIElements { Id = 4, ActionId = 100003, Code = "SenderAgent", TypeCode = "select", Description = "Контрагент, от которого получен документ", Label = "Организация", Hint = "Выберите из словаря контрагента, от которого получен документ", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "DictionaryAgents", SelectFilter = "{'IsCompany' : 'True'}", SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "SenderAgentId", ValueDescriptionFieldCode = "SenderAgentName", Format = null });
            items.Add(new SystemUIElements { Id = 5, ActionId = 100003, Code = "SenderAgentPerson", TypeCode = "select", Description = "Контактное лицо в организации", Label = "Контакт", Hint = "Выберите из словаря контактное лицо в организации, от которой получен документ", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "DictionaryAgentPersons", SelectFilter = "{'AgentCompanyId' : '@SenderAgentId'}", SelectFieldCode = "Id", SelectDescriptionFieldCode = "FullName", ValueFieldCode = "SenderAgentPersonId", ValueDescriptionFieldCode = "SenderAgentPersonName", Format = null });
            items.Add(new SystemUIElements { Id = 6, ActionId = 100003, Code = "SenderNumber", TypeCode = "input", Description = "Входящий номер документа", Label = "Входящий номер документа", Hint = "Введите входящий номер документа", ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "SenderNumber", ValueDescriptionFieldCode = "SenderNumber", Format = null });
            items.Add(new SystemUIElements { Id = 7, ActionId = 100003, Code = "SenderDate", TypeCode = "input", Description = "Дата входящего документа", Label = "Дата входящего документа", Hint = "Введите дату входящего документа", ValueTypeId = 3, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "SenderDate", ValueDescriptionFieldCode = "SenderDate", Format = null });
            items.Add(new SystemUIElements { Id = 8, ActionId = 100003, Code = "Addressee", TypeCode = "input", Description = "Кому адресован документ", Label = "Кому адресован документ", Hint = "Введите кому адресован документ", ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "Addressee", ValueDescriptionFieldCode = "Addressee", Format = null });
            items.Add(new SystemUIElements { Id = 9, ActionId = 100003, Code = "AccessLevel", TypeCode = "select", Description = "Уровень доступа", Label = "Уровень доступа", Hint = "Выберите из словаря уровень доступа", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "AdminAccessLevels", SelectFilter = null, SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "AccessLevelId", ValueDescriptionFieldCode = "AccessLevelName", Format = null });

            return items;
        }

        public static List<SystemValueTypes> GetSystemValueTypes()
        {
            var items = new List<SystemValueTypes>();

            items.Add(new SystemValueTypes { Id = 1, Code = "text", Description = "text" });
            items.Add(new SystemValueTypes { Id = 2, Code = "number", Description = "number" });
            items.Add(new SystemValueTypes { Id = 3, Code = "date", Description = "date" });

            return items;
        }

        public static List<DictionaryFileTypes> GetDictionaryFileTypes()
        {
            var items = new List<DictionaryFileTypes>();

            items.Add(new DictionaryFileTypes { Id = 0, Name = "Main", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryFileTypes { Id = 1, Name = "Additional", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryFileTypes { Id = 2, Name = "SubscribePdf", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionarySigningTypes> GetDictionarySigningTypes()
        {
            var items = new List<DictionarySigningTypes>();

            items.Add(new DictionarySigningTypes { Id = 0, Name = "Hash", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySigningTypes { Id = 1, Name = "InternalSign", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySigningTypes { Id = 2, Name = "CertificateSign", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionaryDocumentDirections> GetDictionaryDocumentDirections()
        {
            var items = new List<DictionaryDocumentDirections>();

            items.Add(new DictionaryDocumentDirections { Id = 1, Code = "1", Name = "##l@DictionaryDocumentDirections:Incoming@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryDocumentDirections { Id = 2, Code = "2", Name = "##l@DictionaryDocumentDirections:Outcoming@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryDocumentDirections { Id = 3, Code = "3", Name = "##l@DictionaryDocumentDirections:Internal@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionaryEventTypes> GetDictionaryEventTypes()
        {
            var items = new List<DictionaryEventTypes>();

            items.Add(new DictionaryEventTypes { Id = 100, Code = null, Name = "Поступил входящий документ", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 110, Code = null, Name = "Создан проект", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 115, Code = null, Name = "Добавлен файл", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 117, Code = null, Name = "Изменен файл", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 119, Code = null, Name = "Удален файл", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 120, Code = null, Name = "Исполнение документа", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 130, Code = null, Name = "Подписание документа", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 140, Code = null, Name = "Визирование документа", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 150, Code = null, Name = "Утверждение документа", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 160, Code = null, Name = "Согласование документа", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 200, Code = null, Name = "Направлен для сведения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 205, Code = null, Name = "Передано управление проектом", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 207, Code = null, Name = "Замена должности в документе", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 210, Code = null, Name = "Направлен для исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Исполнение" });
            items.Add(new DictionaryEventTypes { Id = 211, Code = null, Name = "Изменены параметры направлен для исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Исполнение" });
            items.Add(new DictionaryEventTypes { Id = 212, Code = null, Name = "Направлен для контроля", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Контроль" });
            items.Add(new DictionaryEventTypes { Id = 213, Code = null, Name = "Изменены параметры направлен для контроля", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Контроль" });
            items.Add(new DictionaryEventTypes { Id = 214, Code = null, Name = "Направлен для отв.исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Исполнение" });
            items.Add(new DictionaryEventTypes { Id = 215, Code = null, Name = "Изменены параметры направлен для отв.исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Исполнение" });
            items.Add(new DictionaryEventTypes { Id = 220, Code = null, Name = "Направлен для рассмотрения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 221, Code = null, Name = "Рассмотрен положительно", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 222, Code = null, Name = "Рассмотрен отрицательно", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 230, Code = null, Name = "Направлен для сведения внешнему агенту", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 250, Code = null, Name = "Направлен на визирование", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Виза" });
            items.Add(new DictionaryEventTypes { Id = 251, Code = null, Name = "Завизирован", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 252, Code = null, Name = "Отказано в визировании", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 253, Code = null, Name = "Отозван с визирования", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 260, Code = null, Name = "Направлен на согласование", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Согласование" });
            items.Add(new DictionaryEventTypes { Id = 261, Code = null, Name = "Согласован", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 262, Code = null, Name = "Отказано в согласовании", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 263, Code = null, Name = "Отозван с согласования", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 270, Code = null, Name = "Направлен на утверждение", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Утверждение" });
            items.Add(new DictionaryEventTypes { Id = 271, Code = null, Name = "Утвержден", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 272, Code = null, Name = "Отказано в утверждении", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 273, Code = null, Name = "Отозван с утверждения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 280, Code = null, Name = "Направлен на подпись", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Подпись" });
            items.Add(new DictionaryEventTypes { Id = 281, Code = null, Name = "Подписан", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 282, Code = null, Name = "Отказано в подписании", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 283, Code = null, Name = "Отозван с подписания", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 300, Code = null, Name = "Взят на контроль", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Контроль" });
            items.Add(new DictionaryEventTypes { Id = 301, Code = null, Name = "Снят с контроля", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 310, Code = null, Name = "Изменить параметры контроля", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Контроль" });
            items.Add(new DictionaryEventTypes { Id = 315, Code = null, Name = "Изменить параметры контроля для исполнителя", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 320, Code = null, Name = "Поручение выполнено", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Рассмотрение отчета" });
            items.Add(new DictionaryEventTypes { Id = 321, Code = null, Name = "Результат принят", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 322, Code = null, Name = "Результат отклонен", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 330, Code = null, Name = "Контролирую документ", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 340, Code = null, Name = "Являюсь ответственным исполнителем", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 350, Code = null, Name = "Являюсь соисполнителем", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 360, Code = null, Name = "Принято", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 3, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 400, Code = null, Name = "Отменено", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 410, Code = null, Name = "Изменен текст", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 420, Code = null, Name = "Установлен срок исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 430, Code = null, Name = "Изменен срок исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 440, Code = null, Name = "Назначен ответсвенный исполнитель", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 441, Code = null, Name = "Отменено назначение ответсвенным исполнителем", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 450, Code = null, Name = "Очередной срок исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 460, Code = null, Name = "Истекает срок исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 470, Code = null, Name = "Срок исполнения истек", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 500, Code = null, Name = "Направлено сообщение", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 9, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 505, Code = null, Name = "Добавлен бумажный носитель", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 7, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 507, Code = null, Name = "Отметка нахождения бумажного носителя у себя", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 7, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 509, Code = null, Name = "Отметка порчи бумажного носителя", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 7, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 510, Code = null, Name = "Переданы бумажные носители", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 7, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 600, Code = null, Name = "Примечание", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 8, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 601, Code = null, Name = "Формулировка задачи", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 8, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 610, Code = null, Name = "Передан на рассмотрение руководителю", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 3, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 620, Code = null, Name = "Получен после рассмотрения руководителем", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 3, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 700, Code = null, Name = "Направлен на регистрацию", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 701, Code = null, Name = "Зарегистрирован", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 702, Code = null, Name = "Отказано в регистрации", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 712, Code = null, Name = "Отозван проект", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 800, Code = null, Name = "Запущено исполнение плана работы по документу", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 810, Code = null, Name = "Остановлено исполнение плана работы по документу", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 998, Code = null, Name = "Работа возобновлена", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 999, Code = null, Name = "Работа завершена", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });

            return items;
        }

        public static List<DictionaryImportanceEventTypes> GetDictionaryImportanceEventTypes()
        {
            var items = new List<DictionaryImportanceEventTypes>();

            items.Add(new DictionaryImportanceEventTypes { Id = 1, Code = null, Name = "##l@DictionaryImportanceEventTypes:DocumentMoovement@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryImportanceEventTypes { Id = 2, Code = null, Name = "##l@DictionaryImportanceEventTypes:ImportantEvents@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryImportanceEventTypes { Id = 3, Code = null, Name = "##l@DictionaryImportanceEventTypes:AdditionalEvents@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryImportanceEventTypes { Id = 7, Code = null, Name = "##l@DictionaryImportanceEventTypes:PaperMoovement@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryImportanceEventTypes { Id = 8, Code = null, Name = "##l@DictionaryImportanceEventTypes:Message@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryImportanceEventTypes { Id = 9, Code = null, Name = "##l@DictionaryImportanceEventTypes:Internal@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionaryResultTypes> GetDictionaryResultTypes()
        {
            var items = new List<DictionaryResultTypes>();

            items.Add(new DictionaryResultTypes { Id = -4, Name = "Подписание", IsExecute = true, IsActive = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = -3, Name = "Отказ", IsExecute = false, IsActive = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = -2, Name = "Отзыв", IsExecute = false, IsActive = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = -1, Name = "Изменение контроля", IsExecute = false, IsActive = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = 2841, Name = "Отлично", IsExecute = true, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = 2842, Name = "Хорошо", IsExecute = true, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = 2843, Name = "Удовлетворительно", IsExecute = true, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = 2844, Name = "Плохо", IsExecute = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = 4062, Name = "Без оценки", IsExecute = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionarySendTypes> GetDictionarySendTypes()
        {
            var items = new List<DictionarySendTypes>();

            items.Add(new DictionarySendTypes { Id = 10, Code = null, Name = "##l@DictionarySendTypes:SendForResponsibleExecution@l##", IsImportant = true, SubordinationTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 20, Code = null, Name = "##l@DictionarySendTypes:SendForControl@l##", IsImportant = true, SubordinationTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 30, Code = null, Name = "##l@DictionarySendTypes:SendForExecution@l##", IsImportant = true, SubordinationTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 40, Code = null, Name = "##l@DictionarySendTypes:SendForInformation@l##", IsImportant = false, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 45, Code = null, Name = "##l@DictionarySendTypes:SendForInformationExternal@l##", IsImportant = false, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 50, Code = null, Name = "##l@DictionarySendTypes:SendForConsideration@l##", IsImportant = false, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 250, Code = null, Name = "##l@DictionarySendTypes:SendForVisaing@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 260, Code = null, Name = "##l@DictionarySendTypes:SendForАgreement@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 270, Code = null, Name = "##l@DictionarySendTypes:SendForАpproval@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 280, Code = null, Name = "##l@DictionarySendTypes:SendForSigning@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionarySubordinationTypes> GetDictionarySubordinationTypes()
        {
            var items = new List<DictionarySubordinationTypes>();

            items.Add(new DictionarySubordinationTypes { Id = 1, Code = "Informing", Name = "##l@DictionarySubordinationTypes:Informing@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubordinationTypes { Id = 2, Code = "Execution", Name = "##l@DictionarySubordinationTypes:Execution@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionarySubscriptionStates> GetDictionarySubscriptionStates()
        {
            var items = new List<DictionarySubscriptionStates>();

            items.Add(new DictionarySubscriptionStates { Id = 1, Code = "No", Name = "##l@DictionarySubscriptionStates:No@l##", IsSuccess = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubscriptionStates { Id = 2, Code = "Violated", Name = "##l@DictionarySubscriptionStates:Violated@l##", IsSuccess = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubscriptionStates { Id = 11, Code = "Visa", Name = "##l@DictionarySubscriptionStates:Visa@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubscriptionStates { Id = 12, Code = "Аgreement", Name = "##l@DictionarySubscriptionStates:Аgreement@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubscriptionStates { Id = 13, Code = "Аpproval", Name = "##l@DictionarySubscriptionStates:Аpproval@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubscriptionStates { Id = 14, Code = "Sign", Name = "##l@DictionarySubscriptionStates:Sign@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }
        public static List<DictionaryPositionExecutorTypes> GetDictionaryPositionExecutorTypes()
        {
            var items = new List<DictionaryPositionExecutorTypes>();

            items.Add(new DictionaryPositionExecutorTypes { Id = 4, Code = "Personal", Name = "Назначен на должность", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryPositionExecutorTypes { Id = 5, Code = "Referent", Name = "Является референтом", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryPositionExecutorTypes { Id = 6, Code = "IO", Name = "Исполяет обязанности", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionaryLinkTypes> GetDictionaryLinkTypes()
        {
            var items = new List<DictionaryLinkTypes>();

            items.Add(new DictionaryLinkTypes { Id = 100, Name = "В ответ", IsImportant = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryLinkTypes { Id = 110, Name = "Во исполнение", IsImportant = true, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryLinkTypes { Id = 200, Name = "В дополнение", IsImportant = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryLinkTypes { Id = 202, Name = "Повторно", IsImportant = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryLinkTypes { Id = 210, Name = "Во изменение", IsImportant = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryLinkTypes { Id = 220, Name = "В отмену", IsImportant = true, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

    }
}
