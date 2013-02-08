using System;
using System.Text.RegularExpressions;

namespace CrawlWave.Common
{
	/// <summary>
	/// Defines an interface for a generic Validator
	/// </summary>
	public interface IValidator
	{
		/// <summary>
		/// Performs Validation of an input string.
		/// </summary>
		/// <param name="input">The input string to validate.</param>
		/// <returns>True if the input is valid, otherwise false.</returns>
		bool Validate(string input);
	}

	/// <summary>
	/// EmailValidator is a Singleton class that implements <see cref="IValidator"/>. It can
	/// provide email address validation according to the RFC822 specification.
	/// </summary>
	public class EmailValidator: IValidator
	{
		#region Private members

		private static EmailValidator instance;
		private Regex regex;
		
		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private EmailValidator()
		{
			string strRegex = @"^([a-zA-Z0-9_\-])+(\.([a-zA-Z0-9_\-])+)*@((\[(((([0-1])?([0-9])?[0-9])|(2[0-4][0-9])|(2[0-5][0-5])))\.(((([0-1])?([0-9])?[0-9])|(2[0-4][0-9])|(2[0-5][0-5])))\.(((([0-1])?([0-9])?[0-9])|(2[0-4][0-9])|(2[0-5][0-5])))\.(((([0-1])?([0-9])?[0-9])|(2[0-4][0-9])|(2[0-5][0-5]))\]))|((([a-zA-Z0-9])+(([\-])+([a-zA-Z0-9])+)*\.)+([a-zA-Z])+(([\-])+([a-zA-Z0-9])+)*))$";
			// perhaps use this one "^([^\[\]()<>@,;:\\".]+|\"([^"\\\r]|\\.)*\")(\.([^\[\]()<>@,;:\\".]+|\"([^"\\\r]|\\.)*\"))*@(\[([^\\\[\]\r]|\\.)*\]|[^\[\]()<>@,;:\\".]+)(\.(\[([^\\\[\]\r]|\\.)*\]|[^\[\]()<>@,;:\\".]+))*$";
			//@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
			regex = new Regex(strRegex);
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="EmailValidator"/> class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="EmailValidator"/>.</returns>
		public static EmailValidator Instance()
		{
			if(instance == null)
			{
				instance = new EmailValidator();
			}
			return instance;
		}

		#endregion

		#region IValidator members

		/// <summary>
		/// Validates an email address according to the RFC822 specification.
		/// </summary>
		/// <param name="input">The email address to validate</param>
		/// <returns>True if input is a valid email address, false otherwise</returns>
		public bool Validate(string input)
		{
			try
			{
				if (regex.IsMatch(input))
					return (true);
				else
					return (false);
			}
			catch
			{
				return false;
			}
		}

		#endregion
	}

	/// <summary>
	/// PhoneValidator is a Singleton class that implements <see cref="IValidator"/>. It can
	/// provide telephone number validation for Greece.
	/// </summary>
	/// <remarks>
	/// The PhoneValidator accepts numbers in the following formats as valid:
	/// <list type="bullet">
	/// <item>2101234567</item>
	/// <item>210 1234567</item>
	/// <item>210-1234567</item>
	/// <item>210 - 1234567</item>
	/// <item>2120123456</item>
	/// <item>2120 123456</item>
	/// <item>2120-123456</item>
	/// <item>2120 - 123456</item>
	/// <item>2123012345</item>
	/// <item>21230 12345</item>
	/// <item>21230-12345</item>
	/// <item>21230 - 12345</item>
	/// <item>69x1234567</item>
	/// <item>69x 1234567</item>
	/// <item>69x-1234567</item>
	/// <item>69x - 1234567</item>
	/// <item>+30|0030... (one of the above)</item>
	/// <item>(+30)|(0030)... (one of the above)</item>
	/// </list>
	/// </remarks>
	public class PhoneValidator : IValidator
	{
		#region Private members

		private static PhoneValidator instance;
		private Regex regex;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private PhoneValidator()
		{
			regex = new Regex(@"^((\(\+30\))|(\+30)|(\(?0030\)?))?((\s?)|(\s?-\s?))[2|6](([0-9]{1}0((\s?)|(\s?-\s?))[0-9]{7})|([0-9]{2}0((\s?)|(\s?-\s?))[0-9]{6})|([0-9]{3}0((\s?)|(\s?-\s?))[0-9]{5})|(9[3|4|6|7|8|9]((\s?)|(\s?-\s?))[0-9]{7}))$");
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="PhoneValidator"/> class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="PhoneValidator"/>.</returns>
		public static PhoneValidator Instance()
		{
			if(instance == null)
			{
				instance = new PhoneValidator();
			}
			return instance;
		}

		#endregion

		#region IValidator Members

		/// <summary>
		/// Validates a greek telephone number.
		/// </summary>
		/// <param name="input">The telephone number to validate.</param>
		/// <returns>True if input is a valid telephone number, false otherwise.</returns>
		public bool Validate(string input)
		{
			try
			{
				if (regex.IsMatch(input))
					return (true);
				else
					return (false);
			}
			catch
			{
				return false;
			}
		}

		#endregion
	}

	/// <summary>
	/// UrlValidator is a Singleton class that implements <see cref="IValidator"/>. It can
	/// validate Uniform Resource Locators, according to RFC 1738
	/// </summary>
	public class UrlValidator : IValidator
	{
		#region Private members

		private static UrlValidator instance;
		private Uri uri;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private UrlValidator()
		{
			uri = null;
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="PhoneValidator"/> class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="PhoneValidator"/>.</returns>
		public static UrlValidator Instance()
		{
			if(instance == null)
			{
				instance = new UrlValidator();
			}
			return instance;
		}

		#endregion		
		
		#region IValidator Members

		/// <summary>
		/// Vaidates a Uniform Resource Locators (URLs)
		/// </summary>
		/// <param name="input">A string containing the Url to validate.</param>
		/// <returns>True if the input is a valid Url, false otherwise.</returns>
		public bool Validate(string input)
		{
			try
			{
				uri = new Uri(input);
				uri = null;
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion
	}
}
