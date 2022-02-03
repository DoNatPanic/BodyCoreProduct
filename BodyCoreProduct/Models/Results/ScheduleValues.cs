using System.Collections.Generic;

namespace BodyCoreProduct.Models
{
	public class ScheduleValues
	{
		// Расчетный период
		public List<int> WeeksValues { get; set; }

		// Дата начала периода
		public List<string> Date { get; set; }

		// Масса тела на момент расчетного периода
		public List<float> WeightValues { get; set; }

		// % жировой ткани в организме на момент расчетного периода
		public List<float> FatPercentValues { get; set; }

		// Суточная норма килокалорий
		public List<int> KkalValues { get; set; }

		// Суточная норма белков (гр.)
		public List<int> ProteinValues { get; set; }

		// Суточная норма жиров (гр.)
		public List<int> FatValues { get; set; }

		// Суточная норма углеводов (гр.)
		public List<int> CarbongydrateValues { get; set; }

		// Типы телосложений, которые достигаются в процессе следования плану 
		public List<WeightToWeek> UnderfatZone { get; set; }
		public List<WeightToWeek> AthleticZone { get; set; }
		public List<WeightToWeek> FitZone { get; set; }
		public List<WeightToWeek> HealthyZone { get; set; }
		public List<WeightToWeek> OverfatZone { get; set; }
		public List<WeightToWeek> ObeseZone { get; set; }
	}
}
