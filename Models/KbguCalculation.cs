using BodyCoreProduct.Models.Enums;

namespace BodyCoreProduct.Models
{
	public static class KbguCalculation
	{
		// Вычислить 4 основных показателя
		public static float[] GetKBGU(float weight, float height, float age, Gender gender, Activity activity)
		{
			float activityLevelValue = _activityValueDict[activity];

			float[] result = new float[4];

			// kkal = BMR * активность
			result[0] = getBMR(weight, height, age, gender) * activityLevelValue;

			// белки (гр)
			result[1] = 1.8f * weight;

			// жиры (гр)
			result[2] = 1.1f * weight;

			// углеводы (гр)
			result[3] = (result[0] - 4.1f * result[1] - 9.29f * result[2]) / 9.29f;

			return result;
		}

		// Вычислить уменьшение показателей КБЖУ для программы снижения веса
		public static float GetKBGUrecession(float fatPercent, Gender gender)
		{
			return gender == Gender.female ? 0.018069f + 0.004991f * fatPercent : 0.100955f + 0.000471f * fatPercent;
		}

		// Вычислить уменьшение веса
		public static float GetWeightRecession(float fatPercent, Gender gender)
		{
			return gender == Gender.female ? -0.001597f + 0.000250f * fatPercent : 0.000951f + 0.000262f * fatPercent;
		}

		// Вычислить % жировой ткани в организме (простой расчет)
		public static float FatPercent(float height, float weight, Gender gender)
		{
			float imt = weight / (height / 100 * height / 100);
			float femaleFat = -5.079140f + 1.370550f * imt + 0.005301f * imt * imt;
			float maleFat = -19.487921f + 1.524293f * imt - 0.000091f * imt * imt;

			return gender == Gender.female ? femaleFat : maleFat;
		}

		// Вычислить % жировой ткани в организме (детальный расчет)
		public static float FatPercentHardMode(float height, Gender gender, float waist, float hips, float neck)
		{
			float femFat = 163.205f * (float)Math.Log10(waist + hips - neck) - 97.684f * (float)Math.Log10(height) - 104.912f;
			float mFat = 86.01f * (float)Math.Log10(waist - neck) - 70.041f * (float)Math.Log10(height) + 30.3f;

			return gender == Gender.female ? femFat : mFat; ;
		}

		// Удельный вес, соответствующий уровеню активности
		private static readonly Dictionary<Activity, float> _activityValueDict = new Dictionary<Activity, float>
		{
			{ Activity.verySmall, 1.2f },
			{ Activity.small, 1.375f },
			{ Activity.average, 1.46f },
			{ Activity.aboveAverage, 1.55f },
			{ Activity.increased, 1.64f },
			{ Activity.high, 1.72f },
			{ Activity.veryHigh, 1.9f }
		};

		// Вычислить BMR - базальную скорость обмена
		private static float getBMR(float weight, float height, float age, Gender gender)
		{
			float result = 9.99f * weight + 6.25f * height - 4.92f * age;
			result = gender == Gender.female ? result - 161 : result + 5;

			return result;
		}
	}
}
