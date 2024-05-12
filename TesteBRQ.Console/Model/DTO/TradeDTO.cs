using System.Globalization;
using TesteBRQ.Console.Model.Interfaces;

namespace TesteBRQ.Console.Model.DTO
{
	public class TradeDTO : ITrade
	{
		private double _value;
		private string _clientSector;
		private DateTime _nextPaymentDate = DateTime.MinValue;
		private bool _isPoliticallyExposed = false;

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

		public bool IsPoliticallyExposed
		{
			get => _isPoliticallyExposed;
		}


		public TradeDTO(double tradeValue, string clientSector, DateTime nextPaymentDate, bool? isPoliticallyExposed = false) 
		{ 
			_value = tradeValue;
			_clientSector = clientSector;
			_nextPaymentDate = nextPaymentDate;
			_isPoliticallyExposed = (isPoliticallyExposed.HasValue) ? isPoliticallyExposed.Value : false;
		}

		public void SetCategoryName(string categoryName) => _categoryName = categoryName;
		public string GetCategoryName() => _categoryName;

		public void SetReferenceDate(DateTime referenceDate) => _referenceDate = referenceDate;
		public DateTime GetReferenceDate() => _referenceDate;

		public static bool TryParse(string value, out TradeDTO? result)
		{
			char separator = ' ';
			int expectedSize = 4;
			int expectedSizeLegacy = 3; //created to maintain compatibility to trades in files that do not expose the new property

			double tradeValue = 0;
			string clientSector = string.Empty;
			DateTime nextPaymentDate = DateTime.MinValue;
			bool isPoliticallyExposed = false;

			result = null;
			string[] allowedSectors = { "Private", "Public" };

			if (string.IsNullOrEmpty(value)) 
				return false;

			var valueArray = value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			
			if(valueArray.Length != expectedSize && valueArray.LongLength != expectedSizeLegacy)
				return false;

			if (!double.TryParse(valueArray[0], out tradeValue))
				return false;

			clientSector = valueArray[1];
			if (!allowedSectors.Contains(clientSector, StringComparer.OrdinalIgnoreCase))
				return false;

			if (!DateTime.TryParse(valueArray[2], CultureInfo.InvariantCulture ,out nextPaymentDate))
				return false;

			if (valueArray.Length == expectedSize)
				if (!Boolean.TryParse(valueArray[3].ToLower(), out isPoliticallyExposed))
					return false;

			result = new TradeDTO(tradeValue, clientSector, nextPaymentDate, isPoliticallyExposed);
			return true;
		}
	}
}
