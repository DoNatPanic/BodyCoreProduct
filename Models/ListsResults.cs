using BodyCore.ViewModels;
using System.Collections.Generic;

namespace BodyCore.Models
{
	public class ListsResults
	{
		public List<KbguListViewModel> KbguValues { get; set; }

		public List<WeightListViewModel> WeightValues { get; set; }
		public List<FatListViewModel> FatValues { get; set; }
		public List<WeightListViewModel> UnderfatZone { get; set; }
		public List<WeightListViewModel> AthleticZone { get; set; }
		public List<WeightListViewModel> FitZone { get; set; }
		public List<WeightListViewModel> HealthyZone { get; set; }
		public List<WeightListViewModel> OverfatZone { get; set; }
		public List<WeightListViewModel> ObeseZone { get; set; }


	}
}
