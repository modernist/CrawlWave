if not (N'@CrawlWaveDBUser@' = N'sa')
BEGIN
	if not exists (select * from master.dbo.syslogins where loginname = N'@CrawlWaveDBUser@')
	BEGIN
		declare @logindb nvarchar(132), @loginlang nvarchar(132)
		select @logindb = N'CrawlWave', @loginlang = N'ελληνικά'

		if @logindb is null or not exists (select * from master.dbo.sysdatabases where name = @logindb)

			select @logindb = N'master'

		if @loginlang is null or (not exists (select * from master.dbo.syslanguages where name = @loginlang) and @loginlang <> N'us_english')
			select @loginlang = @@language

		exec sp_addlogin N'@CrawlWaveDBUser@', '@CrawlWaveDBPass@', @logindb, @loginlang
	END
END
GO

exec sp_addsrvrolemember N'@CrawlWaveDBUser@', dbcreator
GO

if not exists (select * from master.dbo.syslogins where loginname = N'@MachineName@\ASPNET')
BEGIN
	exec sp_grantlogin N'@MachineName@\ASPNET'
	exec sp_defaultdb N'@MachineName@\ASPNET', N'master'
	exec sp_defaultlanguage N'@MachineName@\ASPNET', N'us_english'
END
GO

USE [CrawlWave]
if not exists (select * from dbo.sysusers where name = N'ASPNET' and uid < 16382)
  EXEC sp_grantdbaccess N'@MachineName@\ASPNET', N'ASPNET'
GO

USE [CrawlWave]
if not exists (select * from dbo.sysusers where name = N'@CrawlWaveDBUser@' and uid < 16382)
  EXEC sp_grantdbaccess N'@CrawlWaveDBUser@'
GO

USE [CrawlWave]
exec sp_addrolemember N'db_owner', N'@CrawlWaveDBUser@'
GO
