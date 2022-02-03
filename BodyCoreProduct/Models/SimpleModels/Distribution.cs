using System.Collections.Generic;

namespace BodyCoreProduct.Models
{
	public class Distribution
	{
		public string DistrName { get; set; }
		public List<SimpleReport> Measurements { get; set; }
	}
}
