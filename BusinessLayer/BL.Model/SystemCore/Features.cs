namespace BL.Model.SystemCore
{
    public static class Features
    {   
        public const string AccessList = "AccessList";
        public const string Accounts = "Accounts";
        public const string Addresses = "Addresses";
        public const string AddressTypes = "AddressTypes";
        public const string Admins = "Admins";
        public const string Assignments = "Assignments";
        public const string AuthLog = "AuthLog";
        public const string ChangeLogin = "ChangeLogin";
        public const string ChangePassword = "ChangePassword";
        public const string Companies = "Companies";
        public const string ContactPersons = "ContactPersons";
        public const string Contacts = "Contacts";
        public const string ContactTypes = "ContactTypes";
        public const string Contents = "Contents";
        public const string ControlQuestions = "ControlQuestions";
        public const string ControlQuestion = "ControlQuestion";
        public const string Departments = "Departments";
        public const string DocumentTypes = "DocumentTypes";
        public const string Employees = "Employees";
        public const string Executors = "Executors";
        public const string Files = "Files";
        public const string FilePdf = "FilePdf";
        public const string FilePreview = "FilePreview";
        public const string Fingerprints = "Fingerprints";
        public const string Image = "Image";
        public const string Info = "Info";
        public const string Journals = "Journals";
        public const string Notifications = "Notifications";
        public const string Papers = "Papers";
        public const string Parameters = "Parameters";
        public const string Passport = "Passport";
        public const string Permissions = "Permissions";
        public const string Persons = "Persons";
        public const string Plan = "Plan";
        public const string Positions = "Positions";
        public const string Roles = "Roles";
        public const string SendRules = "SendRules";
        public const string Sessions = "Sessions";
        public const string Settings = "Settings";
        public const string SignCertificates = "SignCertificates";
        public const string SignLists = "SignLists";
        public const string Tags = "Tags";
        public const string Tasks = "Tasks";
        public const string TempFileStorage = "TempFileStorage";


        public const string AccessLevels = "AccessLevels";
        public const string Actions = "Actions";
        public const string AssignmentTypes = "AssignmentTypes";
        public const string DocumentDirections = "DocumentDirections";
        public const string EventTypes = "EventTypes";
        public const string Formats = "Formats";
        public const string Formulas = "Formulas";
        public const string ImportanceEventTypes = "ImportanceEventTypes";
        public const string Languages = "Languages";
        public const string LinkTypes = "LinkTypes";
        public const string Objects = "Objects";
        public const string Patterns = "Patterns";
        public const string ResultTypes = "ResultTypes";
        public const string SendTypes = "SendTypes";
        public const string SubordinationTypes = "SubordinationTypes";
        public const string StageTypes = "StageTypes";
        public const string ValueTypes = "ValueTypes";


        public const string Events = "Events";
        public const string DocumentAccesses = "DocumentAccesses";
        public const string Links = "Links";
        public const string SavedFilters = "SavedFilters";
        public const string Signs = "Signs";
        public const string Waits = "Waits";
        public const string WorkGroups = "WorkGroups";
        public const string Favourite = "Favourite";

        public const string OnlineUsers = "OnlineUsers";

        public static int GetId(string Name)
        {
            int res = 0;

            switch (Name)
            {
                case AccessList:            res = 1000; break;
                case Addresses:             res = 1010; break;
                case AddressTypes:          res = 1020; break;
                case Admins:                res = 1030; break;
                case Assignments:           res = 1040; break;
                case AuthLog:               res = 1050; break;
                case ChangeLogin:           res = 1060; break;
                case ChangePassword:        res = 1070; break;
                case Companies:             res = 1080; break;
                case ContactPersons:        res = 1090; break;
                case Contacts:              res = 1100; break;
                case ContactTypes:          res = 1110; break;
                case Contents:              res = 1120; break;
                case Departments:           res = 1130; break;
                case DocumentTypes:         res = 1140; break;
                case Employees:             res = 1150; break;
                case Executors:             res = 1160; break;
                case Files:                 res = 1170; break;
                case Image:                 res = 1180; break;
                case Info:                  res = 1190; break;
                case Journals:              res = 1200; break;
                case Papers:                res = 1210; break;
                case Parameters:            res = 1220; break;
                case Passport:              res = 1230; break;
                case Permissions:           res = 1240; break;
                case Persons:               res = 1250; break;
                case Plan:                  res = 1260; break;
                case Positions:             res = 1270; break;
                case Roles:                 res = 1280; break;
                case SendRules:             res = 1290; break;
                case Sessions:              res = 1300; break;
                case Settings:              res = 1310; break;
                case SignCertificates:      res = 1320; break;
                case SignLists:             res = 1330; break;
                case Tags:                  res = 1340; break;
                case Tasks:                 res = 1350; break;
                case TempFileStorage:       res = 1360; break;
                case AccessLevels:          res = 1370; break;
                case Actions:               res = 1380; break;
                case AssignmentTypes:       res = 1390; break;
                case DocumentDirections:    res = 1400; break;
                case EventTypes:            res = 1410; break;
                case Formats:               res = 1420; break;
                case Formulas:              res = 1430; break;
                case ImportanceEventTypes:  res = 1440; break;
                case Languages:             res = 1450; break;
                case LinkTypes:             res = 1460; break;
                case Objects:               res = 1470; break;
                case Patterns:              res = 1480; break;
                case ResultTypes:           res = 1490; break;
                case SendTypes:             res = 1500; break;
                case SubordinationTypes:    res = 1510; break;
                case ValueTypes:            res = 1520; break;

                case Events:                res = 1530; break;
//                case Accesses:              res = 1540; break;
                case DocumentAccesses:      res = 1550; break;
                case Links:                 res = 1560; break;
                case SavedFilters:          res = 1570; break;
                case Signs:                 res = 1580; break;
                case Waits:                 res = 1590; break;
                case WorkGroups:            res = 1600; break;
                case Favourite:             res = 1610; break;

                case OnlineUsers:           res = 1620; break;
                case Accounts:              res = 1630; break;

                default: throw new System.Exception();
            }

            return res;
        }
        
    }
}