namespace BodyCore.Models
{
	public class AgeGenderFatNorm
	{
		public float AgeMin;
		public float AgeMax;
		public string Gender;
		public float [] FatIntervals = new float[5];

		public AgeGenderFatNorm()
		{

		}
		public AgeGenderFatNorm(float[] fatIntervals, float ageMin, float ageMax, string gender)
		{
			for(int i = 0; i < 5; i++ )
			{
				FatIntervals[i] =fatIntervals[i];
 			}
			AgeMin = ageMin;
			AgeMax = ageMax;
			Gender = gender;
		}

		/*public string GetConclusion()
		{
			var conclusion = "";
			if ( FatPercent <= FatIntervals[0] )
				conclusion = "Истощение";
			else if ( FatPercent > FatIntervals[0] && FatPercent <= FatIntervals[1] )
				conclusion = "Атлетическое телосложение";
			else if ( FatPercent > FatIntervals[1] && FatPercent <= FatIntervals[2] )
				conclusion = "Хорошая физическая форма";
			else if ( FatPercent > FatIntervals[2] && FatPercent <= FatIntervals[3] )
				conclusion = "Средний уровень жира";
			else if ( FatPercent > FatIntervals[3] && FatPercent <= FatIntervals[4] )
				conclusion = "Наличие лишнего веса";
			else conclusion = "Ожирение";

			return conclusion;
		}*/
	}
}
