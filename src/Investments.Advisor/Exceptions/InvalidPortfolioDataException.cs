using System;

namespace Investments.Advisor.Exceptions
{
	public class InvalidPortfolioDataException : Exception
	{
		public InvalidPortfolioDataException()
		{
		}

		public InvalidPortfolioDataException(string? message) : base(message)
		{
		}

		public InvalidPortfolioDataException(string? message, Exception? innerException) : base(message, innerException)
		{
		}
	}
}
