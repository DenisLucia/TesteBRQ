using Newtonsoft.Json;
using System.Configuration;
using System.Reflection;
using TesteBRQ.Console.ErrorHandling;
using TesteBRQ.Console.Model.DTO;
using TesteBRQ.Console.Model.Entities;
using TesteBRQ.Console.Model.Rules;

namespace TesteBRQ.Console.Service
{
	public class CategoryService
	{
		private const string RULES_NAMESPACE = "TesteBRQ.Console.Model.Rules.";
		
		/// <summary>
		/// Reads the categories.json file
		/// </summary>
		/// <returns>file content</returns>
		private Result<string> ReadCategoriesFile()
		{
			string json = string.Empty;
			StreamReader sr = StreamReader.Null;
			try
			{
				string filePath = ConfigurationManager.AppSettings["CategoriesFilePath"]!;
				sr = new StreamReader(filePath);
				json = sr.ReadToEnd();
				return new Result<string>().ValidateSuccess(json);
			}
			catch (Exception ex)
			{
				return new Result<string>().ValidateFailure(Error.CreateError("CategoryService.ReadCategoriesFile", ex));
			}
			finally
			{
				if (sr != null)
					sr.Close();
			}
		}

		/// <summary>
		/// Uses Reflection to retrieve and adjust the CreateRule method
		/// </summary>
		/// <param name="rule">Description of the validation rule</param>
		/// <param name="ruleType">Type of the class that implements the rule</param>
		/// <returns>Result containing the MethodInfo object corresponding to CreateRule method</returns>
		private Result<MethodInfo> GetCreateRuleMethod(RuleDescription rule, Type? ruleType)
		{
			var result = new Result<MethodInfo>();
			const string CONTEXT = "CategoryService.GetCreateRuleMethod";

			if (ruleType == null)
				return result.ValidateFailure(Error.CreateError(CONTEXT, 
					"Rule " + RULES_NAMESPACE + rule.Type + " not found! Review the categories configuration file"));

			MethodInfo? method = ruleType.GetMethod("CreateRule", BindingFlags.Public | BindingFlags.Static);
			if(method == null)
				return result.ValidateFailure(Error.CreateError(CONTEXT, 
					"Method not found! Review the categories configuration file"));

			if (method.GetParameters().Length != rule.Parameters.Length)
				return result.ValidateFailure(Error.CreateError(CONTEXT, 
					"Wrong number of parameters! Review the categories configuration file"));

			ParameterInfo[] parameterInfos = method.GetParameters();
			for (int i = 0; i < rule.Parameters.Length; i++)
			{
				rule.Parameters[i] = Convert.ChangeType(rule.Parameters[i], parameterInfos[i].ParameterType);
			}

			return result.ValidateSuccess(method);
		}

		/// <summary>
		/// Reads the categories.json file and loads its data into a List of CategoryDTO
		/// </summary>
		/// <returns>Result containing the CategoryDTO List</returns>
		private Result<List<CategoryDTO>> LoadCategories()
		{
			var jsonResult = ReadCategoriesFile();
			if (jsonResult.Failure)
				return new Result<List<CategoryDTO>>().ValidateFailure(jsonResult.Errors);

			var categoriesDTO = JsonConvert.DeserializeObject<List<CategoryDTO>>(jsonResult.Value!);
			if (categoriesDTO == null || categoriesDTO.Count == 0)
				return new Result<List<CategoryDTO>>().ValidateFailure(
					Error.CreateError("CategoryService.LoadCategories", "Error importing categories"));

			return new Result<List<CategoryDTO>>().ValidateSuccess(categoriesDTO);
		}

		/// <summary>
		/// Transforms the CategoryDTO list in a Category list and configures the rules for
		/// those categories to be used further for trade evaluation
		/// </summary>
		/// <param name="categoriesDto">List os categories retrieved from the categories.json file</param>
		/// <returns>Result containing the list of categories</returns>
		private Result<List<Category>> BuildCategories(List<CategoryDTO> categoriesDto)
		{
			var result = new Result<List<Category>>();

			List<Category> categories = new List<Category>();

			foreach (var catDto in categoriesDto)
			{
				List<IBaseRule> rules = [];
				foreach (var rule in catDto.Rules)
				{
					Type? ruleType = Type.GetType(RULES_NAMESPACE + rule.Type);
					var createRuleMethodResult = GetCreateRuleMethod(rule, ruleType);
					if(createRuleMethodResult.Failure)
						return result.ValidateFailure(createRuleMethodResult.Errors);

					try
					{
						MethodInfo createRuleMethod = createRuleMethodResult.Value!;
						object? ruleResult = createRuleMethod.Invoke(null, rule.Parameters);
						if(ruleResult is null)
							return result.ValidateFailure(Error.CreateError("CategoryService.BuildCategories", 
								new NullReferenceException("Method returned Null"))); 
						
						rules.Add((IBaseRule)ruleResult);
					}
					catch (Exception ex)
					{
						return result.ValidateFailure(Error.CreateError("CategoryService.BuildCategories", ex));
					}

				}

				if (rules.Count > 0) 
				{
					var categoryResult = Category.CreateCategory(catDto.Name, rules);
					if (categoryResult.Failure)
						return result.ValidateFailure(categoryResult.Errors);

					categories.Add(categoryResult.Value!);
				}
			}

			return result.ValidateSuccess(categories);
		}

		public Result<List<Category>> GetCategories() 
		{
			var result = new Result<List<Category>>();

			var resultDto = this.LoadCategories();
			if(resultDto.Failure)
				return result.ValidateFailure(resultDto.Errors);

			return this.BuildCategories(resultDto.Value!);
		}
	}
}
