namespace BL.Model.Enums
{
    public enum EnumEventTypes
    {
        AddNewDocument = 110, //Создан проект
        ControlOn = 300,//Взят на контроль
        ControlChange = 310,//Изменить контроль
        ControlOff = 301,//Снят с контроля
        AddNote = 600,//Примечание
        SetInWork = 998,//	Работа возобновлена
        SetOutWork = 999,//	Работа завершена
        SendMessage = 500,//	Направлено сообщение
        ChangeExecutor = 205,// Изменение исполнителя по документу
    }
}