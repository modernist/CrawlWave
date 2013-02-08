IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'CrawlWave')
	DROP DATABASE [CrawlWave]
GO

CREATE DATABASE [CrawlWave]
ON PRIMARY
( NAME = N'CrawlWave_Indexes',
   FILENAME = N'@IndexesPath@CrawlWave_Indexes.MDF',
   SIZE = @IndexesSize@MB,
   @IndexesSizeMax@
   FILEGROWTH = 15% ),
FILEGROUP [SECONDARY]
( NAME = N'CrawlWave_Data',
   FILENAME = N'@DataPath@CrawlWave_Data.NDF',
   SIZE = @DataSize@MB,
   @DataSizeMax@
   FILEGROWTH = 15% )
LOG ON
( NAME = N'CrawlWave_Log',
   FILENAME = N'@LogPath@CrawlWave_Log.LDF',
   SIZE = @LogSize@MB,
   @LogSizeMax@
   FILEGROWTH = 10% )
 COLLATE Greek_CI_AS
GO

exec sp_dboption N'CrawlWave', N'autoclose', N'false'
GO

exec sp_dboption N'CrawlWave', N'bulkcopy', N'false'
GO

exec sp_dboption N'CrawlWave', N'trunc. log', N'true'
GO

exec sp_dboption N'CrawlWave', N'torn page detection', N'true'
GO

exec sp_dboption N'CrawlWave', N'read only', N'false'
GO

exec sp_dboption N'CrawlWave', N'dbo use', N'false'
GO

exec sp_dboption N'CrawlWave', N'single', N'false'
GO

exec sp_dboption N'CrawlWave', N'autoshrink', N'false'
GO

exec sp_dboption N'CrawlWave', N'ANSI null default', N'false'
GO

exec sp_dboption N'CrawlWave', N'recursive triggers', N'false'
GO

exec sp_dboption N'CrawlWave', N'ANSI nulls', N'false'
GO

exec sp_dboption N'CrawlWave', N'concat null yields null', N'false'
GO

exec sp_dboption N'CrawlWave', N'cursor close on commit', N'false'
GO

exec sp_dboption N'CrawlWave', N'default to local cursor', N'false'
GO

exec sp_dboption N'CrawlWave', N'quoted identifier', N'false'
GO

exec sp_dboption N'CrawlWave', N'ANSI warnings', N'false'
GO

exec sp_dboption N'CrawlWave', N'auto create statistics', N'true'
GO

exec sp_dboption N'CrawlWave', N'auto update statistics', N'true'
GO
