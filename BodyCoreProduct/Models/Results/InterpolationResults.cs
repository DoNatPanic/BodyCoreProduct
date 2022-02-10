using System;
using System.Collections.Generic;
using System.Globalization;

namespace BodyCoreProduct.Models
{
	public class InterpolationResults : BaseResults
	{
		public string InputDreamWeight { get; set; }
		public string CriticalWeight { get; set; }
		public string LimitWeight { get; set; }
		public string InputActivity { get; set; }
		public string WeeksCount { get; set; }
		public ScheduleValues ScheduleValues { get; set; }

		private Dictionary<BodyType, List<WeightToTime>> _bodyTypeZonesDict;

		public InterpolationResults() { }

		public InterpolationResults(float height, float initWeight, float dream_weight, float age, string gender,
			string activity, float waist, float hips, float neck, bool hardMode)
		{
			// Отображение введенных показателей
			InputHeight = height.ToString();
			InputWeight = initWeight.ToString();
			InputAge = age.ToString();
			InputGender = gender.ToString();
			InputActivity = activity.ToString();
			InputDreamWeight = dream_weight == 0 ? "" : dream_weight.ToString();

			// Пол
			Gender genderValue = GenderDict[gender];

			// Отнесение человека к одной из 6 категорий по половозрастноым признакам
			AgeGenderClassification selectedCategory = SelectCategory(genderValue, age);

			// Расчет распределения зон телосложения, результирущих показателей % жировой ткани, веса и кол-ва недель
			var results = CalcSchedule(height, initWeight, dream_weight, age, genderValue,
				activity, waist, hips, neck, hardMode);

			// График распределения зон телосложений
			ScheduleValues = results.Item1;

			// Получаем словарь со списками распределения графика похудения по типам телосложения
			_bodyTypeZonesDict = GetBodyZonesDictionary(height, initWeight, dream_weight, waist, 
				hips, neck, genderValue, selectedCategory, hardMode);

			// Масса на границе начала зоны истощения
			LimitWeight = GetLimitWeight(_bodyTypeZonesDict[BodyType.thin]);

			// Критическая масса тела (несовместимая с жизнью)
			CriticalWeight = GetCriticalWeight(genderValue, results.Item2, results.Item3);

			// Кол-во недель в плане
			WeeksCount = results.Item4.ToString();

			// Рекомендации 
			Recomendations = GetWarning(LimitWeight, CriticalWeight);

			// Распределение веса в зависимости от недель (для каждой зоны телосложения отдельная зависимость)
			Distribution = GetInterpltnDistribution(ResultDict, _bodyTypeZonesDict);

			// Фокус на график
			Anchor = "charts";
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

		private (float, float) CalcFatPercent(float height, float weight, Gender gender, float waist, float hips, 
			float neck, bool hardMode)
		{
			float fatPercentDifference = 0;

			float fatPercent;
			if (hardMode)
			{
				var hard = KbguCalculation.FatPercentHardMode(height, gender, waist, hips, neck);
				var simple = KbguCalculation.FatPercent(height, weight, gender);
				fatPercent = hard;

				// Расчет разницы между простым и детальным расчетом
				fatPercentDifference = simple > hard ? -Math.Abs(simple - hard) : Math.Abs(simple - hard);
			}
			else
			{
				fatPercent = KbguCalculation.FatPercent(height, weight, gender);
			}

			return (fatPercent, fatPercentDifference);
		}

		private Dictionary<BodyType, List<WeightToTime>> GetBodyZonesDictionary(float height, float initWeight, float dream_weight, 
			float waist, float hips, float neck, Gender gender, AgeGenderClassification selectedCategory, bool hardMode)
		{
			var dictionary = new Dictionary<BodyType, List<WeightToTime>>()
			{
				{ BodyType.thin, BodyTypeZones.UnderfatZone = new List<WeightToTime>() },
				{ BodyType.athletic, BodyTypeZones.AthleticZone = new List<WeightToTime>() },
				{ BodyType.good, BodyTypeZones.FitZone = new List<WeightToTime>() },
				{ BodyType.average, BodyTypeZones.HealthyZone = new List<WeightToTime>() },
				{ BodyType.excess, BodyTypeZones.OverfatZone = new List<WeightToTime>() },
				{ BodyType.extra, BodyTypeZones.ObeseZone = new List<WeightToTime>() },
			};

			float weight = 0;
			float fatPercent = 0;
			float fatPercentDifference = 0;
			int week = 0;
			do
			{
				// Вес и %жировой ткани
				if (week == 0)
				{
					// Начальный вес
					weight = initWeight;

					// Расчет % жировой ткани
					var res = CalcFatPercent(height, weight, gender, waist, hips, neck, hardMode);
					fatPercent = res.Item1;
					fatPercentDifference = res.Item2;
				}
				else
				{
					// Пересчет веса
					weight = (1 - KbguCalculation.GetWeightRecession(fatPercent, gender)) * weight;

					// Персчет % жировой ткани
					fatPercent = KbguCalculation.FatPercent(height, weight, gender) + fatPercentDifference;
				}

				// Инициализация элементов списков словаря нулевыми значениями
				for(int i = 0; i < BODY_TYPES_CNT; i++)
				{
					var enm = (BodyType)i;
					dictionary[enm].Add(new WeightToTime { PeriodId = week, Weight = 0 });
				}

				// Заполнение элементов списков словаря значениями если вес находится в рассматриваемой зоне
				for (int i = BODY_TYPES_CNT - 1; i >= 0; i--)
				{
					var enm = (BodyType)i;
					if (fatPercent > selectedCategory.FatIntervals[i])
					{
						dictionary[enm][week].Weight = (float)Math.Round(weight, 2);
						break;
					}
				}
				week++;
			}
			while (!IsCriticalFatValue(gender, fatPercent) && weight > dream_weight);

			return dictionary;
		}

		private (ScheduleValues, float, float, int) CalcSchedule(float height, float initWeight, float dream_weight,
			float age, Gender gender, string activity, float waist, float hips, float neck, bool hardMode)
		{
			ScheduleValues scheduleValues = new ScheduleValues();

			float weight = 0;
			float fatPercent = 0;
			float fatPercentDifference = 0;

			int week = 0;
			do
			{
				float kkal;
				float protein;
				float fat;
				float carbohydrate;

				// Период и Дата
				scheduleValues.WeeksValues.Add(week);
				scheduleValues.Date.Add(DateTime.Now.AddDays(week * 7).ToString("dd/MM/yy", CultureInfo.InvariantCulture));

				// Вес и %жировой ткани
				if (week == 0)
				{
					// Начальный вес
					weight = initWeight;

					// Расчет % жировой ткани
					var res = CalcFatPercent(height, weight, gender, waist, hips, neck, hardMode);
					fatPercent = res.Item1;
					fatPercentDifference = res.Item2;

					// Расчет нормы калорий для поддержания веса
					kkal = KbguCalculation.GetKBGU(weight, height, age, gender, ActivityDict[activity])[0];
				}
				else
				{
					// Пересчет веса
					weight = (1 - KbguCalculation.GetWeightRecession(fatPercent, gender)) * weight;

					// Персчет % жировой ткани
					fatPercent = KbguCalculation.FatPercent(height, weight, gender) + fatPercentDifference;

					// Пересчет калорий
					var recession = 1 - KbguCalculation.GetKBGUrecession(fatPercent, gender);
					kkal = recession * KbguCalculation.GetKBGU(weight, height, age, gender, ActivityDict[activity])[0];
				}

				scheduleValues.WeightValues.Add((float)Math.Round(weight, 2));
				scheduleValues.FatPercentValues.Add((float)Math.Round(fatPercent, 2));

				// Норма БЖУ
				protein = KbguCalculation.GetKBGU(weight, height, age, gender, ActivityDict[activity])[1];
				fat = KbguCalculation.GetKBGU(weight, height, age, gender, ActivityDict[activity])[2];
				carbohydrate = KbguCalculation.GetKBGU(weight, height, age, gender, ActivityDict[activity])[3];

				scheduleValues.KkalValues.Add((int)Math.Round(kkal));
				scheduleValues.ProteinValues.Add((int)Math.Round(protein));
				scheduleValues.FatValues.Add((int)Math.Round(fat));
				scheduleValues.CarbongydrateValues.Add((int)Math.Round(carbohydrate));

				week++;
			}
			while (!IsCriticalFatValue(gender, fatPercent) && weight > dream_weight);

			return (scheduleValues, fatPercent, weight, week);
		}

		private bool IsCriticalFatValue(Gender gender, float fatPercent)
		{
			bool isCriticalFatValue = false;
			if ((gender == Gender.female && fatPercent <= 15f) || (gender == Gender.male && fatPercent < 6.5f))
				isCriticalFatValue = true;
			return isCriticalFatValue;
		}

		private string GetCriticalWeight(Gender gender, float fatPercent, float weight)
		{
			string criticalWeight = "";
			if (IsCriticalFatValue(gender, fatPercent))
				criticalWeight = Math.Round(weight, 2).ToString();
			return criticalWeight;
		}

		private string GetLimitWeight(List<WeightToTime> underFatZone)
		{
			string limitWeight = "";
			foreach (var t in underFatZone)
			{
				if (t.Weight != 0)
				{
					limitWeight = Math.Round(t.Weight).ToString();
					break;
				}
			}
			return limitWeight;
		}

		private string GetWarning(string limitWeight, string criticalWeight)
		{
			string warning = "";

			if (limitWeight != "")
			{
				warning += $"Рекомендуется не снижать вес до области истощения. " +
					$"Начало границы истощения для вас начинается при { limitWeight } кг.";

				if (criticalWeight != "")
				{
					warning += $" Критическая масса тела (несовместимая с жизнью) составляет { criticalWeight } кг.";
				}
			}
			return warning;
		}

		private Distribution GetInterpltnDistribution(Dictionary<BodyType, BodyTypeAttributes> result, 
			Dictionary<BodyType, List<WeightToTime>> dictionary)
		{
			var distribution = GetDistribution(result);
			distribution.DistributionName = InterpltnDistributionName;

			for (int i = 0; i < BODY_TYPES_CNT; i++)
			{
				var enm = (BodyType)i;
				distribution.Measurements[i].BodyTypeZone = dictionary[enm];
			}

			return distribution;
		}
	}
}
