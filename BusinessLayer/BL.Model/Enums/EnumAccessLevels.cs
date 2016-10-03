namespace BL.Model.Enums
{
    public enum EnumAccessLevels
    {
        /// <summary>
        /// Только лично
        /// </summary>
        Personally = 10,

        /// <summary>
        /// Лично+референты
        /// </summary>
        PersonallyAndReferents = 20,

        /// <summary>
        /// Лично+референты+ИО
        /// </summary>
        PersonallyAndIOAndReferents = 30

        
       //Внимание есть отличие от старой системы
    }
}