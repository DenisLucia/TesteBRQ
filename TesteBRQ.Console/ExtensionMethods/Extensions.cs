namespace TesteBRQ.Console.ExtensionMethods
{
	public static class Extensions
	{
		public static string Capitalize(this string str)
		{
			if(string.IsNullOrEmpty(str)) 
				return string.Empty;

			str = str.ToLower();
			
			return str.Substring(0, 1).ToUpper() + str.Substring(1);
			
		}

		public static bool IsBetween<T>(this T x, T min, T max) where T : IComparable<T>
		{
			if(min.CompareTo(max) > 0) 
				return false;

			return x.CompareTo(min) >= 0 && x.CompareTo(max) <= 0;
		}

		public static bool IsDate(this string value)
		{
			if(string.IsNullOrEmpty(value)) 
				return false;

			return DateTime.TryParse(value, out var date);
		}
	}
}
