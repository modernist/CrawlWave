using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CrawlWave.Common;
using System.Data;

namespace CrawlWave.ServerCommon
{
	public interface ICrawlWaveServer
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool IsAlive();

		/// <summary>
		/// Updates the computer hardware info related to a client.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client.</param>
		/// <param name="info">The <see cref="CWComputerInfo"/> of the client computer.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException GetClientComputerInfo(ClientInfo ci, CWComputerInfo info);
		
		/// <summary>
		/// Stores the results that the clients return after crawling a set of Urls.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client returning the data.</param>
		/// <param name="data">An array of <see cref="UrlCrawlData"/> objects containing the data of the crawled urls.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		/// <remarks>
		/// This method does not actually update the database with the data of the crawled
		/// urls, since that is a very time-consuming process and the timeouts that occur
		/// would slow down the clients. Also this might lead to 'losing' urls because the
		/// database server will have to deal with great load if a big number of clients is
		/// returning data all the time. To avoid these problems the server simply updates
		/// the database with the fresh robots.txt files fetched by the clients and stores
		/// the data in a compressed zip file on disk. This allows their processing and the
		/// update of the database with the data of the urls and the links they contain to
		/// be performed at a later time by another process (the DBUpdater plugin).
		/// </remarks>
		SerializedException GetCrawlResults(ClientInfo ci, UrlCrawlData[] data);
		
		/// <summary>
		/// Performs the registration of a new client for a registered user of the system.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> containing the information of the
		/// client that wishes to be registered to the system.</param>
		/// <param name="info">The <see cref="CWComputerInfo"/> containing information about
		/// the hardware of the computer running the client.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		/// <remarks>
		/// The ClientInfo argument is passed by reference. After the registration process
		/// is complete the argument will be updated with the ID that has been assigned to
		/// the new client.
		/// </remarks>
		SerializedException RegisterClient(ref ClientInfo ci, CWComputerInfo info);
		
		/// <summary>
		/// Performs the registration of a new user to the system.
		/// </summary>
		/// <param name="ID">The ID that will be assigned to the new user, passed by reference.</param>
		/// <param name="username">The username requested from the new user.</param>
		/// <param name="password">The hash of the new user's password.</param>
		/// <param name="email">The user's email address.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException RegisterUser(ref int ID, string username, byte[] password, string email);
		
		/// <summary>
		/// Selects and returns a list of all the banned hosts.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client performing the call.</param>
		/// <param name="data">A <see cref="DataSet"/> that will contain the list of banned hosts.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException SendBannedHosts(ClientInfo ci, out DataSet data);

		/// <summary>
		/// Selects and returns the latest version of the client updates available.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client performing the call.</param>
		/// <param name="version">The latest version update available.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException SendLatestVersion(ClientInfo ci, out string version);

		/// <summary>
		/// Selects and returns a list of all the servers available.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client performing the call.</param>
		/// <param name="data">A <see cref="DataSet"/> that will contain the list of servers.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException SendServers(ClientInfo ci, out DataSet data);

		/// <summary>
		/// Selects and returns the requested client update version.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client performing the call.</param>
		/// <param name="version">The requested version.</param>
		/// <param name="data">A <see cref="DataSet"/> that will contain the list of servers.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException SendUpdatedVersion(ClientInfo ci, string version, out byte[] data);
		
		/// <summary>
		/// Selects and returns a set of urls that are ready to be crawled.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client requesting urls to crawl.</param>
		/// <param name="data">An array of <see cref="InternetUrlToCrawl"/> objects containing the selected urls.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException SendUrlsToCrawl(ClientInfo ci, out InternetUrlToCrawl[] data);

		/// <summary>
		/// Selects and returns the statistics for a certain user.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client requesting the statistics.</param>
		/// <param name="stats">The <see cref="UserStatistics"/> of the user.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		SerializedException SendUserStatistics(ClientInfo ci, out UserStatistics stats);
	}
}
