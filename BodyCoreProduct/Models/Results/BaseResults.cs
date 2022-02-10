using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BodyCoreProduct.Models
{
	public class BaseResults
	{
		public const int BODY_TYPES_CNT = 6;
		public string InputHeight { get; set; }
		public string InputWeight { get; set; }
		public string InputAge { get; set; }
		public string InputGender { get; set; }
		public string Recomendations { get; set; }
		public string Anchor { get; set; }
		public Distribution Distribution { get; set; }

		public static readonly Dictionary<BodyType, string> BodyTypeZonesDescrDict = new Dictionary<BodyType, string>()
		{
			{ BodyType.thin, "Истощение" },
			{ BodyType.athletic, "Атлетическое телосложение" },
			{ BodyType.good, "Хорошая физическая форма" },
			{ BodyType.average, "Средний уровень жира" },
			{ BodyType.excess, "Наличие лишнего веса" },
			{ BodyType.extra, "Ожирение" },
		};

		protected const string ExcessVisceralFat = "Превышен уровень висцеральной жировой ткани";
		protected const string AnalizeDistributionName = "Типы телосложения в зависимости от % жировой ткани " +
			"в организме для людей данного пола и возраста";
		protected const string InterpltnDistributionName = "Ваш вес в зависимости от недели";

		protected static readonly ObservableCollection<AgeGenderClassification> PersonCategories =
			new ObservableCollection<AgeGenderClassification>()
		{
			new AgeGenderClassification()
			{
				Gender = Gender.female,
				FatIntervals = new float[BODY_TYPES_CNT + 1] { 0f, 20f, 24f, 28f, 33f, 39f, 100f },
				AgeMin = 16,
				AgeMax = 40
			},
			new AgeGenderClassification()
			{
				Gender = Gender.female,
				FatIntervals = new float[BODY_TYPES_CNT + 1] { 0f, 22f, 26f, 30f, 34f, 40f, 100f },
				AgeMin = 40,
				AgeMax = 60,
			},
			new AgeGenderClassification()
			{
				Gender = Gender.female,
				FatIntervals = new float[BODY_TYPES_CNT + 1] { 0f, 23f, 28f, 32f, 36f, 42f, 100f },
				AgeMin = 60,
				AgeMax = 80,
			},
			new AgeGenderClassification()
			{
				Gender = Gender.male,
				FatIntervals = new float[BODY_TYPES_CNT + 1] { 0f, 7f, 11f, 16f, 20f, 25f, 100f },
				AgeMin = 16,
				AgeMax = 40
			},
			new AgeGenderClassification()
			{
				Gender = Gender.male,
				FatIntervals = new float[BODY_TYPES_CNT + 1] { 0f, 10f, 14f, 18f, 22f, 28f, 100f },
				AgeMin = 40,
				AgeMax = 60,
			},
			new AgeGenderClassification()
			{
				Gender = Gender.male,
				FatIntervals = new float[BODY_TYPES_CNT + 1] { 0f, 12f, 16f, 20f, 25f, 30f, 100f },
				AgeMin = 60,
				AgeMax = 80,
			}
		};

		protected static readonly Dictionary<string, Gender> GenderDict = new Dictionary<string, Gender>()
		{
			{ "Женщина", Gender.female },
			{ "Мужчина", Gender.male }
		};

		protected static readonly Dictionary<string, Activity> ActivityDict = new Dictionary<string, Activity>()
		{
			{ "Минимальная", Activity.verySmall },
			{ "Небольшая", Activity.small },
			{ "Средняя", Activity.average },
			{ "Выше среднего", Activity.aboveAverage },
			{ "Повышенная", Activity.increased },
			{ "Высокая", Activity.high },
			{ "Очень высокая", Activity.veryHigh }
		};

		protected static readonly Dictionary<BodyType, BodyTypeAttributes> ResultDict = new Dictionary<BodyType, BodyTypeAttributes>()
		{
			{ BodyType.thin, new BodyTypeAttributes(){
				Name = BodyTypeZonesDescrDict[BodyType.thin],
				Color = "rgba(123,104,238,0.4)",
				Recomendations = "Не рекомендуется снижать вес, т.к. у вас низкий показатель содержания жира в организме."}  
			},
			{ BodyType.athletic, new BodyTypeAttributes(){
				Name = BodyTypeZonesDescrDict[BodyType.athletic],
				Color = "rgba(30,144,255,0.4)",
				Recomendations = "Желательно не снижать вес. Ваше тело имеет низкий процент абдоминальной жировой ткани." }
			},
			{ BodyType.good, new BodyTypeAttributes(){
				Name = BodyTypeZonesDescrDict[BodyType.good],
				Color = "rgba(60,179,113,0.4)",
				Recomendations = "Данный показатель является нормой. " }
			},
			{ BodyType.average, new BodyTypeAttributes(){
				Name = BodyTypeZonesDescrDict[BodyType.average],
				Color = "rgba(255,215,0,0.4)",
				Recomendations = "Показатель слегка превышает норму" +
				" Вы можете немного подкорректировать показатель, воспользовавшись нашим помощником по снижению веса.",
				 }
			},
			{ BodyType.excess, new BodyTypeAttributes(){
				Name = BodyTypeZonesDescrDict[BodyType.excess],
				Color = "rgba(255,69,0,0.4)",
				Recomendations = "Показатель завышен. Мы создали инструмент для составления индивидуального плана безопасного" +
				" и комфортного снижения веса. Если вы посчитаете нужным, то сможете воспользоваться данным инструментом",
				 }
			},
			{ BodyType.extra, new BodyTypeAttributes(){
				Name = BodyTypeZonesDescrDict[BodyType.extra],
				Color = "rgba(255,0,0,0.1)",
				Recomendations = "Показатель значительно превышает норму. Однако, вам не стоит отчаиваться." +
				" Потому что на этом сайте есть инструмент для составления индивидуального плана безопасного" +
				" и комфортного снижения веса. Если вы посчитаете нужным, то сможете воспользоваться данным инструментом",
				}
			}
		};

		protected Distribution GetDistribution(Dictionary<BodyType, BodyTypeAttributes> result)
		{
			var distribution = new Distribution
			{
				Measurements = new List<Measurements>()
			};

			for (int i = 0; i < BODY_TYPES_CNT; i++)
			{
				BodyType enm = (BodyType)i;
				distribution.Measurements.Add(new Measurements() { BodyTypeName = result[enm].Name, Color = result[enm].Color });
			}
			return distribution;
		}
	}
}
