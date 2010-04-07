
using System;

namespace Fu.Exceptions
{
	/// <summary>
	/// Error occured while executing a step.
	/// </summary>
	public partial class StepExecutionException : Exception
	{
		public StepExecutionException() :
			base("Error occured while executing a step.") { }
		public StepExecutionException(string msg) :
			base(msg) { }
		public StepExecutionException(string msg, Exception innerException) :
			base(msg, innerException) { }
	}
	
	/// <summary>
	/// Unable to parse the request data.
	/// </summary>
	public partial class BadRequestDataException : Exception
	{
		public BadRequestDataException() :
			base("Unable to parse the request data.") { }
		public BadRequestDataException(string msg) :
			base(msg) { }
		public BadRequestDataException(string msg, Exception innerException) :
			base(msg, innerException) { }
	}
	
}