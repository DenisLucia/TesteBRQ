using Newtonsoft.Json;

namespace TesteBRQ.Console.Model.DTO
{
	public sealed class CategoryDTO
	{
		public string Name { get; set; } = string.Empty;

		[JsonProperty("rules")]
		public List<RuleDescription> Rules { get; set; }
	}
}
