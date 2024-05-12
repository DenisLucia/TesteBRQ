using TesteBRQ.Console.ErrorHandling;
using TesteBRQ.Console.Model.DTO;
using TesteBRQ.Console.Model.Enums;

namespace TesteBRQ.Console.Model.Rules
{
	public class ClientSectorRule : IBaseRule
	{
        public ClientSector	Sector { get; private set; }

		private ClientSectorRule(ClientSector sector) 
		{
			Sector = sector;
		}

		private static Result<ClientSectorRule> Validate(string clientSectorStr, out ClientSector sector)
		{
			var result = new Result<ClientSectorRule>();
			if (string.IsNullOrEmpty(clientSectorStr))
				clientSectorStr = "Any";

			if (!Enum.TryParse<ClientSector>(clientSectorStr, out sector))
				result.ValidateFailure(Error.CreateError("ClientSectorRule.Validate", "Invalid client sector"));

			return result;
		}

		public static ClientSectorRule CreateRule(string strSector)
		{
			ClientSector sector = ClientSector.Any;
			var result = Validate(strSector, out sector);
			if (result.Failure)
				throw new Exception(result.Errors[0].ErrorMessage);

			return new ClientSectorRule(sector);
		}
			
		public EvaluationStatus Evaluate(TradeDTO trade)
		{
			if(trade.ClientSector.Equals(Sector.ToString(), StringComparison.OrdinalIgnoreCase))
				return EvaluationStatus.Adherent;

			return EvaluationStatus.NonAdherent;
		}
	}
}
