namespace TesteBRQ.Console.Model.Interfaces
{
	public interface ITrade
	{
		double Value { get; }
		string ClientSector { get; }
		DateTime NextPaymentDate { get; }
		bool IsPoliticallyExposed { get; }

	}
}
