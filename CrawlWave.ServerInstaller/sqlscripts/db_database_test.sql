IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'CrawlWaveTest')
	DROP DATABASE [CrawlWaveTest]
GO

CREATE DATABASE [CrawlWaveTest]
ON PRIMARY
( NAME = N'CrawlWaveTest_Indexes',
   FILENAME = N'@IndexesPath@CrawlWaveTest_Indexes.MDF',
   SIZE = @IndexesSize@MB,
   @IndexesSizeMax@
   FILEGROWTH = 15% ),
FILEGROUP [SECONDARY]
( NAME = N'CrawlWaveTest_Data',
   FILENAME = N'@DataPath@CrawlWaveTest_Data.NDF',
   SIZE = @DataSize@MB,
   @DataSizeMax@
   FILEGROWTH = 15% )
LOG ON
( NAME = N'CrawlWaveTest_Log',
   FILENAME = N'@LogPath@CrawlWaveTest_Log.LDF',
   SIZE = @LogSize@MB,
   @LogSizeMax@
   FILEGROWTH = 10% )
 COLLATE Greek_CI_AS
GO

exec sp_dboption N'CrawlWaveTest', N'autoclose', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'bulkcopy', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'trunc. log', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'torn page detection', N'true'
GO

exec sp_dboption N'CrawlWaveTest', N'read only', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'dbo use', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'single', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'autoshrink', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'ANSI null default', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'recursive triggers', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'ANSI nulls', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'concat null yields null', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'cursor close on commit', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'default to local cursor', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'quoted identifier', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'ANSI warnings', N'false'
GO

exec sp_dboption N'CrawlWaveTest', N'auto create statistics', N'true'
GO

exec sp_dboption N'CrawlWaveTest', N'auto update statistics', N'true'
GO
