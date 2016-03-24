using System;



namespace BL.Database.Common
{
    static public class CommonFilterUtilites
    {
        static public String[] GetWhereExptessions(string SearchExpression)
        {
            string res = SearchExpression.Trim();
             
            while (res.Contains("  "))
            {
                res =  res.Replace("  ", " ");
            }

            return res.Split(' ');
        }

//Public Function GET_TXT_WH(FieldName As String, Text As String)
//Dim l_Text As String
//Dim l_s As String
//Dim ArrOr() As String
//Dim ArrAnd() As String
//Dim i As Long
//Dim j As Long

//If FieldName = "" Then Exit Function

//l_Text = Text
//l_Text = Trim(l_Text)

//If l_Text = "" Then Exit Function

//Do While InStr(l_Text, "  ") > 0
//  l_Text = Replace(l_Text, "  ", " ")
//Loop

//Do While InStr(l_Text, ",,") > 0
//  l_Text = Replace(l_Text, ",,", ",")
//Loop


//'l_Text = Replace(l_Text, "?", "_")
//'l_Text = Replace(l_Text, "*", "%")


//ArrOr = Split(l_Text, ",")

//l_Text = ""

//For i = 0 To UBound(ArrOr)
//  ArrAnd = Split(Trim(ArrOr(i)), " ")
  
//  l_s = ""
//  For j = 0 To UBound(ArrAnd)
//    If Trim(ArrAnd(j)) <> "" Then
//      l_s = l_s + IIf(l_s = "", "", " and ") + "UPPER(" + FieldName + ") like '%" + UCase(ArrAnd(j)) + "%'"
//    End If
//  Next j


//  l_Text = l_Text + IIf(l_Text = "", "", " or ") + "(" + l_s + ")"
  
//Next i


//l_Text = " and (" + l_Text + ")"


//'For i = 0 To UBound(Arr)
//'  l_Text = l_Text + " and UPPER(" + FieldName + ") like '%" + UCase(Arr(i)) + "%' "
//'Next i

//Erase ArrAnd
//Erase ArrOr

//GET_TXT_WH = l_Text

//End Function
    }
}
