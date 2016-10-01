-- Obsolete columns used in previous password storage system.
ALTER TABLE [dbo].[Users] DROP COLUMN [PasswordIterations];
ALTER TABLE [dbo].[Users] DROP COLUMN [PasswordSalt];
ALTER TABLE [dbo].[Users] DROP COLUMN [PasswordHash];

-- All users have updated their profile with a password using the new system.
-- Now the column can be correctly constrained.
ALTER TABLE [dbo].[Users] ALTER COLUMN [Password] VARCHAR(160) NOT NULL;
