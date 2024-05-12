using TesteBRQ.Console.ErrorHandling;
using TesteBRQ.Console.ExtensionMethods;
using TesteBRQ.Console.Model.Rules;

namespace TesteBRQ.Console.Model.Entities
{
	public sealed class Category
    {
		private readonly List<IBaseRule> _rules = new();

		public Guid CategoryID { get; private set; }
        public string Name { get; private set; } = string.Empty;

        public IReadOnlyList<IBaseRule> Rules => _rules;


        private Category(string name, List<IBaseRule> rules)
        {
            CategoryID = Guid.NewGuid();
            Name = name;
            _rules.AddRange(rules);
        }

        private static Result<Category> ValidateCategory(string name, List<IBaseRule> rules)
        {
            const string CONTEXT = "Category.Validation";
            var result = new Result<Category>();

			if (string.IsNullOrEmpty(name))
                result.ValidateFailure(Error.CreateError(CONTEXT,
                    "The category name must be informed"));
            else if (name.Length.IsBetween(5, 50))
                result.ValidateFailure(Error.CreateError(CONTEXT,
                    "The category name length must be between 5 and 50 characters"));

            if(rules is null || rules.Count == 0)
				result.ValidateFailure(Error.CreateError(CONTEXT,
					"Cannot create category with no validation rules"));

			return result;
        }

        public static Result<Category> CreateCategory(string name, List<IBaseRule> rules)
        {
			var result = ValidateCategory(name, rules);

            if (result.Failure)
                return result;

            var newCategory = new Category(name, rules);

            return result.ValidateSuccess(newCategory);
        }

        public Result<Category>AddRule(IBaseRule rule) 
        {
            _rules.Add(rule);
            return new Result<Category>().ValidateSuccess(this);
        }

		public Result<Category> AddRules(List<IBaseRule> rules)
		{
			_rules.AddRange(rules);
			return new Result<Category>().ValidateSuccess(this);
		}
	}
}
