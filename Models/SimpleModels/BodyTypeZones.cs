using System.Collections.Generic;

namespace BodyCoreProduct.Models.SimpleModels
{
	public static class BodyTypeZones
	{
		/// <summary>
		/// Зона истощения
		/// </summary>
		public static List<WeightToTime>? UnderfatZone { get; set; }

		/// <summary>
		/// Атлетическое телосложение
		/// </summary>
		public static List<WeightToTime>? AthleticZone { get; set; }

		/// <summary>
		/// Хорошая физическая форма
		/// </summary>
		public static List<WeightToTime>? FitZone { get; set; }

		/// <summary>
		/// Средний уровень жира
		/// </summary>
		public static List<WeightToTime>? HealthyZone { get; set; }

		/// <summary>
		/// Наличие лишнего веса
		/// </summary>
		public static List<WeightToTime>? OverfatZone { get; set; }

		/// <summary>
		/// Ожирение
		/// </summary>
		public static List<WeightToTime>? ObeseZone { get; set; }
	}
}
