namespace BodyCoreProduct.Models.SimpleModels
{
	public class Measurements
	{
		/// <summary>
		/// Название зоны распределения
		/// </summary>
		public string? BodyTypeName { get; set; }

		/// <summary>
		/// Относительное кол-во жировой ткани в % 
		/// </summary>
		public float? Quantity { get; set; }

		/// <summary>
		/// Распределение графика плана похудения в выбранной зоне
		/// </summary>
		public List<WeightToTime>? BodyTypeZone { get; set; }

		/// <summary>
		/// Цвет для данной зоны
		/// </summary>
		public string? Color { get; set; }
	}
}
