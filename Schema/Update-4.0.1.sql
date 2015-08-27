USE [Belletrix];

-- Drop the null contrainst on the on/off campus column.
ALTER TABLE [dbo].[ActivityLog] ALTER COLUMN [OnCampus] [bit] NULL;
