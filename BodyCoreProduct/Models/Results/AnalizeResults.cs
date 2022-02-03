using System;
using System.Collections.Generic;

namespace BodyCoreProduct.Models
{
	public class AnalizeResults : Base
	{
		public string InputWaist { get; set; }
		public string InputHips { get; set; }
		public string InputNeck { get; set; }
		public string Statement { get; set; }
		public string ViscellarFatConclusion { get; set; }
		public string LinkVisibility { get; set; }
		public string ConclusionColor { get; set; }
		public string Anchor { get; set; }
		public List<Distribution> Distributions { get; set; }

		public AnalizeResults()
		{
			// Пустые введенные показатели
			InputGender = "";
			InputAge = "";
			InputWaist = "";
			InputHips = "";
			InputNeck = "";
			InputHeight = "";
			InputWeight = "";

			// Пустые строки с рекомендациями
			ViscellarFatConclusion = "";
			Statement = "";
			Recomendations = "";
			LinkVisibility = "hidden";
			ConclusionColor = "rgba(255,255,255,1)";

			// Пустой график распределения
			Distributions = new List<Distribution>()
			{
				new Distribution()
				{
					DistrName = "",
					Measurements = new List<SimpleReport>()
					{
						new SimpleReport()
						{
							DimensionOne = "",
							ColorRGB = "rgba(0,0,0,0)",
							Quantity = 0
						}
					}
				}
			};
		}

		public AnalizeResults(string gender, float age, float waist, float hips, float neck,
			float height, float weight, bool hardMode)
		{
			// Отображение введенных показателей
			InputGender = gender;
			InputAge = age.ToString();
			InputWaist = waist == 0 ? "" : waist.ToString();
			InputHips = hips == 0 ? "" : hips.ToString();
			InputNeck = neck == 0 ? "" : neck.ToString();
			InputHeight = height.ToString();
			InputWeight = weight.ToString();

			Gender genderValue = GenderDict[gender];

			// Расчет % жировой ткани в организме
			float fatPercent = CalcFatPercent(genderValue, waist, hips, neck, height, weight, hardMode);

			// Не завышен ли уровень висцеральной жировой ткани
			ViscellarFatConclusion = CalcVisceralFat(genderValue, waist, hips, hardMode);

			// Отнесение человека к одной из 6 категорий по половозрастноым признакам
			AgeGenderClassification selectedCategory = SelectCategory(genderValue, age);

			// Вычисление типа телосложения человека
			BodyTypeZone bodyTypeZone = CalcBodyType(selectedCategory, fatPercent);

			// Результат: тип телосложения и % жировой ткани
			Statement = $"У вас наблюдается {Result[bodyTypeZone].Name.ToLower()}, жировая ткань в организме составляет {Math.Round(fatPercent, 2)} %";

			// Рекомендация
			Recomendations = Result[bodyTypeZone].Recomendations;

			// Цвет строки рекомендации
			ConclusionColor = Result[bodyTypeZone].Color;

			// Свойство видимости ссылки на страницу с составлением плана снижения веса
			LinkVisibility = GetVisibility(bodyTypeZone);

			// Распределение по типам телосложения в зав-сти от % жировой ткани в организме для выбранной половозрастной категории
			Distributions = GetDistribution(Result, selectedCategory);

			// Фокус на график
			Anchor = "charts";
		}

		private float CalcFatPercent(Gender gender, float waist, float hips, float neck, float height, float weight, bool hardMode)
		{
			float fatPercent = KbguCalculation.FatPercent(height, weight, gender);
			if (hardMode)
			{
				fatPercent = KbguCalculation.FatPercentHardMode(height, gender, waist, hips, neck);
			}
			return fatPercent;
		}

		private string CalcVisceralFat(Gender gender, float waist, float hips, bool hardMode)
		{
			string visceralFatConclusion = "";
			if (hardMode)
			{
				float visceralFat = waist / hips;
				if ((gender == Gender.male && visceralFat > 1f) || (gender == Gender.female && visceralFat > 0.85f))
					visceralFatConclusion = ExcessVisceralFat;
			}
			return visceralFatConclusion;
		}

		private AgeGenderClassification SelectCategory(Gender gender, float age)
		{
			AgeGenderClassification selectedCategory = new AgeGenderClassification();

			foreach (var category in PersonCategories)
			{
				if (age >= category.AgeMin && age < category.AgeMax && gender == category.Gender)
				{
					selectedCategory = category;
					break;
				}
			}
			return selectedCategory;
		}

		private BodyTypeZone CalcBodyType(AgeGenderClassification selectedCategory, float fatPercent)
		{
			BodyTypeZone bodyTypeZone = BodyTypeZone.good;

			if (fatPercent <= selectedCategory.FatIntervals[0])
			{
				bodyTypeZone = BodyTypeZone.thin;
			}
			else if (fatPercent > selectedCategory.FatIntervals[0] && fatPercent <= selectedCategory.FatIntervals[1])
			{
				bodyTypeZone = BodyTypeZone.athletic;
			}
			else if (fatPercent > selectedCategory.FatIntervals[1] && fatPercent <= selectedCategory.FatIntervals[2])
			{
				bodyTypeZone = BodyTypeZone.good;
			}
			else if (fatPercent > selectedCategory.FatIntervals[2] && fatPercent <= selectedCategory.FatIntervals[3])
			{
				bodyTypeZone = BodyTypeZone.average;
			}
			else if (fatPercent > selectedCategory.FatIntervals[3] && fatPercent <= selectedCategory.FatIntervals[4])
			{
				bodyTypeZone = BodyTypeZone.excess;
			}
			else if (fatPercent > selectedCategory.FatIntervals[4])
			{
				bodyTypeZone = BodyTypeZone.extra;
			}
			return bodyTypeZone;
		}

		private string GetVisibility(BodyTypeZone bodyTypeZone)
		{
			var visibility = "hidden";
			if (bodyTypeZone == BodyTypeZone.average || bodyTypeZone == BodyTypeZone.excess || bodyTypeZone == BodyTypeZone.extra)
				visibility = "visible";
			return visibility;
		}

		private List<Distribution> GetDistribution(Dictionary<BodyTypeZone, CategoryAttributes> result, AgeGenderClassification selectedCategory)
		{
			return new List<Distribution>
			{
				new Distribution
				{
					DistrName = DistributionName,

					Measurements = new List<SimpleReport>()
					{
						new SimpleReport() {
							DimensionOne = result[BodyTypeZone.thin].Name,
							Quantity = selectedCategory.FatIntervals[0] - 0f,
							ColorRGB = result[BodyTypeZone.thin].Color
						},
						new SimpleReport()
						{
							DimensionOne = result[BodyTypeZone.athletic].Name,
							Quantity = selectedCategory.FatIntervals[1] - selectedCategory.FatIntervals[0],
							ColorRGB = result[BodyTypeZone.athletic].Color
						},
						new SimpleReport()
						{
							DimensionOne = result[BodyTypeZone.good].Name,
							Quantity = selectedCategory.FatIntervals[2] - selectedCategory.FatIntervals[1],
							ColorRGB = result[BodyTypeZone.good].Color 
						},
						new SimpleReport()
						{
							DimensionOne = result[BodyTypeZone.average].Name,
							Quantity = selectedCategory.FatIntervals[3] - selectedCategory.FatIntervals[2],
							ColorRGB = result[BodyTypeZone.average].Color
						},
						new SimpleReport()
						{
							DimensionOne = result[BodyTypeZone.excess].Name,
							Quantity = selectedCategory.FatIntervals[4] - selectedCategory.FatIntervals[3],
							ColorRGB = result[BodyTypeZone.excess].Color 
						},
						new SimpleReport()
						{
							DimensionOne = result[BodyTypeZone.extra].Name,
							Quantity = 100f - selectedCategory.FatIntervals[4],
							ColorRGB = result[BodyTypeZone.extra].Color
						}
					}
				}
			};
		}
	}
}
