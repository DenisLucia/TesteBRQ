namespace TesteBRQ.Console.ErrorHandling
{
	public record struct Error
	{
        public string? Context { get; init; }
        public string? ErrorMessage { get; init; }
        public Exception ExceptionData { get; init; }

		private Error(string context, string errorMessage) 
        { 
            this.Context = context; 
            this.ErrorMessage = errorMessage; 
        }

        private Error(string context, Exception ex)
        {
            this.Context = context;
            this.ExceptionData = ex;
        }

        public static Error CreateError(string context, string errorMessage) =>
            new Error(context, errorMessage);

        public static Error CreateError(string context, Exception ex) =>
            new Error(context, ex);


    }
}
