UPDATE  [dbo].[Students]
SET     [CampusEmail] = LOWER([CampusEmail]),
        [AlternateEmail] = LOWER([AlternateEmail]);

UPDATE  [dbo].[ActivityLogPerson]
SET     [Email] = LOWER([Email]);
