-- The public key shown in the URL that allows a visitor direct access to an
-- associated promo's entry form.
ALTER TABLE [Belletrix].[dbo].[UserPromo] ADD [PublicToken] [char](32);

-- Log user's IP address to start tracking where a student is altered from.
ALTER TABLE [Belletrix].[dbo].[EventLog] ADD [IPAddress] [varchar](15);
