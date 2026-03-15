using System;

namespace Investments.Advisor.Exceptions
{
	public class InvalidBvbDataException : Exception
	{
		public InvalidBvbDataException()
		{
		}

		public InvalidBvbDataException(string message) : base(message)
		{
		}

		public InvalidBvbDataException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
