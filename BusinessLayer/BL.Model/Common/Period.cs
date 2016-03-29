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

        public PeriodTypes Type
        {
            set { _Type = value; }
            get { return _Type; }
        }

        public DateTime? DateBeg
        {
            set { _DateBeg = value; }
            get { return _DateBeg; }
        }

        public DateTime? DateEnd
        {
            set { _DateEnd = value; }
            get { return _DateEnd; }
        }

        public bool IsActive
        {
            get { return _DateBeg != null && _DateEnd != null; }
        }
    }
}
