using BodyCore.ViewModels;
using System.Collections.Generic;

namespace BodyCore.Models
{
	public class ListsResults
	{
		public List<KbguListViewModel> KbguValues { get; set; }

		public List<WeightListViewModel> WeightValues { get; set; }
		public List<FatListViewModel> FatValues { get; set; }
	}
}
