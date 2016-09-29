namespace BL.Model.Enums
{
    public enum EnumCopyMode
    {
        /// <summary>
        /// Копирует без учета существующей настроки (перетирание)
        /// </summary>
        Сoverage = 1,

        /// <summary>
        /// Копирует с сохранением текущей настройки (объединение)
        /// </summary>
        Сombination = 2
    }
}