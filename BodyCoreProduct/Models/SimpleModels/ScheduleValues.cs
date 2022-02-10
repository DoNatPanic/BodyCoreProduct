using System.Collections.Generic;

namespace BodyCoreProduct.Models
{
	public class ScheduleValues
	{
		/// <summary>
		/// Расчетный период
		/// </summary>
		public List<int> WeeksValues { get; set; }

		/// <summary>
		/// Дата начала расчетного периода
		/// </summary>
		public List<string> Date { get; set; }

		/// <summary>
		/// Масса тела на момент расчетного периода
		/// </summary>
		public List<float> WeightValues { get; set; }

		/// <summary>
		/// % жировой ткани в организме на момент расчетного периода
		/// </summary>
		public List<float> FatPercentValues { get; set; }

		/// <summary>
		/// Суточная норма килокалорий
		/// </summary>
		public List<int> KkalValues { get; set; }

		/// <summary>
		/// Суточная норма белков (гр.)
		/// </summary>
		public List<int> ProteinValues { get; set; }

		/// <summary>
		/// Суточная норма жиров (гр.)
		/// </summary>
		public List<int> FatValues { get; set; }

		/// <summary>
		/// Суточная норма углеводов (гр.)
		/// </summary>
		public List<int> CarbongydrateValues { get; set; }

		public ScheduleValues()
		{
			WeeksValues = new List<int>();
			Date = new List<string>();
			WeightValues = new List<float>();
			FatPercentValues = new List<float>();
			KkalValues = new List<int>();
			ProteinValues = new List<int>();
			FatValues = new List<int>();
			CarbongydrateValues = new List<int>();
		}
	}
}
