using System;
using System.Collections.Generic;
using System.Globalization;

namespace BodyCoreProduct.Models
{
	public class InterpolationResults : Base
	{
		public string InputDreamWeight { get; set; }
		public string CriticalWeight { get; set; }
		public string LimitWeight { get; set; }
		public string InputActivity { get; set; }
		public string WeeksCount { get; set; }
		public string Anchor { get; set; }
		public ScheduleValues ScheduleValues { get; set; }

		public InterpolationResults()
		{
			// Пустые введенные показатели
			InputHeight = "";
			InputWeight = "";
			InputAge = "";
			InputGender = "";
			InputDreamWeight = "";
			InputActivity = "";

			// Пустые строки с результатом
			CriticalWeight = "";
			LimitWeight = "";
			WeeksCount = "";
			Recomendations = "";

			// Пустые показатели плана снижения веса
			ScheduleValues = InitScheduleValues();
		}

		public InterpolationResults(float height, float initWeight, float dream_weight, float age, string gender,
			string activity, float waist, float hips, float neck, bool hardMode)
		{
			// Пустые введенные показатели
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
			var results = CalcValues(height, initWeight, dream_weight, age, genderValue,
				activity, waist, hips, neck, hardMode, selectedCategory);

			// График распределения зон телосложений
			ScheduleValues = results.Item1;

			// Масса на границе начала зоны истощения
			LimitWeight = GetLimitWeight(ScheduleValues.UnderfatZone);

			// Критическая масса тела (несовместимая с жизнью)
			CriticalWeight = GetCriticalWeight(genderValue, results.Item2, results.Item3);

			// Кол-во недель в плане
			WeeksCount = results.Item4.ToString();

			// Рекомендации 
			Recomendations = GetWarning(LimitWeight, CriticalWeight);

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

		private ScheduleValues InitScheduleValues()
		{
			return new ScheduleValues()
			{
				WeeksValues = new List<int>(),
				Date = new List<string>(),

				WeightValues = new List<float>(),
				FatPercentValues = new List<float>(),

				KkalValues = new List<int>(),
				ProteinValues = new List<int>(),
				FatValues = new List<int>(),
				CarbongydrateValues = new List<int>(),

				UnderfatZone = new List<WeightToWeek>(),
				AthleticZone = new List<WeightToWeek>(),
				FitZone = new List<WeightToWeek>(),
				HealthyZone = new List<WeightToWeek>(),
				OverfatZone = new List<WeightToWeek>(),
				ObeseZone = new List<WeightToWeek>()
			};
		}

		private (ScheduleValues, float, float, int) CalcValues(float height, float initWeight, float dream_weight, float age, Gender gender,
			string activity, float waist, float hips, float neck, bool hardMode, AgeGenderClassification selectedCategory)
		{
			 ScheduleValues scheduleValues = InitScheduleValues();

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

					// Расчет нормы калорий для поддержания веса
					kkal = KbguCalculation.GetKBGU(weight, height, age, gender, activity)[0];
				}
				else
				{
					// Пересчет веса
					weight = (1 - KbguCalculation.GetWeightRecession(fatPercent, gender)) * weight;

					// Персчет % жировой ткани
					fatPercent = KbguCalculation.FatPercent(height, weight, gender) + fatPercentDifference;

					// Пересчет калорий
					kkal = (1 - KbguCalculation.GetKBGUrecession(fatPercent, gender)) * KbguCalculation.GetKBGU(weight, height, age, gender, activity)[0];
				}

				scheduleValues.WeightValues.Add((float)Math.Round(weight, 2));
				scheduleValues.FatPercentValues.Add((float)Math.Round(fatPercent, 2));

				// Норма БЖУ
				protein = KbguCalculation.GetKBGU(weight, height, age, gender, activity)[1];
				fat = KbguCalculation.GetKBGU(weight, height, age, gender, activity)[2];
				carbohydrate = KbguCalculation.GetKBGU(weight, height, age, gender, activity)[3];

				scheduleValues.KkalValues.Add((int)Math.Round(kkal));
				scheduleValues.ProteinValues.Add((int)Math.Round(protein));
				scheduleValues.FatValues.Add((int)Math.Round(fat));
				scheduleValues.CarbongydrateValues.Add((int)Math.Round(carbohydrate));

				// Зоны
				scheduleValues.UnderfatZone.Add(new WeightToWeek { Week = week, Weight = 0 });
				scheduleValues.AthleticZone.Add(new WeightToWeek { Week = week, Weight = 0 });
				scheduleValues.FitZone.Add(new WeightToWeek { Week = week, Weight = 0 });
				scheduleValues.HealthyZone.Add(new WeightToWeek { Week = week, Weight = 0 });
				scheduleValues.OverfatZone.Add(new WeightToWeek { Week = week, Weight = 0 });
				scheduleValues.ObeseZone.Add(new WeightToWeek { Week = week, Weight = 0 });

				if (fatPercent <= selectedCategory.FatIntervals[0])
				{
					scheduleValues.UnderfatZone[week].Weight = (float)Math.Round(weight, 2);
				}
				else if (fatPercent > selectedCategory.FatIntervals[0] && fatPercent <= selectedCategory.FatIntervals[1])
				{
					scheduleValues.AthleticZone[week].Weight = (float)Math.Round(weight, 2);
				}
				else if (fatPercent > selectedCategory.FatIntervals[1] && fatPercent <= selectedCategory.FatIntervals[2])
				{
					scheduleValues.FitZone[week].Weight = (float)Math.Round(weight, 2);
				}
				else if (fatPercent > selectedCategory.FatIntervals[2] && fatPercent <= selectedCategory.FatIntervals[3])
				{
					scheduleValues.HealthyZone[week].Weight = (float)Math.Round(weight, 2);
				}
				else if (fatPercent > selectedCategory.FatIntervals[3] && fatPercent <= selectedCategory.FatIntervals[4])
				{
					scheduleValues.OverfatZone[week].Weight = (float)Math.Round(weight, 2);
				}
				else if (fatPercent > selectedCategory.FatIntervals[4])
				{
					scheduleValues.ObeseZone[week].Weight = (float)Math.Round(weight, 2);
				}

				week++;
			}
			while (!IsCriticalFatValue(gender, fatPercent) && weight > dream_weight);

			return ( scheduleValues, fatPercent, weight, week );
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

		private string GetLimitWeight(List<WeightToWeek> underFatZone)
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
	}
}
