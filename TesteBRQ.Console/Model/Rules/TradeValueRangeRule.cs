using System;
using TesteBRQ.Console.ErrorHandling;
using TesteBRQ.Console.ExtensionMethods;
using TesteBRQ.Console.Model.DTO;
using TesteBRQ.Console.Model.Entities;
using TesteBRQ.Console.Model.Enums;

namespace TesteBRQ.Console.Model.Rules
{
	public class TradeValueRangeRule: IBaseRule
	{
		public double MinTradeValue { get; private set; } = 0;
		public double MaxTradeValue { get; private set; } = 0;

        private TradeValueRangeRule(double minTradeValue, double maxTradeValue)
        {
            MinTradeValue = minTradeValue;
			MaxTradeValue = maxTradeValue;
        }

        private static Result<TradeValueRangeRule> ValidateTradeValues(double minTradeValue, double maxTradeValue) 
		{
			const string CONTEXT = "TradeValueRange.Validation";
			var result = new Result<TradeValueRangeRule>();

			if(minTradeValue == 0 &&  maxTradeValue == 0)
				result.ValidateFailure(Error.CreateError(CONTEXT,
					"Invalid Range. Either the Minimum or the Maximum Trade Value must be greater than zero"));

			if (minTradeValue < 0)
				result.ValidateFailure(Error.CreateError(CONTEXT,
					"The minimum trade value must be equal or higher than zero"));

			if (maxTradeValue < 0)
				result.ValidateFailure(Error.CreateError(CONTEXT,
					"The maximum trade value must be equal or higher than zero"));

			if (minTradeValue > maxTradeValue)
				result.ValidateFailure(Error.CreateError(CONTEXT,
					"The maximum trade must higher than the minimum value"));

			return result;
		}

		public static TradeValueRangeRule CreateRule(double minTradeValue, double maxTradeValue)
		{
			var result = ValidateTradeValues(minTradeValue, maxTradeValue);
			if (result.Failure)
				throw new Exception(result.Errors[0].ErrorMessage);

			return new TradeValueRangeRule(minTradeValue, maxTradeValue);
		}

		public EvaluationStatus Evaluate(TradeDTO trade)
		{
			if (MaxTradeValue == 0)
			{
				if (trade.Value >= MinTradeValue)
					return EvaluationStatus.Adherent;
			}
			else
			{
				if (trade.Value.IsBetween(MinTradeValue, MaxTradeValue))
					return EvaluationStatus.Adherent;
			}

			return EvaluationStatus.NonAdherent;

		}
	}
}
