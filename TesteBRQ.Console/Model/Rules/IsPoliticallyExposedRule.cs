using TesteBRQ.Console.Model.DTO;
using TesteBRQ.Console.Model.Enums;

namespace TesteBRQ.Console.Model.Rules
{
	public class IsPoliticallyExposedRule : IBaseRule
	{
		public bool IsPoliticallyExposed { get; private set; }

		private IsPoliticallyExposedRule(bool isPoliticallyExposed) 
		{
			this.IsPoliticallyExposed = isPoliticallyExposed;
		}

		public static IsPoliticallyExposedRule CreateRule(bool isPoliticallyExposed) =>
			new(isPoliticallyExposed);

		public EvaluationStatus Evaluate(TradeDTO trade)
		{
			if(IsPoliticallyExposed == trade.IsPoliticallyExposed)
				return EvaluationStatus.Adherent;

			return EvaluationStatus.NonAdherent;
		}


	}
}
