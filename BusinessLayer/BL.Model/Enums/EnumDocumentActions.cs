namespace BL.Model.Enums
{
    public enum EnumDocumentActions
    {
        AddDocument = 11, // Создать документ по шаблону
        CopyDocument = 13, // Создать документ копированием
        ModifyDocument = 15, // Изменить документ
        DeleteDocument = 17, // Удалить документ
        ChangeExecutor = 18, // Передать управление
        RegisterDocument = 19, // Зарегистрировать документ
        ControlOn = 21, // Взять на контроль
        ControlChange = 23, // Изменить параметры контроля
        ControlOff = 25, // Снять с контроля
        SendMessage = 51, // Направить сообщение участникам рабочей группы
        AddNote = 53, // Добавить примечание
        AddFavourite = 55, // Добавить в избранное
        DeleteFavourite = 57, // Удалить из избранного
        FinishWork = 95, // Закончить работу с документом
        StartWork = 97, // Возобновить работу с документом
    }
}