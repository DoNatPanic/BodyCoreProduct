namespace BodyCoreProduct.Models
{
	public class AgeGenderClassification
	{
		public float AgeMin;
		public float AgeMax;
		public Gender Gender;
		public float [] FatIntervals = new float[6];

		public AgeGenderClassification()
		{

		}

		public AgeGenderClassification(float[] fatIntervals, float ageMin, float ageMax, Gender gender)
		{
			for(int i = 0; i < 5; i++ )
			{
				FatIntervals[i] = fatIntervals[i];
 			}
			AgeMin = ageMin;
			AgeMax = ageMax;
			Gender = gender;
		}
	}
}
