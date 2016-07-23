namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.AdminAccessLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 400),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
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
                        LanguageId = c.Int(nullable: false),
                        Label = c.String(maxLength: 400),
                        Value = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminLanguages", t => t.LanguageId)
                .Index(t => new { t.Label, t.LanguageId }, unique: true, name: "IX_Label")
                .Index(t => t.LanguageId);
            
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
                "DMS.DictionaryPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Name = c.String(maxLength: 2000),
                        FullName = c.String(maxLength: 2000),
                        DepartmentId = c.Int(nullable: false),
                        ExecutorAgentId = c.Int(),
                        MainExecutorAgentId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.ParentId)
                .ForeignKey("DMS.DictionaryDepartments", t => t.DepartmentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.MainExecutorAgentId)
                .Index(t => t.ParentId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ExecutorAgentId)
                .Index(t => t.MainExecutorAgentId);
            
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
                        ChiefPositionId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.ChiefPositionId)
                .ForeignKey("DMS.DictionaryDepartments", t => t.ParentId)
                .ForeignKey("DMS.DictionaryCompanies", t => t.CompanyId)
                .Index(t => new { t.CompanyId, t.ParentId, t.Name }, name: "IX_CompanyParentName")
                .Index(t => t.ParentId)
                .Index(t => t.ChiefPositionId);
            
            CreateTable(
                "DMS.DictionaryCompanies",
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
                "DMS.DocumentSavedFilters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        Icon = c.String(maxLength: 400),
                        Filter = c.String(maxLength: 2000),
                        IsCommon = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Icon, t.PositionId, t.ClientId }, unique: true, name: "IX_IconPosition")
                .Index(t => t.PositionId);
            
            CreateTable(
                "DMS.DictionaryAgents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 2000),
                        ResidentTypeId = c.Int(),
                        IsCompany = c.Boolean(nullable: false),
                        IsIndividual = c.Boolean(nullable: false),
                        IsEmployee = c.Boolean(nullable: false),
                        IsBank = c.Boolean(nullable: false),
                        IsUser = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LanguageId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminLanguages", t => t.LanguageId)
                .ForeignKey("DMS.DictionaryResidentTypes", t => t.ResidentTypeId)
                .Index(t => t.ClientId)
                .Index(t => t.ResidentTypeId)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "DMS.DictionaryAgentAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        AgentBankId = c.Int(nullable: false),
                        AccountNumber = c.String(maxLength: 2000),
                        IsMain = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryAgentBanks", t => t.AgentBankId)
                .Index(t => new { t.AgentId, t.Name }, unique: true, name: "IX_AgentName")
                .Index(t => t.AgentBankId);
            
            CreateTable(
                "DMS.DictionaryAgentBanks",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        MFOCode = c.String(maxLength: 400),
                        Swift = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.MFOCode, unique: true);
            
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
                        Name = c.String(maxLength: 400),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.Name, t.ClientId }, unique: true, name: "IX_Name");
            
            CreateTable(
                "DMS.DictionaryAgentCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false),
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
                .Index(t => t.FullName, unique: true)
                .Index(t => t.TaxCode, unique: true);
            
            CreateTable(
                "DMS.DictionaryAgentPersons",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        FullName = c.String(maxLength: 400),
                        LastName = c.String(maxLength: 2000),
                        FirstName = c.String(maxLength: 2000),
                        MiddleName = c.String(maxLength: 2000),
                        TaxCode = c.String(maxLength: 2000),
                        IsMale = c.Boolean(nullable: false),
                        PassportSerial = c.String(maxLength: 2000),
                        PassportNumber = c.Int(),
                        PassportText = c.String(maxLength: 2000),
                        PassportDate = c.DateTime(),
                        BirthDate = c.DateTime(),
                        AgentCompanyId = c.Int(),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgentCompanies", t => t.AgentCompanyId)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.FullName, unique: true)
                .Index(t => t.AgentCompanyId);
            
            CreateTable(
                "DMS.DictionaryAgentContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        ContactTypeId = c.Int(nullable: false),
                        Contact = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
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
                        PersonnelNumber = c.String(maxLength: 400),
                        AgentPersonId = c.Int(nullable: false),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.PersonnelNumber, unique: true)
                .Index(t => t.AgentPersonId, unique: true);
            
            CreateTable(
                "DMS.DictionaryAgentUsers",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.UserId, unique: true);
            
            CreateTable(
                "DMS.EncryptionCertificates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Certificate = c.Binary(),
                        Extension = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ValidFromDate = c.DateTime(),
                        ValidToDate = c.DateTime(),
                        IsPublic = c.Boolean(nullable: false),
                        IsPrivate = c.Boolean(nullable: false),
                        AgentId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.EncryptionCertificateTypes", t => t.TypeId)
                .Index(t => t.AgentId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "DMS.EncryptionCertificateTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                .Index(t => t.AgentId)
                .Index(t => new { t.PositionId, t.AgentId, t.StartDate }, unique: true, name: "IX_PositionAgentStartDate")
                .Index(t => t.PositionExecutorTypeId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.DicPositionExecutorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
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
                "DMS.DictionarySubordinationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
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
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        IsImportant = c.Boolean(nullable: false),
                        SubordinationTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionarySubordinationTypes", t => t.SubordinationTypeId)
                .Index(t => t.Name, unique: true)
                .Index(t => t.SubordinationTypeId);
            
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
                "DMS.DocumentTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryTags", t => t.TagId)
                .Index(t => new { t.DocumentId, t.TagId }, unique: true, name: "IX_DocumentTag")
                .Index(t => t.TagId);
            
            CreateTable(
                "DMS.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TemplateDocumentId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        DocumentSubjectId = c.Int(),
                        Description = c.String(maxLength: 2000),
                        AddDescription = c.String(),
                        IsRegistered = c.Boolean(),
                        RegistrationJournalId = c.Int(),
                        NumerationPrefixFormula = c.String(maxLength: 2000),
                        RegistrationNumber = c.Int(),
                        RegistrationNumberSuffix = c.String(maxLength: 100),
                        RegistrationNumberPrefix = c.String(maxLength: 100),
                        RegistrationDate = c.DateTime(),
                        ExecutorPositionId = c.Int(nullable: false),
                        ExecutorPositionExeAgentId = c.Int(nullable: false),
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
                .ForeignKey("DMS.DictionaryDocumentSubjects", t => t.DocumentSubjectId)
                .ForeignKey("DMS.DictionaryPositions", t => t.ExecutorPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorPositionExeAgentId)
                .ForeignKey("DMS.DictionaryRegistrationJournals", t => t.RegistrationJournalId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SenderAgentId)
                .ForeignKey("DMS.DictionaryAgentPersons", t => t.SenderAgentPersonId)
                .ForeignKey("DMS.TemplateDocuments", t => t.TemplateDocumentId)
                .Index(t => new { t.IsRegistered, t.Id, t.TemplateDocumentId }, name: "IX_IsRegistered")
                .Index(t => t.CreateDate)
                .Index(t => t.DocumentSubjectId)
                .Index(t => t.RegistrationJournalId)
                .Index(t => t.ExecutorPositionId)
                .Index(t => t.ExecutorPositionExeAgentId)
                .Index(t => t.SenderAgentId)
                .Index(t => t.SenderAgentPersonId);
            
            CreateTable(
                "DMS.DocumentAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        IsInWork = c.Boolean(nullable: false),
                        IsFavourite = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => new { t.DocumentId, t.PositionId }, unique: true, name: "IX_DocumentPosition")
                .Index(t => new { t.PositionId, t.DocumentId }, unique: true, name: "IX_PositionDocument")
                .Index(t => t.AccessLevelId);
            
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
                "DMS.DocumentEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        EventTypeId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Date = c.DateTime(nullable: false),
                        TaskId = c.Int(),
                        Description = c.String(maxLength: 2000),
                        AddDescription = c.String(),
                        SourcePositionId = c.Int(),
                        SourcePositionExecutorAgentId = c.Int(),
                        SourceAgentId = c.Int(),
                        TargetPositionId = c.Int(),
                        TargetPositionExecutorAgentId = c.Int(),
                        TargetAgentId = c.Int(),
                        IsAvailableWithinTask = c.Boolean(nullable: false),
                        SendDate = c.DateTime(),
                        ReadDate = c.DateTime(),
                        ReadAgentId = c.Int(),
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
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetPositionExecutorAgentId)
                .Index(t => new { t.ReadDate, t.TargetPositionId, t.DocumentId, t.SourcePositionId }, name: "IX_DocumentEvents_ReadDate")
                .Index(t => t.EventTypeId)
                .Index(t => t.Date)
                .Index(t => new { t.IsAvailableWithinTask, t.TaskId }, name: "IX_DocumentEvents_IsAvailableWithinTask")
                .Index(t => t.SourcePositionExecutorAgentId)
                .Index(t => t.SourceAgentId)
                .Index(t => t.TargetPositionExecutorAgentId)
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
                "DMS.DocumentSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Stage = c.Int(nullable: false),
                        SendTypeId = c.Int(nullable: false),
                        SourcePositionId = c.Int(nullable: false),
                        SourcePositionExecutorAgentId = c.Int(nullable: false),
                        SourceAgentId = c.Int(nullable: false),
                        TargetPositionId = c.Int(),
                        TargetPositionExecutorAgentId = c.Int(),
                        TargetAgentId = c.Int(),
                        TaskId = c.Int(),
                        Description = c.String(maxLength: 2000),
                        AddDescription = c.String(),
                        DueDate = c.DateTime(),
                        DueDay = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        IsInitial = c.Boolean(nullable: false),
                        IsWorkGroup = c.Boolean(nullable: false),
                        IsAddControl = c.Boolean(nullable: false),
                        SelfDueDate = c.DateTime(),
                        SelfDueDay = c.Int(),
                        SelfAttentionDate = c.DateTime(),
                        IsAvailableWithinTask = c.Boolean(nullable: false),
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
                .ForeignKey("DMS.DocumentEvents", t => t.StartEventId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetPositionExecutorAgentId)
                .ForeignKey("DMS.DocumentTasks", t => t.TaskId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendTypeId)
                .Index(t => t.SourcePositionId)
                .Index(t => t.SourcePositionExecutorAgentId)
                .Index(t => t.SourceAgentId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetPositionExecutorAgentId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.TaskId)
                .Index(t => t.AccessLevelId)
                .Index(t => t.StartEventId)
                .Index(t => t.CloseEventId);
            
            CreateTable(
                "DMS.DocumentTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        PositionExecutorAgentId = c.Int(nullable: false),
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
                .Index(t => new { t.DocumentId, t.Task }, unique: true, name: "IX_DocumentTask")
                .Index(t => t.PositionId)
                .Index(t => t.PositionExecutorAgentId)
                .Index(t => t.AgentId);
            
            CreateTable(
                "DMS.DocumentTaskAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DocumentTasks", t => t.TaskId)
                .Index(t => new { t.PositionId, t.TaskId }, unique: true, name: "IX_PositionTask")
                .Index(t => new { t.TaskId, t.PositionId }, unique: true, name: "IX_TaskPosition");
            
            CreateTable(
                "DMS.DocumentSubscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DocumentEvents", t => t.DoneEventId)
                .ForeignKey("DMS.DocumentEvents", t => t.SendEventId)
                .ForeignKey("DMS.DictionarySubscriptionStates", t => t.SubscriptionStateId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendEventId)
                .Index(t => t.DoneEventId)
                .Index(t => t.SubscriptionStateId);
            
            CreateTable(
                "DMS.DictionarySubscriptionStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        IsSuccess = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DictionaryEventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        SourceDescription = c.String(maxLength: 2000),
                        TargetDescription = c.String(maxLength: 2000),
                        WaitDescription = c.String(maxLength: 2000),
                        ImportanceEventTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryImportanceEventTypes", t => t.ImportanceEventTypeId)
                .Index(t => t.Name, unique: true)
                .Index(t => t.ImportanceEventTypeId);
            
            CreateTable(
                "DMS.DictionaryImportanceEventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DocumentWaits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        OnEventId = c.Int(nullable: false),
                        OffEventId = c.Int(),
                        ResultTypeId = c.Int(),
                        DueDate = c.DateTime(),
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
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        IsExecute = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "DMS.DocumentPapers",
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
                        IsInWork = c.Boolean(nullable: false),
                        LastPaperEventId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DocumentEvents", t => t.LastPaperEventId)
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
                "DMS.DocumentFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 200),
                        OrderNumber = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        Extension = c.String(maxLength: 200),
                        FileType = c.String(maxLength: 2000),
                        FileSize = c.Long(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Content = c.String(),
                        IsAdditional = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsWorkedOut = c.Boolean(),
                        Hash = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsMainVersion = c.Boolean(nullable: false),
                        ExecutorPositionId = c.Int(nullable: false),
                        ExecutorPositionExeAgentId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.ExecutorPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorPositionExeAgentId)
                .Index(t => new { t.DocumentId, t.Name, t.Extension, t.Version }, unique: true, name: "IX_DocumentNameExtensionVersion")
                .Index(t => new { t.DocumentId, t.OrderNumber, t.Version }, unique: true, name: "IX_DocumentOrderNumberVersion")
                .Index(t => t.ExecutorPositionId)
                .Index(t => t.ExecutorPositionExeAgentId)
                .Index(t => t.LastChangeDate);
            
            CreateTable(
                "DMS.DocumentLinks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        ParentDocumentId = c.Int(nullable: false),
                        LinkTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryLinkTypes", t => t.LinkTypeId)
                .ForeignKey("DMS.Documents", t => t.ParentDocumentId)
                .Index(t => new { t.DocumentId, t.ParentDocumentId }, unique: true, name: "IX_DocumentParentDocument")
                .Index(t => t.ParentDocumentId)
                .Index(t => t.LinkTypeId);
            
            CreateTable(
                "DMS.DictionaryLinkTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 400),
                        IsImportant = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
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
                .Index(t => new { t.Name, t.DepartmentId, t.Index, t.ClientId }, unique: true, name: "IX_Name");
            
            CreateTable(
                "DMS.DocumentRestrictedSendLists",
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
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => new { t.DocumentId, t.PositionId }, unique: true, name: "IX_DocumentPosition")
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.TemplateDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        IsHard = c.Boolean(nullable: false),
                        DocumentDirectionId = c.Int(nullable: false),
                        DocumentTypeId = c.Int(nullable: false),
                        DocumentSubjectId = c.Int(),
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
                .ForeignKey("DMS.DictionaryDocumentSubjects", t => t.DocumentSubjectId)
                .ForeignKey("DMS.DictionaryDocumentTypes", t => t.DocumentTypeId)
                .ForeignKey("DMS.DictionaryRegistrationJournals", t => t.RegistrationJournalId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SenderAgentId)
                .ForeignKey("DMS.DictionaryAgentPersons", t => t.SenderAgentPersonId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Name, t.ClientId }, unique: true, name: "IX_Name")
                .Index(t => t.DocumentDirectionId)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.DocumentSubjectId)
                .Index(t => t.RegistrationJournalId)
                .Index(t => t.SenderAgentId)
                .Index(t => t.SenderAgentPersonId);
            
            CreateTable(
                "DMS.DictionaryDocumentDirections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
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
                        IsAdditional = c.Boolean(nullable: false),
                        Hash = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .Index(t => new { t.DocumentId, t.Name, t.Extention }, unique: true, name: "IX_DocumentNameExtention")
                .Index(t => new { t.DocumentId, t.OrderNumber }, unique: true, name: "IX_DocumentOrderNumber");
            
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
                        SourcePositionId = c.Int(),
                        TargetPositionId = c.Int(),
                        TargetAgentId = c.Int(),
                        TaskId = c.Int(),
                        Description = c.String(maxLength: 2000),
                        Stage = c.Int(nullable: false),
                        DueDay = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        IsWorkGroup = c.Boolean(nullable: false),
                        IsAddControl = c.Boolean(nullable: false),
                        SelfDueDate = c.DateTime(),
                        SelfDueDay = c.Int(),
                        SelfAttentionDate = c.DateTime(),
                        IsAvailableWithinTask = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("DMS.DictionarySendTypes", t => t.SendTypeId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SourcePositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("DMS.TemplateDocumentTasks", t => t.TaskId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendTypeId)
                .Index(t => t.SourcePositionId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.TaskId)
                .Index(t => t.AccessLevelId);
            
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
                "DMS.AdminRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.Name, t.ClientId }, unique: true, name: "IX_Name");
            
            CreateTable(
                "DMS.AdminRoleActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        RecordId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemActions", t => t.ActionId)
                .ForeignKey("DMS.AdminRoles", t => t.RoleId)
                .Index(t => new { t.ActionId, t.RoleId, t.RecordId }, unique: true, name: "IX_ActionRoleRecord")
                .Index(t => t.RoleId);
            
            CreateTable(
                "DMS.SystemActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        API = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        Category = c.String(maxLength: 2000),
                        IsGrantable = c.Boolean(nullable: false),
                        IsGrantableByRecordId = c.Boolean(nullable: false),
                        IsVisible = c.Boolean(nullable: false),
                        GrantId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemActions", t => t.GrantId)
                .ForeignKey("DMS.SystemObjects", t => t.ObjectId)
                .Index(t => new { t.ObjectId, t.Code }, unique: true, name: "IX_ObjectCode")
                .Index(t => t.GrantId);
            
            CreateTable(
                "DMS.SystemObjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.AdminUserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.UserId)
                .ForeignKey("DMS.AdminRoles", t => t.RoleId)
                .Index(t => new { t.UserId, t.RoleId, t.StartDate }, unique: true, name: "IX_UserRoleStartDate")
                .Index(t => t.RoleId);
            
            CreateTable(
                "DMS.CustomDictionaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DictionaryTypeId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
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
                        Description = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => new { t.Code, t.ClientId }, unique: true, name: "IX_Code");
            
            CreateTable(
                "DMS.FullTextIndexCashes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        ObjectType = c.Int(nullable: false),
                        OperationType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId)
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
                .Index(t => new { t.PropertyLinkId, t.RecordId }, unique: true, name: "IX_PropertyLinkRecord");
            
            CreateTable(
                "DMS.SystemSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Key = c.String(maxLength: 400),
                        Value = c.String(maxLength: 2000),
                        ExecutorAgentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorAgentId)
                .Index(t => t.ClientId)
                .Index(t => new { t.Key, t.ExecutorAgentId, t.ClientId }, unique: true, name: "IX_KeyExecutorAgent")
                .Index(t => t.ExecutorAgentId);
            
            CreateTable(
                "DMS.SystemUIElements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
            DropForeignKey("DMS.SystemSettings", "ExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.Properties", "ValueTypeId", "DMS.SystemValueTypes");
            DropForeignKey("DMS.PropertyValues", "PropertyLinkId", "DMS.PropertyLinks");
            DropForeignKey("DMS.PropertyLinks", "PropertyId", "DMS.Properties");
            DropForeignKey("DMS.PropertyLinks", "ObjectId", "DMS.SystemObjects");
            DropForeignKey("DMS.CustomDictionaries", "DictionaryTypeId", "DMS.CustomDictionaryTypes");
            DropForeignKey("DMS.AdminUserRoles", "RoleId", "DMS.AdminRoles");
            DropForeignKey("DMS.AdminUserRoles", "UserId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.AdminRoleActions", "RoleId", "DMS.AdminRoles");
            DropForeignKey("DMS.AdminRoleActions", "ActionId", "DMS.SystemActions");
            DropForeignKey("DMS.SystemFields", "ValueTypeId", "DMS.SystemValueTypes");
            DropForeignKey("DMS.SystemFields", "ObjectId", "DMS.SystemObjects");
            DropForeignKey("DMS.SystemActions", "ObjectId", "DMS.SystemObjects");
            DropForeignKey("DMS.SystemActions", "GrantId", "DMS.SystemActions");
            DropForeignKey("DMS.AdminPositionRoles", "RoleId", "DMS.AdminRoles");
            DropForeignKey("DMS.DictionaryTags", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentTags", "TagId", "DMS.DictionaryTags");
            DropForeignKey("DMS.Documents", "TemplateDocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocumentSendLists", "TaskId", "DMS.TemplateDocumentTasks");
            DropForeignKey("DMS.TemplateDocumentTasks", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.TemplateDocumentTasks", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocumentSendLists", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.TemplateDocumentSendLists", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.TemplateDocumentSendLists", "SourcePositionId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.TemplateDocumentSendLists", "SendTypeId", "DMS.DictionarySendTypes");
            DropForeignKey("DMS.TemplateDocumentSendLists", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocumentSendLists", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.TemplateDocuments", "SenderAgentPersonId", "DMS.DictionaryAgentPersons");
            DropForeignKey("DMS.TemplateDocuments", "SenderAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.TempDocRestrictedSendLists", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.TempDocRestrictedSendLists", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TempDocRestrictedSendLists", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.TemplateDocuments", "RegistrationJournalId", "DMS.DictionaryRegistrationJournals");
            DropForeignKey("DMS.TemplateDocuments", "DocumentTypeId", "DMS.DictionaryDocumentTypes");
            DropForeignKey("DMS.TemplateDocuments", "DocumentSubjectId", "DMS.DictionaryDocumentSubjects");
            DropForeignKey("DMS.TemplateDocumentFiles", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocuments", "DocumentDirectionId", "DMS.DictionaryDocumentDirections");
            DropForeignKey("DMS.DocumentTags", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.Documents", "SenderAgentPersonId", "DMS.DictionaryAgentPersons");
            DropForeignKey("DMS.Documents", "SenderAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentRestrictedSendLists", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentRestrictedSendLists", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentRestrictedSendLists", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.Documents", "RegistrationJournalId", "DMS.DictionaryRegistrationJournals");
            DropForeignKey("DMS.DictionaryRegistrationJournals", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.DocumentLinks", "ParentDocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentLinks", "LinkTypeId", "DMS.DictionaryLinkTypes");
            DropForeignKey("DMS.DocumentLinks", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentFiles", "ExecutorPositionExeAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentFiles", "ExecutorPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentFiles", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.Documents", "ExecutorPositionExeAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.Documents", "ExecutorPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEvents", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEvents", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "SourcePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEvents", "SourceAgentId", "DMS.DictionaryAgents");
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
            DropForeignKey("DMS.DocumentEvents", "EventTypeId", "DMS.DictionaryEventTypes");
            DropForeignKey("DMS.DictionaryEventTypes", "ImportanceEventTypeId", "DMS.DictionaryImportanceEventTypes");
            DropForeignKey("DMS.DocumentSubscriptions", "SubscriptionStateId", "DMS.DictionarySubscriptionStates");
            DropForeignKey("DMS.DocumentSubscriptions", "SendEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentSubscriptions", "DoneEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentSubscriptions", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentEvents", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentTaskAccesses", "TaskId", "DMS.DocumentTasks");
            DropForeignKey("DMS.DocumentTaskAccesses", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSendLists", "TaskId", "DMS.DocumentTasks");
            DropForeignKey("DMS.DocumentTasks", "PositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentTasks", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEvents", "TaskId", "DMS.DocumentTasks");
            DropForeignKey("DMS.DocumentTasks", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentTasks", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSendLists", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "StartEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentSendLists", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "SourcePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSendLists", "SourceAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "SendTypeId", "DMS.DictionarySendTypes");
            DropForeignKey("DMS.DocumentEvents", "SendListId", "DMS.DocumentSendLists");
            DropForeignKey("DMS.DocumentSendLists", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentSendLists", "CloseEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentSendLists", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.DocumentEvents", "ParentEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.Documents", "DocumentSubjectId", "DMS.DictionaryDocumentSubjects");
            DropForeignKey("DMS.DictionaryDocumentSubjects", "ParentId", "DMS.DictionaryDocumentSubjects");
            DropForeignKey("DMS.DocumentAccesses", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentAccesses", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentAccesses", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.DicStandartSendListContents", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DicStandartSendListContents", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DicStandartSendListContents", "StandartSendListId", "DMS.DictionaryStandartSendLists");
            DropForeignKey("DMS.DicStandartSendListContents", "SendTypeId", "DMS.DictionarySendTypes");
            DropForeignKey("DMS.DictionarySendTypes", "SubordinationTypeId", "DMS.DictionarySubordinationTypes");
            DropForeignKey("DMS.DicStandartSendListContents", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.DictionaryStandartSendLists", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.AdminSubordinations", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.AdminSubordinations", "SubordinationTypeId", "DMS.DictionarySubordinationTypes");
            DropForeignKey("DMS.AdminSubordinations", "SourcePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.AdminPositionRoles", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DictionaryPositionExecutors", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DictionaryPositionExecutors", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DictionaryPositionExecutors", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryPositionExecutors", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.DictionaryPositions", "MainExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryPositions", "ExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgents", "ResidentTypeId", "DMS.DictionaryResidentTypes");
            DropForeignKey("DMS.DictionaryAgents", "LanguageId", "DMS.AdminLanguages");
            DropForeignKey("DMS.EncryptionCertificates", "TypeId", "DMS.EncryptionCertificateTypes");
            DropForeignKey("DMS.EncryptionCertificates", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentUsers", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentPersons", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentEmployees", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentContacts", "ContactTypeId", "DMS.DictionaryContactTypes");
            DropForeignKey("DMS.DictionaryAgentContacts", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentCompanies", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentPersons", "AgentCompanyId", "DMS.DictionaryAgentCompanies");
            DropForeignKey("DMS.DictionaryAgentBanks", "Id", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentAddresses", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgentAddresses", "AdressTypeId", "DMS.DictionaryAddressTypes");
            DropForeignKey("DMS.DictionaryAgentAccounts", "AgentBankId", "DMS.DictionaryAgentBanks");
            DropForeignKey("DMS.DictionaryAgentAccounts", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSavedFilters", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DictionaryPositions", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.DictionaryPositions", "ParentId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DictionaryDepartments", "CompanyId", "DMS.DictionaryCompanies");
            DropForeignKey("DMS.DictionaryDepartments", "ParentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.DictionaryDepartments", "ChiefPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.AdminLanguageValues", "LanguageId", "DMS.AdminLanguages");
            DropIndex("DMS.SystemUIElements", new[] { "ValueTypeId" });
            DropIndex("DMS.SystemUIElements", "IX_ActionCode");
            DropIndex("DMS.SystemSettings", new[] { "ExecutorAgentId" });
            DropIndex("DMS.SystemSettings", "IX_KeyExecutorAgent");
            DropIndex("DMS.SystemSettings", new[] { "ClientId" });
            DropIndex("DMS.PropertyValues", "IX_PropertyLinkRecord");
            DropIndex("DMS.PropertyLinks", new[] { "PropertyId" });
            DropIndex("DMS.PropertyLinks", "IX_ObjectPropertyFilers");
            DropIndex("DMS.Properties", new[] { "ValueTypeId" });
            DropIndex("DMS.Properties", "IX_Code");
            DropIndex("DMS.Properties", new[] { "ClientId" });
            DropIndex("DMS.SystemLogs", new[] { "LogDate" });
            DropIndex("DMS.SystemLogs", new[] { "ClientId" });
            DropIndex("DMS.CustomDictionaryTypes", "IX_Code");
            DropIndex("DMS.CustomDictionaryTypes", new[] { "ClientId" });
            DropIndex("DMS.CustomDictionaries", "IX_DictionaryTypeCode");
            DropIndex("DMS.AdminUserRoles", new[] { "RoleId" });
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleStartDate");
            DropIndex("DMS.SystemValueTypes", new[] { "Code" });
            DropIndex("DMS.SystemFields", new[] { "ValueTypeId" });
            DropIndex("DMS.SystemFields", "IX_ObjectCode");
            DropIndex("DMS.SystemObjects", new[] { "Code" });
            DropIndex("DMS.SystemActions", new[] { "GrantId" });
            DropIndex("DMS.SystemActions", "IX_ObjectCode");
            DropIndex("DMS.AdminRoleActions", new[] { "RoleId" });
            DropIndex("DMS.AdminRoleActions", "IX_ActionRoleRecord");
            DropIndex("DMS.AdminRoles", "IX_Name");
            DropIndex("DMS.AdminRoles", new[] { "ClientId" });
            DropIndex("DMS.TemplateDocumentTasks", new[] { "PositionId" });
            DropIndex("DMS.TemplateDocumentTasks", "IX_DocumentTask");
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "TaskId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "TargetAgentId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "TargetPositionId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "SourcePositionId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "SendTypeId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "DocumentId" });
            DropIndex("DMS.TempDocRestrictedSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.TempDocRestrictedSendLists", new[] { "PositionId" });
            DropIndex("DMS.TempDocRestrictedSendLists", "IX_DocumentPosition");
            DropIndex("DMS.DictionaryDocumentTypes", "IX_Name");
            DropIndex("DMS.DictionaryDocumentTypes", new[] { "ClientId" });
            DropIndex("DMS.TemplateDocumentFiles", "IX_DocumentOrderNumber");
            DropIndex("DMS.TemplateDocumentFiles", "IX_DocumentNameExtention");
            DropIndex("DMS.DictionaryDocumentDirections", new[] { "Name" });
            DropIndex("DMS.DictionaryDocumentDirections", new[] { "Code" });
            DropIndex("DMS.TemplateDocuments", new[] { "SenderAgentPersonId" });
            DropIndex("DMS.TemplateDocuments", new[] { "SenderAgentId" });
            DropIndex("DMS.TemplateDocuments", new[] { "RegistrationJournalId" });
            DropIndex("DMS.TemplateDocuments", new[] { "DocumentSubjectId" });
            DropIndex("DMS.TemplateDocuments", new[] { "DocumentTypeId" });
            DropIndex("DMS.TemplateDocuments", new[] { "DocumentDirectionId" });
            DropIndex("DMS.TemplateDocuments", "IX_Name");
            DropIndex("DMS.TemplateDocuments", new[] { "ClientId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "PositionId" });
            DropIndex("DMS.DocumentRestrictedSendLists", "IX_DocumentPosition");
            DropIndex("DMS.DictionaryRegistrationJournals", "IX_Name");
            DropIndex("DMS.DictionaryRegistrationJournals", new[] { "ClientId" });
            DropIndex("DMS.DictionaryLinkTypes", new[] { "Name" });
            DropIndex("DMS.DocumentLinks", new[] { "LinkTypeId" });
            DropIndex("DMS.DocumentLinks", new[] { "ParentDocumentId" });
            DropIndex("DMS.DocumentLinks", "IX_DocumentParentDocument");
            DropIndex("DMS.DocumentFiles", new[] { "LastChangeDate" });
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionId" });
            DropIndex("DMS.DocumentFiles", "IX_DocumentOrderNumberVersion");
            DropIndex("DMS.DocumentFiles", "IX_DocumentNameExtensionVersion");
            DropIndex("DMS.DocumentPaperLists", new[] { "ClientId" });
            DropIndex("DMS.DocumentPapers", new[] { "LastPaperEventId" });
            DropIndex("DMS.DocumentPapers", "IX_DocumentNameOrderNumber");
            DropIndex("DMS.DictionaryResultTypes", new[] { "Name" });
            DropIndex("DMS.DocumentWaits", new[] { "DueDate" });
            DropIndex("DMS.DocumentWaits", new[] { "ResultTypeId" });
            DropIndex("DMS.DocumentWaits", new[] { "OffEventId" });
            DropIndex("DMS.DocumentWaits", new[] { "OnEventId" });
            DropIndex("DMS.DocumentWaits", new[] { "ParentId" });
            DropIndex("DMS.DocumentWaits", new[] { "DocumentId" });
            DropIndex("DMS.DictionaryImportanceEventTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryEventTypes", new[] { "ImportanceEventTypeId" });
            DropIndex("DMS.DictionaryEventTypes", new[] { "Name" });
            DropIndex("DMS.DictionarySubscriptionStates", new[] { "Name" });
            DropIndex("DMS.DictionarySubscriptionStates", new[] { "Code" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "SubscriptionStateId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "DoneEventId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "SendEventId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "DocumentId" });
            DropIndex("DMS.DocumentTaskAccesses", "IX_TaskPosition");
            DropIndex("DMS.DocumentTaskAccesses", "IX_PositionTask");
            DropIndex("DMS.DocumentTasks", new[] { "AgentId" });
            DropIndex("DMS.DocumentTasks", new[] { "PositionExecutorAgentId" });
            DropIndex("DMS.DocumentTasks", new[] { "PositionId" });
            DropIndex("DMS.DocumentTasks", "IX_DocumentTask");
            DropIndex("DMS.DocumentSendLists", new[] { "CloseEventId" });
            DropIndex("DMS.DocumentSendLists", new[] { "StartEventId" });
            DropIndex("DMS.DocumentSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TaskId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetPositionId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourceAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SendTypeId" });
            DropIndex("DMS.DocumentSendLists", new[] { "DocumentId" });
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
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourceAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.DocumentEvents", "IX_DocumentEvents_IsAvailableWithinTask");
            DropIndex("DMS.DocumentEvents", new[] { "Date" });
            DropIndex("DMS.DocumentEvents", new[] { "EventTypeId" });
            DropIndex("DMS.DocumentEvents", "IX_DocumentEvents_ReadDate");
            DropIndex("DMS.DictionaryDocumentSubjects", new[] { "ParentId" });
            DropIndex("DMS.DictionaryDocumentSubjects", "IX_Name");
            DropIndex("DMS.DictionaryDocumentSubjects", new[] { "ClientId" });
            DropIndex("DMS.DocumentAccesses", new[] { "AccessLevelId" });
            DropIndex("DMS.DocumentAccesses", "IX_PositionDocument");
            DropIndex("DMS.DocumentAccesses", "IX_DocumentPosition");
            DropIndex("DMS.Documents", new[] { "SenderAgentPersonId" });
            DropIndex("DMS.Documents", new[] { "SenderAgentId" });
            DropIndex("DMS.Documents", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.Documents", new[] { "ExecutorPositionId" });
            DropIndex("DMS.Documents", new[] { "RegistrationJournalId" });
            DropIndex("DMS.Documents", new[] { "DocumentSubjectId" });
            DropIndex("DMS.Documents", new[] { "CreateDate" });
            DropIndex("DMS.Documents", "IX_IsRegistered");
            DropIndex("DMS.DocumentTags", new[] { "TagId" });
            DropIndex("DMS.DocumentTags", "IX_DocumentTag");
            DropIndex("DMS.DictionaryTags", "IX_PositionName");
            DropIndex("DMS.DictionaryTags", new[] { "ClientId" });
            DropIndex("DMS.DictionarySendTypes", new[] { "SubordinationTypeId" });
            DropIndex("DMS.DictionarySendTypes", new[] { "Name" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "AccessLevelId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "TargetAgentId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "TargetPositionId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "SendTypeId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "StandartSendListId" });
            DropIndex("DMS.DictionaryStandartSendLists", "IX_PositionName");
            DropIndex("DMS.DictionaryStandartSendLists", new[] { "ClientId" });
            DropIndex("DMS.DictionarySubordinationTypes", new[] { "Name" });
            DropIndex("DMS.DictionarySubordinationTypes", new[] { "Code" });
            DropIndex("DMS.AdminSubordinations", new[] { "SubordinationTypeId" });
            DropIndex("DMS.AdminSubordinations", new[] { "TargetPositionId" });
            DropIndex("DMS.AdminSubordinations", "IX_SourceTargetType");
            DropIndex("DMS.DicPositionExecutorTypes", new[] { "Name" });
            DropIndex("DMS.DicPositionExecutorTypes", new[] { "Code" });
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "AccessLevelId" });
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "PositionExecutorTypeId" });
            DropIndex("DMS.DictionaryPositionExecutors", "IX_PositionAgentStartDate");
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "AgentId" });
            DropIndex("DMS.DictionaryResidentTypes", "IX_Name");
            DropIndex("DMS.DictionaryResidentTypes", new[] { "ClientId" });
            DropIndex("DMS.EncryptionCertificates", new[] { "TypeId" });
            DropIndex("DMS.EncryptionCertificates", new[] { "AgentId" });
            DropIndex("DMS.DictionaryAgentUsers", new[] { "UserId" });
            DropIndex("DMS.DictionaryAgentUsers", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "AgentPersonId" });
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "PersonnelNumber" });
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "Id" });
            DropIndex("DMS.DictionaryContactTypes", "IX_Name");
            DropIndex("DMS.DictionaryContactTypes", "IX_Code");
            DropIndex("DMS.DictionaryContactTypes", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentContacts", new[] { "ContactTypeId" });
            DropIndex("DMS.DictionaryAgentContacts", "IX_AgentContactTypeContact");
            DropIndex("DMS.DictionaryAgentPersons", new[] { "AgentCompanyId" });
            DropIndex("DMS.DictionaryAgentPersons", new[] { "FullName" });
            DropIndex("DMS.DictionaryAgentPersons", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "TaxCode" });
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "FullName" });
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "Id" });
            DropIndex("DMS.DictionaryAddressTypes", "IX_Name");
            DropIndex("DMS.DictionaryAddressTypes", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentAddresses", new[] { "AdressTypeId" });
            DropIndex("DMS.DictionaryAgentAddresses", "IX_AdressType");
            DropIndex("DMS.DictionaryAgentBanks", new[] { "MFOCode" });
            DropIndex("DMS.DictionaryAgentBanks", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentAccounts", new[] { "AgentBankId" });
            DropIndex("DMS.DictionaryAgentAccounts", "IX_AgentName");
            DropIndex("DMS.DictionaryAgents", new[] { "LanguageId" });
            DropIndex("DMS.DictionaryAgents", new[] { "ResidentTypeId" });
            DropIndex("DMS.DictionaryAgents", new[] { "ClientId" });
            DropIndex("DMS.DocumentSavedFilters", new[] { "PositionId" });
            DropIndex("DMS.DocumentSavedFilters", "IX_IconPosition");
            DropIndex("DMS.DocumentSavedFilters", new[] { "ClientId" });
            DropIndex("DMS.DictionaryCompanies", "IX_Name");
            DropIndex("DMS.DictionaryCompanies", new[] { "ClientId" });
            DropIndex("DMS.DictionaryDepartments", new[] { "ChiefPositionId" });
            DropIndex("DMS.DictionaryDepartments", new[] { "ParentId" });
            DropIndex("DMS.DictionaryDepartments", "IX_CompanyParentName");
            DropIndex("DMS.DictionaryPositions", new[] { "MainExecutorAgentId" });
            DropIndex("DMS.DictionaryPositions", new[] { "ExecutorAgentId" });
            DropIndex("DMS.DictionaryPositions", new[] { "DepartmentId" });
            DropIndex("DMS.DictionaryPositions", new[] { "ParentId" });
            DropIndex("DMS.AdminPositionRoles", new[] { "RoleId" });
            DropIndex("DMS.AdminPositionRoles", "IX_PositionRole");
            DropIndex("DMS.AdminLanguageValues", new[] { "LanguageId" });
            DropIndex("DMS.AdminLanguageValues", "IX_Label");
            DropIndex("DMS.AdminLanguages", new[] { "Name" });
            DropIndex("DMS.AdminLanguages", new[] { "Code" });
            DropIndex("DMS.AdminAccessLevels", new[] { "Name" });
            DropTable("DMS.SystemUIElements");
            DropTable("DMS.SystemSettings");
            DropTable("DMS.PropertyValues");
            DropTable("DMS.PropertyLinks");
            DropTable("DMS.Properties");
            DropTable("DMS.SystemLogs");
            DropTable("DMS.FullTextIndexCashes");
            DropTable("DMS.CustomDictionaryTypes");
            DropTable("DMS.CustomDictionaries");
            DropTable("DMS.AdminUserRoles");
            DropTable("DMS.SystemValueTypes");
            DropTable("DMS.SystemFields");
            DropTable("DMS.SystemObjects");
            DropTable("DMS.SystemActions");
            DropTable("DMS.AdminRoleActions");
            DropTable("DMS.AdminRoles");
            DropTable("DMS.TemplateDocumentTasks");
            DropTable("DMS.TemplateDocumentSendLists");
            DropTable("DMS.TempDocRestrictedSendLists");
            DropTable("DMS.DictionaryDocumentTypes");
            DropTable("DMS.TemplateDocumentFiles");
            DropTable("DMS.DictionaryDocumentDirections");
            DropTable("DMS.TemplateDocuments");
            DropTable("DMS.DocumentRestrictedSendLists");
            DropTable("DMS.DictionaryRegistrationJournals");
            DropTable("DMS.DictionaryLinkTypes");
            DropTable("DMS.DocumentLinks");
            DropTable("DMS.DocumentFiles");
            DropTable("DMS.DocumentPaperLists");
            DropTable("DMS.DocumentPapers");
            DropTable("DMS.DictionaryResultTypes");
            DropTable("DMS.DocumentWaits");
            DropTable("DMS.DictionaryImportanceEventTypes");
            DropTable("DMS.DictionaryEventTypes");
            DropTable("DMS.DictionarySubscriptionStates");
            DropTable("DMS.DocumentSubscriptions");
            DropTable("DMS.DocumentTaskAccesses");
            DropTable("DMS.DocumentTasks");
            DropTable("DMS.DocumentSendLists");
            DropTable("DMS.DocumentEvents");
            DropTable("DMS.DictionaryDocumentSubjects");
            DropTable("DMS.DocumentAccesses");
            DropTable("DMS.Documents");
            DropTable("DMS.DocumentTags");
            DropTable("DMS.DictionaryTags");
            DropTable("DMS.DictionarySendTypes");
            DropTable("DMS.DicStandartSendListContents");
            DropTable("DMS.DictionaryStandartSendLists");
            DropTable("DMS.DictionarySubordinationTypes");
            DropTable("DMS.AdminSubordinations");
            DropTable("DMS.DicPositionExecutorTypes");
            DropTable("DMS.DictionaryPositionExecutors");
            DropTable("DMS.DictionaryResidentTypes");
            DropTable("DMS.EncryptionCertificateTypes");
            DropTable("DMS.EncryptionCertificates");
            DropTable("DMS.DictionaryAgentUsers");
            DropTable("DMS.DictionaryAgentEmployees");
            DropTable("DMS.DictionaryContactTypes");
            DropTable("DMS.DictionaryAgentContacts");
            DropTable("DMS.DictionaryAgentPersons");
            DropTable("DMS.DictionaryAgentCompanies");
            DropTable("DMS.DictionaryAddressTypes");
            DropTable("DMS.DictionaryAgentAddresses");
            DropTable("DMS.DictionaryAgentBanks");
            DropTable("DMS.DictionaryAgentAccounts");
            DropTable("DMS.DictionaryAgents");
            DropTable("DMS.DocumentSavedFilters");
            DropTable("DMS.DictionaryCompanies");
            DropTable("DMS.DictionaryDepartments");
            DropTable("DMS.DictionaryPositions");
            DropTable("DMS.AdminPositionRoles");
            DropTable("DMS.AdminLanguageValues");
            DropTable("DMS.AdminLanguages");
            DropTable("DMS.AdminAccessLevels");
        }
    }
}
