namespace BodyCoreProduct.Models.SimpleModels
{
	public class WeightToTime
	{
		/// <summary>
		/// Порядковый номер периода
		/// </summary>
		public int PeriodId { get; set; }

		/// <summary>
		/// Значение массы тела на начало периода
		/// </summary>
		public float Weight { get; set; }
	}
}
