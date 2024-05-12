using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteBRQ.Console.Model.Enums;

namespace TesteBRQ.Console.Model.DTO
{
	public sealed class CategoryDTO
	{
		public string Name { get; set; } = string.Empty;

		[JsonProperty("rules")]
		public List<RuleDescription> Rules { get; set; }
	}
}
