USE [belletrix];

IF (OBJECT_ID('dbo.DeleteStudent', 'P') IS NOT NULL) DROP PROCEDURE [dbo].[DeleteStudent];

IF OBJECT_ID('dbo.StudentDesiredLanguages', 'U') IS NOT NULL DROP TABLE [dbo].[StudentDesiredLanguages];
IF OBJECT_ID('dbo.StudentFluentLanguages', 'U') IS NOT NULL DROP TABLE [dbo].[StudentFluentLanguages];
IF OBJECT_ID('dbo.StudentStudiedLanguages', 'U') IS NOT NULL DROP TABLE [dbo].[StudentStudiedLanguages];
IF OBJECT_ID('dbo.Matriculation', 'U') IS NOT NULL DROP TABLE [dbo].[Matriculation];
IF OBJECT_ID('dbo.Languages', 'U') IS NOT NULL DROP TABLE [dbo].[Languages];
IF OBJECT_ID('dbo.Minors', 'U') IS NOT NULL DROP TABLE [dbo].[Minors];
IF OBJECT_ID('dbo.Majors', 'U') IS NOT NULL DROP TABLE [dbo].[Majors];
IF OBJECT_ID('dbo.StudentStudyAbroadWishlist', 'U') IS NOT NULL DROP TABLE [dbo].[StudentStudyAbroadWishlist];
IF OBJECT_ID('dbo.StudentNotes', 'U') IS NOT NULL DROP TABLE [dbo].[StudentNotes];
IF OBJECT_ID('dbo.EventLog', 'U') IS NOT NULL DROP TABLE [dbo].[EventLog];
IF OBJECT_ID('dbo.StudentPromoLog', 'U') IS NOT NULL DROP TABLE [dbo].[StudentPromoLog];
IF OBJECT_ID('dbo.StudyAbroad', 'U') IS NOT NULL DROP TABLE [dbo].[StudyAbroad];
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL DROP TABLE [dbo].[Students];
IF OBJECT_ID('dbo.Countries', 'U') IS NOT NULL DROP TABLE [dbo].[Countries];
IF OBJECT_ID('dbo.UserPromo', 'U') IS NOT NULL DROP TABLE [dbo].[UserPromo];
IF OBJECT_ID('dbo.ActivityLogParticipant', 'U') IS NOT NULL DROP TABLE [dbo].[ActivityLogParticipant];
IF OBJECT_ID('dbo.ActivityLog', 'U') IS NOT NULL DROP TABLE [dbo].[ActivityLog];
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE [dbo].[Users];
IF OBJECT_ID('dbo.Programs', 'U') IS NOT NULL DROP TABLE [dbo].[Programs];
IF OBJECT_ID('dbo.StudyAbroadProgramTypes', 'U') IS NOT NULL DROP TABLE [dbo].[StudyAbroadProgramTypes];
IF OBJECT_ID('dbo.ProgramTypes', 'U') IS NOT NULL DROP TABLE [dbo].[ProgramTypes];
IF OBJECT_ID('dbo.ActivityLogPerson', 'U') IS NOT NULL DROP TABLE [dbo].[ActivityLogPerson];
IF OBJECT_ID('dbo.Exceptions', 'U') IS NOT NULL DROP TABLE [dbo].[Exceptions];


CREATE TABLE [dbo].[Countries] (
    [Id]            [int] NOT NULL IDENTITY,
    [Name]          [nvarchar](64) NOT NULL,
    [Abbreviation]  [varchar](3) NOT NULL,
    [IsRegion]      [bit] NOT NULL,

    CONSTRAINT [PK_Countries] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_Countries_Name] UNIQUE ([Name], [Abbreviation], [IsRegion])
);

INSERT INTO [dbo].[Countries]
([Abbreviation], [IsRegion], [Name])
VALUES
('US', 0, N'United States'),
('CA', 0, N'Canada'),
('AF', 0, N'Afghanistan'),
('AL', 0, N'Albania'),
('DZ', 0, N'Algeria'),
('DS', 0, N'American Samoa'),
('AD', 0, N'Andorra'),
('AO', 0, N'Angola'),
('AI', 0, N'Anguilla'),
('AQ', 0, N'Antarctica'),
('AG', 0, N'Antigua and/or Barbuda'),
('AR', 0, N'Argentina'),
('AM', 0, N'Armenia'),
('AW', 0, N'Aruba'),
('AU', 0, N'Australia'),
('AT', 0, N'Austria'),
('AZ', 0, N'Azerbaijan'),
('BS', 0, N'Bahamas'),
('BH', 0, N'Bahrain'),
('BD', 0, N'Bangladesh'),
('BB', 0, N'Barbados'),
('BY', 0, N'Belarus'),
('BE', 0, N'Belgium'),
('BZ', 0, N'Belize'),
('BJ', 0, N'Benin'),
('BM', 0, N'Bermuda'),
('BT', 0, N'Bhutan'),
('BO', 0, N'Bolivia'),
('BA', 0, N'Bosnia and Herzegovina'),
('BW', 0, N'Botswana'),
('BV', 0, N'Bouvet Island'),
('BR', 0, N'Brazil'),
('IO', 0, N'British lndian Ocean Territory'),
('BN', 0, N'Brunei Darussalam'),
('BG', 0, N'Bulgaria'),
('BF', 0, N'Burkina Faso'),
('BI', 0, N'Burundi'),
('KH', 0, N'Cambodia'),
('CM', 0, N'Cameroon'),
('CV', 0, N'Cape Verde'),
('KY', 0, N'Cayman Islands'),
('CF', 0, N'Central African Republic'),
('TD', 0, N'Chad'),
('CL', 0, N'Chile'),
('CN', 0, N'China'),
('CX', 0, N'Christmas Island'),
('CC', 0, N'Cocos (Keeling) Islands'),
('CO', 0, N'Colombia'),
('KM', 0, 'Comoros'),
('CG', 0, N'Congo'),
('CK', 0, N'Cook Islands'),
('CR', 0, N'Costa Rica'),
('HR', 0, N'Croatia (Hrvatska)'),
('CU', 0, N'Cuba'),
('CY', 0, N'Cyprus'),
('CZ', 0, N'Czech Republic'),
('DK', 0, N'Denmark'),
('DJ', 0, N'Djibouti'),
('DM', 0, N'Dominica'),
('DO', 0, N'Dominican Republic'),
('TP', 0, N'East Timor'),
('EC', 0, N'Ecuador'),
('EG', 0, N'Egypt'),
('SV', 0, N'El Salvador'),
('GQ', 0, N'Equatorial Guinea'),
('ER', 0, N'Eritrea'),
('EE', 0, N'Estonia'),
('ET', 0, N'Ethiopia'),
('FK', 0, N'Falkland Islands (Malvinas)'),
('FO', 0, N'Faroe Islands'),
('FJ', 0, N'Fiji'),
('FI', 0, N'Finland'),
('FR', 0, N'France'),
('FX', 0, N'France, Metropolitan'),
('GF', 0, N'French Guiana'),
('PF', 0, N'French Polynesia'),
('TF', 0, N'French Southern Territories'),
('GA', 0, N'Gabon'),
('GM', 0, N'Gambia'),
('GE', 0, N'Georgia'),
('DE', 0, N'Germany'),
('GH', 0, N'Ghana'),
('GI', 0, N'Gibraltar'),
('GR', 0, N'Greece'),
('GL', 0, N'Greenland'),
('GD', 0, N'Grenada'),
('GP', 0, N'Guadeloupe'),
('GU', 0, N'Guam'),
('GT', 0, N'Guatemala'),
('GN', 0, N'Guinea'),
('GW', 0, N'Guinea-Bissau'),
('GY', 0, N'Guyana'),
('HT', 0, N'Haiti'),
('HM', 0, N'Heard and Mc Donald Islands'),
('HN', 0, N'Honduras'),
('HK', 0, N'Hong Kong'),
('HU', 0, N'Hungary'),
('IS', 0, N'Iceland'),
('IN', 0, N'India'),
('ID', 0, N'Indonesia'),
('IR', 0, N'Iran'),
('IQ', 0, N'Iraq'),
('IE', 0, N'Ireland'),
('IL', 0, N'Israel'),
('IT', 0, N'Italy'),
('CI', 0, N'Ivory Coast'),
('JM', 0, N'Jamaica'),
('JP', 0, N'Japan'),
('JO', 0, N'Jordan'),
('KZ', 0, N'Kazakhstan'),
('KE', 0, N'Kenya'),
('KI', 0, N'Kiribati'),
('KP', 0, N'North Korea'),
('KR', 0, N'South Korea'),
('KW', 0, N'Kuwait'),
('KG', 0, N'Kyrgyzstan'),
('LA', 0, N'Laos'),
('LV', 0, N'Latvia'),
('LB', 0, N'Lebanon'),
('LS', 0, N'Lesotho'),
('LR', 0, N'Liberia'),
('LY', 0, N'Libya'),
('LI', 0, N'Liechtenstein'),
('LT', 0, N'Lithuania'),
('LU', 0, N'Luxembourg'),
('MO', 0, N'Macau'),
('MK', 0, N'Macedonia'),
('MG', 0, N'Madagascar'),
('MW', 0, N'Malawi'),
('MY', 0, N'Malaysia'),
('MV', 0, N'Maldives'),
('ML', 0, N'Mali'),
('MT', 0, N'Malta'),
('MH', 0, N'Marshall Islands'),
('MQ', 0, N'Martinique'),
('MR', 0, N'Mauritania'),
('MU', 0, N'Mauritius'),
('TY', 0, N'Mayotte'),
('MX', 0, N'Mexico'),
('FM', 0, N'Micronesia'),
('MD', 0, N'Moldova'),
('MC', 0, N'Monaco'),
('MN', 0, N'Mongolia'),
('MS', 0, N'Montserrat'),
('MA', 0, N'Morocco'),
('MZ', 0, N'Mozambique'),
('MM', 0, N'Myanmar'),
('NA', 0, N'Namibia'),
('NR', 0, N'Nauru'),
('NP', 0, N'Nepal'),
('NL', 0, N'Netherlands'),
('AN', 0, N'Netherlands Antilles'),
('NC', 0, N'New Caledonia'),
('NZ', 0, N'New Zealand'),
('NI', 0, N'Nicaragua'),
('NE', 0, N'Niger'),
('NG', 0, N'Nigeria'),
('NU', 0, N'Niue'),
('NF', 0, N'Norfolk Island'),
('MP', 0, N'Northern Mariana Islands'),
('NO', 0, N'Norway'),
('OM', 0, N'Oman'),
('PK', 0, N'Pakistan'),
('PW', 0, N'Palau'),
('PA', 0, N'Panama'),
('PG', 0, N'Papua New Guinea'),
('PY', 0, N'Paraguay'),
('PE', 0, N'Peru'),
('PH', 0, N'Philippines'),
('PN', 0, N'Pitcairn'),
('PL', 0, N'Poland'),
('PT', 0, N'Portugal'),
('PR', 0, N'Puerto Rico'),
('QA', 0, N'Qatar'),
('RE', 0, N'Reunion'),
('RO', 0, N'Romania'),
('RU', 0, N'Russian Federation'),
('RW', 0, N'Rwanda'),
('KN', 0, N'Saint Kitts and Nevis'),
('LC', 0, N'Saint Lucia'),
('VC', 0, N'Saint Vincent and the Grenadines'),
('WS', 0, N'Samoa'),
('SM', 0, N'San Marino'),
('ST', 0, N'Sao Tome and Principe'),
('SA', 0, N'Saudi Arabia'),
('SN', 0, N'Senegal'),
('SC', 0, N'Seychelles'),
('SL', 0, N'Sierra Leone'),
('SG', 0, N'Singapore'),
('SK', 0, N'Slovakia'),
('SI', 0, N'Slovenia'),
('SB', 0, N'Solomon Islands'),
('SO', 0, N'Somalia'),
('ZA', 0, N'South Africa'),
('GS', 0, N'South Georgia South Sandwich Islands'),
('ES', 0, N'Spain'),
('LK', 0, N'Sri Lanka'),
('SH', 0, N'Saint Helena'),
('PM', 0, N'Saint Pierre and Miquelon'),
('SD', 0, N'Sudan'),
('SR', 0, N'Suriname'),
('SJ', 0, N'Svalbard and Jan Mayen Islands'),
('SZ', 0, N'Swaziland'),
('SE', 0, N'Sweden'),
('CH', 0, N'Switzerland'),
('SY', 0, N'Syria'),
('TW', 0, N'Taiwan'),
('TJ', 0, N'Tajikistan'),
('TZ', 0, N'Tanzania'),
('TH', 0, N'Thailand'),
('TG', 0, N'Togo'),
('TK', 0, N'Tokelau'),
('TO', 0, N'Tonga'),
('TT', 0, N'Trinidad and Tobago'),
('TN', 0, N'Tunisia'),
('TR', 0, N'Turkey'),
('TM', 0, N'Turkmenistan'),
('TC', 0, N'Turks and Caicos Islands'),
('TV', 0, N'Tuvalu'),
('UG', 0, N'Uganda'),
('UA', 0, N'Ukraine'),
('AE', 0, N'United Arab Emirates'),
('GB', 0, N'United Kingdom'),
('UM', 0, N'United States minor outlying islands'),
('UY', 0, N'Uruguay'),
('UZ', 0, N'Uzbekistan'),
('VU', 0, N'Vanuatu'),
('VA', 0, N'Vatican City'),
('VE', 0, N'Venezuela'),
('VN', 0, N'Vietnam'),
('VG', 0, N'Virgin Islands (British)'),
('VI', 0, N'Virgin Islands (U.S.)'),
('WF', 0, N'Wallis and Futuna Islands'),
('EH', 0, N'Western Sahara'),
('YE', 0, N'Yemen'),
('YU', 0, N'Yugoslavia'),
('ZR', 0, N'Zaire'),
('ZM', 0, N'Zambia'),
('ZW', 0, N'Zimbabwe'),
('', 0, N'Anywhere'),
('EU', 1, N'Europe'),
('AS', 1, N'Asia'),
('AF', 1, N'Africa'),
('LA', 1, N'Latin America'),
('ES', 1, N'English-speaking');


CREATE TABLE [dbo].[Majors] (
    [Id]    [int] NOT NULL IDENTITY,
    [Name]  [varchar](128) NOT NULL,

    CONSTRAINT [PK_Majors] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_Majors_Name] UNIQUE ([Name])
);

INSERT INTO [dbo].[Majors]
([Name])
VALUES
('Arts Management'),
('Biology'),
('Business'),
('Business Administration'),
('Business Management'),
('Chemical Engineering'),
('Chemistry'),
('Computer Science'),
('Early Childhood'),
('Education'),
('English'),
('Entrepreneurial Studies'),
('Interdisciplinary Studies'),
('International Business'),
('Journalism and Media Studies'),
('Mechanical Engineering'),
('Music Education'),
('Political Science'),
('Psychology'),
('Social Work'),
('Special Education'),
('Theater'),
('Undecided'),
('Women''s Studies');


CREATE TABLE [dbo].[Minors] (
    [Id]    [int] NOT NULL IDENTITY,
    [Name]  [varchar](128) NOT NULL,

    CONSTRAINT [PK_Minors] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_Minors_Name] UNIQUE ([Name])
);

INSERT INTO [Minors]
([Name])
VALUES
('Arts Management'),
('Biology'),
('Business'),
('Business Administration'),
('Business Management'),
('Chemistry'),
('Computer Science'),
('Early Childhood'),
('Education'),
('English'),
('Entrepreneurial Studies'),
('Global Studies'),
('Interdisciplinary Studies'),
('International Business'),
('Journalism and Media Studies'),
('Mechanical Engineering'),
('Music Education'),
('Political Science'),
('Psychology'),
('Social Work'),
('Spanish'),
('Special Education'),
('Theater'),
('Undecided'),
('Women''s Studies');


CREATE TABLE [dbo].[Languages] (
    [Id]    [int] NOT NULL IDENTITY,
    [Name]  [varchar](32) NOT NULL,

    CONSTRAINT [PK_Languages] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_Languages_Name] UNIQUE ([Name])
);

INSERT INTO [Languages]
([Name])
VALUES
('Mandarin'),
('Spanish'),
('English'),
('Hindi'),
('Arabic'),
('Portuguese'),
('Bengali'),
('Russian'),
('Japanese'),
('Punjabi'),
('German'),
('Javanese'),
('Wu'),
('Malay/Indonesian'),
('Telugu'),
('Vietnamese'),
('Korean'),
('French'),
('Marathi'),
('Tamil'),
('Urdu'),
('Persian'),
('Turkish'),
('Italian'),
('Cantonese'),
('Hausa'),
('Kiswahili');


CREATE TABLE [dbo].[Students] (
    [Id]                        [int] NOT NULL IDENTITY,
    [Created]                   [datetime] NOT NULL,
    [InitialMeeting]            [date],
    [FirstName]                 [nvarchar](64) NOT NULL,
    [MiddleName]                [nvarchar](64),
    [LastName]                  [nvarchar](64) NOT NULL,
    [StreetAddress]             [nvarchar](128),
    [StreetAddress2]            [nvarchar](128),
    [City]                      [nvarchar](128),
    [State]                     [nvarchar](32),
    [PostalCode]                [nvarchar](16),
    [Classification]            [int],
    [StudentId]                 [varchar](32),
    [PhoneNumber]               [varchar](32),
    [LivingOnCampus]            [bit],
    [EnrolledFullTime]          [bit],
    [Citizenship]               [int],
    [PellGrantRecipient]        [bit],
    [PassportHolder]            [bit],
    [PhiBetaDeltaMember]        [bit],
    [Gpa]                       [decimal](3,2),
    [CampusEmail]               [varchar](128),
    [AlternateEmail]            [varchar](128),
    [EnteringYear]              [int],
    [DraduatingYear]            [int],
    [Dob]                       [date],

    CONSTRAINT [PK_Students] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Students_Citizenship] FOREIGN KEY ([Citizenship]) REFERENCES [dbo].[Countries] ([id])
);


CREATE TABLE [dbo].[StudentFluentLanguages] (
    [StudentId]     [int] NOT NULL,
    [LanguageId]    [int] NOT NULL,

    CONSTRAINT [PK_StudentFluentLanguages] PRIMARY KEY ([StudentId], [LanguageId])
);


CREATE TABLE [dbo].[StudentDesiredLanguages] (
    [StudentId]     [int] NOT NULL,
    [LanguageId]    [int] NOT NULL,

    CONSTRAINT [PK_StudentDesiredLanguages] PRIMARY KEY ([StudentId], [LanguageId])
);


CREATE TABLE [dbo].[StudentStudiedLanguages] (
    [StudentId]     [int] NOT NULL,
    [LanguageId]    [int] NOT NULL,

    CONSTRAINT [PK_StudentStudiedLanguages] PRIMARY KEY ([StudentId], [LanguageId])
);


CREATE TABLE [dbo].[Matriculation] (
    [StudentId]     [int] NOT NULL,
    [MajorId]       [int] NOT NULL,
    [IsMajor]       [bit] NOT NULL,

    CONSTRAINT [PK_Matriculation] PRIMARY KEY ([StudentId], [MajorId], [IsMajor])
);


-- Location and semesters that students wish to study abroad to.
CREATE TABLE [dbo].[StudentStudyAbroadWishlist] (
    [StudentId]     [int] NOT NULL,
    [CountryId]     [int],
    [Year]          [int], -- Desired four digit year
    [Period]        [int], -- One of StudentStudyAbroadWishlistModel.PeriodValue

    CONSTRAINT [FK_StudentStudyAbroadWishlist_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id])
);


-- Logins and information about users of the application.
CREATE TABLE [dbo].[Users] (
    [Id]                    [int] NOT NULL IDENTITY,
    [FirstName]             [nvarchar](64) NOT NULL,
    [LastName]              [nvarchar](64) NOT NULL,
    [Login]                 [varchar](24) NOT NULL,
    [PasswordIterations]    [int] NOT NULL,
    [PasswordSalt]          [char](32) NOT NULL,
    [PasswordHash]          [char](32) NOT NULL,
    [Created]               [datetime] NOT NULL,
    [LastLogin]             [datetime],
    [Email]                 [varchar](128) NOT NULL,
    [Admin]                 [bit] NOT NULL DEFAULT 0,
    [Active]                [bit] NOT NULL DEFAULT 1,

    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_Users_Login] UNIQUE ([Login])
);

-- Password is the same as the login.
INSERT INTO [dbo].[Users]
([FirstName], [LastName], [Login], [PasswordIterations], [PasswordSalt], [PasswordHash], [Created], [Email], [Admin])
VALUES
(N'Andrei', N'Nicholson', 'anicholson', 1000, 'og85e2R6TvXl+8SuOqv3EWTc7eWX3aje', 'RzfdfsXpQQTg1E+n3wnLMEZbjJoEkqEf', GETUTCDATE(), 'contact@andreinicholson.com', 1);


-- Promotional users attached to student entries.
CREATE TABLE [dbo].[UserPromo] (
    [Id]                [int] NOT NULL IDENTITY,
    [Description]       [nvarchar](256) NOT NULL,
    [CreatedBy]         [int] NOT NULL,
    [Created]           [datetime] NOT NULL,
    [Code]              [varchar](32) NOT NULL,
    [Active]            [bit] NOT NULL DEFAULT 1,

    CONSTRAINT [PK_UserPromo] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_UserPromo_Code] UNIQUE ([Code]),
    CONSTRAINT [FK_UserPromo_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([Id])
);


CREATE TABLE [dbo].[StudentNotes] (
    [Id]            [int] NOT NULL IDENTITY,
    [StudentId]     [int] NOT NULL,
    [CreatedBy]     [int] NOT NULL,
    [EntryDate]     [datetime] NOT NULL,
    [Note]          [nvarchar](max) NOT NULL,

    CONSTRAINT [PK_StudentNotes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentNotes_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id]),
    CONSTRAINT [FK_StudentNotes_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([Id])
);


CREATE TABLE [dbo].[EventLog] (
    [Id]            [int] NOT NULL IDENTITY,
    [Date]          [datetime] NOT NULL,
    [ModifiedBy]    [int],
    [StudentId]     [int],
    [UserId]        [int],
    [Type]          [int] NOT NULL,
    [Action]        [nvarchar](512),

    CONSTRAINT [PK_EventLog] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EventLog_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_EventLog_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id]),
    CONSTRAINT [FK_EventLog_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);


CREATE TABLE [dbo].[StudentPromoLog] (
    [PromoId]       [int] NOT NULL,
    [StudentId]     [int] NOT NULL,
    [Created]       [datetime] NOT NULL,

    CONSTRAINT [PK_StudentPromoLog] PRIMARY KEY ([PromoId], [StudentId]),
    CONSTRAINT [FK_StudentPromoLog_PromoId] FOREIGN KEY ([PromoId]) REFERENCES [dbo].[UserPromo] ([Id]),
    CONSTRAINT [FK_StudentPromoLog_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id])
);


CREATE TABLE [dbo].[Programs] (
    [id]            [int] NOT NULL IDENTITY,
    [name]          [nvarchar](128) NOT NULL,
    [abbreviation]  [varchar](24),

    CONSTRAINT [PK_Programs] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_Programs_Name_Abbreviation] UNIQUE([name], [abbreviation])
);


INSERT INTO [dbo].[Programs]
([name], [abbreviation])
VALUES
(N'Organization for Tropical Studies', 'OTS'),
(N'Multidisciplinary International Research Training program', 'MIRT'),
(N'American Institute for Foreign Study', 'AIFS'),
(N'New York University', 'NYU'),
(N'Brethren Colleges Abroad', 'BCA'),
(N'School for International Training', 'SIT'),
(N'International Study Program', 'ISP'),
(N'International Exchange Student Program', 'ISEP'),
(N'University of Virgin Islands', 'UVI'),
(N'University of North Carolina at Chapel Hill', 'UNC'),
(N'Global Learning Semesters', NULL),
(N'Semester At Sea', NULL),
(N'Veritas Universidad', NULL),
(N'UMC Volunteer', NULL),
(N'Spring Break', NULL),
(N'GlobaLinks', NULL),
(N'Scranton Women''s Leadership Cntr.', NULL),
(N'Bennett Maymester', NULL),
(N'Global Bus. Leadership Experience', NULL),
(N'UN Climate Change Conference', NULL),
(N'New Media', NULL),
(N'NYU Florence', NULL),
(N'NYU Ghana', NULL),
(N'Grahamstown Festival', NULL),
(N'Council on International Educational Exchange', 'CIEE'),
(N'CISabroad', 'CIS'),
(N'Mid-Atlantic Consortium-Center for Academic Excellence', 'MAC-CAE'),
(N'NYU London', NULL),
(N'Institute for Future Global Leaders UVI', NULL),
(N'Tec de Monterrey', NULL),
(N'Global Semesters', NULL),
(N'Global Linkages', NULL);


CREATE TABLE [dbo].[ProgramTypes] (
    [Id]            [int] NOT NULL IDENTITY,
    [Name]          [nvarchar](32) NOT NULL,
    [ShortTerm]     [bit] NOT NULL,

    CONSTRAINT [PK_ProgramTypes] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_ProgramTypes_Name] UNIQUE ([Name])
);


INSERT INTO [dbo].[ProgramTypes]
([Name], [ShortTerm])
VALUES
(N'Maymester', 1),
(N'Spring Break', 1),
(N'Internship', 1),
(N'Research Experience (Short-Term)', 1),
(N'Research Experience (Long-Term)', 0),
(N'Semester', 0),
(N'Summer', 1),
(N'Other', 1),
(N'Academic Year', 0);


CREATE TABLE [dbo].[StudyAbroad] (
    [Id]                [int] NOT NULL IDENTITY,
    [StudentId]         [int] NOT NULL,
    [Semester]          [int] NOT NULL,
    [Year]              [int] NOT NULL,
    [StartDate]         [date],
    [EndDate]           [date],
    [CreditBearing]     [bit] NOT NULL,
    [Internship]        [bit] NOT NULL,
    [CountryId]         [int] NOT NULL,
    [City]              [varchar](64),
    [ProgramId]         [int] NOT NULL,

    CONSTRAINT [PK_StudyAbroad] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_StudyAbroad_StudentId_Semester_Year_CountryId] UNIQUE ([StudentId], [Semester], [Year], [CountryId]),
    CONSTRAINT [FK_StudyAbroad_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id]),
    CONSTRAINT [FK_StudyAbroad_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Countries] ([Id]),
    CONSTRAINT [FK_StudyAbroad_ProgramId] FOREIGN KEY ([ProgramId]) REFERENCES [dbo].[Programs] ([Id])
);


CREATE TABLE [dbo].[StudyAbroadProgramTypes] (
    [StudyAbroadId]     [int] NOT NULL,
    [ProgramTypeId]     [int] NOT NULL,

    CONSTRAINT [PK_StudyAbroadProgramTypes] PRIMARY KEY ([StudyAbroadId], [ProgramTypeId]),
    CONSTRAINT [FK_StudyAbroadProgramTypes_ProgramTypeId] FOREIGN KEY ([ProgramTypeId]) REFERENCES [dbo].[ProgramTypes] ([Id])
);


CREATE TABLE [dbo].[ActivityLogPerson] (
    [Id]            [int] NOT NULL IDENTITY,
    [FullName]      [nvarchar](128) NOT NULL,
    [Description]   [nvarchar](256),
    [Phone]         [varchar](32),
    [Email]         [varchar](128),

    CONSTRAINT [PK_ActivityLogPerson] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_ActivityLogPerson_FullName] UNIQUE ([FullName])
);


CREATE TABLE [dbo].[ActivityLog] (
    [Id]            [int] NOT NULL IDENTITY,
    [Created]       [datetime] NOT NULL,
    [CreatedBy]     [int] NOT NULL,
    [Title]         [nvarchar](256) NOT NULL,
    [Title2]        [nvarchar](256),
    [Title3]        [nvarchar](256),
    [Organizers]    [nvarchar](256),
    [Location]      [nvarchar](512),
    [StartDate]     [date] NOT NULL,
    [EndDate]       [date] NOT NULL,
    [OnCampus]      [bit] NOT NULL,
    [WebSite]       [varchar](2048),
    [Notes]         [nvarchar](max),

    CONSTRAINT [PK_ActivityLog] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_ActivityLog_Title] UNIQUE ([Title]),
    CONSTRAINT [FK_ActivityLog_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([Id])
);


-- Each activity log much have at least one record in this table. The TypeId
-- value matches the ActivityLogTypes enum.
CREATE TABLE [dbo].[ActivityLogTypes] (
    [EventId]       [int] NOT NULL,
    [TypeId]        [int] NOT NULL,

    CONSTRAINT [PK_ActivityLogTypes] PRIMARY KEY ([EventId], [TypeId]),
    CONSTRAINT [FK_ActivityLogTypes_EventId] FOREIGN KEY ([EventId]) REFERENCES [dbo].[ActivityLog] ([Id])
);


CREATE TABLE [dbo].[ActivityLogParticipant] (
    [EventId]           [int] NOT NULL,
    [PersonId]          [int] NOT NULL,
    [ParticipantType]   [int] NOT NULL,

    CONSTRAINT [PK_ActivityLogParticipant] PRIMARY KEY ([EventId], [PersonId]),
    CONSTRAINT [FK_ActivityLogParticipant_EventId] FOREIGN KEY ([EventId]) REFERENCES [dbo].[ActivityLog] ([Id]),
    CONSTRAINT [FK_ActivityLogParticipant_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[ActivityLogPerson] ([Id])
);


CREATE TABLE [dbo].[Exceptions](
    [Id]                [bigint] NOT NULL IDENTITY,
    [GUID]              [uniqueidentifier] NOT NULL,
    [ApplicationName]   [nvarchar](50) NOT NULL,
    [MachineName]       [nvarchar](50) NOT NULL,
    [CreationDate]      [datetime] NOT NULL,
    [Type]              [nvarchar](100) NOT NULL,
    [IsProtected]       [bit] NOT NULL default(0),
    [Host]              [nvarchar](100) NULL,
    [Url]               [nvarchar](500) NULL,
    [HTTPMethod]        [nvarchar](10) NULL,
    [IPAddress]         [varchar](40) NULL,
    [Source]            [nvarchar](100) NULL,
    [Message]           [nvarchar](1000) NULL,
    [Detail]            [nvarchar](max) NULL,	
    [StatusCode]        [int] NULL,
    [SQL]               [nvarchar](max) NULL,
    [DeletionDate]      [datetime] NULL,
    [FullJson]          [nvarchar](max) NULL,
    [ErrorHash]         [int] NULL,
    [DuplicateCount]    [int] NOT NULL default(1),

    CONSTRAINT [PK_Exceptions] PRIMARY KEY CLUSTERED ([Id] ASC)
    WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_Exceptions_GUID_ApplicationName_DeletionDate_CreationDate] ON [dbo].[Exceptions] 
(
	[GUID] ASC,
	[ApplicationName] ASC,
	[DeletionDate] ASC,
	[CreationDate] DESC
);

CREATE NONCLUSTERED INDEX [IX_Exceptions_ErrorHash_ApplicationName_CreationDate_DeletionDate] ON [dbo].[Exceptions] 
(
	[ErrorHash] ASC,
	[ApplicationName] ASC,
	[CreationDate] DESC,
	[DeletionDate] ASC
);

CREATE NONCLUSTERED INDEX [IX_Exceptions_ApplicationName_DeletionDate_CreationDate_Filtered] ON [dbo].[Exceptions] 
(
	[ApplicationName] ASC,
	[DeletionDate] ASC,
	[CreationDate] DESC
)
WHERE DeletionDate Is Null;

GO


CREATE PROCEDURE [dbo].[DeleteStudent]
    @StudentId [int]
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [dbo].[Students] WHERE [StudentId] = @StudentId)
    BEGIN TRY
        THROW 50001, 'Student ID not found in [Users] table.', 1
    END TRY
    BEGIN CATCH
    END CATCH

    DELETE FROM [dbo].[EventLog] WHERE [StudentId] = @StudentId;
    DELETE FROM [dbo].[Matriculation] WHERE [StudentId] = @StudentId;
    DELETE FROM [dbo].[StudentDesiredLanguages] WHERE [StudentId] = @StudentId;
    DELETE FROM [dbo].[StudentFluentLanguages] WHERE [StudentId] = @StudentId;
    DELETE FROM [dbo].[StudentNotes] WHERE [StudentId] = @StudentId;
    DELETE FROM [dbo].[StudentStudyAbroadWishlist] WHERE [StudentId] = @StudentId;
    DELETE FROM [dbo].[StudyAbroad] WHERE [StudentId] = @StudentId;
    DELETE FROM [dbo].[Students] WHERE [Id] = @StudentId;
END
GO
