using System.Collections.Generic;

namespace BodyCoreProduct.Models
{
	// Объект этого класса используется в разметке для отображения графиков
	public class Distribution
	{
		/// <summary>
		/// Название распределения
		/// </summary>
		public string DistributionName { get; set; }

		/// <summary>
		/// Кол-во размерностей, равно количеству типов телосложения (переменная BODY_TYPES_CNT в классе BaseResults.cs)
		/// </summary>
		public List<Measurements> Measurements { get; set; }
	}
}
