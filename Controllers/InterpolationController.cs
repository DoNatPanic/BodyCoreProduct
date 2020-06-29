using System.Collections.Generic;
using BodyCore.Data.Models;
using BodyCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using BodyCore.Models;
using System.Linq;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using BodyCore.Models.Email;
using java.io;
using java.nio.charset;
using System.Text;

namespace BodyCore.Controllers
{
	public class InterpolationController : Controller
	{
		private static string[] resultsName = new string[6]
		{ "Истощение", "Атлетическое телосложение", "Хорошая физическая форма",
				"Средний уровень жира", "Наличие лишнего веса", "Ожирение"};
		private static string[] resultsColors = new string[6]
		{"rgba(123,104,238,0.4)", "rgba(30,144,255,0.4)", "rgba(60,179,113,0.4)",
				"rgba(255,215,0,0.4)", "rgba(255,69,0,0.4)", "rgba(255,0,0,0.4)"};

		private static string[] resultsRecomendations = new string[6]
		{"Рассчеты показали, что Вам крайне не рекомендуется снижать вес. Очень низкий процент абдоминальной жировой ткани в организме.",
				"У Вас прекрасная форма, желательно не снижать вес. Ваше тело имеет низкий процент абдоминальной жировой ткани.",
				"У Вас хорошая форма, можно не снижать вес. Нормальный показатель абдоминальной жировой такни в организме.",
				"Рассчеты показали, что у Вас легка превышен уровень абдоминальной жировой ткани в организме. Вы можете немного подкорректировать показатель, воспользовавшись нашим помощником по снижению веса.",
				"Рассчеты показали, что процент абдоминальной жировой ткани в организме завышен. Но не стоит отчаиваться! Потому что мы можем Вам помочь. Здесь Вы сможете составить индивидуальный план безопасного и комфортного снижения веса.",
				"Рассчеты показали, что в организме высокий процент абдоминальной жировой ткани. Однако, не стоит отчаиваться! Потому что Вы можете подкорректировать форму, воспользовавшись нашим помощником по снижению веса. Он составит Вам индивидуальный план безопасного и комфортного снижения веса."};

		List<AgeGenderFatNorm> objList = new List<AgeGenderFatNorm>(){
		new AgeGenderFatNorm(new float[5] { 20f, 24f, 28f, 33f, 39f }, 16, 40, "Женщина"),
		new AgeGenderFatNorm(new float[5] { 22f, 26f, 30f, 34f, 40f }, 40, 60, "Женщина"),
		new AgeGenderFatNorm(new float[5] { 23f, 28f, 32f, 36f, 42f }, 60, 80, "Женщина"),
		new AgeGenderFatNorm(new float[5] { 7f, 11f, 16f, 20f, 25f }, 16, 40, "Мужчина"),
		new AgeGenderFatNorm(new float[5] { 10f, 14f, 18f, 22f, 28f }, 40, 60, "Мужчина"),
		new AgeGenderFatNorm(new float[5] { 12f, 16f, 20f, 25f, 30f }, 60, 80, "Мужчина")};


		[HttpGet]
		public async Task<IActionResult> Input()
		{
			WeightLossViewModel model = new WeightLossViewModel();
			var lstModel = new List<StackedViewModel>();
			lstModel.Add(new StackedViewModel
			{
				StackedDimensionOne = "",
				LstData = new List<SimpleReportViewModel>(){
				new SimpleReportViewModel(){DimensionOne = "", Quantity = 0, ColorRGB = "rgba(0,0,0,0)"}}
			});
			model.CommonModel = lstModel;

			List<KbguListViewModel> kbguList = new List<KbguListViewModel>() { new KbguListViewModel { Week = 0, Kbgu = 0 } };
			List<WeightListViewModel> weightList = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<FatListViewModel> fatPercentList = new List<FatListViewModel>() { new FatListViewModel { Week = 0, Fat = 0 } };

			List<WeightListViewModel> underfatZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<WeightListViewModel> athleticZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<WeightListViewModel> fitZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<WeightListViewModel> healthyZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<WeightListViewModel> overfatZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };
			List<WeightListViewModel> obeseZone = new List<WeightListViewModel>() { new WeightListViewModel { Week = 0, Weight = 0 } };

			ListsResults lst = new ListsResults();
			ArrayResults arr = new ArrayResults();

			lst.WeightValues = weightList;
			lst.KbguValues = kbguList;
			lst.FatValues = fatPercentList;
			lst.UnderfatZone = underfatZone;
			lst.AthleticZone = athleticZone;
			lst.FitZone = fitZone;
			lst.HealthyZone = healthyZone;
			lst.OverfatZone = overfatZone;
			lst.ObeseZone = obeseZone;

			model.listsResults = lst;
			model.arrayResults = arr;

			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> Input( float height, float weight, float dream_weight, float age, string gender, string activity, float waist, float hips, float neck, bool hardMode, bool sendemail )
		{
			WeightLossViewModel model = new WeightLossViewModel();
			string result = "";
			string colorRGB = "";
			string recomendations = "";
			float initFatPercent = 0;
			float keepWeightKkal = 0;
			float fatPercent = 0;
			string visceralFatConclusion = "";

			float initWeight = weight;
			float kbgu;
			float protein;
			float fat;
			float carbohydrate;
			float criticalWeight = 0f;

			KBGU mKBGU = new KBGU();

			AgeGenderFatNorm destObj = new AgeGenderFatNorm();

			List<KbguListViewModel> kbguList = new List<KbguListViewModel>();
			List<WeightListViewModel> weightList = new List<WeightListViewModel>();
			List<FatListViewModel> fatParcentList = new List<FatListViewModel>();
			ArrayResults arr = new ArrayResults();
			ListsResults lst = new ListsResults();

			List<WeightListViewModel> underfatZone = new List<WeightListViewModel>();
			List<WeightListViewModel> athleticZone = new List<WeightListViewModel>();
			List<WeightListViewModel> fitZone = new List<WeightListViewModel>();
			List<WeightListViewModel> healthyZone = new List<WeightListViewModel>();
			List<WeightListViewModel> overfatZone = new List<WeightListViewModel>();
			List<WeightListViewModel> obeseZone = new List<WeightListViewModel>();

			float fatPercentDifference = 0;
			weightList.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
			keepWeightKkal = mKBGU.getKBGU(weight, height, age, gender, activity)[0]; //это норма калорий для поддержания начального веса
			keepWeightKkal = (int) Math.Round(Math.Round(keepWeightKkal, 2) / 10) * 10; //округляем до десятков
			protein = mKBGU.getKBGU(weight, height, age, gender, activity)[1];
			fat = mKBGU.getKBGU(weight, height, age, gender, activity)[2];
			carbohydrate = mKBGU.getKBGU(weight, height, age, gender, activity)[3];


			if ( hardMode )
			{
				var hard = mKBGU.fatPercentHardMode(height, weight, gender, age, waist, hips, neck);
				var simple = mKBGU.fatPercent(height, weight, gender);
				fatPercentDifference = simple > hard ? -Math.Abs(simple - hard) : Math.Abs(simple - hard);
				fatPercent = hard;

				float visceralFat = waist / hips;
				visceralFatConclusion = " Уровень висцеральной жировой ткани не превышен";
				if ( ( gender == "Мужчина" && visceralFat > 1f ) || ( gender == "Женщина" && visceralFat > 0.85f ) )
					visceralFatConclusion = " Уровень висцеральной жировой ткани превышен.";
			}
			else
			{
				fatPercent = mKBGU.fatPercent(height, weight, gender);
			}

			initFatPercent = fatPercent;

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
						result = resultsName[0];
						colorRGB = resultsColors[0];
						recomendations = resultsRecomendations[0];
					}
					else if ( fatPercent > destObj.FatIntervals[0] && fatPercent <= destObj.FatIntervals[1] )
					{
						underfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						athleticZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
						fitZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						healthyZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						overfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						obeseZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						result = resultsName[1];
						colorRGB = resultsColors[1];
						recomendations = resultsRecomendations[1];
					}
					else if ( fatPercent > destObj.FatIntervals[1] && fatPercent <= destObj.FatIntervals[2] )
					{
						underfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						athleticZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						fitZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
						healthyZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						overfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						obeseZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						result = resultsName[2];
						colorRGB = resultsColors[2];
						recomendations = resultsRecomendations[2];
					}
					else if ( fatPercent > destObj.FatIntervals[2] && fatPercent <= destObj.FatIntervals[3] )
					{
						underfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						athleticZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						fitZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						healthyZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
						overfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						obeseZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						result = resultsName[3];
						colorRGB = resultsColors[3];
						recomendations = resultsRecomendations[3];
					}
					else if ( fatPercent > destObj.FatIntervals[3] && fatPercent <= destObj.FatIntervals[4] )
					{
						underfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						athleticZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						fitZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						healthyZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						overfatZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
						obeseZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						result = resultsName[4];
						colorRGB = resultsColors[4];
						recomendations = resultsRecomendations[4];
					}
					else if ( fatPercent > destObj.FatIntervals[4] )
					{
						underfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						athleticZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						fitZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						healthyZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						overfatZone.Add(new WeightListViewModel { Week = 1, Weight = 0 });
						obeseZone.Add(new WeightListViewModel { Week = 1, Weight = (float) Math.Round(weight, 2) });
						result = resultsName[5];
						colorRGB = resultsColors[5];
						recomendations = resultsRecomendations[5];
					}
				}
			}
			fatParcentList.Add(new FatListViewModel { Week = 1, Fat = (float) Math.Round(fatPercent, 2) });
			kbgu = ( 1 - mKBGU.getKBGUrecession(fatPercent, gender) ) * mKBGU.getKBGU(weight, height, age, gender, activity)[0];
			kbguList.Add(new KbguListViewModel { Week = 1, Kbgu = (float) Math.Round(kbgu, 2), Protein = protein, Fat = fat, Carbohydrate = carbohydrate });
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
				if ( ( gender == "Женщина" && fatPercent <= 15f ) || ( gender == "Мужчина" && fatPercent < 6.5f ) )
					criticalWeight = weight;
			}
			while ( ( ( gender == "Женщина" && fatPercent > 15f ) || ( gender == "Мужчина" && fatPercent > 6.5f ) ) && weight > dream_weight );

			//уходит ли график в зону истощения
			float limitWeight = 0;
			foreach ( var t in underfatZone )
			{
				if ( t.Weight != 0 )
				{
					limitWeight = (float) Math.Round(t.Weight, 0);
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

			var lstModel = new List<StackedViewModel>();
			lstModel.Add(new StackedViewModel
			{
				StackedDimensionOne = "Процент жира",
				LstData = new List<SimpleReportViewModel>(){
				new SimpleReportViewModel(){DimensionOne = resultsName[0], Quantity = destObj.FatIntervals[0] - 0f, ColorRGB = resultsColors[0] },
				new SimpleReportViewModel(){DimensionOne = resultsName[1], Quantity = destObj.FatIntervals[1] - destObj.FatIntervals[0], ColorRGB = resultsColors[1] },
				new SimpleReportViewModel(){DimensionOne = resultsName[2], Quantity = destObj.FatIntervals[2] - destObj.FatIntervals[1], ColorRGB = resultsColors[2] },
				new SimpleReportViewModel(){DimensionOne = resultsName[3], Quantity = destObj.FatIntervals[3] - destObj.FatIntervals[2], ColorRGB = resultsColors[3] },
				new SimpleReportViewModel(){DimensionOne = resultsName[4], Quantity = destObj.FatIntervals[4] - destObj.FatIntervals[3], ColorRGB = resultsColors[4] },
				new SimpleReportViewModel(){DimensionOne = resultsName[5], Quantity = 100f - destObj.FatIntervals[4], ColorRGB = resultsColors[5] }}
			});

			lst.WeightValues = weightList;
			lst.KbguValues = kbguList;
			lst.FatValues = fatParcentList;
			lst.UnderfatZone = underfatZone;
			lst.AthleticZone = athleticZone;
			lst.FitZone = fitZone;
			lst.HealthyZone = healthyZone;
			lst.OverfatZone = overfatZone;
			lst.ObeseZone = obeseZone;

			model.CommonModel = lstModel;
			model.Conclusion = $"У вас наблюдается {result.ToLower()}, процент жира: {Math.Round(initFatPercent, 2)}";
			model.ChartName = $"Данные для {gender.ToLower().TrimEnd('а')} вашей возрастной категории";
			model.ConclusionColor = colorRGB;
			model.Recomendations = recomendations;
			model.KeepWeightKkal = $"Для поддержания текущей массы тела калорийность вашего рациона должна составлять {keepWeightKkal} ккал.";
			model.ViscellarFatConclusion = visceralFatConclusion;
			model.Anchor = "charts";

			model.listsResults = lst;
			model.arrayResults = arr;
			model.InputHeight = height.ToString();
			model.InputWeight = initWeight.ToString();
			model.InputAge = age.ToString();
			model.InputGender = gender.ToString();
			model.InputActivity = activity.ToString();
			model.InputDreamWeight = dream_weight == 0 ? "" : dream_weight.ToString();
			model.InputWaist = waist == 0 ? "" : waist.ToString();
			model.InputHips = hips == 0 ? "" : hips.ToString();
			model.InputNeck = neck == 0 ? "" : neck.ToString();
			model.CriticalWeight = criticalWeight == 0 ? "" : Math.Round(criticalWeight, 0).ToString();
			model.LimitWeight = limitWeight == 0 ? "" : limitWeight.ToString();
			model.WeeksCount = week.ToString();
			model.InUnderfatZone = limitWeight == 0 ? "" : $"Рекомендуем не снижать вес до области истощения (левая граница фиолетовой зоны на графике ниже), что составляет {limitWeight} кг. " +
				"Истощение может оказывать негативное влияние на организм: приводить к снижению иммунитета и сокращению физической и умственной активности";
			model.Anchor = "charts";

			/*string post = "";
			if ( sendemail )
			{
				post = User.Claims.ToArray()[1].Value;
				Message message;
				EmailSender _emailSender = new EmailSender();
				using ( MemoryStream stream = new MemoryStream() )
				{
					var table = $"<table><thead><tr><th>Week</th><th>Date</th><th>Weight(кг)</th><th>Kkal</th><th>Proteins (g)</th><th>Fats (g)</th><th>Cabongydrates (g)</th><th>% of fat</th></tr></thead><tbody>";
					
					for (int i = 0;  i < model.arrayResults.KbguValues.Length; i++ )
					{
						var row = $"<tr><td>{model.arrayResults.WeeksValues[i]}</td>" +
						$"<td>{model.arrayResults.date[i]}</td><td>{model.arrayResults.WeightValues[i]}</td><td>{model.arrayResults.KbguValues[i]}</td>" +
						$"<td>{model.arrayResults.ProteinValues[i]}</td><td>{model.arrayResults.ProteinValues[i]}</td><td>{model.arrayResults.FatValues[i]}</td>" +
						$"<td>{model.arrayResults.CabongydrateValues[i]}</td><td>{model.arrayResults.FatPercentValues[i]}</td></tr>";
						table += row;
					}
					table += "</tbody></table>";


				System.IO.StringReader reader = new System.IO.StringReader("table");
				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
				Document PdfFile = new Document(PageSize.A4);
				PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
				PdfFile.Open();
				XMLWorkerHelper.GetInstance().ParseXHtml(writer, PdfFile, reader);
				PdfFile.Close();
				

				var file = File(stream.ToArray(), "application/pdf", "Results.pdf");
				message = new Message(new string[] { post }, "План здорового снижения веса",
				$"<p>Добрый день!</p><p>Мы составили для вас план снижения веса. Документ можно найти во вложениях к этому письму." +
				" Помните, что прогноз является примерным, и может незначительно отличаться от реальных результатов в" +
				" зависимостимости от индивидуальных особенностей организма.</p><p>Ждем вас снова на нашем сайте" +
				" <a href='http://healthyweight.ru'>http://healthyweight.ru</a> и желаем успехов в достижении результатов!</p>", file);

				}
				await _emailSender.SendEmailAsync(message);
			}*/
			return View(model);
		}
	}
}