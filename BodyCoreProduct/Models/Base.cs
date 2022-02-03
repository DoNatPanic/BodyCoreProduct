using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BodyCoreProduct.Models
{
	public enum Gender
	{
		male,
		female
	}

	public enum BodyTypeZone
	{
		thin,
		athletic,
		good,
		average,
		excess,
		extra
	}

	public enum Activity
	{
		male,
		female
	}

	public class Base
	{
		public string InputHeight { get; set; }
		public string InputWeight { get; set; }
		public string InputAge { get; set; }
		public string InputGender { get; set; }
		public string Recomendations { get; set; }

		protected string ExcessVisceralFat = "Превышен уровень висцеральной жировой ткани.";
		protected string DistributionName = "Распределение";

		protected ObservableCollection<AgeGenderClassification> PersonCategories = new ObservableCollection<AgeGenderClassification>(){
			new AgeGenderClassification(new float[5] { 20f, 24f, 28f, 33f, 39f }, 16, 40, Gender.female),
			new AgeGenderClassification(new float[5] { 22f, 26f, 30f, 34f, 40f }, 40, 60, Gender.female),
			new AgeGenderClassification(new float[5] { 23f, 28f, 32f, 36f, 42f }, 60, 80, Gender.female),
			new AgeGenderClassification(new float[5] { 7f, 11f, 16f, 20f, 25f }, 16, 40, Gender.male),
			new AgeGenderClassification(new float[5] { 10f, 14f, 18f, 22f, 28f }, 40, 60, Gender.male),
			new AgeGenderClassification(new float[5] { 12f, 16f, 20f, 25f, 30f }, 60, 80, Gender.male)
			};

		protected Dictionary<string, Gender> GenderDict = new Dictionary<string, Gender>()
		{
			{ "Женщина", Gender.female },
			{ "Мужчина", Gender.male }
		};

		protected Dictionary<BodyTypeZone, CategoryAttributes> Result = new Dictionary<BodyTypeZone, CategoryAttributes>()
		{
			{ BodyTypeZone.thin, new CategoryAttributes(){
				Name = "Истощение",
				Color = "rgba(123,104,238,0.4)",
				Recomendations = "Рассчеты показали, что Вам крайне не рекомендуется снижать вес. Очень низкий процент абдоминальной жировой ткани в организме."}  },
			{ BodyTypeZone.athletic, new CategoryAttributes(){
				Name = "Атлетическое телосложение",
				Color = "rgba(30,144,255,0.4)",
				Recomendations = "У Вас прекрасная форма, желательно не снижать вес. Ваше тело имеет низкий процент абдоминальной жировой ткани." }
			},
			{ BodyTypeZone.good, new CategoryAttributes(){
				Name = "Хорошая физическая форма",
				Color = "rgba(60,179,113,0.4)",
				Recomendations = "У Вас хорошая форма, можно не снижать вес. Нормальный показатель абдоминальной жировой такни в организме." }
			},
			{ BodyTypeZone.average, new CategoryAttributes(){
				Name = "Средний уровень жира",
				Color = "rgba(255,215,0,0.4)",
				Recomendations = "Рассчеты показали, что у Вас легка превышен уровень абдоминальной жировой ткани в организме." +
				" Вы можете немного подкорректировать показатель, воспользовавшись нашим помощником по снижению веса.",
				 }
			},
			{ BodyTypeZone.excess, new CategoryAttributes(){
				Name = "Наличие лишнего веса",
				Color = "rgba(255,69,0,0.4)",
				Recomendations = "Рассчеты показали, что процент абдоминальной жировой ткани в организме завышен. Но не стоит отчаиваться!" +
				" Потому что мы можем Вам помочь. Здесь Вы сможете составить индивидуальный план безопасного и комфортного снижения веса.",
				 }
			},
			{ BodyTypeZone.extra, new CategoryAttributes(){
				Name = "Ожирение",
				Color = "rgba(255,0,0,0.4)",
				Recomendations = "Рассчеты показали, что в организме высокий процент абдоминальной жировой ткани. Однако, не стоит отчаиваться!" +
				" Потому что Вы можете подкорректировать форму, воспользовавшись нашим помощником по снижению веса." +
				" Он составит Вам индивидуальный план безопасного и комфортного снижения веса."
				}
			}
		};
	}
}
