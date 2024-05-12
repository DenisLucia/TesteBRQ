using Newtonsoft.Json;

namespace TesteBRQ.Console.Model.DTO
{
	public class RuleDescription
	{
		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("parameters")]
		public object[] Parameters { get; set; }
	}
}
