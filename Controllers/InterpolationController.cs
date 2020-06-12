using System.Collections.Generic;
using BodyCore.Data.Models;
using BodyCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using BodyCore.Models;
using System.Linq;
using System;
using System.Globalization;

namespace BodyCore.Controllers
{
	public class InterpolationController : Controller
	{
		List<AgeGenderFatNorm> objList = new List<AgeGenderFatNorm>(){
		new AgeGenderFatNorm(new float[5] { 20f, 24f, 28f, 33f, 39f }, 16, 40, "Женщина"),
		new AgeGenderFatNorm(new float[5] { 22f, 26f, 30f, 34f, 40f }, 40, 60, "Женщина"),
		new AgeGenderFatNorm(new float[5] { 23f, 28f, 32f, 36f, 42f }, 60, 80, "Женщина"),
		new AgeGenderFatNorm(new float[5] { 7f, 11f, 16f, 20f, 25f }, 16, 40, "Мужчина"),
		new AgeGenderFatNorm(new float[5] { 10f, 14f, 18f, 22f, 28f }, 40, 60, "Мужчина"),
		new AgeGenderFatNorm(new float[5] { 12f, 16f, 20f, 25f, 30f }, 60, 80, "Мужчина")};

		[HttpGet]
		public IActionResult Input()
		{
			List<KbguListViewModel> kbguList = new List<KbguListViewModel>() { new KbguListViewModel { Week = 0, Kbgu = 0 } }; 
			List<WeightListViewModel> weightList = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0} };
			List<FatListViewModel> fatPercentList = new List<FatListViewModel>() { new FatListViewModel { Week = 0, Fat = 0 } };

			List<WeightListViewModel> underfatZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<WeightListViewModel> athleticZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<WeightListViewModel> fitZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<WeightListViewModel> healthyZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<WeightListViewModel> overfatZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<WeightListViewModel> obeseZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };

			ListsResults lst = new ListsResults();
			ArrayResults arr = new ArrayResults();
			ResultListViewModel obj = new ResultListViewModel();
			
			lst.WeightValues = weightList;
			lst.KbguValues = kbguList;
			lst.FatValues = fatPercentList;
			lst.UnderfatZone = underfatZone;
			lst.AthleticZone = athleticZone;
			lst.FitZone = fitZone;
			lst.HealthyZone = healthyZone;
			lst.OverfatZone = overfatZone;
			lst.ObeseZone = obeseZone;

			obj.listsResults = lst;
			obj.arrayResults = arr;

			return View(obj);
		}


		[HttpPost]
		public IActionResult Input( bool disclaimer, float height, float weight, float dream_weight, float age, string gender, string activity, float waist, float hips, float neck, bool hardMode )
		{
			if ( !disclaimer )
			{
				List<KbguListViewModel> kbguList = new List<KbguListViewModel>() { new KbguListViewModel { Week = 0, Kbgu = 0 } };
				List<WeightListViewModel> weightList = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
				List<FatListViewModel> fatPercentList = new List<FatListViewModel>() { new FatListViewModel { Week = 0, Fat = 0 } };

				ListsResults lst = new ListsResults();
				ArrayResults arr = new ArrayResults();
				ResultListViewModel obj = new ResultListViewModel();

				lst.WeightValues = weightList;
				lst.KbguValues = kbguList;
				lst.FatValues = fatPercentList;
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

				List<WeightListViewModel> underfatZone = new List<WeightListViewModel>();
				List<WeightListViewModel> athleticZone = new List<WeightListViewModel>();
				List<WeightListViewModel> fitZone = new List<WeightListViewModel>();
				List<WeightListViewModel> healthyZone = new List<WeightListViewModel>();
				List<WeightListViewModel> overfatZone = new List<WeightListViewModel>();
				List<WeightListViewModel> obeseZone = new List<WeightListViewModel>();
				
				AgeGenderFatNorm destObj = new AgeGenderFatNorm();

				float fatPercentDifference = 0;
				weightList.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
				//kbgu = mKBGU.getKBGU(weight, height, age, gender, activity)[0]; //это нормаа калорий для поддержания начального веса
				protein = mKBGU.getKBGU(weight, height, age, gender, activity)[1];
				fat = mKBGU.getKBGU(weight, height, age, gender, activity)[2];
				carbohydrate = mKBGU.getKBGU(weight, height, age, gender, activity)[3];

				if ( hardMode )
				{
					var hard = mKBGU.fatPercentHardMode(height, weight, gender, age, waist, hips, neck);
					var simple = mKBGU.fatPercent(height, weight, gender);
					fatPercentDifference = simple > hard ? - Math.Abs(simple - hard) : Math.Abs(simple - hard);
					fatPercent = hard;
				}
				else
				{
					fatPercent = mKBGU.fatPercent(height, weight, gender);
				}

				foreach ( var iter in objList )
				{
					if ( age >= iter.AgeMin && age < iter.AgeMax && gender == iter.Gender )
					{
						destObj = iter;
						if ( fatPercent <= destObj.FatIntervals[0] )
						{
							underfatZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
							athleticZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							fitZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							healthyZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							overfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							obeseZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						}
						else if ( fatPercent > destObj.FatIntervals[0] && fatPercent <= destObj.FatIntervals[1] )
						{
							underfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							athleticZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
							fitZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							healthyZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							overfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							obeseZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						}
						else if ( fatPercent > destObj.FatIntervals[1] && fatPercent <= destObj.FatIntervals[2] )
						{
							underfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							athleticZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							fitZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
							healthyZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							overfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							obeseZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						}
						else if ( fatPercent > destObj.FatIntervals[2] && fatPercent <= destObj.FatIntervals[3] )
						{
							underfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							athleticZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							fitZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							healthyZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
							overfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							obeseZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						}
						else if ( fatPercent > destObj.FatIntervals[3] && fatPercent <= destObj.FatIntervals[4] )
						{
							underfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							athleticZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							fitZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							healthyZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							overfatZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
							obeseZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						}
						else if ( fatPercent > destObj.FatIntervals[4] )
						{
							underfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							athleticZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							fitZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							healthyZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							overfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
							obeseZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
						}
					}
				}
				fatParcentList.Add(new FatListViewModel { Week = 1, Fat = (float) Math.Round(fatPercent, 2) });
				kbgu = ( 1 - mKBGU.getKBGUrecession(fatPercent, gender) ) * mKBGU.getKBGU(weight, height, age, gender, activity)[0];
				kbguList.Add(new KbguListViewModel { Week = 1, Kbgu = (float)Math.Round(kbgu, 2), Protein = protein, Fat = fat, Carbohydrate = carbohydrate });
				int week = 1;
				do
				{
					week++;

					weight = ( 1 - mKBGU.getWeightRecession(fatPercent, gender) ) * weight;
					weightList.Add(new WeightListViewModel { Week = week, Weight = (float) Math.Round(weight, 2) });

					fatPercent = mKBGU.fatPercent(height, weight, gender) + fatPercentDifference;
					foreach ( var iter in objList )
					{
						if ( age >= iter.AgeMin && age < iter.AgeMax && gender == iter.Gender )
						{
							destObj = iter;
							if ( fatPercent <= destObj.FatIntervals[0] )
							{
								underfatZone.Add(new WeightListViewModel { Week = week, Weight = (float) Math.Round(weight, 2) });
								athleticZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								fitZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								healthyZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								overfatZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								obeseZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
							}
							else if ( fatPercent > destObj.FatIntervals[0] && fatPercent <= destObj.FatIntervals[1] )
							{
								underfatZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								athleticZone.Add(new WeightListViewModel { Week = week, Weight = (float) Math.Round(weight, 2) });
								fitZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								healthyZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								overfatZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								obeseZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
							}
							else if ( fatPercent > destObj.FatIntervals[1] && fatPercent <= destObj.FatIntervals[2] )
							{
								underfatZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								athleticZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								fitZone.Add(new WeightListViewModel { Week = week, Weight = (float) Math.Round(weight, 2) });
								healthyZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								overfatZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								obeseZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
							}
							else if ( fatPercent > destObj.FatIntervals[2] && fatPercent <= destObj.FatIntervals[3] )
							{
								underfatZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								athleticZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								fitZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								healthyZone.Add(new WeightListViewModel { Week = week, Weight = (float) Math.Round(weight, 2) });
								overfatZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								obeseZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
							}
							else if ( fatPercent > destObj.FatIntervals[3] && fatPercent <= destObj.FatIntervals[4] )
							{
								underfatZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								athleticZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								fitZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								healthyZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								overfatZone.Add(new WeightListViewModel { Week = week, Weight = (float) Math.Round(weight, 2) });
								obeseZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
							}
							else if ( fatPercent > destObj.FatIntervals[4] )
							{
								underfatZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								athleticZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								fitZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								healthyZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								overfatZone.Add(new WeightListViewModel { Week = week, Weight = 0 });
								obeseZone.Add(new WeightListViewModel { Week = week, Weight = (float) Math.Round(weight, 2) });
							}
						}
					}
					fatParcentList.Add(new FatListViewModel { Week = week, Fat = (float) Math.Round(fatPercent, 2) });

					kbgu = ( 1 - mKBGU.getKBGUrecession(fatPercent, gender) ) * mKBGU.getKBGU(weight, height, age, gender, activity)[0];
					protein = mKBGU.getKBGU(weight, height, age, gender, activity)[1];
					fat = mKBGU.getKBGU(weight, height, age, gender, activity)[2];
					carbohydrate = mKBGU.getKBGU(weight, height, age, gender, activity)[3];
					kbguList.Add(new KbguListViewModel { Week = week, Kbgu = (float) Math.Round(kbgu, 2), Protein = protein, Fat = fat, Carbohydrate = carbohydrate });

					//расчитываем критическую массу, если процент жира упал до предельной отметки
					if ( (gender == "Женщина" && fatPercent <= 15f) || ( gender == "Мужчина" && fatPercent < 6.5f ))
						criticalWeight = weight;
				}
				while ( ( ( gender == "Женщина" && fatPercent > 15f ) || ( gender == "Мужчина" && fatPercent > 6.5f ) ) && weight > dream_weight );

				//уходит ли график в зону истощения
				float limitWeight = 0;
				foreach (var t in underfatZone )
				{
					if(t.Weight != 0 )
					{
						limitWeight = t.Weight;
						break;
					}
				}

				arr.WeeksValues = new float[week];
				arr.date = new string[week];
				arr.WeightValues = new float[week];
				arr.FatPercentValues = new float[week];
				arr.KbguValues = new int[week];
				arr.ProteinValues = new int[week];
				arr.FatValues = new int[week];
				arr.CabongydrateValues = new int[week];

				int iter0 = 0;
				foreach ( var it in weightList.Select(x => x.Week) )
				{
					arr.WeeksValues[iter0] = it;
					iter0++;
				}
				int iter1 = 0;
				foreach ( var it in weightList.Select(x => x.Weight) )
				{
					arr.WeightValues[iter1] = (float) Math.Round(it, 1);
					iter1++;
				}
				int iter2 = 0;
				foreach ( var it in fatParcentList.Select(x => x.Fat) )
				{
					arr.FatPercentValues[iter2] = (float) Math.Round(it, 1);
					iter2++;
				}
				int iter3 = 0;
				foreach ( var it in kbguList )
				{
					arr.KbguValues[iter3] = (int) Math.Round(Math.Round(it.Kbgu, 2) / 10) * 10;
					arr.ProteinValues[iter3] = (int) Math.Round(Math.Round(it.Protein, 2) / 10) * 10;
					arr.FatValues[iter3] = (int) Math.Round(Math.Round(it.Fat, 2) / 10) * 10;
					arr.CabongydrateValues[iter3] = (int) Math.Round(Math.Round(it.Carbohydrate, 2) / 10) * 10;
					iter3++;
				}

				for ( int i = 0; i < week; i++ )
				{
					var datestr = DateTime.Now.AddDays(i * 7).ToString("dd/MM/yy", CultureInfo.InvariantCulture);
					arr.date[i] = datestr;
				}

				lst.WeightValues = weightList;
				lst.KbguValues = kbguList;
				lst.FatValues = fatParcentList;

				lst.UnderfatZone = underfatZone;
				lst.AthleticZone = athleticZone;
				lst.FitZone = fitZone;
				lst.HealthyZone = healthyZone;
				lst.OverfatZone = overfatZone;
				lst.ObeseZone = obeseZone;

				obj.listsResults = lst;
				obj.arrayResults = arr;

				obj.InputHeight = height.ToString();
				obj.InputWeight = initWeight.ToString();
				obj.InputAge = age.ToString();
				obj.InputGender = gender.ToString();
				obj.InputActivity = activity.ToString();
				obj.InputDreamWeight = dream_weight == 0 ? "" : dream_weight.ToString();
				obj.AvailableWeight = criticalWeight == 0 ? "" : Math.Round(criticalWeight, 2).ToString();
				obj.WeeksCount = week.ToString();
				obj.InUnderfatZone = limitWeight == 0 ? "" : $"Рекомендуем не снижать вес до области истощения. Ваша предельная масса составляет {Math.Round(limitWeight,2)} кг.";
				obj.Anchor = "charts";

				return View(obj);
			}
		}
	}
}