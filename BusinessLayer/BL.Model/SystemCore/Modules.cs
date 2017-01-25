
namespace BL.Model.SystemCore
{
    public static class Modules
    {
        public const string List = "List";
        public const string User = "User";
        public const string System = "System";
        public const string Org = "Org";
        public const string Department = "Department";
        public const string Position = "Position";
        public const string Person = "Person";
        public const string Employee = "Employee";
        public const string Company = "Company";
        public const string Bank = "Bank";
        public const string Journal = "Journal";
        public const string DocumentType = "DocumentType";
        public const string ContactType = "ContactType";
        public const string AddressType = "AddressType";
        public const string Tags = "Tags";
        public const string Templates = "Templates";
        public const string SendList = "SendList";
        public const string Role = "Role";
        public const string Auth = "Auth";
        public const string Auditlog = "Auditlog";
        public const string Settings = "Settings";
        public const string CustomDictionaries = "CustomDictionaries";
        public const string Tools = "Tools";


        public const string Documents = "Documents";
        public const string PaperList = "PaperList";

        public static int GetId(string Name)
        {
            int res = 0;

            switch (Name)
            {
                case List:                  res = 100; break;
                case User:                  res = 110; break;
                case System:                res = 120; break;
                case Org:                   res = 130; break;
                case Department:            res = 140; break;
                case Position:              res = 150; break;
                case Person:                res = 160; break;
                case Employee:              res = 170; break;
                case Company:               res = 180; break;
                case Bank:                  res = 190; break;
                case Journal:               res = 200; break;
                case DocumentType:          res = 210; break;
                case ContactType:           res = 220; break;
                case AddressType:           res = 230; break;
                case Tags:                  res = 240; break;
                case Templates:             res = 250; break;
                case SendList:              res = 260; break;
                case Role:                  res = 270; break;
                case Auth:                  res = 280; break;
                case Auditlog:              res = 290; break;
                case Settings:              res = 300; break;
                case CustomDictionaries:    res = 310; break;
                case Tools:                 res = 320; break;

                case Documents:              res = 330; break;
                case PaperList:             res = 340; break;

                default: throw new System.Exception();
            }

            return res;
        }

    }
}