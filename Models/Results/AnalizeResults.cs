using BodyCoreProduct.Models.Enums;
using BodyCoreProduct.Models.SimpleModels;

namespace BodyCoreProduct.Models.Results
{
	public class AnalizeResults : BaseResults
	{
		public string? InputWaist { get; set; }
		public string? InputHips { get; set; }
		public string? InputNeck { get; set; }
		public string? ViscellarFatConclusion { get; set; }
		public string LinkVisibility { get; set; } = "hidden";
		public string ChartVisibility { get; set; } = "hidden";

		public AnalizeResults() { }

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
			BodyType bodyTypeZone = CalcBodyType(selectedCategory, fatPercent);

			// Результат: тип телосложения и % жировой ткани + Рекомендация
			Recomendations = $"У вас наблюдается {ResultDict[bodyTypeZone]?.Name?.ToLower()}," +
				$" жировая ткань в организме составляет {Math.Round(fatPercent, 2)} % " +
				ResultDict[bodyTypeZone].Recomendations;

			// Свойство видимости ссылки на страницу с составлением плана снижения веса
			LinkVisibility = GetVisibility(bodyTypeZone);

			// Свойство видимости графика
			ChartVisibility = "visible";

			// Распределение по типам телосложения в зав-сти от % жировой ткани в организме для выбранной половозрастной категории
			Distribution = GetAnalizeDistribution(ResultDict, selectedCategory);

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

		private static AgeGenderClassification SelectCategory(Gender gender, float age)
		{
			AgeGenderClassification selectedCategory = new();

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

		private static BodyType CalcBodyType(AgeGenderClassification selectedCategory, float fatPercent)
		{
			var bodyTypeZone = BodyType.thin;

			for (int i = BODY_TYPES_CNT - 1; i >= 0; i--)
			{
				var enm = (BodyType)i;
				if (fatPercent > selectedCategory.FatIntervals[i])
				{
					bodyTypeZone = enm;
					break;
				}
			}
			return bodyTypeZone;
		}

		private static string GetVisibility(BodyType bodyTypeZone)
		{
			var visibility = "hidden";
			if (bodyTypeZone == BodyType.average || bodyTypeZone == BodyType.excess || bodyTypeZone == BodyType.extra)
				visibility = "visible";
			return visibility;
		}

		private Distribution GetAnalizeDistribution(Dictionary<BodyType, BodyTypeAttributes> result,
			AgeGenderClassification selectedCategory)
		{
			var distribution = GetDistribution(result);
			distribution.DistributionName = AnalizeDistributionName;

			for (int i = 0; i < BODY_TYPES_CNT; i++)
			{
				var prevValue = i == 0 ? 0 : distribution.Measurements![i - 1].Quantity;
				var diff = selectedCategory.FatIntervals[i + 1] - selectedCategory.FatIntervals[i];
				distribution.Measurements![i].Quantity = diff + prevValue;
			}
			return distribution;
		}
	}
}
