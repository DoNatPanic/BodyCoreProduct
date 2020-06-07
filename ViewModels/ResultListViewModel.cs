using BodyCore.Models;
using System.Collections.Generic;

namespace BodyCore.ViewModels
{
	public class ResultListViewModel
	{
		public ListsResults listsResults = new ListsResults();

		public ArrayResults arrayResults = new ArrayResults();
		public string InputHeight { get; set; } = "";
		public string InputWeight { get; set; } = "";
		public string InputDreamWeight { get; set; } = "";
		public string AvailableWeight { get; set; } = "";
		public string InputAge { get; set; } = "";
		public string InputGender { get; set; } = "";
		public string InputActivity { get; set; } = "";
		public string WeeksCount { get; set; } = "";
		public string InUnderfatZone { get; set; } = "";
		public string Anchor { get; set; }
	}
}
