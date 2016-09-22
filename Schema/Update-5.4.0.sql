-- No longer need unique codes to access promos.
ALTER TABLE [Belletrix].[dbo].[UserPromo] DROP CONSTRAINT [UN_UserPromo_Code];
ALTER TABLE [Belletrix].[dbo].[UserPromo] DROP COLUMN [Code];

-- The public key shown in the URL that allows a visitor direct access to an
-- associated promo's entry form.
ALTER TABLE [Belletrix].[dbo].[UserPromo] ADD [PublicToken] [uniqueidentifier];

UPDATE [Belletrix].[dbo].[UserPromo] SET [PublicToken] = NEWID();

ALTER TABLE [Belletrix].[dbo].[UserPromo] ALTER COLUMN [PublicToken] [uniqueidentifier] NOT NULL;


-- Log user's IP address to start tracking where a student is altered from.
ALTER TABLE [Belletrix].[dbo].[EventLog] ADD [IPAddress] [varchar](15);
