﻿namespace BL.Model.Enums
{
    public enum EnumPropertyActions
    {
        AddProperty = 311001, // Добавить динамический аттрибут
        ModifyProperty = 311005, // Изменить динамический аттрибут
        DeleteProperty = 311009, // Удалить динамический аттрибут
        AddPropertyLink = 312001, // Добавить связь динамических аттрибутов
        ModifyPropertyLink = 312005, // Изменить связь динамических аттрибутов
        DeletePropertyLink = 312009, // Удалить связь динамических аттрибутов

        ModifyPropertyValues = 313005

        // При добавлении действия не забудь добавить действие в ImportData: BL.Database.DatabaseContext.DmsDbImportData!!!
    }
}