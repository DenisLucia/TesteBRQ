using System.Globalization;

namespace TesteBRQ.Console.Model.DTO
{
	public class TradeDTO
	{
		public double Value { get; set; }
		public string ClientSector { get; set; }
		public DateTime NextPaymentDate { get; set; }
		public string CategoryName { get; set; } = "NOT_MAPPED";
		public DateTime ReferenceDate { get; set; }

		public TradeDTO(double tradeValue, string clientSector, DateTime nextPaymentDate) 
		{ 
			Value = tradeValue;
			ClientSector = clientSector;
			NextPaymentDate = nextPaymentDate;
		}

		public static bool TryParse(string value, out TradeDTO? result)
		{
			char separator = ' ';
			int expectedSize = 3;

			double tradeValue = 0;
			string clientSector = string.Empty;
			DateTime nextPaymentDate = DateTime.MinValue;

			result = null;
			string[] allowedSectors = { "Private", "Public" };

			if (string.IsNullOrEmpty(value)) 
				return false;

			var valueArray = value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			
			if(valueArray.Length != expectedSize )
				return false;

			if (!double.TryParse(valueArray[0], out tradeValue))
				return false;

			clientSector = valueArray[1];
			if (!allowedSectors.Contains(clientSector, StringComparer.OrdinalIgnoreCase))
				return false;

			if (!DateTime.TryParse(valueArray[2], CultureInfo.InvariantCulture ,out nextPaymentDate))
				return false;
			
			result = new TradeDTO(tradeValue, clientSector, nextPaymentDate);
			return true;
		}
	}
}
