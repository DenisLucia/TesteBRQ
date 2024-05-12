using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteBRQ.Console.ErrorHandling
{
	public class Result<T>
	{
		private readonly List<Error> _errors = new();

		public bool Success { get; private set; }
        public bool Failure => !Success;
        public T? Value { get; private set; }
        public IReadOnlyList<Error> Errors => _errors;

		private Result(bool success, T? value = default(T))
		{
			this.Success = success;
			this.Value = (success ? value : default(T));
		}

		private Result(Error error) : this(false)
		{
			_errors.Add(error);
		}

		private Result(IEnumerable<Error> errors) : this(false)
		{
			_errors.AddRange(errors);
		}


		public Result() : this(true) { }

		public Result<T> ValidateSuccess(T? value = default(T)) => 
			new(true, value);

		public Result<T> ValidateFailure(Error error) =>
			new(error);

		public Result<T> ValidateFailure(IEnumerable<Error> error) =>
			new(error);

	}
}
