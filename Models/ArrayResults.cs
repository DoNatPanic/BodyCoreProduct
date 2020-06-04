using System;

namespace BodyCore.ViewModels
{
	public class ArrayResults
	{
		public float[] WeeksValues { get; set; } = { 0 };
		public string[] date { get; set; } = { DateTime.Now.ToShortDateString() };
		public float[] KbguValues { get; set; } = { 0 };
		public float[] ProteinValues { get; set; } = { 0 };
		public float[] FatValues { get; set; } = { 0 };
		public float[] CabongydrateValues { get; set; } = { 0 };
		public float[] WeightValues { get; set; } = { 0 };
		public float[] FatPercentValues { get; set; } = { 0 };

	}
}
