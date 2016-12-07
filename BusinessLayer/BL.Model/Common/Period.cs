using BL.Model.Enums;
using System;

namespace BL.Model.Common
{
    /// <summary>
    /// Перид - временной интервал
    /// </summary>
    public class Period

    {
        private DateTime? _DateBeg;
        private DateTime? _DateEnd;
        PeriodTypes _Type;


        public Period()
        { }

        public Period(DateTime DateBeg, DateTime DateEnd)
        {
            this._DateBeg = DateBeg;
            this._DateEnd = DateEnd;
        }

        /// <summary>
        /// Тип периода
        /// </summary>
        public PeriodTypes Type
        {
            set { _Type = value; }
            get { return _Type; }
        }

        /// <summary>
        /// Дата начала периода
        /// </summary>
        public DateTime? DateBeg
        {
            set { _DateBeg = value; }
            get { return _DateBeg; }
        }

        /// <summary>
        /// Дата окончания периода
        /// </summary>
        public DateTime? DateEnd
        {
            set { _DateEnd = value; }
            get { return _DateEnd; }
        }

        /// <summary>
        /// Период задан
        /// </summary>
        public bool HasValue => _DateBeg != null && _DateEnd != null;
    }
}
