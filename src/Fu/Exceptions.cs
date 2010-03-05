
using System;

namespace Fu.Exceptions
{
	/// <summary>
	/// Signify the walk to stop all further processing.
	/// </summary>
	public partial class StopWalkException : Exception
	{
		public StopWalkException() :
			base("Signify the walk to stop all further processing.") { }
		public StopWalkException(string msg) :
			base(msg) { }
		public StopWalkException(string msg, Exception innerException) :
			base(msg, innerException) { }
	}
	
	/// <summary>
	/// Skip currently processing step and move to next step immediately
	/// </summary>
	public partial class SkipStepException : Exception
	{
		public SkipStepException() :
			base("Skip currently processing step and move to next step immediately") { }
		public SkipStepException(string msg) :
			base(msg) { }
		public SkipStepException(string msg, Exception innerException) :
			base(msg, innerException) { }
	}
	
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