using System;
using System.Collections.Generic;
using CrawlWave.Common;

namespace CrawlWave.Client.Common
{
	/// <summary>
	/// ICrawlerController defines the interface for a Remotable object that will be used
	/// to control the CrawlWave Client's Crawler through Remoting.
	/// </summary>
	public interface ICrawlerController
	{
		/// <summary>
		/// Gets the Crawler's <see cref="CrawlerState"/>.
		/// </summary>
		CrawlerState GetState();

		/// <summary>
		/// Gets a <see cref="ClientSettings"/> object with the CrawLWave Client's settings.
		/// </summary>
		ClientSettings GetSettings();

		/// <summary>
		/// Gets a queue of the last <see cref="EventLoggerEntry"/> objects logged by the Crawler.
		/// </summary>
		Queue<EventLoggerEntry> GetEventQueue();

		/// <summary>
		/// Gets the amount of memory consumed by the Crawler in KB.
		/// </summary>
		int GetMemoryUsage();
		
		/// <summary>
		/// Gets the crawler's statistics (Urls Visited, Total Bytes etc.).
		/// </summary>
		long [] GetStatistics();

		/// <summary>
		/// Attempts to retrieve the user's statistics from the server.
		/// </summary>
		/// <param name="stats">The statistics of the user.</param>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException GetUserStatistics(ref UserStatistics stats);
		
		/// <summary>
		/// Sets the CrawlWave Client's <see cref="ClientSettings"/>.
		/// </summary>
		/// <param name="settings"></param>
		void SetSettings(ClientSettings settings);

		/// <summary>
		/// Attempts to perform the registration of a new user.
		/// </summary>
		/// <param name="UserName">The user's username.</param>
		/// <param name="Password">The user's password.</param>
		/// <param name="Email">The user's email address.</param>
		/// <returns></returns>
		SerializedException RegisterUser(string UserName, string Password, string Email);

		/// <summary>
		/// Attempts to Start the crawling process.
		/// </summary>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException Start();

		/// <summary>
		/// Attempts to Stop the crawling process.
		/// </summary>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException Stop();

		/// <summary>
		/// Attempts to Pause the crawling process.
		/// </summary>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException Pause();

		/// <summary>
		/// Attempts to Resume the crawling process.
		/// </summary>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException Resume();

		/// <summary>
		/// Attempts to terminate the crawler process.
		/// </summary>
		/// <param name="sx">Returns Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</param>
		void Terminate(ref SerializedException sx);

	}
}
