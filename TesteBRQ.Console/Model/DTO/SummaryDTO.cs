using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteBRQ.Console.ErrorHandling;


namespace TesteBRQ.Console.Model.DTO
{
	public class SummaryDTO
	{
        public string FileName { get; private set; }
		public List<TradeDTO> Trades { get; private set; } = new();
		public List<Error> Errors { get; private set; } = new();

        public SummaryDTO(string fileName, List<TradeDTO> trades, List<Error> errors) 
        {
            this.FileName = fileName;
            this.Trades = trades;
			this.Errors = errors;
        }

		public SummaryDTO(string fileName, List<TradeDTO> trades)
		{
			this.FileName = fileName;
			this.Trades = trades;
		}

		public SummaryDTO(string fileName, List<Error> errors)
		{
			this.FileName = fileName;
			this.Errors = errors;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(FileName);
			sb.AppendLine("--------------------------------------");

			foreach (var trade in this.Trades)
				sb.AppendLine(trade.CategoryName);

			if (Errors.Count > 0)
			{
				sb.AppendLine("\nERRORS\n");
				foreach (var error in this.Errors)
					sb.AppendLine(error.ErrorMessage);
			}

			sb.AppendLine("--------------------------------------");

			return sb.ToString();
		}

	}
}
