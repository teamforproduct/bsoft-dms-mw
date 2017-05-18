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


    /*
     Есть проблема с private const string . Часто возникают мешанические ошибки при добавлении новых исключений копированием
     Приходится в  ДУБЛИРОВАТЬ название класса ошибки.
     
     В обработчике ошибок на основании названиея класса ошибки формироуется лейбл для перевода. Поэтому нет смысла поддерживать 
         
    */

    public class DmsExceptions : System.Exception
    {
        public DmsExceptions() : base("DmsException") { }
        public DmsExceptions(System.Exception ex) : base("DmsException", ex) { }
        public List<string> Parameters { set; get; }
        public IEnumerable<string> Errors { set; get; }
    }

    public class TokenAlreadyExists : DmsExceptions
    {
        public TokenAlreadyExists() : base() { }
        public TokenAlreadyExists(System.Exception ex) : base(ex) { }
    }

    public class SettingValueIsNotSet : DmsExceptions
    {
        public SettingValueIsNotSet(string settingKey) : base() { Parameters = new List<string> { settingKey }; }
        public SettingValueIsNotSet(System.Exception ex) : base(ex) { }
    }

    public class SettingValueIsInvalid : DmsExceptions
    {
        public SettingValueIsInvalid(string settingKey) : base() { Parameters = new List<string> { settingKey }; }
        public SettingValueIsInvalid(System.Exception ex) : base(ex) { }
    }

    public class FilterRequired : DmsExceptions
    {
        public FilterRequired() : base() { }
        public FilterRequired(System.Exception ex) : base(ex) { }
    }

    public class OrgRequired : DmsExceptions
    {
        public OrgRequired() : base() { }
        public OrgRequired(System.Exception ex) : base(ex) { }
    }

    public class DepartmentRequired : DmsExceptions
    {
        public DepartmentRequired() : base() { }
        public DepartmentRequired(System.Exception ex) : base(ex) { }
    }

    public class PositionRequired : DmsExceptions
    {
        public PositionRequired() : base() { }
        public PositionRequired(System.Exception ex) : base(ex) { }
    }

    public class FingerprintRequired : DmsExceptions
    {
        public FingerprintRequired() : base() { }
        public FingerprintRequired(System.Exception ex) : base(ex) { }
    }

    #region [+] LicenceError ... 


    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// </summary>
    public class LicenceError : DmsExceptions
    {
        public LicenceError() : base() { }
        public LicenceError(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// Лицензия просрочена
    /// </summary>
    public class LicenceExpired : DmsExceptions
    {
        public LicenceExpired() : base() { }
        public LicenceExpired(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// Превышено количество зарегистрированных пользователей
    /// </summary>
    public class LicenceExceededNumberOfRegisteredUsers : DmsExceptions
    {
        public LicenceExceededNumberOfRegisteredUsers() : base() { }
        public LicenceExceededNumberOfRegisteredUsers(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// Превышено количество подключенных пользователей
    /// </summary>
    public class LicenceExceededNumberOfConnectedUsers : DmsExceptions
    {
        public LicenceExceededNumberOfConnectedUsers() : base() { }
        public LicenceExceededNumberOfConnectedUsers(System.Exception ex) : base(ex) { }
    }
    #endregion

    public class DefaultLanguageIsNotSet : DmsExceptions
    {
        public DefaultLanguageIsNotSet() : base() { }
        public DefaultLanguageIsNotSet(System.Exception ex) : base(ex) { }
    }

    public class ClientRequestIsNotFound : DmsExceptions
    {
        public ClientRequestIsNotFound() : base() { }
        public ClientRequestIsNotFound(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке регистрации нового клиента
    /// Имя клиента уже существует
    /// Client Name already exists
    /// </summary>
    public class ClientCodeAlreadyExists : DmsExceptions
    {
        public ClientCodeAlreadyExists(string Code) : base() { Parameters = new List<string> { Code }; }
        public ClientCodeAlreadyExists(System.Exception ex) : base(ex) { }
    }

    public class ClientCreateException : DmsExceptions
    {
        public ClientCreateException(System.Exception ex) : base(ex) { }
        public ClientCreateException(IEnumerable<string> Errors) : base() { base.Errors = Errors; }
    }


    /// <summary>
    /// Сообщение при проверке проверочного кода клиента
    /// Проверочный код неверен
    /// Verification code is invalid
    /// </summary>
    public class ClientVerificationCodeIncorrect : DmsExceptions
    {
        public ClientVerificationCodeIncorrect() : base() { }
        public ClientVerificationCodeIncorrect(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке работы с клиентом
    /// Клиент не найден
    /// Client not found
    /// </summary>
    public class ClientIsNotFound : DmsExceptions
    {
        public ClientIsNotFound() : base() { }
        public ClientIsNotFound(System.Exception ex) : base(ex) { }
    }

    public class ClientCodeRequired : DmsExceptions
    {
        public ClientCodeRequired() : base() { }
        public ClientCodeRequired(System.Exception ex) : base(ex) { }
    }

    public class ClientCodeInvalid : DmsExceptions
    {
        public ClientCodeInvalid() : base() { }
        public ClientCodeInvalid(System.Exception ex) : base(ex) { }
    }


    public class ServerIsNotFound : DmsExceptions
    {
        public ServerIsNotFound() : base() { }
        public ServerIsNotFound(System.Exception ex) : base(ex) { }
    }

    public class ClientServerIsNotSet : DmsExceptions
    {
        public ClientServerIsNotSet(string clientName) : base() { }
        public ClientServerIsNotSet(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке регистрации нового клиента
    /// Имя пользователя уже существует
    /// User Name already exists
    /// </summary>
    public class UserNameAlreadyExists : DmsExceptions
    {
        public UserNameAlreadyExists(string userName) : base() { Parameters = new List<string> { userName }; }
        public UserNameAlreadyExists(System.Exception ex) : base(ex) { }
    }

    public class UserCouldNotBeAdded : DmsExceptions
    {
        public UserCouldNotBeAdded(string userName, IEnumerable<string> Errors) : base() { Parameters = new List<string> { userName }; base.Errors = Errors; }
        public UserCouldNotBeAdded(System.Exception ex) : base(ex) { }
    }

    public class UserCouldNotBeDeleted : DmsExceptions
    {
        public UserCouldNotBeDeleted(string userName, IEnumerable<string> Errors) : base() { Parameters = new List<string> { userName }; base.Errors = Errors; }
        public UserCouldNotBeDeleted(System.Exception ex) : base(ex) { }
    }

    public class UserLoginCouldNotBeChanged : DmsExceptions
    {
        public UserLoginCouldNotBeChanged(string userName, IEnumerable<string> Errors) : base() { Parameters = new List<string> { userName }; base.Errors = Errors; }
        public UserLoginCouldNotBeChanged(System.Exception ex) : base(ex) { }
    }

    public class UserParmsCouldNotBeChanged : DmsExceptions
    {
        public UserParmsCouldNotBeChanged(IEnumerable<string> Errors) : base() { base.Errors = Errors; }
        public UserParmsCouldNotBeChanged(System.Exception ex) : base(ex) { }
    }

    public class UserIsNotDefined : DmsExceptions
    {
        public UserIsNotDefined() : base() { }
        public UserIsNotDefined(System.Exception ex) : base(ex) { }
    }

    public class UserContextIsNotDefined : DmsExceptions
    {
        public UserContextIsNotDefined() : base() { }
        public UserContextIsNotDefined(System.Exception ex) : base(ex) { }
    }

    public class UserUnauthorized : DmsExceptions
    {
        public UserUnauthorized() : base() { }
        public UserUnauthorized(System.Exception ex) : base(ex) { }
    }

    public class UserMustChangePassword : DmsExceptions
    {
        public UserMustChangePassword() : base() { }
        public UserMustChangePassword(System.Exception ex) : base(ex) { }
    }

    public class UserMustConfirmEmail : DmsExceptions
    {
        public UserMustConfirmEmail() : base() { }
        public UserMustConfirmEmail(System.Exception ex) : base(ex) { }
    }

    // НЕ переименовывать - есть if на фронте
    public class UserFingerprintIsIncorrect : DmsExceptions
    {
        public UserFingerprintIsIncorrect() : base() { }
        public UserFingerprintIsIncorrect(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// UserContextUseOnly
    /// </summary>
    public class UserAccessIsDenied : DmsExceptions
    {
        public UserAccessIsDenied() : base() { }
        public UserAccessIsDenied(System.Exception ex) : base(ex) { }
    }

    public class UserNameOrPasswordIsIncorrect : DmsExceptions
    {
        public UserNameOrPasswordIsIncorrect() : base() { }
        public UserNameOrPasswordIsIncorrect(System.Exception ex) : base(ex) { }
    }

    // НЕ переименовывать - есть if на фронте
    public class UserAnswerIsIncorrect : DmsExceptions
    {
        public UserAnswerIsIncorrect() : base() { }
        public UserAnswerIsIncorrect(System.Exception ex) : base(ex) { }
    }

    public class EmployeeIsDeactivated : DmsExceptions
    {
        public EmployeeIsDeactivated(string UserName) : base() { Parameters = new List<string> { UserName }; }
        public EmployeeIsDeactivated(System.Exception ex) : base(ex) { }
    }
    public class UserIsLockout : DmsExceptions
    {
        public UserIsLockout(string UserName) : base() { Parameters = new List<string> { UserName }; }
        public UserIsLockout(System.Exception ex) : base(ex) { }
    }

    public class UserIsLockoutByAdmin : DmsExceptions
    {
        public UserIsLockoutByAdmin(string UserName) : base() { Parameters = new List<string> { UserName }; }
        public UserIsLockoutByAdmin(System.Exception ex) : base(ex) { }
    }

    public class RoleNameAlreadyExists : DmsExceptions
    {
        public RoleNameAlreadyExists(string roleName) : base() { Parameters = new List<string> { roleName }; }
        public RoleNameAlreadyExists(System.Exception ex) : base(ex) { }
    }


    public class RoleCouldNotBeAdded : DmsExceptions
    {
        public RoleCouldNotBeAdded(string roleName, IEnumerable<string> Errors) : base() { Parameters = new List<string> { roleName }; base.Errors = Errors; }
        public RoleCouldNotBeAdded(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке получения ключей
    /// </summary>
    public class CryptographicError : DmsExceptions
    {
        public CryptographicError() : base() { }
        public CryptographicError(System.Exception ex) : base(ex) { }
    }
    #region [+] DatabaseError ...

    /// <summary>
    /// Сообщение при ошибке работы с базой данных
    /// </summary>
    public class DatabaseError : DmsExceptions
    {
        public DatabaseError() : base() { }
        public DatabaseError(System.Exception ex) : base(ex) { }
        public DatabaseError(IEnumerable<string> Errors) : base() { base.Errors = Errors; }
    }

    /// <summary>
    /// Сообщение при ошибке работы с базой данных
    /// </summary>
    public class DatabaseIsNotSet : DmsExceptions
    {
        public DatabaseIsNotSet() : base() { }
        public DatabaseIsNotSet(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение при ошибке работы с базой данных
    /// </summary>
    public class DatabaseIsNotFound : DmsExceptions
    {
        public DatabaseIsNotFound() : base() { }
        public DatabaseIsNotFound(System.Exception ex) : base(ex) { }
    }
    #endregion

    /// <summary>
    /// Искомой комманды не существует или она не описана
    /// </summary>
    public class CommandNotDefinedError : DmsExceptions
    {
        public CommandNotDefinedError(string CommandName) : base() { Parameters = new List<string> { CommandName }; }
        public CommandNotDefinedError(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Параметры АПИ некорректны
    /// </summary>
    public class WrongAPIParameters : DmsExceptions
    {
        public WrongAPIParameters() : base() { }
        public WrongAPIParameters(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Параметр комманды неверного типа
    /// </summary>
    public class WrongParameterValueError : DmsExceptions
    {
        public WrongParameterValueError() : base() { }
        public WrongParameterValueError(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Параметр комманды неверного типа
    /// </summary>
    public class WrongParameterTypeError : DmsExceptions
    {
        public WrongParameterTypeError() : base() { }
        public WrongParameterTypeError(System.Exception ex) : base(ex) { }
    }
    public class AccessIsDenied : DmsExceptions
    {
        //TODO: DmsExceptions:передавать параметры
        public AccessIsDenied() : base() { }
        public AccessIsDenied(System.Exception ex) : base(ex) { }
    }

    public class ActionIsDenied : DmsExceptions
    {
        //TODO: DmsExceptions:передавать параметры
        public ActionIsDenied() : base() { }
        public ActionIsDenied(string ActionName) : base() { Parameters = new List<string> { ActionName }; }
        public ActionIsDenied(System.Exception ex) : base(ex) { }
    }






    public class EmployeePositionExecutorIsIncorrect : DmsExceptions
    {
        public EmployeePositionExecutorIsIncorrect() : base() { }
        public EmployeePositionExecutorIsIncorrect(System.Exception ex) : base(ex) { }
    }

    public class EmployeeNotExecuteAnyPosition : DmsExceptions
    {
        public EmployeeNotExecuteAnyPosition(string UserName) : base() { Parameters = new List<string> { UserName }; }
        public EmployeeNotExecuteAnyPosition(System.Exception ex) : base(ex) { }
    }

    public class EmployeeNotExecuteCheckPosition : DmsExceptions
    {
        public EmployeeNotExecuteCheckPosition() : base() { }
        public EmployeeNotExecuteCheckPosition(System.Exception ex) : base(ex) { }
    }

    public class EmployeeHasNoAccessToDocument : DmsExceptions
    {
        public EmployeeHasNoAccessToDocument() : base() { }
        public EmployeeHasNoAccessToDocument(System.Exception ex) : base(ex) { }
    }

    public class FileNotExists : DmsExceptions
    {
        public FileNotExists() : base() { }
        public FileNotExists(System.Exception ex) : base(ex) { }
    }

    //TODO PDF Change message to pdf file
    public class FilePdfNotExists : DmsExceptions
    {
        public FilePdfNotExists() : base() { }
        public FilePdfNotExists(System.Exception ex) : base(ex) { }
    }



    public class DocumentHasAlreadyHasLink : DmsExceptions
    {
        //TODO: DmsExceptions:передавать параметры
        public DocumentHasAlreadyHasLink() : base() { }
        public DocumentHasAlreadyHasLink(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Параметр комманды неверного типа
    /// </summary>
    public class NobodyIsChosen : DmsExceptions
    {
        public NobodyIsChosen() : base() { }
        public NobodyIsChosen(System.Exception ex) : base(ex) { }
    }
    public class DocumentCannotBeModifiedOrDeleted : DmsExceptions
    {
        public DocumentCannotBeModifiedOrDeleted() : base() { }
        public DocumentCannotBeModifiedOrDeleted(System.Exception ex) : base(ex) { }
    }

    public class CannotSaveFile : DmsExceptions
    {
        public CannotSaveFile() : base() { }
        public CannotSaveFile(System.Exception ex) : base(ex) { }
    }

    public class UnknownDocumentFile : DmsExceptions
    {
        public UnknownDocumentFile() : base() { }
        public UnknownDocumentFile(System.Exception ex) : base(ex) { }
    }
    public class CannotAccessToFile : DmsExceptions
    {
        public CannotAccessToFile() : base() { }
        public CannotAccessToFile(System.Exception ex) : base(ex) { }
    }
    public class DocumentFileWasChangedExternally : DmsExceptions
    {
        public DocumentFileWasChangedExternally() : base() { }
        public DocumentFileWasChangedExternally(System.Exception ex) : base(ex) { }
    }
    public class TaskNotFoundOrUserHasNoAccess : DmsExceptions
    {
        public TaskNotFoundOrUserHasNoAccess() : base() { }
        public TaskNotFoundOrUserHasNoAccess(System.Exception ex) : base(ex) { }
    }
    public class PaperNotFoundOrUserHasNoAccess : DmsExceptions
    {
        public PaperNotFoundOrUserHasNoAccess() : base() { }
        public PaperNotFoundOrUserHasNoAccess(System.Exception ex) : base(ex) { }
    }
    public class PaperListNotFoundOrUserHasNoAccess : DmsExceptions
    {
        public PaperListNotFoundOrUserHasNoAccess() : base() { }
        public PaperListNotFoundOrUserHasNoAccess(System.Exception ex) : base(ex) { }
    }
    public class DocumentNotFoundOrUserHasNoAccess : DmsExceptions
    {
        public DocumentNotFoundOrUserHasNoAccess() : base() { }
        public DocumentNotFoundOrUserHasNoAccess(System.Exception ex) : base(ex) { }
    }
    public class TemplateDocumentNotFoundOrUserHasNoAccess : DmsExceptions
    {
        public TemplateDocumentNotFoundOrUserHasNoAccess() : base() { }
        public TemplateDocumentNotFoundOrUserHasNoAccess(System.Exception ex) : base(ex) { }
    }
    public class CouldNotAddTemplate : DmsExceptions
    {
        public CouldNotAddTemplate() : base() { }
        public CouldNotAddTemplate(System.Exception ex) : base(ex) { }
    }

    public class CouldNotDeleteTemplate : DmsExceptions
    {
        public CouldNotDeleteTemplate() : base() { }
        public CouldNotDeleteTemplate(System.Exception ex) : base(ex) { }
    }

    public class CouldNotAddTemplateFile : DmsExceptions
    {
        public CouldNotAddTemplateFile() : base() { }
        public CouldNotAddTemplateFile(System.Exception ex) : base(ex) { }
    }

    public class CouldNotAddTemplateTask : DmsExceptions
    {
        public CouldNotAddTemplateTask() : base() { }
        public CouldNotAddTemplateTask(System.Exception ex) : base(ex) { }
    }


    public class TemplateDocumentIsNotValid : DmsExceptions
    {
        public TemplateDocumentIsNotValid() : base() { }
        public TemplateDocumentIsNotValid(System.Exception ex) : base(ex) { }
    }
    public class DocumentCouldNotBeRegistered : DmsExceptions
    {
        public DocumentCouldNotBeRegistered() : base() { }
        public DocumentCouldNotBeRegistered(System.Exception ex) : base(ex) { }
    }
    public class DocumentCouldNotBeRegisteredNoValidSign : DmsExceptions
    {
        public DocumentCouldNotBeRegisteredNoValidSign() : base() { }
        public DocumentCouldNotBeRegisteredNoValidSign(System.Exception ex) : base(ex) { }
    }
    public class CouldNotChangeAttributeLaunchPlan : DmsExceptions
    {
        public CouldNotChangeAttributeLaunchPlan() : base() { }
        public CouldNotChangeAttributeLaunchPlan(System.Exception ex) : base(ex) { }
    }
    public class CouldNotChangeFavourite : DmsExceptions
    {
        public CouldNotChangeFavourite() : base() { }
        public CouldNotChangeFavourite(System.Exception ex) : base(ex) { }
    }
    public class CouldNotChangeIsInWork : DmsExceptions
    {
        public CouldNotChangeIsInWork() : base() { }
        public CouldNotChangeIsInWork(System.Exception ex) : base(ex) { }
    }
    public class DocumentHasAlredyBeenRegistered : DmsExceptions
    {
        public DocumentHasAlredyBeenRegistered() : base() { }
        public DocumentHasAlredyBeenRegistered(System.Exception ex) : base(ex) { }
    }
    public class PlanPointHasAlredyBeenLaunched : DmsExceptions
    {
        public PlanPointHasAlredyBeenLaunched() : base() { }
        public PlanPointHasAlredyBeenLaunched(System.Exception ex) : base(ex) { }
    }


    public class NeedInformationAboutCorrespondent : DmsExceptions
    {
        public NeedInformationAboutCorrespondent() : base() { }
        public NeedInformationAboutCorrespondent(System.Exception ex) : base(ex) { }
    }

    public class ResetPasswordCodeInvalid : DmsExceptions
    {
        public ResetPasswordCodeInvalid(IEnumerable<string> Errors) : base() { base.Errors = Errors; }
        public ResetPasswordCodeInvalid(System.Exception ex) : base(ex) { }
    }

    public class ResetPasswordFailed : DmsExceptions
    {
        public ResetPasswordFailed() : base() { }
        public ResetPasswordFailed(System.Exception ex) : base(ex) { }
    }


    public class DocumentRestrictedSendListDuplication : DmsExceptions
    {
        public DocumentRestrictedSendListDuplication() : base() { }
        public DocumentRestrictedSendListDuplication(System.Exception ex) : base(ex) { }
    }
    public class ContriolHasNotBeenChanged : DmsExceptions
    {
        public ContriolHasNotBeenChanged() : base() { }
        public ContriolHasNotBeenChanged(System.Exception ex) : base(ex) { }
    }

    public class WrongDocumentSendListEntry : DmsExceptions
    {
        public WrongDocumentSendListEntry() : base() { }
        public WrongDocumentSendListEntry(System.Exception ex) : base(ex) { }
    }
    public class TargetIsNotDefined : DmsExceptions
    {
        public TargetIsNotDefined() : base() { }
        public TargetIsNotDefined(System.Exception ex) : base(ex) { }
    }
    public class ResponsibleExecutorIsNotDefined : DmsExceptions
    {
        public ResponsibleExecutorIsNotDefined() : base() { }
        public ResponsibleExecutorIsNotDefined(System.Exception ex) : base(ex) { }
    }
    public class ResponsibleExecutorHasAlreadyBeenDefined : DmsExceptions
    {
        public ResponsibleExecutorHasAlreadyBeenDefined() : base() { }
        public ResponsibleExecutorHasAlreadyBeenDefined(System.Exception ex) : base(ex) { }
    }
    public class ControlerHasAlreadyBeenDefined : DmsExceptions
    {
        public ControlerHasAlreadyBeenDefined() : base() { }
        public ControlerHasAlreadyBeenDefined(System.Exception ex) : base(ex) { }
    }
    public class TaskIsNotDefined : DmsExceptions
    {
        public TaskIsNotDefined() : base() { }
        public TaskIsNotDefined(System.Exception ex) : base(ex) { }
    }
    public class SubordinationHasBeenViolated : DmsExceptions
    {
        public SubordinationHasBeenViolated() : base() { }
        public SubordinationHasBeenViolated(System.Exception ex) : base(ex) { }
    }
    public class SubordinationForDueDateHasBeenViolated : DmsExceptions
    {
        public SubordinationForDueDateHasBeenViolated() : base() { }
        public SubordinationForDueDateHasBeenViolated(System.Exception ex) : base(ex) { }
    }
    public class DocumentSendListNotFoundInDocumentRestrictedSendList : DmsExceptions
    {
        public DocumentSendListNotFoundInDocumentRestrictedSendList() : base() { }
        public DocumentSendListNotFoundInDocumentRestrictedSendList(System.Exception ex) : base(ex) { }
    }
    public class DocumentSendListDoesNotMatchTheTemplate : DmsExceptions
    {
        public DocumentSendListDoesNotMatchTheTemplate() : base() { }
        public DocumentSendListDoesNotMatchTheTemplate(System.Exception ex) : base(ex) { }
    }
    public class DocumentRestrictedSendListDoesNotMatchTheTemplate : DmsExceptions
    {
        public DocumentRestrictedSendListDoesNotMatchTheTemplate() : base() { }
        public DocumentRestrictedSendListDoesNotMatchTheTemplate(System.Exception ex) : base(ex) { }
    }
    public class EventNotFoundOrUserHasNoAccess : DmsExceptions
    {
        public EventNotFoundOrUserHasNoAccess() : base() { }
        public EventNotFoundOrUserHasNoAccess(System.Exception ex) : base(ex) { }
    }
    public class CouldNotPerformOperation : DmsExceptions
    {
        public CouldNotPerformOperation() : base() { }
        public CouldNotPerformOperation(System.Exception ex) : base(ex) { }
    }
    public class CouldNotPerformOperationWithPaper : DmsExceptions
    {
        public CouldNotPerformOperationWithPaper() : base() { }
        public CouldNotPerformOperationWithPaper(System.Exception ex) : base(ex) { }
    }
    public class WaitNotFoundOrUserHasNoAccess : DmsExceptions
    {
        public WaitNotFoundOrUserHasNoAccess() : base() { }
        public WaitNotFoundOrUserHasNoAccess(System.Exception ex) : base(ex) { }
    }
    public class WaitHasAlreadyClosed : DmsExceptions
    {
        public WaitHasAlreadyClosed() : base() { }
        public WaitHasAlreadyClosed(System.Exception ex) : base(ex) { }
    }
    public class ExecutorAgentForPositionIsNotDefined : DmsExceptions
    {
        public ExecutorAgentForPositionIsNotDefined() : base() { }
        public ExecutorAgentForPositionIsNotDefined(System.Exception ex) : base(ex) { }
    }
    #region [+] Dictionary ...

    /// <summary>
    /// Сообщение об ошибке, когда невозможно добавить данные в справочник
    /// </summary>
    public class DictionaryRecordCouldNotBeAdded : DmsExceptions
    {
        public DictionaryRecordCouldNotBeAdded() : base() { }
        public DictionaryRecordCouldNotBeAdded(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение о невозможности удаления записи из справочника
    /// </summary>
    public class DictionaryRecordCouldNotBeDeleted : DmsExceptions
    {
        public DictionaryRecordCouldNotBeDeleted() : base() { }
        public DictionaryRecordCouldNotBeDeleted(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение о невозможности удаления системной записи из справочника
    /// </summary>
    public class DictionarySystemRecordCouldNotBeDeleted : DmsExceptions
    {
        public DictionarySystemRecordCouldNotBeDeleted() : base() { }
        public DictionarySystemRecordCouldNotBeDeleted(System.Exception ex) : base(ex) { }
    }

    public class DefaultRolesCouldNotBeModified : DmsExceptions
    {
        public DefaultRolesCouldNotBeModified() : base() { }
        public DefaultRolesCouldNotBeModified(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение об ошибке, когда пытаются обновить несуществующую строку справочника в БД
    /// </summary>
    public class DictionaryRecordWasNotFound : DmsExceptions
    {
        public DictionaryRecordWasNotFound() : base() { }
        public DictionaryRecordWasNotFound(System.Exception ex) : base(ex) { }
    }


    public class DictionaryContactTypeNameNotUnique : DmsExceptions
    {
        public DictionaryContactTypeNameNotUnique(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionaryContactTypeNameNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryContactTypeCodeNotUnique : DmsExceptions
    {
        public DictionaryContactTypeCodeNotUnique(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionaryContactTypeCodeNotUnique(System.Exception ex) : base(ex) { }
    }
    public class DictionaryAgentAccountNumberNotUnique : DmsExceptions
    {
        public DictionaryAgentAccountNumberNotUnique(string AccountNumber) : base() { Parameters = new List<string> { AccountNumber }; }
        public DictionaryAgentAccountNumberNotUnique(System.Exception ex) : base(ex) { }
    }
    public class DictionaryAddressTypeNameNotUnique : DmsExceptions
    {
        public DictionaryAddressTypeNameNotUnique(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionaryAddressTypeNameNotUnique(System.Exception ex) : base(ex) { }
    }
    public class DictionaryAddressTypeCodeNotUnique : DmsExceptions
    {
        public DictionaryAddressTypeCodeNotUnique(string Code) : base() { Parameters = new List<string> { Code }; }
        public DictionaryAddressTypeCodeNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryAddressTypeNotUnique : DmsExceptions
    {
        public DictionaryAddressTypeNotUnique(string Code) : base() { Parameters = new List<string> { Code }; }
        public DictionaryAddressTypeNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryAddressNameNotUnique : DmsExceptions
    {
        public DictionaryAddressNameNotUnique(string PostCode, string Address) : base() { Parameters = new List<string> { PostCode, Address }; }
        public DictionaryAddressNameNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке добавить контакт агенту с и  имя агента
    /// </summary>
    public class DictionaryContactNotUnique : DmsExceptions
    {
        public DictionaryContactNotUnique(string value) : base() { Parameters = new List<string> { value }; }
        public DictionaryContactNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryContactTypeNotUnique : DmsExceptions
    {
        public DictionaryContactTypeNotUnique(string AgentName, string Value) : base() { Parameters = new List<string> { AgentName, Value }; }
        public DictionaryContactTypeNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать паспортные данные физлица
    /// </summary>
    public class DictionaryAgentPersonPassportNotUnique : DmsExceptions
    {
        public DictionaryAgentPersonPassportNotUnique(string PassportSerial, int? PassportNumber) : base() { Parameters = new List<string> { PassportSerial, PassportNumber?.ToString() }; }
        public DictionaryAgentPersonPassportNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН физлица
    /// </summary>
    public class DictionaryAgentPersonTaxCodeNotUnique : DmsExceptions
    {
        public DictionaryAgentPersonTaxCodeNotUnique(string TaxCode) : base() { Parameters = new List<string> { TaxCode }; }
        public DictionaryAgentPersonTaxCodeNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать паспортные данные человека
    /// </summary>
    public class DictionaryAgentPeoplePassportNotUnique : DmsExceptions
    {
        public DictionaryAgentPeoplePassportNotUnique(string PassportSerial, int? PassportNumber) : base() { Parameters = new List<string> { PassportSerial, PassportNumber?.ToString() }; }
        public DictionaryAgentPeoplePassportNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН сотрудника
    /// </summary>
    public class DictionaryAgentEmployeeTaxCodeNotUnique : DmsExceptions
    {
        public DictionaryAgentEmployeeTaxCodeNotUnique(string TaxCode) : base() { Parameters = new List<string> { TaxCode }; }
        public DictionaryAgentEmployeeTaxCodeNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН сотрудника
    /// </summary>
    public class DictionaryAgentEmployeePersonnelNumberNotUnique : DmsExceptions
    {
        public DictionaryAgentEmployeePersonnelNumberNotUnique(int PersonnelNumber) : base() { Parameters = new List<string> { PersonnelNumber.ToString() }; }
        public DictionaryAgentEmployeePersonnelNumberNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН сотрудника
    /// </summary>
    public class DictionaryAgentBankMFOCodeNotUnique : DmsExceptions
    {
        public DictionaryAgentBankMFOCodeNotUnique(string Name, string MFOCode) : base() { Parameters = new List<string> { Name, MFOCode }; }
        public DictionaryAgentBankMFOCodeNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН юрлица
    /// </summary>
    public class DictionaryAgentCompanyTaxCodeNotUnique : DmsExceptions
    {
        public DictionaryAgentCompanyTaxCodeNotUnique(string Name, string TaxCode) : base() { Parameters = new List<string> { Name, TaxCode }; }
        public DictionaryAgentCompanyTaxCodeNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать VATCode юрлица
    /// </summary>
    public class DictionaryAgentCompanyVATCodeNotUnique : DmsExceptions
    {
        public DictionaryAgentCompanyVATCodeNotUnique(string Name, string VATCode) : base() { Parameters = new List<string> { Name, VATCode }; }
        public DictionaryAgentCompanyVATCodeNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать VATCode юрлица
    /// </summary>
    public class DictionaryAgentCompanyOKPOCodeNotUnique : DmsExceptions
    {
        public DictionaryAgentCompanyOKPOCodeNotUnique(string Name, string OKPOCode) : base() { Parameters = new List<string> { Name, OKPOCode }; }
        public DictionaryAgentCompanyOKPOCodeNotUnique(System.Exception ex) : base(ex) { }
    }



    public class DictionaryTagNotFoundOrUserHasNoAccess : DmsExceptions
    {
        public DictionaryTagNotFoundOrUserHasNoAccess() : base() { }
        public DictionaryTagNotFoundOrUserHasNoAccess(System.Exception ex) : base(ex) { }
    }
    public class DictionaryCustomDictionaryNotUnique : DmsExceptions
    {
        public DictionaryCustomDictionaryNotUnique(string Code) : base() { Parameters = new List<string> { Code }; }
        public DictionaryCustomDictionaryNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryCustomDictionaryTypeNotUnique : DmsExceptions
    {
        public DictionaryCustomDictionaryTypeNotUnique(string Code) : base() { Parameters = new List<string> { Code }; }
        public DictionaryCustomDictionaryTypeNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionarysdDepartmentNotBeSubordinated : DmsExceptions
    {
        public DictionarysdDepartmentNotBeSubordinated(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionarysdDepartmentNotBeSubordinated(System.Exception ex) : base(ex) { }
    }

    public class DictionaryDepartmentNameNotUnique : DmsExceptions
    {
        public DictionaryDepartmentNameNotUnique(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionaryDepartmentNameNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryDocumentSubjectNameNotUnique : DmsExceptions
    {
        public DictionaryDocumentSubjectNameNotUnique(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionaryDocumentSubjectNameNotUnique(System.Exception ex) : base(ex) { }
    }
    public class DictionaryDocumentTypeNameNotUnique : DmsExceptions
    {
        public DictionaryDocumentTypeNameNotUnique(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionaryDocumentTypeNameNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryPositionExecutorNotUnique : DmsExceptions
    {//Сотрудник \"{1}\" не может быть назначен на должность повторно \"{0}\" c {2} по {3}
        public DictionaryPositionExecutorNotUnique(string PositionName, string AgentName, string StartDate, string EndDate) : base()
        { Parameters = new List<string> { PositionName, AgentName, StartDate, EndDate }; }
        public DictionaryPositionExecutorNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryPositionExecutorIsInvalidPeriod : DmsExceptions
    {
        public DictionaryPositionExecutorIsInvalidPeriod() : base() { }
        public DictionaryPositionExecutorIsInvalidPeriod(System.Exception ex) : base(ex) { }
    }

    public class DictionaryPositionExecutorEventExists : DmsExceptions
    {
        public DictionaryPositionExecutorEventExists() : base() { }
        public DictionaryPositionExecutorEventExists(System.Exception ex) : base(ex) { }
    }

    public class DictionaryPositionExecutorPersonalNotUnique : DmsExceptions
    {//На должность \"{0}\" штатно назначен \"{1}\" c {2} по {3}
        public DictionaryPositionExecutorPersonalNotUnique(string PositionName, string AgentName, string StartDate, string EndDate) : base()
        { Parameters = new List<string> { PositionName, AgentName, StartDate, EndDate }; }
        public DictionaryPositionExecutorPersonalNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryPositionExecutorIONotUnique : DmsExceptions
    {
        public DictionaryPositionExecutorIONotUnique(string PositionName, string AgentName, string StartDate, string EndDate) : base()
        { Parameters = new List<string> { PositionName, AgentName, StartDate, EndDate }; }
        public DictionaryPositionExecutorIONotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryPositionExecutorReferentNotUnique : DmsExceptions
    {
        public DictionaryPositionExecutorReferentNotUnique(string PositionName, string AgentName, string StartDate, string EndDate) : base()
        { Parameters = new List<string> { PositionName, AgentName, StartDate, EndDate }; }
        public DictionaryPositionExecutorReferentNotUnique(System.Exception ex) : base(ex) { }
    }


    public class DictionaryRegistrationJournalNotUnique : DmsExceptions
    {
        public DictionaryRegistrationJournalNotUnique(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionaryRegistrationJournalNotUnique(System.Exception ex) : base(ex) { }
    }
    public class DictionaryStandartSendListNotUnique : DmsExceptions
    {
        public DictionaryStandartSendListNotUnique(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionaryStandartSendListNotUnique(System.Exception ex) : base(ex) { }
    }
    public class DictionaryStandartSendListContentNotUnique : DmsExceptions
    {
        public DictionaryStandartSendListContentNotUnique(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionaryStandartSendListContentNotUnique(System.Exception ex) : base(ex) { }
    }

    public class DictionaryTagNotUnique : DmsExceptions
    {
        public DictionaryTagNotUnique(string Name) : base() { Parameters = new List<string> { Name }; }
        public DictionaryTagNotUnique(System.Exception ex) : base(ex) { }
    }
    #endregion

    #region [+] Admin ...
    /// <summary>
    /// Сообщение об ошибке, когда при добавлении или изменении строки появится дубль
    /// </summary>
    public class AdminRecordNotUnique : DmsExceptions
    {
        public AdminRecordNotUnique() : base() { }
        public AdminRecordNotUnique(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение об ошибке, когда невозможно добавить данные в админку
    /// </summary>
    public class AdminRecordCouldNotBeAdded : DmsExceptions
    {
        public AdminRecordCouldNotBeAdded() : base() { }
        public AdminRecordCouldNotBeAdded(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение о невозможности удаления записи в админке
    /// </summary>
    public class AdminRecordCouldNotBeDeleted : DmsExceptions
    {
        public AdminRecordCouldNotBeDeleted() : base() { }
        public AdminRecordCouldNotBeDeleted(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Сообщение об ошибке, когда пытаются обновить несуществующую строку справочника в БД
    /// </summary>
    public class AdminRecordWasNotFound : DmsExceptions
    {
        public AdminRecordWasNotFound() : base() { }
        public AdminRecordWasNotFound(System.Exception ex) : base(ex) { }
    }
    #endregion
    public class RecordNotUnique : DmsExceptions
    {
        public RecordNotUnique() : base() { }
        public RecordNotUnique(System.Exception ex) : base(ex) { }
    }
    public class IncomingModelIsNotValid : DmsExceptions
    {
        public IncomingModelIsNotValid(string Text) : base() { Parameters = new List<string> { Text }; }
        public IncomingModelIsNotValid(System.Exception ex) : base(ex) { }
    }
    public class NotFilledWithAdditionalRequiredAttributes : DmsExceptions
    {
        public NotFilledWithAdditionalRequiredAttributes() : base() { }
        public NotFilledWithAdditionalRequiredAttributes(System.Exception ex) : base(ex) { }
    }
    public class SigningTypeNotAllowed : DmsExceptions
    {
        public SigningTypeNotAllowed() : base() { }
        public SigningTypeNotAllowed(System.Exception ex) : base(ex) { }
    }
    public class EncryptionCertificateWasNotFound : DmsExceptions
    {
        public EncryptionCertificateWasNotFound() : base() { }
        public EncryptionCertificateWasNotFound(System.Exception ex) : base(ex) { }
    }

    public class EncryptionCertificateDigitalSignatureFailed : DmsExceptions
    {
        public EncryptionCertificateDigitalSignatureFailed() : base() { }
        public EncryptionCertificateDigitalSignatureFailed(System.Exception ex) : base(ex) { }
    }

    public class EncryptionCertificateHasExpired : DmsExceptions
    {
        public EncryptionCertificateHasExpired() : base() { }
        public EncryptionCertificateHasExpired(System.Exception ex) : base(ex) { }
    }

    public class EncryptionCertificatePrivateKeyСanNotBeExported : DmsExceptions
    {
        public EncryptionCertificatePrivateKeyСanNotBeExported() : base() { }
        public EncryptionCertificatePrivateKeyСanNotBeExported(System.Exception ex) : base(ex) { }
    }

    /// <summary>
    /// Ошибка возникает, если в полнотекстовом поиске введена комбинация, возвращающая много результатов
    /// </summary>
    public class SystemFullTextTooManyResults : DmsExceptions
    {
        public SystemFullTextTooManyResults(string word) : base() { Parameters = new List<string> { word }; }
        public SystemFullTextTooManyResults(System.Exception ex) : base(ex) { }
    }
    public class WrongCasheKey : DmsExceptions
    {
        public WrongCasheKey() : base() { }
        public WrongCasheKey(System.Exception ex) : base(ex) { }
    }

    //public class LockoutAgentUser : DmsExceptions
    //{
    //    private const string  = "##l@DmsExceptions:LockoutAgentUser@l##";
    //    public LockoutAgentUser() : base() { }
    //    public LockoutAgentUser(System.Exception ex) : base(ex) { }
    //}
}