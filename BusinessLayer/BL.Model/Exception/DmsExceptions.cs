using System;
using System.Collections;
using System.Collections.Generic;

namespace BL.Model.Exception
{
    /*
    С точки зрения самой архитектуры важно чтобы соблюдался один закон - 
    все ошибки должны отлавливаться и обрабатываться на одном уровне (кроме возможно специфических каких-то случаев). 
    Это логический уровень - сервисы и команды. Уровень БД не генерирует ошибок, за исключением непредвиденных ситуаций. 
    На логическом уровне происходит отлов экспешенов нижнего уровня и преобразование их в человеческий формат (удобочитаемый для пользователя), 
    делаются все проверки и в случае неуспешности проверок генерируются наши ошибки.
    второй момент архитектуры: проверки тоже должны быть только на одном уровне. 
    на всех более глубоких уровнях мы считаем что данные туда приходят уже проверенные и достоверные и дополнительно входящие данные уже не проверяем. 
    это делается для того чтобы избежать многочисленных проверок одного и того же. 
    */
    public class DmsExceptions : System.Exception
    {
        public DmsExceptions(string message) : base(message) { }
        public DmsExceptions(string message, System.Exception ex) : base(message, ex) { }

        public List<string> Parameters { set; get; }
    }
    #region [+] LicenceError ... 


    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// </summary>
    public class LicenceError : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:LicenceInformationError@l##";
        public LicenceError() : base(_MESSAGE) { }
        public LicenceError(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// Лицензия просрочена
    /// </summary>
    public class LicenceExpired : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:LicenceExpired@l##";
        public LicenceExpired() : base(_MESSAGE) { }
        public LicenceExpired(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// Превышено количество зарегистрированных пользователей
    /// </summary>
    public class LicenceExceededNumberOfRegisteredUsers : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:LicenceExceededNumberOfRegisteredUsers@l##";
        public LicenceExceededNumberOfRegisteredUsers() : base(_MESSAGE) { }
        public LicenceExceededNumberOfRegisteredUsers(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// Превышено количество подключенных пользователей
    /// </summary>
    public class LicenceExceededNumberOfConnectedUsers : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:LicenceExceededNumberOfConnectedUsers@l##";
        public LicenceExceededNumberOfConnectedUsers() : base(_MESSAGE) { }
        public LicenceExceededNumberOfConnectedUsers(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    #endregion

    /// <summary>
    /// Сообщение при ошибке регистрации нового клиента
    /// Имя клиента уже существует
    /// Client Name already exists
    /// </summary>
    public class ClientNameAlreadyExists : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ClientNameAlreadyExists@l##";
        public ClientNameAlreadyExists() : base(_MESSAGE) { }
        public ClientNameAlreadyExists(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class ClientCodeAlreadyExists : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ClientCodeAlreadyExists@l##";
        public ClientCodeAlreadyExists(string Code) : base(_MESSAGE) { Parameters = new List<string> { Code }; }
        public ClientCodeAlreadyExists(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение при проверке проверочного кода клиента
    /// Проверочный код неверен
    /// Verification code is invalid
    /// </summary>
    public class ClientVerificationCodeIncorrect : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ClientVerificationCodeIncorrect@l##";
        public ClientVerificationCodeIncorrect() : base(_MESSAGE) { }
        public ClientVerificationCodeIncorrect(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке работы с клиентом
    /// Клиент не найден
    /// Client not found
    /// </summary>
    public class ClientIsNotFound : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ClientIsNotFound@l##";
        public ClientIsNotFound() : base(_MESSAGE) { }
        public ClientIsNotFound(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке регистрации нового клиента
    /// Имя пользователя уже существует
    /// User Name already exists
    /// </summary>
    public class UserNameAlreadyExists : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserNameAlreadyExists@l##";
        public UserNameAlreadyExists() : base(_MESSAGE) { }
        public UserNameAlreadyExists(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке получения ключей
    /// </summary>
    public class CryptographicError : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CryptographicError@l##";
        public CryptographicError() : base(_MESSAGE) { }
        public CryptographicError(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    #region [+] DatabaseError ...

    /// <summary>
    /// Сообщение при ошибке работы с базой данных
    /// </summary>
    public class DatabaseError : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DatabaseError@l##";
        public DatabaseError() : base(_MESSAGE) { }
        public DatabaseError(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке работы с базой данных
    /// </summary>
    public class DatabaseIsNotSet : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DatabaseIsNotSet@l##";
        public DatabaseIsNotSet() : base(_MESSAGE) { }
        public DatabaseIsNotSet(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке работы с базой данных
    /// </summary>
    public class DatabaseIsNotFound : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DatabaseIsNotFound@l##";
        public DatabaseIsNotFound() : base(_MESSAGE) { }
        public DatabaseIsNotFound(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    #endregion

    /// <summary>
    /// Искомой комманды не существует или она не описана
    /// </summary>
    public class CommandNotDefinedError : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CommandNotDefinedError@l##";
        public CommandNotDefinedError() : base(_MESSAGE) { }
        public CommandNotDefinedError(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Параметр комманды неверного типа
    /// </summary>
    public class WrongParameterValueError : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:WrongParameterValueError@l##";
        public WrongParameterValueError() : base(_MESSAGE) { }
        public WrongParameterValueError(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Параметр комманды неверного типа
    /// </summary>
    public class WrongParameterTypeError : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:WrongParameterTypeError@l##";
        public WrongParameterTypeError() : base(_MESSAGE) { }
        public WrongParameterTypeError(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class AccessIsDenied : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:AccessIsDenied@l##";
        //TODO: DmsExceptions:передавать параметры
        public AccessIsDenied() : base(_MESSAGE) { }
        public AccessIsDenied(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class ActionIsDenied : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ActionIsDenied@l##";
        //TODO: DmsExceptions:передавать параметры
        public ActionIsDenied() : base(_MESSAGE) { }
        public ActionIsDenied(string ActionName) : base(_MESSAGE) { Parameters = new List<string> { ActionName }; }
        public ActionIsDenied(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class UserAccessIsDenied : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserAccessIsDenied@l##";
        public UserAccessIsDenied() : base(_MESSAGE) { }
        public UserAccessIsDenied(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class UserIsDeactivated : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserIsDeactivated@l##";
        public UserIsDeactivated (string UserName) : base(_MESSAGE) { Parameters = new List<string> { UserName }; }
        public UserIsDeactivated(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class UserNotExecuteAnyPosition : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserNotExecuteAnyPosition@l##";
        public UserNotExecuteAnyPosition(string UserName) : base(_MESSAGE) { Parameters = new List<string> { UserName }; }
        public UserNotExecuteAnyPosition(System.Exception ex) : base(_MESSAGE, ex) { }
    }


    public class DocumentHasAlreadyHasLink : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentHasAlreadyHasLink@l##";
        //TODO: DmsExceptions:передавать параметры
        public DocumentHasAlreadyHasLink() : base(_MESSAGE) { }
        public DocumentHasAlreadyHasLink(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Параметр комманды неверного типа
    /// </summary>
    public class NobodyIsChosen : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:NobodyIsChosen@l##";
        public NobodyIsChosen() : base(_MESSAGE) { }
        public NobodyIsChosen(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DocumentCannotBeModifiedOrDeleted : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentCannotBeModifiedOrDeleted@l##";
        public DocumentCannotBeModifiedOrDeleted() : base(_MESSAGE) { }
        public DocumentCannotBeModifiedOrDeleted(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class UserHasNoAccessToDocument : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserHasNoAccessToDocument@l##";
        public UserHasNoAccessToDocument() : base(_MESSAGE) { }
        public UserHasNoAccessToDocument(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class CannotSaveFile : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CannotSaveFile@l##";
        public CannotSaveFile() : base(_MESSAGE) { }
        public CannotSaveFile(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class UserFileNotExists : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserFileNotExists@l##";
        public UserFileNotExists() : base(_MESSAGE) { }
        public UserFileNotExists(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class UnknownDocumentFile : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UnknownDocumentFile@l##";
        public UnknownDocumentFile() : base(_MESSAGE) { }
        public UnknownDocumentFile(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class CannotAccessToFile : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CannotAccessToFile@l##";
        public CannotAccessToFile() : base(_MESSAGE) { }
        public CannotAccessToFile(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DocumentFileWasChangedExternally : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentFileWasChangedExternally@l##";
        public DocumentFileWasChangedExternally() : base(_MESSAGE) { }
        public DocumentFileWasChangedExternally(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class TaskNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:TaskNotFoundOrUserHasNoAccess@l##";
        public TaskNotFoundOrUserHasNoAccess() : base(_MESSAGE) { }
        public TaskNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class PaperNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:PaperNotFoundOrUserHasNoAccess@l##";
        public PaperNotFoundOrUserHasNoAccess() : base(_MESSAGE) { }
        public PaperNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class PaperListNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:PaperListNotFoundOrUserHasNoAccess@l##";
        public PaperListNotFoundOrUserHasNoAccess() : base(_MESSAGE) { }
        public PaperListNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DocumentNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentNotFoundOrUserHasNoAccess@l##";
        public DocumentNotFoundOrUserHasNoAccess() : base(_MESSAGE) { }
        public DocumentNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class TemplateDocumentNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:TemplateDocumentNotFoundOrUserHasNoAccess@l##";
        public TemplateDocumentNotFoundOrUserHasNoAccess() : base(_MESSAGE) { }
        public TemplateDocumentNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class CouldNotModifyTemplateDocument : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotModifyTemplateDocument@l##";
        public CouldNotModifyTemplateDocument() : base(_MESSAGE) { }
        public CouldNotModifyTemplateDocument(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class TemplateDocumentIsNotValid : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:TemplateDocumentIsNotValid@l##";
        public TemplateDocumentIsNotValid() : base(_MESSAGE) { }
        public TemplateDocumentIsNotValid(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DocumentCouldNotBeRegistered : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentCouldNotBeRegistered@l##";
        public DocumentCouldNotBeRegistered() : base(_MESSAGE) { }
        public DocumentCouldNotBeRegistered(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class CouldNotChangeAttributeLaunchPlan : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotChangeAttributeLaunchPlan@l##";
        public CouldNotChangeAttributeLaunchPlan() : base(_MESSAGE) { }
        public CouldNotChangeAttributeLaunchPlan(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class CouldNotChangeFavourite : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotChangeFavourite@l##";
        public CouldNotChangeFavourite() : base(_MESSAGE) { }
        public CouldNotChangeFavourite(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class CouldNotChangeIsInWork : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotChangeIsInWork@l##";
        public CouldNotChangeIsInWork() : base(_MESSAGE) { }
        public CouldNotChangeIsInWork(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DocumentHasAlredyBeenRegistered : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentHasAlredyBeenRegistered@l##";
        public DocumentHasAlredyBeenRegistered() : base(_MESSAGE) { }
        public DocumentHasAlredyBeenRegistered(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class PlanPointHasAlredyBeenLaunched : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:PlanPointHasAlredyBeenLaunched@l##";
        public PlanPointHasAlredyBeenLaunched() : base(_MESSAGE) { }
        public PlanPointHasAlredyBeenLaunched(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class UserPositionIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserPositionIsNotDefined@l##";
        public UserPositionIsNotDefined() : base(_MESSAGE) { }
        public UserPositionIsNotDefined(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class NeedInformationAboutCorrespondent : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:NeedInformationAboutCorrespondent@l##";
        public NeedInformationAboutCorrespondent() : base(_MESSAGE) { }
        public NeedInformationAboutCorrespondent(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class UserNameIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserNameIsNotDefined@l##";
        public UserNameIsNotDefined() : base(_MESSAGE) { }
        public UserNameIsNotDefined(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class UserUnauthorized : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserUnauthorized@l##";
        public UserUnauthorized() : base(_MESSAGE) { }
        public UserUnauthorized(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DocumentRestrictedSendListDuplication : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentRestrictedSendListDuplication@l##";
        public DocumentRestrictedSendListDuplication() : base(_MESSAGE) { }
        public DocumentRestrictedSendListDuplication(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class ContriolHasNotBeenChanged : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ContriolHasNotBeenChanged@l##";
        public ContriolHasNotBeenChanged() : base(_MESSAGE) { }
        public ContriolHasNotBeenChanged(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class WrongDocumentSendListEntry : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:WrongDocumentSendListEntry@l##";
        public WrongDocumentSendListEntry() : base(_MESSAGE) { }
        public WrongDocumentSendListEntry(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class TargetIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:TargetIsNotDefined@l##";
        public TargetIsNotDefined() : base(_MESSAGE) { }
        public TargetIsNotDefined(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class ResponsibleExecutorIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ResponsibleExecutorIsNotDefined@l##";
        public ResponsibleExecutorIsNotDefined() : base(_MESSAGE) { }
        public ResponsibleExecutorIsNotDefined(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class ResponsibleExecutorHasAlreadyBeenDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ResponsibleExecutorHasAlreadyBeenDefined@l##";
        public ResponsibleExecutorHasAlreadyBeenDefined() : base(_MESSAGE) { }
        public ResponsibleExecutorHasAlreadyBeenDefined(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class ControlerHasAlreadyBeenDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ControlerHasAlreadyBeenDefined@l##";
        public ControlerHasAlreadyBeenDefined() : base(_MESSAGE) { }
        public ControlerHasAlreadyBeenDefined(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class TaskIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:TaskIsNotDefined@l##";
        public TaskIsNotDefined() : base(_MESSAGE) { }
        public TaskIsNotDefined(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class SubordinationHasBeenViolated : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:SubordinationHasBeenViolated@l##";
        public SubordinationHasBeenViolated() : base(_MESSAGE) { }
        public SubordinationHasBeenViolated(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DocumentSendListNotFoundInDocumentRestrictedSendList : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentSendListNotFoundInDocumentRestrictedSendList@l##";
        public DocumentSendListNotFoundInDocumentRestrictedSendList() : base(_MESSAGE) { }
        public DocumentSendListNotFoundInDocumentRestrictedSendList(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DocumentSendListDoesNotMatchTheTemplate : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentSendListDoesNotMatchTheTemplate@l##";
        public DocumentSendListDoesNotMatchTheTemplate() : base(_MESSAGE) { }
        public DocumentSendListDoesNotMatchTheTemplate(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DocumentRestrictedSendListDoesNotMatchTheTemplate : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentRestrictedSendListDoesNotMatchTheTemplate@l##";
        public DocumentRestrictedSendListDoesNotMatchTheTemplate() : base(_MESSAGE) { }
        public DocumentRestrictedSendListDoesNotMatchTheTemplate(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class EventNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:EventNotFoundOrUserHasNoAccess@l##";
        public EventNotFoundOrUserHasNoAccess() : base(_MESSAGE) { }
        public EventNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class CouldNotPerformOperation : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotPerformThisOperation@l##";
        public CouldNotPerformOperation() : base(_MESSAGE) { }
        public CouldNotPerformOperation(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class CouldNotPerformOperationWithPaper : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotPerformOperationWithPaper@l##";
        public CouldNotPerformOperationWithPaper() : base(_MESSAGE) { }
        public CouldNotPerformOperationWithPaper(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class WaitNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:WaitNotFoundOrUserHasNoAccess@l##";
        public WaitNotFoundOrUserHasNoAccess() : base(_MESSAGE) { }
        public WaitNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class WaitHasAlreadyClosed : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:WaitHasAlreadyClosed@l##";
        public WaitHasAlreadyClosed() : base(_MESSAGE) { }
        public WaitHasAlreadyClosed(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class ExecutorAgentForPositionIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ExecutorAgentForPositionIsNotDefined@l##";
        public ExecutorAgentForPositionIsNotDefined() : base(_MESSAGE) { }
        public ExecutorAgentForPositionIsNotDefined(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    #region [+] Dictionary ...

    /// <summary>
    /// Сообщение об ошибке, когда невозможно добавить данные в справочник
    /// </summary>
    public class DictionaryRecordCouldNotBeAdded : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryRecordCouldNotBeAdded@l##";
        public DictionaryRecordCouldNotBeAdded() : base(_MESSAGE) { }
        public DictionaryRecordCouldNotBeAdded(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение о невозможности удаления записи из справочника
    /// </summary>
    public class DictionaryRecordCouldNotBeDeleted : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryRecordCouldNotBeDeleted@l##";
        public DictionaryRecordCouldNotBeDeleted() : base(_MESSAGE) { }
        public DictionaryRecordCouldNotBeDeleted(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение о невозможности удаления системной записи из справочника
    /// </summary>
    public class DictionarySystemRecordCouldNotBeDeleted : DmsExceptions
    {
        // pss локализация
        private const string _MESSAGE = "##l@DmsExceptions:DictionarySystemRecordCouldNotBeDeleted@l##";
        public DictionarySystemRecordCouldNotBeDeleted() : base(_MESSAGE) { }
        public DictionarySystemRecordCouldNotBeDeleted(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение об ошибке, когда пытаются обновить несуществующую строку справочника в БД
    /// </summary>
    public class DictionaryRecordWasNotFound : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryRecordWasNotFound@l##";
        public DictionaryRecordWasNotFound() : base(_MESSAGE) { }
        public DictionaryRecordWasNotFound(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение об ошибке, когда при добавлении или изменении строки в справочнике появится дубль
    /// </summary>
    public class DictionaryRecordNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryRecordNotUnique@l##";
        public DictionaryRecordNotUnique() : base(_MESSAGE) { }
        public DictionaryRecordNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать имя агента
    /// </summary>
    public class DictionaryAgentNameNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentNameNotUnique@l##";
        public DictionaryAgentNameNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryAgentNameNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryContactTypeNameNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryContactTypeNameNotUnique@l##";
        public DictionaryContactTypeNameNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryContactTypeNameNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryContactTypeCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryContactTypeCodeNotUnique@l##";
        public DictionaryContactTypeCodeNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryContactTypeCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DictionaryAgentAccountNumberNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentAccountNumberNotUnique@l##";
        public DictionaryAgentAccountNumberNotUnique(string AccountNumber) : base(_MESSAGE) { Parameters = new List<string> { AccountNumber }; }
        public DictionaryAgentAccountNumberNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DictionaryAddressTypeNameNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAddressTypeNameNotUnique@l##";
        public DictionaryAddressTypeNameNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryAddressTypeNameNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DictionaryAddressTypeCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAddressTypeCodeNotUnique@l##";
        public DictionaryAddressTypeCodeNotUnique(string Code) : base(_MESSAGE) { Parameters = new List<string> { Code }; }
        public DictionaryAddressTypeCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryAgentAddressTypeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentAddressTypeNotUnique@l##";
        public DictionaryAgentAddressTypeNotUnique() : base(_MESSAGE) { }
        public DictionaryAgentAddressTypeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryAgentAddressNameNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentAddressNameNotUnique@l##";
        public DictionaryAgentAddressNameNotUnique(string PostCode, string Address) : base(_MESSAGE) { Parameters = new List<string> { PostCode, Address }; }
        public DictionaryAgentAddressNameNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке добавить контакт агенту с и  имя агента
    /// </summary>
    public class DictionaryAgentContactNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentContactNotUnique@l##";
        public DictionaryAgentContactNotUnique(string value) : base(_MESSAGE) { Parameters = new List<string> { value }; }
        public DictionaryAgentContactNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryAgentContactTypeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentContactTypeNotUnique@l##";
        public DictionaryAgentContactTypeNotUnique(string AgentName, string Value) : base(_MESSAGE) { Parameters = new List<string> { AgentName, Value }; }
        public DictionaryAgentContactTypeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать паспортные данные физлица
    /// </summary>
    public class DictionaryAgentPersonPassportNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentPersonPassportNotUnique@l##";
        public DictionaryAgentPersonPassportNotUnique(string PassportSerial, int? PassportNumber) : base(_MESSAGE) { Parameters = new List<string> { PassportSerial, PassportNumber?.ToString() }; }
        public DictionaryAgentPersonPassportNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН физлица
    /// </summary>
    public class DictionaryAgentPersonTaxCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentPersonTaxCodeNotUnique@l##";
        public DictionaryAgentPersonTaxCodeNotUnique(string TaxCode) : base(_MESSAGE) { Parameters = new List<string> { TaxCode }; }
        public DictionaryAgentPersonTaxCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать паспортные данные сотрудника
    /// </summary>
    public class DictionaryAgentEmployeePassportNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentEmployeePassportNotUnique@l##";
        public DictionaryAgentEmployeePassportNotUnique(string PassportSerial, int? PassportNumber) : base(_MESSAGE) { Parameters = new List<string> { PassportSerial, PassportNumber?.ToString() }; }
        public DictionaryAgentEmployeePassportNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН сотрудника
    /// </summary>
    public class DictionaryAgentEmployeeTaxCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentEmployeeTaxCodeNotUnique@l##";
        public DictionaryAgentEmployeeTaxCodeNotUnique(string TaxCode) : base(_MESSAGE) { Parameters = new List<string> { TaxCode }; }
        public DictionaryAgentEmployeeTaxCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН сотрудника
    /// </summary>
    public class DictionaryAgentEmployeePersonnelNumberNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentEmployeePersonnelNumberNotUnique@l##";
        public DictionaryAgentEmployeePersonnelNumberNotUnique(string PersonnelNumber) : base(_MESSAGE) { Parameters = new List<string> { PersonnelNumber }; }
        public DictionaryAgentEmployeePersonnelNumberNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН сотрудника
    /// </summary>
    public class DictionaryAgentBankMFOCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentBankMFOCodeNotUnique@l##";
        public DictionaryAgentBankMFOCodeNotUnique(string Name, string MFOCode) : base(_MESSAGE) { Parameters = new List<string> { Name, MFOCode }; }
        public DictionaryAgentBankMFOCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН юрлица
    /// </summary>
    public class DictionaryAgentCompanyTaxCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentCompanyTaxCodeNotUnique@l##";
        public DictionaryAgentCompanyTaxCodeNotUnique(string Name, string TaxCode) : base(_MESSAGE) { Parameters = new List<string> { Name, TaxCode }; }
        public DictionaryAgentCompanyTaxCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать VATCode юрлица
    /// </summary>
    public class DictionaryAgentCompanyVATCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentCompanyVATCodeNotUnique@l##";
        public DictionaryAgentCompanyVATCodeNotUnique(string Name, string VATCode) : base(_MESSAGE) { Parameters = new List<string> { Name, VATCode }; }
        public DictionaryAgentCompanyVATCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать VATCode юрлица
    /// </summary>
    public class DictionaryAgentCompanyOKPOCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentCompanyOKPOCodeNotUnique@l##";
        public DictionaryAgentCompanyOKPOCodeNotUnique(string Name, string OKPOCode) : base(_MESSAGE) { Parameters = new List<string> { Name, OKPOCode }; }
        public DictionaryAgentCompanyOKPOCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DictionaryTagNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryTagNotFoundOrUserHasNoAccess@l##";
        public DictionaryTagNotFoundOrUserHasNoAccess() : base(_MESSAGE) { }
        public DictionaryTagNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DictionaryCostomDictionaryNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryCostomDictionaryNotUnique@l##";
        public DictionaryCostomDictionaryNotUnique() : base(_MESSAGE) { }
        public DictionaryCostomDictionaryNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryCostomDictionaryTypeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryCostomDictionaryTypeNotUnique@l##";
        public DictionaryCostomDictionaryTypeNotUnique(string Code) : base(_MESSAGE) { Parameters = new List<string> { Code }; }
        public DictionaryCostomDictionaryTypeNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionarysdDepartmentNotBeSubordinated : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionarysdDepartmentNotBeSubordinated@l##";
        public DictionarysdDepartmentNotBeSubordinated(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionarysdDepartmentNotBeSubordinated(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryDepartmentNameNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryDepartmentNameNotUnique@l##";
        public DictionaryDepartmentNameNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryDepartmentNameNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryDocumentSubjectNameNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryDocumentSubjectNameNotUnique@l##";
        public DictionaryDocumentSubjectNameNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryDocumentSubjectNameNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DictionaryDocumentTypeNameNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryDocumentTypeNameNotUnique@l##";
        public DictionaryDocumentTypeNameNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryDocumentTypeNameNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryPositionExecutorNotUnique : DmsExceptions
    {//Сотрудник \"{1}\" не может быть назначен на должность повторно \"{0}\" c {2} по {3}
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryPositionExecutorNotUnique@l##";
        public DictionaryPositionExecutorNotUnique(string PositionName, string AgentName, DateTime StartDate, DateTime EndDate) : base(_MESSAGE)
        { Parameters = new List<string> { PositionName, AgentName, StartDate.ToString(), EndDate.ToString() }; }
        public DictionaryPositionExecutorNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryPositionExecutorIsInvalidPeriod : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryPositionExecutorIsInvalidPeriod@l##";
        public DictionaryPositionExecutorIsInvalidPeriod() : base(_MESSAGE) { }
        public DictionaryPositionExecutorIsInvalidPeriod(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryPositionExecutorPersonalNotUnique : DmsExceptions
    {//На должность \"{0}\" штатно назначен \"{1}\" c {2} по {3}
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryPositionExecutorPersonalNotUnique@l##";
        public DictionaryPositionExecutorPersonalNotUnique(string PositionName, string AgentName, DateTime StartDate, DateTime EndDate) : base(_MESSAGE)
        { Parameters = new List<string> { PositionName, AgentName, StartDate.ToString(), EndDate.ToString() }; }
        public DictionaryPositionExecutorPersonalNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryPositionExecutorIONotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryPositionExecutorIONotUnique@l##";
        public DictionaryPositionExecutorIONotUnique(string PositionName, string AgentName, DateTime StartDate, DateTime EndDate) : base(_MESSAGE)
        { Parameters = new List<string> { PositionName, AgentName, StartDate.ToString(), EndDate.ToString() }; }
        public DictionaryPositionExecutorIONotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryPositionExecutorReferentNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryPositionExecutorReferentNotUnique@l##";
        public DictionaryPositionExecutorReferentNotUnique(string PositionName, string AgentName, DateTime StartDate, DateTime EndDate) : base(_MESSAGE)
        { Parameters = new List<string> { PositionName, AgentName, StartDate.ToString(), EndDate.ToString() }; }
        public DictionaryPositionExecutorReferentNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }


    public class DictionaryRegistrationJournalNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryRegistrationJournalNotUnique@l##";
        public DictionaryRegistrationJournalNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryRegistrationJournalNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DictionaryStandartSendListNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryStandartSendListNotUnique@l##";
        public DictionaryStandartSendListNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryStandartSendListNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class DictionaryStandartSendListContentNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryStandartSendListContentNotUnique@l##";
        public DictionaryStandartSendListContentNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryStandartSendListContentNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class DictionaryTagNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryTagNotUnique@l##";
        public DictionaryTagNotUnique(string Name) : base(_MESSAGE) { Parameters = new List<string> { Name }; }
        public DictionaryTagNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    #endregion

    #region [+] Admin ...
    /// <summary>
    /// Сообщение об ошибке, когда при добавлении или изменении строки появится дубль
    /// </summary>
    public class AdminRecordNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:AdminRecordNotUnique@l##";
        public AdminRecordNotUnique() : base(_MESSAGE) { }
        public AdminRecordNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение об ошибке, когда невозможно добавить данные в админку
    /// </summary>
    public class AdminRecordCouldNotBeAdded : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:AdminRecordCouldNotBeAdded@l##";
        public AdminRecordCouldNotBeAdded() : base(_MESSAGE) { }
        public AdminRecordCouldNotBeAdded(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение о невозможности удаления записи в админке
    /// </summary>
    public class AdminRecordCouldNotBeDeleted : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:AdminRecordCouldNotBeDeleted@l##";
        public AdminRecordCouldNotBeDeleted() : base(_MESSAGE) { }
        public AdminRecordCouldNotBeDeleted(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение об ошибке, когда пытаются обновить несуществующую строку справочника в БД
    /// </summary>
    public class AdminRecordWasNotFound : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:AdminRecordWasNotFound@l##";
        public AdminRecordWasNotFound() : base(_MESSAGE) { }
        public AdminRecordWasNotFound(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    #endregion
    public class RecordNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:RecordNotUnique@l##";
        public RecordNotUnique() : base(_MESSAGE) { }
        public RecordNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class IncomingModelIsNotValid : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:IncomingModelIsNotValid@l##";
        public IncomingModelIsNotValid(string Text) : base(_MESSAGE) { Parameters = new List<string> { Text }; }
        public IncomingModelIsNotValid(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class NotFilledWithAdditionalRequiredAttributes : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:NotFilledWithAdditionalRequiredAttributes@l##";
        public NotFilledWithAdditionalRequiredAttributes() : base(_MESSAGE) { }
        public NotFilledWithAdditionalRequiredAttributes(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class SigningTypeNotAllowed : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:SigningTypeNotAllowed@l##";
        public SigningTypeNotAllowed() : base(_MESSAGE) { }
        public SigningTypeNotAllowed(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class EncryptionCertificateWasNotFound : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:EncryptionCertificateWasNotFound@l##";
        public EncryptionCertificateWasNotFound() : base(_MESSAGE) { }
        public EncryptionCertificateWasNotFound(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class EncryptionCertificateDigitalSignatureFailed : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:EncryptionCertificateDigitalSignatureFailed@l##";
        public EncryptionCertificateDigitalSignatureFailed() : base(_MESSAGE) { }
        public EncryptionCertificateDigitalSignatureFailed(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    public class EncryptionCertificateHasExpired : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:EncryptionCertificateHasExpired@l##";
        public EncryptionCertificateHasExpired() : base(_MESSAGE) { }
        public EncryptionCertificateHasExpired(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    public class EncryptionCertificatePrivateKeyСanNotBeExported : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:EncryptionCertificatePrivateKeyСanNotBeExported@l##";
        public EncryptionCertificatePrivateKeyСanNotBeExported() : base(_MESSAGE) { }
        public EncryptionCertificatePrivateKeyСanNotBeExported(System.Exception ex) : base(_MESSAGE, ex) { }
    }
}