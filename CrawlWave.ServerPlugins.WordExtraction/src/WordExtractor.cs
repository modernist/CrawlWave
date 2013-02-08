using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CrawlWave.Common;

namespace CrawlWave.ServerPlugins.WordExtraction
{
	/// <summary>
	/// WordExtractor is a Singleton class that will be used during the extraction of words
	/// from the HTML content of a web page or a text file.
	/// </summary>
	public class WordExtractor
	{
		#region Private Members

		private static WordExtractor instance;
		private PluginSettings settings;
		private Mutex mutex;
		private Regex regStripTags, regStripScripts, regTitleTags, regDescriptionTags, regKeywordTags;
		private Stemming stemming;
		private SortedList slStopWordsEnglish;
		private SortedList slStopWordsGreek;
		private CultureInfo culture;
		private static char []Delimiters=new char[]{' ',',','=','+','.',':','-','_','/','\\','{','}','\"','\'','!','@','#','$','%','^','&','─','ё','╔','╘','╝','≥','*','(',')','[',']','|','~','`','║',';','<','>','?','╚','╩','\t','\n','\r'};
		private static readonly string[] arStopWordsEnglish = new string[]
		{
			"A",
			"A'S",
			"ABLE",
			"ABOUT",
			"ABOVE",
			"ACCORDING",
			"ACCORDINGLY",
			"ACROSS",
			"ACTUALLY",
			"AFTER",
			"AFTERWARDS",
			"AGAIN",
			"AGAINST",
			"AIN'T",
			"ALL",
			"ALLOW",
			"ALLOWS",
			"ALMOST",
			"ALONE",
			"ALONG",
			"ALREADY",
			"ALSO",
			"ALTHOUGH",
			"ALWAYS",
			"AM",
			"AMONG",
			"AMONGST",
			"AN",
			"AND",
			"ANOTHER",
			"ANY",
			"ANYBODY",
			"ANYHOW",
			"ANYONE",
			"ANYTHING",
			"ANYWAY",
			"ANYWAYS",
			"ANYWHERE",
			"APART",
			"APPEAR",
			"APPRECIATE",
			"APPROPRIATE",
			"ARE",
			"AREN'T",
			"AROUND",
			"AS",
			"ASIDE",
			"ASK",
			"ASKING",
			"ASSOCIATED",
			"AT",
			"AVAILABLE",
			"AWAY",
			"AWFULLY",
			"B",
			"BE",
			"BECAME",
			"BECAUSE",
			"BECOME",
			"BECOMES",
			"BECOMING",
			"BEEN",
			"BEFORE",
			"BEFOREHAND",
			"BEHIND",
			"BEING",
			"BELIEVE",
			"BELOW",
			"BESIDE",
			"BESIDES",
			"BEST",
			"BETTER",
			"BETWEEN",
			"BEYOND",
			"BOTH",
			"BRIEF",
			"BUT",
			"BY",
			"C",
			"C'MON",
			"C'S",
			"CAME",
			"CAN",
			"CAN'T",
			"CANNOT",
			"CANT",
			"CAUSE",
			"CAUSES",
			"CERTAIN",
			"CERTAINLY",
			"CHANGES",
			"CLEARLY",
			"CO",
			"COM",
			"COME",
			"COMES",
			"CONCERNING",
			"CONSEQUENTLY",
			"CONSIDER",
			"CONSIDERING",
			"CONTAIN",
			"CONTAINING",
			"CONTAINS",
			"CORRESPONDING",
			"COULD",
			"COULDN'T",
			"COURSE",
			"CURRENTLY",
			"D",
			"DEFINITELY",
			"DESCRIBED",
			"DESPITE",
			"DID",
			"DIDN'T",
			"DIFFERENT",
			"DO",
			"DOES",
			"DOESN'T",
			"DOING",
			"DON'T",
			"DONE",
			"DOWN",
			"DOWNWARDS",
			"DURING",
			"E",
			"EACH",
			"EDU",
			"EG",
			"EIGHT",
			"EITHER",
			"ELSE",
			"ELSEWHERE",
			"ENOUGH",
			"ENTIRELY",
			"ESPECIALLY",
			"ET",
			"ETC",
			"EVEN",
			"EVER",
			"EVERY",
			"EVERYBODY",
			"EVERYONE",
			"EVERYTHING",
			"EVERYWHERE",
			"EX",
			"EXACTLY",
			"EXAMPLE",
			"EXCEPT",
			"F",
			"FAR",
			"FEW",
			"FIFTH",
			"FIRST",
			"FIVE",
			"FOLLOWED",
			"FOLLOWING",
			"FOLLOWS",
			"FOR",
			"FORMER",
			"FORMERLY",
			"FORTH",
			"FOUR",
			"FROM",
			"FURTHER",
			"FURTHERMORE",
			"G",
			"GET",
			"GETS",
			"GETTING",
			"GIVEN",
			"GIVES",
			"GO",
			"GOES",
			"GOING",
			"GONE",
			"GOT",
			"GOTTEN",
			"GREETINGS",
			"H",
			"HAD",
			"HADN'T",
			"HAPPENS",
			"HARDLY",
			"HAS",
			"HASN'T",
			"HAVE",
			"HAVEN'T",
			"HAVING",
			"HE",
			"HE'S",
			"HELLO",
			"HELP",
			"HENCE",
			"HER",
			"HERE",
			"HERE'S",
			"HEREAFTER",
			"HEREBY",
			"HEREIN",
			"HEREUPON",
			"HERS",
			"HERSELF",
			"HI",
			"HIM",
			"HIMSELF",
			"HIS",
			"HITHER",
			"HOPEFULLY",
			"HOW",
			"HOWBEIT",
			"HOWEVER",
			"I",
			"I'D",
			"I'LL",
			"I'M",
			"I'VE",
			"IE",
			"IF",
			"IGNORED",
			"IMMEDIATE",
			"IN",
			"INASMUCH",
			"INC",
			"INDEED",
			"INDICATE",
			"INDICATED",
			"INDICATES",
			"INNER",
			"INSOFAR",
			"INSTEAD",
			"INTO",
			"INWARD",
			"IS",
			"ISN'T",
			"IT",
			"IT'D",
			"IT'LL",
			"IT'S",
			"ITS",
			"ITSELF",
			"J",
			"JUST",
			"K",
			"KEEP",
			"KEEPS",
			"KEPT",
			"KNOW",
			"KNOWS",
			"KNOWN",
			"L",
			"LAQUO",
			"LAST",
			"LATELY",
			"LATER",
			"LATTER",
			"LATTERLY",
			"LEAST",
			"LESS",
			"LEST",
			"LET",
			"LET'S",
			"LIKE",
			"LIKED",
			"LIKELY",
			"LITTLE",
			"LOOK",
			"LOOKING",
			"LOOKS",
			"LTD",
			"M",
			"MAINLY",
			"MANY",
			"MAY",
			"MAYBE",
			"ME",
			"MEAN",
			"MEANWHILE",
			"MERELY",
			"MIGHT",
			"MORE",
			"MOREOVER",
			"MOST",
			"MOSTLY",
			"MUCH",
			"MUST",
			"MY",
			"MYSELF",
			"N",
			"NAME",
			"NAMELY",
			"NBSP",
			"ND",
			"NEAR",
			"NEARLY",
			"NECESSARY",
			"NEED",
			"NEEDS",
			"NEITHER",
			"NEVER",
			"NEVERTHELESS",
			"NEW",
			"NEXT",
			"NINE",
			"NO",
			"NOBODY",
			"NON",
			"NONE",
			"NOONE",
			"NOR",
			"NORMALLY",
			"NOT",
			"NOTHING",
			"NOVEL",
			"NOW",
			"NOWHERE",
			"O",
			"OBVIOUSLY",
			"OF",
			"OFF",
			"OFTEN",
			"OH",
			"OK",
			"OKAY",
			"OLD",
			"ON",
			"ONCE",
			"ONE",
			"ONES",
			"ONLY",
			"ONTO",
			"OR",
			"OTHER",
			"OTHERS",
			"OTHERWISE",
			"OUGHT",
			"OUR",
			"OURS",
			"OURSELVES",
			"OUT",
			"OUTSIDE",
			"OVER",
			"OVERALL",
			"OWN",
			"P",
			"PARTICULAR",
			"PARTICULARLY",
			"PER",
			"PERHAPS",
			"PLACED",
			"PLEASE",
			"PLUS",
			"POSSIBLE",
			"PRESUMABLY",
			"PROBABLY",
			"PROVIDES",
			"Q",
			"QUE",
			"QUITE",
			"QV",
			"R",
			"RAQUO",
			"RATHER",
			"RD",
			"RE",
			"REALLY",
			"REASONABLY",
			"REGARDING",
			"REGARDLESS",
			"REGARDS",
			"RELATIVELY",
			"RESPECTIVELY",
			"RIGHT",
			"S",
			"SAID",
			"SAME",
			"SAW",
			"SAY",
			"SAYING",
			"SAYS",
			"SECOND",
			"SECONDLY",
			"SEE",
			"SEEING",
			"SEEM",
			"SEEMED",
			"SEEMING",
			"SEEMS",
			"SEEN",
			"SELF",
			"SELVES",
			"SENSIBLE",
			"SENT",
			"SERIOUS",
			"SERIOUSLY",
			"SEVEN",
			"SEVERAL",
			"SHALL",
			"SHE",
			"SHOULD",
			"SHOULDN'T",
			"SINCE",
			"SIX",
			"SO",
			"SOME",
			"SOMEBODY",
			"SOMEHOW",
			"SOMEONE",
			"SOMETHING",
			"SOMETIME",
			"SOMETIMES",
			"SOMEWHAT",
			"SOMEWHERE",
			"SOON",
			"SORRY",
			"SPECIFIED",
			"SPECIFY",
			"SPECIFYING",
			"STILL",
			"SUB",
			"SUCH",
			"SUP",
			"SURE",
			"T",
			"T'S",
			"TAKE",
			"TAKEN",
			"TELL",
			"TENDS",
			"TH",
			"THAN",
			"THANK",
			"THANKS",
			"THANX",
			"THAT",
			"THAT'S",
			"THATS",
			"THE",
			"THEIR",
			"THEIRS",
			"THEM",
			"THEMSELVES",
			"THEN",
			"THENCE",
			"THERE",
			"THERE'S",
			"THEREAFTER",
			"THEREBY",
			"THEREFORE",
			"THEREIN",
			"THERES",
			"THEREUPON",
			"THESE",
			"THEY",
			"THEY'D",
			"THEY'LL",
			"THEY'RE",
			"THEY'VE",
			"THINK",
			"THIRD",
			"THIS",
			"THOROUGH",
			"THOROUGHLY",
			"THOSE",
			"THOUGH",
			"THREE",
			"THROUGH",
			"THROUGHOUT",
			"THRU",
			"THUS",
			"TO",
			"TOGETHER",
			"TOO",
			"TOOK",
			"TOWARD",
			"TOWARDS",
			"TRIED",
			"TRIES",
			"TRULY",
			"TRY",
			"TRYING",
			"TWICE",
			"TWO",
			"U",
			"UN",
			"UNDER",
			"UNFORTUNATELY",
			"UNLESS",
			"UNLIKELY",
			"UNTIL",
			"UNTO",
			"UP",
			"UPON",
			"US",
			"USE",
			"USED",
			"USEFUL",
			"USES",
			"USING",
			"USUALLY",
			"UUCP",
			"V",
			"VALUE",
			"VARIOUS",
			"VERY",
			"VIA",
			"VIZ",
			"VS",
			"W",
			"WANT",
			"WANTS",
			"WAS",
			"WASN'T",
			"WAY",
			"WE",
			"WE'D",
			"WE'LL",
			"WE'RE",
			"WE'VE",
			"WELCOME",
			"WELL",
			"WENT",
			"WERE",
			"WEREN'T",
			"WHAT",
			"WHAT'S",
			"WHATEVER",
			"WHEN",
			"WHENCE",
			"WHENEVER",
			"WHERE",
			"WHERE'S",
			"WHEREAFTER",
			"WHEREAS",
			"WHEREBY",
			"WHEREIN",
			"WHEREUPON",
			"WHEREVER",
			"WHETHER",
			"WHICH",
			"WHILE",
			"WHITHER",
			"WHO",
			"WHO'S",
			"WHOEVER",
			"WHOLE",
			"WHOM",
			"WHOSE",
			"WHY",
			"WILL",
			"WILLING",
			"WISH",
			"WITH",
			"WITHIN",
			"WITHOUT",
			"WON'T",
			"WONDER",
			"WOULD",				
			"WOULDN'T",
			"X",
			"Y",
			"YES",
			"YET",
			"YOU",
			"YOU'D",
			"YOU'LL",
			"YOU'RE",
			"YOU'VE",
			"YOUR",
			"YOURS",
			"YOURSELF",
			"YOURSELVES",
			"Z",
			"ZERO"
		};

		private static readonly string[] arStopWordsGreek = new string[]
		{	
			"а",
			"ац",
			"аки",
			"акиломо",
			"акка",
			"аккиыс",
			"аккоу",
			"акт",
			"ала",
			"але",
			"алесыс",
			"алпоте",
			"ам",
			"амалеса",
			"амалетану",
			"амте",
			"амти",
			"амтипеяа",
			"ап",
			"апо",
			"ая",
			"аяца",
			"ас",
			"ауяио",
			"аута",
			"аутес",
			"аутг",
			"аутгм",
			"аутгс",
			"ауто",
			"аутои",
			"аутом",
			"аутос",
			"аутоу",
			"аутоус",
			"аутым",
			"ажотоу",
			"ажоу",
			"б",
			"ба",
			"бд",
			"бебаиа",
			"бк",
			"ц",
			"циа",
			"циати",
			"ця",
			"д",
			"дда",
			"де",
			"деима",
			"дем",
			"дгхем",
			"дгк",
			"дгкадг",
			"диа",
			"дийг",
			"дийо",
			"дийос",
			"диокоу",
			"дивыс",
			"дяв",
			"дуо",
			"е",
			"ецы",
			"еды",
			"еиделг",
			"еихе",
			"еилаи",
			"еиласте",
			"еимаи",
			"еисаи",
			"еисте",
			"еите",
			"еива",
			"еивале",
			"еивам",
			"еивате",
			"еиве",
			"еивес",
			"ей",
			"ейеи",
			"ейеима",
			"ейеимес",
			"ейеимг",
			"ейеимгс",
			"ейеимо",
			"ейеимои",
			"ейеимом",
			"ейеимос",
			"ейеимоу",
			"ейеимоус",
			"ейеимым",
			"елас",
			"елеис",
			"елема",
			"елпяос",
			"ем",
			"ема",
			"емам",
			"емас",
			"емос",
			"емы",
			"ен",
			"енгс",
			"енисоу",
			"ены",
			"епеидг",
			"епеита",
			"епи",
			"еписгс",
			"есас",
			"есеис",
			"есема",
			"есу",
			"етоутес",
			"етоутг",
			"етоуто",
			"етоутос",
			"етоутоу",
			"етси",
			"еуце",
			"еве",
			"евеи",
			"евеис",
			"евете",
			"еволе",
			"евомтас",
			"евоуле",
			"евоум",
			"евы",
			"ф",
			"г",
			"гдг",
			"гластам",
			"гласте",
			"глоум",
			"гсастам",
			"гсасте",
			"гсоум",
			"гтам",
			"х",
			"ха",
			"и",
			"идиа",
			"идио",
			"идиос",
			"идиыс",
			"исале",
			"исыс",
			"й",
			"йа",
			"йахокоу",
			"йахыс",
			"йаи",
			"йайа",
			"йака",
			"йалиа",
			"йалиас",
			"йамеис",
			"йамема",
			"йамемас",
			"йамемос",
			"йапоиа",
			"йапоиас",
			"йапоиес",
			"йапоио",
			"йапоиои",
			"йапоиом",
			"йапоиос",
			"йапоиоу",
			"йапоиоус",
			"йапоиым",
			"йапоте",
			"йапоу",
			"йапыс",
			"йата",
			"йати",
			"йатити",
			"йатопим",
			"йаты",
			"йи",
			"йос",
			"йтк",
			"йуб",
			"йуяиыс",
			"к",
			"кицо",
			"л",
			"ла",
			"лафи",
			"лайаяи",
			"лакиста",
			"лаяс",
			"лас",
			"ле",
			"леиом",
			"леса",
			"лета",
			"летану",
			"лг",
			"лгм",
			"лгпыс",
			"лгте",
			"лиа",
			"лиас",
			"лик",
			"локис",
			"локомоти",
			"ломолиас",
			"ломос",
			"лоу",
			"лпа",
			"лпяабо",
			"лпяос",
			"м",
			"ма",
			"маи",
			"мыяис",
			"н",
			"о",
			"ои",
			"окоцуяа",
			"окотека",
			"олыс",
			"опоиа",
			"опоиас",
			"опоиес",
			"опоио",
			"опоиои",
			"опоиом",
			"опоиос",
			"опоиоу",
			"опоиым",
			"опоте",
			"опоу",
			"опоудгпоте",
			"опыс",
			"оса",
			"осес",
			"осг",
			"осгс",
			"осо",
			"осодгпоте",
			"осои",
			"осом",
			"осос",
			"осоу",
			"осым",
			"отам",
			"оти",
			"оу",
			"оуте",
			"оуж",
			"ови",
			"п",
			"па",
			"памтоу",
			"памы",
			"паяа",
			"пб",
			"пеяа",
			"пеяи",
			"пеяипоу",
			"пиа",
			"пио",
			"писы",
			"пкгм",
			"поиа",
			"поиас",
			"поиес",
			"поио",
			"поиои",
			"поиом",
			"поиос",
			"поиоу",
			"поиоус",
			"поиым",
			"поку",
			"поса",
			"посес",
			"посг",
			"посгс",
			"посо",
			"посои",
			"посом",
			"посос",
			"посоу",
			"посым",
			"поте",
			"поу",
			"поухема",
			"пяб",
			"пяепеи",
			"пяим",
			"пяо",
			"пяос",
			"пяотоу",
			"пяовтес",
			"пв",
			"пыс",
			"я",
			"с",
			"сам",
			"сас",
			"се",
			"сгл",
			"сглеяа",
			"соу",
			"ста",
			"стг",
			"стгм",
			"стгс",
			"стис",
			"сто",
			"стом",
			"стоу",
			"стоус",
			"стя",
			"сум",
			"сумала",
			"сведом",
			"т",
			"та",
			"таде",
			"тава",
			"тавате",
			"тес",
			"тетоиа",
			"тетоиас",
			"тетоиес",
			"тетоио",
			"тетоиои",
			"тетоиом",
			"тетоиос",
			"тетоиоу",
			"тетоиоус",
			"тетоиым",
			"тетя",
			"тг",
			"тгм",
			"тгс",
			"ти",
			"типота",
			"типоте",
			"тис",
			"то",
			"тои",
			"том",
			"тос",
			"тосг",
			"тосгс",
			"тосо",
			"тосос",
			"тосоу",
			"тоте",
			"тоу",
			"тоукависто",
			"тоукавистом",
			"тоус",
			"тоута",
			"тоутес",
			"тоутг",
			"тоутгс",
			"тоуто",
			"тоутос",
			"тоутоу",
			"тоутым",
			"тяиа",
			"тувом",
			"тым",
			"у",
			"уц",
			"упея",
			"упо",
			"устеяа",
			"ж",
			"в",
			"вця",
			"вик",
			"вл",
			"втес",
			"выяис",
			"ь",
			"ы",
			"ыс",
			"ысотоу",
			"ыспоу",
			"ысте",
			"ыстосо"	
		};

		#endregion
		
		#region Constructors and Initialization methods
		
		/// <summary>
		/// The public access point of the object
		/// </summary>
		/// <returns>The single instance</returns>
		public static WordExtractor Instance()
		{
			
			if(instance==null)
			{
				// Only one thread can obtain a mutex
				Mutex imutex = new Mutex();
				imutex.WaitOne();
				if(instance==null)
				{
					instance=new WordExtractor();
				}
				imutex.Close();
			}
			return instance;
		}

		/// <summary>
		/// The private constructor of the WordExtractor class
		/// </summary>
		private WordExtractor()
		{
			mutex = new Mutex();
			settings = PluginSettings.Instance();
			regStripTags= new Regex("<[^>]*>", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);//<[^>]+> or   >(?:(?<t>[^<]*))
			regStripScripts=new Regex(@"(?i)<script([^>])*>(\w|\W)*</script([^>])*>",RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled); //@"(?i)<script([^>])*>(\w|\W)*</script([^>])*>" or @"<script[^>]*>(\w|\W)*?</script[^>]*>"
			regTitleTags=new Regex("<\\s*title[^>]*>[^<]*<\\s*/title\\s*>",RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			regKeywordTags=new Regex("<meta\\s*name\\s*=\\s*\"keywords\"\\s*content\\s*=\\s*\"[^>]*\">", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			regDescriptionTags=new Regex("<meta\\s*name\\s*=\\s*\"description\"\\s*content\\s*=\\s*\"[^>]*\">", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			stemming=Stemming.Instance();
			slStopWordsEnglish=new SortedList(arStopWordsEnglish.Length);
			slStopWordsGreek=new SortedList(arStopWordsGreek.Length);
			culture = new CultureInfo("el-GR");
			InitializeWordLists();
		}

		/// <summary>
		/// Initializes the sorted lists with the available stop words
		/// </summary>
		private void InitializeWordLists()
		{
			foreach(string s in arStopWordsEnglish)
			{
				try
				{
					slStopWordsEnglish.Add(s, null);
				}
				catch
				{
					//if something goes wrong, e.g. a duplicate stop word exists
					continue;
				}
				finally
				{
					slStopWordsEnglish.TrimToSize();
				}
			}
			foreach(string s in arStopWordsGreek)
			{
				try
				{
					slStopWordsGreek.Add(s, null);
				}
				catch
				{
					//if something goes wrong, e.g. a duplicate stop word exists
					continue;
				}
				finally
				{
					slStopWordsGreek.TrimToSize();
				}
			}
		}

		#endregion

		#region Existence checking methods

		/// <summary>
		/// Checks if the word is contained in the English Stop Words list
		/// </summary>
		/// <param name="Word">The word to check</param>
		/// <returns>True if the word exists</returns>
		public bool ExistsInEnglishStopWords(string Word)
		{
			return (slStopWordsEnglish.ContainsKey(Word));
		}

		/// <summary>
		/// Checks if the word is contained in the Greek Stop Words list
		/// </summary>
		/// <param name="Word">The word to check</param>
		/// <returns>True if the word exists</returns>
		public bool ExistsInGreekStopWords(string Word)
		{
			return (slStopWordsGreek.ContainsKey(Word));
		}

		/// <summary>
		/// Checks if the word is contained in the Stop Words list
		/// </summary>
		/// <param name="Word">The word to check</param>
		/// <returns>True if the word exists</returns>
		public bool ExistsInAllStopWords(string Word)
		{
			return (ExistsInGreekStopWords(Word)||ExistsInEnglishStopWords(Word));
		}

		#endregion

		#region Word Extraction Related Methods

		/// <summary>
		/// Converts a string to its capitalized form.
		/// </summary>
		/// <param name="strToCapitalize">The string to capitalize</param>
		/// <returns>The capitalized string</returns>
		public string CapitalizeString(string strToCapitalize)
		{
			StringBuilder sb=new StringBuilder(strToCapitalize.ToUpper(culture));
			sb.Replace('╒','а');
			sb.Replace('╦','е');
			sb.Replace('╧','г');
			sb.Replace('╨','и');
			sb.Replace('з','и');
			sb.Replace('╪','о');
			sb.Replace('╬','у');
			sb.Replace('ш','у');
			sb.Replace('©','ы');
			sb.Replace('╧','г');
			sb.Replace('ю','и');
			sb.Replace('Ю','у');
			return sb.ToString();
		}

		/// <summary>
		/// This method takes a capitalized word as input and performs a 2-level Stemming
		/// </summary>
		/// <param name="word">The word to stem</param>
		/// <returns>The stemmed word</returns>
		private string StemWord(string word)
		{		
			return stemming.StemWord(word);
		}

		/// <summary>
		/// Checks if a word contains only numbers
		/// </summary>
		/// <param name="word">The word to check</param>
		/// <returns>True if the word is a number</returns>
		public bool IsNumericString(string word)
		{
			try
			{
				double.Parse(word);
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Splits a string into words and returns a sorted list that contains the
		/// words and the number of times they appear in the original string.
		/// </summary>
		/// <param name="strInput">The string to split into words</param>
		/// <returns>A SortedList containing the words and their frequency</returns>
		private SortedList SplitString(string strInput)
		{
			SortedList sl=new SortedList();
			string[] words=strInput.Split(Delimiters);
			foreach(string s in words)
			{
				string word=s.Trim();
				if(word.Length>1)
				{
					if((ExistsInAllStopWords(word))||(IsNumericString(word)))
					{
						continue;
					}
					try
					{
						word=StemWord(word);
						if(sl.ContainsKey(word))
						{
							//this word already exists. Just increment it's count
							sl[word]=(int)sl[word]+1;
						}
						else
						{
							//The word does not exist, add it to the list with a count of 1
							sl.Add(word, 1);					
						}
					}
					catch
					{
						continue;
					}
				}
			}
			sl.TrimToSize();
			return sl;
		}

		/// <summary>
		/// Scans a string containing HTML code for description and keywords
		/// meta tags and builds a string that contains them separated by spaces.
		/// </summary>
		/// <param name="strHTML">the HTML to parse</param>
		/// <returns>A string containing the meta tags</returns>
		private string ExtractMetaTags(string strHTML)
		{
			StringBuilder sb=new StringBuilder();
			MatchCollection matches=regDescriptionTags.Matches(strHTML);
			string tmp=String.Empty;
			char [] toTrim=new char [] { '"','>', '<'};
			foreach(Match m in matches)
			{
				tmp=m.Value.Substring(m.Value.LastIndexOf('='));
				tmp=tmp.Substring(0, tmp.LastIndexOf('"'));
				sb.Append(tmp.Trim(toTrim));
			}
			matches=regKeywordTags.Matches(strHTML);
			foreach(Match m in matches)
			{
				tmp=m.Value.Substring(m.Value.LastIndexOf('='));
				tmp=tmp.Substring(0, tmp.LastIndexOf('"'));
				sb.Append(tmp.Trim(toTrim));
			}
			GC.Collect();
			return sb.ToString();
		}

		/// <summary>
		/// Scans an HTML document for title tags and returns the page's title.
		/// </summary>
		/// <param name="strHTML">The HTML text to parse</param>
		/// <returns>A string containing the page title</returns>
		private string ExtractTitle(string strHTML)
		{
			MatchCollection m=regTitleTags.Matches(strHTML);
			if(m.Count==0)
			{
				return "";
			}
			else
			{
				string tmp=(m[0].Value);
				tmp=tmp.Substring(tmp.IndexOf('>')+1);
				tmp=tmp.Substring(0,tmp.IndexOf('<'));
				return tmp;
			}
		}

		/// <summary>
		/// This method takes as an input an HTML or plain text document, removes all the
		/// tags and then performs a word extraction after capitalizing all words. It takes
		/// into account the title, description and keywords meta tags according to the user's
		/// preferences.
		/// </summary>
		/// <param name="strInput">The string from which to extract words</param>
		/// <returns>A sorted list containing the words and their frequency</returns>
		public SortedList ExtractWords(string strInput)
		{
			try
			{
				mutex.WaitOne();
				try
				{
					string text=regStripScripts.Replace(strInput," ");
					StringBuilder sb=new StringBuilder(regStripTags.Replace(text," "));
					if(settings.ExtractMetaTags)
					{
						sb.Append(' ');
						sb.Append(ExtractMetaTags(strInput));
					}
					if(settings.ExtractTitleTag)
					{
						sb.Append(' ');
						sb.Append(ExtractTitle(strInput));
					}
					string CleanText=CapitalizeString(sb.ToString());
					return SplitString(CleanText);
				}
				catch
				{
					GC.Collect();
					return new SortedList();
				}
			}
			finally
			{
				mutex.ReleaseMutex(); //release the mutex
			}
		}

		#endregion
	}
}
