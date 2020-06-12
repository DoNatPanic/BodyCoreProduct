using System;

namespace BodyCore.ViewModels
{
	public class ArrayResults
	{
		public float[] WeeksValues { get; set; } = { 0 };
		//public string[] date { get; set; } = { DateTime.Now.ToShortDateString() };
		public string[] date { get; set; } = { "" };
		public int[] KbguValues { get; set; } = { 0 };
		public int[] ProteinValues { get; set; } = { 0 };
		public int[] FatValues { get; set; } = { 0 };
		public int[] CabongydrateValues { get; set; } = { 0 };
		public float[] WeightValues { get; set; } = { 0 };
		public float[] FatPercentValues { get; set; } = { 0 };

	}
}
