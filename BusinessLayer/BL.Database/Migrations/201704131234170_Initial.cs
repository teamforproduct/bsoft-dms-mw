namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.AdminAccessLevels",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.AdminEmployeeDepartments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryDepartments", t => t.DepartmentId)
                .ForeignKey("DMS.DictionaryAgentEmployees", t => t.EmployeeId)
                .Index(t => new { t.EmployeeId, t.DepartmentId }, unique: true, name: "IX_EmployeeDepartment")
                .Index(t => t.EmployeeId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "DMS.DictionaryDepartments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        CompanyId = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 400),
                        FullName = c.String(maxLength: 2000),
                        FullPath = c.String(maxLength: 2000),
                        ChiefPositionId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.ChiefPositionId)
                .ForeignKey("DMS.DictionaryCompanies", t => t.CompanyId)
                .ForeignKey("DMS.DictionaryDepartments", t => t.ParentId)
                .Index(t => new { t.CompanyId, t.ParentId, t.Name }, name: "IX_CompanyParentName")
                .Index(t => t.ParentId)
                .Index(t => t.ChiefPositionId);
            
            CreateTable(
                "DMS.DictionaryPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Name = c.String(maxLength: 2000),
                        FullName = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        ExecutorAgentId = c.Int(),
                        PositionExecutorTypeId = c.Int(),
                        MainExecutorAgentId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.ParentId)
                .ForeignKey("DMS.DictionaryDepartments", t => t.DepartmentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorAgentId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.PositionExecutorTypeId)
                .ForeignKey("DMS.DictionaryAgents", t => t.MainExecutorAgentId)
                .Index(t => t.ParentId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ExecutorAgentId)
                .Index(t => t.PositionExecutorTypeId)
                .Index(t => t.MainExecutorAgentId);
            
            CreateTable(
                "DMS.DocumentAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        IsInWork = c.Boolean(nullable: false),
                        IsFavourite = c.Boolean(nullable: false),
                        IsAddLater = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CountNewEvents = c.Int(),
                        CountWaits = c.Int(),
                        OverDueCountWaits = c.Int(),
                        MinDueDate = c.DateTime(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => new { t.DocumentId, t.PositionId }, unique: true, name: "IX_DocumentPosition")
                .Index(t => new { t.PositionId, t.DocumentId }, unique: true, name: "IX_PositionDocument")
                .Index(t => t.AgentId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.DictionaryAgents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        Image = c.Binary(),
                        ResidentTypeId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryResidentTypes", t => t.ResidentTypeId)
                .Index(t => t.ClientId)
                .Index(t => t.ResidentTypeId);
            
            CreateTable(
                "DMS.DictionaryAgentAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        AccountNumber = c.String(maxLength: 2000),
                        IsMain = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        AgentBank_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryAgentBanks", t => t.AgentBank_Id)
                .Index(t => new { t.AgentId, t.Name }, unique: true, name: "IX_AgentName")
                .Index(t => t.AgentBank_Id);
            
            CreateTable(
                "DMS.DictionaryAgentBanks",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        MFOCode = c.String(maxLength: 400),
                        FullName = c.String(maxLength: 400),
                        Swift = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.FullName, t.ClientId }, unique: true, name: "IX_FullName")
                .Index(t => new { t.MFOCode, t.ClientId }, unique: true, name: "IX_MFOCode");
            
            CreateTable(
                "DMS.DictionaryAgentAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        AdressTypeId = c.Int(nullable: false),
                        PostCode = c.String(maxLength: 2000),
                        Address = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAddressTypes", t => t.AdressTypeId)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .Index(t => new { t.AgentId, t.AdressTypeId }, unique: true, name: "IX_AdressType")
                .Index(t => t.AdressTypeId);
            
            CreateTable(
                "DMS.DictionaryAddressTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        SpecCode = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.Code, t.ClientId }, unique: true, name: "IX_Code")
                .Index(t => new { t.Name, t.ClientId }, unique: true, name: "IX_Name");
            
            CreateTable(
                "DMS.DictionaryAgentCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        FullName = c.String(maxLength: 400),
                        TaxCode = c.String(maxLength: 400),
                        OKPOCode = c.String(maxLength: 2000),
                        VATCode = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.FullName, t.ClientId }, unique: true, name: "IX_FullName")
                .Index(t => new { t.TaxCode, t.ClientId }, unique: true, name: "IX_TaxCode");
            
            CreateTable(
                "DMS.DictionaryAgentPersons",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        AgentCompanyId = c.Int(),
                        Position = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgentCompanies", t => t.AgentCompanyId)
                .ForeignKey("DMS.DictionaryAgentPeoples", t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => t.AgentCompanyId);
            
            CreateTable(
                "DMS.DictionaryAgentPeoples",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        FullName = c.String(maxLength: 400),
                        LastName = c.String(maxLength: 2000),
                        FirstName = c.String(maxLength: 2000),
                        MiddleName = c.String(maxLength: 2000),
                        BirthDate = c.DateTime(),
                        IsMale = c.Boolean(),
                        PassportSerial = c.String(maxLength: 2000),
                        PassportNumber = c.Int(),
                        PassportText = c.String(maxLength: 2000),
                        PassportDate = c.DateTime(),
                        TaxCode = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.ClientId);
            
            CreateTable(
                "DMS.DictionaryAgentContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        ContactTypeId = c.Int(nullable: false),
                        Contact = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryContactTypes", t => t.ContactTypeId)
                .Index(t => new { t.AgentId, t.ContactTypeId, t.Contact }, unique: true, name: "IX_AgentContactTypeContact")
                .Index(t => t.ContactTypeId);
            
            CreateTable(
                "DMS.DictionaryContactTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        SpecCode = c.String(maxLength: 2000),
                        InputMask = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.Code, t.ClientId }, unique: true, name: "IX_Code")
                .Index(t => new { t.Name, t.ClientId }, unique: true, name: "IX_Name");
            
            CreateTable(
                "DMS.DictionaryAgentEmployees",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        PersonnelNumber = c.Int(nullable: false),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.PersonnelNumber, t.ClientId }, unique: true, name: "IX_PersonnelNumber");
            
            CreateTable(
                "DMS.DictionaryPositionExecutors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        PositionExecutorTypeId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        Description = c.String(maxLength: 2000),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.PositionExecutorTypeId)
                .ForeignKey("DMS.DictionaryAgentEmployees", t => t.AgentId)
                .Index(t => t.AgentId)
                .Index(t => new { t.PositionId, t.AgentId, t.StartDate }, unique: true, name: "IX_PositionAgentStartDate")
                .Index(t => t.PositionExecutorTypeId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.DicPositionExecutorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        Suffix = c.String(maxLength: 400),
                        Description = c.String(maxLength: 400),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.AdminUserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        PositionExecutorId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositionExecutors", t => t.PositionExecutorId)
                .ForeignKey("DMS.AdminRoles", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => new { t.RoleId, t.PositionExecutorId }, unique: true, name: "IX_UserRoleExecutor")
                .Index(t => t.PositionExecutorId);
            
            CreateTable(
                "DMS.AdminRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        RoleTypeId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminRoleTypes", t => t.RoleTypeId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Name, t.ClientId }, unique: true, name: "IX_Name")
                .Index(t => t.RoleTypeId);
            
            CreateTable(
                "DMS.AdminPositionRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.AdminRoles", t => t.RoleId)
                .Index(t => new { t.PositionId, t.RoleId }, unique: true, name: "IX_PositionRole")
                .Index(t => t.RoleId);
            
            CreateTable(
                "DMS.AdminRolePermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemPermissions", t => t.PermissionId)
                .ForeignKey("DMS.AdminRoles", t => t.RoleId)
                .Index(t => new { t.PermissionId, t.RoleId }, unique: true, name: "IX_PermissionRole");
            
            CreateTable(
                "DMS.SystemPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        FeatureId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemAccessTypes", t => t.AccessTypeId)
                .ForeignKey("DMS.SystemFeatures", t => t.FeatureId)
                .ForeignKey("DMS.SystemModules", t => t.ModuleId)
                .Index(t => new { t.ModuleId, t.FeatureId, t.AccessTypeId }, unique: true, name: "IX_ModuleFeatureAccessType");
            
            CreateTable(
                "DMS.SystemAccessTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemActions",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        PermissionId = c.Int(),
                        ObjectId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        Category = c.String(maxLength: 2000),
                        SystemActions_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemActions", t => t.SystemActions_Id)
                .ForeignKey("DMS.SystemObjects", t => t.ObjectId)
                .ForeignKey("DMS.SystemPermissions", t => t.PermissionId)
                .Index(t => t.PermissionId)
                .Index(t => new { t.ObjectId, t.Code }, unique: true, name: "IX_ObjectCode")
                .Index(t => t.SystemActions_Id);
            
            CreateTable(
                "DMS.SystemObjects",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemFields",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ObjectId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        ValueTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemObjects", t => t.ObjectId)
                .ForeignKey("DMS.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => new { t.ObjectId, t.Code }, unique: true, name: "IX_ObjectCode")
                .Index(t => t.ValueTypeId);
            
            CreateTable(
                "DMS.SystemValueTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemModules", t => t.ModuleId)
                .Index(t => new { t.ModuleId, t.Code }, unique: true, name: "IX_ModuleCode");
            
            CreateTable(
                "DMS.SystemModules",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.AdminRoleTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DictionaryCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        FullName = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.FullName, t.ClientId }, unique: true, name: "IX_FullName");
            
            CreateTable(
                "DMS.DictionaryAgentUsers",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        UserName = c.String(maxLength: 256),
                        LanguageId = c.Int(nullable: false),
                        LastPositionChose = c.String(maxLength: 2000),
                        IsSendEMail = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminLanguages", t => t.LanguageId)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.UserId, t.ClientId }, unique: true, name: "IX_UserId")
                .Index(t => t.LanguageId);
            
            CreateTable(
                "DMS.AdminLanguages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.AdminLanguageValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        LanguageId = c.Int(nullable: false),
                        Label = c.String(maxLength: 400),
                        Value = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminLanguages", t => t.LanguageId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Label, t.LanguageId, t.ClientId }, unique: true, name: "IX_Label")
                .Index(t => t.LanguageId);
            
            CreateTable(
                "DMS.EncryptionCertificates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 400),
                        Thumbprint = c.String(maxLength: 2000),
                        Certificate = c.Binary(),
                        CreateDate = c.DateTime(nullable: false),
                        NotBefore = c.DateTime(),
                        NotAfter = c.DateTime(),
                        IsRememberPassword = c.Boolean(nullable: false),
                        AgentId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .Index(t => t.AgentId);
            
            CreateTable(
                "DMS.DocumentSubscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        SendEventId = c.Int(nullable: false),
                        DoneEventId = c.Int(),
                        Description = c.String(maxLength: 2000),
                        SubscriptionStateId = c.Int(),
                        Hash = c.String(maxLength: 2000),
                        FullHash = c.String(maxLength: 2000),
                        ChangedHash = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        SigningTypeId = c.Int(nullable: false),
                        InternalSign = c.String(),
                        CertificateSign = c.String(),
                        CertificateId = c.Int(),
                        CertificateSignCreateDate = c.DateTime(),
                        CertificatePositionId = c.Int(),
                        CertificatePositionExecutorAgentId = c.Int(),
                        CertificatePositionExecutorTypeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.EncryptionCertificates", t => t.CertificateId)
                .ForeignKey("DMS.DictionaryPositions", t => t.CertificatePositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.CertificatePositionExecutorAgentId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.CertificatePositionExecutorTypeId)
                .ForeignKey("DMS.DocumentEvents", t => t.DoneEventId)
                .ForeignKey("DMS.DocumentEvents", t => t.SendEventId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionarySigningTypes", t => t.SigningTypeId)
                .ForeignKey("DMS.DictionarySubscriptionStates", t => t.SubscriptionStateId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendEventId)
                .Index(t => t.DoneEventId)
                .Index(t => t.SubscriptionStateId)
                .Index(t => t.SigningTypeId)
                .Index(t => t.CertificateId)
                .Index(t => t.CertificatePositionId)
                .Index(t => t.CertificatePositionExecutorAgentId)
                .Index(t => t.CertificatePositionExecutorTypeId);
            
            CreateTable(
                "DMS.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentDirectionId = c.Int(),
                        DocumentTypeId = c.Int(),
                        TemplateDocumentId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        DocumentSubject = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        Image = c.Binary(),
                        AddDescription = c.String(maxLength: 2000),
                        IsRegistered = c.Boolean(),
                        RegistrationJournalId = c.Int(),
                        NumerationPrefixFormula = c.String(maxLength: 2000),
                        RegistrationNumber = c.Int(),
                        RegistrationNumberSuffix = c.String(maxLength: 100),
                        RegistrationNumberPrefix = c.String(maxLength: 100),
                        RegistrationDate = c.DateTime(),
                        ExecutorPositionId = c.Int(),
                        ExecutorPositionExeAgentId = c.Int(nullable: false),
                        ExecutorPositionExeTypeId = c.Int(),
                        SenderAgentId = c.Int(),
                        SenderAgentPersonId = c.Int(),
                        SenderNumber = c.String(maxLength: 2000),
                        SenderDate = c.DateTime(),
                        Addressee = c.String(maxLength: 2000),
                        LinkId = c.Int(),
                        IsLaunchPlan = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryDocumentDirections", t => t.DocumentDirectionId)
                .ForeignKey("DMS.DictionaryDocumentTypes", t => t.DocumentTypeId)
                .ForeignKey("DMS.DictionaryPositions", t => t.ExecutorPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorPositionExeAgentId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.ExecutorPositionExeTypeId)
                .ForeignKey("DMS.DictionaryRegistrationJournals", t => t.RegistrationJournalId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SenderAgentId)
                .ForeignKey("DMS.DictionaryAgentPersons", t => t.SenderAgentPersonId)
                .ForeignKey("DMS.TemplateDocuments", t => t.TemplateDocumentId)
                .Index(t => new { t.IsRegistered, t.Id, t.TemplateDocumentId }, name: "IX_IsRegistered")
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentDirectionId)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.TemplateDocumentId)
                .Index(t => t.CreateDate)
                .Index(t => t.RegistrationJournalId)
                .Index(t => t.ExecutorPositionId)
                .Index(t => t.ExecutorPositionExeAgentId)
                .Index(t => t.ExecutorPositionExeTypeId)
                .Index(t => t.SenderAgentId)
                .Index(t => t.SenderAgentPersonId);
            
            CreateTable(
                "DMS.DictionaryDocumentDirections",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DictionaryDocumentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.Name, t.ClientId }, unique: true, name: "IX_Name");
            
            CreateTable(
                "DMS.DocumentEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        EventTypeId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Date = c.DateTime(nullable: false),
                        TaskId = c.Int(),
                        Description = c.String(maxLength: 2000),
                        AddDescription = c.String(maxLength: 2000),
                        SourcePositionId = c.Int(),
                        SourcePositionExecutorAgentId = c.Int(),
                        SourcePositionExecutorTypeId = c.Int(),
                        SourceAgentId = c.Int(),
                        TargetPositionId = c.Int(),
                        TargetPositionExecutorAgentId = c.Int(),
                        TargetPositionExecutorTypeId = c.Int(),
                        TargetAgentId = c.Int(),
                        SendDate = c.DateTime(),
                        ReadDate = c.DateTime(),
                        ReadAgentId = c.Int(),
                        IsChanged = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        PaperId = c.Int(),
                        SendListId = c.Int(),
                        ParentEventId = c.Int(),
                        PaperListId = c.Int(),
                        PaperPlanAgentId = c.Int(),
                        PaperPlanDate = c.DateTime(),
                        PaperSendAgentId = c.Int(),
                        PaperSendDate = c.DateTime(),
                        PaperRecieveAgentId = c.Int(),
                        PaperRecieveDate = c.DateTime(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DocumentEvents", t => t.ParentEventId)
                .ForeignKey("DMS.DocumentSendLists", t => t.SendListId)
                .ForeignKey("DMS.DocumentTasks", t => t.TaskId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryEventTypes", t => t.EventTypeId)
                .ForeignKey("DMS.DocumentPapers", t => t.PaperId)
                .ForeignKey("DMS.DocumentPaperLists", t => t.PaperListId)
                .ForeignKey("DMS.DictionaryAgents", t => t.PaperPlanAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.PaperRecieveAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.PaperSendAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ReadAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SourceAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.SourcePositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SourcePositionExecutorAgentId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.SourcePositionExecutorTypeId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetPositionExecutorAgentId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.TargetPositionExecutorTypeId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentId)
                .Index(t => new { t.ReadDate, t.TargetPositionId, t.DocumentId, t.SourcePositionId }, name: "IX_ReadDate")
                .Index(t => t.EventTypeId)
                .Index(t => t.Date)
                .Index(t => t.TaskId)
                .Index(t => t.SourcePositionId)
                .Index(t => t.SourcePositionExecutorAgentId)
                .Index(t => t.SourcePositionExecutorTypeId)
                .Index(t => t.SourceAgentId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetPositionExecutorAgentId)
                .Index(t => t.TargetPositionExecutorTypeId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.ReadAgentId)
                .Index(t => t.PaperId)
                .Index(t => t.SendListId)
                .Index(t => t.ParentEventId)
                .Index(t => t.PaperListId)
                .Index(t => t.PaperPlanAgentId)
                .Index(t => t.PaperSendAgentId)
                .Index(t => t.PaperRecieveAgentId)
                .Index(t => t.LastChangeDate);
            
            CreateTable(
                "DMS.DocumentEventAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        PositionExecutorTypeId = c.Int(),
                        SendDate = c.DateTime(),
                        ReadDate = c.DateTime(),
                        ReadAgentId = c.Int(),
                        IsFavourite = c.Boolean(nullable: false),
                        IsAddLater = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DocumentEvents", t => t.EventId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.PositionExecutorTypeId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ReadAgentId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentId)
                .Index(t => t.EventId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.PositionExecutorTypeId)
                .Index(t => t.ReadAgentId);
            
            CreateTable(
                "DMS.DocumentEventAccessGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                        AccessGroupTypeId = c.Int(nullable: false),
                        CompanyId = c.Int(),
                        DepartmentId = c.Int(),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        StandartSendListId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryCompanies", t => t.CompanyId)
                .ForeignKey("DMS.DictionaryDepartments", t => t.DepartmentId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DocumentEvents", t => t.EventId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DictionaryStandartSendLists", t => t.StandartSendListId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentId)
                .Index(t => t.EventId)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.StandartSendListId);
            
            CreateTable(
                "DMS.DictionaryStandartSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        PositionId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => t.ClientId)
                .Index(t => new { t.PositionId, t.Name, t.ClientId }, unique: true, name: "IX_PositionName");
            
            CreateTable(
                "DMS.DicStandartSendListContents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StandartSendListId = c.Int(nullable: false),
                        Stage = c.Int(nullable: false),
                        SendTypeId = c.Int(nullable: false),
                        TargetPositionId = c.Int(),
                        TargetAgentId = c.Int(),
                        Task = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        DueDate = c.DateTime(),
                        DueDay = c.Int(),
                        AccessLevelId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("DMS.DictionarySendTypes", t => t.SendTypeId)
                .ForeignKey("DMS.DictionaryStandartSendLists", t => t.StandartSendListId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.TargetPositionId)
                .Index(t => t.StandartSendListId)
                .Index(t => t.SendTypeId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.DictionarySendTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                        IsImportant = c.Boolean(nullable: false),
                        SubordinationTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionarySubordinationTypes", t => t.SubordinationTypeId)
                .Index(t => t.Name, unique: true)
                .Index(t => t.SubordinationTypeId);
            
            CreateTable(
                "DMS.DictionarySubordinationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DocumentSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        StageTypeId = c.Int(),
                        Stage = c.Int(nullable: false),
                        SendTypeId = c.Int(nullable: false),
                        SourcePositionId = c.Int(nullable: false),
                        SourcePositionExecutorAgentId = c.Int(),
                        SourcePositionExecutorTypeId = c.Int(),
                        SourceAgentId = c.Int(nullable: false),
                        TargetPositionId = c.Int(),
                        TargetPositionExecutorAgentId = c.Int(),
                        TargetPositionExecutorTypeId = c.Int(),
                        TargetAgentId = c.Int(),
                        TaskId = c.Int(),
                        Description = c.String(maxLength: 2000),
                        AddDescription = c.String(maxLength: 2000),
                        DueDate = c.DateTime(),
                        DueDay = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        IsInitial = c.Boolean(nullable: false),
                        IsWorkGroup = c.Boolean(nullable: false),
                        IsAddControl = c.Boolean(nullable: false),
                        SelfDescription = c.String(maxLength: 2000),
                        SelfDueDate = c.DateTime(),
                        SelfDueDay = c.Int(),
                        SelfAttentionDate = c.DateTime(),
                        SelfAttentionDay = c.Int(),
                        StartEventId = c.Int(),
                        CloseEventId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("DMS.DocumentEvents", t => t.CloseEventId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionarySendTypes", t => t.SendTypeId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SourceAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.SourcePositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SourcePositionExecutorAgentId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.SourcePositionExecutorTypeId)
                .ForeignKey("DMS.DictionaryStageTypes", t => t.StageTypeId)
                .ForeignKey("DMS.DocumentEvents", t => t.StartEventId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetPositionExecutorAgentId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.TargetPositionExecutorTypeId)
                .ForeignKey("DMS.DocumentTasks", t => t.TaskId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentId)
                .Index(t => t.StageTypeId)
                .Index(t => t.SendTypeId)
                .Index(t => t.SourcePositionId)
                .Index(t => t.SourcePositionExecutorAgentId)
                .Index(t => t.SourcePositionExecutorTypeId)
                .Index(t => t.SourceAgentId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetPositionExecutorAgentId)
                .Index(t => t.TargetPositionExecutorTypeId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.TaskId)
                .Index(t => t.AccessLevelId)
                .Index(t => t.StartEventId)
                .Index(t => t.CloseEventId);
            
            CreateTable(
                "DMS.DocumentSendListAccessGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        SendListId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                        AccessGroupTypeId = c.Int(nullable: false),
                        CompanyId = c.Int(),
                        DepartmentId = c.Int(),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        StandartSendListId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryCompanies", t => t.CompanyId)
                .ForeignKey("DMS.DictionaryDepartments", t => t.DepartmentId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DocumentSendLists", t => t.SendListId)
                .ForeignKey("DMS.DictionaryStandartSendLists", t => t.StandartSendListId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendListId)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.StandartSendListId);
            
            CreateTable(
                "DMS.DictionaryStageTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DocumentTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        PositionExecutorAgentId = c.Int(nullable: false),
                        PositionExecutorTypeId = c.Int(),
                        AgentId = c.Int(nullable: false),
                        Task = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.PositionExecutorAgentId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.PositionExecutorTypeId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => new { t.DocumentId, t.Task }, unique: true, name: "IX_DocumentTask")
                .Index(t => t.PositionId)
                .Index(t => t.PositionExecutorAgentId)
                .Index(t => t.PositionExecutorTypeId)
                .Index(t => t.AgentId);
            
            CreateTable(
                "DMS.DocumentTaskAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DocumentTasks", t => t.TaskId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => new { t.PositionId, t.TaskId }, unique: true, name: "IX_PositionTask")
                .Index(t => new { t.TaskId, t.PositionId }, unique: true, name: "IX_TaskPosition");
            
            CreateTable(
                "DMS.DictionaryEventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        SourceDescription = c.String(maxLength: 2000),
                        TargetDescription = c.String(maxLength: 2000),
                        WaitDescription = c.String(maxLength: 2000),
                        ImportanceEventTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryImportanceEventTypes", t => t.ImportanceEventTypeId)
                .Index(t => t.Name, unique: true)
                .Index(t => t.ImportanceEventTypeId);
            
            CreateTable(
                "DMS.DictionaryImportanceEventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DocumentFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        EventId = c.Int(),
                        Name = c.String(maxLength: 2000),
                        OrderNumber = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        Extension = c.String(maxLength: 2000),
                        FileType = c.String(maxLength: 2000),
                        FileSize = c.Long(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Content = c.String(maxLength: 2000),
                        TypeId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsWorkedOut = c.Boolean(),
                        Hash = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsMainVersion = c.Boolean(nullable: false),
                        ExecutorPositionId = c.Int(),
                        ExecutorPositionExeAgentId = c.Int(nullable: false),
                        ExecutorPositionExeTypeId = c.Int(),
                        IsPdfCreated = c.Boolean(),
                        LastPdfAccessDate = c.DateTime(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DocumentEvents", t => t.EventId)
                .ForeignKey("DMS.DictionaryPositions", t => t.ExecutorPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorPositionExeAgentId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.ExecutorPositionExeTypeId)
                .ForeignKey("DMS.DictionaryFileTypes", t => t.TypeId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => new { t.DocumentId, t.Name, t.Extension, t.Version }, unique: true, name: "IX_DocumentNameExtensionVersion")
                .Index(t => new { t.DocumentId, t.OrderNumber, t.Version }, unique: true, name: "IX_DocumentOrderNumberVersion")
                .Index(t => t.EventId)
                .Index(t => t.TypeId)
                .Index(t => t.ExecutorPositionId)
                .Index(t => t.ExecutorPositionExeAgentId)
                .Index(t => t.ExecutorPositionExeTypeId)
                .Index(t => t.LastChangeDate);
            
            CreateTable(
                "DMS.DictionaryFileTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DocumentWaits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        OnEventId = c.Int(nullable: false),
                        OffEventId = c.Int(),
                        ResultTypeId = c.Int(),
                        DueDate = c.DateTime(),
                        PlanDueDate = c.DateTime(),
                        AttentionDate = c.DateTime(),
                        TargetDescription = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DocumentWaits", t => t.ParentId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DocumentEvents", t => t.OffEventId)
                .ForeignKey("DMS.DocumentEvents", t => t.OnEventId)
                .ForeignKey("DMS.DictionaryResultTypes", t => t.ResultTypeId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentId)
                .Index(t => t.ParentId)
                .Index(t => t.OnEventId)
                .Index(t => t.OffEventId)
                .Index(t => t.ResultTypeId)
                .Index(t => t.DueDate);
            
            CreateTable(
                "DMS.DictionaryResultTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 2000),
                        IsExecute = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DocumentPapers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        IsMain = c.Boolean(nullable: false),
                        IsOriginal = c.Boolean(nullable: false),
                        IsCopy = c.Boolean(nullable: false),
                        PageQuantity = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        IsInWork = c.Boolean(nullable: false),
                        LastPaperEventId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DocumentEvents", t => t.LastPaperEventId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => new { t.DocumentId, t.Name, t.IsMain, t.IsOriginal, t.IsCopy, t.OrderNumber }, unique: true, name: "IX_DocumentNameOrderNumber")
                .Index(t => t.LastPaperEventId);
            
            CreateTable(
                "DMS.DocumentPaperLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId);
            
            CreateTable(
                "DMS.DocumentLinks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        ParentDocumentId = c.Int(nullable: false),
                        LinkTypeId = c.Int(nullable: false),
                        ExecutorPositionId = c.Int(),
                        ExecutorPositionExeAgentId = c.Int(nullable: false),
                        ExecutorPositionExeTypeId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.ExecutorPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorPositionExeAgentId)
                .ForeignKey("DMS.DicPositionExecutorTypes", t => t.ExecutorPositionExeTypeId)
                .ForeignKey("DMS.DictionaryLinkTypes", t => t.LinkTypeId)
                .ForeignKey("DMS.Documents", t => t.ParentDocumentId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => new { t.DocumentId, t.ParentDocumentId }, unique: true, name: "IX_DocumentParentDocument")
                .Index(t => t.ParentDocumentId)
                .Index(t => t.LinkTypeId)
                .Index(t => t.ExecutorPositionId)
                .Index(t => t.ExecutorPositionExeAgentId)
                .Index(t => t.ExecutorPositionExeTypeId);
            
            CreateTable(
                "DMS.DictionaryLinkTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        IsImportant = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DictionaryRegistrationJournals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 200),
                        DepartmentId = c.Int(nullable: false),
                        Index = c.String(maxLength: 200),
                        NumerationPrefixFormula = c.String(maxLength: 2000),
                        PrefixFormula = c.String(maxLength: 2000),
                        SuffixFormula = c.String(maxLength: 2000),
                        DirectionCodes = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryDepartments", t => t.DepartmentId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Name, t.DepartmentId, t.Index, t.ClientId }, unique: true, name: "IX_Name")
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "DMS.AdminRegistrationJournalPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PositionId = c.Int(nullable: false),
                        RegJournalId = c.Int(nullable: false),
                        RegJournalAccessTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DictionaryRegistrationJournals", t => t.RegJournalId)
                .ForeignKey("DMS.DicRegJournalAccessTypes", t => t.RegJournalAccessTypeId)
                .Index(t => new { t.PositionId, t.RegJournalId, t.RegJournalAccessTypeId }, unique: true, name: "IX_JournalPositionType")
                .Index(t => t.RegJournalId)
                .Index(t => t.RegJournalAccessTypeId);
            
            CreateTable(
                "DMS.DicRegJournalAccessTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DocumentRestrictedSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => new { t.DocumentId, t.PositionId }, unique: true, name: "IX_DocumentPosition")
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.DocumentTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryTags", t => t.TagId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => new { t.DocumentId, t.TagId }, unique: true, name: "IX_DocumentTag")
                .Index(t => t.TagId);
            
            CreateTable(
                "DMS.DictionaryTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        PositionId = c.Int(),
                        Color = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => t.ClientId)
                .Index(t => new { t.PositionId, t.Name, t.ClientId }, unique: true, name: "IX_PositionName");
            
            CreateTable(
                "DMS.TemplateDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        IsHard = c.Boolean(nullable: false),
                        IsForProject = c.Boolean(nullable: false),
                        IsForDocument = c.Boolean(nullable: false),
                        DocumentDirectionId = c.Int(nullable: false),
                        DocumentTypeId = c.Int(nullable: false),
                        DocumentSubject = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        RegistrationJournalId = c.Int(),
                        SenderAgentId = c.Int(),
                        SenderAgentPersonId = c.Int(),
                        Addressee = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryDocumentDirections", t => t.DocumentDirectionId)
                .ForeignKey("DMS.DictionaryDocumentTypes", t => t.DocumentTypeId)
                .ForeignKey("DMS.DictionaryRegistrationJournals", t => t.RegistrationJournalId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SenderAgentId)
                .ForeignKey("DMS.DictionaryAgentPersons", t => t.SenderAgentPersonId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Name, t.ClientId }, unique: true, name: "IX_Name")
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentDirectionId)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.RegistrationJournalId)
                .Index(t => t.SenderAgentId)
                .Index(t => t.SenderAgentPersonId);
            
            CreateTable(
                "DMS.TemplateDocumentAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => new { t.DocumentId, t.PositionId }, unique: true, name: "IX_DocumentPosition")
                .Index(t => t.PositionId);
            
            CreateTable(
                "DMS.TemplateDocumentFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 200),
                        OrderNumber = c.Int(nullable: false),
                        Extention = c.String(maxLength: 200),
                        FileType = c.String(maxLength: 2000),
                        FileSize = c.Long(nullable: false),
                        TypeId = c.Int(nullable: false),
                        Hash = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsPdfCreated = c.Boolean(),
                        LastPdfAccessDate = c.DateTime(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryFileTypes", t => t.TypeId)
                .Index(t => new { t.DocumentId, t.Name, t.Extention }, unique: true, name: "IX_DocumentNameExtention")
                .Index(t => new { t.DocumentId, t.OrderNumber }, unique: true, name: "IX_DocumentOrderNumber")
                .Index(t => t.TypeId);
            
            CreateTable(
                "DMS.TemplateDocumentPapers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        IsMain = c.Boolean(nullable: false),
                        IsOriginal = c.Boolean(nullable: false),
                        IsCopy = c.Boolean(nullable: false),
                        PageQuantity = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .Index(t => new { t.DocumentId, t.Name, t.IsMain, t.IsOriginal, t.IsCopy, t.OrderNumber }, unique: true, name: "IX_DocumentNameOrderNumber");
            
            CreateTable(
                "DMS.TempDocRestrictedSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => new { t.DocumentId, t.PositionId }, unique: true, name: "IX_DocumentPosition")
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.TemplateDocumentSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        SendTypeId = c.Int(nullable: false),
                        TargetPositionId = c.Int(),
                        TargetAgentId = c.Int(),
                        TaskId = c.Int(),
                        Description = c.String(maxLength: 2000),
                        StageTypeId = c.Int(),
                        Stage = c.Int(nullable: false),
                        DueDay = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        IsWorkGroup = c.Boolean(nullable: false),
                        IsAddControl = c.Boolean(nullable: false),
                        SelfDescription = c.String(maxLength: 2000),
                        SelfDueDay = c.Int(),
                        SelfAttentionDay = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("DMS.DictionarySendTypes", t => t.SendTypeId)
                .ForeignKey("DMS.DictionaryStageTypes", t => t.StageTypeId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("DMS.TemplateDocumentTasks", t => t.TaskId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendTypeId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.TaskId)
                .Index(t => t.StageTypeId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.TemplateDocumentSendListAccessGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        SendListId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                        AccessGroupTypeId = c.Int(nullable: false),
                        CompanyId = c.Int(),
                        DepartmentId = c.Int(),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        StandartSendListId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryCompanies", t => t.CompanyId)
                .ForeignKey("DMS.DictionaryDepartments", t => t.DepartmentId)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.TemplateDocumentSendLists", t => t.SendListId)
                .ForeignKey("DMS.DictionaryStandartSendLists", t => t.StandartSendListId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendListId)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.StandartSendListId);
            
            CreateTable(
                "DMS.TemplateDocumentTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        Task = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => new { t.DocumentId, t.Task }, unique: true, name: "IX_DocumentTask")
                .Index(t => t.PositionId);
            
            CreateTable(
                "DMS.DictionarySigningTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DictionarySubscriptionStates",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        IsSuccess = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DictionaryResidentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.Name, t.ClientId }, unique: true, name: "IX_Name");
            
            CreateTable(
                "DMS.AdminSubordinations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourcePositionId = c.Int(nullable: false),
                        TargetPositionId = c.Int(nullable: false),
                        SubordinationTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.SourcePositionId)
                .ForeignKey("DMS.DictionarySubordinationTypes", t => t.SubordinationTypeId)
                .ForeignKey("DMS.DictionaryPositions", t => t.TargetPositionId)
                .Index(t => new { t.SourcePositionId, t.TargetPositionId, t.SubordinationTypeId }, unique: true, name: "IX_SourceTargetType")
                .Index(t => t.TargetPositionId)
                .Index(t => t.SubordinationTypeId);
            
            CreateTable(
                "DMS.CustomDictionaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DictionaryTypeId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.CustomDictionaryTypes", t => t.DictionaryTypeId)
                .Index(t => new { t.DictionaryTypeId, t.Code }, unique: true, name: "IX_DictionaryTypeCode");
            
            CreateTable(
                "DMS.CustomDictionaryTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.Code, t.ClientId }, unique: true, name: "IX_Code");
            
            CreateTable(
                "DMS.DictionaryAgentFavorites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        ObjectId = c.Int(nullable: false),
                        Module = c.String(maxLength: 200),
                        Feature = c.String(maxLength: 200),
                        Date = c.DateTime(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .Index(t => t.AgentId, name: "IX_Agent")
                .Index(t => new { t.AgentId, t.ObjectId, t.Module, t.Feature }, unique: true, name: "IX_AgentObjectModuleFeature")
                .Index(t => t.Module)
                .Index(t => t.Feature);
            
            CreateTable(
                "DMS.DictionaryDocumentSubjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        Name = c.String(maxLength: 400),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryDocumentSubjects", t => t.ParentId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Name, t.ClientId }, unique: true, name: "IX_Name")
                .Index(t => t.ParentId);
            
            CreateTable(
                "DMS.DictionarySettingTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.SystemSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Key = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        Value = c.String(maxLength: 2000),
                        ValueTypeId = c.Int(nullable: false),
                        SettingTypeId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        ExecutorAgentId = c.Int(),
                        AccessType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorAgentId)
                .ForeignKey("DMS.DictionarySettingTypes", t => t.SettingTypeId)
                .ForeignKey("DMS.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Key, t.ExecutorAgentId, t.ClientId }, unique: true, name: "IX_KeyExecutorAgent")
                .Index(t => t.ValueTypeId)
                .Index(t => t.SettingTypeId)
                .Index(t => t.ExecutorAgentId);
            
            CreateTable(
                "DMS.DocumentSavedFilters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        UserId = c.Int(),
                        Name = c.String(maxLength: 400),
                        Icon = c.String(maxLength: 400),
                        Filter = c.String(maxLength: 2000),
                        IsCommon = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgentUsers", t => t.UserId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Name, t.UserId, t.ClientId }, unique: true, name: "IX_NameUser")
                .Index(t => t.UserId);
            
            CreateTable(
                "DMS.FullTextIndexCashes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ObjectId = c.Int(nullable: false),
                        ObjectType = c.Int(nullable: false),
                        OperationType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId);
            
            CreateTable(
                "DMS.SystemLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        LogLevel = c.Int(nullable: false),
                        Message = c.String(maxLength: 2000),
                        LogTrace = c.String(maxLength: 2000),
                        LogException = c.String(maxLength: 2000),
                        ExecutorAgentId = c.Int(),
                        ObjectId = c.Int(),
                        ObjectLog = c.String(maxLength: 2000),
                        ActionId = c.Int(),
                        RecordId = c.Int(),
                        LogDate = c.DateTime(nullable: false),
                        LogDate1 = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemActions", t => t.ActionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorAgentId)
                .ForeignKey("DMS.SystemObjects", t => t.ObjectId)
                .Index(t => t.ClientId)
                .Index(t => t.ExecutorAgentId)
                .Index(t => t.ObjectId)
                .Index(t => t.ActionId)
                .Index(t => t.LogDate);
            
            CreateTable(
                "DMS.Properties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        TypeCode = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        Label = c.String(maxLength: 2000),
                        Hint = c.String(maxLength: 2000),
                        ValueTypeId = c.Int(),
                        OutFormat = c.String(maxLength: 2000),
                        InputFormat = c.String(maxLength: 2000),
                        SelectAPI = c.String(maxLength: 2000),
                        SelectFilter = c.String(maxLength: 2000),
                        SelectFieldCode = c.String(maxLength: 2000),
                        SelectDescriptionFieldCode = c.String(maxLength: 2000),
                        SelectTable = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Code, t.ClientId }, unique: true, name: "IX_Code")
                .Index(t => t.ValueTypeId);
            
            CreateTable(
                "DMS.PropertyLinks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        ObjectId = c.Int(nullable: false),
                        Filers = c.String(maxLength: 400),
                        IsMandatory = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemObjects", t => t.ObjectId)
                .ForeignKey("DMS.Properties", t => t.PropertyId)
                .Index(t => new { t.ObjectId, t.PropertyId, t.Filers }, unique: true, name: "IX_ObjectPropertyFilers")
                .Index(t => t.PropertyId);
            
            CreateTable(
                "DMS.PropertyValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyLinkId = c.Int(nullable: false),
                        RecordId = c.Int(nullable: false),
                        ValueString = c.String(maxLength: 2000),
                        ValueDate = c.DateTime(),
                        ValueNumeric = c.Double(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.PropertyLinks", t => t.PropertyLinkId)
                .Index(t => new { t.PropertyLinkId, t.RecordId }, unique: true, name: "IX_PropertyLinkRecord")
                .Index(t => new { t.RecordId, t.PropertyLinkId }, unique: true, name: "IX_RecordId");
            
            CreateTable(
                "DMS.SystemFormats",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemFormulas",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        Example = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemPatterns",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemSearchQueryLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        FeatureId = c.Int(nullable: false),
                        SearchQueryText = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId);
            
            CreateTable(
                "DMS.SystemUIElements",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        TypeCode = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        Label = c.String(maxLength: 2000),
                        Hint = c.String(maxLength: 2000),
                        ValueTypeId = c.Int(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                        IsReadOnly = c.Boolean(nullable: false),
                        IsVisible = c.Boolean(nullable: false),
                        SelectAPI = c.String(maxLength: 2000),
                        SelectFilter = c.String(maxLength: 2000),
                        SelectFieldCode = c.String(maxLength: 2000),
                        SelectDescriptionFieldCode = c.String(maxLength: 2000),
                        ValueFieldCode = c.String(maxLength: 2000),
                        ValueDescriptionFieldCode = c.String(maxLength: 2000),
                        Format = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemActions", t => t.ActionId)
                .ForeignKey("DMS.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => new { t.ActionId, t.Code }, unique: true, name: "IX_ActionCode")
                .Index(t => t.ValueTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.SystemUIElements", "ValueTypeId", "DMS.SystemValueTypes");
            DropForeignKey("DMS.SystemUIElements", "ActionId", "DMS.SystemActions");
            DropForeignKey("DMS.Properties", "ValueTypeId", "DMS.SystemValueTypes");
            DropForeignKey("DMS.PropertyValues", "PropertyLinkId", "DMS.PropertyLinks");
            DropForeignKey("DMS.PropertyLinks", "PropertyId", "DMS.Properties");
            DropForeignKey("DMS.PropertyLinks", "ObjectId", "DMS.SystemObjects");
            DropForeignKey("DMS.SystemLogs", "ObjectId", "DMS.SystemObjects");
            DropForeignKey("DMS.SystemLogs", "ExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.SystemLogs", "ActionId", "DMS.SystemActions");
            DropForeignKey("DMS.DocumentSavedFilters", "UserId", "DMS.DictionaryAgentUsers");
            DropForeignKey("DMS.SystemSettings", "ValueTypeId", "DMS.SystemValueTypes");
            DropForeignKey("DMS.SystemSettings", "SettingTypeId", "DMS.DictionarySettingTypes");
            DropForeignKey("DMS.SystemSettings", "ExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryDocumentSubjects", "ParentId", "DMS.DictionaryDocumentSubjects");
            DropForeignKey("DMS.DictionaryAgentFavorites", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.CustomDictionaries", "DictionaryTypeId", "DMS.CustomDictionaryTypes");
            DropForeignKey("DMS.AdminEmployeeDepartments", "EmployeeId", "DMS.DictionaryAgentEmployees");
            DropForeignKey("DMS.AdminEmployeeDepartments", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.DictionaryDepartments", "ParentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.AdminSubordinations", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.AdminSubordinations", "SubordinationTypeId", "DMS.DictionarySubordinationTypes");
            DropForeignKey("DMS.AdminSubordinations", "SourcePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DictionaryPositions", "MainExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryPositions", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DictionaryPositions", "ExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentAccesses", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentAccesses", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgents", "ResidentTypeId", "DMS.DictionaryResidentTypes");
            DropForeignKey("DMS.DocumentSubscriptions", "SubscriptionStateId", "DMS.DictionarySubscriptionStates");
            DropForeignKey("DMS.DocumentSubscriptions", "SigningTypeId", "DMS.DictionarySigningTypes");
            DropForeignKey("DMS.TemplateDocumentSendLists", "TaskId", "DMS.TemplateDocumentTasks");
            DropForeignKey("DMS.TemplateDocumentTasks", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.TemplateDocumentTasks", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocumentSendLists", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.TemplateDocumentSendLists", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.TemplateDocumentSendLists", "StageTypeId", "DMS.DictionaryStageTypes");
            DropForeignKey("DMS.TemplateDocumentSendLists", "SendTypeId", "DMS.DictionarySendTypes");
            DropForeignKey("DMS.TemplateDocumentSendLists", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocumentSendLists", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "StandartSendListId", "DMS.DictionaryStandartSendLists");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "SendListId", "DMS.TemplateDocumentSendLists");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "CompanyId", "DMS.DictionaryCompanies");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.TemplateDocuments", "SenderAgentPersonId", "DMS.DictionaryAgentPersons");
            DropForeignKey("DMS.TemplateDocuments", "SenderAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.TempDocRestrictedSendLists", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.TempDocRestrictedSendLists", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TempDocRestrictedSendLists", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.TemplateDocuments", "RegistrationJournalId", "DMS.DictionaryRegistrationJournals");
            DropForeignKey("DMS.TemplateDocumentPapers", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocuments", "DocumentTypeId", "DMS.DictionaryDocumentTypes");
            DropForeignKey("DMS.Documents", "TemplateDocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocumentFiles", "TypeId", "DMS.DictionaryFileTypes");
            DropForeignKey("DMS.TemplateDocumentFiles", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocuments", "DocumentDirectionId", "DMS.DictionaryDocumentDirections");
            DropForeignKey("DMS.TemplateDocumentAccesses", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.TemplateDocumentAccesses", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.DictionaryTags", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentTags", "TagId", "DMS.DictionaryTags");
            DropForeignKey("DMS.DocumentTags", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentSubscriptions", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.Documents", "SenderAgentPersonId", "DMS.DictionaryAgentPersons");
            DropForeignKey("DMS.Documents", "SenderAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentRestrictedSendLists", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentRestrictedSendLists", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentRestrictedSendLists", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.Documents", "RegistrationJournalId", "DMS.DictionaryRegistrationJournals");
            DropForeignKey("DMS.AdminRegistrationJournalPositions", "RegJournalAccessTypeId", "DMS.DicRegJournalAccessTypes");
            DropForeignKey("DMS.AdminRegistrationJournalPositions", "RegJournalId", "DMS.DictionaryRegistrationJournals");
            DropForeignKey("DMS.AdminRegistrationJournalPositions", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DictionaryRegistrationJournals", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.DocumentLinks", "ParentDocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentLinks", "LinkTypeId", "DMS.DictionaryLinkTypes");
            DropForeignKey("DMS.DocumentLinks", "ExecutorPositionExeTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentLinks", "ExecutorPositionExeAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentLinks", "ExecutorPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentLinks", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.Documents", "ExecutorPositionExeTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.Documents", "ExecutorPositionExeAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.Documents", "ExecutorPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEvents", "TargetPositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentEvents", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEvents", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "SourcePositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentEvents", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "SourcePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEvents", "SourceAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSubscriptions", "SendEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEvents", "ReadAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "PaperSendAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "PaperRecieveAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "PaperPlanAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "PaperListId", "DMS.DocumentPaperLists");
            DropForeignKey("DMS.DocumentPapers", "LastPaperEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapers");
            DropForeignKey("DMS.DocumentPapers", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentWaits", "ResultTypeId", "DMS.DictionaryResultTypes");
            DropForeignKey("DMS.DocumentWaits", "OnEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentWaits", "OffEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentWaits", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentWaits", "ParentId", "DMS.DocumentWaits");
            DropForeignKey("DMS.DocumentFiles", "TypeId", "DMS.DictionaryFileTypes");
            DropForeignKey("DMS.DocumentFiles", "ExecutorPositionExeTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentFiles", "ExecutorPositionExeAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentFiles", "ExecutorPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentFiles", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentFiles", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentEvents", "EventTypeId", "DMS.DictionaryEventTypes");
            DropForeignKey("DMS.DictionaryEventTypes", "ImportanceEventTypeId", "DMS.DictionaryImportanceEventTypes");
            DropForeignKey("DMS.DocumentSubscriptions", "DoneEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEvents", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentTaskAccesses", "TaskId", "DMS.DocumentTasks");
            DropForeignKey("DMS.DocumentTaskAccesses", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSendLists", "TaskId", "DMS.DocumentTasks");
            DropForeignKey("DMS.DocumentTasks", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentTasks", "PositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentTasks", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEvents", "TaskId", "DMS.DocumentTasks");
            DropForeignKey("DMS.DocumentTasks", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentTasks", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "TargetPositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentSendLists", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSendLists", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "StartEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentSendLists", "StageTypeId", "DMS.DictionaryStageTypes");
            DropForeignKey("DMS.DocumentSendLists", "SourcePositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentSendLists", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "SourcePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSendLists", "SourceAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "SendTypeId", "DMS.DictionarySendTypes");
            DropForeignKey("DMS.DocumentEvents", "SendListId", "DMS.DocumentSendLists");
            DropForeignKey("DMS.DocumentSendLists", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentSendLists", "CloseEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentSendLists", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "StandartSendListId", "DMS.DictionaryStandartSendLists");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "SendListId", "DMS.DocumentSendLists");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "CompanyId", "DMS.DictionaryCompanies");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "ParentEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEventAccessGroups", "StandartSendListId", "DMS.DictionaryStandartSendLists");
            DropForeignKey("DMS.DicStandartSendListContents", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DicStandartSendListContents", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DicStandartSendListContents", "StandartSendListId", "DMS.DictionaryStandartSendLists");
            DropForeignKey("DMS.DicStandartSendListContents", "SendTypeId", "DMS.DictionarySendTypes");
            DropForeignKey("DMS.DictionarySendTypes", "SubordinationTypeId", "DMS.DictionarySubordinationTypes");
            DropForeignKey("DMS.DicStandartSendListContents", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.DictionaryStandartSendLists", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEventAccessGroups", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEventAccessGroups", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEventAccessGroups", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentEventAccessGroups", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.DocumentEventAccessGroups", "CompanyId", "DMS.DictionaryCompanies");
            DropForeignKey("DMS.DocumentEventAccessGroups", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEventAccesses", "ReadAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEventAccesses", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentEventAccesses", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEventAccesses", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEventAccesses", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentEventAccesses", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.Documents", "DocumentTypeId", "DMS.DictionaryDocumentTypes");
            DropForeignKey("DMS.Documents", "DocumentDirectionId", "DMS.DictionaryDocumentDirections");
            DropForeignKey("DMS.DocumentAccesses", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentSubscriptions", "CertificatePositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentSubscriptions", "CertificatePositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSubscriptions", "CertificatePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSubscriptions", "CertificateId", "DMS.EncryptionCertificates");
            DropForeignKey("DMS.EncryptionCertificates", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentUsers", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.AdminLanguageValues", "LanguageId", "DMS.AdminLanguages");
            DropForeignKey("DMS.DictionaryAgentUsers", "LanguageId", "DMS.AdminLanguages");
            DropForeignKey("DMS.DictionaryAgentPersons", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentPeoples", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryCompanies", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryDepartments", "CompanyId", "DMS.DictionaryCompanies");
            DropForeignKey("DMS.DictionaryAgentEmployees", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryPositionExecutors", "AgentId", "DMS.DictionaryAgentEmployees");
            DropForeignKey("DMS.AdminUserRoles", "RoleId", "DMS.AdminRoles");
            DropForeignKey("DMS.AdminRoles", "RoleTypeId", "DMS.AdminRoleTypes");
            DropForeignKey("DMS.AdminRolePermissions", "RoleId", "DMS.AdminRoles");
            DropForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions");
            DropForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures");
            DropForeignKey("DMS.SystemFeatures", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions");
            DropForeignKey("DMS.SystemFields", "ValueTypeId", "DMS.SystemValueTypes");
            DropForeignKey("DMS.SystemFields", "ObjectId", "DMS.SystemObjects");
            DropForeignKey("DMS.SystemActions", "ObjectId", "DMS.SystemObjects");
            DropForeignKey("DMS.SystemActions", "SystemActions_Id", "DMS.SystemActions");
            DropForeignKey("DMS.SystemPermissions", "AccessTypeId", "DMS.SystemAccessTypes");
            DropForeignKey("DMS.AdminPositionRoles", "RoleId", "DMS.AdminRoles");
            DropForeignKey("DMS.AdminPositionRoles", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.AdminUserRoles", "PositionExecutorId", "DMS.DictionaryPositionExecutors");
            DropForeignKey("DMS.DictionaryPositionExecutors", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DictionaryPositionExecutors", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DictionaryPositionExecutors", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryPositionExecutors", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.DictionaryAgentContacts", "ContactTypeId", "DMS.DictionaryContactTypes");
            DropForeignKey("DMS.DictionaryAgentContacts", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentCompanies", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentPersons", "Id", "DMS.DictionaryAgentPeoples");
            DropForeignKey("DMS.DictionaryAgentPersons", "AgentCompanyId", "DMS.DictionaryAgentCompanies");
            DropForeignKey("DMS.DictionaryAgentBanks", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentAddresses", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentAddresses", "AdressTypeId", "DMS.DictionaryAddressTypes");
            DropForeignKey("DMS.DictionaryAgentAccounts", "AgentBank_Id", "DMS.DictionaryAgentBanks");
            DropForeignKey("DMS.DictionaryAgentAccounts", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentAccesses", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.DictionaryPositions", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.DictionaryPositions", "ParentId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DictionaryDepartments", "ChiefPositionId", "DMS.DictionaryPositions");
            DropIndex("DMS.SystemUIElements", new[] { "ValueTypeId" });
            DropIndex("DMS.SystemUIElements", "IX_ActionCode");
            DropIndex("DMS.SystemSearchQueryLogs", new[] { "ClientId" });
            DropIndex("DMS.SystemPatterns", new[] { "Code" });
            DropIndex("DMS.SystemFormulas", new[] { "Code" });
            DropIndex("DMS.SystemFormats", new[] { "Code" });
            DropIndex("DMS.PropertyValues", "IX_RecordId");
            DropIndex("DMS.PropertyValues", "IX_PropertyLinkRecord");
            DropIndex("DMS.PropertyLinks", new[] { "PropertyId" });
            DropIndex("DMS.PropertyLinks", "IX_ObjectPropertyFilers");
            DropIndex("DMS.Properties", new[] { "ValueTypeId" });
            DropIndex("DMS.Properties", "IX_Code");
            DropIndex("DMS.Properties", new[] { "ClientId" });
            DropIndex("DMS.SystemLogs", new[] { "LogDate" });
            DropIndex("DMS.SystemLogs", new[] { "ActionId" });
            DropIndex("DMS.SystemLogs", new[] { "ObjectId" });
            DropIndex("DMS.SystemLogs", new[] { "ExecutorAgentId" });
            DropIndex("DMS.SystemLogs", new[] { "ClientId" });
            DropIndex("DMS.FullTextIndexCashes", new[] { "ClientId" });
            DropIndex("DMS.DocumentSavedFilters", new[] { "UserId" });
            DropIndex("DMS.DocumentSavedFilters", "IX_NameUser");
            DropIndex("DMS.DocumentSavedFilters", new[] { "ClientId" });
            DropIndex("DMS.SystemSettings", new[] { "ExecutorAgentId" });
            DropIndex("DMS.SystemSettings", new[] { "SettingTypeId" });
            DropIndex("DMS.SystemSettings", new[] { "ValueTypeId" });
            DropIndex("DMS.SystemSettings", "IX_KeyExecutorAgent");
            DropIndex("DMS.SystemSettings", new[] { "ClientId" });
            DropIndex("DMS.DictionarySettingTypes", new[] { "Name" });
            DropIndex("DMS.DictionarySettingTypes", new[] { "Code" });
            DropIndex("DMS.DictionaryDocumentSubjects", new[] { "ParentId" });
            DropIndex("DMS.DictionaryDocumentSubjects", "IX_Name");
            DropIndex("DMS.DictionaryDocumentSubjects", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentFavorites", new[] { "Feature" });
            DropIndex("DMS.DictionaryAgentFavorites", new[] { "Module" });
            DropIndex("DMS.DictionaryAgentFavorites", "IX_AgentObjectModuleFeature");
            DropIndex("DMS.DictionaryAgentFavorites", "IX_Agent");
            DropIndex("DMS.CustomDictionaryTypes", "IX_Code");
            DropIndex("DMS.CustomDictionaryTypes", new[] { "ClientId" });
            DropIndex("DMS.CustomDictionaries", "IX_DictionaryTypeCode");
            DropIndex("DMS.AdminSubordinations", new[] { "SubordinationTypeId" });
            DropIndex("DMS.AdminSubordinations", new[] { "TargetPositionId" });
            DropIndex("DMS.AdminSubordinations", "IX_SourceTargetType");
            DropIndex("DMS.DictionaryResidentTypes", "IX_Name");
            DropIndex("DMS.DictionaryResidentTypes", new[] { "ClientId" });
            DropIndex("DMS.DictionarySubscriptionStates", new[] { "Name" });
            DropIndex("DMS.DictionarySubscriptionStates", new[] { "Code" });
            DropIndex("DMS.DictionarySigningTypes", new[] { "Name" });
            DropIndex("DMS.TemplateDocumentTasks", new[] { "PositionId" });
            DropIndex("DMS.TemplateDocumentTasks", "IX_DocumentTask");
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "StandartSendListId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "AgentId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "PositionId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "DepartmentId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "CompanyId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "SendListId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "DocumentId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "StageTypeId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "TaskId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "TargetAgentId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "TargetPositionId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "SendTypeId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "DocumentId" });
            DropIndex("DMS.TempDocRestrictedSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.TempDocRestrictedSendLists", new[] { "PositionId" });
            DropIndex("DMS.TempDocRestrictedSendLists", "IX_DocumentPosition");
            DropIndex("DMS.TemplateDocumentPapers", "IX_DocumentNameOrderNumber");
            DropIndex("DMS.TemplateDocumentFiles", new[] { "TypeId" });
            DropIndex("DMS.TemplateDocumentFiles", "IX_DocumentOrderNumber");
            DropIndex("DMS.TemplateDocumentFiles", "IX_DocumentNameExtention");
            DropIndex("DMS.TemplateDocumentAccesses", new[] { "PositionId" });
            DropIndex("DMS.TemplateDocumentAccesses", "IX_DocumentPosition");
            DropIndex("DMS.TemplateDocuments", new[] { "SenderAgentPersonId" });
            DropIndex("DMS.TemplateDocuments", new[] { "SenderAgentId" });
            DropIndex("DMS.TemplateDocuments", new[] { "RegistrationJournalId" });
            DropIndex("DMS.TemplateDocuments", new[] { "DocumentTypeId" });
            DropIndex("DMS.TemplateDocuments", new[] { "DocumentDirectionId" });
            DropIndex("DMS.TemplateDocuments", new[] { "EntityTypeId" });
            DropIndex("DMS.TemplateDocuments", "IX_Name");
            DropIndex("DMS.TemplateDocuments", new[] { "ClientId" });
            DropIndex("DMS.DictionaryTags", "IX_PositionName");
            DropIndex("DMS.DictionaryTags", new[] { "ClientId" });
            DropIndex("DMS.DocumentTags", new[] { "TagId" });
            DropIndex("DMS.DocumentTags", "IX_DocumentTag");
            DropIndex("DMS.DocumentTags", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentTags", new[] { "ClientId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "PositionId" });
            DropIndex("DMS.DocumentRestrictedSendLists", "IX_DocumentPosition");
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "ClientId" });
            DropIndex("DMS.DicRegJournalAccessTypes", new[] { "Name" });
            DropIndex("DMS.DicRegJournalAccessTypes", new[] { "Code" });
            DropIndex("DMS.AdminRegistrationJournalPositions", new[] { "RegJournalAccessTypeId" });
            DropIndex("DMS.AdminRegistrationJournalPositions", new[] { "RegJournalId" });
            DropIndex("DMS.AdminRegistrationJournalPositions", "IX_JournalPositionType");
            DropIndex("DMS.DictionaryRegistrationJournals", new[] { "DepartmentId" });
            DropIndex("DMS.DictionaryRegistrationJournals", "IX_Name");
            DropIndex("DMS.DictionaryRegistrationJournals", new[] { "ClientId" });
            DropIndex("DMS.DictionaryLinkTypes", new[] { "Name" });
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionExeTypeId" });
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionId" });
            DropIndex("DMS.DocumentLinks", new[] { "LinkTypeId" });
            DropIndex("DMS.DocumentLinks", new[] { "ParentDocumentId" });
            DropIndex("DMS.DocumentLinks", "IX_DocumentParentDocument");
            DropIndex("DMS.DocumentLinks", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentLinks", new[] { "ClientId" });
            DropIndex("DMS.DocumentPaperLists", new[] { "ClientId" });
            DropIndex("DMS.DocumentPapers", new[] { "LastPaperEventId" });
            DropIndex("DMS.DocumentPapers", "IX_DocumentNameOrderNumber");
            DropIndex("DMS.DocumentPapers", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentPapers", new[] { "ClientId" });
            DropIndex("DMS.DictionaryResultTypes", new[] { "Name" });
            DropIndex("DMS.DocumentWaits", new[] { "DueDate" });
            DropIndex("DMS.DocumentWaits", new[] { "ResultTypeId" });
            DropIndex("DMS.DocumentWaits", new[] { "OffEventId" });
            DropIndex("DMS.DocumentWaits", new[] { "OnEventId" });
            DropIndex("DMS.DocumentWaits", new[] { "ParentId" });
            DropIndex("DMS.DocumentWaits", new[] { "DocumentId" });
            DropIndex("DMS.DocumentWaits", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentWaits", new[] { "ClientId" });
            DropIndex("DMS.DictionaryFileTypes", new[] { "Name" });
            DropIndex("DMS.DocumentFiles", new[] { "LastChangeDate" });
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionExeTypeId" });
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionId" });
            DropIndex("DMS.DocumentFiles", new[] { "TypeId" });
            DropIndex("DMS.DocumentFiles", new[] { "EventId" });
            DropIndex("DMS.DocumentFiles", "IX_DocumentOrderNumberVersion");
            DropIndex("DMS.DocumentFiles", "IX_DocumentNameExtensionVersion");
            DropIndex("DMS.DocumentFiles", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentFiles", new[] { "ClientId" });
            DropIndex("DMS.DictionaryImportanceEventTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryEventTypes", new[] { "ImportanceEventTypeId" });
            DropIndex("DMS.DictionaryEventTypes", new[] { "Name" });
            DropIndex("DMS.DocumentTaskAccesses", "IX_TaskPosition");
            DropIndex("DMS.DocumentTaskAccesses", "IX_PositionTask");
            DropIndex("DMS.DocumentTaskAccesses", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentTaskAccesses", new[] { "ClientId" });
            DropIndex("DMS.DocumentTasks", new[] { "AgentId" });
            DropIndex("DMS.DocumentTasks", new[] { "PositionExecutorTypeId" });
            DropIndex("DMS.DocumentTasks", new[] { "PositionExecutorAgentId" });
            DropIndex("DMS.DocumentTasks", new[] { "PositionId" });
            DropIndex("DMS.DocumentTasks", "IX_DocumentTask");
            DropIndex("DMS.DocumentTasks", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentTasks", new[] { "ClientId" });
            DropIndex("DMS.DictionaryStageTypes", new[] { "Name" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "StandartSendListId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "AgentId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "PositionId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "DepartmentId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "CompanyId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "SendListId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "DocumentId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "ClientId" });
            DropIndex("DMS.DocumentSendLists", new[] { "CloseEventId" });
            DropIndex("DMS.DocumentSendLists", new[] { "StartEventId" });
            DropIndex("DMS.DocumentSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TaskId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetPositionExecutorTypeId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetPositionId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourceAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionExecutorTypeId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SendTypeId" });
            DropIndex("DMS.DocumentSendLists", new[] { "StageTypeId" });
            DropIndex("DMS.DocumentSendLists", new[] { "DocumentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentSendLists", new[] { "ClientId" });
            DropIndex("DMS.DictionarySubordinationTypes", new[] { "Name" });
            DropIndex("DMS.DictionarySubordinationTypes", new[] { "Code" });
            DropIndex("DMS.DictionarySendTypes", new[] { "SubordinationTypeId" });
            DropIndex("DMS.DictionarySendTypes", new[] { "Name" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "AccessLevelId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "TargetAgentId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "TargetPositionId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "SendTypeId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "StandartSendListId" });
            DropIndex("DMS.DictionaryStandartSendLists", "IX_PositionName");
            DropIndex("DMS.DictionaryStandartSendLists", new[] { "ClientId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "StandartSendListId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "AgentId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "PositionId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "DepartmentId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "CompanyId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "EventId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "DocumentId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "ClientId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "ReadAgentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "PositionExecutorTypeId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "AgentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "PositionId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "EventId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "DocumentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "ClientId" });
            DropIndex("DMS.DocumentEvents", new[] { "LastChangeDate" });
            DropIndex("DMS.DocumentEvents", new[] { "PaperRecieveAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "PaperSendAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "PaperPlanAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "PaperListId" });
            DropIndex("DMS.DocumentEvents", new[] { "ParentEventId" });
            DropIndex("DMS.DocumentEvents", new[] { "SendListId" });
            DropIndex("DMS.DocumentEvents", new[] { "PaperId" });
            DropIndex("DMS.DocumentEvents", new[] { "ReadAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionExecutorTypeId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourceAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionExecutorTypeId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionId" });
            DropIndex("DMS.DocumentEvents", new[] { "TaskId" });
            DropIndex("DMS.DocumentEvents", new[] { "Date" });
            DropIndex("DMS.DocumentEvents", new[] { "EventTypeId" });
            DropIndex("DMS.DocumentEvents", "IX_ReadDate");
            DropIndex("DMS.DocumentEvents", new[] { "DocumentId" });
            DropIndex("DMS.DocumentEvents", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentEvents", new[] { "ClientId" });
            DropIndex("DMS.DictionaryDocumentTypes", "IX_Name");
            DropIndex("DMS.DictionaryDocumentTypes", new[] { "ClientId" });
            DropIndex("DMS.DictionaryDocumentDirections", new[] { "Name" });
            DropIndex("DMS.DictionaryDocumentDirections", new[] { "Code" });
            DropIndex("DMS.Documents", new[] { "SenderAgentPersonId" });
            DropIndex("DMS.Documents", new[] { "SenderAgentId" });
            DropIndex("DMS.Documents", new[] { "ExecutorPositionExeTypeId" });
            DropIndex("DMS.Documents", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.Documents", new[] { "ExecutorPositionId" });
            DropIndex("DMS.Documents", new[] { "RegistrationJournalId" });
            DropIndex("DMS.Documents", new[] { "CreateDate" });
            DropIndex("DMS.Documents", new[] { "TemplateDocumentId" });
            DropIndex("DMS.Documents", new[] { "DocumentTypeId" });
            DropIndex("DMS.Documents", new[] { "DocumentDirectionId" });
            DropIndex("DMS.Documents", new[] { "EntityTypeId" });
            DropIndex("DMS.Documents", new[] { "ClientId" });
            DropIndex("DMS.Documents", "IX_IsRegistered");
            DropIndex("DMS.DocumentSubscriptions", new[] { "CertificatePositionExecutorTypeId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "CertificatePositionExecutorAgentId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "CertificatePositionId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "CertificateId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "SigningTypeId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "SubscriptionStateId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "DoneEventId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "SendEventId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "DocumentId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "ClientId" });
            DropIndex("DMS.EncryptionCertificates", new[] { "AgentId" });
            DropIndex("DMS.AdminLanguageValues", new[] { "LanguageId" });
            DropIndex("DMS.AdminLanguageValues", "IX_Label");
            DropIndex("DMS.AdminLanguageValues", new[] { "ClientId" });
            DropIndex("DMS.AdminLanguages", new[] { "Name" });
            DropIndex("DMS.AdminLanguages", new[] { "Code" });
            DropIndex("DMS.DictionaryAgentUsers", new[] { "LanguageId" });
            DropIndex("DMS.DictionaryAgentUsers", "IX_UserId");
            DropIndex("DMS.DictionaryAgentUsers", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentUsers", new[] { "Id" });
            DropIndex("DMS.DictionaryCompanies", "IX_FullName");
            DropIndex("DMS.DictionaryCompanies", new[] { "ClientId" });
            DropIndex("DMS.DictionaryCompanies", new[] { "Id" });
            DropIndex("DMS.AdminRoleTypes", new[] { "Name" });
            DropIndex("DMS.AdminRoleTypes", new[] { "Code" });
            DropIndex("DMS.SystemModules", new[] { "Code" });
            DropIndex("DMS.SystemFeatures", "IX_ModuleCode");
            DropIndex("DMS.SystemValueTypes", new[] { "Code" });
            DropIndex("DMS.SystemFields", new[] { "ValueTypeId" });
            DropIndex("DMS.SystemFields", "IX_ObjectCode");
            DropIndex("DMS.SystemObjects", new[] { "Code" });
            DropIndex("DMS.SystemActions", new[] { "SystemActions_Id" });
            DropIndex("DMS.SystemActions", "IX_ObjectCode");
            DropIndex("DMS.SystemActions", new[] { "PermissionId" });
            DropIndex("DMS.SystemAccessTypes", new[] { "Code" });
            DropIndex("DMS.SystemPermissions", "IX_ModuleFeatureAccessType");
            DropIndex("DMS.AdminRolePermissions", "IX_PermissionRole");
            DropIndex("DMS.AdminPositionRoles", new[] { "RoleId" });
            DropIndex("DMS.AdminPositionRoles", "IX_PositionRole");
            DropIndex("DMS.AdminRoles", new[] { "RoleTypeId" });
            DropIndex("DMS.AdminRoles", "IX_Name");
            DropIndex("DMS.AdminRoles", new[] { "ClientId" });
            DropIndex("DMS.AdminUserRoles", new[] { "PositionExecutorId" });
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleExecutor");
            DropIndex("DMS.AdminUserRoles", new[] { "RoleId" });
            DropIndex("DMS.DicPositionExecutorTypes", new[] { "Name" });
            DropIndex("DMS.DicPositionExecutorTypes", new[] { "Code" });
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "AccessLevelId" });
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "PositionExecutorTypeId" });
            DropIndex("DMS.DictionaryPositionExecutors", "IX_PositionAgentStartDate");
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "AgentId" });
            DropIndex("DMS.DictionaryAgentEmployees", "IX_PersonnelNumber");
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "Id" });
            DropIndex("DMS.DictionaryContactTypes", "IX_Name");
            DropIndex("DMS.DictionaryContactTypes", "IX_Code");
            DropIndex("DMS.DictionaryContactTypes", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentContacts", new[] { "ContactTypeId" });
            DropIndex("DMS.DictionaryAgentContacts", "IX_AgentContactTypeContact");
            DropIndex("DMS.DictionaryAgentPeoples", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentPeoples", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentPersons", new[] { "AgentCompanyId" });
            DropIndex("DMS.DictionaryAgentPersons", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentPersons", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentCompanies", "IX_TaxCode");
            DropIndex("DMS.DictionaryAgentCompanies", "IX_FullName");
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "Id" });
            DropIndex("DMS.DictionaryAddressTypes", "IX_Name");
            DropIndex("DMS.DictionaryAddressTypes", "IX_Code");
            DropIndex("DMS.DictionaryAddressTypes", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentAddresses", new[] { "AdressTypeId" });
            DropIndex("DMS.DictionaryAgentAddresses", "IX_AdressType");
            DropIndex("DMS.DictionaryAgentBanks", "IX_MFOCode");
            DropIndex("DMS.DictionaryAgentBanks", "IX_FullName");
            DropIndex("DMS.DictionaryAgentBanks", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentBanks", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentAccounts", new[] { "AgentBank_Id" });
            DropIndex("DMS.DictionaryAgentAccounts", "IX_AgentName");
            DropIndex("DMS.DictionaryAgents", new[] { "ResidentTypeId" });
            DropIndex("DMS.DictionaryAgents", new[] { "ClientId" });
            DropIndex("DMS.DocumentAccesses", new[] { "AccessLevelId" });
            DropIndex("DMS.DocumentAccesses", new[] { "AgentId" });
            DropIndex("DMS.DocumentAccesses", "IX_PositionDocument");
            DropIndex("DMS.DocumentAccesses", "IX_DocumentPosition");
            DropIndex("DMS.DocumentAccesses", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentAccesses", new[] { "ClientId" });
            DropIndex("DMS.DictionaryPositions", new[] { "MainExecutorAgentId" });
            DropIndex("DMS.DictionaryPositions", new[] { "PositionExecutorTypeId" });
            DropIndex("DMS.DictionaryPositions", new[] { "ExecutorAgentId" });
            DropIndex("DMS.DictionaryPositions", new[] { "DepartmentId" });
            DropIndex("DMS.DictionaryPositions", new[] { "ParentId" });
            DropIndex("DMS.DictionaryDepartments", new[] { "ChiefPositionId" });
            DropIndex("DMS.DictionaryDepartments", new[] { "ParentId" });
            DropIndex("DMS.DictionaryDepartments", "IX_CompanyParentName");
            DropIndex("DMS.AdminEmployeeDepartments", new[] { "DepartmentId" });
            DropIndex("DMS.AdminEmployeeDepartments", new[] { "EmployeeId" });
            DropIndex("DMS.AdminEmployeeDepartments", "IX_EmployeeDepartment");
            DropIndex("DMS.AdminAccessLevels", new[] { "Name" });
            DropTable("DMS.SystemUIElements");
            DropTable("DMS.SystemSearchQueryLogs");
            DropTable("DMS.SystemPatterns");
            DropTable("DMS.SystemFormulas");
            DropTable("DMS.SystemFormats");
            DropTable("DMS.PropertyValues");
            DropTable("DMS.PropertyLinks");
            DropTable("DMS.Properties");
            DropTable("DMS.SystemLogs");
            DropTable("DMS.FullTextIndexCashes");
            DropTable("DMS.DocumentSavedFilters");
            DropTable("DMS.SystemSettings");
            DropTable("DMS.DictionarySettingTypes");
            DropTable("DMS.DictionaryDocumentSubjects");
            DropTable("DMS.DictionaryAgentFavorites");
            DropTable("DMS.CustomDictionaryTypes");
            DropTable("DMS.CustomDictionaries");
            DropTable("DMS.AdminSubordinations");
            DropTable("DMS.DictionaryResidentTypes");
            DropTable("DMS.DictionarySubscriptionStates");
            DropTable("DMS.DictionarySigningTypes");
            DropTable("DMS.TemplateDocumentTasks");
            DropTable("DMS.TemplateDocumentSendListAccessGroups");
            DropTable("DMS.TemplateDocumentSendLists");
            DropTable("DMS.TempDocRestrictedSendLists");
            DropTable("DMS.TemplateDocumentPapers");
            DropTable("DMS.TemplateDocumentFiles");
            DropTable("DMS.TemplateDocumentAccesses");
            DropTable("DMS.TemplateDocuments");
            DropTable("DMS.DictionaryTags");
            DropTable("DMS.DocumentTags");
            DropTable("DMS.DocumentRestrictedSendLists");
            DropTable("DMS.DicRegJournalAccessTypes");
            DropTable("DMS.AdminRegistrationJournalPositions");
            DropTable("DMS.DictionaryRegistrationJournals");
            DropTable("DMS.DictionaryLinkTypes");
            DropTable("DMS.DocumentLinks");
            DropTable("DMS.DocumentPaperLists");
            DropTable("DMS.DocumentPapers");
            DropTable("DMS.DictionaryResultTypes");
            DropTable("DMS.DocumentWaits");
            DropTable("DMS.DictionaryFileTypes");
            DropTable("DMS.DocumentFiles");
            DropTable("DMS.DictionaryImportanceEventTypes");
            DropTable("DMS.DictionaryEventTypes");
            DropTable("DMS.DocumentTaskAccesses");
            DropTable("DMS.DocumentTasks");
            DropTable("DMS.DictionaryStageTypes");
            DropTable("DMS.DocumentSendListAccessGroups");
            DropTable("DMS.DocumentSendLists");
            DropTable("DMS.DictionarySubordinationTypes");
            DropTable("DMS.DictionarySendTypes");
            DropTable("DMS.DicStandartSendListContents");
            DropTable("DMS.DictionaryStandartSendLists");
            DropTable("DMS.DocumentEventAccessGroups");
            DropTable("DMS.DocumentEventAccesses");
            DropTable("DMS.DocumentEvents");
            DropTable("DMS.DictionaryDocumentTypes");
            DropTable("DMS.DictionaryDocumentDirections");
            DropTable("DMS.Documents");
            DropTable("DMS.DocumentSubscriptions");
            DropTable("DMS.EncryptionCertificates");
            DropTable("DMS.AdminLanguageValues");
            DropTable("DMS.AdminLanguages");
            DropTable("DMS.DictionaryAgentUsers");
            DropTable("DMS.DictionaryCompanies");
            DropTable("DMS.AdminRoleTypes");
            DropTable("DMS.SystemModules");
            DropTable("DMS.SystemFeatures");
            DropTable("DMS.SystemValueTypes");
            DropTable("DMS.SystemFields");
            DropTable("DMS.SystemObjects");
            DropTable("DMS.SystemActions");
            DropTable("DMS.SystemAccessTypes");
            DropTable("DMS.SystemPermissions");
            DropTable("DMS.AdminRolePermissions");
            DropTable("DMS.AdminPositionRoles");
            DropTable("DMS.AdminRoles");
            DropTable("DMS.AdminUserRoles");
            DropTable("DMS.DicPositionExecutorTypes");
            DropTable("DMS.DictionaryPositionExecutors");
            DropTable("DMS.DictionaryAgentEmployees");
            DropTable("DMS.DictionaryContactTypes");
            DropTable("DMS.DictionaryAgentContacts");
            DropTable("DMS.DictionaryAgentPeoples");
            DropTable("DMS.DictionaryAgentPersons");
            DropTable("DMS.DictionaryAgentCompanies");
            DropTable("DMS.DictionaryAddressTypes");
            DropTable("DMS.DictionaryAgentAddresses");
            DropTable("DMS.DictionaryAgentBanks");
            DropTable("DMS.DictionaryAgentAccounts");
            DropTable("DMS.DictionaryAgents");
            DropTable("DMS.DocumentAccesses");
            DropTable("DMS.DictionaryPositions");
            DropTable("DMS.DictionaryDepartments");
            DropTable("DMS.AdminEmployeeDepartments");
            DropTable("DMS.AdminAccessLevels");
        }
    }
}
