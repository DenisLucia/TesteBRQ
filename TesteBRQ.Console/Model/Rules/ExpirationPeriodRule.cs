using TesteBRQ.Console.ErrorHandling;
using TesteBRQ.Console.Model.DTO;
using TesteBRQ.Console.Model.Enums;

namespace TesteBRQ.Console.Model.Rules
{
	public class ExpirationPeriodRule: IBaseRule
	{
        public int MinExpirationInDays { get; private set; } = 0;
        public int MaxExpirationInDays { get; private set; } = 0;

		private ExpirationPeriodRule(int min, int max)
        {
            MinExpirationInDays = min;
            MaxExpirationInDays = max;
        }

        private static Result<ExpirationPeriodRule> ValidateExpirationPeriod(int min, int max)
        {
            const string CONTEXT = "ExpirationPeriod.Validation";
            var result = new Result<ExpirationPeriodRule>();

            if(min == 0 && max == 0)
				result.ValidateFailure(Error.CreateError(CONTEXT,
					"Invalid Expiration Period"));

			if (min < 0 || max < 0)
                result.ValidateFailure(Error.CreateError(CONTEXT, 
                    "Both Min exp. and Max exp. must be greater than zero"));

            if (min > max && max != 0)
                result.ValidateFailure(Error.CreateError(CONTEXT, 
                    "Min expiration period must be lower than Max expiration period"));

            return result;
		}

        public static ExpirationPeriodRule CreateRule(int min, int max)
        {
            var result = ValidateExpirationPeriod(min, max);
            if (result.Failure)
                throw new Exception(result.Errors[0].ErrorMessage);

            return new ExpirationPeriodRule(min, max);
        }

		public EvaluationStatus Evaluate(TradeDTO trade)
		{
			if (trade.NextPaymentDate >= trade.ReferenceDate)
				return EvaluationStatus.NonAdherent;


			var days = (trade.ReferenceDate - trade.NextPaymentDate).Days;

			if ((days <= MaxExpirationInDays || MaxExpirationInDays == 0) &&
			    (days >= MinExpirationInDays))
				return EvaluationStatus.Adherent;


			return EvaluationStatus.NonAdherent;
		}
	}
}
