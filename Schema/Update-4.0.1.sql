USE [Belletrix];

-- Drop the null contrainst on the on/off campus column.
ALTER TABLE [dbo].[ActivityLog] ALTER COLUMN [OnCampus] [bit] NULL;

-- Add "other" program for experiences.
INSERT INTO [dbo].[Programs] ([Name], [Abbreviation]) VALUES (N'Other', 'OTHER');
