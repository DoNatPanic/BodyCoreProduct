
using System;
using System.Collections.Generic;
using BodyCore.Data.Models;
using BodyCore.Models;
using BodyCore.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BodyCore.Controllers
{
	public class AnalizeController : Controller
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
			new AgeGenderFatNorm(new float[5] { 12f, 16f, 20f, 25f, 30f }, 60, 80, "Мужчина")
	};

		[HttpGet]
		public IActionResult GetAnalize()
		{
			CommonReportViewModel commonLst = new CommonReportViewModel();
			var lstModel = new List<StackedViewModel>();
			lstModel.Add(new StackedViewModel
			{
				StackedDimensionOne = "",
				LstData = new List<SimpleReportViewModel>(){
				new SimpleReportViewModel(){DimensionOne = "", Quantity = 0, ColorRGB = "rgba(0,0,0,0)"}}
			});
			commonLst.CommonModel = lstModel;
			return View(commonLst);
		}



		[HttpPost]
		public IActionResult GetAnalize(string gender, float age, float waist, float hips, float neck,
			float height, float weight, bool hardMode)
		{
			AgeGenderFatNorm destObj = new AgeGenderFatNorm() ;
			string result = "";
			string colorRGB = "";
			string recomendations = "";
			float fatPercent = 0;
			string linkVisibility = "hidden";
			string visceralFatConclusion = "";
			KBGU mKBGU = new KBGU();


			if ( hardMode )
			{
				fatPercent = mKBGU.fatPercentHardMode(height, weight, gender, age, waist, hips, neck);
				float visceralFat = waist / hips;
				visceralFatConclusion = " Уровень висцеральной жировой ткани не превышен";
				if ( (gender == "Мужчина" && visceralFat > 1f) || ( gender == "Женщина" && visceralFat > 0.85f ) )
					visceralFatConclusion = " Уровень висцеральной жировой ткани превышен.";
			}
			else
			{
				fatPercent = mKBGU.fatPercent(height, weight, gender);
			}

			foreach (var iter in objList )
			{
				if ( age >= iter.AgeMin && age < iter.AgeMax && gender == iter.Gender )
				{
					destObj = iter;
					if ( fatPercent <= destObj.FatIntervals[0] )
					{
						result = resultsName[0];
						colorRGB = resultsColors[0];
						recomendations = resultsRecomendations[0];
						linkVisibility = "hidden";

					}
					else if ( fatPercent > destObj.FatIntervals[0] && fatPercent <= destObj.FatIntervals[1] )
					{
						result = resultsName[1];
						colorRGB = resultsColors[1];
						recomendations = resultsRecomendations[1];
						linkVisibility = "hidden";
					}
					else if ( fatPercent > destObj.FatIntervals[1] && fatPercent <= destObj.FatIntervals[2] )
					{
						result = resultsName[2];
						colorRGB = resultsColors[2];
						recomendations = resultsRecomendations[2];
						linkVisibility = "hidden";
					}
					else if ( fatPercent > destObj.FatIntervals[2] && fatPercent <= destObj.FatIntervals[3] )
					{
						result = resultsName[3];
						colorRGB = resultsColors[3];
						recomendations = resultsRecomendations[3];
						linkVisibility = "visible";
					}
					else if ( fatPercent > destObj.FatIntervals[3] && fatPercent <= destObj.FatIntervals[4] )
					{
						result = resultsName[4];
						colorRGB = resultsColors[4];
						recomendations = resultsRecomendations[4];
						linkVisibility = "visible";
					}
					else if ( fatPercent > destObj.FatIntervals[4] )
					{
						result = resultsName[5];
						colorRGB = resultsColors[5];
						recomendations = resultsRecomendations[5];
						linkVisibility = "visible";
					}
				}
			}

			CommonReportViewModel commonLst = new CommonReportViewModel();
			
			var lstModel = new List<StackedViewModel>();
			lstModel.Add( new StackedViewModel{StackedDimensionOne = "Процент жира", LstData = new List<SimpleReportViewModel>(){
				new SimpleReportViewModel(){DimensionOne = resultsName[0], Quantity = destObj.FatIntervals[0] - 0f, ColorRGB = resultsColors[0] },
				new SimpleReportViewModel(){DimensionOne = resultsName[1], Quantity = destObj.FatIntervals[1] - destObj.FatIntervals[0], ColorRGB = resultsColors[1] },
				new SimpleReportViewModel(){DimensionOne = resultsName[2], Quantity = destObj.FatIntervals[2] - destObj.FatIntervals[1], ColorRGB = resultsColors[2] },
				new SimpleReportViewModel(){DimensionOne = resultsName[3], Quantity = destObj.FatIntervals[3] - destObj.FatIntervals[2], ColorRGB = resultsColors[3] },
				new SimpleReportViewModel(){DimensionOne = resultsName[4], Quantity = destObj.FatIntervals[4] - destObj.FatIntervals[3], ColorRGB = resultsColors[4] },
				new SimpleReportViewModel(){DimensionOne = resultsName[5], Quantity = 100f - destObj.FatIntervals[4], ColorRGB = resultsColors[5] }}
			} );

			//lstModel.Add();
			commonLst.CommonModel = lstModel;
			commonLst.Conclusion = $"У вас наблюдается {result.ToLower()}, процент жира: {Math.Round(fatPercent, 2)}";
			commonLst.InputGender = gender.ToString();
			commonLst.InputAge = age.ToString();
			commonLst.InputWaist = waist == 0 ? "" : waist.ToString();
			commonLst.InputHips = hips == 0 ? "" : hips.ToString();
			commonLst.InputNeck = neck == 0 ? "" : neck.ToString();
			commonLst.InputHeight = height.ToString();
			commonLst.InputWeight = weight.ToString();
			commonLst.ChartName = $"Данные для {gender.ToLower().TrimEnd('а')} вашей возрастной категории";
			commonLst.ConclusionColor = colorRGB;
			commonLst.Recomendations = recomendations;
			commonLst.ViscellarFatConclusion = visceralFatConclusion;
			commonLst.LinkVisibility = linkVisibility;
			commonLst.ChartVisibility = "visible";
			commonLst.Anchor = "charts";
			return View(commonLst);
		}
	}
}