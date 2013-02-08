using System;
using CrawlWave.Common;

namespace CrawlWave.Client.Common
{
	/// <summary>
	/// CrawlerStatus is an enumeration that defines all the different states in which the
	/// CrawlWave.Client.Crawler can be at a given time.
	/// </summary>
	public enum CrawlerState
	{
		/// <summary>
		/// Indicates that the crawler is inactive
		/// </summary>
		Stopped,
		/// <summary>
		/// Indicates that the crawler is active but currently not visiting pages
		/// </summary>
		Paused,
		/// <summary>
		/// Indicated that the crawler is active and visiting pages
		/// </summary>
		Running
	}
}
