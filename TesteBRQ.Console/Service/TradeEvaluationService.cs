using System.Configuration;
using TesteBRQ.Console.ErrorHandling;
using TesteBRQ.Console.ExtensionMethods;
using TesteBRQ.Console.Model.DTO;
using TesteBRQ.Console.Model.Entities;
using TesteBRQ.Console.Model.Rules;

namespace TesteBRQ.Console.Service
{
	public class TradeEvaluationService
	{
		private string tradesFilesFolder = string.Empty;
		private string tradesFilesErrorFolder = string.Empty;
		private string tradesFilesProcessedFolder = string.Empty;

		private List<Category> CategoriesToEvaluate = new();

        public TradeEvaluationService()
        {
			var result = LoadFolders();
			if(result.Failure)
			{
				string mensagem = string.Empty;
				foreach (Error error in result.Errors)
					mensagem += error.ErrorMessage + Environment.NewLine;

				throw new Exception(mensagem);
			}
		}

		/// <summary>
		/// Verify if the folders for the trades files do exist
		/// </summary>
		/// <returns>Result containing a bool value wich indicates that all the folders do exist </returns>
		private Result<bool> ValidateFolders()
		{
			const string CONTEXT = "TradeEvaluationService.ValidateFolders";
			var result = new Result<bool>();

			if (!Directory.Exists(tradesFilesFolder))
				result.ValidateFailure(Error.CreateError(CONTEXT, string.Format("Directory {0} does not exist", tradesFilesFolder)));

			if (!Directory.Exists(tradesFilesErrorFolder))
				result.ValidateFailure(Error.CreateError(CONTEXT, string.Format("Directory {0} does not exist", tradesFilesErrorFolder)));

			if (!Directory.Exists(tradesFilesProcessedFolder))
				result.ValidateFailure(Error.CreateError(CONTEXT, string.Format("Directory {0} does not exist", tradesFilesProcessedFolder)));

			return result;
		}

		/// <summary>
		/// Retrieves the folders path from the Application config file
		/// </summary>
		/// <returns>Result containing a bool value wich indicates that all the folders could be retrieved </returns>
		private Result<bool> LoadFolders()
		{
			const string CONTEXT = "TradeEvaluationService.LoadFolders";
			var result = new Result<bool>();

			try
			{
				tradesFilesFolder = ConfigurationManager.AppSettings["TradesFilesFolder"];
				tradesFilesErrorFolder = ConfigurationManager.AppSettings["TradesFilesErrorFolder"];
				tradesFilesProcessedFolder = ConfigurationManager.AppSettings["TradesFilesProcessedFolder"];
			}
			catch
			{
				return result.ValidateFailure(Error.CreateError(CONTEXT, "Error retrieving data from application config file"));
			}

			return ValidateFolders();
		}

		/// <summary>
		/// Get all the files in the trades files to be processed
		/// </summary>
		/// <returns>Result containing a list of string filled with the complete files paths</returns>
		private Result<List<string>> GetFilesToBeProcessed()
		{
			const string CONTEXT = "TradeEvaluationService.GetFilesToBeProcessed";
			var result = new Result<List<string>>();

			List<string> files = [];

			try
			{
				DirectoryInfo di = new DirectoryInfo(tradesFilesFolder);
				
				foreach(var file in di.GetFiles("*.txt", SearchOption.TopDirectoryOnly))
					files.Add(file.FullName);

				return result.ValidateSuccess(files);
			}
			catch(Exception ex)
			{
				return result.ValidateFailure(Error.CreateError(CONTEXT, ex));
			}
		}

		/// <summary>
		/// Verifies if the given file exists and is not empty
		/// </summary>
		/// <param name="fileName">file path to be validated</param>
		/// <returns>Result containing a bool that indicates the file exists and is not empty</returns>
		private Result<bool> ValidateFile(string fileName)
		{
			const string CONTEXT = "TradeEvaluationService.ValidateFile";
			var result = new Result<bool>();

			if (!File.Exists(fileName))
				return result.ValidateFailure(Error.CreateError(CONTEXT, string.Format("File {0} does not exist", fileName)));

			FileInfo info = new FileInfo(fileName);
			if (info.Length == 0)
			{
				return result.ValidateFailure(Error.CreateError(CONTEXT, string.Format("File {0} is empty", fileName)));
			}

			return result;
		}

		/// <summary>
		/// Reads the given file and validate its data.
		/// If all the file data is OK, then a list of transactions is filled
		/// </summary>
		/// <param name="fileName">file path</param>
		/// <param name="trades">out parameter - list of TradeDTO that is filled if all data is OK</param>
		/// <returns>
		///		Result containing a bool that indicates the file exists and is not empty and
		///		a list of TradeDTO is also returned via output parameter when validations are OK
		/// </returns>
		private Result<bool> ValidateFileFormat(string fileName, out List<TradeDTO> trades)
		{
			const string CONTEXT = "TradeEvaluationService.ValidateFileFormat";
			trades = new List<TradeDTO>();
			
			
			var result =  ValidateFile(fileName);
			if (result.Failure)
				return result;

			StreamReader sr = StreamReader.Null;

			try
			{
				string fileFormatError = "{0}:\n Invalid format - Expected: {1}";

				sr = new StreamReader(fileName);

				string line;
				int index = 0;
				int expectedTrades = 0;
				int tradesFound = 0;
				DateTime refDate = DateTime.MinValue;

				while ((line = sr.ReadLine()) != null)
				{
					if (index == 0)
					{
						if (!line.IsDate())
							return result.ValidateFailure(Error.CreateError(CONTEXT, string.Format(fileFormatError, fileName, "Reference Date")));
						else
							refDate = DateTime.Parse(line.Trim());
					}
					else if (index == 1)
					{
						if (!int.TryParse(line, out expectedTrades))
							return result.ValidateFailure(Error.CreateError(CONTEXT, string.Format(fileFormatError, fileName, "Number of trades")));
					}
					else
					{
						if (!TradeDTO.TryParse(line, out var trade))
						{
							trades.Clear();
							return result.ValidateFailure(Error.CreateError(CONTEXT, string.Format(fileFormatError, fileName, "Valid trade")));
						}
						else
						{
							trade.ReferenceDate = refDate;
							trades.Add(trade!);
						}
							

						tradesFound++;
					}

					index++;
				}

				if (tradesFound != expectedTrades)
					result.ValidateFailure(Error.CreateError(CONTEXT, string.Format(fileFormatError, "Number of trades found differs from expected")));



				return result;
			}
			catch (Exception ex)
			{
				trades.Clear();
				return result.ValidateFailure(Error.CreateError(CONTEXT, ex));
			}
			finally
			{
				if (sr != StreamReader.Null)
					sr.Close();
			}
		}

		/// <summary>
		/// Orchestrates the file validation and reading, retrieving the list of TradeDTO (when no errors found)
		/// and makes all the evaluation of the trades, complementing each Trade with its correct category.
		/// If no category fits a trade, then the text NOT_MAPPED is used to indicate the abscence of a scenario
		/// </summary>
		/// <param name="fileName">file path</param>
		/// <returns>Result containing a list of TradeDTO with categories filled</returns>
        private Result<List<TradeDTO>> ReadTradeFile(string fileName)
		{
			var result = new Result<List<TradeDTO>>();

			List<TradeDTO> tradesDto = new List<TradeDTO>();
			var validationResult = ValidateFileFormat(fileName, out tradesDto);

			if (validationResult.Failure)
			{
				File.Move(fileName, tradesFilesErrorFolder, true);
				return result.ValidateFailure(validationResult.Errors);
			}

			foreach(TradeDTO trade in tradesDto)
			{
				foreach(Category category in CategoriesToEvaluate) 
				{
					bool isThis = true;
					foreach(IBaseRule rule in category.Rules)
					{
						isThis = (rule.Evaluate(trade) == Model.Enums.EvaluationStatus.Adherent);
						if (!isThis) break;
					}

					if (isThis)
					{
						trade.CategoryName = category.Name;
						break;
					}
				}
			}


			File.Move(fileName, tradesFilesProcessedFolder, true);
			return result.ValidateSuccess(tradesDto);
		}

		/// <summary>
		/// Retrieves the categories list from CategoriesService
		/// </summary>
		/// <returns>Result containing a bool that indicates that the operation ended successfully</returns>
		private Result<bool> GetCategories()
		{
			var result = new Result<bool>();
			CategoryService categoryService = new CategoryService();
			var categoriesResult = categoryService.GetCategories();
			if (categoriesResult.Failure)
				result.ValidateFailure(categoriesResult.Errors);

			this.CategoriesToEvaluate = categoriesResult.Value!;
			return result;

		}

		public Result<List<SummaryDTO>> AnalyzeTrades()
		{
			var result = new Result<List<SummaryDTO>>();

			List<SummaryDTO> summaries = new List<SummaryDTO>();

			var categoriesResult = this.GetCategories();
			if (categoriesResult.Failure)
				result.ValidateFailure(categoriesResult.Errors);

			var processFilesResult = this.GetFilesToBeProcessed();
			if (processFilesResult.Failure)
				result.ValidateFailure(processFilesResult.Errors);

			List<string> processFiles = processFilesResult.Value!;

			processFiles.ForEach(processFile =>
			{
				var resultTrades = this.ReadTradeFile(processFile);
				if(resultTrades.Failure)
					summaries.Add(new SummaryDTO(processFile, resultTrades.Errors.ToList()));
				else
					summaries.Add(new SummaryDTO(processFile, resultTrades.Value!));
			});

			return result.ValidateSuccess(summaries);
		}


	}
}
