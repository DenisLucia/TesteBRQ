using TesteBRQ.Console.Model.DTO;
using TesteBRQ.Console.Model.Entities;
using TesteBRQ.Console.Model.Enums;

namespace TesteBRQ.Console.Model.Rules
{
	public interface IBaseRule
	{

		public EvaluationStatus Evaluate(TradeDTO trade);
    }
}
