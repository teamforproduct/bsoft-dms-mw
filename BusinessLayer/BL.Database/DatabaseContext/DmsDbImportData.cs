using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.System;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.SystemCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Database.DatabaseContext
{
    public static class DmsDbImportData
    {

        private static List<SystemModules> systemModules = new List<SystemModules>();
        private static List<SystemFeatures> systemFeatures = new List<SystemFeatures>();
        private static List<SystemPermissions> systemPermissions = new List<SystemPermissions>();
        private static List<AdminRolePermissions> systemRolePermissions = new List<AdminRolePermissions>();

        private static string GetLabel(string module, string item) => "##l@" + module.Trim() + ":" + item.Trim() + "@l##";
        private static int GetConcatId(params int[] arr) => Convert.ToInt32(string.Join("", arr));
        public static int GetFeatureId(string module, string feature) => GetConcatId(Modules.GetId(module), Features.GetId(feature));
        public static int GetPermissionId(string module, string feature, EnumAccessTypes type) => GetConcatId(Modules.GetId(module), Features.GetId(feature), type.GetHashCode());

        public static List<SystemModules> GetSystemModules() => systemModules;
        public static List<SystemFeatures> GetSystemFeatures() => systemFeatures;
        public static List<SystemPermissions> GetSystemPermissions() => systemPermissions;
        public static List<AdminRolePermissions> GetAdminRolePermissions() => systemRolePermissions;

        private static void AddPermission(int order, string module, string feature, bool r = true, bool c = true, bool u = true, bool d = true,
            List<Roles> readRoles = null, List<Roles> createRoles = null,
            List<Roles> updateRoles = null, List<Roles> deleteRoles = null)
        {
            var m = systemModules.Where(x => x.Code == module).FirstOrDefault();

            if (m == null)
            {
                m = new SystemModules { Id = Modules.GetId(module), Code = module, Name = GetLabel("Modules", module), Order = order };
                systemModules.Add(m);
            }

            var f = new SystemFeatures { Id = GetFeatureId(module, feature), ModuleId = Modules.GetId(module), Code = feature, Name = GetLabel(module, feature), Order = order };
            systemFeatures.Add(f);

            if (r)
            {
                var id = GetPermissionId(module, feature, EnumAccessTypes.R);
                systemPermissions.Add(new SystemPermissions { Id = id, AccessTypeId = (int)EnumAccessTypes.R, ModuleId = m.Id, FeatureId = f.Id });
                readRoles?.ForEach(x => systemRolePermissions.Add(new AdminRolePermissions { PermissionId = id, RoleId = (int)x }));
            }

            if (c)
            {
                var id = GetPermissionId(module, feature, EnumAccessTypes.C);
                systemPermissions.Add(new SystemPermissions { Id = id, AccessTypeId = (int)EnumAccessTypes.C, ModuleId = m.Id, FeatureId = f.Id });
                createRoles?.ForEach(x => systemRolePermissions.Add(new AdminRolePermissions { PermissionId = id, RoleId = (int)x }));
            }
            if (u)
            {
                var id = GetPermissionId(module, feature, EnumAccessTypes.U);
                systemPermissions.Add(new SystemPermissions { Id = id, AccessTypeId = (int)EnumAccessTypes.U, ModuleId = m.Id, FeatureId = f.Id });
                updateRoles?.ForEach(x => systemRolePermissions.Add(new AdminRolePermissions { PermissionId = id, RoleId = (int)x }));
            }
            if (d)
            {
                var id = GetPermissionId(module, feature, EnumAccessTypes.D);
                systemPermissions.Add(new SystemPermissions { Id = id, AccessTypeId = (int)EnumAccessTypes.D, ModuleId = m.Id, FeatureId = f.Id });
                deleteRoles?.ForEach(x => systemRolePermissions.Add(new AdminRolePermissions { PermissionId = id, RoleId = (int)x }));
            }




        }


        public static void InitPermissions()
        {
            systemPermissions.Clear();
            systemModules.Clear();
            systemFeatures.Clear();
            systemRolePermissions.Clear();

            AddPermission(100, Modules.Org, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg }
                );
            AddPermission(110, Modules.Org, Features.Addresses,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg }
                );
            AddPermission(120, Modules.Org, Features.Contacts,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg }
                );
            AddPermission(200, Modules.Department, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg }
                );
            AddPermission(210, Modules.Department, Features.Admins, u: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg }
                );
            AddPermission(300, Modules.Position, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments }
                );
            AddPermission(310, Modules.Position, Features.SendRules, c: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments }
                );
            AddPermission(320, Modules.Position, Features.Executors,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments }
                );
            AddPermission(330, Modules.Position, Features.Roles, c: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments }
                );
            AddPermission(340, Modules.Position, Features.Journals, c: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments }
                );
            AddPermission(350, Modules.Position, Features.DocumentAccesses, r: false, c: false, d: false,
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.DocumAccess }
                );


            AddPermission(400, Modules.Journal, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementJournals, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementJournals, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementJournals, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementJournals, Roles.ManagementDocumDictionaries }
                );
            AddPermission(410, Modules.Journal, Features.Positions, c: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementJournals, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementJournals, Roles.ManagementDocumDictionaries }
                );

            AddPermission(500, Modules.Templates, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );
            AddPermission(510, Modules.Templates, Features.Tasks,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );
            AddPermission(520, Modules.Templates, Features.Files,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );
            AddPermission(530, Modules.Templates, Features.Papers,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );
            AddPermission(540, Modules.Templates, Features.Plan,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );
            AddPermission(550, Modules.Templates, Features.SignLists,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );
            AddPermission(560, Modules.Templates, Features.AccessList, u: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );
            AddPermission(570, Modules.Templates, Features.Accesses, u: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );

            AddPermission(600, Modules.DocumentType, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );
            AddPermission(610, Modules.DocumentType, Features.Parameters,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );

            AddPermission(700, Modules.Role, Features.Info,
                readRoles: new List<Roles> { Roles.Admin },
                createRoles: new List<Roles> { Roles.Admin },
                updateRoles: new List<Roles> { Roles.Admin },
                deleteRoles: new List<Roles> { Roles.Admin }
                );
            AddPermission(710, Modules.Role, Features.Permissions, c: false, d: false,
                readRoles: new List<Roles> { Roles.Admin },
                updateRoles: new List<Roles> { Roles.Admin}
                );
            AddPermission(720, Modules.Role, Features.Employees, c: false, u: false, d: false,
                readRoles: new List<Roles> { Roles.Admin }
                );
            AddPermission(730, Modules.Role, Features.Positions, c: false, u: false, d: false,
                readRoles: new List<Roles> { Roles.Admin }
                );


            AddPermission(800, Modules.Employee, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees }
                );
            AddPermission(810, Modules.Employee, Features.Assignments,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees }
                );
            AddPermission(820, Modules.Employee, Features.Roles, c: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees }
                );
            AddPermission(830, Modules.Employee, Features.Passport, c: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees }
                );
            AddPermission(840, Modules.Employee, Features.Addresses,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees }
                );
            AddPermission(850, Modules.Employee, Features.Contacts,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementOrg, Roles.ManagementDepartments, Roles.ManagementEmployees }
                );

            AddPermission(860, Modules.Employee, Features.AddInOrg, r: false, u: false, d: false,
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementEmployees, Roles.ManagementDepartments, Roles.ManagementOrg }
                );

            AddPermission(900, Modules.Company, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );
            AddPermission(910, Modules.Company, Features.ContactPersons,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents, Roles.ManagementContactPersons },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents, Roles.ManagementContactPersons },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents, Roles.ManagementContactPersons },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents, Roles.ManagementContactPersons }
                );
            AddPermission(920, Modules.Company, Features.Addresses,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );
            AddPermission(930, Modules.Company, Features.Contacts,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );
            AddPermission(940, Modules.Company, Features.Accounts,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );


            AddPermission(1000, Modules.Person, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );
            AddPermission(1010, Modules.Person, Features.Passport, c: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );
            AddPermission(1020, Modules.Person, Features.Addresses,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );
            AddPermission(1030, Modules.Person, Features.Contacts,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );

            AddPermission(1110, Modules.Bank, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );
            AddPermission(1120, Modules.Bank, Features.Addresses,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );
            AddPermission(1130, Modules.Bank, Features.Contacts,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementAgents }
                );

            AddPermission(1200, Modules.Tags, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );

            AddPermission(1300, Modules.SendList, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementDepartments },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementDepartments },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementDepartments },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementDepartments }
                );
            AddPermission(1310, Modules.SendList, Features.Contents,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementDepartments },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementDepartments },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementDepartments },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementDepartments }
                );

            AddPermission(1400, Modules.ContactType, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementAgents, Roles.ManagementEmployees },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementAgents, Roles.ManagementEmployees },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementAgents, Roles.ManagementEmployees },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementAgents, Roles.ManagementEmployees }
                );
            AddPermission(1410, Modules.AddressType, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementAgents, Roles.ManagementEmployees },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementAgents, Roles.ManagementEmployees },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementAgents, Roles.ManagementEmployees },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries, Roles.ManagementAgents, Roles.ManagementEmployees }
                );

            AddPermission(1500, Modules.Auditlog, Features.Info, c: false, u: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.Auditlog }
                );

            AddPermission(1600, Modules.Auth, Features.Info, c: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementAuth, Roles.ManagementDepartments },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementAuth, Roles.ManagementDepartments }
                );
            AddPermission(1610, Modules.Settings, Features.Info, c: false, d: false,
                readRoles: new List<Roles> { Roles.Admin },
                updateRoles: new List<Roles> { Roles.Admin }
                );
            AddPermission(1620, Modules.CustomDictionaries, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );
            AddPermission(1630, Modules.CustomDictionaries, Features.Contents,
                readRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                createRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                updateRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.ManagementDocumDictionaries }
                );

            AddPermission(1700, Modules.Tools, Features.Info, r: false, u: false, d: false,
                createRoles: new List<Roles> { Roles.Admin }
                );

            AddPermission(2000, Modules.Documents, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.User },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.User }
                );
            AddPermission(2010, Modules.Documents, Features.Files,
                readRoles: new List<Roles> { Roles.Admin, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.User },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.User }
                );
            AddPermission(2020, Modules.Documents, Features.Papers,
                readRoles: new List<Roles> { Roles.Admin, Roles.DocumPapers, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.DocumPapers, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.DocumPapers, Roles.User },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.DocumPapers, Roles.User }
                );

            AddPermission(2040, Modules.Documents, Features.Tasks,
                readRoles: new List<Roles> { Roles.Admin, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.User },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.User }
                );
            AddPermission(2050, Modules.Documents, Features.AccessList, u: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.User },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.User }
                );
            AddPermission(2060, Modules.Documents, Features.Plan,
                readRoles: new List<Roles> { Roles.Admin, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.User },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.User }
                );
            AddPermission(2070, Modules.Documents, Features.Tags, u: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.User }
                );
            AddPermission(2080, Modules.Documents, Features.Links, u: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.User },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.User }
                );
            AddPermission(2090, Modules.Documents, Features.Favourite, r: false, c: false, d: false,
                updateRoles: new List<Roles> { Roles.Admin, Roles.User, Roles.Viewer }
                );

            AddPermission(2110, Modules.Documents, Features.Events, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.DocumActions, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.DocumActions, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.DocumActions, Roles.User }
                );
            AddPermission(2120, Modules.Documents, Features.Waits, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.DocumWaits, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.DocumWaits, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.DocumWaits, Roles.User }
                );
            AddPermission(2130, Modules.Documents, Features.Signs, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.DocumWaits, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.DocumWaits, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.DocumWaits, Roles.User }
                );
            AddPermission(2150, Modules.Documents, Features.WorkGroups, c: false, u: false, d: false,
                readRoles: new List<Roles> { Roles.Admin, Roles.User, Roles.Viewer }
                );
            AddPermission(2190, Modules.Documents, Features.SavedFilters,
                readRoles: new List<Roles> { Roles.Admin, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.User },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.User }
                );

            AddPermission(2200, Modules.PaperList, Features.Info,
                readRoles: new List<Roles> { Roles.Admin, Roles.DocumPapers, Roles.User, Roles.Viewer },
                createRoles: new List<Roles> { Roles.Admin, Roles.DocumPapers, Roles.User },
                updateRoles: new List<Roles> { Roles.Admin, Roles.DocumPapers, Roles.User },
                deleteRoles: new List<Roles> { Roles.Admin, Roles.DocumPapers, Roles.User }
                );


            //AddPermission(2300, Modules.User, Features.Positions,
            //    readRoles: new List<Roles> { Roles.Admin, Roles.User },
            //    createRoles: new List<Roles> { Roles.Admin, Roles.User },
            //    updateRoles: new List<Roles> { Roles.Admin, Roles.User },
            //    deleteRoles: new List<Roles> { Roles.Admin, Roles.User }
            //    );




        }





        public static List<SystemAccessTypes> GetSystemAccessTypes()
        {
            var items = new List<SystemAccessTypes>();

            items.Add(GetSystemAccessType(EnumAccessTypes.C, 2));
            items.Add(GetSystemAccessType(EnumAccessTypes.R, 1));
            items.Add(GetSystemAccessType(EnumAccessTypes.U, 3));
            items.Add(GetSystemAccessType(EnumAccessTypes.D, 4));

            return items;
        }

        private static SystemAccessTypes GetSystemAccessType(EnumAccessTypes id, int order)

        {
            return new SystemAccessTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = GetLabel("AccessTypes", id.ToString()),
                Order = order,
            };
        }

        #region [+] SystemObjects ...

        public static List<SystemObjects> GetSystemObjects()
        {
            var items = new List<SystemObjects>();

            items.Add(GetSystemObjects(EnumObjects.System));
            items.Add(GetSystemObjects(EnumObjects.SystemObjects));
            items.Add(GetSystemObjects(EnumObjects.SystemActions));

            items.Add(GetSystemObjects(EnumObjects.Documents));
            items.Add(GetSystemObjects(EnumObjects.DocumentAccesses));
            items.Add(GetSystemObjects(EnumObjects.DocumentRestrictedSendLists));
            items.Add(GetSystemObjects(EnumObjects.DocumentSendLists));
            items.Add(GetSystemObjects(EnumObjects.DocumentSendListAccessGroups));
            items.Add(GetSystemObjects(EnumObjects.DocumentFiles));
            items.Add(GetSystemObjects(EnumObjects.DocumentLinks));
            items.Add(GetSystemObjects(EnumObjects.DocumentSendListStages));
            items.Add(GetSystemObjects(EnumObjects.DocumentEvents));
            items.Add(GetSystemObjects(EnumObjects.DocumentEventAccessGroups));
            items.Add(GetSystemObjects(EnumObjects.DocumentEventAccesses));
            items.Add(GetSystemObjects(EnumObjects.DocumentWaits));
            items.Add(GetSystemObjects(EnumObjects.DocumentSubscriptions));
            items.Add(GetSystemObjects(EnumObjects.DocumentTasks));
            items.Add(GetSystemObjects(EnumObjects.DocumentPapers));
            items.Add(GetSystemObjects(EnumObjects.DocumentPaperEvents));
            items.Add(GetSystemObjects(EnumObjects.DocumentPaperLists));
            items.Add(GetSystemObjects(EnumObjects.DocumentSavedFilters));
            items.Add(GetSystemObjects(EnumObjects.DocumentTags));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDocumentType));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAddressType));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDocumentSubjects));
            items.Add(GetSystemObjects(EnumObjects.DictionaryRegistrationJournals));
            items.Add(GetSystemObjects(EnumObjects.DictionaryContactType));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgents));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentAddresses));
            items.Add(GetSystemObjects(EnumObjects.DictionaryBankAddress));
            items.Add(GetSystemObjects(EnumObjects.DictionaryClientCompanyAddress));
            items.Add(GetSystemObjects(EnumObjects.DictionaryCompanyAddress));
            items.Add(GetSystemObjects(EnumObjects.DictionaryEmployeeAddress));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPersonAddress));
            items.Add(GetSystemObjects(EnumObjects.DictionaryContacts));
            items.Add(GetSystemObjects(EnumObjects.DictionaryBankContact));
            items.Add(GetSystemObjects(EnumObjects.DictionaryClientCompanyContact));
            items.Add(GetSystemObjects(EnumObjects.DictionaryCompanyContact));
            items.Add(GetSystemObjects(EnumObjects.DictionaryEmployeeContact));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPersonContact));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentPeople));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentPersons));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentEmployees));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentCompanies));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentBanks));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentUsers));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentAccounts));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentClientCompanies));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDepartments));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositions));
            items.Add(GetSystemObjects(EnumObjects.DictionaryStandartSendListContent));
            items.Add(GetSystemObjects(EnumObjects.DictionaryStandartSendLists));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositionExecutorTypes));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositionExecutors));
            items.Add(GetSystemObjects(EnumObjects.Template));
            items.Add(GetSystemObjects(EnumObjects.TemplateSendList));
            items.Add(GetSystemObjects(EnumObjects.TemplateRestrictedSendList));
            items.Add(GetSystemObjects(EnumObjects.TemplateTask));
            items.Add(GetSystemObjects(EnumObjects.TemplateFiles));
            items.Add(GetSystemObjects(EnumObjects.TemplatePaper));
            items.Add(GetSystemObjects(EnumObjects.TemplateAccess));
            items.Add(GetSystemObjects(EnumObjects.DictionaryTag));
            items.Add(GetSystemObjects(EnumObjects.CustomDictionaryTypes));
            items.Add(GetSystemObjects(EnumObjects.CustomDictionaries));
            items.Add(GetSystemObjects(EnumObjects.Properties));
            items.Add(GetSystemObjects(EnumObjects.PropertyLinks));
            items.Add(GetSystemObjects(EnumObjects.PropertyValues));

            items.Add(GetSystemObjects(EnumObjects.EncryptionCertificates));

            items.Add(GetSystemObjects(EnumObjects.AdminRoles));
            items.Add(GetSystemObjects(EnumObjects.AdminRolePermission));
            items.Add(GetSystemObjects(EnumObjects.AdminPositionRoles));
            items.Add(GetSystemObjects(EnumObjects.AdminUserRoles));
            items.Add(GetSystemObjects(EnumObjects.AdminSubordination));
            //items.Add(GetSystemObjects(EnumObjects.DepartmentAdmin));
            items.Add(GetSystemObjects(EnumObjects.AdminRegistrationJournalPositions));
            items.Add(GetSystemObjects(EnumObjects.AdminEmployeeDepartments));

            items.Add(GetSystemObjects(EnumObjects.SystemSettings));


            return items;
        }

        private static SystemObjects GetSystemObjects(EnumObjects id)
        {
            string description = GetLabel("Objects", id.ToString());

            return new SystemObjects()
            {
                Id = (int)id,
                Code = id.ToString(),
            };
        }
        #endregion

        #region [+] SystemActions ...
        public static List<SystemActions> GetSystemActions()
        {
            var items = new List<SystemActions>();


            items.Add(GetSysAct(EnumActions.AddDocument, EnumObjects.Documents));
            items.Add(GetSysAct(EnumActions.AddLinkedDocument, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Info, EnumAccessTypes.C), category: EnumActionCategories.Document));
            items.Add(GetSysAct(EnumActions.CopyDocument, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Info, EnumAccessTypes.C), category: EnumActionCategories.Document));
            items.Add(GetSysAct(EnumActions.ModifyDocument, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Info, EnumAccessTypes.U), category: EnumActionCategories.Document));
            items.Add(GetSysAct(EnumActions.DeleteDocument, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Info, EnumAccessTypes.D), category: EnumActionCategories.Document));
            items.Add(GetSysAct(EnumActions.LaunchPlan, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Plan, EnumAccessTypes.U), category: EnumActionCategories.Informing));
            items.Add(GetSysAct(EnumActions.StopPlan, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Plan, EnumAccessTypes.U), category: EnumActionCategories.Informing));
            items.Add(GetSysAct(EnumActions.ChangeExecutor, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Info, EnumAccessTypes.U), category: EnumActionCategories.Document));
            items.Add(GetSysAct(EnumActions.RegisterDocument, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Info, EnumAccessTypes.C), category: EnumActionCategories.Document));
            items.Add(GetSysAct(EnumActions.MarkDocumentEventAsRead, EnumObjects.Documents));
            items.Add(GetSysAct(EnumActions.SendDocument, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Events, EnumAccessTypes.C), category: EnumActionCategories.Informing));
            items.Add(GetSysAct(EnumActions.SendMessage, EnumObjects.DocumentEvents, GetPermissionId(Modules.Documents, Features.Events, EnumAccessTypes.C), category: EnumActionCategories.Informing));
            items.Add(GetSysAct(EnumActions.AddNote, EnumObjects.DocumentEvents, GetPermissionId(Modules.Documents, Features.Events, EnumAccessTypes.C), category: EnumActionCategories.Informing));

            items.Add(GetSysAct(EnumActions.SendForInformation, EnumObjects.Documents));
            items.Add(GetSysAct(EnumActions.SendForConsideration, EnumObjects.Documents));
            items.Add(GetSysAct(EnumActions.SendForInformationExternal, EnumObjects.Documents));
            items.Add(GetSysAct(EnumActions.SendForExecution, EnumObjects.Documents));
            items.Add(GetSysAct(EnumActions.SendForVisaing, EnumObjects.Documents));
            items.Add(GetSysAct(EnumActions.SendForАgreement, EnumObjects.Documents));
            items.Add(GetSysAct(EnumActions.SendForАpproval, EnumObjects.Documents));
            items.Add(GetSysAct(EnumActions.SendForSigning, EnumObjects.Documents));
            items.Add(GetSysAct(EnumActions.ReportRegistrationCardDocument, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Info, EnumAccessTypes.R), category: EnumActionCategories.Reports));
            items.Add(GetSysAct(EnumActions.ReportRegisterTransmissionDocuments, EnumObjects.DocumentPaperLists, GetPermissionId(Modules.PaperList, Features.Info, EnumAccessTypes.R), category: EnumActionCategories.ParerLists));
            items.Add(GetSysAct(EnumActions.ReportDocumentForDigitalSignature, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Info, EnumAccessTypes.R), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.AddFavourite, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Favourite, EnumAccessTypes.U), category: EnumActionCategories.Additional));
            items.Add(GetSysAct(EnumActions.DeleteFavourite, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Favourite, EnumAccessTypes.U), category: EnumActionCategories.Additional));

            items.Add(GetSysAct(EnumActions.FinishWork, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Info, EnumAccessTypes.U), category: EnumActionCategories.Additional));
            items.Add(GetSysAct(EnumActions.StartWork, EnumObjects.Documents, GetPermissionId(Modules.Documents, Features.Info, EnumAccessTypes.U), category: EnumActionCategories.Additional));
            items.Add(GetSysAct(EnumActions.ChangePosition, EnumObjects.Documents, GetPermissionId(Modules.Position, Features.DocumentAccesses, EnumAccessTypes.U), category: EnumActionCategories.Additional));

            items.Add(GetSysAct(EnumActions.AddDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists, GetPermissionId(Modules.Documents, Features.AccessList, EnumAccessTypes.C)));
            items.Add(GetSysAct(EnumActions.AddByStandartSendListDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists, GetPermissionId(Modules.Documents, Features.AccessList, EnumAccessTypes.C)));
            items.Add(GetSysAct(EnumActions.DeleteDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists, GetPermissionId(Modules.Documents, Features.AccessList, EnumAccessTypes.D)));

            items.Add(GetSysAct(EnumActions.AddDocumentSendList, EnumObjects.DocumentSendListStages, GetPermissionId(Modules.Documents, Features.Plan, EnumAccessTypes.C)));
            items.Add(GetSysAct(EnumActions.AddDocumentSendListStage, EnumObjects.DocumentSendListStages));
            items.Add(GetSysAct(EnumActions.DeleteDocumentSendListStage, EnumObjects.DocumentSendListStages));
            items.Add(GetSysAct(EnumActions.ModifyDocumentSendList, EnumObjects.DocumentSendLists, GetPermissionId(Modules.Documents, Features.Plan, EnumAccessTypes.U)));
            items.Add(GetSysAct(EnumActions.DeleteDocumentSendList, EnumObjects.DocumentSendLists, GetPermissionId(Modules.Documents, Features.Plan, EnumAccessTypes.D)));
            items.Add(GetSysAct(EnumActions.CopyDocumentSendList, EnumObjects.DocumentSendLists, GetPermissionId(Modules.Documents, Features.Plan, EnumAccessTypes.C)));
            items.Add(GetSysAct(EnumActions.LaunchDocumentSendListItem, EnumObjects.DocumentSendLists, GetPermissionId(Modules.Documents, Features.Plan, EnumAccessTypes.U)));

            items.Add(GetSysAct(EnumActions.AddDocumentFile, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.C)));
            items.Add(GetSysAct(EnumActions.ModifyDocumentFile, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.U)));
            items.Add(GetSysAct(EnumActions.DeleteDocumentFile, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.D)));
//            items.Add(GetSysAct(EnumDocumentActions.AddDocumentFileUseMainNameFile, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.C)));
            items.Add(GetSysAct(EnumActions.AcceptDocumentFile, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.U), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.RejectDocumentFile, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.U), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.RenameDocumentFile, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.U)));
            items.Add(GetSysAct(EnumActions.DeleteDocumentFileVersion, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.D)));
            items.Add(GetSysAct(EnumActions.DeleteDocumentFileVersionFinal, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.D)));

            items.Add(GetSysAct(EnumActions.RestoreDocumentFileVersion, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.D)));

            //            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentFileVersionRecord, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.D)));
            items.Add(GetSysAct(EnumActions.AcceptMainVersionDocumentFile, EnumObjects.DocumentFiles, GetPermissionId(Modules.Documents, Features.Files, EnumAccessTypes.U)));

            items.Add(GetSysAct(EnumActions.AddDocumentLink, EnumObjects.DocumentLinks, GetPermissionId(Modules.Documents, Features.Links, EnumAccessTypes.C)));
            items.Add(GetSysAct(EnumActions.DeleteDocumentLink, EnumObjects.DocumentLinks, GetPermissionId(Modules.Documents, Features.Links, EnumAccessTypes.D)));


            items.Add(GetSysAct(EnumActions.ControlOn, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.C), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.ControlChange, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.U), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.SendForExecutionChange, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.U), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.ControlTargetChange, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.U), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.ControlOff, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.U), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.AskPostponeDueDate, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.C), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.CancelPostponeDueDate, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.U), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.MarkExecution, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.C), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.AcceptResult, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.U), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.CancelExecution, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.U), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.RejectResult, EnumObjects.DocumentWaits, GetPermissionId(Modules.Documents, Features.Waits, EnumAccessTypes.U), category: EnumActionCategories.Controls));
            items.Add(GetSysAct(EnumActions.WithdrawVisaing, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.WithdrawАgreement, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.WithdrawАpproval, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.WithdrawSigning, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.AffixVisaing, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.AffixАgreement, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.AffixАpproval, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.AffixSigning, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.SelfAffixSigning, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.RejectVisaing, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.RejectАgreement, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.RejectАpproval, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.RejectSigning, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.U), category: EnumActionCategories.Signing));
            items.Add(GetSysAct(EnumActions.VerifySigning, EnumObjects.DocumentSubscriptions, GetPermissionId(Modules.Documents, Features.Signs, EnumAccessTypes.R), category: EnumActionCategories.Signing));

            items.Add(GetSysAct(EnumActions.AddDocumentTask, EnumObjects.DocumentTasks, GetPermissionId(Modules.Documents, Features.Tasks, EnumAccessTypes.C)));
            items.Add(GetSysAct(EnumActions.ModifyDocumentTask, EnumObjects.DocumentTasks, GetPermissionId(Modules.Documents, Features.Tasks, EnumAccessTypes.U)));
            items.Add(GetSysAct(EnumActions.DeleteDocumentTask, EnumObjects.DocumentTasks, GetPermissionId(Modules.Documents, Features.Tasks, EnumAccessTypes.D)));

            items.Add(GetSysAct(EnumActions.AddDocumentPaper, EnumObjects.DocumentPapers, GetPermissionId(Modules.Documents, Features.Papers, EnumAccessTypes.C), category: EnumActionCategories.Parers));
            items.Add(GetSysAct(EnumActions.ModifyDocumentPaper, EnumObjects.DocumentPapers, GetPermissionId(Modules.Documents, Features.Papers, EnumAccessTypes.U), category: EnumActionCategories.Parers));
            items.Add(GetSysAct(EnumActions.MarkOwnerDocumentPaper, EnumObjects.DocumentPapers, GetPermissionId(Modules.Documents, Features.Papers, EnumAccessTypes.D), category: EnumActionCategories.Parers));
            items.Add(GetSysAct(EnumActions.MarkСorruptionDocumentPaper, EnumObjects.DocumentPapers, GetPermissionId(Modules.Documents, Features.Papers, EnumAccessTypes.U), category: EnumActionCategories.Parers));
            items.Add(GetSysAct(EnumActions.DeleteDocumentPaper, EnumObjects.DocumentPapers, GetPermissionId(Modules.Documents, Features.Papers, EnumAccessTypes.U), category: EnumActionCategories.Parers));
            items.Add(GetSysAct(EnumActions.PlanDocumentPaperEvent, EnumObjects.DocumentPaperEvents, GetPermissionId(Modules.Documents, Features.Papers, EnumAccessTypes.U), category: EnumActionCategories.Parers));
            items.Add(GetSysAct(EnumActions.CancelPlanDocumentPaperEvent, EnumObjects.DocumentPaperEvents, GetPermissionId(Modules.Documents, Features.Papers, EnumAccessTypes.U), category: EnumActionCategories.Parers));
            items.Add(GetSysAct(EnumActions.SendDocumentPaperEvent, EnumObjects.DocumentPaperEvents, GetPermissionId(Modules.Documents, Features.Papers, EnumAccessTypes.U), category: EnumActionCategories.Parers));
            items.Add(GetSysAct(EnumActions.CancelSendDocumentPaperEvent, EnumObjects.DocumentPaperEvents, GetPermissionId(Modules.Documents, Features.Papers, EnumAccessTypes.U), category: EnumActionCategories.Parers));
            items.Add(GetSysAct(EnumActions.RecieveDocumentPaperEvent, EnumObjects.DocumentPaperEvents, GetPermissionId(Modules.Documents, Features.Papers, EnumAccessTypes.U), category: EnumActionCategories.Parers));

            items.Add(GetSysAct(EnumActions.AddDocumentPaperList, EnumObjects.DocumentPaperLists, GetPermissionId(Modules.PaperList, Features.Info, EnumAccessTypes.C), category: EnumActionCategories.ParerLists));
            items.Add(GetSysAct(EnumActions.ModifyDocumentPaperList, EnumObjects.DocumentPaperLists, GetPermissionId(Modules.PaperList, Features.Info, EnumAccessTypes.U), category: EnumActionCategories.ParerLists));
            items.Add(GetSysAct(EnumActions.DeleteDocumentPaperList, EnumObjects.DocumentPaperLists, GetPermissionId(Modules.PaperList, Features.Info, EnumAccessTypes.D), category: EnumActionCategories.ParerLists));

            items.Add(GetSysAct(EnumActions.AddSavedFilter, EnumObjects.DocumentSavedFilters, GetPermissionId(Modules.Documents, Features.SavedFilters, EnumAccessTypes.C)));
            items.Add(GetSysAct(EnumActions.ModifySavedFilter, EnumObjects.DocumentSavedFilters, GetPermissionId(Modules.Documents, Features.SavedFilters, EnumAccessTypes.U)));
            items.Add(GetSysAct(EnumActions.DeleteSavedFilter, EnumObjects.DocumentSavedFilters, GetPermissionId(Modules.Documents, Features.SavedFilters, EnumAccessTypes.D)));
            items.Add(GetSysAct(EnumActions.ModifyDocumentTags, EnumObjects.DocumentTags, GetPermissionId(Modules.Documents, Features.Tags, EnumAccessTypes.C)));

            items.Add(GetSysAct(EnumActions.AddTemplate, EnumObjects.Template));
            items.Add(GetSysAct(EnumActions.CopyTemplate, EnumObjects.Template));
            items.Add(GetSysAct(EnumActions.ModifyTemplate, EnumObjects.Template));
            items.Add(GetSysAct(EnumActions.DeleteTemplate, EnumObjects.Template));
            items.Add(GetSysAct(EnumActions.AddTemplateSendList, EnumObjects.TemplateSendList));
            items.Add(GetSysAct(EnumActions.ModifyTemplateSendList, EnumObjects.TemplateSendList));
            items.Add(GetSysAct(EnumActions.DeleteTemplateSendList, EnumObjects.TemplateSendList));
            items.Add(GetSysAct(EnumActions.AddTemplateRestrictedSendList, EnumObjects.TemplateRestrictedSendList));
            items.Add(GetSysAct(EnumActions.ModifyTemplateRestrictedSendList, EnumObjects.TemplateRestrictedSendList));
            items.Add(GetSysAct(EnumActions.DeleteTemplateRestrictedSendList, EnumObjects.TemplateRestrictedSendList));
            items.Add(GetSysAct(EnumActions.AddTemplateTask, EnumObjects.TemplateTask));
            items.Add(GetSysAct(EnumActions.ModifyTemplateTask, EnumObjects.TemplateTask));
            items.Add(GetSysAct(EnumActions.DeleteTemplateTask, EnumObjects.TemplateTask));
            items.Add(GetSysAct(EnumActions.AddTemplateFile, EnumObjects.TemplateFiles));
            items.Add(GetSysAct(EnumActions.ModifyTemplateFile, EnumObjects.TemplateFiles));
            items.Add(GetSysAct(EnumActions.DeleteTemplateFile, EnumObjects.TemplateFiles));
            items.Add(GetSysAct(EnumActions.AddTemplatePaper, EnumObjects.TemplatePaper));
            items.Add(GetSysAct(EnumActions.ModifyTemplatePaper, EnumObjects.TemplatePaper));
            items.Add(GetSysAct(EnumActions.DeleteTemplatePaper, EnumObjects.TemplatePaper));

            items.Add(GetSysAct(EnumActions.AddTemplateAccess, EnumObjects.TemplateAccess));
            items.Add(GetSysAct(EnumActions.ModifyTemplateAccess, EnumObjects.TemplateAccess));
            items.Add(GetSysAct(EnumActions.DeleteTemplateAccess, EnumObjects.TemplateAccess));


            items.Add(GetSysAct(EnumActions.AddDocumentType, EnumObjects.DictionaryDocumentType));
            items.Add(GetSysAct(EnumActions.ModifyDocumentType, EnumObjects.DictionaryDocumentType));
            items.Add(GetSysAct(EnumActions.DeleteDocumentType, EnumObjects.DictionaryDocumentType));
            items.Add(GetSysAct(EnumActions.AddAddressType, EnumObjects.DictionaryAddressType));
            items.Add(GetSysAct(EnumActions.ModifyAddressType, EnumObjects.DictionaryAddressType));
            items.Add(GetSysAct(EnumActions.DeleteAddressType, EnumObjects.DictionaryAddressType));
            items.Add(GetSysAct(EnumActions.AddRegistrationJournal, EnumObjects.DictionaryRegistrationJournals));
            items.Add(GetSysAct(EnumActions.ModifyRegistrationJournal, EnumObjects.DictionaryRegistrationJournals));
            items.Add(GetSysAct(EnumActions.DeleteRegistrationJournal, EnumObjects.DictionaryRegistrationJournals));
            items.Add(GetSysAct(EnumActions.AddContactType, EnumObjects.DictionaryContactType));
            items.Add(GetSysAct(EnumActions.ModifyContactType, EnumObjects.DictionaryContactType));
            items.Add(GetSysAct(EnumActions.DeleteContactType, EnumObjects.DictionaryContactType));
            items.Add(GetSysAct(EnumActions.SetAgentImage, EnumObjects.DictionaryAgents));
            items.Add(GetSysAct(EnumActions.DeleteAgentImage, EnumObjects.DictionaryAgents));

            items.Add(GetSysAct(EnumActions.AddAgentContact, EnumObjects.DictionaryContacts));
            items.Add(GetSysAct(EnumActions.ModifyAgentContact, EnumObjects.DictionaryContacts));
            items.Add(GetSysAct(EnumActions.DeleteAgentContact, EnumObjects.DictionaryContacts));
            items.Add(GetSysAct(EnumActions.AddBankContact, EnumObjects.DictionaryBankContact));
            items.Add(GetSysAct(EnumActions.ModifyBankContact, EnumObjects.DictionaryBankContact));
            items.Add(GetSysAct(EnumActions.DeleteBankContact, EnumObjects.DictionaryBankContact));
            items.Add(GetSysAct(EnumActions.AddClientCompanyContact, EnumObjects.DictionaryClientCompanyContact));
            items.Add(GetSysAct(EnumActions.ModifyClientCompanyContact, EnumObjects.DictionaryClientCompanyContact));
            items.Add(GetSysAct(EnumActions.DeleteClientCompanyContact, EnumObjects.DictionaryClientCompanyContact));
            items.Add(GetSysAct(EnumActions.AddCompanyContact, EnumObjects.DictionaryCompanyContact));
            items.Add(GetSysAct(EnumActions.ModifyCompanyContact, EnumObjects.DictionaryCompanyContact));
            items.Add(GetSysAct(EnumActions.DeleteCompanyContact, EnumObjects.DictionaryCompanyContact));
            items.Add(GetSysAct(EnumActions.AddEmployeeContact, EnumObjects.DictionaryEmployeeContact));
            items.Add(GetSysAct(EnumActions.ModifyEmployeeContact, EnumObjects.DictionaryEmployeeContact));
            items.Add(GetSysAct(EnumActions.DeleteEmployeeContact, EnumObjects.DictionaryEmployeeContact));
            items.Add(GetSysAct(EnumActions.AddPersonContact, EnumObjects.DictionaryPersonContact));
            items.Add(GetSysAct(EnumActions.ModifyPersonContact, EnumObjects.DictionaryPersonContact));
            items.Add(GetSysAct(EnumActions.DeletePersonContact, EnumObjects.DictionaryPersonContact));


            items.Add(GetSysAct(EnumActions.AddAgentAddress, EnumObjects.DictionaryAgentAddresses));
            items.Add(GetSysAct(EnumActions.ModifyAgentAddress, EnumObjects.DictionaryAgentAddresses));
            items.Add(GetSysAct(EnumActions.DeleteAgentAddress, EnumObjects.DictionaryAgentAddresses));
            items.Add(GetSysAct(EnumActions.AddBankAddress, EnumObjects.DictionaryBankAddress));
            items.Add(GetSysAct(EnumActions.ModifyBankAddress, EnumObjects.DictionaryBankAddress));
            items.Add(GetSysAct(EnumActions.DeleteBankAddress, EnumObjects.DictionaryBankAddress));
            items.Add(GetSysAct(EnumActions.AddClientCompanyAddress, EnumObjects.DictionaryClientCompanyAddress));
            items.Add(GetSysAct(EnumActions.ModifyClientCompanyAddress, EnumObjects.DictionaryClientCompanyAddress));
            items.Add(GetSysAct(EnumActions.DeleteClientCompanyAddress, EnumObjects.DictionaryClientCompanyAddress));
            items.Add(GetSysAct(EnumActions.AddCompanyAddress, EnumObjects.DictionaryCompanyAddress));
            items.Add(GetSysAct(EnumActions.ModifyCompanyAddress, EnumObjects.DictionaryCompanyAddress));
            items.Add(GetSysAct(EnumActions.DeleteCompanyAddress, EnumObjects.DictionaryCompanyAddress));
            items.Add(GetSysAct(EnumActions.AddEmployeeAddress, EnumObjects.DictionaryEmployeeAddress));
            items.Add(GetSysAct(EnumActions.ModifyEmployeeAddress, EnumObjects.DictionaryEmployeeAddress));
            items.Add(GetSysAct(EnumActions.DeleteEmployeeAddress, EnumObjects.DictionaryEmployeeAddress));
            items.Add(GetSysAct(EnumActions.AddPersonAddress, EnumObjects.DictionaryPersonAddress));
            items.Add(GetSysAct(EnumActions.ModifyPersonAddress, EnumObjects.DictionaryPersonAddress));
            items.Add(GetSysAct(EnumActions.DeletePersonAddress, EnumObjects.DictionaryPersonAddress));

            items.Add(GetSysAct(EnumActions.ModifyAgentPeoplePassport, EnumObjects.DictionaryAgentPeople));
            items.Add(GetSysAct(EnumActions.AddAgentPerson, EnumObjects.DictionaryAgentPersons));
            items.Add(GetSysAct(EnumActions.ModifyAgentPerson, EnumObjects.DictionaryAgentPersons));
            items.Add(GetSysAct(EnumActions.DeleteAgentPerson, EnumObjects.DictionaryAgentPersons));


            items.Add(GetSysAct(EnumActions.AddAgentEmployee, EnumObjects.DictionaryAgentEmployees));
            items.Add(GetSysAct(EnumActions.ModifyAgentEmployee, EnumObjects.DictionaryAgentEmployees));
            items.Add(GetSysAct(EnumActions.DeleteAgentEmployee, EnumObjects.DictionaryAgentEmployees));
            items.Add(GetSysAct(EnumActions.ModifyAgentEmployeeLanguage, EnumObjects.DictionaryAgentEmployees));

            items.Add(GetSysAct(EnumActions.AddDepartment, EnumObjects.DictionaryDepartments));
            items.Add(GetSysAct(EnumActions.ModifyDepartment, EnumObjects.DictionaryDepartments));
            items.Add(GetSysAct(EnumActions.DeleteDepartment, EnumObjects.DictionaryDepartments));

            items.Add(GetSysAct(EnumActions.AddPosition, EnumObjects.DictionaryPositions));
            items.Add(GetSysAct(EnumActions.ModifyPosition, EnumObjects.DictionaryPositions));
            items.Add(GetSysAct(EnumActions.DeletePosition, EnumObjects.DictionaryPositions));

            items.Add(GetSysAct(EnumActions.AddAgentCompany, EnumObjects.DictionaryAgentCompanies));
            items.Add(GetSysAct(EnumActions.ModifyAgentCompany, EnumObjects.DictionaryAgentCompanies));
            items.Add(GetSysAct(EnumActions.DeleteAgentCompany, EnumObjects.DictionaryAgentCompanies));

            items.Add(GetSysAct(EnumActions.AddAgentBank, EnumObjects.DictionaryAgentBanks));
            items.Add(GetSysAct(EnumActions.ModifyAgentBank, EnumObjects.DictionaryAgentBanks));
            items.Add(GetSysAct(EnumActions.DeleteAgentBank, EnumObjects.DictionaryAgentBanks));
            items.Add(GetSysAct(EnumActions.AddAgentAccount, EnumObjects.DictionaryAgentAccounts));
            items.Add(GetSysAct(EnumActions.ModifyAgentAccount, EnumObjects.DictionaryAgentAccounts));
            items.Add(GetSysAct(EnumActions.DeleteAgentAccount, EnumObjects.DictionaryAgentAccounts));
            items.Add(GetSysAct(EnumActions.AddStandartSendListContent, EnumObjects.DictionaryStandartSendListContent));
            items.Add(GetSysAct(EnumActions.ModifyStandartSendListContent, EnumObjects.DictionaryStandartSendListContent));
            items.Add(GetSysAct(EnumActions.DeleteStandartSendListContent, EnumObjects.DictionaryStandartSendListContent));
            items.Add(GetSysAct(EnumActions.AddStandartSendList, EnumObjects.DictionaryStandartSendLists));
            items.Add(GetSysAct(EnumActions.ModifyStandartSendList, EnumObjects.DictionaryStandartSendLists));
            items.Add(GetSysAct(EnumActions.DeleteStandartSendList, EnumObjects.DictionaryStandartSendLists));
            items.Add(GetSysAct(EnumActions.AddOrg, EnumObjects.DictionaryAgentClientCompanies));
            items.Add(GetSysAct(EnumActions.ModifyOrg, EnumObjects.DictionaryAgentClientCompanies));
            items.Add(GetSysAct(EnumActions.DeleteOrg, EnumObjects.DictionaryAgentClientCompanies));
            items.Add(GetSysAct(EnumActions.AddExecutorType, EnumObjects.DictionaryPositionExecutorTypes));
            items.Add(GetSysAct(EnumActions.ModifyExecutorType, EnumObjects.DictionaryPositionExecutorTypes));
            items.Add(GetSysAct(EnumActions.DeleteExecutorType, EnumObjects.DictionaryPositionExecutorTypes));
            items.Add(GetSysAct(EnumActions.AddExecutor, EnumObjects.DictionaryPositionExecutors));
            items.Add(GetSysAct(EnumActions.ModifyExecutor, EnumObjects.DictionaryPositionExecutors));
            items.Add(GetSysAct(EnumActions.DeleteExecutor, EnumObjects.DictionaryPositionExecutors));
            items.Add(GetSysAct(EnumActions.AddTag, EnumObjects.DictionaryTag));
            items.Add(GetSysAct(EnumActions.ModifyTag, EnumObjects.DictionaryTag));
            items.Add(GetSysAct(EnumActions.DeleteTag, EnumObjects.DictionaryTag));
            items.Add(GetSysAct(EnumActions.AddCustomDictionaryType, EnumObjects.CustomDictionaryTypes));
            items.Add(GetSysAct(EnumActions.ModifyCustomDictionaryType, EnumObjects.CustomDictionaryTypes));
            items.Add(GetSysAct(EnumActions.DeleteCustomDictionaryType, EnumObjects.CustomDictionaryTypes));
            items.Add(GetSysAct(EnumActions.AddCustomDictionary, EnumObjects.CustomDictionaries));
            items.Add(GetSysAct(EnumActions.ModifyCustomDictionary, EnumObjects.CustomDictionaries));
            items.Add(GetSysAct(EnumActions.DeleteCustomDictionary, EnumObjects.CustomDictionaries));

            items.Add(GetSysAct(EnumActions.AddProperty, EnumObjects.Properties));
            items.Add(GetSysAct(EnumActions.ModifyProperty, EnumObjects.Properties));
            items.Add(GetSysAct(EnumActions.DeleteProperty, EnumObjects.Properties));
            items.Add(GetSysAct(EnumActions.AddPropertyLink, EnumObjects.PropertyLinks));
            items.Add(GetSysAct(EnumActions.ModifyPropertyLink, EnumObjects.PropertyLinks));
            items.Add(GetSysAct(EnumActions.DeletePropertyLink, EnumObjects.PropertyLinks));
            items.Add(GetSysAct(EnumActions.ModifyPropertyValues, EnumObjects.PropertyValues));

            items.Add(GetSysAct(EnumActions.AddEncryptionCertificate, EnumObjects.EncryptionCertificates));
            items.Add(GetSysAct(EnumActions.ModifyEncryptionCertificate, EnumObjects.EncryptionCertificates));
            items.Add(GetSysAct(EnumActions.VerifyPdf, EnumObjects.EncryptionCertificates));
            items.Add(GetSysAct(EnumActions.DeleteEncryptionCertificate, EnumObjects.EncryptionCertificates));

            items.Add(GetSysAct(EnumActions.AddRole, EnumObjects.AdminRoles));
            items.Add(GetSysAct(EnumActions.ModifyRole, EnumObjects.AdminRoles));
            items.Add(GetSysAct(EnumActions.DeleteRole, EnumObjects.AdminRoles));

            items.Add(GetSysAct(EnumActions.SetPositionRole, EnumObjects.AdminPositionRoles));
            items.Add(GetSysAct(EnumActions.DuplicatePositionRoles, EnumObjects.AdminPositionRoles));

            items.Add(GetSysAct(EnumActions.SetRolePermission, EnumObjects.AdminRolePermission));
            items.Add(GetSysAct(EnumActions.SetRolePermissionByModuleFeature, EnumObjects.AdminRolePermission));
            items.Add(GetSysAct(EnumActions.SetRolePermissionByModule, EnumObjects.AdminRolePermission));
            items.Add(GetSysAct(EnumActions.SetRolePermissionByModuleAccessType, EnumObjects.AdminRolePermission));


            items.Add(GetSysAct(EnumActions.SetUserRole, EnumObjects.AdminUserRoles));
            items.Add(GetSysAct(EnumActions.SetUserRoleByAssignment, EnumObjects.AdminUserRoles));

            items.Add(GetSysAct(EnumActions.SetSubordination, EnumObjects.AdminSubordination));
            items.Add(GetSysAct(EnumActions.SetSubordinationByCompany, EnumObjects.AdminSubordination));
            items.Add(GetSysAct(EnumActions.SetSubordinationByDepartment, EnumObjects.AdminSubordination));
            items.Add(GetSysAct(EnumActions.SetDefaultSubordination, EnumObjects.AdminSubordination));
            items.Add(GetSysAct(EnumActions.DuplicateSubordinations, EnumObjects.AdminSubordination));
            items.Add(GetSysAct(EnumActions.SetAllSubordination, EnumObjects.AdminSubordination));

            //items.Add(GetSysAct(EnumAdminActions.SetRegistrationJournalPosition, EnumObjects.AdminRegistrationJournalPositions));
            //items.Add(GetSysAct(EnumAdminActions.SetRegistrationJournalPositionByCompany, EnumObjects.AdminRegistrationJournalPositions, isVisible: false, grantId: (int)EnumAdminActions.SetRegistrationJournalPosition));
            //items.Add(GetSysAct(EnumAdminActions.SetRegistrationJournalPositionByDepartment, EnumObjects.AdminRegistrationJournalPositions, isVisible: false, grantId: (int)EnumAdminActions.SetRegistrationJournalPosition));
            //items.Add(GetSysAct(EnumAdminActions.SetDefaultRegistrationJournalPosition, EnumObjects.AdminRegistrationJournalPositions, isVisible: false, grantId: (int)EnumAdminActions.SetRegistrationJournalPosition));
            //items.Add(GetSysAct(EnumAdminActions.DuplicateRegistrationJournalPositions, EnumObjects.AdminRegistrationJournalPositions, isVisible: false, grantId: (int)EnumAdminActions.SetRegistrationJournalPosition));
            //items.Add(GetSysAct(EnumAdminActions.SetAllRegistrationJournalPosition, EnumObjects.AdminRegistrationJournalPositions, isVisible: false, grantId: (int)EnumAdminActions.SetRegistrationJournalPosition));


            items.Add(GetSysAct(EnumActions.AddDepartmentAdmin, EnumObjects.DictionaryDepartments));
            items.Add(GetSysAct(EnumActions.DeleteDepartmentAdmin, EnumObjects.DictionaryDepartments));

            items.Add(GetSysAct(EnumActions.ChangeLogin, EnumObjects.DictionaryAgentUsers));

            items.Add(GetSysAct(EnumActions.Login, EnumObjects.System));
            items.Add(GetSysAct(EnumActions.SetSetting, EnumObjects.SystemSettings));
            // при добавлении действия не забудь добавить перевод! DMS_WebAPI.Models.ApplicationDbImportData GetAdminLanguageValuesForActions

            return items;
        }

        public static void CheckSystemActions()
        {
            return; // При переходе на Permissions CheckSystemActions потерял смысл и даже вредет. В базе нет необходимости хранить действия

            //int actionsCountByEnums =
            //Enum.GetValues(typeof(EnumAdminActions)).Cast<EnumAdminActions>().Where(x => x > 0).Count() +
            //Enum.GetValues(typeof(EnumEncryptionActions)).Cast<EnumEncryptionActions>().Where(x => x > 0).Count() +
            //Enum.GetValues(typeof(EnumPropertyActions)).Cast<EnumPropertyActions>().Where(x => x > 0).Count() +
            //Enum.GetValues(typeof(EnumDictionaryActions)).Cast<EnumDictionaryActions>().Where(x => x > 0).Count() +
            //Enum.GetValues(typeof(EnumDocumentActions)).Cast<EnumDocumentActions>().Where(x => x > 0).Count() +
            //Enum.GetValues(typeof(EnumSystemActions)).Cast<EnumSystemActions>().Where(x => x > 0).Count();

            //var actionsCountByList = GetSystemActions().Count();

            //if (actionsCountByEnums != actionsCountByList)
            //{
            //    List<EnumModel> list = CheckSystemActions2();
            //    string s = string.Empty;
            //    foreach (var item in list)
            //    {
            //        s += "items.Add(GetSysAct(" + item.EnumName + "." + item.Name + ", EnumObjects.?));" + "\r\n";
            //    }
            //    throw new Exception("Так не пойдет! Нужно GetSystemActions поддерживать в актуальном состоянии \r\n" + s);
            //}


        }

        public static List<EnumModel> CheckSystemActions2()
        {
            var AdminActionsList = GetListByEnum<EnumActions>().Where(x => x.Value > 0);
            var EncryptionActionsList = GetListByEnum<EnumActions>().Where(x => x.Value > 0);
            var PropertyActionsList = GetListByEnum<EnumActions>().Where(x => x.Value > 0);
            var DictionaryActionsList = GetListByEnum<EnumActions>().Where(x => x.Value > 0);
            var DocumentActionsList = GetListByEnum<EnumActions>().Where(x => x.Value > 0);
            var SystemActionsList = GetListByEnum<EnumActions>().Where(x => x.Value > 0);

            var actionsList = GetSystemActions();
            List<EnumModel> ActionsList = new List<EnumModel>();
            ActionsList.AddRange(AdminActionsList);
            ActionsList.AddRange(EncryptionActionsList);
            ActionsList.AddRange(PropertyActionsList);
            ActionsList.AddRange(DictionaryActionsList);
            ActionsList.AddRange(DocumentActionsList);
            ActionsList.AddRange(SystemActionsList);

            List<EnumModel> ResList = new List<EnumModel>();

            foreach (var action in ActionsList)
            {
                if (!ExistAction(actionsList, action))
                {
                    ResList.Add(action);
                }
            }

            return ResList;

        }

        private static bool ExistAction(List<SystemActions> items, EnumModel item)
        {
            foreach (var i in items)
            {
                if (i.Id == item.Value) return true;
            }

            return false;
        }

        public static List<EnumModel> GetListByEnum<T>()
        {
            var array = (T[])(Enum.GetValues(typeof(T)).Cast<T>());
            return array
              .Select(a => new EnumModel
              {
                  EnumName = typeof(T).Name,
                  Value = Convert.ToInt32(a),
                  Name = a.ToString(),
              })
              .OrderBy(kvp => kvp.Name)
              .ToList();
        }

        private static SystemActions GetSysAct(EnumActions id, EnumObjects objId, int? permissionId = null, EnumActionCategories? category = null)
        {
            string description = GetLabel("Actions", id.ToString());
            return GetSystemAction((int)id, id.ToString(), objId, description, permissionId, category);
        }


        private static SystemActions GetSystemAction(int id, string code, EnumObjects objId, string description,
            int? permissionId, EnumActionCategories? category = null)

        {
            return new SystemActions()
            {
                Id = id,
                ObjectId = (int)objId,
                Code = ((EnumActions)id).ToString(),
                //Description = description,
                CategoryId = (int?)category,
                PermissionId = permissionId
            };
        }
        #endregion

        public static List<AdminAccessLevels> GetAdminAccessLevels()
        {
            var items = new List<AdminAccessLevels>();

            items.Add(GetAdminAccessLevel(EnumAccessLevels.Personally));
            items.Add(GetAdminAccessLevel(EnumAccessLevels.PersonallyAndReferents));
            items.Add(GetAdminAccessLevel(EnumAccessLevels.PersonallyAndIOAndReferents));

            return items;
        }

        private static AdminAccessLevels GetAdminAccessLevel(EnumAccessLevels id)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new AdminAccessLevels()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = name,
            };
        }

        public static List<AdminRoleTypes> GetAdminRoleTypes()
        {
            var items = new List<AdminRoleTypes>();

            items.Add(GetAdminRoleType(Roles.Admin));
            items.Add(GetAdminRoleType(Roles.User));
            items.Add(GetAdminRoleType(Roles.Viewer));
            items.Add(GetAdminRoleType(Roles.Auditlog));
            items.Add(GetAdminRoleType(Roles.DocumAccess));
            items.Add(GetAdminRoleType(Roles.DocumActions));
            items.Add(GetAdminRoleType(Roles.DocumPapers));
            items.Add(GetAdminRoleType(Roles.DocumSign));
            items.Add(GetAdminRoleType(Roles.DocumWaits));
            items.Add(GetAdminRoleType(Roles.ManagementAgents));
            items.Add(GetAdminRoleType(Roles.ManagementAuth));
            items.Add(GetAdminRoleType(Roles.ManagementContactPersons));
            items.Add(GetAdminRoleType(Roles.ManagementDocumDictionaries));
            items.Add(GetAdminRoleType(Roles.ManagementEmployees));
            items.Add(GetAdminRoleType(Roles.ManagementJournals));
            items.Add(GetAdminRoleType(Roles.ManagementOrg));
            items.Add(GetAdminRoleType(Roles.ManagementDepartments));

            return items;
        }

        private static AdminRoleTypes GetAdminRoleType(Roles id)
        {
            string name = GetLabel("Roles", id.ToString());
            return new AdminRoleTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
                Name = name,
            };
        }

        public static List<SystemUIElements> GetSystemUIElements()
        {
            var items = new List<SystemUIElements>();

            //items.Add(GetSystemUIElement(10, EnumUIElements.DocumentSubject, "textarea", ));
            //items.Add(GetSystemUIElement(20, EnumUIElements.Description, "textarea", ));
            //items.Add(GetSystemUIElement(30, EnumUIElements.SenderAgent, "select", ));
            //items.Add(GetSystemUIElement(40, EnumUIElements.SenderAgentPerson, "select", ));
            //items.Add(GetSystemUIElement(50, EnumUIElements.SenderNumber, "input", ));
            //items.Add(GetSystemUIElement(60, EnumUIElements.SenderDate, "input", ));
            //items.Add(GetSystemUIElement(70, EnumUIElements.Addressee, "input", ));
            //items.Add(GetSystemUIElement(80, EnumUIElements.AccessLevel, "select", ));



            //items.Add(new SystemUIElements {  ValueTypeId = 1, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "DocumentSubject", ValueDescriptionFieldCode = "DocumentSubject", Format = null });
            //items.Add(new SystemUIElements {  ValueTypeId = 1, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "Description", ValueDescriptionFieldCode = "Description", Format = null });
            //items.Add(new SystemUIElements {  ValueTypeId = 2, SelectAPI = "DictionaryAgents", SelectFilter = "{'IsCompany' : 'True'}", SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "SenderAgentId", ValueDescriptionFieldCode = "SenderAgentName", Format = null });
            //items.Add(new SystemUIElements {  ValueTypeId = 2, SelectAPI = "DictionaryAgentPersons", SelectFilter = "{'CompanyIDs' : '@SenderAgentId'}", SelectFieldCode = "Id", SelectDescriptionFieldCode = "FullName", ValueFieldCode = "SenderAgentPersonId", ValueDescriptionFieldCode = "SenderAgentPersonName", Format = null });
            //items.Add(new SystemUIElements {  ValueTypeId = 1, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "SenderNumber", ValueDescriptionFieldCode = "SenderNumber", Format = null });
            //items.Add(new SystemUIElements {  ValueTypeId = 3, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "SenderDate", ValueDescriptionFieldCode = "SenderDate", Format = null });
            //items.Add(new SystemUIElements {  ValueTypeId = 1, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "Addressee", ValueDescriptionFieldCode = "Addressee", Format = null });
            //items.Add(new SystemUIElements {  ValueTypeId = 2, SelectAPI = "AdminAccessLevels", SelectFilter = null, SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "AccessLevelId", ValueDescriptionFieldCode = "AccessLevelName", Format = null });
            //TODO add meta for templates!!!!

            return items;
        }

        private static SystemUIElements GetSystemUIElement(int order, EnumUIElements id, string typeCode, EnumValueTypes valType)
        {
            string label = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString() + ".Label");
            string descr = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString() + ".Description");
            string hint = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString()) + ".Hint";

            return new SystemUIElements()
            {
                Id = (int)id,
                Code = id.ToString(),
                ActionId = (int)EnumActions.ModifyDocument,
                TypeCode = typeCode,
                //Description = descr,
                //Label = label,
                //Hint = hint,
                ValueTypeId = (int)valType,
                IsMandatory = false,
                IsReadOnly = false,
                IsVisible = false,

                Order = order,
            };
        }

        public static List<SystemValueTypes> GetSystemValueTypes()
        {
            // Синхронизировать с ApplicationDbImportData
            var items = new List<SystemValueTypes>();

            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Text, Code = EnumValueTypes.Text.ToString() });
            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Number, Code = EnumValueTypes.Number.ToString() });
            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Date, Code = EnumValueTypes.Date.ToString() });
            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Api, Code = EnumValueTypes.Api.ToString() });
            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Bool, Code = EnumValueTypes.Bool.ToString() });
            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Password, Code = EnumValueTypes.Password.ToString() });

            return items;
        }

        public static List<DictionaryFileTypes> GetDictionaryFileTypes()
        {
            var items = new List<DictionaryFileTypes>();

            items.Add(GetDictionaryFileType(EnumFileTypes.Main));
            items.Add(GetDictionaryFileType(EnumFileTypes.Additional));
            items.Add(GetDictionaryFileType(EnumFileTypes.SubscribePdf));

            return items;
        }

        private static DictionaryFileTypes GetDictionaryFileType(EnumFileTypes id)
        {
//            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionaryFileTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
//                Name = name,
            };
        }

        public static List<DictionarySigningTypes> GetDictionarySigningTypes()
        {
            var items = new List<DictionarySigningTypes>();

            items.Add(GetDictionarySigningType(EnumSigningTypes.Hash));
            items.Add(GetDictionarySigningType(EnumSigningTypes.InternalSign));
            items.Add(GetDictionarySigningType(EnumSigningTypes.CertificateSign));

            return items;
        }

        private static DictionarySigningTypes GetDictionarySigningType(EnumSigningTypes id)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionarySigningTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = name,
            };
        }

        public static List<DictionaryDocumentDirections> GetDictionaryDocumentDirections()
        {
            var items = new List<DictionaryDocumentDirections>();

            items.Add(GetDictionaryDocumentDirection(EnumDocumentDirections.Incoming));
            items.Add(GetDictionaryDocumentDirection(EnumDocumentDirections.Outcoming));
            items.Add(GetDictionaryDocumentDirection(EnumDocumentDirections.Internal));

            return items;
        }

        private static DictionaryDocumentDirections GetDictionaryDocumentDirection(EnumDocumentDirections id)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionaryDocumentDirections()
            {
                Id = (int)id,
                Code = id.ToString(),
            };
        }

        public static List<DictionaryEventTypes> GetDictionaryEventTypes()
        {
            var items = new List<DictionaryEventTypes>();

            items.Add(GetDictionaryEventType(EnumEventTypes.AddNewDocument, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AddDocumentFile, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RenameDocumentFile, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ModifyDocumentFile, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.DeleteDocumentFileVersion, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RestoreDocumentFileVersion, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.DeleteDocumentFile, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectDocumentFile, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AcceptDocumentFile, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForInformation, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ChangeExecutor, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ChangePosition, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForExecution, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForExecutionChange, EnumImportanceEventTypes.DocumentMoovement, "Исполнение"));

            items.Add(GetDictionaryEventType(EnumEventTypes.SendForConsideration, EnumImportanceEventTypes.DocumentMoovement, "Исполнение"));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForInformationExternal, EnumImportanceEventTypes.DocumentMoovement, "Исполнение"));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForVisaing, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AffixVisaing, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectVisaing, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.WithdrawVisaing, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForАgreement, EnumImportanceEventTypes.DocumentMoovement, "Виза"));
            items.Add(GetDictionaryEventType(EnumEventTypes.AffixАgreement, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectАgreement, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.WithdrawАgreement, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForАpproval, EnumImportanceEventTypes.DocumentMoovement, "Согласование"));
            items.Add(GetDictionaryEventType(EnumEventTypes.AffixАpproval, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectАpproval, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.WithdrawАpproval, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForSigning, EnumImportanceEventTypes.DocumentMoovement, "Утверждение"));
            items.Add(GetDictionaryEventType(EnumEventTypes.AffixSigning, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectSigning, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.WithdrawSigning, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ControlOn, EnumImportanceEventTypes.DocumentMoovement, "Подпись"));
            items.Add(GetDictionaryEventType(EnumEventTypes.ControlOff, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ControlChange, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ControlTargetChange, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AskPostponeDueDate, EnumImportanceEventTypes.ImportantEvents, "Перенос срока"));
            items.Add(GetDictionaryEventType(EnumEventTypes.CancelPostponeDueDate, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.MarkExecution, EnumImportanceEventTypes.ImportantEvents, "Контроль"));
            items.Add(GetDictionaryEventType(EnumEventTypes.AcceptResult, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.CancelExecution, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectResult, EnumImportanceEventTypes.ImportantEvents, "Контроль"));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendMessage, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AddNewPaper, EnumImportanceEventTypes.ImportantEvents, "Рассмотрение отчета"));
            items.Add(GetDictionaryEventType(EnumEventTypes.MarkOwnerDocumentPaper, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.MarkСorruptionDocumentPaper, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.MoveDocumentPaper, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AddLink, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.DeleteLink, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AddNote, EnumImportanceEventTypes.AdditionalEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.TaskFormulation, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.Registered, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.LaunchPlan, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.StopPlan, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SetInWork, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SetOutWork, EnumImportanceEventTypes.ImportantEvents, null));

            return items;
        }

        private static DictionaryEventTypes GetDictionaryEventType(EnumEventTypes id, EnumImportanceEventTypes importanceEventTypeId, string waitDescription = null)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionaryEventTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
//                Name = name,
                SourceDescription = null,
                TargetDescription = null,
                ImportanceEventTypeId = (int)importanceEventTypeId,
                WaitDescription = waitDescription,
            };
        }

        public static List<DictionaryImportanceEventTypes> GetDictionaryImportanceEventTypes()
        {
            var items = new List<DictionaryImportanceEventTypes>();

            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.DocumentMoovement));
            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.ImportantEvents));
            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.AdditionalEvents));
            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.PaperMoovement));
            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.Message));
            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.Internal));

            return items;
        }

        private static DictionaryImportanceEventTypes GetDictionaryImportanceEventType(EnumImportanceEventTypes id)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionaryImportanceEventTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
            };
        }

        public static List<DictionaryResultTypes> GetDictionaryResultTypes()
        {
            var items = new List<DictionaryResultTypes>();

            items.Add(GetDictionaryResultType(EnumResultTypes.CloseByAffixing, IsExecute: true, IsActive: false));
            items.Add(GetDictionaryResultType(EnumResultTypes.CloseByRejecting, IsExecute: false, IsActive: false));
            items.Add(GetDictionaryResultType(EnumResultTypes.CloseByWithdrawing, IsExecute: false, IsActive: false));
            items.Add(GetDictionaryResultType(EnumResultTypes.CloseByChanging, IsExecute: false, IsActive: false));
            items.Add(GetDictionaryResultType(EnumResultTypes.Excellent, IsExecute: true, IsActive: true));
            items.Add(GetDictionaryResultType(EnumResultTypes.Good, IsExecute: true, IsActive: true));
            items.Add(GetDictionaryResultType(EnumResultTypes.Satisfactorily, IsExecute: true, IsActive: true));
            items.Add(GetDictionaryResultType(EnumResultTypes.Bad, IsExecute: false, IsActive: true));
            items.Add(GetDictionaryResultType(EnumResultTypes.WithoutEvaluation, IsExecute: false, IsActive: true));

            return items;
        }

        private static DictionaryResultTypes GetDictionaryResultType(EnumResultTypes id, bool IsExecute, bool IsActive)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionaryResultTypes()
            {
                Id = (int)id,
                //Name = name,
                IsExecute = IsExecute,
            };
        }

        public static List<DictionarySendTypes> GetDictionarySendTypes()
        {
            var items = new List<DictionarySendTypes>();

            items.Add(GetDictionarySendType(10, EnumSendTypes.Information, isImportant: false, subordinationTypeId: 1));
            items.Add(GetDictionarySendType(20, EnumSendTypes.Consideration, isImportant: false, subordinationTypeId: 1));
            items.Add(GetDictionarySendType(30, EnumSendTypes.Execution, isImportant: true, subordinationTypeId: 2));
            items.Add(GetDictionarySendType(40, EnumSendTypes.Аgreement, isImportant: true, subordinationTypeId: 1));
            items.Add(GetDictionarySendType(50, EnumSendTypes.Signing, isImportant: true, subordinationTypeId: 1));
            items.Add(GetDictionarySendType(60, EnumSendTypes.Visaing, isImportant: true, subordinationTypeId: 1));
            items.Add(GetDictionarySendType(70, EnumSendTypes.Аpproval, isImportant: true, subordinationTypeId: 1));
            //items.Add(GetDictionarySendType(80, EnumSendTypes.SendForControl, isImportant: true, subordinationTypeId: 2));
            //items.Add(GetDictionarySendType(90, EnumSendTypes.SendForResponsibleExecution, isImportant: true, subordinationTypeId: 2));
            items.Add(GetDictionarySendType(100, EnumSendTypes.InformationExternal, isImportant: false, subordinationTypeId: 1));

            return items;
        }

        private static DictionarySendTypes GetDictionarySendType(int order, EnumSendTypes id, bool isImportant, int subordinationTypeId)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionarySendTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = name,
                IsImportant = isImportant,
                SubordinationTypeId = subordinationTypeId,
                Order = order,
            };
        }

        public static List<DictionarySubordinationTypes> GetDictionarySubordinationTypes()
        {
            var items = new List<DictionarySubordinationTypes>();

            items.Add(GetDictionarySubordinationType(EnumSubordinationTypes.Informing));
            items.Add(GetDictionarySubordinationType(EnumSubordinationTypes.Execution));

            return items;
        }

        private static DictionarySubordinationTypes GetDictionarySubordinationType(EnumSubordinationTypes id)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionarySubordinationTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = name,
            };
        }

        public static List<DictionaryStageTypes> GetDictionaryStageTypes()
        {
            var items = new List<DictionaryStageTypes>();

            items.Add(GetDictionaryStageType(EnumStageTypes.Signing));
            items.Add(GetDictionaryStageType(EnumStageTypes.Sending));

            return items;
        }

        private static DictionaryStageTypes GetDictionaryStageType(EnumStageTypes id)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionaryStageTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = name,
            };
        }

        public static List<DicRegJournalAccessTypes> GetDictionaryRegistrationJournalAccessTypes()
        {
            var items = new List<DicRegJournalAccessTypes>();

            items.Add(GetDictionaryRegistrationJournalAccessType(EnumRegistrationJournalAccessTypes.View));
            items.Add(GetDictionaryRegistrationJournalAccessType(EnumRegistrationJournalAccessTypes.Registration));

            return items;
        }

        private static DicRegJournalAccessTypes GetDictionaryRegistrationJournalAccessType(EnumRegistrationJournalAccessTypes id)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DicRegJournalAccessTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = name,
            };
        }

        public static List<DictionarySubscriptionStates> GetDictionarySubscriptionStates()
        {
            var items = new List<DictionarySubscriptionStates>();

            items.Add(GetDictionarySubscriptionState(EnumSubscriptionStates.No, false));
            items.Add(GetDictionarySubscriptionState(EnumSubscriptionStates.Violated, false));
            items.Add(GetDictionarySubscriptionState(EnumSubscriptionStates.Visa, true));
            items.Add(GetDictionarySubscriptionState(EnumSubscriptionStates.Аgreement, true));
            items.Add(GetDictionarySubscriptionState(EnumSubscriptionStates.Аpproval, true));
            items.Add(GetDictionarySubscriptionState(EnumSubscriptionStates.Sign, true));

            return items;
        }

        private static DictionarySubscriptionStates GetDictionarySubscriptionState(EnumSubscriptionStates id, bool isSuccess)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionarySubscriptionStates()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = name,
                IsSuccess = isSuccess,
            };
        }

        public static List<DictionaryPositionExecutorTypes> GetDictionaryPositionExecutorTypes()
        {
            var items = new List<DictionaryPositionExecutorTypes>();

            items.Add(GetDictionaryPositionExecutorType(EnumPositionExecutionTypes.Personal, true));
            items.Add(GetDictionaryPositionExecutorType(EnumPositionExecutionTypes.IO));
            items.Add(GetDictionaryPositionExecutorType(EnumPositionExecutionTypes.Referent));

            return items;
        }

        private static DictionaryPositionExecutorTypes GetDictionaryPositionExecutorType(EnumPositionExecutionTypes id, bool WithoutSuffix = false)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            string description = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString() + ".Description");
            string suffix = WithoutSuffix ? string.Empty : GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString() + ".Suffix");

            return new DictionaryPositionExecutorTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = name,
                //Description = description,
                Suffix = suffix,
            };
        }

        public static List<DictionaryLinkTypes> GetDictionaryLinkTypes()
        {
            var items = new List<DictionaryLinkTypes>();

            items.Add(GetDictionaryLinkType(EnumLinkTypes.Answer, false));
            items.Add(GetDictionaryLinkType(EnumLinkTypes.Execution, true));
            items.Add(GetDictionaryLinkType(EnumLinkTypes.Additionally, false));
            items.Add(GetDictionaryLinkType(EnumLinkTypes.Repeatedly, false));
            items.Add(GetDictionaryLinkType(EnumLinkTypes.Change, false));
            items.Add(GetDictionaryLinkType(EnumLinkTypes.Cancel, true));

            return items;
        }

        private static DictionaryLinkTypes GetDictionaryLinkType(EnumLinkTypes id, bool IsImportant)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionaryLinkTypes()
            {
                Id = (int)id,
                //Name = name,
                IsImportant = IsImportant,
            };
        }


        public static List<SystemFormulas> GetSystemFormulas()
        {
            var items = new List<SystemFormulas>();

            items.Add(GetSystemFormula(EnumSystemFormulas.RegistrationJournalId, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.RegistrationJournalIndex, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.RegistrationJournalDepartmentCode, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.InitiativeRegistrationFullNumber, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.InitiativeRegistrationNumberPrefix, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.InitiativeRegistrationNumberSuffix, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.InitiativeRegistrationNumber, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.InitiativeRegistrationSenderNumber, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.ExecutorPositionDepartmentCode, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.SubscriptionsPositionDepartmentCode, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.CurrentPositionDepartmentCode, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.Date, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.DocumentSendListLastAgentExternalFirstSymbolName, ""));
            items.Add(GetSystemFormula(EnumSystemFormulas.OrdinalNumberDocumentLinkForCorrespondent, ""));

            return items;
        }

        private static SystemFormulas GetSystemFormula(EnumSystemFormulas id, string example = "")
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            string description = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString() + ".Description");
            return new SystemFormulas()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = name,
                //Description = description,
                Example = example,
            };
        }

        public static List<SystemPatterns> GetSystemPatterns()
        {
            var items = new List<SystemPatterns>();

            items.Add(GetSystemPattern(EnumSystemPatterns.Condition, "c"));
            items.Add(GetSystemPattern(EnumSystemPatterns.Formula, "v"));
            items.Add(GetSystemPattern(EnumSystemPatterns.Format, "f"));
            items.Add(GetSystemPattern(EnumSystemPatterns.Length, "l"));

            return items;
        }

        private static SystemPatterns GetSystemPattern(EnumSystemPatterns id, string code)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            string description = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString() + ".Description");
            return new SystemPatterns()
            {
                Id = (int)id,
                Code = code,
                //Name = name,
                //Description = description,
            };
        }

        public static List<SystemFormats> GetSystemFormats()
        {
            var items = new List<SystemFormats>();

            items.Add(GetSystemFormat(EnumSystemFormats.Year, "yyyy"));
            items.Add(GetSystemFormat(EnumSystemFormats.Day, "dd"));
            items.Add(GetSystemFormat(EnumSystemFormats.Month, "MM"));
            items.Add(GetSystemFormat(EnumSystemFormats.Year2, "yy"));

            return items;
        }

        private static SystemFormats GetSystemFormat(EnumSystemFormats id, string code)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            string description = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString() + ".Description");
            return new SystemFormats()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = "##l@SystemFormats:" + (id).ToString() + "@l##",
                //Description = description,
            };
        }
        public static List<DictionarySettingTypes> GetDictionarySettingTypes()
        {
            var items = new List<DictionarySettingTypes>();

            items.Add(GetDictionarySettingType(EnumSettingTypes.DocumentFlow, 10));
            items.Add(GetDictionarySettingType(EnumSettingTypes.Mail, 20));
            items.Add(GetDictionarySettingType(EnumSettingTypes.Report, 30));
            items.Add(GetDictionarySettingType(EnumSettingTypes.General, 99));
            return items;
        }

        private static DictionarySettingTypes GetDictionarySettingType(EnumSettingTypes id, int order)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new DictionarySettingTypes()
            {
                Id = (int)id,
                Code = id.ToString(),
                //Name = name,
                Order = order,
            };
        }


    }

}
