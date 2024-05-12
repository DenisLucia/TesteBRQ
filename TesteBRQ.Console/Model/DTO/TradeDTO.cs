using System.Globalization;
using TesteBRQ.Console.Model.Interfaces;

namespace TesteBRQ.Console.Model.DTO
{
	public class TradeDTO : ITrade
	{
		private double _value;
		private string _clientSector;
		private DateTime _nextPaymentDate = DateTime.MinValue;
		
		private string _categoryName = "NOT_MAPPED";
		private DateTime _referenceDate = DateTime.MinValue;

		public double Value
		{
			get => _value;
		}

		public string ClientSector
		{ 
			get => _clientSector;
		}
		

		public DateTime NextPaymentDate 
		{
			get => _nextPaymentDate;
		}


		public TradeDTO(double tradeValue, string clientSector, DateTime nextPaymentDate) 
		{ 
			_value = tradeValue;
			_clientSector = clientSector;
			_nextPaymentDate = nextPaymentDate;
		}

		public void SetCategoryName(string categoryName) => _categoryName = categoryName;
		public string GetCategoryName() => _categoryName;

		public void SetReferenceDate(DateTime referenceDate) => _referenceDate = referenceDate;
		public DateTime GetReferenceDate() => _referenceDate;

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
