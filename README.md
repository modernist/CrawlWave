CrawlWave
=========

An obsolete and antiquated small scale distributed web crawler in C#

Written back in 2003-2004, CrawlWave is a distributed web crawler, primarily targeted to greek web pages.

Current Status
--------------

Given that the project was targeted at .NET Framework 2.0 using the technologies available at the time, it may not compile cleanly under new versions of the framework. Effort has been put into making it build successfully under MS Visual Studio 2010, but further modernization is required.

Moreover, since it was a student project (the outcome of an MSc Thesis), it suffers from many design issues. Some of them are described in the following TODO list.

TODO List:
----------

1. Create a proper db access layer based on System.Data.Common or employ a No-SQL DB like MongoDB
2. Remove Web services, replace them by WCF / remoting interfaces or an AMQP broker
3. Remove code duplication, revise inheritance
4. Revise locking / synchronization mechanisms and queue management
5. Fix Singleton-itis
6. Allow plugins to target specific phases of processing
7. Revise caching mechanisms
8. Build a proper 'Url-Seen' filter
9. Integrate the operations of common plugins, like the DBUpdater and UrlSelection, in the server core, and allow plugins to redefine aspects of these operations
10. Revise logging mechanism
11. Use a full-blown HTML parser, like HtmlAgilityPack
12. Convert CrawlWave.Client to a windows service, integrate the functionality provided by CrawlWave.Scheduler in it
13. Create a simple launcher/update for the Client
14. Implement Plugin LifeCycle management (install, uninstall, activate, deactivate, etc)
15. Use generics on collections and other interfaces
16. Implement content extraction from other sources
