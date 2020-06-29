using BodyCore.Models;
using System.Collections.Generic;

namespace BodyCore.ViewModels
{
	public class WeightLossViewModel
	{
		public ListsResults listsResults = new ListsResults();

		public ArrayResults arrayResults = new ArrayResults();
		public string InputHeight { get; set; } = "";
		public string InputWeight { get; set; } = "";
		public string InputDreamWeight { get; set; } = "";
		public string CriticalWeight { get; set; } = "";
		public string LimitWeight { get; set; } = "";
		public string InputAge { get; set; } = "";
		public string InputGender { get; set; } = "";
		public string InputActivity { get; set; } = "";
		public string InputWaist { get; set; } = "";
		public string InputHips { get; set; } = "";
		public string InputNeck { get; set; } = "";
		public string WeeksCount { get; set; } = "";
		public string InUnderfatZone { get; set; } = "";
		public string Anchor { get; set; }
		public string Conclusion { get; set; } = "";
		public string ChartName { get; set; } = "";
		public string Recomendations { get; set; } = "";
		public string ViscellarFatConclusion { get; set; } = "";
		public string KeepWeightKkal { get; set; } = "";
		public string ConclusionColor { get; set; } = "rgba(255,255,255,1)";
		public List<StackedViewModel> CommonModel { get; set; }
	}
}
