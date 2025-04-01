using BodyCoreProduct.Models.Enums;

namespace BodyCoreProduct.Models
{
	/* Пол и возраст значительно влияют на процент содержания жировой ткани, 
	 * поэтому создаются N категорий людей, для которых приписывают интервалы типов телосложения
	 * Кол-во интервалов такое же, сколько и типов телосложения (переменная BODY_TYPES_CNT в классе BaseResults.cs)
	 * Значит массив FatIntervals имеет размер равный BODY_TYPES_CNT + 1
	 */
	public struct AgeGenderClassification
	{
		/// <summary>
		/// Нижний возрастной порог категории
		/// </summary>
		public float AgeMin { get; set; }

		/// <summary>
		/// Верхний возрастной порог категории
		/// </summary>
		public float AgeMax { get; set; }

		/// <summary>
		/// Пол данной категории
		/// </summary>
		public Gender Gender { get; set; }

		/// <summary>
		/// Интервалы содержания жира в организме человека для
		/// соотнесения его с тем или иным типом телосложения 
		/// </summary>
		public float[] FatIntervals { get; set; }
	}
}
