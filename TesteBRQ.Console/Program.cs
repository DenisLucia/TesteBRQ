using TesteBRQ.Console.ErrorHandling;
using TesteBRQ.Console.Model.DTO;
using TesteBRQ.Console.Service;

public class Program
{
	private static void PrintErrorsAndQuit(IEnumerable<Error> errors, int exitCode)
	{
		foreach (var error in errors)
			Console.WriteLine(error.ErrorMessage);

		Console.ReadKey();
		Console.WriteLine("\n\nPress <ENTER> to quit...");
		Environment.Exit(exitCode);
	}

	public static void Main(string[] args)
	{
		TradeEvaluationService service = new TradeEvaluationService();
		var result = service.AnalyzeTrades();
		if (result.Failure)
			PrintErrorsAndQuit(result.Errors, 1);

		List<SummaryDTO> summaries = result.Value!;
		summaries.ForEach(summary => Console.WriteLine(summary.ToString()));

	}
}

