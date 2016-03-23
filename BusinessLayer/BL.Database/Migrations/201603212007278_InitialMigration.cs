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
                        Name = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.AdminLanguages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.AdminLanguageValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        Label = c.String(maxLength: 2000),
                        Value = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.AdminLanguages", t => t.LanguageId)
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
                .Index(t => t.RoleId)
                .Index(t => t.PositionId);
            
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
                        Name = c.String(maxLength: 2000),
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
                .Index(t => t.ParentId)
                .Index(t => t.CompanyId)
                .Index(t => t.ChiefPositionId);
            
            CreateTable(
                "DMS.DictionaryCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.DocumentSavedFilters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PositionId = c.Int(),
                        Icon = c.String(maxLength: 2000),
                        Filter = c.String(maxLength: 2000),
                        IsCommon = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "DMS.DictionaryAgents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                .Index(t => t.ResidentTypeId)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "DMS.DictionaryAgentAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 2000),
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
                .Index(t => t.AgentId)
                .Index(t => t.AgentBankId);
            
            CreateTable(
                "DMS.DictionaryAgentBanks",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        MFOCode = c.String(maxLength: 2000),
                        Swift = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id);
            
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
                .Index(t => t.AgentId)
                .Index(t => t.AdressTypeId);
            
            CreateTable(
                "DMS.DictionaryAddressTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.DictionaryAgentCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        FullName = c.String(maxLength: 2000),
                        TaxCode = c.String(maxLength: 2000),
                        OKPOCode = c.String(maxLength: 2000),
                        VATCode = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "DMS.DictionaryAgentPersons",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        FullName = c.String(maxLength: 2000),
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
                .Index(t => t.AgentCompanyId);
            
            CreateTable(
                "DMS.DictionaryAgentContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        ContactTypeId = c.Int(nullable: false),
                        Contact = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryContactTypes", t => t.ContactTypeId)
                .Index(t => t.AgentId)
                .Index(t => t.ContactTypeId);
            
            CreateTable(
                "DMS.DictionaryContactTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        InputMask = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.DictionaryAgentEmployees",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        PersonnelNumber = c.String(maxLength: 2000),
                        AgentPersonId = c.Int(nullable: false),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id);
            
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
                .Index(t => t.Id);
            
            CreateTable(
                "DMS.DictionaryResidentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                .Index(t => t.PositionId)
                .Index(t => t.PositionExecutorTypeId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.DicPositionExecutorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.DictionaryStandartSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        PositionId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => t.PositionId);
            
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
                .Index(t => t.SubordinationTypeId);
            
            CreateTable(
                "DMS.DictionarySubordinationTypes",
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
                "DMS.DictionaryTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        PositionId = c.Int(),
                        Color = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => t.PositionId);
            
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
                .Index(t => t.DocumentId)
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
                        IsRegistered = c.Boolean(nullable: false),
                        RegistrationJournalId = c.Int(),
                        NumerationPrefixFormula = c.String(maxLength: 2000),
                        RegistrationNumber = c.Int(),
                        RegistrationNumberSuffix = c.String(maxLength: 2000),
                        RegistrationNumberPrefix = c.String(maxLength: 2000),
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
                .Index(t => t.TemplateDocumentId)
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
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.DictionaryDocumentSubjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Name = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryDocumentSubjects", t => t.ParentId)
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
                        SourcePositionId = c.Int(),
                        SourcePositionExecutorAgentId = c.Int(),
                        SourceAgentId = c.Int(nullable: false),
                        TargetPositionId = c.Int(),
                        TargetPositionExecutorAgentId = c.Int(),
                        TargetAgentId = c.Int(),
                        IsAvailableWithinTask = c.Boolean(nullable: false),
                        SendDate = c.DateTime(),
                        ReadDate = c.DateTime(),
                        ReadAgentId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DocumentTasks", t => t.TaskId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryEventTypes", t => t.EventTypeId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ReadAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SourceAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.SourcePositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SourcePositionExecutorAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetPositionExecutorAgentId)
                .Index(t => t.DocumentId)
                .Index(t => t.EventTypeId)
                .Index(t => t.TaskId)
                .Index(t => t.SourcePositionId)
                .Index(t => t.SourcePositionExecutorAgentId)
                .Index(t => t.SourceAgentId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetPositionExecutorAgentId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.ReadAgentId);
            
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
                        DueDate = c.DateTime(),
                        DueDay = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        IsInitial = c.Boolean(nullable: false),
                        IsAddControl = c.Boolean(nullable: false),
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
                        Task = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.PositionExecutorAgentId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId)
                .Index(t => t.PositionExecutorAgentId)
                .Index(t => t.AgentId);
            
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
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        IsSuccess = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.DocumentEventReaders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        SendDate = c.DateTime(),
                        ReadDate = c.DateTime(),
                        ReadAgentId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DocumentEvents", t => t.EventId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ReadAgentId)
                .Index(t => t.EventId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.ReadAgentId);
            
            CreateTable(
                "DMS.DictionaryEventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        SourceDescription = c.String(maxLength: 2000),
                        TargetDescription = c.String(maxLength: 2000),
                        ImportanceEventTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryImportanceEventTypes", t => t.ImportanceEventTypeId)
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
                .PrimaryKey(t => t.Id);
            
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
                        TargetAttentionDate = c.DateTime(),
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
                .Index(t => t.ResultTypeId);
            
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
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.DocumentFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 2000),
                        OrderNumber = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        Extension = c.String(maxLength: 2000),
                        FileType = c.String(maxLength: 2000),
                        FileSize = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Content = c.Binary(),
                        IsAdditional = c.Boolean(nullable: false),
                        Hash = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .Index(t => t.DocumentId);
            
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
                .Index(t => t.DocumentId)
                .Index(t => t.ParentDocumentId)
                .Index(t => t.LinkTypeId);
            
            CreateTable(
                "DMS.DictionaryLinkTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        IsImportant = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.DictionaryRegistrationJournals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        DepartmentId = c.Int(nullable: false),
                        Index = c.String(maxLength: 2000),
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
                .Index(t => t.DepartmentId);
            
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
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "DMS.TemplateDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
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
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.TemplateDocumentFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 2000),
                        OrderNumber = c.Int(nullable: false),
                        Extention = c.String(maxLength: 2000),
                        FileType = c.String(maxLength: 2000),
                        FileSize = c.Int(nullable: false),
                        Content = c.Binary(),
                        IsAdditional = c.Boolean(nullable: false),
                        Hash = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "DMS.DictionaryDocumentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                .Index(t => t.DocumentId)
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
                        IsAddControl = c.Boolean(nullable: false),
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
                        Task = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "DMS.AdminRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        PositionId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                .Index(t => t.RoleId)
                .Index(t => t.ActionId);
            
            CreateTable(
                "DMS.SystemActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        API = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsGrantable = c.Boolean(nullable: false),
                        IsGrantableByRecordId = c.Boolean(nullable: false),
                        IsVisible = c.Boolean(nullable: false),
                        GrantId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemActions", t => t.GrantId)
                .ForeignKey("DMS.SystemObjects", t => t.ObjectId)
                .Index(t => t.ObjectId)
                .Index(t => t.GrantId);
            
            CreateTable(
                "DMS.SystemObjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.SystemFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        ValueTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemObjects", t => t.ObjectId)
                .ForeignKey("DMS.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => t.ObjectId)
                .Index(t => t.ValueTypeId);
            
            CreateTable(
                "DMS.SystemValueTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.AdminUserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.UserId)
                .ForeignKey("DMS.AdminRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
                .Index(t => t.SourcePositionId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.SubordinationTypeId);
            
            CreateTable(
                "DMS.CustomDictionaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DictionaryTypeId = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.CustomDictionaryTypes", t => t.DictionaryTypeId)
                .Index(t => t.DictionaryTypeId);
            
            CreateTable(
                "DMS.CustomDictionaryTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.SystemLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogLevel = c.Int(nullable: false),
                        Message = c.String(maxLength: 2000),
                        LogTrace = c.String(maxLength: 2000),
                        LogException = c.String(maxLength: 2000),
                        ExecutorAgentId = c.Int(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorAgentId)
                .Index(t => t.ExecutorAgentId);
            
            CreateTable(
                "DMS.Properties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => t.ValueTypeId);
            
            CreateTable(
                "DMS.PropertyLinks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        ObjectId = c.Int(nullable: false),
                        Filers = c.String(maxLength: 2000),
                        IsMandatory = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemObjects", t => t.ObjectId)
                .ForeignKey("DMS.Properties", t => t.PropertyId)
                .Index(t => t.PropertyId)
                .Index(t => t.ObjectId);
            
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
                .Index(t => t.PropertyLinkId);
            
            CreateTable(
                "DMS.SystemSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(maxLength: 2000),
                        Value = c.String(maxLength: 2000),
                        ExecutorAgentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorAgentId)
                .Index(t => t.ExecutorAgentId);
            
            CreateTable(
                "DMS.SystemUIElements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActionId = c.Int(nullable: false),
                        Code = c.String(maxLength: 2000),
                        TypeCode = c.String(maxLength: 2000),
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
                .Index(t => t.ActionId)
                .Index(t => t.ValueTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.SystemUIElements", "ValueTypeId", "DMS.SystemValueTypes");
            DropForeignKey("DMS.SystemUIElements", "ActionId", "DMS.SystemActions");
            DropForeignKey("DMS.SystemSettings", "ExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.PropertyValues", "PropertyLinkId", "DMS.PropertyLinks");
            DropForeignKey("DMS.PropertyLinks", "PropertyId", "DMS.Properties");
            DropForeignKey("DMS.PropertyLinks", "ObjectId", "DMS.SystemObjects");
            DropForeignKey("DMS.Properties", "ValueTypeId", "DMS.SystemValueTypes");
            DropForeignKey("DMS.SystemLogs", "ExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.CustomDictionaries", "DictionaryTypeId", "DMS.CustomDictionaryTypes");
            DropForeignKey("DMS.AdminSubordinations", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.AdminSubordinations", "SubordinationTypeId", "DMS.DictionarySubordinationTypes");
            DropForeignKey("DMS.AdminSubordinations", "SourcePositionId", "DMS.DictionaryPositions");
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
            DropForeignKey("DMS.DocumentWaits", "ResultTypeId", "DMS.DictionaryResultTypes");
            DropForeignKey("DMS.DocumentWaits", "OnEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentWaits", "OffEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentWaits", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentWaits", "ParentId", "DMS.DocumentWaits");
            DropForeignKey("DMS.DocumentEvents", "EventTypeId", "DMS.DictionaryEventTypes");
            DropForeignKey("DMS.DictionaryEventTypes", "ImportanceEventTypeId", "DMS.DictionaryImportanceEventTypes");
            DropForeignKey("DMS.DocumentEventReaders", "ReadAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEventReaders", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEventReaders", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEventReaders", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSubscriptions", "SubscriptionStateId", "DMS.DictionarySubscriptionStates");
            DropForeignKey("DMS.DocumentSubscriptions", "SendEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentSubscriptions", "DoneEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentSubscriptions", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentEvents", "DocumentId", "DMS.Documents");
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
            DropForeignKey("DMS.DocumentSendLists", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentSendLists", "CloseEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentSendLists", "AccessLevelId", "DMS.AdminAccessLevels");
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
            DropForeignKey("DMS.AdminPositionRoles", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DictionaryPositionExecutors", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DictionaryPositionExecutors", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DictionaryPositionExecutors", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryPositionExecutors", "AccessLevelId", "DMS.AdminAccessLevels");
            DropForeignKey("DMS.DictionaryPositions", "MainExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryPositions", "ExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DictionaryAgents", "ResidentTypeId", "DMS.DictionaryResidentTypes");
            DropForeignKey("DMS.DictionaryAgents", "LanguageId", "DMS.AdminLanguages");
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
            DropIndex("DMS.SystemUIElements", new[] { "ActionId" });
            DropIndex("DMS.SystemSettings", new[] { "ExecutorAgentId" });
            DropIndex("DMS.PropertyValues", new[] { "PropertyLinkId" });
            DropIndex("DMS.PropertyLinks", new[] { "ObjectId" });
            DropIndex("DMS.PropertyLinks", new[] { "PropertyId" });
            DropIndex("DMS.Properties", new[] { "ValueTypeId" });
            DropIndex("DMS.SystemLogs", new[] { "ExecutorAgentId" });
            DropIndex("DMS.CustomDictionaries", new[] { "DictionaryTypeId" });
            DropIndex("DMS.AdminSubordinations", new[] { "SubordinationTypeId" });
            DropIndex("DMS.AdminSubordinations", new[] { "TargetPositionId" });
            DropIndex("DMS.AdminSubordinations", new[] { "SourcePositionId" });
            DropIndex("DMS.AdminUserRoles", new[] { "RoleId" });
            DropIndex("DMS.AdminUserRoles", new[] { "UserId" });
            DropIndex("DMS.SystemFields", new[] { "ValueTypeId" });
            DropIndex("DMS.SystemFields", new[] { "ObjectId" });
            DropIndex("DMS.SystemActions", new[] { "GrantId" });
            DropIndex("DMS.SystemActions", new[] { "ObjectId" });
            DropIndex("DMS.AdminRoleActions", new[] { "ActionId" });
            DropIndex("DMS.AdminRoleActions", new[] { "RoleId" });
            DropIndex("DMS.TemplateDocumentTasks", new[] { "PositionId" });
            DropIndex("DMS.TemplateDocumentTasks", new[] { "DocumentId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "TaskId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "TargetAgentId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "TargetPositionId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "SourcePositionId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "SendTypeId" });
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "DocumentId" });
            DropIndex("DMS.TempDocRestrictedSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.TempDocRestrictedSendLists", new[] { "PositionId" });
            DropIndex("DMS.TempDocRestrictedSendLists", new[] { "DocumentId" });
            DropIndex("DMS.TemplateDocumentFiles", new[] { "DocumentId" });
            DropIndex("DMS.TemplateDocuments", new[] { "SenderAgentPersonId" });
            DropIndex("DMS.TemplateDocuments", new[] { "SenderAgentId" });
            DropIndex("DMS.TemplateDocuments", new[] { "RegistrationJournalId" });
            DropIndex("DMS.TemplateDocuments", new[] { "DocumentSubjectId" });
            DropIndex("DMS.TemplateDocuments", new[] { "DocumentTypeId" });
            DropIndex("DMS.TemplateDocuments", new[] { "DocumentDirectionId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "AccessLevelId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "PositionId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "DocumentId" });
            DropIndex("DMS.DictionaryRegistrationJournals", new[] { "DepartmentId" });
            DropIndex("DMS.DocumentLinks", new[] { "LinkTypeId" });
            DropIndex("DMS.DocumentLinks", new[] { "ParentDocumentId" });
            DropIndex("DMS.DocumentLinks", new[] { "DocumentId" });
            DropIndex("DMS.DocumentFiles", new[] { "DocumentId" });
            DropIndex("DMS.DocumentWaits", new[] { "ResultTypeId" });
            DropIndex("DMS.DocumentWaits", new[] { "OffEventId" });
            DropIndex("DMS.DocumentWaits", new[] { "OnEventId" });
            DropIndex("DMS.DocumentWaits", new[] { "ParentId" });
            DropIndex("DMS.DocumentWaits", new[] { "DocumentId" });
            DropIndex("DMS.DictionaryEventTypes", new[] { "ImportanceEventTypeId" });
            DropIndex("DMS.DocumentEventReaders", new[] { "ReadAgentId" });
            DropIndex("DMS.DocumentEventReaders", new[] { "AgentId" });
            DropIndex("DMS.DocumentEventReaders", new[] { "PositionId" });
            DropIndex("DMS.DocumentEventReaders", new[] { "EventId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "SubscriptionStateId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "DoneEventId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "SendEventId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "DocumentId" });
            DropIndex("DMS.DocumentTasks", new[] { "AgentId" });
            DropIndex("DMS.DocumentTasks", new[] { "PositionExecutorAgentId" });
            DropIndex("DMS.DocumentTasks", new[] { "PositionId" });
            DropIndex("DMS.DocumentTasks", new[] { "DocumentId" });
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
            DropIndex("DMS.DocumentEvents", new[] { "ReadAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourceAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionId" });
            DropIndex("DMS.DocumentEvents", new[] { "TaskId" });
            DropIndex("DMS.DocumentEvents", new[] { "EventTypeId" });
            DropIndex("DMS.DocumentEvents", new[] { "DocumentId" });
            DropIndex("DMS.DictionaryDocumentSubjects", new[] { "ParentId" });
            DropIndex("DMS.DocumentAccesses", new[] { "AccessLevelId" });
            DropIndex("DMS.DocumentAccesses", new[] { "PositionId" });
            DropIndex("DMS.DocumentAccesses", new[] { "DocumentId" });
            DropIndex("DMS.Documents", new[] { "SenderAgentPersonId" });
            DropIndex("DMS.Documents", new[] { "SenderAgentId" });
            DropIndex("DMS.Documents", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.Documents", new[] { "ExecutorPositionId" });
            DropIndex("DMS.Documents", new[] { "RegistrationJournalId" });
            DropIndex("DMS.Documents", new[] { "DocumentSubjectId" });
            DropIndex("DMS.Documents", new[] { "TemplateDocumentId" });
            DropIndex("DMS.DocumentTags", new[] { "TagId" });
            DropIndex("DMS.DocumentTags", new[] { "DocumentId" });
            DropIndex("DMS.DictionaryTags", new[] { "PositionId" });
            DropIndex("DMS.DictionarySendTypes", new[] { "SubordinationTypeId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "AccessLevelId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "TargetAgentId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "TargetPositionId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "SendTypeId" });
            DropIndex("DMS.DicStandartSendListContents", new[] { "StandartSendListId" });
            DropIndex("DMS.DictionaryStandartSendLists", new[] { "PositionId" });
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "AccessLevelId" });
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "PositionExecutorTypeId" });
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "PositionId" });
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "AgentId" });
            DropIndex("DMS.DictionaryAgentUsers", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentContacts", new[] { "ContactTypeId" });
            DropIndex("DMS.DictionaryAgentContacts", new[] { "AgentId" });
            DropIndex("DMS.DictionaryAgentPersons", new[] { "AgentCompanyId" });
            DropIndex("DMS.DictionaryAgentPersons", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentAddresses", new[] { "AdressTypeId" });
            DropIndex("DMS.DictionaryAgentAddresses", new[] { "AgentId" });
            DropIndex("DMS.DictionaryAgentBanks", new[] { "Id" });
            DropIndex("DMS.DictionaryAgentAccounts", new[] { "AgentBankId" });
            DropIndex("DMS.DictionaryAgentAccounts", new[] { "AgentId" });
            DropIndex("DMS.DictionaryAgents", new[] { "LanguageId" });
            DropIndex("DMS.DictionaryAgents", new[] { "ResidentTypeId" });
            DropIndex("DMS.DocumentSavedFilters", new[] { "PositionId" });
            DropIndex("DMS.DictionaryDepartments", new[] { "ChiefPositionId" });
            DropIndex("DMS.DictionaryDepartments", new[] { "CompanyId" });
            DropIndex("DMS.DictionaryDepartments", new[] { "ParentId" });
            DropIndex("DMS.DictionaryPositions", new[] { "MainExecutorAgentId" });
            DropIndex("DMS.DictionaryPositions", new[] { "ExecutorAgentId" });
            DropIndex("DMS.DictionaryPositions", new[] { "DepartmentId" });
            DropIndex("DMS.DictionaryPositions", new[] { "ParentId" });
            DropIndex("DMS.AdminPositionRoles", new[] { "PositionId" });
            DropIndex("DMS.AdminPositionRoles", new[] { "RoleId" });
            DropIndex("DMS.AdminLanguageValues", new[] { "LanguageId" });
            DropTable("DMS.SystemUIElements");
            DropTable("DMS.SystemSettings");
            DropTable("DMS.PropertyValues");
            DropTable("DMS.PropertyLinks");
            DropTable("DMS.Properties");
            DropTable("DMS.SystemLogs");
            DropTable("DMS.CustomDictionaryTypes");
            DropTable("DMS.CustomDictionaries");
            DropTable("DMS.AdminSubordinations");
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
            DropTable("DMS.DictionaryResultTypes");
            DropTable("DMS.DocumentWaits");
            DropTable("DMS.DictionaryImportanceEventTypes");
            DropTable("DMS.DictionaryEventTypes");
            DropTable("DMS.DocumentEventReaders");
            DropTable("DMS.DictionarySubscriptionStates");
            DropTable("DMS.DocumentSubscriptions");
            DropTable("DMS.DocumentTasks");
            DropTable("DMS.DocumentSendLists");
            DropTable("DMS.DocumentEvents");
            DropTable("DMS.DictionaryDocumentSubjects");
            DropTable("DMS.DocumentAccesses");
            DropTable("DMS.Documents");
            DropTable("DMS.DocumentTags");
            DropTable("DMS.DictionaryTags");
            DropTable("DMS.DictionarySubordinationTypes");
            DropTable("DMS.DictionarySendTypes");
            DropTable("DMS.DicStandartSendListContents");
            DropTable("DMS.DictionaryStandartSendLists");
            DropTable("DMS.DicPositionExecutorTypes");
            DropTable("DMS.DictionaryPositionExecutors");
            DropTable("DMS.DictionaryResidentTypes");
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
