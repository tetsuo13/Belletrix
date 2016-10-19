-- Update country codes to ISO 3166-1 alpha-2.

-- Prefix some which are not part of the ISO.
UPDATE  [dbo].[Countries]
SET     [Abbreviation] =
            CASE
                WHEN [Name] = 'Asia' AND [Abbreviation] = 'AS' AND [IsRegion] = 1 THEN 'XAS'
                WHEN [Name] = 'Africa' AND [Abbreviation] = 'AF' AND [IsRegion] = 1 THEN 'XAF'
                WHEN [Name] = 'Latin America' AND [Abbreviation] = 'LA' AND [IsRegion] = 1 THEN 'XLA'
                WHEN [Name] = 'English-speaking' AND [Abbreviation] = 'ES' AND [IsRegion] = 1 THEN 'XES'
                WHEN [Name] = 'Spanish-speaking' AND [Abbreviation] = 'SS' AND [IsRegion] = 1 THEN 'XSS'
                ELSE [Abbreviation]
            END;

-- Country code corrections.
UPDATE  [dbo].[Countries]
SET     [Abbreviation] =
            CASE
                WHEN [Name] = 'American Samoa' AND [Abbreviation] = 'DS' AND [IsRegion] = 0 THEN 'AS'
                WHEN [Name] = 'Mayotte' AND [Abbreviation] = 'TY' AND [IsRegion] = 0 THEN 'YT'
                ELSE [Abbreviation]
            END;

UPDATE  [dbo].[Countries]
SET     [Name] =
            CASE [Abbreviation]
                WHEN 'IO' THEN 'British Indian Ocean Territory'
                WHEN 'CV' THEN 'Cabo Verde'
                WHEN 'HR' THEN 'Croatia'
                WHEN 'CZ' THEN 'Czechia'
                WHEN 'EC' THEN 'Ecuador'
                WHEN 'HM' THEN 'Heard Island and McDonald Islands'
                WHEN 'MO' THEN 'Macao'
                WHEN 'UM' THEN 'United States Minor Outlying Islands'
                WHEN 'WF' THEN 'Wallis and Futuna'
                WHEN 'SJ' THEN 'Svalbard and Jan Mayen'
                ELSE [Name]
            END;

INSERT INTO [dbo].[Countries]
([Name], [Abbreviation], [IsRegion])
VALUES
('Congo, the Democratic Republic of the', 'CD', 0);

-- Get rid of the obsolete password hashing scheme.

-- Obsolete columns used in previous password storage system.
ALTER TABLE [dbo].[Users] DROP COLUMN [PasswordIterations];
ALTER TABLE [dbo].[Users] DROP COLUMN [PasswordSalt];
ALTER TABLE [dbo].[Users] DROP COLUMN [PasswordHash];

-- All users have updated their profile with a password using the new system.
-- Now the column can be correctly constrained.
ALTER TABLE [dbo].[Users] ALTER COLUMN [Password] VARCHAR(160) NOT NULL;
