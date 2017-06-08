namespace BL.Model.Enums
{
    /// <summary>
    /// Типы доступов к событию
    /// </summary>
    public enum EnumEventAccessTypes
    {
        /// <summary>
        /// Отправитель
        /// </summary>
        Source = 10,
        /// <summary>
        /// Отправитель
        /// </summary>
        Controller = 15,

        /// <summary>
        /// Получатель
        /// </summary>
        Target = 20,

        /// <summary>
        /// Копия
        /// </summary>
        TargetCopy = 30,

        /// <summary>
        /// Досылка
        /// </summary>
        TargetAdditional = 40,

    }
}