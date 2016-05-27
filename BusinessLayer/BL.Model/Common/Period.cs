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
        public bool IsActive
        {
            get { return _DateBeg != null && _DateEnd != null; }
        }


        /*
        
        ' Вычисление квартала по дате.
Public Function GetQuarter(Optional WorkDate As Variant) As Long
Dim l_Date As Date
l_Date = GetCurDate(WorkDate)

Dim l_P  As Double

l_P = Month(l_Date) / 3

If l_P <= 1 Then
  GetQuarter = 1
ElseIf l_P > 1 And l_P <= 2 Then
  GetQuarter = 2
ElseIf l_P > 2 And l_P <= 3 Then
  GetQuarter = 3
Else
  GetQuarter = 4
End If
End Function

' Определение первого дня текущей недели.
Public Function GetFirstOfWeek(Optional WorkDate As Variant, Optional FirstWeekday As VbDayOfWeek = vbUseSystemDayOfWeek) As Date
Dim l_Date As Date
l_Date = GetCurDate(WorkDate)

GetFirstOfWeek = l_Date - Weekday(l_Date, FirstWeekday) + 1
End Function


' Определение последнего дня текущей недели.
Public Function GetLastOfWeek(Optional WorkDate As Variant, Optional FirstWeekday As VbDayOfWeek = vbUseSystemDayOfWeek) As Date
Dim l_Date As Date
l_Date = GetCurDate(WorkDate)

GetLastOfWeek = l_Date - Weekday(l_Date, FirstWeekday) + 7
End Function

' Определение первого дня месяца.
Function GetFirstOfMonth(Optional WorkDate As Variant) As Date
Dim l_Date As Date
l_Date = GetCurDate(WorkDate)

GetFirstOfMonth = DateSerial(Year(l_Date), Month(l_Date), 1)
End Function

' Определение последнего дня месяца.
Function GetLastOfMonth(Optional WorkDate As Variant) As Date
Dim l_Date As Date
l_Date = GetCurDate(WorkDate)

' Первый день следующего месяца, и вычитается один день
GetLastOfMonth = DateSerial(Year(l_Date), Month(l_Date) + 1, 1) - 1
End Function

' Определение первого дня предыдущего месяца.
Public Function GetFirstOfPreviousMonth(Optional WorkDate As Variant) As Date
Dim l_Date As Date
l_Date = GetCurDate(WorkDate)

GetFirstOfPreviousMonth = DateSerial(Year(l_Date), Month(l_Date) - 1, 1)
End Function

' Определение последнего дня предыдущего месяца.
Public Function GetLastOfPreviousMonth(Optional WorkDate As Variant) As Date
Dim l_Date As Date
l_Date = GetCurDate(WorkDate)

GetLastOfPreviousMonth = DateSerial(Year(l_Date), Month(l_Date), 0)
End Function


' Определение первого дня текущего квартала.
Public Function GetFirstOfQuarter(Optional WorkDate As Variant) As Date
Dim l_Date As Date
l_Date = GetCurDate(WorkDate)

GetFirstOfQuarter = DateSerial(Year(l_Date), Int((Month(l_Date) - 1) / 3) * 3 + 1, 1)
End Function

' Определение последнего дня текущего квартала.
Public Function GetLastOfQuarter(Optional WorkDate As Variant) As Date
Dim l_Date As Date
l_Date = GetCurDate(WorkDate)

GetLastOfQuarter = DateSerial(Year(l_Date), Int((Month(l_Date) - 1) / 3) * 3 + 4, 0)
End Function

' Установка квартала.
Public Sub SetQuarterNum(QuarterNum As Integer)


DEBUG_PRINT_PROC "BSFT_COMM_PERIOD " + l_Info + " SET_QRT" ' Procedure Log.

d(2).Value = d(1).Value
'd(2).Year = d(1).Year
d(1).Value = PeriodBeg(2, Val(QuarterNum))
d_2__Value = PeriodEnd(2, Val(QuarterNum))
End Sub

Public Sub SetQuarter(Optional WorkDate As Variant)
d(1).Value = GetFirstOfQuarter(WorkDate)
d_2__Value = GetLastOfQuarter(WorkDate)
End Sub

' Установка месяца.
Public Sub SetMonthNum(MonthNum As Integer)


DEBUG_PRINT_PROC "BSFT_COMM_PERIOD " + l_Info + " SET_MNTH" ' Procedure Log.

d(2).Value = d(1).Value
'd(2).Year = d(1).Year
d(1).Value = PeriodBeg(3, Val(MonthNum))
d_2__Value = PeriodEnd(3, Val(MonthNum))
End Sub

Public Sub SetMonth(Optional WorkDate As Variant)
d(1).Value = GetFirstOfMonth(WorkDate)
d_2__Value = GetLastOfMonth(WorkDate)
End Sub

Private Property Let d_2__Value(ByVal dt As Date)
d(2).Value = Int(dt) + 0.99999
End Property

Private Sub BeforeSetPeriod()
If Not Active Then
  Active = True
End If

ONDATE_OFF
End Sub

Private Function GetCurDate(WorkDate As Variant) As Date
If IsNull(WorkDate) Or Not IsDate(WorkDate) Then
  GetCurDate = Int(CurDate) ' DateTime.Date
Else
  GetCurDate = Int(CDate(WorkDate))
End If
End Function

Public Sub SetCurDay(Optional WorkDate As Variant)
BeforeSetPeriod

d(1).Value = GetCurDate(WorkDate)
d_2__Value = GetCurDate(WorkDate)
End Sub

Public Sub SetYesterday(Optional WorkDate As Variant)
BeforeSetPeriod

d(1).Value = GetCurDate(WorkDate) - 1
d_2__Value = GetCurDate(WorkDate) - 1

End Sub

Public Sub SetCurWeek(Optional WorkDate As Variant)
BeforeSetPeriod

d(1).Value = GetFirstOfWeek(WorkDate)
d_2__Value = GetLastOfWeek(WorkDate)

End Sub

Public Sub SetWeekAgo(Optional WorkDate As Variant)
BeforeSetPeriod

d(1).Value = GetCurDate(WorkDate) - 7
d_2__Value = GetCurDate(WorkDate)

End Sub

Public Sub SetLastWeek(Optional WorkDate As Variant)
BeforeSetPeriod

d(1).Value = GetFirstOfWeek(WorkDate) - 7
d_2__Value = GetLastOfWeek(WorkDate) - 7

End Sub

Public Sub SetLastTwoWeek(Optional WorkDate As Variant)
BeforeSetPeriod

d(1).Value = GetFirstOfWeek(WorkDate) - 14
d_2__Value = GetLastOfWeek(WorkDate) - 7

End Sub

Public Sub SetCurMonth(Optional WorkDate As Variant)
  SetMonthNum Month(GetCurDate(WorkDate))
End Sub

Public Sub SetLastMonth(Optional WorkDate As Variant)

BeforeSetPeriod

d(1).Value = GetFirstOfPreviousMonth
d_2__Value = GetLastOfPreviousMonth

End Sub

Public Sub SetTwoMonth(Optional WorkDate As Variant)
Dim l_Date As Date

l_Date = GetCurDate(WorkDate)

BeforeSetPeriod

d(1).Value = DateSerial(Year(l_Date), Month(l_Date) - 1, 1)
d_2__Value = DateSerial(Year(l_Date), Month(l_Date) + 1, 0)

End Sub

Public Sub SetThreeMonth(Optional WorkDate As Variant)
Dim l_Date As Date

l_Date = GetCurDate(WorkDate)

BeforeSetPeriod

d(1).Value = DateSerial(Year(l_Date), Month(l_Date) - 2, 1)
d_2__Value = DateSerial(Year(l_Date), Month(l_Date) + 1, 0)

End Sub

Public Sub SetCurYear(Optional WorkDate As Variant)
Dim l_Date As Date

l_Date = GetCurDate(WorkDate)

BeforeSetPeriod

d(1).Value = DateSerial(Year(l_Date), 1, 1)
d_2__Value = DateSerial(Year(l_Date) + 1, 1, 0)

End Sub


Public Sub SetCurQuarter(Optional WorkDate As Variant)
Dim l_Date As Date

l_Date = GetCurDate(WorkDate)

BeforeSetPeriod

' Устанавливаю текущий квартал (текущего года).
d(1).Value = DateSerial(Year(l_Date), 1, 1)
d_2__Value = DateSerial(Year(l_Date), 12, 31)

SetQuarterNum GetQuarter(l_Date)

End Sub

Public Sub SetYearNum(YearNum As Long)

BeforeSetPeriod

d(1).Value = PeriodBeg(1, YearNum)
d_2__Value = PeriodEnd(1, YearNum)

End Sub

Public Sub SetBegMonth()
BeforeSetPeriod
d(1).Value = DateSerial(Year(d(1).Value), Month(d(1).Value), 1)
End Sub

Public Sub SetBegQuarter()
BeforeSetPeriod
d(1).Value = DateSerial(Year(d(1).Value), (Month(d(1).Value) \ 3) * 3 + 1, 1)
End Sub


Public Sub SetBegYear()
BeforeSetPeriod
d(1).Value = DateSerial(Year(d(1).Value), 1, 1)
End Sub

Public Sub SetEndMonth()
BeforeSetPeriod
d_2__Value = DateSerial(Year(d(2)), Month(d(2)) + 1, 0)
End Sub

Public Sub SetEndQuarter()
BeforeSetPeriod
d_2__Value = DateSerial(Year(d(2)), (Month(d(2)) \ 3) * 3 + 3 + 1, 0)
End Sub


Public Sub SetEndYear()
BeforeSetPeriod
d_2__Value = DateSerial(Year(d(2)), 12, 31)
End Sub


        */



    }
}
