using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyCore.ViewModels
{
	public class FatCalculationViewModel
	{
		public string InputHeight { get; set; } = "";
		public string InputWeight { get; set; } = "";
		public string InputWaist { get; set; } = "";
		public string InputHips { get; set; } = "";
		public string InputNeck{ get; set; } = "";
		public string InputGender { get; set; } = "";
		public string InputAge { get; set; } = "";
		public float CalculatedFatPercent { get; set; } = 0;
		public string Conclusion { get; set; } = "";
	}
}
