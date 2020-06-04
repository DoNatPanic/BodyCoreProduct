using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyCore.Data.Models
{
	public class KBGU
	{
		float getBMR( float weight, float height, float age, string gender )
		{
			// BMR - базальная скорость обмена
			float result = 9.99f * weight + 6.25f * height - 4.92f * age;
			result = gender == "Женщина" ? result - 161 : result + 5;
			return result;
		}

		public float[] getKBGU( float weight, float height, float age, string gender, string activity )
		{
			float activityLevel = activityDict[activity];
			float[] result = new float[4];

			// kkal = BMR * активность
			result[0] = getBMR(weight, height, age, gender) * activityLevel;

			// белки (гр)
			result[1] = 1.8f * weight;

			// жиры (гр)
			result[2] = 1.1f * weight;

			// углеводы (гр)
			result[3] = ( result[0] - 4.1f * result[1] - 9.29f * result[2] ) / 9.29f;

			return result;
		}

		// активность
		Dictionary<string, float> activityDict = new Dictionary<string, float> {
			{"Минимальная", 1.2f },
			{"Небольшая", 1.375f },
			{"Средняя", 1.46f },
			{"Выше среднего", 1.55f },
			{"Повышенная", 1.64f },
			{"Высокая", 1.72f },
			{"Очень высокая", 1.9f }
		};
		public float getKBGUrecession( float fatPercent, string gender )
		{
			float result = gender == "Женщина" ? 0.018069f + 0.004991f * fatPercent : 0.100955f + 0.000471f * fatPercent;
			return result;
		}
		public float getWeightRecession( float fatPercent, string gender )
		{
			float result = gender == "Женщина" ? -0.001597f + 0.000250f * fatPercent : 0.000951f + 0.000262f * fatPercent;
			return result;
		}

		public float fatPercent( float height, float weight, string gender )
		{
			float imt = weight / ( height / 100 * height / 100 );
			float femaleFat = -5.079140f + 1.370550f * imt + 0.005301f * imt * imt;
			float maleFat = -19.487921f + 1.524293f * imt - 0.000091f * imt * imt;
			float result = gender == "Женщина" ? femaleFat : maleFat;

			return result;
		}
		public float fatPercentHardMode( float height, float weight, string gender, float age, float waist, float hips, float neck)
		{
			float femFat = 163.205f * (float) Math.Log10(waist + hips - neck) - 97.684f * (float) Math.Log10(height) - 104.912f;
			float mFat = 86.01f * (float) Math.Log10(waist - neck) - 70.041f * (float) Math.Log10(height) + 30.3f;
			float result = gender == "Женщина" ? femFat : mFat;

			return result;
		}
	}
}
