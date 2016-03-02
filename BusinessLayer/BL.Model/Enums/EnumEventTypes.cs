namespace BL.Model.Enums
{
    public enum EnumEventTypes
    {
        AddNewDocument = 110, //Создан проект
        ControlOn = 300,//Взят на контроль
        ControlChange = 310,//Изменить контроль
        ControlOff = 301,//Снят с контроля

        MarkExecution = 320,//Поручение выполнено

        AddNote = 600,//Примечание
        SetInWork = 998,//	Работа возобновлена
        SetOutWork = 999,//	Работа завершена
        SendMessage = 500,//	Направлено сообщение
        ChangeExecutor = 205,// Изменение исполнителя по документу

        SendForInformation = 200,//	Направлен для сведения
        SendForConsideration = 220,//	Направлен для рассмотрения
        SendForVisaing = 250,// Направить для визирования 
        SendForАgreement = 260,// Направить для согласование 
        SendForАpproval = 270,// Направить для утверждения 
        SendForSigning = 280,// Направить для подписи 
        SendForExecution = 210,//   Направлен для исполнения
        SendForControl = 212,//    Направлен для исполнения (на контроль)
        SendForResponsibleExecution = 214,//  Направлен для исполнения (отв. исполнитель)

        LaunchPlan = 800, //   Запущено исполнение плана работы по документу
        StopPlan = 810,// Остановлено исполнение плана работы по документу
    }
}