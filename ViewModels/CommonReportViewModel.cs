using System.Collections.Generic;

namespace BodyCore.ViewModels
{
	public class CommonReportViewModel
	{
		public string Conclusion { get; set; } = "";
		public string ChartName { get; set; } = "";
		public string Recomendations { get; set; } = "";
		public string LinkVisibility { get; set; } = "hidden";
		public string ConclusionColor { get; set; } = "rgba(255,255,255,1)";
		public List<StackedViewModel> CommonModel { get; set; }
		public string InputHeight { get; set; } = "";
		public string InputWeight { get; set; } = "";
		public string InputAge { get; set; } = "";
		public string InputWaist { get; set; } = "";
		public string InputHips { get; set; } = "";
		public string InputNeck { get; set; } = "";
		public string InputGender { get; set; } = "";
		public string ChartVisibility { get; set; } = "hidden";
	}
}
