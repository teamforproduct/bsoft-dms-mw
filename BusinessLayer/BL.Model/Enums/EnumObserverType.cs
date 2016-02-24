namespace BL.Model.Enums
{
    /// <summary>
    /// тип наблюдателя за коммандами. 
    /// </summary>
    public enum EnumObserverType
    {
        /// <summary>
        /// Должен выполняться ДО команды
        /// </summary>
        Before = 0,

        /// <summary>
        /// должнен выполняться ПОСЛЕ команды
        /// </summary>
        After = 1
    }
}