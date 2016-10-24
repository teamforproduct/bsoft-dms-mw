namespace BL.Model.Enums
{
    public enum EnumSystemActions
    {
        Login = 0, //Вход в систему
        SetSetting = 900001, // Добавить динамический аттрибут

        // При добавлении действия не забудь добавить действие в ImportData: BL.Database.DatabaseContext.DmsDbImportData!!!
    }
}