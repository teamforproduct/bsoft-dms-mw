namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "BSOFT_DOB_REAL.AdminAccessLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.AdminLanguages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.AdminLanguageValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        Label = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.AdminLanguages", t => t.LanguageId)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "BSOFT_DOB_REAL.AdminPositionRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .ForeignKey("BSOFT_DOB_REAL.AdminRoles", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Name = c.String(),
                        FullName = c.String(),
                        DepartmentId = c.Int(nullable: false),
                        ExecutorAgentId = c.Int(),
                        MainExecutorAgentId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.ParentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryDepartments", t => t.DepartmentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.ExecutorAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.MainExecutorAgentId)
                .Index(t => t.ParentId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ExecutorAgentId)
                .Index(t => t.MainExecutorAgentId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryDepartments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        CompanyId = c.Int(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        FullName = c.String(),
                        ChiefPositionId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.ChiefPositionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryDepartments", t => t.ParentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryCompanies", t => t.CompanyId)
                .Index(t => t.ParentId)
                .Index(t => t.CompanyId)
                .Index(t => t.ChiefPositionId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DocumentSavedFilters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PositionId = c.Int(),
                        Icon = c.String(),
                        Filter = c.String(),
                        IsCommon = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryAgents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ResidentTypeId = c.Int(),
                        IsCompany = c.Boolean(nullable: false),
                        IsIndividual = c.Boolean(nullable: false),
                        IsEmployee = c.Boolean(nullable: false),
                        IsBank = c.Boolean(nullable: false),
                        IsUser = c.Boolean(nullable: false),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LanguageId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.AdminLanguages", t => t.LanguageId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryResidentTypes", t => t.ResidentTypeId)
                .Index(t => t.ResidentTypeId)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryAgentAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        Name = c.String(),
                        AgentBankId = c.Int(nullable: false),
                        AccountNumber = c.String(),
                        IsMain = c.Boolean(nullable: false),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.AgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgentBanks", t => t.AgentBankId)
                .Index(t => t.AgentId)
                .Index(t => t.AgentBankId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryAgentBanks",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        MFOCode = c.String(),
                        Swift = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryAgentAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        AdressTypeId = c.Int(nullable: false),
                        PostCode = c.String(),
                        Address = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAddressTypes", t => t.AdressTypeId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.AgentId)
                .Index(t => t.AgentId)
                .Index(t => t.AdressTypeId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryAddressTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryAgentCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        FullName = c.String(),
                        TaxCode = c.String(),
                        OKPOCode = c.String(),
                        VATCode = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryAgentPersons",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        FullName = c.String(),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        TaxCode = c.String(),
                        IsMale = c.Boolean(nullable: false),
                        PassportSerial = c.String(),
                        PassportNumber = c.Int(),
                        PassportText = c.String(),
                        PassportDate = c.DateTime(),
                        BirthDate = c.DateTime(),
                        AgentCompanyId = c.Int(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgentCompanies", t => t.AgentCompanyId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.AgentCompanyId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryAgentContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        ContactTypeId = c.Int(nullable: false),
                        Contact = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.AgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryContactTypes", t => t.ContactTypeId)
                .Index(t => t.AgentId)
                .Index(t => t.ContactTypeId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryContactTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        InputMask = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryAgentEmployees",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        PersonnelNumber = c.String(),
                        AgentPersonId = c.Int(nullable: false),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryAgentUsers",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryResidentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryPositionExecutors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        PositionExecutorTypeId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.AgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .ForeignKey("BSOFT_DOB_REAL.DicPositionExecutorTypes", t => t.PositionExecutorTypeId)
                .Index(t => t.AgentId)
                .Index(t => t.PositionId)
                .Index(t => t.PositionExecutorTypeId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DicPositionExecutorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryStandartSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PositionId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DicStandartSendListContents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StandartSendListId = c.Int(nullable: false),
                        Stage = c.Int(nullable: false),
                        SendTypeId = c.Int(nullable: false),
                        TargetPositionId = c.Int(),
                        TargetAgentId = c.Int(),
                        Task = c.String(),
                        Description = c.String(),
                        DueDate = c.DateTime(),
                        DueDay = c.Int(),
                        AccessLevelId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("BSOFT_DOB_REAL.DictionarySendTypes", t => t.SendTypeId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryStandartSendLists", t => t.StandartSendListId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.TargetPositionId)
                .Index(t => t.StandartSendListId)
                .Index(t => t.SendTypeId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionarySendTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        IsImportant = c.Boolean(nullable: false),
                        SubordinationTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionarySubordinationTypes", t => t.SubordinationTypeId)
                .Index(t => t.SubordinationTypeId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionarySubordinationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PositionId = c.Int(),
                        Color = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DocumentTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryTags", t => t.TagId)
                .Index(t => t.DocumentId)
                .Index(t => t.TagId);
            
            CreateTable(
                "BSOFT_DOB_REAL.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TemplateDocumentId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        DocumentSubjectId = c.Int(),
                        Description = c.String(),
                        IsRegistered = c.Boolean(nullable: false),
                        RegistrationJournalId = c.Int(),
                        NumerationPrefixFormula = c.String(),
                        RegistrationNumber = c.Int(),
                        RegistrationNumberSuffix = c.String(),
                        RegistrationNumberPrefix = c.String(),
                        RegistrationDate = c.DateTime(),
                        ExecutorPositionId = c.Int(nullable: false),
                        ExecutorPositionExeAgentId = c.Int(nullable: false),
                        SenderAgentId = c.Int(),
                        SenderAgentPersonId = c.Int(),
                        SenderNumber = c.String(),
                        SenderDate = c.DateTime(),
                        Addressee = c.String(),
                        LinkId = c.Int(),
                        IsLaunchPlan = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryDocumentSubjects", t => t.DocumentSubjectId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.ExecutorPositionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.ExecutorPositionExeAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryRegistrationJournals", t => t.RegistrationJournalId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.SenderAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgentPersons", t => t.SenderAgentPersonId)
                .ForeignKey("BSOFT_DOB_REAL.TemplateDocuments", t => t.TemplateDocumentId)
                .Index(t => t.TemplateDocumentId)
                .Index(t => t.DocumentSubjectId)
                .Index(t => t.RegistrationJournalId)
                .Index(t => t.ExecutorPositionId)
                .Index(t => t.ExecutorPositionExeAgentId)
                .Index(t => t.SenderAgentId)
                .Index(t => t.SenderAgentPersonId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DocumentAccesses",
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
                .ForeignKey("BSOFT_DOB_REAL.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryDocumentSubjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryDocumentSubjects", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DocumentEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        EventTypeId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Date = c.DateTime(nullable: false),
                        TaskId = c.Int(),
                        Description = c.String(),
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
                .ForeignKey("BSOFT_DOB_REAL.DocumentTasks", t => t.TaskId)
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryEventTypes", t => t.EventTypeId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.ReadAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.SourceAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.SourcePositionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.SourcePositionExecutorAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.TargetPositionExecutorAgentId)
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
                "BSOFT_DOB_REAL.DocumentSendLists",
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
                        Description = c.String(),
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
                .ForeignKey("BSOFT_DOB_REAL.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("BSOFT_DOB_REAL.DocumentEvents", t => t.CloseEventId)
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionarySendTypes", t => t.SendTypeId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.SourceAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.SourcePositionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.SourcePositionExecutorAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DocumentEvents", t => t.StartEventId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.TargetPositionExecutorAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DocumentTasks", t => t.TaskId)
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
                "BSOFT_DOB_REAL.DocumentTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        PositionExecutorAgentId = c.Int(nullable: false),
                        AgentId = c.Int(nullable: false),
                        Task = c.String(),
                        Description = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.AgentId)
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.PositionExecutorAgentId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId)
                .Index(t => t.PositionExecutorAgentId)
                .Index(t => t.AgentId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DocumentSubscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        SendEventId = c.Int(nullable: false),
                        DoneEventId = c.Int(),
                        Description = c.String(),
                        SubscriptionStateId = c.Int(),
                        Hash = c.String(),
                        ChangedHash = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DocumentEvents", t => t.DoneEventId)
                .ForeignKey("BSOFT_DOB_REAL.DocumentEvents", t => t.SendEventId)
                .ForeignKey("BSOFT_DOB_REAL.DictionarySubscriptionStates", t => t.SubscriptionStateId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendEventId)
                .Index(t => t.DoneEventId)
                .Index(t => t.SubscriptionStateId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionarySubscriptionStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        IsSuccess = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DocumentEventReaders",
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
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.AgentId)
                .ForeignKey("BSOFT_DOB_REAL.DocumentEvents", t => t.EventId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.ReadAgentId)
                .Index(t => t.EventId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.ReadAgentId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryEventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        SourceDescription = c.String(),
                        TargetDescription = c.String(),
                        ImportanceEventTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryImportanceEventTypes", t => t.ImportanceEventTypeId)
                .Index(t => t.ImportanceEventTypeId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryImportanceEventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DocumentWaits",
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
                        TargetDescription = c.String(),
                        TargetAttentionDate = c.DateTime(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DocumentWaits", t => t.ParentId)
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DocumentEvents", t => t.OffEventId)
                .ForeignKey("BSOFT_DOB_REAL.DocumentEvents", t => t.OnEventId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryResultTypes", t => t.ResultTypeId)
                .Index(t => t.DocumentId)
                .Index(t => t.ParentId)
                .Index(t => t.OnEventId)
                .Index(t => t.OffEventId)
                .Index(t => t.ResultTypeId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryResultTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsExecute = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DocumentFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(),
                        OrderNumber = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        Extension = c.String(),
                        FileType = c.String(),
                        FileSize = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Content = c.Binary(),
                        IsAdditional = c.Boolean(nullable: false),
                        Hash = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.DocumentId)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DocumentLinks",
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
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryLinkTypes", t => t.LinkTypeId)
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.ParentDocumentId)
                .Index(t => t.DocumentId)
                .Index(t => t.ParentDocumentId)
                .Index(t => t.LinkTypeId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryLinkTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsImportant = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryRegistrationJournals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DepartmentId = c.Int(nullable: false),
                        Index = c.String(),
                        NumerationPrefixFormula = c.String(),
                        PrefixFormula = c.String(),
                        SuffixFormula = c.String(),
                        DirectionCodes = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryDepartments", t => t.DepartmentId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DocumentRestrictedSendLists",
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
                .ForeignKey("BSOFT_DOB_REAL.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("BSOFT_DOB_REAL.Documents", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "BSOFT_DOB_REAL.TemplateDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsHard = c.Boolean(nullable: false),
                        DocumentDirectionId = c.Int(nullable: false),
                        DocumentTypeId = c.Int(nullable: false),
                        DocumentSubjectId = c.Int(),
                        Description = c.String(),
                        RegistrationJournalId = c.Int(),
                        SenderAgentId = c.Int(),
                        SenderAgentPersonId = c.Int(),
                        Addressee = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryDocumentDirections", t => t.DocumentDirectionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryDocumentSubjects", t => t.DocumentSubjectId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryDocumentTypes", t => t.DocumentTypeId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryRegistrationJournals", t => t.RegistrationJournalId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.SenderAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgentPersons", t => t.SenderAgentPersonId)
                .Index(t => t.DocumentDirectionId)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.DocumentSubjectId)
                .Index(t => t.RegistrationJournalId)
                .Index(t => t.SenderAgentId)
                .Index(t => t.SenderAgentPersonId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryDocumentDirections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.TemplateDocumentFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(),
                        OrderNumber = c.Int(nullable: false),
                        Extention = c.String(),
                        FileType = c.String(),
                        FileSize = c.Int(nullable: false),
                        Content = c.Binary(),
                        IsAdditional = c.Boolean(nullable: false),
                        Hash = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.TemplateDocuments", t => t.DocumentId)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "BSOFT_DOB_REAL.DictionaryDocumentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.TempDocRestrictedSendLists",
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
                .ForeignKey("BSOFT_DOB_REAL.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("BSOFT_DOB_REAL.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "BSOFT_DOB_REAL.TemplateDocumentSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        SendTypeId = c.Int(nullable: false),
                        SourcePositionId = c.Int(),
                        TargetPositionId = c.Int(),
                        TargetAgentId = c.Int(),
                        TaskId = c.Int(),
                        Description = c.String(),
                        Stage = c.Int(nullable: false),
                        DueDay = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        IsAddControl = c.Boolean(nullable: false),
                        IsAvailableWithinTask = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("BSOFT_DOB_REAL.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionarySendTypes", t => t.SendTypeId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.SourcePositionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("BSOFT_DOB_REAL.TemplateDocumentTasks", t => t.TaskId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendTypeId)
                .Index(t => t.SourcePositionId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.TaskId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "BSOFT_DOB_REAL.TemplateDocumentTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        Task = c.String(),
                        Description = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.PositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "BSOFT_DOB_REAL.AdminRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PositionId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.AdminRoleActions",
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
                .ForeignKey("BSOFT_DOB_REAL.SystemActions", t => t.ActionId)
                .ForeignKey("BSOFT_DOB_REAL.AdminRoles", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.ActionId);
            
            CreateTable(
                "BSOFT_DOB_REAL.SystemActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        Code = c.String(),
                        API = c.String(),
                        Description = c.String(),
                        IsGrantable = c.Boolean(nullable: false),
                        IsGrantableByRecordId = c.Boolean(nullable: false),
                        IsVisible = c.Boolean(nullable: false),
                        GrantId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.SystemActions", t => t.GrantId)
                .ForeignKey("BSOFT_DOB_REAL.SystemObjects", t => t.ObjectId)
                .Index(t => t.ObjectId)
                .Index(t => t.GrantId);
            
            CreateTable(
                "BSOFT_DOB_REAL.SystemObjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.SystemFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        ValueTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.SystemObjects", t => t.ObjectId)
                .ForeignKey("BSOFT_DOB_REAL.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => t.ObjectId)
                .Index(t => t.ValueTypeId);
            
            CreateTable(
                "BSOFT_DOB_REAL.SystemValueTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.AdminUserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.UserId)
                .ForeignKey("BSOFT_DOB_REAL.AdminRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "BSOFT_DOB_REAL.AdminSubordinations",
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
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.SourcePositionId)
                .ForeignKey("BSOFT_DOB_REAL.DictionarySubordinationTypes", t => t.SubordinationTypeId)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryPositions", t => t.TargetPositionId)
                .Index(t => t.SourcePositionId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.SubordinationTypeId);
            
            CreateTable(
                "BSOFT_DOB_REAL.CustomDictionaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DictionaryTypeId = c.Int(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.CustomDictionaryTypes", t => t.DictionaryTypeId)
                .Index(t => t.DictionaryTypeId);
            
            CreateTable(
                "BSOFT_DOB_REAL.CustomDictionaryTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BSOFT_DOB_REAL.SystemLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogLevel = c.Int(nullable: false),
                        Message = c.String(),
                        LogTrace = c.String(),
                        LogException = c.String(),
                        ExecutorAgentId = c.Int(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.ExecutorAgentId)
                .Index(t => t.ExecutorAgentId);
            
            CreateTable(
                "BSOFT_DOB_REAL.Properties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        TypeCode = c.String(),
                        Description = c.String(),
                        Label = c.String(),
                        Hint = c.String(),
                        ValueTypeId = c.Int(),
                        OutFormat = c.String(),
                        InputFormat = c.String(),
                        SelectAPI = c.String(),
                        SelectFilter = c.String(),
                        SelectFieldCode = c.String(),
                        SelectDescriptionFieldCode = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => t.ValueTypeId);
            
            CreateTable(
                "BSOFT_DOB_REAL.PropertyLinks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        ObjectId = c.Int(nullable: false),
                        Filers = c.String(),
                        IsMandatory = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.SystemObjects", t => t.ObjectId)
                .ForeignKey("BSOFT_DOB_REAL.Properties", t => t.PropertyId)
                .Index(t => t.PropertyId)
                .Index(t => t.ObjectId);
            
            CreateTable(
                "BSOFT_DOB_REAL.PropertyValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyLinkId = c.Int(nullable: false),
                        RecordId = c.Int(nullable: false),
                        ValueString = c.String(),
                        ValueDate = c.DateTime(),
                        ValueNumeric = c.Double(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.PropertyLinks", t => t.PropertyLinkId)
                .Index(t => t.PropertyLinkId);
            
            CreateTable(
                "BSOFT_DOB_REAL.SystemSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                        ExecutorAgentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.DictionaryAgents", t => t.ExecutorAgentId)
                .Index(t => t.ExecutorAgentId);
            
            CreateTable(
                "BSOFT_DOB_REAL.SystemUIElements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActionId = c.Int(nullable: false),
                        Code = c.String(),
                        TypeCode = c.String(),
                        Description = c.String(),
                        Label = c.String(),
                        Hint = c.String(),
                        ValueTypeId = c.Int(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                        IsReadOnly = c.Boolean(nullable: false),
                        IsVisible = c.Boolean(nullable: false),
                        SelectAPI = c.String(),
                        SelectFilter = c.String(),
                        SelectFieldCode = c.String(),
                        SelectDescriptionFieldCode = c.String(),
                        ValueFieldCode = c.String(),
                        ValueDescriptionFieldCode = c.String(),
                        Format = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BSOFT_DOB_REAL.SystemActions", t => t.ActionId)
                .ForeignKey("BSOFT_DOB_REAL.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => t.ActionId)
                .Index(t => t.ValueTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("BSOFT_DOB_REAL.SystemUIElements", "ValueTypeId", "BSOFT_DOB_REAL.SystemValueTypes");
            DropForeignKey("BSOFT_DOB_REAL.SystemUIElements", "ActionId", "BSOFT_DOB_REAL.SystemActions");
            DropForeignKey("BSOFT_DOB_REAL.SystemSettings", "ExecutorAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.PropertyValues", "PropertyLinkId", "BSOFT_DOB_REAL.PropertyLinks");
            DropForeignKey("BSOFT_DOB_REAL.PropertyLinks", "PropertyId", "BSOFT_DOB_REAL.Properties");
            DropForeignKey("BSOFT_DOB_REAL.PropertyLinks", "ObjectId", "BSOFT_DOB_REAL.SystemObjects");
            DropForeignKey("BSOFT_DOB_REAL.Properties", "ValueTypeId", "BSOFT_DOB_REAL.SystemValueTypes");
            DropForeignKey("BSOFT_DOB_REAL.SystemLogs", "ExecutorAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.CustomDictionaries", "DictionaryTypeId", "BSOFT_DOB_REAL.CustomDictionaryTypes");
            DropForeignKey("BSOFT_DOB_REAL.AdminSubordinations", "TargetPositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.AdminSubordinations", "SubordinationTypeId", "BSOFT_DOB_REAL.DictionarySubordinationTypes");
            DropForeignKey("BSOFT_DOB_REAL.AdminSubordinations", "SourcePositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.AdminUserRoles", "RoleId", "BSOFT_DOB_REAL.AdminRoles");
            DropForeignKey("BSOFT_DOB_REAL.AdminUserRoles", "UserId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.AdminRoleActions", "RoleId", "BSOFT_DOB_REAL.AdminRoles");
            DropForeignKey("BSOFT_DOB_REAL.AdminRoleActions", "ActionId", "BSOFT_DOB_REAL.SystemActions");
            DropForeignKey("BSOFT_DOB_REAL.SystemFields", "ValueTypeId", "BSOFT_DOB_REAL.SystemValueTypes");
            DropForeignKey("BSOFT_DOB_REAL.SystemFields", "ObjectId", "BSOFT_DOB_REAL.SystemObjects");
            DropForeignKey("BSOFT_DOB_REAL.SystemActions", "ObjectId", "BSOFT_DOB_REAL.SystemObjects");
            DropForeignKey("BSOFT_DOB_REAL.SystemActions", "GrantId", "BSOFT_DOB_REAL.SystemActions");
            DropForeignKey("BSOFT_DOB_REAL.AdminPositionRoles", "RoleId", "BSOFT_DOB_REAL.AdminRoles");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryTags", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DocumentTags", "TagId", "BSOFT_DOB_REAL.DictionaryTags");
            DropForeignKey("BSOFT_DOB_REAL.Documents", "TemplateDocumentId", "BSOFT_DOB_REAL.TemplateDocuments");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocumentSendLists", "TaskId", "BSOFT_DOB_REAL.TemplateDocumentTasks");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocumentTasks", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocumentTasks", "DocumentId", "BSOFT_DOB_REAL.TemplateDocuments");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocumentSendLists", "TargetPositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocumentSendLists", "TargetAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocumentSendLists", "SourcePositionId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocumentSendLists", "SendTypeId", "BSOFT_DOB_REAL.DictionarySendTypes");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocumentSendLists", "DocumentId", "BSOFT_DOB_REAL.TemplateDocuments");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocumentSendLists", "AccessLevelId", "BSOFT_DOB_REAL.AdminAccessLevels");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocuments", "SenderAgentPersonId", "BSOFT_DOB_REAL.DictionaryAgentPersons");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocuments", "SenderAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.TempDocRestrictedSendLists", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.TempDocRestrictedSendLists", "DocumentId", "BSOFT_DOB_REAL.TemplateDocuments");
            DropForeignKey("BSOFT_DOB_REAL.TempDocRestrictedSendLists", "AccessLevelId", "BSOFT_DOB_REAL.AdminAccessLevels");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocuments", "RegistrationJournalId", "BSOFT_DOB_REAL.DictionaryRegistrationJournals");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocuments", "DocumentTypeId", "BSOFT_DOB_REAL.DictionaryDocumentTypes");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocuments", "DocumentSubjectId", "BSOFT_DOB_REAL.DictionaryDocumentSubjects");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocumentFiles", "DocumentId", "BSOFT_DOB_REAL.TemplateDocuments");
            DropForeignKey("BSOFT_DOB_REAL.TemplateDocuments", "DocumentDirectionId", "BSOFT_DOB_REAL.DictionaryDocumentDirections");
            DropForeignKey("BSOFT_DOB_REAL.DocumentTags", "DocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.Documents", "SenderAgentPersonId", "BSOFT_DOB_REAL.DictionaryAgentPersons");
            DropForeignKey("BSOFT_DOB_REAL.Documents", "SenderAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentRestrictedSendLists", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DocumentRestrictedSendLists", "DocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentRestrictedSendLists", "AccessLevelId", "BSOFT_DOB_REAL.AdminAccessLevels");
            DropForeignKey("BSOFT_DOB_REAL.Documents", "RegistrationJournalId", "BSOFT_DOB_REAL.DictionaryRegistrationJournals");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryRegistrationJournals", "DepartmentId", "BSOFT_DOB_REAL.DictionaryDepartments");
            DropForeignKey("BSOFT_DOB_REAL.DocumentLinks", "ParentDocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentLinks", "LinkTypeId", "BSOFT_DOB_REAL.DictionaryLinkTypes");
            DropForeignKey("BSOFT_DOB_REAL.DocumentLinks", "DocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentFiles", "DocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.Documents", "ExecutorPositionExeAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.Documents", "ExecutorPositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEvents", "TargetPositionExecutorAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEvents", "TargetPositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEvents", "TargetAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEvents", "SourcePositionExecutorAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEvents", "SourcePositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEvents", "SourceAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEvents", "ReadAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentWaits", "ResultTypeId", "BSOFT_DOB_REAL.DictionaryResultTypes");
            DropForeignKey("BSOFT_DOB_REAL.DocumentWaits", "OnEventId", "BSOFT_DOB_REAL.DocumentEvents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentWaits", "OffEventId", "BSOFT_DOB_REAL.DocumentEvents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentWaits", "DocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentWaits", "ParentId", "BSOFT_DOB_REAL.DocumentWaits");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEvents", "EventTypeId", "BSOFT_DOB_REAL.DictionaryEventTypes");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryEventTypes", "ImportanceEventTypeId", "BSOFT_DOB_REAL.DictionaryImportanceEventTypes");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEventReaders", "ReadAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEventReaders", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEventReaders", "EventId", "BSOFT_DOB_REAL.DocumentEvents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEventReaders", "AgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSubscriptions", "SubscriptionStateId", "BSOFT_DOB_REAL.DictionarySubscriptionStates");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSubscriptions", "SendEventId", "BSOFT_DOB_REAL.DocumentEvents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSubscriptions", "DoneEventId", "BSOFT_DOB_REAL.DocumentEvents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSubscriptions", "DocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEvents", "DocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "TaskId", "BSOFT_DOB_REAL.DocumentTasks");
            DropForeignKey("BSOFT_DOB_REAL.DocumentTasks", "PositionExecutorAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentTasks", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DocumentEvents", "TaskId", "BSOFT_DOB_REAL.DocumentTasks");
            DropForeignKey("BSOFT_DOB_REAL.DocumentTasks", "DocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentTasks", "AgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "TargetPositionExecutorAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "TargetPositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "TargetAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "StartEventId", "BSOFT_DOB_REAL.DocumentEvents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "SourcePositionExecutorAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "SourcePositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "SourceAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "SendTypeId", "BSOFT_DOB_REAL.DictionarySendTypes");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "DocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "CloseEventId", "BSOFT_DOB_REAL.DocumentEvents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSendLists", "AccessLevelId", "BSOFT_DOB_REAL.AdminAccessLevels");
            DropForeignKey("BSOFT_DOB_REAL.Documents", "DocumentSubjectId", "BSOFT_DOB_REAL.DictionaryDocumentSubjects");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryDocumentSubjects", "ParentId", "BSOFT_DOB_REAL.DictionaryDocumentSubjects");
            DropForeignKey("BSOFT_DOB_REAL.DocumentAccesses", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DocumentAccesses", "DocumentId", "BSOFT_DOB_REAL.Documents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentAccesses", "AccessLevelId", "BSOFT_DOB_REAL.AdminAccessLevels");
            DropForeignKey("BSOFT_DOB_REAL.DicStandartSendListContents", "TargetPositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DicStandartSendListContents", "TargetAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DicStandartSendListContents", "StandartSendListId", "BSOFT_DOB_REAL.DictionaryStandartSendLists");
            DropForeignKey("BSOFT_DOB_REAL.DicStandartSendListContents", "SendTypeId", "BSOFT_DOB_REAL.DictionarySendTypes");
            DropForeignKey("BSOFT_DOB_REAL.DictionarySendTypes", "SubordinationTypeId", "BSOFT_DOB_REAL.DictionarySubordinationTypes");
            DropForeignKey("BSOFT_DOB_REAL.DicStandartSendListContents", "AccessLevelId", "BSOFT_DOB_REAL.AdminAccessLevels");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryStandartSendLists", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.AdminPositionRoles", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryPositionExecutors", "PositionExecutorTypeId", "BSOFT_DOB_REAL.DicPositionExecutorTypes");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryPositionExecutors", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryPositionExecutors", "AgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryPositionExecutors", "AccessLevelId", "BSOFT_DOB_REAL.AdminAccessLevels");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryPositions", "MainExecutorAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryPositions", "ExecutorAgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgents", "ResidentTypeId", "BSOFT_DOB_REAL.DictionaryResidentTypes");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgents", "LanguageId", "BSOFT_DOB_REAL.AdminLanguages");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentUsers", "Id", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentPersons", "Id", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentEmployees", "Id", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentContacts", "ContactTypeId", "BSOFT_DOB_REAL.DictionaryContactTypes");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentContacts", "AgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentCompanies", "Id", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentPersons", "AgentCompanyId", "BSOFT_DOB_REAL.DictionaryAgentCompanies");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentBanks", "Id", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentAddresses", "AgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentAddresses", "AdressTypeId", "BSOFT_DOB_REAL.DictionaryAddressTypes");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentAccounts", "AgentBankId", "BSOFT_DOB_REAL.DictionaryAgentBanks");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryAgentAccounts", "AgentId", "BSOFT_DOB_REAL.DictionaryAgents");
            DropForeignKey("BSOFT_DOB_REAL.DocumentSavedFilters", "PositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryPositions", "DepartmentId", "BSOFT_DOB_REAL.DictionaryDepartments");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryPositions", "ParentId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryDepartments", "CompanyId", "BSOFT_DOB_REAL.DictionaryCompanies");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryDepartments", "ParentId", "BSOFT_DOB_REAL.DictionaryDepartments");
            DropForeignKey("BSOFT_DOB_REAL.DictionaryDepartments", "ChiefPositionId", "BSOFT_DOB_REAL.DictionaryPositions");
            DropForeignKey("BSOFT_DOB_REAL.AdminLanguageValues", "LanguageId", "BSOFT_DOB_REAL.AdminLanguages");
            DropIndex("BSOFT_DOB_REAL.SystemUIElements", new[] { "ValueTypeId" });
            DropIndex("BSOFT_DOB_REAL.SystemUIElements", new[] { "ActionId" });
            DropIndex("BSOFT_DOB_REAL.SystemSettings", new[] { "ExecutorAgentId" });
            DropIndex("BSOFT_DOB_REAL.PropertyValues", new[] { "PropertyLinkId" });
            DropIndex("BSOFT_DOB_REAL.PropertyLinks", new[] { "ObjectId" });
            DropIndex("BSOFT_DOB_REAL.PropertyLinks", new[] { "PropertyId" });
            DropIndex("BSOFT_DOB_REAL.Properties", new[] { "ValueTypeId" });
            DropIndex("BSOFT_DOB_REAL.SystemLogs", new[] { "ExecutorAgentId" });
            DropIndex("BSOFT_DOB_REAL.CustomDictionaries", new[] { "DictionaryTypeId" });
            DropIndex("BSOFT_DOB_REAL.AdminSubordinations", new[] { "SubordinationTypeId" });
            DropIndex("BSOFT_DOB_REAL.AdminSubordinations", new[] { "TargetPositionId" });
            DropIndex("BSOFT_DOB_REAL.AdminSubordinations", new[] { "SourcePositionId" });
            DropIndex("BSOFT_DOB_REAL.AdminUserRoles", new[] { "RoleId" });
            DropIndex("BSOFT_DOB_REAL.AdminUserRoles", new[] { "UserId" });
            DropIndex("BSOFT_DOB_REAL.SystemFields", new[] { "ValueTypeId" });
            DropIndex("BSOFT_DOB_REAL.SystemFields", new[] { "ObjectId" });
            DropIndex("BSOFT_DOB_REAL.SystemActions", new[] { "GrantId" });
            DropIndex("BSOFT_DOB_REAL.SystemActions", new[] { "ObjectId" });
            DropIndex("BSOFT_DOB_REAL.AdminRoleActions", new[] { "ActionId" });
            DropIndex("BSOFT_DOB_REAL.AdminRoleActions", new[] { "RoleId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocumentTasks", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocumentTasks", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocumentSendLists", new[] { "AccessLevelId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocumentSendLists", new[] { "TaskId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocumentSendLists", new[] { "TargetAgentId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocumentSendLists", new[] { "TargetPositionId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocumentSendLists", new[] { "SourcePositionId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocumentSendLists", new[] { "SendTypeId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocumentSendLists", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.TempDocRestrictedSendLists", new[] { "AccessLevelId" });
            DropIndex("BSOFT_DOB_REAL.TempDocRestrictedSendLists", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.TempDocRestrictedSendLists", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocumentFiles", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocuments", new[] { "SenderAgentPersonId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocuments", new[] { "SenderAgentId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocuments", new[] { "RegistrationJournalId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocuments", new[] { "DocumentSubjectId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocuments", new[] { "DocumentTypeId" });
            DropIndex("BSOFT_DOB_REAL.TemplateDocuments", new[] { "DocumentDirectionId" });
            DropIndex("BSOFT_DOB_REAL.DocumentRestrictedSendLists", new[] { "AccessLevelId" });
            DropIndex("BSOFT_DOB_REAL.DocumentRestrictedSendLists", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.DocumentRestrictedSendLists", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryRegistrationJournals", new[] { "DepartmentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentLinks", new[] { "LinkTypeId" });
            DropIndex("BSOFT_DOB_REAL.DocumentLinks", new[] { "ParentDocumentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentLinks", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentFiles", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentWaits", new[] { "ResultTypeId" });
            DropIndex("BSOFT_DOB_REAL.DocumentWaits", new[] { "OffEventId" });
            DropIndex("BSOFT_DOB_REAL.DocumentWaits", new[] { "OnEventId" });
            DropIndex("BSOFT_DOB_REAL.DocumentWaits", new[] { "ParentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentWaits", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryEventTypes", new[] { "ImportanceEventTypeId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEventReaders", new[] { "ReadAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEventReaders", new[] { "AgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEventReaders", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEventReaders", new[] { "EventId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSubscriptions", new[] { "SubscriptionStateId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSubscriptions", new[] { "DoneEventId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSubscriptions", new[] { "SendEventId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSubscriptions", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentTasks", new[] { "AgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentTasks", new[] { "PositionExecutorAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentTasks", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.DocumentTasks", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "CloseEventId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "StartEventId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "AccessLevelId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "TaskId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "TargetAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "TargetPositionId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "SourceAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "SourcePositionId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "SendTypeId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSendLists", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEvents", new[] { "ReadAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEvents", new[] { "TargetAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEvents", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEvents", new[] { "TargetPositionId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEvents", new[] { "SourceAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEvents", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEvents", new[] { "SourcePositionId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEvents", new[] { "TaskId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEvents", new[] { "EventTypeId" });
            DropIndex("BSOFT_DOB_REAL.DocumentEvents", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryDocumentSubjects", new[] { "ParentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentAccesses", new[] { "AccessLevelId" });
            DropIndex("BSOFT_DOB_REAL.DocumentAccesses", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.DocumentAccesses", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.Documents", new[] { "SenderAgentPersonId" });
            DropIndex("BSOFT_DOB_REAL.Documents", new[] { "SenderAgentId" });
            DropIndex("BSOFT_DOB_REAL.Documents", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("BSOFT_DOB_REAL.Documents", new[] { "ExecutorPositionId" });
            DropIndex("BSOFT_DOB_REAL.Documents", new[] { "RegistrationJournalId" });
            DropIndex("BSOFT_DOB_REAL.Documents", new[] { "DocumentSubjectId" });
            DropIndex("BSOFT_DOB_REAL.Documents", new[] { "TemplateDocumentId" });
            DropIndex("BSOFT_DOB_REAL.DocumentTags", new[] { "TagId" });
            DropIndex("BSOFT_DOB_REAL.DocumentTags", new[] { "DocumentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryTags", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.DictionarySendTypes", new[] { "SubordinationTypeId" });
            DropIndex("BSOFT_DOB_REAL.DicStandartSendListContents", new[] { "AccessLevelId" });
            DropIndex("BSOFT_DOB_REAL.DicStandartSendListContents", new[] { "TargetAgentId" });
            DropIndex("BSOFT_DOB_REAL.DicStandartSendListContents", new[] { "TargetPositionId" });
            DropIndex("BSOFT_DOB_REAL.DicStandartSendListContents", new[] { "SendTypeId" });
            DropIndex("BSOFT_DOB_REAL.DicStandartSendListContents", new[] { "StandartSendListId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryStandartSendLists", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryPositionExecutors", new[] { "AccessLevelId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryPositionExecutors", new[] { "PositionExecutorTypeId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryPositionExecutors", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryPositionExecutors", new[] { "AgentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentUsers", new[] { "Id" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentEmployees", new[] { "Id" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentContacts", new[] { "ContactTypeId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentContacts", new[] { "AgentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentPersons", new[] { "AgentCompanyId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentPersons", new[] { "Id" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentCompanies", new[] { "Id" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentAddresses", new[] { "AdressTypeId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentAddresses", new[] { "AgentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentBanks", new[] { "Id" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentAccounts", new[] { "AgentBankId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgentAccounts", new[] { "AgentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgents", new[] { "LanguageId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryAgents", new[] { "ResidentTypeId" });
            DropIndex("BSOFT_DOB_REAL.DocumentSavedFilters", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryDepartments", new[] { "ChiefPositionId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryDepartments", new[] { "CompanyId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryDepartments", new[] { "ParentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryPositions", new[] { "MainExecutorAgentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryPositions", new[] { "ExecutorAgentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryPositions", new[] { "DepartmentId" });
            DropIndex("BSOFT_DOB_REAL.DictionaryPositions", new[] { "ParentId" });
            DropIndex("BSOFT_DOB_REAL.AdminPositionRoles", new[] { "PositionId" });
            DropIndex("BSOFT_DOB_REAL.AdminPositionRoles", new[] { "RoleId" });
            DropIndex("BSOFT_DOB_REAL.AdminLanguageValues", new[] { "LanguageId" });
            DropTable("BSOFT_DOB_REAL.SystemUIElements");
            DropTable("BSOFT_DOB_REAL.SystemSettings");
            DropTable("BSOFT_DOB_REAL.PropertyValues");
            DropTable("BSOFT_DOB_REAL.PropertyLinks");
            DropTable("BSOFT_DOB_REAL.Properties");
            DropTable("BSOFT_DOB_REAL.SystemLogs");
            DropTable("BSOFT_DOB_REAL.CustomDictionaryTypes");
            DropTable("BSOFT_DOB_REAL.CustomDictionaries");
            DropTable("BSOFT_DOB_REAL.AdminSubordinations");
            DropTable("BSOFT_DOB_REAL.AdminUserRoles");
            DropTable("BSOFT_DOB_REAL.SystemValueTypes");
            DropTable("BSOFT_DOB_REAL.SystemFields");
            DropTable("BSOFT_DOB_REAL.SystemObjects");
            DropTable("BSOFT_DOB_REAL.SystemActions");
            DropTable("BSOFT_DOB_REAL.AdminRoleActions");
            DropTable("BSOFT_DOB_REAL.AdminRoles");
            DropTable("BSOFT_DOB_REAL.TemplateDocumentTasks");
            DropTable("BSOFT_DOB_REAL.TemplateDocumentSendLists");
            DropTable("BSOFT_DOB_REAL.TempDocRestrictedSendLists");
            DropTable("BSOFT_DOB_REAL.DictionaryDocumentTypes");
            DropTable("BSOFT_DOB_REAL.TemplateDocumentFiles");
            DropTable("BSOFT_DOB_REAL.DictionaryDocumentDirections");
            DropTable("BSOFT_DOB_REAL.TemplateDocuments");
            DropTable("BSOFT_DOB_REAL.DocumentRestrictedSendLists");
            DropTable("BSOFT_DOB_REAL.DictionaryRegistrationJournals");
            DropTable("BSOFT_DOB_REAL.DictionaryLinkTypes");
            DropTable("BSOFT_DOB_REAL.DocumentLinks");
            DropTable("BSOFT_DOB_REAL.DocumentFiles");
            DropTable("BSOFT_DOB_REAL.DictionaryResultTypes");
            DropTable("BSOFT_DOB_REAL.DocumentWaits");
            DropTable("BSOFT_DOB_REAL.DictionaryImportanceEventTypes");
            DropTable("BSOFT_DOB_REAL.DictionaryEventTypes");
            DropTable("BSOFT_DOB_REAL.DocumentEventReaders");
            DropTable("BSOFT_DOB_REAL.DictionarySubscriptionStates");
            DropTable("BSOFT_DOB_REAL.DocumentSubscriptions");
            DropTable("BSOFT_DOB_REAL.DocumentTasks");
            DropTable("BSOFT_DOB_REAL.DocumentSendLists");
            DropTable("BSOFT_DOB_REAL.DocumentEvents");
            DropTable("BSOFT_DOB_REAL.DictionaryDocumentSubjects");
            DropTable("BSOFT_DOB_REAL.DocumentAccesses");
            DropTable("BSOFT_DOB_REAL.Documents");
            DropTable("BSOFT_DOB_REAL.DocumentTags");
            DropTable("BSOFT_DOB_REAL.DictionaryTags");
            DropTable("BSOFT_DOB_REAL.DictionarySubordinationTypes");
            DropTable("BSOFT_DOB_REAL.DictionarySendTypes");
            DropTable("BSOFT_DOB_REAL.DicStandartSendListContents");
            DropTable("BSOFT_DOB_REAL.DictionaryStandartSendLists");
            DropTable("BSOFT_DOB_REAL.DicPositionExecutorTypes");
            DropTable("BSOFT_DOB_REAL.DictionaryPositionExecutors");
            DropTable("BSOFT_DOB_REAL.DictionaryResidentTypes");
            DropTable("BSOFT_DOB_REAL.DictionaryAgentUsers");
            DropTable("BSOFT_DOB_REAL.DictionaryAgentEmployees");
            DropTable("BSOFT_DOB_REAL.DictionaryContactTypes");
            DropTable("BSOFT_DOB_REAL.DictionaryAgentContacts");
            DropTable("BSOFT_DOB_REAL.DictionaryAgentPersons");
            DropTable("BSOFT_DOB_REAL.DictionaryAgentCompanies");
            DropTable("BSOFT_DOB_REAL.DictionaryAddressTypes");
            DropTable("BSOFT_DOB_REAL.DictionaryAgentAddresses");
            DropTable("BSOFT_DOB_REAL.DictionaryAgentBanks");
            DropTable("BSOFT_DOB_REAL.DictionaryAgentAccounts");
            DropTable("BSOFT_DOB_REAL.DictionaryAgents");
            DropTable("BSOFT_DOB_REAL.DocumentSavedFilters");
            DropTable("BSOFT_DOB_REAL.DictionaryCompanies");
            DropTable("BSOFT_DOB_REAL.DictionaryDepartments");
            DropTable("BSOFT_DOB_REAL.DictionaryPositions");
            DropTable("BSOFT_DOB_REAL.AdminPositionRoles");
            DropTable("BSOFT_DOB_REAL.AdminLanguageValues");
            DropTable("BSOFT_DOB_REAL.AdminLanguages");
            DropTable("BSOFT_DOB_REAL.AdminAccessLevels");
        }
    }
}
