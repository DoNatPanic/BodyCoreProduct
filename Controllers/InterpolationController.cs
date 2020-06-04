using System.Collections.Generic;
using BodyCore.Data.Models;
using BodyCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using BodyCore.Models;
using System.Linq;
using System;

namespace BodyCore.Controllers
{
	public class InterpolationController : Controller
	{
		[HttpGet]
		public IActionResult Input()
		{
			List<KbguListViewModel> kbguList = new List<KbguListViewModel>() { new KbguListViewModel { Week = 0, Kbgu = 0 } }; 
			List<WeightListViewModel> weightList = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0} };

			ListsResults lst = new ListsResults();
			ArrayResults arr = new ArrayResults();
			ResultListViewModel obj = new ResultListViewModel();
			
			lst.WeightValues = weightList;
			lst.KbguValues = kbguList;
			obj.listsResults = lst;
			obj.arrayResults = arr;

			return View(obj);
		}

		[HttpPost]
		public IActionResult Input( bool disclaimer, float height, float weight, float dream_weight, float age, string gender, string activity )
		{
			if ( !disclaimer )
			{
				List<KbguListViewModel> kbguList = new List<KbguListViewModel>() { new KbguListViewModel { Week = 0, Kbgu = 0 } };
				List<WeightListViewModel> weightList = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };

				ListsResults lst = new ListsResults();
				ArrayResults arr = new ArrayResults();
				ResultListViewModel obj = new ResultListViewModel();

				lst.WeightValues = weightList;
				lst.KbguValues = kbguList;
				obj.listsResults = lst;
				obj.arrayResults = arr;

				return View(obj);
			}
			else
			{
				float initWeight = weight;
				float kbgu;
				float protein;
				float fat;
				float carbohydrate;
				float fatPercent;
				float criticalWeight = 0f;

				KBGU mKBGU = new KBGU();

				List<KbguListViewModel> kbguList = new List<KbguListViewModel>();
				List<WeightListViewModel> weightList = new List<WeightListViewModel>();
				List<FatListViewModel> fatParcentList = new List<FatListViewModel>();
				ArrayResults arr = new ArrayResults();
				ListsResults lst = new ListsResults();
				ResultListViewModel obj = new ResultListViewModel();

				weightList.Add(new WeightListViewModel { Week = 1, Weight = weight });
				kbgu = mKBGU.getKBGU(weight, height, age, gender, activity)[0];
				protein = mKBGU.getKBGU(weight, height, age, gender, activity)[1];
				fat = mKBGU.getKBGU(weight, height, age, gender, activity)[2];
				carbohydrate = mKBGU.getKBGU(weight, height, age, gender, activity)[3];
				kbguList.Add(new KbguListViewModel { Week = 1, Kbgu = kbgu, Protein = protein, Fat = fat, Carbohydrate = carbohydrate });
				fatPercent = mKBGU.fatPercent(height, weight, gender);
				fatParcentList.Add(new FatListViewModel { Week = 1, Fat = fatPercent });

				int week = 1;
				do
				{
					week++;

					weight = ( 1 - mKBGU.getWeightRecession(fatPercent, gender) ) * weight;
					weightList.Add(new WeightListViewModel { Week = week, Weight = weight });

					fatPercent = mKBGU.fatPercent(height, weight, gender);
					fatParcentList.Add(new FatListViewModel { Week = week, Fat = fatPercent });

					kbgu = ( 1 - mKBGU.getKBGUrecession(fatPercent, gender) ) * mKBGU.getKBGU(weight, height, age, gender, activity)[0];
					protein = mKBGU.getKBGU(weight, height, age, gender, activity)[1];
					fat = mKBGU.getKBGU(weight, height, age, gender, activity)[2];
					carbohydrate = mKBGU.getKBGU(weight, height, age, gender, activity)[3];
					kbguList.Add(new KbguListViewModel { Week = week, Kbgu = kbgu, Protein = protein, Fat = fat, Carbohydrate = carbohydrate });

					//расчитываем критическую массу, если процент жира упал до предельной отметки
					if ( (gender == "Женщина" && fatPercent <= 15f) || ( gender == "Мужчина" && fatPercent < 6.5f ))
						criticalWeight = weight;
				}
				while ( ( ( gender == "Женщина" && fatPercent > 15f ) || ( gender == "Мужчина" && fatPercent > 6.5f ) ) && weight > dream_weight );

				arr.WeeksValues = new float[week];
				arr.date = new string[week];
				arr.WeightValues = new float[week];
				arr.FatPercentValues = new float[week];
				arr.KbguValues = new float[week];
				arr.ProteinValues = new float[week];
				arr.FatValues = new float[week];
				arr.CabongydrateValues = new float[week];

				int iter0 = 0;
				foreach ( var it in weightList.Select(x => x.Week) )
				{
					arr.WeeksValues[iter0] = it;
					iter0++;
				}
				int iter1 = 0;
				foreach ( var it in weightList.Select(x => x.Weight) )
				{
					arr.WeightValues[iter1] = (float) Math.Round(it, 2);
					iter1++;
				}
				int iter2 = 0;
				foreach ( var it in fatParcentList.Select(x => x.Fat) )
				{
					arr.FatPercentValues[iter2] = (float) Math.Round(it, 2);
					iter2++;
				}
				int iter3 = 0;
				foreach ( var it in kbguList )
				{
					arr.KbguValues[iter3] = (float) Math.Round(it.Kbgu, 2);
					arr.ProteinValues[iter3] = (float) Math.Round(it.Protein, 2);
					arr.FatValues[iter3] = (float) Math.Round(it.Fat, 2);
					arr.CabongydrateValues[iter3] = (float) Math.Round(it.Carbohydrate, 2);
					iter3++;
				}

				for ( int i = 0; i < week; i++ )
				{
					arr.date[i] = DateTime.Now.AddDays(i * 7).ToShortDateString();
				}

				lst.WeightValues = weightList;
				lst.KbguValues = kbguList;

				obj.listsResults = lst;
				obj.arrayResults = arr;

				obj.InputHeight = height.ToString();
				obj.InputWeight = initWeight.ToString();
				obj.InputAge = age.ToString();
				obj.InputGender = gender.ToString();
				obj.InputActivity = activity.ToString();
				obj.InputDreamWeight = dream_weight == 0 ? "" : dream_weight.ToString();
				obj.AvailableWeight = criticalWeight == 0 ? "" : criticalWeight.ToString();
				obj.WeeksCount = week.ToString();

				return View(obj);
			}
		}
	}
}