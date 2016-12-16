USE [Belletrix];

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
IF OBJECT_ID('dbo.ActivityLogTypes', 'U') IS NOT NULL DROP TABLE [dbo].[ActivityLogTypes];
IF OBJECT_ID('dbo.ActivityLog', 'U') IS NOT NULL DROP TABLE [dbo].[ActivityLog];
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE [dbo].[Users];
IF OBJECT_ID('dbo.Documents', 'U') IS NOT NULL DROP TABLE [dbo].[Documents];
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

-- https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2
-- Country codes not in ISO 3166-1 prefixed with "X".
INSERT INTO [dbo].[Countries]
([Name], [Abbreviation], [IsRegion])
VALUES
('Europe', 'EU', 1),
('Asia', 'XAS', 1),
('Africa', 'XAF', 1),
('Latin America', 'XLA', 1),
('English-speaking', 'XES', 1),
('United States', 'US', 0),
('Canada', 'CA', 0),
('Albania', 'AL', 0),
('Afghanistan', 'AF', 0),
('Algeria', 'DZ', 0),
('American Samoa', 'AS', 0),
('Andorra', 'AD', 0),
('Angola', 'AO', 0),
('Anguilla', 'AI', 0),
('Antarctica', 'AQ', 0),
('Antigua and/or Barbuda', 'AG', 0),
('Argentina', 'AR', 0),
('Armenia', 'AM', 0),
('Aruba', 'AW', 0),
('Australia', 'AU', 0),
('Austria', 'AT', 0),
('Azerbaijan', 'AZ', 0),
('Bahamas', 'BS', 0),
('Bahrain', 'BH', 0),
('Bangladesh', 'BD', 0),
('Barbados', 'BB', 0),
('Belarus', 'BY', 0),
('Belgium', 'BE', 0),
('Belize', 'BZ', 0),
('Benin', 'BJ', 0),
('Bermuda', 'BM', 0),
('Bhutan', 'BT', 0),
('Bolivia', 'BO', 0),
('Bosnia and Herzegovina', 'BA', 0),
('Botswana', 'BW', 0),
('Bouvet Island', 'BV', 0),
('Brazil', 'BR', 0),
('British Indian Ocean Territory', 'IO', 0),
('Brunei Darussalam', 'BN', 0),
('Bulgaria', 'BG', 0),
('Burkina Faso', 'BF', 0),
('Burundi', 'BI', 0),
('Cambodia', 'KH', 0),
('Cameroon', 'CM', 0),
('Cabo Verde', 'CV', 0),
('Cayman Islands', 'KY', 0),
('Central African Republic', 'CF', 0),
('Chad', 'TD', 0),
('Chile', 'CL', 0),
('China', 'CN', 0),
('Christmas Island', 'CX', 0),
('Cocos (Keeling) Islands', 'CC', 0),
('Colombia', 'CO', 0),
('Comoros', 'KM', 0),
('Congo, the Democratic Republic of the', 'CD', 0),
('Congo', 'CG', 0),
('Cook Islands', 'CK', 0),
('Costa Rica', 'CR', 0),
('Croatia', 'HR', 0),
('Cuba', 'CU', 0),
('Cyprus', 'CY', 0),
('Czechia', 'CZ', 0),
('Denmark', 'DK', 0),
('Djibouti', 'DJ', 0),
('Dominica', 'DM', 0),
('Dominican Republic', 'DO', 0),
('East Timor', 'TP', 0),
('Ecuador', 'EC', 0),
('Egypt', 'EG', 0),
('El Salvador', 'SV', 0),
('Equatorial Guinea', 'GQ', 0),
('Eritrea', 'ER', 0),
('Estonia', 'EE', 0),
('Ethiopia', 'ET', 0),
('Falkland Islands (Malvinas)', 'FK', 0),
('Faroe Islands', 'FO', 0),
('Fiji', 'FJ', 0),
('Finland', 'FI', 0),
('France', 'FR', 0),
('France, Metropolitan', 'FX', 0),
('French Guiana', 'GF', 0),
('French Polynesia', 'PF', 0),
('French Southern Territories', 'TF', 0),
('Gabon', 'GA', 0),
('Gambia', 'GM', 0),
('Georgia', 'GE', 0),
('Germany', 'DE', 0),
('Ghana', 'GH', 0),
('Peru', 'PE', 0),
('Philippines', 'PH', 0),
('Pitcairn', 'PN', 0),
('Poland', 'PL', 0),
('Portugal', 'PT', 0),
('Puerto Rico', 'PR', 0),
('Qatar', 'QA', 0),
('Reunion', 'RE', 0),
('Romania', 'RO', 0),
('Gibraltar', 'GI', 0),
('Greece', 'GR', 0),
('Greenland', 'GL', 0),
('Grenada', 'GD', 0),
('Guadeloupe', 'GP', 0),
('Guam', 'GU', 0),
('Guatemala', 'GT', 0),
('Guinea', 'GN', 0),
('Guinea-Bissau', 'GW', 0),
('Guyana', 'GY', 0),
('Haiti', 'HT', 0),
('Heard Island and McDonald Islands', 'HM', 0),
('Honduras', 'HN', 0),
('Hong Kong', 'HK', 0),
('Hungary', 'HU', 0),
('Iceland', 'IS', 0),
('India', 'IN', 0),
('Indonesia', 'ID', 0),
('Iran (Islamic Republic of)', 'IR', 0),
('Iraq', 'IQ', 0),
('Ireland', 'IE', 0),
('Israel', 'IL', 0),
('Italy', 'IT', 0),
('Ivory Coast', 'CI', 0),
('Jamaica', 'JM', 0),
('Japan', 'JP', 0),
('Jordan', 'JO', 0),
('Kazakhstan', 'KZ', 0),
('Kenya', 'KE', 0),
('Kiribati', 'KI', 0),
('Kuwait', 'KW', 0),
('Kyrgyzstan', 'KG', 0),
('Latvia', 'LV', 0),
('Lebanon', 'LB', 0),
('Lesotho', 'LS', 0),
('Liberia', 'LR', 0),
('Liechtenstein', 'LI', 0),
('Lithuania', 'LT', 0),
('Luxembourg', 'LU', 0),
('Macao', 'MO', 0),
('Macedonia', 'MK', 0),
('Madagascar', 'MG', 0),
('Malawi', 'MW', 0),
('Malaysia', 'MY', 0),
('Maldives', 'MV', 0),
('Mali', 'ML', 0),
('Malta', 'MT', 0),
('Marshall Islands', 'MH', 0),
('Martinique', 'MQ', 0),
('Mauritania', 'MR', 0),
('Mauritius', 'MU', 0),
('Mayotte', 'YT', 0),
('Mexico', 'MX', 0),
('Monaco', 'MC', 0),
('Mongolia', 'MN', 0),
('Montserrat', 'MS', 0),
('Morocco', 'MA', 0),
('Mozambique', 'MZ', 0),
('Myanmar', 'MM', 0),
('Namibia', 'NA', 0),
('Nauru', 'NR', 0),
('Nepal', 'NP', 0),
('Netherlands', 'NL', 0),
('Netherlands Antilles', 'AN', 0),
('New Caledonia', 'NC', 0),
('New Zealand', 'NZ', 0),
('Nicaragua', 'NI', 0),
('Niger', 'NE', 0),
('Nigeria', 'NG', 0),
('Niue', 'NU', 0),
('Northern Mariana Islands', 'MP', 0),
('Norway', 'NO', 0),
('Oman', 'OM', 0),
('Pakistan', 'PK', 0),
('Palau', 'PW', 0),
('Panama', 'PA', 0),
('Papua New Guinea', 'PG', 0),
('Paraguay', 'PY', 0),
('North Korea', 'KP', 0),
('Laos', 'LA', 0),
('Libya', 'LY', 0),
('Micronesia', 'FM', 0),
('Moldova', 'MD', 0),
('Norfolk Island', 'NF', 0),
('Russian Federation', 'RU', 0),
('Rwanda', 'RW', 0),
('Saint Kitts and Nevis', 'KN', 0),
('Saint Lucia', 'LC', 0),
('Saint Vincent and the Grenadines', 'VC', 0),
('Samoa', 'WS', 0),
('San Marino', 'SM', 0),
('Sao Tome and Principe', 'ST', 0),
('Saudi Arabia', 'SA', 0),
('Senegal', 'SN', 0),
('Seychelles', 'SC', 0),
('Sierra Leone', 'SL', 0),
('Singapore', 'SG', 0),
('Slovakia', 'SK', 0),
('Slovenia', 'SI', 0),
('Solomon Islands', 'SB', 0),
('Somalia', 'SO', 0),
('South Africa', 'ZA', 0),
('South Georgia South Sandwich Islands', 'GS', 0),
('Spain', 'ES', 0),
('Sri Lanka', 'LK', 0),
('Sudan', 'SD', 0),
('Suriname', 'SR', 0),
('Swaziland', 'SZ', 0),
('Sweden', 'SE', 0),
('Switzerland', 'CH', 0),
('Taiwan', 'TW', 0),
('Tajikistan', 'TJ', 0),
('Thailand', 'TH', 0),
('Togo', 'TG', 0),
('Tokelau', 'TK', 0),
('Tonga', 'TO', 0),
('Trinidad and Tobago', 'TT', 0),
('Tunisia', 'TN', 0),
('Turkey', 'TR', 0),
('Turkmenistan', 'TM', 0),
('Turks and Caicos Islands', 'TC', 0),
('Tuvalu', 'TV', 0),
('Uganda', 'UG', 0),
('Ukraine', 'UA', 0),
('United Arab Emirates', 'AE', 0),
('United Kingdom', 'GB', 0),
('United States Minor Outlying Islands', 'UM', 0),
('Uruguay', 'UY', 0),
('Uzbekistan', 'UZ', 0),
('Vanuatu', 'VU', 0),
('Venezuela', 'VE', 0),
('Vietnam', 'VN', 0),
('Virgin Islands (U.S.)', 'VI', 0),
('Wallis and Futuna', 'WF', 0),
('Western Sahara', 'EH', 0),
('Yemen', 'YE', 0),
('Yugoslavia', 'YU', 0),
('Zaire', 'ZR', 0),
('Zambia', 'ZM', 0),
('Zimbabwe', 'ZW', 0),
('Anywhere', '', 0),
('South Korea', 'KR', 0),
('Saint Helena', 'SH', 0),
('Saint Pierre and Miquelon', 'PM', 0),
('Svalbard and Jan Mayen', 'SJ', 0),
('Syria', 'SY', 0),
('Tanzania', 'TZ', 0),
('Vatican City', 'VA', 0),
('Virgin Islands (British)', 'VG', 0),
('Spanish-speaking', 'XSS', 1);


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
    [GraduatingYear]            [int],
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
    [Password]              [varchar](160) NOT NULL,
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
([FirstName], [LastName], [Login], [Password], [Created], [Email], [Admin])
VALUES
(N'Andrei', N'Nicholson', 'anicholson', 'ABRELpYv0o9L+GMhtXd+Vd0jf2KceFc+XYb9vGFb2Riuqvb6UcnFu3ibwUWprGRdJw==', GETUTCDATE(), 'contact@andreinicholson.com', 1);


-- Promotional users attached to student entries.
CREATE TABLE [dbo].[UserPromo] (
    [Id]                [int] NOT NULL IDENTITY,
    [Description]       [nvarchar](256) NOT NULL,
    [CreatedBy]         [int] NOT NULL,
    [Created]           [datetime] NOT NULL,
    [Active]            [bit] NOT NULL DEFAULT 1,

	-- Not the primary key so that it can be regenerated at any time for
	-- security reasons.
    [PublicToken]       [uniqueidentifier] NOT NULL,

    CONSTRAINT [PK_UserPromo] PRIMARY KEY ([Id]),
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
    [IPAddress]     [varchar](15),

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
    [Id]            [int] NOT NULL IDENTITY,
    [Name]          [nvarchar](128) NOT NULL,
    [Abbreviation]  [varchar](24),

    CONSTRAINT [PK_Programs] PRIMARY KEY ([Id]),
    CONSTRAINT [UN_Programs_Name_Abbreviation] UNIQUE([Name], [Abbreviation])
);


INSERT INTO [dbo].[Programs]
([Name], [Abbreviation])
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
(N'Global Linkages', NULL),
(N'Other', 'OTHER');


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
(N'Academic Year', 0),
(N'Service-learning', 0),
(N'Volunteer', 0);


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
    [OnCampus]      [bit],
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


CREATE TABLE [dbo].[Documents] (
    [Id]                [int] NOT NULL IDENTITY,
    [Guid]              [uniqueidentifier] NOT NULL,
    [Created]           [datetime] NOT NULL,
    [CreatedBy]         [int] NOT NULL,
    [LastModified]      [datetime],
    [LastModifiedBy]    [int],
    [ActivityLogId]     [int],
    [Title]             [nvarchar](1024) NOT NULL,
    [Size]              [int] NOT NULL,
    [Content]           [varbinary](max) NOT NULL,

    CONSTRAINT [FK_Documents_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Documents_ActivityLogId] FOREIGN KEY ([ActivityLogId]) REFERENCES [dbo].[ActivityLog] ([Id])
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
