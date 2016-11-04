using BL.Model.Enums;
using DMS_WebAPI.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DMS_WebAPI.Models
{
    public static class ApplicationDbImportData
    {

        private static int IdSequence = 0;

        #region [+] Languages ...

        public static List<AdminLanguages> GetAdminLanguages()
        {
            var items = new List<AdminLanguages>();

            items.Add(new AdminLanguages { Id = 570, Code = "ru", Name = "Русский", IsDefault = true});
            items.Add(new AdminLanguages { Id = 045, Code = "en", Name = "English", IsDefault = false });
            items.Add(new AdminLanguages { Id = 720, Code = "uk", Name = "Українська", IsDefault = false });
            items.Add(new AdminLanguages { Id = 090, Code = "be", Name = "Беларуский", IsDefault = false });
            items.Add(new AdminLanguages { Id = 790, Code = "cs", Name = "Čeština", IsDefault = false });
            items.Add(new AdminLanguages { Id = 481, Code = "de", Name = "Deutsch", IsDefault = false });
            items.Add(new AdminLanguages { Id = 745, Code = "fr", Name = "Français", IsDefault = false });
            items.Add(new AdminLanguages { Id = 740, Code = "pl", Name = "Polszczyzna", IsDefault = false });
            
            return items;
        }

        private static void AddALV(List<AdminLanguageValues> list, string label, string EngLang, string RusLang, string UaLang)
        {
            list.Add(new AdminLanguageValues()
            {
                Id = ++IdSequence,
                LanguageId = 570,
                Label = label,
                Value = RusLang
            });
            list.Add(new AdminLanguageValues()
            {
                Id = ++IdSequence,
                LanguageId = 045,
                Label = label,
                Value = EngLang
            });
            list.Add(new AdminLanguageValues()
            {
                Id = ++IdSequence,
                LanguageId = 720,
                Label = label,
                Value = UaLang
            });
        }

        public static List<AdminLanguageValues> GetAdminLanguageValues()
        {
            IdSequence = 0;

            var list = new List<AdminLanguageValues>();

            list.AddRange(GetObjects());
            list.AddRange(GetActions());

            list.AddRange(GetAdminAccessLevels());
            list.AddRange(GetDictionaryEventTypes());

            AddALV(list, "##l@DmsExceptions:IncomingModelIsNotValid@l##", "Incoming Model is not valid! {0}", "Входящая модель недействительна! {0}", "Вхідна модель недійсна! {0}");
            AddALV(list, "##l@DmsExceptions:WrongParameterTypeError@l##", "Parameter type commands is incorrect!", "Тип параметра комманды указан неверно!", "Тип параметра комманди вказано невірно !");
            AddALV(list, "##l@DmsExceptions:WrongParameterValueError@l##", "Parameters commands incorrect!", "Параметры комманды неверные!", "Параметри комманди невірні !");
            AddALV(list, "##l@DmsExceptions:RecordNotUnique@l##", "Record is not Unique", "Запись не уникальна", "Запис не унікальна");

            AddALV(list, "##l@DmsExceptions:UserNameOrPasswordIsIncorrect@l##", "The user name or password is incorrect", "Неверно введен логин или пароль", "Помилка в логіні або паролі");
            AddALV(list, "##l@DmsExceptions:UserUnauthorized@l##", "Authorization has been denied for this request", "Пользователь не авторизован", "Користувач не авторизований");
            AddALV(list, "##l@DmsExceptions:UserAccessIsDenied@l##", "Access is Denied!", "Отказано в доступе!", "Відмовлено в доступі!");
            AddALV(list, "##l@DmsExceptions:UserIsDeactivated@l##", "Employee \"{0}\" is deactivated", "Пользователь \"{0}\" деактивирован", "Користувач \"{0}\" деактивовано");
            AddALV(list, "##l@DmsExceptions:UserNotExecuteAnyPosition@l##", "Employee \"{0}\" does not execute any position", "Сотрудник \"{0}\" не занимает ни одной должности", "Співробітник \"{0}\" не займає жодної посади");
            AddALV(list, "##l@DmsExceptions:UserNameIsNotDefined@l##", "Employee for the current user could not be defined!", "Контекст пользователя еще не сформирован. НЕЛЬЗЯ ДЕРГАТЬ АПИ!", "Контекст користувача ще не сформований. СМИКАТИ АПІ ЗАБОРОНЕНО!");
            AddALV(list, "##l@DmsExceptions:UserPositionIsNotDefined@l##", "Position for the current user could not be defined!", "Контекст пользователя еще не сформирован. НЕЛЬЗЯ ДЕРГАТЬ АПИ!", "Контекст користувача ще не сформований. СМИКАТИ АПІ ЗАБОРОНЕНО!");


            AddALV(list, "##l@System@l##", "System", "Система", "Система");
            AddALV(list, "##l@DictionaryDocumentDirections:Incoming@l##", "Incoming", "Входящий", "Вхідний");
            AddALV(list, "##l@DictionaryDocumentDirections:Internal@l##", "Internal", "Собственный", "Власний");
            AddALV(list, "##l@DictionaryDocumentDirections:Outcoming@l##", "Outcoming", "Иcходящий", "Іcходящій");
            AddALV(list, "##l@DictionaryImportanceEventTypes:AdditionalEvents@l##", "Secondary events", "Второстепенные события", "Другорядні події");
            AddALV(list, "##l@DictionaryImportanceEventTypes:DocumentMoovement@l##", "Facts movement documents", "Факты движения документов", "Факти руху документів");
            AddALV(list, "##l@DictionaryImportanceEventTypes:ImportantEvents@l##", "Important events", "Важные события", "Важливі події");
            AddALV(list, "##l@DictionaryImportanceEventTypes:Internal@l##", "Own notes", "Собственные примечания", "Власні примітки");
            AddALV(list, "##l@DictionaryImportanceEventTypes:Message@l##", "Messages", "Сообщения", "Повідомлення");
            AddALV(list, "##l@DictionaryImportanceEventTypes:PaperMoovement@l##", "Paper movement", "Движение БН", "Рух ПН");
            AddALV(list, "##l@DictionarySendTypes:SendForConsideration@l##", "For consideration", "Для рассмотрения", "Для розгляду");
            AddALV(list, "##l@DictionarySendTypes:SendForControl@l##", "In control", "На контроль(отв.исп.)", "На контроль (отв");
            AddALV(list, "##l@DictionarySendTypes:SendForExecution@l##", "For execution", "Соисполнителю", "Співвиконавцю");
            AddALV(list, "##l@DictionarySendTypes:SendForInformation@l##", "For information", "Для сведения", "Для довідки");
            AddALV(list, "##l@DictionarySendTypes:SendForInformationExternal@l##", "For information external agents", "Для сведения внешнему агенту", "Для відомості зовнішньому агенту");
            AddALV(list, "##l@DictionarySendTypes:SendForResponsibleExecution@l##", "ResponsibleExecution", "Исполненителю(отв.исп.)", "Ісполненітелю (отв");
            AddALV(list, "##l@DictionarySendTypes:SendForSigning@l##", "For signing", "На подпись", "На підпис");
            AddALV(list, "##l@DictionarySendTypes:SendForVisaing@l##", "For visaing", "На визирование", "На візування");
            AddALV(list, "##l@DictionarySendTypes:SendForАgreement@l##", "For agreement", "На согласование", "На узгодження");
            AddALV(list, "##l@DictionarySendTypes:SendForАpproval@l##", "For approval", "На утверждение", "На затвердження");
            AddALV(list, "##l@DictionarySubordinationTypes:Execution@l##", "Execution", "Исполнение", "Виконання");
            AddALV(list, "##l@DictionarySubordinationTypes:Informing@l##", "Informing", "Информирование", "Інформування");
            AddALV(list, "##l@DictionarySubscriptionStates:No@l##", "No", "Нет", "Ні");
            AddALV(list, "##l@DictionarySubscriptionStates:Sign@l##", "Sign", "Подпись", "Підпис");
            AddALV(list, "##l@DictionarySubscriptionStates:Violated@l##", "Violated", "Нарушена", "Порушена");
            AddALV(list, "##l@DictionarySubscriptionStates:Visa@l##", "Visa", "Виза", "Віза");
            AddALV(list, "##l@DictionarySubscriptionStates:Аgreement@l##", "Аgreement", "Согласование", "Узгодження");
            AddALV(list, "##l@DictionarySubscriptionStates:Аpproval@l##", "Аpproval", "Утверждение", "Затвердження");

            AddALV(list, "##l@DmsExceptions:AccessIsDenied@l##", "Access is Denied! Action: {0}", "Отказано в доступе! Действие: {0}", "Відмовлено в доступі! Дія: {0}");
            AddALV(list, "##l@DmsExceptions:CannotAccessToFile@l##", "Cannot access to user file!", "Файл пользователя не доступен!", "Файл користувача не доступний !");
            AddALV(list, "##l@DmsExceptions:CannotSaveFile@l##", "Error when save user file!", "Ошибка при сохранения файла пользователя!", "Помилка при збереження файлу користувача !");
            AddALV(list, "##l@DmsExceptions:ClientIsNotFound@l##", "Client not found", "Клиент не найден", "Клієнт не найден");
            AddALV(list, "##l@DmsExceptions:ClientNameAlreadyExists@l##", "Client Name already exists", "Имя клиента уже существует", "Ім'я клієнта вже існує");
            AddALV(list, "##l@DmsExceptions:ClientCodeAlreadyExists@l##", "Domain \"{0}\" already exists", "Доменное имя \"{0}\" уже занято", "Доменне ім'я {0} вже зайнято");
            AddALV(list, "##l@DmsExceptions:ClientVerificationCodeIncorrect@l##", "Verification code is invalid", "Проверочный код неверен", "Код перевірки невірний");
            AddALV(list, "##l@DmsExceptions:CommandNotDefinedError@l##", "The desired command for \"{0}\" not found", "Команда для \"{0}\" не найдена", "Команда для {0} не знайдено");
            AddALV(list, "##l@DmsExceptions:CouldNotChangeAttributeLaunchPlan@l##", "Couldn\"t change attribute LaunchPlan", "Невозможно изменить атрибут LaunchPlan", "Неможливо змінити атрибут LaunchPlan");
            AddALV(list, "##l@DmsExceptions:CouldNotChangeFavourite@l##", "Couldn\"t change attribute Favourite", "Невозможно изменить атрибут Favourite", "Неможливо змінити атрибут Favourite");
            AddALV(list, "##l@DmsExceptions:CouldNotChangeIsInWork@l##", "Couldn\"t change attribute IsInWork", "Невозможно изменить атрибут IsInWork", "Неможливо змінити атрибут IsInWork");
            AddALV(list, "##l@DmsExceptions:CouldNotPerformThisOperation@l##", "Could Not Perform This Operation!", "Операция не выполнена!", "Операція не виконана !");
            AddALV(list, "##l@DmsExceptions:CryptographicError@l##", "Encryption Error", "Ошибка шифрования", "Помилка шифрування");
            AddALV(list, "##l@DmsExceptions:DatabaseError@l##", "An error occurred while accessing the database!", "Ошибка при обращении к базе данных!", "Помилка при зверненні до бази даних !");
            AddALV(list, "##l@DmsExceptions:DatabaseIsNotFound@l##", "Database not found", "База данных не найдена", "База даних не знайдено");
            AddALV(list, "##l@DmsExceptions:DatabaseIsNotSet@l##", "The database is not set", "База данных не установлена", "База даних не встановлена");

            AddALV(list, "##l@DmsExceptions:AdminRecordNotUnique@l##", "Setting record should be unique!", "Настроечная запись должена быть уникальна!", "Настроювальна запис должена бути унікальна !");
            AddALV(list, "##l@DmsExceptions:AdminRecordCouldNotBeAdded@l##", "You could not add this setting data!", "Вы не можете добавить настроечные данные", "Ви не можете додати налагоджувальні дані");
            AddALV(list, "##l@DmsExceptions:AdminRecordCouldNotBeDeleted@l##", "You could not delete from this dictionary data!", "Вы не можете удалить настроечные данные", "Ви не можете видалити конфігураційні дані");
            AddALV(list, "##l@DmsExceptions:AdminRecordWasNotFound@l##", "Dictionary record was not found!", "Элемент справочника не найден!", "Елемент довідника не найден !");

            AddALV(list, "##l@DmsExceptions:DictionaryAddressTypeCodeNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryAddressTypeNameNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryContactTypeCodeNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryContactTypeNameNotUnique@l##", "", "", "");

            AddALV(list, "##l@DmsExceptions:DictionaryAgentBankMFOCodeNotUnique@l##", "Bank \"{0}\" MFO \"{1}\" сode should be unique!", "Банк \"{0}\" c МФО \"{1}\" уже есть в справочнике", "Банк {0} c МФО {1} вже є в довіднику");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentCompanyOKPOCodeNotUnique@l##", "Company \"{0}\" OKPO сode should be unique!", "Юридическое лицо с указанным ОКПО уже есть в справочнике", "Юридична особа із зазначеним ОКПО вже є в довіднику");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentCompanyTaxCodeNotUnique@l##", "Company \"{0}\" tax сode should be unique!", "Юридическое лицо с указанным ИНН уже есть в справочнике", "Юридична особа із зазначеним ІПН вже є в довіднику");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentCompanyVATCodeNotUnique@l##", "Company \"{0}\" VAT сode should be unique!", "Юридическое лицо с указанным номером свидетельства НДС уже есть в справочнике", "Юридична особа із зазначеним номером свідоцтва ПДВ вже є в довіднику");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentEmployeePassportNotUnique@l##", "Employee \"{0}\" passport should be unique!", "Сотрудник с указанными паспортными данными уже есть в справочнике", "Співробітник з зазначеними паспортними даними вже є в довіднику");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentEmployeePersonnelNumberNotUnique@l##", "Employee \"{0}\" personnel number should be unique!", "Сотрудник с указанным табельным номером уже есть в справочнике", "Співробітник з зазначеним табельною номером вже є в довіднику");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentEmployeeTaxCodeNotUnique@l##", "Employee \"{0}\" tax code should be unique!", "Сотрудник с указанным ИНН уже есть в справочнике", "Співробітник з зазначеним ІПН вже є в довіднику");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentNameNotUnique@l##", "Agent name \"{0}\" should be unique!", "Агент \"{0}\" уже есть в справочнике", "Агент {0} вже є в довіднику");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentPersonPassportNotUnique@l##", "Person \"{0}\" passport should be unique!", "Физлицо с указанными паспортными данными уже есть в справочнике", "Фізособа з зазначеними паспортними даними вже є в довіднику");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentPersonTaxCodeNotUnique@l##", "Person \"{0}\" tax code should be unique!", "Физлицо с указанным ИНН уже есть в справочнике", "Фізособа із зазначеним ІПН вже є в довіднику");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentContactTypeNotUnique@l##", "Agent contact type should be unique!", "Контакт с указанным типом уже есть у этого агента", "Контакт із зазначеним типом вже є у цього агента");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentContactNotUnique@l##", "Agent contact should be unique!", "Указанный контакт уже есть у этого агента", "Зазначений контакт вже є у цього агента");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentAddressNameNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentAddressTypeNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentAccountNumberNotUnique@l##", "", "", "");

            AddALV(list, "##l@DmsExceptions:DictionaryCostomDictionaryNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryCostomDictionaryTypeNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionarysdDepartmentNotBeSubordinated@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryDepartmentNameNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryDocumentSubjectNameNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryDocumentTypeNameNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorIsInvalidPeriod@l##", "", "Период исполнения задан неверно!", "Період виконання заданий невірно!");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorNotUnique@l##", "", "Сотрудник \"{1}\" не может быть назначен на должность повторно \"{0}\" c {2} по {3}", "Співробітник {1} не може бути призначений на посаду повторно {0} c {2} по {3}");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorPersonalNotUnique@l##", "", "На должность \"{0}\" штатно назначен \"{1}\" c {2} по {3}", "На посаду {0} штатно призначений {1} c {2} по {3}");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorIONotUnique@l##", "", "На должность \"{0}\" назначен исполняющий обязанности \"{1}\" c {2} по {3}", "На посаду {0} призначений виконуючий обов'язки {1} c {2} по {3}");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorReferentNotUnique@l##", "", "На должность \"{0}\" назначен референт \"{1}\" c {2} по {3}", "На посаду {0} призначений референт {1} c {2} по {3}");
            AddALV(list, "##l@DmsExceptions:DictionaryRegistrationJournalNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryStandartSendListNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryStandartSendListContentNotUnique@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryTagNotUnique@l##", "", "", "");



            AddALV(list, "##l@DmsExceptions:DictionaryRecordCouldNotBeAdded@l##", "You could not add this dictionary data!", "Вы не можете добавить данные в этот справочник", "Ви не можете додати дані в цей довідник");
            AddALV(list, "##l@DmsExceptions:DictionaryRecordCouldNotBeDeleted@l##", "You could not delete from this dictionary data!", "Вы не можете удалить данные из этого справочника", "Ви не можете видалити дані з цього довідника");
            AddALV(list, "##l@DmsExceptions:DictionaryRecordNotUnique@l##", "Dictionary record should be unique!", "Элемент справочника должен быть уникален!", "Елемент довідника повинен бути унікальний !");
            AddALV(list, "##l@DmsExceptions:DictionaryRecordWasNotFound@l##", "Dictionary record was not found!", "Элемент справочника не найден!", "Елемент довідника не найден !");
            AddALV(list, "##l@DmsExceptions:DictionarySystemRecordCouldNotBeDeleted@l##", "Dictionary system record was not deleted!", "Невозможно удалить предустановленные записи справочника", "Неможливо видалити встановлені записи довідника");
            AddALV(list, "##l@DmsExceptions:DictionaryTagNotFoundOrUserHasNoAccess@l##", "User could not access this tag!", "Пользователь не имеет доступа к этому тегу!", "Користувач не має доступу до цього тегу !");
            AddALV(list, "##l@DmsExceptions:DocumentCannotBeModifiedOrDeleted@l##", "Document cannot be Modified or Deleted!", "Документ не может быть изменен или удален!", "Документ не може бути змінений або видалений !");
            AddALV(list, "##l@DmsExceptions:DocumentCouldNotBeRegistered@l##", "Document registration has non been successfull! Try again!", "Регистрационный документ не была успешной! Попробуй еще раз!", "Реєстраційний документ не була успішною! Спробуй ще раз!");
            AddALV(list, "##l@DmsExceptions:DocumentFileWasChangedExternally@l##", "The document file has been modified from the outside", "Файл документа был изменен извне", "Файл документа був змінений ззовні");
            AddALV(list, "##l@DmsExceptions:DocumentHasAlreadyHasLink@l##", "Document has already has link!", "Документ уже имеет ссылку!", "Документ вже має посилання !");
            AddALV(list, "##l@DmsExceptions:DocumentHasAlredyBeenRegistered@l##", "Document has already been registered!", "Документ уже зарегистрирован!", "Документ уже зареєстрований !");
            AddALV(list, "##l@DmsExceptions:DocumentNotFoundOrUserHasNoAccess@l##", "User could not access this document!", "Документ не доступен!", "Документ не доступний !");
            AddALV(list, "##l@DmsExceptions:DocumentRestrictedSendListDoesNotMatchTheTemplate@l##", "Document Restricted SendList does not match the template", "Разрешающий список рассылок для документа не соответствует шаблону", "Дозволяє список розсилок для документа не відповідає шаблоном");
            AddALV(list, "##l@DmsExceptions:DocumentRestrictedSendListDuplication@l##", "Duplicate Entry DocumentRestrictSendList", "Дублирование записей в разрешающем списке рассылке для документа", "Дублювання записів в дозвільному списку розсилки для документа");
            AddALV(list, "##l@DmsExceptions:DocumentSendListDoesNotMatchTheTemplate@l##", "Document SendList does not match the template", "Список рассылок для документа не соответствует шаблону", "Список розсилок для документа не відповідає шаблоном");
            AddALV(list, "##l@DmsExceptions:DocumentSendListNotFoundInDocumentRestrictedSendList@l##", "DocumentSendList not found in DocumentRestrictedSendList", "Получатель не найден в разрешающем списке рассылок для документа", "Одержувач не найден в дозвільному списку розсилок для документа");
            AddALV(list, "##l@DmsExceptions:EncryptionCertificatePrivateKeyСanNotBeExported@l##", "The private key can not be exported", "Приватный ключ нельзя экспортировать", "Приватний ключ не можна експортувати");
            AddALV(list, "##l@DmsExceptions:EncryptionCertificateWasNotFound@l##", "The certificate was not found", "Сертификат не был найден", "Сертифікат не був знайдений");
            AddALV(list, "##l@DmsExceptions:EventNotFoundOrUserHasNoAccess@l##", "User could not access this event!", "Пользователь не имеет доступа к этому событию!", "Користувач не має доступу до цієї події !");
            AddALV(list, "##l@DmsExceptions:ExecutorAgentForPositionIsNotDefined@l##", "Executor agent for position is not defined!", "Исполнитель для должности не определен!", "Виконавець для посади не визначений !");
            AddALV(list, "##l@DmsExceptions:LicenceExceededNumberOfConnectedUsers@l##", "You have exceeded the allowed number of connected users", "Превышено разрешенное количество подключенных пользователей", "Перевищено дозволену кількість підключених користувачів");
            AddALV(list, "##l@DmsExceptions:LicenceExceededNumberOfRegisteredUsers@l##", "You have exceeded the allowed number of registered users", "Превышено разрешенное количество зарегистрированных пользователей", "Перевищено дозволену кількість зареєстрованих користувачів");
            AddALV(list, "##l@DmsExceptions:LicenceExpired@l##", "Licence expired", "Срок лицензии истек", "Термін ліцензії закінчився");
            AddALV(list, "##l@DmsExceptions:LicenceInformationError@l##", "The licence is not valid", "Лицензия недействительна", "Ліцензія недійсна");
            AddALV(list, "##l@DmsExceptions:NeedInformationAboutCorrespondent@l##", "Need information about correspondent!", "Нужна информация о корреспонденте!", "Потрібна інформація про кореспондента !");
            AddALV(list, "##l@DmsExceptions:NotFilledWithAdditionalRequiredAttributes@l##", "Not filled with additional required attributes!", "Не заполнены обязательные дополнительные атрибуты!", "Чи не заповнені обов'язкові додаткові атрибути !");
            AddALV(list, "##l@DmsExceptions:PaperListNotFoundOrUserHasNoAccess@l##", "Paper list not found or user has no access", "Список бумага не найдена или пользователь не имеет доступа", "Список папір не знайдена або користувач не має доступу");
            AddALV(list, "##l@DmsExceptions:PaperNotFoundOrUserHasNoAccess@l##", "Paper not found or user has no access", "Бумага не найдена или пользователь не имеет доступа", "Папір не знайдено або користувач не має доступу");
            AddALV(list, "##l@DmsExceptions:PlanPointHasAlredyBeenLaunched@l##", "Plan Point has already been Launched!", "Пункт плана уже запущен!", "Пункт плану вже запущений !");
            AddALV(list, "##l@DmsExceptions:TaskNotFoundOrUserHasNoAccess@l##", "Task not found", "Task не найден", "Task не найден");
            AddALV(list, "##l@DmsExceptions:TemplateDocumentIsNotValid@l##", "The document template is not valid", "Шаблон документа не корректен", "Шаблон документу не коректний");
            AddALV(list, "##l@DmsExceptions:TemplateDocumentNotFoundOrUserHasNoAccess@l##", "User could not access this template document!", "Пользователь не имеет доступ к этот шаблону документа!", "Користувач не має доступ до цей шаблоном документа !");
            AddALV(list, "##l@DmsExceptions:UnknownDocumentFile@l##", "Could not find appropriate document file!", "Не удалось найти соответствующий файл документа!", "Не вдалося знайти відповідний файл документа !");
            AddALV(list, "##l@DmsExceptions:UserFileNotExists@l##", "User file does not exists on Filestore!", "Пользовательский файл не существует в файловом хранилище", "Призначений для користувача файл не існує в файловому сховищі");
            AddALV(list, "##l@DmsExceptions:UserHasNoAccessToDocument@l##", "User could not access this document!", "Пользователь не может получить доступ к этот документ!", "Користувач не може отримати доступ до цей документ !");
            AddALV(list, "##l@DmsExceptions:UserNameAlreadyExists@l##", "User Name already exists", "Имя пользователя уже существует", "Ім'я користувача вже існує");
            AddALV(list, "##l@DmsExceptions:WaitHasAlreadyClosed@l##", "Wait has already closed!", "Ожидание уже закрыто!", "Очікування вже закрито !");
            AddALV(list, "##l@DmsExceptions:WaitNotFoundOrUserHasNoAccess@l##", "User could not access this wait!", "Пользователь не имеет доступа к этим ожиданиям!", "Користувач не має доступу до цих очікувань !");
            AddALV(list, "##l@DmsExceptions:WrongDocumentSendListEntry@l##", "Plan item is wrong.", "Некорректный пункт плана", "Некоректний пункт плану");



            //pss 23.09.2016 Выявил DmsExceptions которые не имели перевода 
            //TODO Требуется локализация (перевод ошибок)
            AddALV(list, "##l@DmsExceptions:ControlerHasAlreadyBeenDefined@l##", "Controler Has Already Been Defined", "Контролер уже определен", "Контролер вже визначено");
            AddALV(list, "##l@DmsExceptions:CouldNotModifyTemplateDocument@l##", "", "", "");
            AddALV(list, "##l@DmsExceptions:CouldNotPerformOperationWithPaper@l##", "Could Not Perform Operation With Paper", "Невозможно осуществить операцию с бумажными носителями", "Неможливо здійснити операцію з паперовими носіями");
            AddALV(list, "##l@DmsExceptions:EncryptionCertificateHasExpired@l##", "Encryption Certificate Has Been Expired", "Сертификат просрочен", "Сертифікат прострочений");
            AddALV(list, "##l@DmsExceptions:NobodyIsChosen@l##", "NobodyIsChosen", "Никто не выбран", "Ніхто не обраний");
            AddALV(list, "##l@DmsExceptions:ResponsibleExecutorHasAlreadyBeenDefined@l##", "Responsible Executor Has Already Been Defined", "Ответственный исполнитель уже определен", "Відповідальний виконавець вже визначено");
            AddALV(list, "##l@DmsExceptions:ResponsibleExecutorIsNotDefined@l##", "Responsible Executor Is Not Defined", "Ответственный исполнитель не определен", "Відповідальний виконавець не визначений");
            AddALV(list, "##l@DmsExceptions:SigningTypeNotAllowed@l##", "Signing Type Is Not Allowed", "Недопустимый тип подписи", "Неприпустимий тип підпису");
            AddALV(list, "##l@DmsExceptions:SubordinationHasBeenViolated@l##", "Subordination Has Been Violated", "Нарушена субординация", "Порушена субординація");
            AddALV(list, "##l@DmsExceptions:TargetIsNotDefined@l##", "Target Is Not Defined", "Получатель не определен", "Одержувач не визначений");
            AddALV(list, "##l@DmsExceptions:TaskIsNotDefined@l##", "Task Is Not Defined", "Задача не определена", "Завдання не визначена");
            AddALV(list, "##l@DmsExceptions:ContriolHasNotBeenChanged@l##", "Contriol Has Not Been Changed", "Параметры контроля не изменены", "Параметри контролю не змінені");


            // после добавления переводов можно обновить их в базе api/v2/Languages/RefreshLanguageValues

            return list;
        }
        public static List<AdminLanguageValues> GetObjects()
        {
            var list = new List<AdminLanguageValues>();

            AddALV(list, "##l@Objects:Documents@l##", "Documents", "Документы", "Документи");
            AddALV(list, "##l@Objects:DocumentAccesses@l##", "Document accesses", "Документы - доступы", "Документи - доступи");
            AddALV(list, "##l@Objects:DocumentRestrictedSendLists@l##", "Document restricted send lists", "Документы - ограничения рассылки", "Документи - обмеження розсилки");
            AddALV(list, "##l@Objects:DocumentSendLists@l##", "Document send lists", "Документы - план работы", "Документи - план роботи");
            AddALV(list, "##l@Objects:DocumentFiles@l##", "Document files", "Документы - файлы", "Документи - файли");
            AddALV(list, "##l@Objects:DocumentLinks@l##", "Document links", "Документы - связи", "Документи - зв'язку");
            AddALV(list, "##l@Objects:DocumentSendListStages@l##", "Document send list stages", "Документы - этапы плана работ", "Документи - етапи плану робіт");
            AddALV(list, "##l@Objects:DocumentEvents@l##", "Document events", "Документы - события", "Документи - події");
            AddALV(list, "##l@Objects:DocumentWaits@l##", "Document waits", "Документы - ожидания", "Документи - очікування");
            AddALV(list, "##l@Objects:DocumentSubscriptions@l##", "Document subscriptions", "Документы - подписи", "Документи - підписи");
            AddALV(list, "##l@Objects:DocumentTasks@l##", "Document tasks", "Документы - задачи", "Документи - завдання");
            AddALV(list, "##l@Objects:DocumentPapers@l##", "Document papers", "Документы - бумажные носители", "Документи - паперові носії");
            AddALV(list, "##l@Objects:DocumentPaperEvents@l##", "Document paper events", "Документы - события по бумажным носителям", "Документи - події по паперових носіїв");
            AddALV(list, "##l@Objects:DocumentPaperLists@l##", "Document paper lists", "Документы - реестры передачи бумажных носителей", "Документи - реєстри передачі паперових носіїв");
            AddALV(list, "##l@Objects:DocumentSavedFilters@l##", "Document saved filters", "Документы - сохраненные фильтры", "Документи - збережені фільтри");
            AddALV(list, "##l@Objects:DocumentTags@l##", "Document tags", "Документы - тэги", "Документи - теги");
            AddALV(list, "##l@Objects:DictionaryDocumentType@l##", "Dictionary document type", "Типы документов", "Типи документів");
            AddALV(list, "##l@Objects:DictionaryAddressType@l##", "Dictionary address type", "Типы адресов", "Типи адрес");
            AddALV(list, "##l@Objects:DictionaryDocumentSubjects@l##", "Dictionary document subjects", "Тематики документов", "Тематики документів");
            AddALV(list, "##l@Objects:DictionaryRegistrationJournals@l##", "Dictionary registration journals", "Журналы регистрации", "Журнали реєстрації");
            AddALV(list, "##l@Objects:DictionaryContactType@l##", "Dictionary contact type", "Типы контактов", "Типи контактів");
            AddALV(list, "##l@Objects:DictionaryAgents@l##", "Dictionary agents", "Контрагенты", "Контрагенти");
            AddALV(list, "##l@Objects:DictionaryContacts@l##", "Dictionary contacts", "Контакты", "Контакти");
            AddALV(list, "##l@Objects:DictionaryAgentAddresses@l##", "Dictionary agent addresses", "Адреса", "Адреси");
            AddALV(list, "##l@Objects:DictionaryAgentPersons@l##", "Dictionary agent persons", "Физические лица", "Фізичні особи");
            AddALV(list, "##l@Objects:DictionaryDepartments@l##", "Dictionary departments", "Структура предприятия", "Структура підприємства");
            AddALV(list, "##l@Objects:DictionaryPositions@l##", "Dictionary positions", "Штатное расписание", "Штатний розклад");
            AddALV(list, "##l@Objects:DictionaryAgentEmployees@l##", "Dictionary agent employees", "Сотрудники", "Співробітники");
            AddALV(list, "##l@Objects:DictionaryAgentCompanies@l##", "Dictionary agent companies", "Юридические лица", "Юридичні особи");
            AddALV(list, "##l@Objects:DictionaryAgentBanks@l##", "Dictionary agent banks", "Контрагенты - банки", "Контрагенти - банки");
            AddALV(list, "##l@Objects:DictionaryAgentAccounts@l##", "Dictionary agent accounts", "Расчетные счета", "Розрахункові рахунки");
            AddALV(list, "##l@Objects:DictionaryStandartSendListContent@l##", "Dictionary standart send list content", "Типовые списки рассылки (содержание)", "Типові списки розсилки (зміст)");
            AddALV(list, "##l@Objects:DictionaryStandartSendLists@l##", "Dictionary standart send lists", "Типовые списки рассылки", "Типові списки розсилки");
            AddALV(list, "##l@Objects:DictionaryAgentClientCompanies@l##", "Dictionary agent client companies", "Компании", "Компанії");
            AddALV(list, "##l@Objects:DictionaryPositionExecutorTypes@l##", "Dictionary position executor types", "Типы исполнителей", "Типи виконавців");
            AddALV(list, "##l@Objects:DictionaryPositionExecutors@l##", "Dictionary position executors", "Исполнители должности", "Виконавці посади");
            AddALV(list, "##l@Objects:TemplateDocument@l##", "Template document", "Шаблоны документов", "Шаблони документів");
            AddALV(list, "##l@Objects:TemplateDocumentSendList@l##", "Template document send list", "Списки рассылки в шаблонах", "Списки розсилки в шаблонах");
            AddALV(list, "##l@Objects:TemplateDocumentRestrictedSendList@l##", "Template document restricted send list", "Ограничительные списки рассылки в шаблонах", "Обмежувальні списки розсилки в шаблонах");
            AddALV(list, "##l@Objects:TemplateDocumentTask@l##", "Template document task", "Задачи в шаблонах", "Завдання в шаблонах");
            AddALV(list, "##l@Objects:TemplateDocumentAttachedFiles@l##", "Template document attached files", "Прикрепленные к шаблонам файлы", "Прикріплені до шаблонів файли");
            AddALV(list, "##l@Objects:DictionaryTag@l##", "Dictionary tag", "Теги", "Теги");
            AddALV(list, "##l@Objects:CustomDictionaryTypes@l##", "Custom dictionary types", "Типы пользовательских словарей", "Типи словників");
            AddALV(list, "##l@Objects:CustomDictionaries@l##", "Custom dictionaries", "Пользовательские словари", "Словники");
            AddALV(list, "##l@Objects:Properties@l##", "Properties", "Динамические аттрибуты", "Динамічні атрибути");
            AddALV(list, "##l@Objects:PropertyLinks@l##", "Property links", "Связи динамических аттрибутов с объектами системы", "Зв'язки динамічних атрибутів з об'єктами системи");
            AddALV(list, "##l@Objects:PropertyValues@l##", "Property values", "Значения динамических аттрибутов", "Значення динамічних атрибутів");

            // Спасибо за то, что добавил перевод! Удачных идей и быстрого кода.

            return list;
        }

        public static List<AdminLanguageValues> GetActions()
        {
            var list = new List<AdminLanguageValues>();

            AddALV(list, "##l@DocumentActions:AddDocument@l##", "Add document", "Создать документ по шаблону", "Створити документ за шаблоном");
            AddALV(list, "##l@DocumentActions:CopyDocument@l##", "Copy document", "Создать документ копированием", "Створити документ копіюванням");
            AddALV(list, "##l@DocumentActions:ModifyDocument@l##", "Modify document", "Изменить документ", "Змінити документ");
            AddALV(list, "##l@DocumentActions:DeleteDocument@l##", "Delete document", "Удалить проект", "Видалити проект");
            AddALV(list, "##l@DocumentActions:LaunchPlan@l##", "Launch plan", "Запустить выполнение плана", "Запустити виконання плану");
            AddALV(list, "##l@DocumentActions:AddDocumentSendListItem@l##", "Add document send list item", "Добавить пункт плана", "Додати пункт плану");
            AddALV(list, "##l@DocumentActions:StopPlan@l##", "Stop plan", "Остановить выполнение плана", "Зупинити виконання плану");
            AddALV(list, "##l@DocumentActions:ChangeExecutor@l##", "Change executor", "Передать управление", "Передати управління");
            AddALV(list, "##l@DocumentActions:RegisterDocument@l##", "Register document", "Зарегистрировать проект", "Зареєструвати проект");
            AddALV(list, "##l@DocumentActions:MarkDocumentEventAsRead@l##", "Mark document event as read", "Отметить прочтение событий по документу", "Відзначити прочитання подій по документу");
            AddALV(list, "##l@DocumentActions:SendForInformation@l##", "Send for information", "Направить для сведения", "Направити для відомості");
            AddALV(list, "##l@DocumentActions:SendForConsideration@l##", "Send for consideration", "Направить для рассмотрения", "Направити для розгляду");
            AddALV(list, "##l@DocumentActions:SendForInformationExternal@l##", "Send for information external", "Направить для сведения внешнему агенту", "Направити для відомості зовнішньому агенту");
            AddALV(list, "##l@DocumentActions:SendForControl@l##", "Send for control", "Направить для контроля", "Направити для контролю");
            AddALV(list, "##l@DocumentActions:SendForResponsibleExecution@l##", "Send for responsible execution", "Направить для отв.исполнения", "Направити для отв");
            AddALV(list, "##l@DocumentActions:SendForExecution@l##", "Send for execution", "Направить для исполнения", "Направити для виконання");
            AddALV(list, "##l@DocumentActions:SendForVisaing@l##", "Send for visaing", "Направить для визирования", "Направити для візування");
            AddALV(list, "##l@DocumentActions:SendForАgreement@l##", "Send for аgreement", "Направить для согласование", "Направити для узгодження");
            AddALV(list, "##l@DocumentActions:SendForАpproval@l##", "Send for аpproval", "Направить для утверждения", "Направити для затвердження");
            AddALV(list, "##l@DocumentActions:SendForSigning@l##", "Send for signing", "Направить для подписи", "Направити для підпису");
            AddALV(list, "##l@DocumentActions:ReportRegistrationCardDocument@l##", "Report registration card document", "Регистрационная карточка", "Реєстраційна картка");
            AddALV(list, "##l@DocumentActions:AddFavourite@l##", "Add favourite", "Добавить в избранное", "Додати в обране");
            AddALV(list, "##l@DocumentActions:DeleteFavourite@l##", "Delete favourite", "Удалить из избранного", "Зняти позначку вибраного");
            AddALV(list, "##l@DocumentActions:FinishWork@l##", "Finish work", "Закончить работу с документом", "Закінчити роботу з документом");
            AddALV(list, "##l@DocumentActions:StartWork@l##", "Start work", "Возобновить работу с документом", "Відновити роботу з документом");
            AddALV(list, "##l@DocumentActions:ChangePosition@l##", "Change position", "Поменять должность в документе", "Поміняти посаду в документі");
            AddALV(list, "##l@DocumentActions:AddDocumentRestrictedSendList@l##", "Add document restricted send list", "Добавить ограничение рассылки", "Додати обмеження розсилки");
            AddALV(list, "##l@DocumentActions:AddByStandartSendListDocumentRestrictedSendList@l##", "Add by standart send list document restricted send list", "Добавить ограничения рассылки по стандартному списку", "Додати обмеження розсилки по стандартному списку");
            AddALV(list, "##l@DocumentActions:DeleteDocumentRestrictedSendList@l##", "Delete document restricted send list", "Удалить ограничение рассылки", "Видалити обмеження розсилки");
            AddALV(list, "##l@DocumentActions:ModifyDocumentSendList@l##", "Modify document send list", "Изменить пункт плана", "Змінити пункт плану");
            AddALV(list, "##l@DocumentActions:DeleteDocumentSendList@l##", "Delete document send list", "Удалить пункт плана", "Видалити пункт плану");
            AddALV(list, "##l@DocumentActions:LaunchDocumentSendListItem@l##", "Launch document send list item", "Запустить пункт плана на исполнение", "Запустити пункт плану на виконання");
            AddALV(list, "##l@DocumentActions:AddDocumentFile@l##", "Add document file", "Добавить файл", "Додати файл");
            AddALV(list, "##l@DocumentActions:ModifyDocumentFile@l##", "Modify document file", "Изменить файл", "Змінити файл");
            AddALV(list, "##l@DocumentActions:DeleteDocumentFile@l##", "Delete document file", "Удалить файл", "Видалити файл");
            AddALV(list, "##l@DocumentActions:AddDocumentFileUseMainNameFile@l##", "Add document file use main name file", "Добавить версию файла к файлу", "Додати версію файлу до файлу");
            AddALV(list, "##l@DocumentActions:AcceptDocumentFile@l##", "Accept document file", "Файл принят", "Файл прийнятий");
            AddALV(list, "##l@DocumentActions:RejectDocumentFile@l##", "Reject document file", "Файл не принят", "Файл не прийнятий");
            AddALV(list, "##l@DocumentActions:RenameDocumentFile@l##", "Rename document file", "Переименовать файл", "Перейменувати файл");
            AddALV(list, "##l@DocumentActions:DeleteDocumentFileVersion@l##", "Delete document file version", "Удалить версию файл", "Видалити версію файл");
            AddALV(list, "##l@DocumentActions:DeleteDocumentFileVersionRecord@l##", "Delete document file version record", "Удалить запись о версим файла", "Видалити запис про версію файлу");
            AddALV(list, "##l@DocumentActions:AcceptMainVersionDocumentFile@l##", "Accept main version document file", "Сделать основной версией", "Зробити основною версією");
            AddALV(list, "##l@DocumentActions:AddDocumentLink@l##", "Add document link", "Добавить связь между документами", "Додати зв'язок між документами");
            AddALV(list, "##l@DocumentActions:DeleteDocumentLink@l##", "Delete document link", "Удалить связь между документами", "Видалити зв'язок між документами");
            AddALV(list, "##l@DocumentActions:AddDocumentSendList@l##", "Add document send list", "Добавить пункт плана", "Додати пункт плану");
            AddALV(list, "##l@DocumentActions:AddByStandartSendListDocumentSendList@l##", "Add by standart send list document send list", "Добавить план работы по стандартному списку", "Додати план роботи по стандартному списку");
            AddALV(list, "##l@DocumentActions:AddDocumentSendListStage@l##", "Add document send list stage", "Добавить этап плана", "Додати етап плану");
            AddALV(list, "##l@DocumentActions:DeleteDocumentSendListStage@l##", "Delete document send list stage", "Удалить этап плана", "Видалити етап плану");
            AddALV(list, "##l@DocumentActions:SendMessage@l##", "Send message", "Направить сообщение участникам рабочей группы", "Направити повідомлення учасникам робочої групи");
            AddALV(list, "##l@DocumentActions:AddNote@l##", "Add note", "Добавить примечание", "Додати примітку");
            AddALV(list, "##l@DocumentActions:ControlOn@l##", "Control on", "Взять на контроль", "Взяти на контроль");
            AddALV(list, "##l@DocumentActions:ControlChange@l##", "Control change", "Изменить параметры контроля", "Змінити параметри контролю");
            AddALV(list, "##l@DocumentActions:SendForExecutionChange@l##", "Send for execution change", "Изменить параметры направлен для исполнения", "Змінити параметри спрямований для виконання");
            AddALV(list, "##l@DocumentActions:SendForResponsibleExecutionChange@l##", "Send for responsible execution change", "Изменить параметры направлен для отв.исполнения", "Змінити параметри спрямований для отв");
            AddALV(list, "##l@DocumentActions:ControlTargetChange@l##", "Control target change", "Изменить параметры контроля для исполнителя", "Змінити параметри контролю для виконавця");
            AddALV(list, "##l@DocumentActions:ControlOff@l##", "Control off", "Снять с контроля", "Зняти з контролю");
            AddALV(list, "##l@DocumentActions:MarkExecution@l##", "Mark execution", "Отметить исполнение", "Відзначити виконання");
            AddALV(list, "##l@DocumentActions:AcceptResult@l##", "Accept result", "Принять результат", "Прийняти результат");
            AddALV(list, "##l@DocumentActions:RejectResult@l##", "Reject result", "Отклонить результат", "Відхилити результат");
            AddALV(list, "##l@DocumentActions:WithdrawVisaing@l##", "Withdraw visaing", "Отозвать с визирования", "Відкликати з візування");
            AddALV(list, "##l@DocumentActions:WithdrawАgreement@l##", "Withdraw аgreement", "Отозвать с согласования", "Відкликати з узгодження");
            AddALV(list, "##l@DocumentActions:WithdrawАpproval@l##", "Withdraw аpproval", "Отозвать с утверждения", "Відкликати з утвердження");
            AddALV(list, "##l@DocumentActions:WithdrawSigning@l##", "Withdraw signing", "Отозвать с подписи", "Відкликати з підпису");
            AddALV(list, "##l@DocumentActions:AffixVisaing@l##", "Affix visaing", "Завизировать", "Завізувати");
            AddALV(list, "##l@DocumentActions:AffixАgreement@l##", "Affix аgreement", "Согласовать", "Узгодити");
            AddALV(list, "##l@DocumentActions:AffixАpproval@l##", "Affix аpproval", "Утвердить", "Затвердити");
            AddALV(list, "##l@DocumentActions:AffixSigning@l##", "Affix signing", "Подписать", "Підписати");
            AddALV(list, "##l@DocumentActions:SelfAffixSigning@l##", "Self affix signing", "Самоподписание", "Самоподпісаніе");
            AddALV(list, "##l@DocumentActions:RejectVisaing@l##", "Reject visaing", "Отказать в визирования", "Відмовити в візування");
            AddALV(list, "##l@DocumentActions:RejectАgreement@l##", "Reject аgreement", "Отказать в согласование", "Відмовити в узгодження");
            AddALV(list, "##l@DocumentActions:RejectАpproval@l##", "Reject аpproval", "Отказать в утверждения", "Відмовити в утвердження");
            AddALV(list, "##l@DocumentActions:RejectSigning@l##", "Reject signing", "Отказать в подписи", "Відмовити в підпису");
            AddALV(list, "##l@DocumentActions:AddDocumentTask@l##", "Add document task", "Добавить задачу", "Додати завдання");
            AddALV(list, "##l@DocumentActions:ModifyDocumentTask@l##", "Modify document task", "Изменить задачу", "Змінити завдання");
            AddALV(list, "##l@DocumentActions:DeleteDocumentTask@l##", "Delete document task", "Удалить задачу", "Видалити завдання");
            AddALV(list, "##l@DocumentActions:AddDocumentPaper@l##", "Add document paper", "Добавить бумажный носитель", "Додати паперовий носій");
            AddALV(list, "##l@DocumentActions:ModifyDocumentPaper@l##", "Modify document paper", "Изменить бумажный носитель", "Змінити паперовий носій");
            AddALV(list, "##l@DocumentActions:MarkOwnerDocumentPaper@l##", "Mark owner document paper", "Отметить нахождение бумажного носителя у себя", "Відзначити знаходження паперового носія у себе");
            AddALV(list, "##l@DocumentActions:MarkСorruptionDocumentPaper@l##", "Mark сorruption document paper", "Отметить порчу бумажного носителя", "Відзначити псування паперового носія");
            AddALV(list, "##l@DocumentActions:DeleteDocumentPaper@l##", "Delete document paper", "Удалить бумажный носитель", "Видалити паперовий носій");
            AddALV(list, "##l@DocumentActions:PlanDocumentPaperEvent@l##", "Plan document paper event", "Планировать движение бумажного носителя", "Планувати рух паперового носія");
            AddALV(list, "##l@DocumentActions:CancelPlanDocumentPaperEvent@l##", "Cancel plan document paper event", "Отменить планирование движения бумажного носителя", "Скасувати планування руху паперового носія");
            AddALV(list, "##l@DocumentActions:SendDocumentPaperEvent@l##", "Send document paper event", "Отметить передачу бумажного носителя", "Відзначити передачу паперового носія");
            AddALV(list, "##l@DocumentActions:CancelSendDocumentPaperEvent@l##", "Cancel send document paper event", "Отменить передачу бумажного носителя", "Скасувати передачу паперового носія");
            AddALV(list, "##l@DocumentActions:RecieveDocumentPaperEvent@l##", "Recieve document paper event", "Отметить прием бумажного носителя", "Відзначити прийом паперового носія");
            AddALV(list, "##l@DocumentActions:AddDocumentPaperList@l##", "Add document paper list", "Добавить реестр бумажных носителей", "Додати реєстр паперових носіїв");
            AddALV(list, "##l@DocumentActions:ModifyDocumentPaperList@l##", "Modify document paper list", "Изменить реестр бумажных носителей", "Змінити реєстр паперових носіїв");
            AddALV(list, "##l@DocumentActions:DeleteDocumentPaperList@l##", "Delete document paper list", "Удалить реестр бумажных носителей", "Видалити реєстр паперових носіїв");
            AddALV(list, "##l@DocumentActions:AddSavedFilter@l##", "Add saved filter", "Добавить сохраненный фильтр", "Додати збережений фільтр");
            AddALV(list, "##l@DocumentActions:ModifySavedFilter@l##", "Modify saved filter", "Изменить сохраненный фильтр", "Змінити збережений фільтр");
            AddALV(list, "##l@DocumentActions:DeleteSavedFilter@l##", "Delete saved filter", "Удалить сохраненный фильтр", "Видалити збережений фільтр");
            AddALV(list, "##l@DocumentActions:ModifyDocumentTags@l##", "Modify document tags", "Изменить тэги по документу", "Змінити теги по документу");
            AddALV(list, "##l@DocumentActions:AddTemplateDocument@l##", "Add template document", "Добавить шаблон документа", "Додати шаблон документа");
            AddALV(list, "##l@DocumentActions:ModifyTemplateDocument@l##", "Modify template document", "Изменить шаблон документа", "Змінити шаблон документа");
            AddALV(list, "##l@DocumentActions:DeleteTemplateDocument@l##", "Delete template document", "Удалить шаблон документа", "Видалити шаблон документа");
            AddALV(list, "##l@DocumentActions:AddTemplateDocumentSendList@l##", "Add template document send list", "Добавить список рассылки в шаблон", "Додати список розсилки в шаблон");
            AddALV(list, "##l@DocumentActions:ModifyTemplateDocumentSendList@l##", "Modify template document send list", "Изменить список рассылки в шаблоне", "Змінити список розсилки в шаблоні");
            AddALV(list, "##l@DocumentActions:DeleteTemplateDocumentSendList@l##", "Delete template document send list", "Удалить список рассылки в шаблоне", "Видалити список розсилки в шаблоні");
            AddALV(list, "##l@DocumentActions:AddTemplateDocumentRestrictedSendList@l##", "Add template document restricted send list", "Добавить ограничительный список рассылки в шаблон", "Додати обмежувальний список розсилки в шаблон");
            AddALV(list, "##l@DocumentActions:ModifyTemplateDocumentRestrictedSendList@l##", "Modify template document restricted send list", "Изменить ограничительный список рассылки в шаблоне", "Змінити обмежувальний список розсилки в шаблоні");
            AddALV(list, "##l@DocumentActions:DeleteTemplateDocumentRestrictedSendList@l##", "Delete template document restricted send list", "Удалить ограничительный список рассылки в шаблоне", "Видалити обмежувальний список розсилки в шаблоні");
            AddALV(list, "##l@DocumentActions:AddTemplateDocumentTask@l##", "Add template document task", "Добавить задачу в шаблон", "Додати завдання в шаблон");
            AddALV(list, "##l@DocumentActions:ModifyTemplateDocumentTask@l##", "Modify template document task", "Изменить задачу в шаблоне", "Змінити завдання в шаблоні");
            AddALV(list, "##l@DocumentActions:DeleteTemplateDocumentTask@l##", "Delete template document task", "Удалить задачу в шаблоне", "Видалити завдання в шаблоні");
            AddALV(list, "##l@DocumentActions:AddTemplateAttachedFile@l##", "Add template attached file", "Добавить файл в шаблон", "Додати файл в шаблон");
            AddALV(list, "##l@DocumentActions:ModifyTemplateAttachedFile@l##", "Modify template attached file", "Изменить файл в шаблоне", "Змінити файл в шаблоні");
            AddALV(list, "##l@DocumentActions:DeleteTemplateAttachedFile@l##", "Delete template attached file", "Удалить файл в шаблоне", "Видалити файл в шаблоні");
            AddALV(list, "##l@DictionaryActions:AddDocumentType@l##", "Add document type", "Добавить тип документа", "Додати тип документа");
            AddALV(list, "##l@DictionaryActions:ModifyDocumentType@l##", "Modify document type", "Изменить тип документа", "Змінити тип документа");
            AddALV(list, "##l@DictionaryActions:DeleteDocumentType@l##", "Delete document type", "Удалить тип документа", "Видалити тип документа");
            AddALV(list, "##l@DictionaryActions:AddAddressType@l##", "Add address type", "Добавить тип адреса", "Додати тип адреси");
            AddALV(list, "##l@DictionaryActions:ModifyAddressType@l##", "Modify address type", "Изменить тип адреса", "Змінити тип адреси");
            AddALV(list, "##l@DictionaryActions:DeleteAddressType@l##", "Delete address type", "Удалить тип адреса", "Видалити тип адреси");
            AddALV(list, "##l@DictionaryActions:AddDocumentSubject@l##", "Add document subject", "Добавить тематику", "Додати тематику");
            AddALV(list, "##l@DictionaryActions:ModifyDocumentSubject@l##", "Modify document subject", "Изменить тематику", "Змінити тематику");
            AddALV(list, "##l@DictionaryActions:DeleteDocumentSubject@l##", "Delete document subject", "Удалить тематику", "Видалити тематику");
            AddALV(list, "##l@DictionaryActions:AddRegistrationJournal@l##", "Add registration journal", "Добавить журнал регистрации", "Додати журнал реєстрації");
            AddALV(list, "##l@DictionaryActions:ModifyRegistrationJournal@l##", "Modify registration journal", "Изменить журнал регистрации", "Змінити журнал реєстрації");
            AddALV(list, "##l@DictionaryActions:DeleteRegistrationJournal@l##", "Delete registration journal", "Удалить журнал регистрации", "Видалити журнал реєстрації");
            AddALV(list, "##l@DictionaryActions:AddContactType@l##", "Add contact type", "Добавить тип контакта", "Додати тип контакту");
            AddALV(list, "##l@DictionaryActions:ModifyContactType@l##", "Modify contact type", "Изменить тип контакта", "Змінити тип контакту");
            AddALV(list, "##l@DictionaryActions:DeleteContactType@l##", "Delete contact type", "Удалить тип контакта", "Видалити тип контакту");
            AddALV(list, "##l@DictionaryActions:AddAgent@l##", "Add agent", "Добавить контрагента", "Додати контрагента");
            AddALV(list, "##l@DictionaryActions:ModifyAgent@l##", "Modify agent", "Изменить контрагента", "Змінити контрагента");
            AddALV(list, "##l@DictionaryActions:DeleteAgent@l##", "Delete agent", "Удалить контрагента", "Видалити контрагента");
            AddALV(list, "##l@DictionaryActions:SetAgentPicture@l##", "Set agent picture", "Установить картинку контрагента", "Змінити аватар контрагента");
            AddALV(list, "##l@DictionaryActions:DeleteAgentPicture@l##", "Delete agent picture", "Удалить картинку контрагента", "Видалити аватар контрагента");
            AddALV(list, "##l@DictionaryActions:AddAgentContact@l##", "Add agent contact", "Добавить контакт", "Додати контакт");
            AddALV(list, "##l@DictionaryActions:ModifyAgentContact@l##", "Modify agent contact", "Изменить контакт", "Змінити контакт");
            AddALV(list, "##l@DictionaryActions:DeleteAgentContact@l##", "Delete agent contact", "Удалить контакт", "Видалити контакт");
            AddALV(list, "##l@DictionaryActions:AddAgentAddress@l##", "Add agent address", "Добавить адрес", "Додати адресу");
            AddALV(list, "##l@DictionaryActions:ModifyAgentAddress@l##", "Modify agent address", "Изменить адрес", "Змінити адресу");
            AddALV(list, "##l@DictionaryActions:DeleteAgentAddress@l##", "Delete agent address", "Удалить адрес", "Видалити адресу");
            AddALV(list, "##l@DictionaryActions:AddAgentPerson@l##", "Add agent person", "Добавить физическое лицо", "Додати фізична особа");
            AddALV(list, "##l@DictionaryActions:ModifyAgentPerson@l##", "Modify agent person", "Изменить физическое лицо", "Змінити фізична особа");
            AddALV(list, "##l@DictionaryActions:DeleteAgentPerson@l##", "Delete agent person", "Удалить физическое лицо", "Видалити фізична особа");
            AddALV(list, "##l@DictionaryActions:AddDepartment@l##", "Add department", "Добавить подразделение", "Додати підрозділ");
            AddALV(list, "##l@DictionaryActions:ModifyDepartment@l##", "Modify department", "Изменить подразделение", "Змінити підрозділ");
            AddALV(list, "##l@DictionaryActions:DeleteDepartment@l##", "Delete department", "Удалить подразделение", "Видалити підрозділ");
            AddALV(list, "##l@DictionaryActions:AddPosition@l##", "Add position", "Добавить должность", "Додати посаду");
            AddALV(list, "##l@DictionaryActions:ModifyPosition@l##", "Modify position", "Изменить должность", "Змінити посаду");
            AddALV(list, "##l@DictionaryActions:DeletePosition@l##", "Delete position", "Удалить должность", "Видалити посаду");
            AddALV(list, "##l@DictionaryActions:AddAgentEmployee@l##", "Add agent employee", "Добавить сотрудника", "Додати співробітника");
            AddALV(list, "##l@DictionaryActions:ModifyAgentEmployee@l##", "Modify agent employee", "Изменить сотрудника", "Змінити співробітника");
            AddALV(list, "##l@DictionaryActions:DeleteAgentEmployee@l##", "Delete agent employee", "Удалить сотрудника", "Видалити співробітника");
            AddALV(list, "##l@DictionaryActions:AddAgentCompany@l##", "Add agent company", "Добавить юридическое лицо", "Додати юридична особа");
            AddALV(list, "##l@DictionaryActions:ModifyAgentCompany@l##", "Modify agent company", "Изменить юридическое лицо", "Змінити юридична особа");
            AddALV(list, "##l@DictionaryActions:DeleteAgentCompany@l##", "Delete agent company", "Удалить юридическое лицо", "Видалити юридична особа");
            AddALV(list, "##l@DictionaryActions:AddAgentBank@l##", "Add agent bank", "Добавить банк", "Додати банк");
            AddALV(list, "##l@DictionaryActions:ModifyAgentBank@l##", "Modify agent bank", "Изменить банк", "Змінити банк");
            AddALV(list, "##l@DictionaryActions:DeleteAgentBank@l##", "Delete agent bank", "Удалить банк", "Видалити банк");
            AddALV(list, "##l@DictionaryActions:AddAgentAccount@l##", "Add agent account", "Добавить расчетный счет", "Додати розрахунковий рахунок");
            AddALV(list, "##l@DictionaryActions:ModifyAgentAccount@l##", "Modify agent account", "Изменить расчетный счет", "Змінити розрахунковий рахунок");
            AddALV(list, "##l@DictionaryActions:DeleteAgentAccount@l##", "Delete agent account", "Удалить расчетный счет", "Видалити розрахунковий рахунок");
            AddALV(list, "##l@DictionaryActions:AddStandartSendListContent@l##", "Add standart send list content", "Добавить содержание типового списка рассылки", "Додати зміст типового списку розсилки");
            AddALV(list, "##l@DictionaryActions:ModifyStandartSendListContent@l##", "Modify standart send list content", "Изменить содержание типового списка рассылки", "Змінити зміст типового списку розсилки");
            AddALV(list, "##l@DictionaryActions:DeleteStandartSendListContent@l##", "Delete standart send list content", "Удалить содержание типового списка рассылки", "Видалити вміст типового списку розсилки");
            AddALV(list, "##l@DictionaryActions:AddStandartSendList@l##", "Add standart send list", "Добавить типовой список рассылки", "Додати типовий список розсилки");
            AddALV(list, "##l@DictionaryActions:ModifyStandartSendList@l##", "Modify standart send list", "Изменить типовой список рассылки", "Змінити типовий список розсилки");
            AddALV(list, "##l@DictionaryActions:DeleteStandartSendList@l##", "Delete standart send list", "Удалить типовой список рассылки", "Видалити типовий список розсилки");
            AddALV(list, "##l@DictionaryActions:AddAgentClientCompany@l##", "Add agent client company", "Добавить компанию", "Додати компанію");
            AddALV(list, "##l@DictionaryActions:ModifyAgentClientCompany@l##", "Modify agent client company", "Изменить компанию", "Змінити компанію");
            AddALV(list, "##l@DictionaryActions:DeleteAgentClientCompany@l##", "Delete agent client company", "Удалить компанию", "Видалити компанію");
            AddALV(list, "##l@DictionaryActions:AddExecutorType@l##", "Add executor type", "Добавить тип исполнителя", "Додати тип виконавця");
            AddALV(list, "##l@DictionaryActions:ModifyExecutorType@l##", "Modify executor type", "Изменить тип исполнителя", "Змінити тип виконавця");
            AddALV(list, "##l@DictionaryActions:DeleteExecutorType@l##", "Delete executor type", "Удалить тип исполнителя", "Видалити тип виконавця");
            AddALV(list, "##l@DictionaryActions:AddExecutor@l##", "Add executor", "Добавить исполнителя", "Додати виконавця");
            AddALV(list, "##l@DictionaryActions:ModifyExecutor@l##", "Modify executor", "Изменить исполнителя", "Змінити виконавця");
            AddALV(list, "##l@DictionaryActions:DeleteExecutor@l##", "Delete executor", "Удалить исполнителя", "Видалити виконавця");
            AddALV(list, "##l@DictionaryActions:AddTag@l##", "Add tag", "Добавить тэг", "Додати тег");
            AddALV(list, "##l@DictionaryActions:ModifyTag@l##", "Modify tag", "Изменить тэг", "Змінити тег");
            AddALV(list, "##l@DictionaryActions:DeleteTag@l##", "Delete tag", "Удалить тэг", "Видалити тег");
            AddALV(list, "##l@DictionaryActions:AddCustomDictionaryType@l##", "Add custom dictionary type", "Добавить тип пользовательского словаря", "Додати тип словника");
            AddALV(list, "##l@DictionaryActions:ModifyCustomDictionaryType@l##", "Modify custom dictionary type", "Изменить тип пользовательского словаря", "Змінити тип словника");
            AddALV(list, "##l@DictionaryActions:DeleteCustomDictionaryType@l##", "Delete custom dictionary type", "Удалить тип пользовательского словаря", "Видалити тип словника");
            AddALV(list, "##l@DictionaryActions:AddCustomDictionary@l##", "Add custom dictionary", "Добавить запись пользовательского словаря", "Додати запис словника");
            AddALV(list, "##l@DictionaryActions:ModifyCustomDictionary@l##", "Modify custom dictionary", "Изменить запись пользовательского словаря", "Змінити запис словника");
            AddALV(list, "##l@DictionaryActions:DeleteCustomDictionary@l##", "Delete custom dictionary", "Удалить запись пользовательского словаря", "Видалити запис словника");
            AddALV(list, "##l@PropertyAction:AddProperty@l##", "Add property", "Добавить динамический аттрибут", "Додати динамічний атрибут");
            AddALV(list, "##l@PropertyAction:ModifyProperty@l##", "Modify property", "Изменить динамический аттрибут", "Змінити динамічний атрибут");
            AddALV(list, "##l@PropertyAction:DeleteProperty@l##", "Delete property", "Удалить динамический аттрибут", "Видалити динамічний атрибут");
            AddALV(list, "##l@PropertyAction:AddPropertyLink@l##", "Add property link", "Добавить связь динамических аттрибутов", "Додати зв'язок динамічних атрибутів");
            AddALV(list, "##l@PropertyAction:ModifyPropertyLink@l##", "Modify property link", "Изменить связь динамических аттрибутов", "Змінити зв'язок динамічних атрибутів");
            AddALV(list, "##l@PropertyAction:DeletePropertyLink@l##", "Delete property link", "Удалить связь динамических аттрибутов", "Видалити зв'язок динамічних атрибутів");
            AddALV(list, "##l@PropertyAction:ModifyPropertyValues@l##", "Modify property values", "Изменить значение динамических аттрибутов", "Змінити значення динамічних атрибутів");
            AddALV(list, "##l@EncryptionActions:AddEncryptionCertificate@l##", "Add encryption certificate", "Добавить сертификат", "Додати сертифікат");
            AddALV(list, "##l@EncryptionActions:ModifyEncryptionCertificate@l##", "Modify encryption certificate", "Изменить сертификат", "Змінити сертифікат");
            AddALV(list, "##l@EncryptionActions:VerifyPdf@l##", "Verify pdf", "Проверка Pdf", "Перевірка Pdf");
            AddALV(list, "##l@EncryptionActions:DeleteEncryptionCertificate@l##", "Delete encryption certificate", "Удалить сертификат", "Видалити сертифікат");
            AddALV(list, "##l@AdminActions:AddRole@l##", "Add role", "Добавить роль", "Додати роль");
            AddALV(list, "##l@AdminActions:ModifyRole@l##", "Modify role", "Изменить роль", "Змінити роль");
            AddALV(list, "##l@AdminActions:DeleteRole@l##", "Delete role", "Удалить роль", "Видалити роль");
            AddALV(list, "##l@AdminActions:SetRoleAction@l##", "Set role actions", "Управление действиями роли", "Управління діями ролі");
            AddALV(list, "##l@AdminActions:SetPositionRole@l##", "Set position roles", "Управление ролями должности", "Управління ролями посади");
            AddALV(list, "##l@AdminActions:SetUserRole@l##", "Set user roles", "Управление ролями пользователя", "Управління ролями користувача");
            AddALV(list, "##l@AdminActions:SetSubordination@l##", "Set subordination", "Управление правилами рассылки", "Управління правилами розсилки");
            AddALV(list, "##l@AdminActions:AddDepartmentAdmin@l##", "Add department admin", "Добавить администратора подразделения", "Додати адміністратора підрозділу");
            AddALV(list, "##l@AdminActions:DeleteDepartmentAdmin@l##", "Delete department admin", "Удалить администратора подразделения", "Видалити адміністратора підрозділу");
            AddALV(list, "##l@SystemActions:SetSetting@l##", "Add setting", "Добавить настройку", "Додати настройку");

            // Спасибо за то, что добавил перевод! Удачных идей и быстрого кода.

            return list;
        }

        public static List<AdminLanguageValues> GetAdminAccessLevels()
        {
            var list = new List<AdminLanguageValues>();

            AddALV(list, "##l@AccessLevels:Personally@l##", "Personally", "Только лично", "Тільки особисто");
            AddALV(list, "##l@AccessLevels:PersonallyAndReferents@l##", "Personally and referents", "Лично и референты", "Особисто і референти");
            AddALV(list, "##l@AccessLevels:PersonallyAndIOAndReferents@l##", "Personally and IO and referents", "Лично, ИО и референты", "Особисто, ВО і референти");

            // Спасибо за то, что добавил перевод! Удачных идей и быстрого кода.

            return list;
        }

        public static List<AdminLanguageValues> GetDictionaryEventTypes()
        {
            var list = new List<AdminLanguageValues>();

            AddALV(list, "##l@EventTypes:ПоступилВходящийДокумент", "Receive an incoming document", "Поступил входящий документ", "Вступив вхідний документ");
            AddALV(list, "##l@EventTypes:AddNewDocument", "Created project", "Создан проект", "Створено проект");
            AddALV(list, "##l@EventTypes:AddDocumentFile", "Added file", "Добавлен файл", "Доданий файл");
            AddALV(list, "##l@EventTypes:RanameDocumentFile", "Changed file", "Изменен файл", "Змінено файл");
            AddALV(list, "##l@EventTypes:ModifyDocumentFile", "Delete the file", "Удален файл", "Вилучений файл");
            AddALV(list, "##l@EventTypes:DeleteDocumentFileVersion", "Design document", "Исполнение документа", "Виконання документа");
            AddALV(list, "##l@EventTypes:DeleteDocumentFile", "The signing of the document", "Подписание документа", "Підписання документа");
            AddALV(list, "##l@EventTypes:RejectDocumentFile", "The sight of the document", "Визирование документа", "Візування документа");
            AddALV(list, "##l@EventTypes:AcceptDocumentFile", "Adoption of the document", "Утверждение документа", "Затвердження документа");
            AddALV(list, "##l@EventTypes:SendForInformation", "Approval of the document", "Согласование документа", "Узгодження документа");
            AddALV(list, "##l@EventTypes:ChangeExecutor", "Directed by reference", "Направлен для сведения", "Направлений для відомості");
            AddALV(list, "##l@EventTypes:ChangePosition", "Submitted project management", "Передано управление проектом", "Передано управління проектом");
            AddALV(list, "##l@EventTypes:SendForExecution", "Changing position in the document", "Замена должности в документе", "Заміна посади в документі");
            AddALV(list, "##l@EventTypes:SendForExecutionChange", "It aims for execution", "Направлен для исполнения", "Направлений для виконання");
            AddALV(list, "##l@EventTypes:SendForControl", "Changed parameters are sent for execution", "Изменены параметры направлен для исполнения", "Змінені параметри спрямований для виконання");
            AddALV(list, "##l@EventTypes:SendForResponsibleExecution", "It aims to control", "Направлен для контроля", "Направлений для контролю");
            AddALV(list, "##l@EventTypes:SendForResponsibleExecutionChange", "Changed parameters are sent to the control", "Изменены параметры направлен для контроля", "Змінені параметри спрямований для контролю");
            AddALV(list, "##l@EventTypes:SendForConsideration", "Directed to otvispolneniya", "Направлен для отв.исполнения", "Направлений для отвісполненія");
            AddALV(list, "##l@EventTypes:SendForInformationExternal", "Changed parameters are sent to otvispolneniya", "Изменены параметры направлен для отв.исполнения", "Змінені параметри спрямований для отвісполненія");
            AddALV(list, "##l@EventTypes:SendForVisaing", "Submitted for consideration", "Направлен для рассмотрения", "Направлений для розгляду");
            AddALV(list, "##l@EventTypes:AffixVisaing", "Considered positive", "Рассмотрен положительно", "Розглянуто позитивно");
            AddALV(list, "##l@EventTypes:RejectVisaing", "Is considered negative", "Рассмотрен отрицательно", "Розглянуто негативно");
            AddALV(list, "##l@EventTypes:WithdrawVisaing", "Directed by reference external agents", "Направлен для сведения внешнему агенту", "Направлений для відомості зовнішньому агенту");
            AddALV(list, "##l@EventTypes:SendForАgreement", "Directed at the sight", "Направлен на визирование", "Спрямований на візування");
            AddALV(list, "##l@EventTypes:AffixАgreement", "It endorses", "Завизирован", "Завізований");
            AddALV(list, "##l@EventTypes:RejectАgreement", "Denied in sight", "Отказано в визировании", "Відмовлено в візуванні");
            AddALV(list, "##l@EventTypes:WithdrawАgreement", "Withdrawn from sight", "Отозван с визирования", "Відкликаний з візування");
            AddALV(list, "##l@EventTypes:SendForАpproval", "It aims for approval", "Направлен на согласование", "Направлений на узгодження");
            AddALV(list, "##l@EventTypes:AffixАpproval", "Agreed", "Согласован", "Погоджено");
            AddALV(list, "##l@EventTypes:RejectАpproval", "Denied agreement", "Отказано в согласовании", "Відмовлено в погодженні");
            AddALV(list, "##l@EventTypes:WithdrawАpproval", "Withdrawn from the agreement", "Отозван с согласования", "Відкликаний з узгодження");
            AddALV(list, "##l@EventTypes:SendForSigning", "Submitted for approval", "Направлен на утверждение", "Направлений на затвердження");
            AddALV(list, "##l@EventTypes:AffixSigning", "Approved", "Утвержден", "Затверджено");
            AddALV(list, "##l@EventTypes:RejectSigning", "It denied in a statement", "Отказано в утверждении", "Відмовлено в утвердженні");
            AddALV(list, "##l@EventTypes:WithdrawSigning", "Withdrawn from the approval", "Отозван с утверждения", "Відкликаний з утвердження");
            AddALV(list, "##l@EventTypes:ControlOn", "I sent for signature", "Направлен на подпись", "Направлений на підпис");
            AddALV(list, "##l@EventTypes:ControlOff", "Signed", "Подписан", "Підписано");
            AddALV(list, "##l@EventTypes:ControlChange", "It refused to sign", "Отказано в подписании", "Відмовлено в підписанні");
            AddALV(list, "##l@EventTypes:ControlTargetChange", "Withdrawn from the signing", "Отозван с подписания", "Відкликаний з підписання");
            AddALV(list, "##l@EventTypes:MarkExecution", "To take control", "Взят на контроль", "Взято на контроль");
            AddALV(list, "##l@EventTypes:AcceptResult", "Out of control", "Снят с контроля", "Знятий з контролю");
            AddALV(list, "##l@EventTypes:RejectResult", "Change the control parameters", "Изменить параметры контроля", "Змінити параметри контролю");
            AddALV(list, "##l@EventTypes:SendMessage", "Change the control parameters for the artist", "Изменить параметры контроля для исполнителя", "Змінити параметри контролю для виконавця");
            AddALV(list, "##l@EventTypes:AddNewPaper", "The order is made", "Поручение выполнено", "Доручення виконано");
            AddALV(list, "##l@EventTypes:MarkOwnerDocumentPaper", "The result is adopted", "Результат принят", "Результат прийнятий");
            AddALV(list, "##l@EventTypes:MarkСorruptionDocumentPaper", "The result is rejected", "Результат отклонен", "Результат відхилений");
            AddALV(list, "##l@EventTypes:MoveDocumentPaper", "Document controls", "Контролирую документ", "Контролюю документ");
            AddALV(list, "##l@EventTypes:AddLink", "It is the responsibility of the Executive", "Являюсь ответственным исполнителем", "Є відповідальним виконавцем");
            AddALV(list, "##l@EventTypes:DeleteLink", "I am a co-executor", "Являюсь соисполнителем", "Є співвиконавцем");
            AddALV(list, "##l@EventTypes:AddNote", "Accepted", "Принято", "Прийнято");
            AddALV(list, "##l@EventTypes:TaskFormulation", "Canceled", "Отменено", "Скасовано");
            AddALV(list, "##l@EventTypes:Registered", "Changed the text", "Изменен текст", "Змінено текст");
            AddALV(list, "##l@EventTypes:LaunchPlan", "Established execution time", "Установлен срок исполнения", "Встановлено термін виконання");
            AddALV(list, "##l@EventTypes:StopPlan", "Changed the date of performance", "Изменен срок исполнения", "Змінено термін виконання");
            AddALV(list, "##l@EventTypes:SetInWork", "Appointed Executive Responsibility", "Назначен ответсвенный исполнитель", "Призначено відповідальний виконавець");
            AddALV(list, "##l@EventTypes:SetOutWork", "Revoke the appointment of the Executive Responsibility", "Отменено назначение ответсвенным исполнителем", "Скасовано призначення відповідальним виконавцем");
            AddALV(list, "##l@EventTypes:ОчереднойСрокИсполнения", "The next execution time", "Очередной срок исполнения", "Черговий термін виконання");
            AddALV(list, "##l@EventTypes:ИстекаетСрокИсполнения", "Deadline Deadline", "Истекает срок исполнения", "Закінчується строк виконання");
            AddALV(list, "##l@EventTypes:СрокИсполненияИстек", "Deadline expired", "Срок исполнения истек", "Термін виконання закінчився");
            AddALV(list, "##l@EventTypes:НаправленоСообщение", "It sent a message", "Направлено сообщение", "Направлено повідомлення");
            AddALV(list, "##l@EventTypes:ДобавленБумажныйНоситель", "Added a paper carrier", "Добавлен бумажный носитель", "Доданий паперовий носій");
            AddALV(list, "##l@EventTypes:ОтметкаНахожденияБумажногоНосителяУСебя", "Stamp paper carrier location at", "Отметка нахождения бумажного носителя у себя", "Відмітка знаходження паперового носія у себе");
            AddALV(list, "##l@EventTypes:ОтметкаПорчиБумажногоНосителя", "Mark damage paper", "Отметка порчи бумажного носителя", "Відмітка псування паперового носія");
            AddALV(list, "##l@EventTypes:ПереданыБумажныеНосители", "Transferred paper", "Переданы бумажные носители", "Передано паперові носії");
            AddALV(list, "##l@EventTypes:Примечание", "Note", "Примечание", "Примітка");
            AddALV(list, "##l@EventTypes:ФормулировкаЗадачи", "Problem Statement", "Формулировка задачи", "Формулювання завдання");
            AddALV(list, "##l@EventTypes:ПереданНаРассмотрениеРуководителю", "Referred to the supervisor", "Передан на рассмотрение руководителю", "Передано на розгляд керівнику");
            AddALV(list, "##l@EventTypes:ПолученПослеРассмотренияРуководителем", "Obtained after consideration of the head", "Получен после рассмотрения руководителем", "Отримано після розгляду керівником");
            AddALV(list, "##l@EventTypes:НаправленНаРегистрацию", "Directed at registration", "Направлен на регистрацию", "Направлений на реєстрацію");
            AddALV(list, "##l@EventTypes:Зарегистрирован", "Joined", "Зарегистрирован", "З нами");
            AddALV(list, "##l@EventTypes:ОтказаноВРегистрации", "Denied registration", "Отказано в регистрации", "Відмовлено в реєстрації");
            AddALV(list, "##l@EventTypes:ОтозванПроект", "Withdraw the draft", "Отозван проект", "Відкликаний проект");
            AddALV(list, "##l@EventTypes:ЗапущеноИсполнениеПланаРаботыПоДокументу", "Started by execution of the work plan document", "Запущено исполнение плана работы по документу", "Запущено виконання плану роботи по документу");
            AddALV(list, "##l@EventTypes:ОстановленоИсполнениеПланаРаботыПоДокументу", "It stops the execution of the document work plan", "Остановлено исполнение плана работы по документу", "Зупинено виконання плану роботи по документу");
            AddALV(list, "##l@EventTypes:РаботаВозобновлена", "Work resumed", "Работа возобновлена", "Робота відновлено");
            AddALV(list, "##l@EventTypes:РаботаЗавершена", "Job completed", "Работа завершена", "Робота завершена");



            // Спасибо за то, что добавил перевод! Удачных идей и быстрого кода.

            return list;
        }



        #endregion


        public static List<AspNetLicences> GetAspNetLicences()
        {
            var items = new List<AspNetLicences>();

            //items.Add(new AspNetLicences { Id = 0, Name = "", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = null, DurationDay = null, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 1, Name = "Base licence", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = 10, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 2, Name = "Small business licence", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = 50, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 3, Name = "Fixed Name business", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = null, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 4, Name = "Unlimited", Description = "", NamedNumberOfConnections = 50, ConcurenteNumberOfConnections = null, DurationDay = null, Functionals = null, IsActive = true });

            return items;
        }

    }
}
