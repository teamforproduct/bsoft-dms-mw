namespace BL.Model.Enums
{
    public enum PeriodTypes
    {
        //  Все положительные типы сохраняются в хранимых параметрах.
        
        /// <summary>
        /// Рабочая дата
        /// </summary>
        CurrentDate = 10,

        /// <summary>
        /// Текущая неделя относительно рабочей даты
        /// </summary>
        CurrentWeek = 20,

        /// <summary>
        /// Текущий месяц относительно рабочей даты
        /// </summary>
        CurrentMonth = 30,

        /// <summary>
        /// Текущий квартал относительно рабочей даты
        /// </summary>
        CurrentQuarter = 40,

        /// <summary>
        /// Текущий год относительно рабочей даты
        /// </summary>
        CurrentYear = 50,

        /// <summary>
        /// Вчера относительно рабочей даты
        /// </summary>
        Yesterday = 90,

        /// <summary>
        /// Последняя неделя относительно рабочей даты
        /// </summary>
        LastWeek = 100,

        /// <summary>
        /// Последние две недели относительно рабочей даты
        /// </summary>
        LastTwoWeek = 110,

        /// <summary>
        /// Неделю назад относительно рабочей даты
        /// </summary>
        WeekAgo = 120,

        /// <summary>
        /// Последний месяц относительно рабочей даты
        /// </summary>
        LastMonth = 160,

        /// <summary>
        /// Два месяца относительно рабочей даты (текущий месяц до конца и минус два месяца)
        /// </summary>
        TwoMonth = 170,

        /// <summary>
        /// Три месяца относительно рабочей даты (текущий месяц до конца и минус три месяца)
        /// </summary>
        ThreeMonth = 180,


        /// <summary>
        /// На начало месяца относительно текущей даты
        /// </summary>
        BegMonth = -410,

        /// <summary>
        /// На начало квартала относительно текущей даты
        /// </summary>
        BegQuarter = -420,

        /// <summary>
        /// На начало года относительно текущей даты
        /// </summary>
        BegYear = -430,


        /// <summary>
        /// На конец месяца относительно текущей даты
        /// </summary>
        EndMonth = -450,

        /// <summary>
        ///  На конец квартала относительно текущей даты
        /// </summary>
        EndQuarter = -460,

        /// <summary>
        ///  На конец года относительно текущей даты
        /// </summary>
        EndYear = -470,


        /// <summary>
        ///  Нет периода
        /// </summary>
        NoPeriod = -500

    }
}