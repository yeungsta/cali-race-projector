using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Reflection;
using RacePredictor.Models;

namespace RacePredictor.Controllers
{
    public class HomeController : Controller
    {
        private RacePredictorDBContext db = new RacePredictorDBContext();
        private static IList<SelectListItem>  m_countyDropdownList;
        private static IList<SelectListItem> m_startYearsDropdownList;
        private static IList<SelectListItem> m_endYearsDropdownList;
        private static RaceDisplay m_Asian;
        private static RaceDisplay m_Black;
        private static RaceDisplay m_Hispanic;
        private static RaceDisplay m_Indian;
        private static RaceDisplay m_Multirace;
        private static RaceDisplay m_Pacific;
        private static RaceDisplay m_White;

        static HomeController()
        {
            //create race display types
            m_Asian = new RaceDisplay { Text = Constants.Asian, Color = Constants.Green };
            m_Black = new RaceDisplay { Text = Constants.Black, Color = Constants.Orange };
            m_Hispanic = new RaceDisplay { Text = Constants.Hispanic, Color = Constants.RoyalBlue };
            m_Indian = new RaceDisplay { Text = Constants.Indian, Color = Constants.Purple };
            m_Multirace = new RaceDisplay { Text = Constants.Multirace, Color = Constants.Cyan };
            m_Pacific = new RaceDisplay { Text = Constants.Pacific, Color = Constants.YellowGreen };
            m_White = new RaceDisplay { Text = Constants.White, Color = Constants.OrangeRed };

            m_countyDropdownList = new List<SelectListItem>();
            m_startYearsDropdownList = new List<SelectListItem>();
            m_endYearsDropdownList = new List<SelectListItem>();

            foreach (Constants.Counties county in EnumToList<Constants.Counties>())
            {
                string description = GetEnumDescription(county);

                if (county == Constants.Counties.California)
                {
                    //California has a description that doesn't match the SQL table entry for California
                    m_countyDropdownList.Add(new SelectListItem { Text = description, Value = county.ToString() });
                }
                else
                {
                    m_countyDropdownList.Add(new SelectListItem { Text = description, Value = description });
                }
            }

            //add an option for the current year
            m_startYearsDropdownList.Add(new SelectListItem { Text = "from Now", Value = DateTime.UtcNow.Year.ToString() });

            for (int x = Constants.YearRangeStart; x <= Constants.YearRangeEnd; x++)
            {
                m_startYearsDropdownList.Add(new SelectListItem { Text = "from " + x.ToString(), Value = x.ToString() });
                m_endYearsDropdownList.Add(new SelectListItem { Text = "to " + x.ToString(), Value = x.ToString() });
            }
        }

        public ActionResult Redirect()
        {
            return View();
        }

        public ActionResult Index()
        {
            RaceQueryViewModel raceQuery = new RaceQueryViewModel();

            raceQuery.County = new DropdownListModel();
            raceQuery.County.Items = m_countyDropdownList;
            raceQuery.County.SelectedItem = Constants.Counties.California.ToString();

            raceQuery.StartYear = new DropdownListModel();
            raceQuery.StartYear.Items = m_startYearsDropdownList;
            raceQuery.StartYear.SelectedItem = "from Now";

            raceQuery.EndYear = new DropdownListModel();
            raceQuery.EndYear.Items = m_endYearsDropdownList;
            raceQuery.EndYear.SelectedItem = Constants.YearRangeEnd.ToString();

            return View(raceQuery);
        }

        //
        // POST
        [HttpPost]
        public ActionResult Index(RaceQueryViewModel raceQuery)
        {
            int startYear = Convert.ToInt32(raceQuery.StartYear.SelectedItem);
            int endYear = Convert.ToInt32(raceQuery.EndYear.SelectedItem);

            Race startYearObj = db.Races.Where(x => x.County == raceQuery.County.SelectedItem).Where(x => x.Year == startYear).Single();

            if (startYearObj == null)
            {
                return HttpNotFound();
            }

            Race endYearObj = db.Races.Where(x => x.County == raceQuery.County.SelectedItem).Where(x => x.Year == endYear).Single();

            if (endYearObj == null)
            {
                return HttpNotFound();
            }

            //re-populate dropdown items
            RaceQueryViewModel outRaceQuery = new RaceQueryViewModel();
            outRaceQuery.County = new DropdownListModel();
            outRaceQuery.County.Items = m_countyDropdownList;
            outRaceQuery.County.SelectedItem = raceQuery.County.SelectedItem;

            outRaceQuery.StartYear = new DropdownListModel();
            outRaceQuery.StartYear.Items = m_startYearsDropdownList;
            outRaceQuery.StartYear.SelectedItem = raceQuery.StartYear.SelectedItem;

            outRaceQuery.EndYear = new DropdownListModel();
            outRaceQuery.EndYear.Items = m_endYearsDropdownList;
            outRaceQuery.EndYear.SelectedItem = raceQuery.EndYear.SelectedItem;

            //populate total population for years
            outRaceQuery.TotalStartYear = FormatCount(startYearObj.Total);
            outRaceQuery.TotalEndYear = FormatCount(endYearObj.Total);
            //calculate total diff results
            int tempDiff = endYearObj.Total - startYearObj.Total;
            float tempPerDiff = (float)tempDiff / (float)startYearObj.Total;
            outRaceQuery.TotalDiffResult = new DiffInfo { Diff = tempDiff, DiffDisplay = FormatCount(tempDiff), PerDiff = tempPerDiff, PerDiffDisplay = FormatPercentage(tempPerDiff) };

            //calculate and order end/start year results
            outRaceQuery.QueryStartYearResult = ProcessYear(startYearObj).OrderByDescending(x => Math.Abs(x.YearInfo.Population));
            outRaceQuery.QueryEndYearResult = ProcessYear(endYearObj).OrderByDescending(x => Math.Abs(x.YearInfo.Population));

            //calculate and order diff results by largest movers
            outRaceQuery.QueryDiffResult = CompareYears(startYearObj, endYearObj).OrderByDescending(x => Math.Abs(x.DiffInfo.Diff));

            return View(outRaceQuery);
        }

        //leave out Total group
        private IList<YearResult> ProcessYear(Race yearObj)
        {
            IList<YearResult> results = new List<YearResult>();

            results.Add(new YearResult { RaceType = m_Asian, YearInfo = new YearInfo { Population = yearObj.Asian, PopulationDisplay = FormatCount(yearObj.Asian), Percentage = yearObj.AsianPer, PercentageDisplay = FormatPercentage(yearObj.AsianPer) } });
            results.Add(new YearResult { RaceType = m_Black, YearInfo = new YearInfo { Population = yearObj.Black, PopulationDisplay = FormatCount(yearObj.Black), Percentage = yearObj.BlackPer, PercentageDisplay = FormatPercentage(yearObj.BlackPer) } });
            results.Add(new YearResult { RaceType = m_Hispanic, YearInfo = new YearInfo { Population = yearObj.Hispanic, PopulationDisplay = FormatCount(yearObj.Hispanic), Percentage = yearObj.HispanicPer, PercentageDisplay = FormatPercentage(yearObj.HispanicPer) } });
            results.Add(new YearResult { RaceType = m_Indian, YearInfo = new YearInfo { Population = yearObj.Indian, PopulationDisplay = FormatCount(yearObj.Indian), Percentage = yearObj.IndianPer, PercentageDisplay = FormatPercentage(yearObj.IndianPer) } });
            results.Add(new YearResult { RaceType = m_Multirace, YearInfo = new YearInfo { Population = yearObj.Multirace, PopulationDisplay = FormatCount(yearObj.Multirace), Percentage = yearObj.MultiracePer, PercentageDisplay = FormatPercentage(yearObj.MultiracePer) } });
            results.Add(new YearResult { RaceType = m_Pacific, YearInfo = new YearInfo { Population = yearObj.Pacific, PopulationDisplay = FormatCount(yearObj.Pacific), Percentage = yearObj.PacificPer, PercentageDisplay = FormatPercentage(yearObj.PacificPer) } });
            //results.Add(new YearResult { RaceType = Constants.Total, YearInfo = new YearInfo { Population = yearObj.Total, PopulationDisplay = FormatCount(yearObj.Total), Percentage = 1.0f, PercentageDisplay = FormatPercentage(1.0f) } });
            results.Add(new YearResult { RaceType = m_White, YearInfo = new YearInfo { Population = yearObj.White, PopulationDisplay = FormatCount(yearObj.White), Percentage = yearObj.WhitePer, PercentageDisplay = FormatPercentage(yearObj.WhitePer) } });

            return results;
        }

        //leave out Total group for trend results
        private IList<DiffResult> CompareYears(Race startYearObj, Race endYearObj)
        {
            IList<DiffResult> results = new List<DiffResult>();
            int tempDiff;
            float tempPerDiff;

            tempDiff = endYearObj.Asian - startYearObj.Asian;
            tempPerDiff = (float)tempDiff / (float)startYearObj.Asian;
            results.Add(new DiffResult { RaceType = m_Asian, DiffInfo = new DiffInfo { Diff = tempDiff, DiffDisplay = FormatCount(tempDiff), PerDiff = tempPerDiff, PerDiffDisplay = FormatPercentage(tempPerDiff) } });

            tempDiff = endYearObj.Black - startYearObj.Black;
            tempPerDiff = (float)tempDiff / (float)startYearObj.Black;
            results.Add(new DiffResult { RaceType = m_Black, DiffInfo = new DiffInfo { Diff = tempDiff, DiffDisplay = FormatCount(tempDiff), PerDiff = tempPerDiff, PerDiffDisplay = FormatPercentage(tempPerDiff) } });
 
            tempDiff = endYearObj.Hispanic - startYearObj.Hispanic;
            tempPerDiff = (float)tempDiff / (float)startYearObj.Hispanic;
            results.Add(new DiffResult { RaceType = m_Hispanic, DiffInfo = new DiffInfo { Diff = tempDiff, DiffDisplay = FormatCount(tempDiff), PerDiff = tempPerDiff, PerDiffDisplay = FormatPercentage(tempPerDiff) } });

            tempDiff = endYearObj.Indian - startYearObj.Indian;
            tempPerDiff = (float)tempDiff / (float)startYearObj.Hispanic;
            results.Add(new DiffResult { RaceType = m_Indian, DiffInfo = new DiffInfo { Diff = tempDiff, DiffDisplay = FormatCount(tempDiff), PerDiff = tempPerDiff, PerDiffDisplay = FormatPercentage(tempPerDiff) } });

            tempDiff = endYearObj.Multirace - startYearObj.Multirace;
            tempPerDiff = (float)tempDiff / (float)startYearObj.Multirace;
            results.Add(new DiffResult { RaceType = m_Multirace, DiffInfo = new DiffInfo { Diff = tempDiff, DiffDisplay = FormatCount(tempDiff), PerDiff = tempPerDiff, PerDiffDisplay = FormatPercentage(tempPerDiff) } });

            tempDiff = endYearObj.Pacific - startYearObj.Pacific;
            tempPerDiff = (float)tempDiff / (float)startYearObj.Pacific;
            results.Add(new DiffResult { RaceType = m_Pacific, DiffInfo = new DiffInfo { Diff = tempDiff, DiffDisplay = FormatCount(tempDiff), PerDiff = tempPerDiff, PerDiffDisplay = FormatPercentage(tempPerDiff) } });

            tempDiff = endYearObj.White - startYearObj.White;
            tempPerDiff = (float)tempDiff / (float)startYearObj.White;
            results.Add(new DiffResult { RaceType = m_White, DiffInfo = new DiffInfo { Diff = tempDiff, DiffDisplay = FormatCount(tempDiff), PerDiff = tempPerDiff, PerDiffDisplay = FormatPercentage(tempPerDiff) } });

            return results;
        }

        private string FormatPercentage(float perDiff)
        {
            return (perDiff * 100).ToString("F1") + "%";
        }

        private string FormatCount(int count)
        {
            float tempDiff = (float)count;
            float mil = 1000000f;
            float k = 1000f;

            if (Math.Abs(tempDiff) > mil)
            {
                return (tempDiff / mil).ToString("F1") + "m";
            }
            else if (Math.Abs(count) > k)
            {
                return (tempDiff / k).ToString("F1") + "k";
            }

            return count.ToString();
        }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        private static IEnumerable<T> EnumToList<T>()
        {
            Type enumType = typeof(T);

            // Can't use generic type constraints on value types,
            // so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            Array enumValArray = Enum.GetValues(enumType);
            List<T> enumValList = new List<T>(enumValArray.Length);

            foreach (int val in enumValArray)
            {
                enumValList.Add((T)Enum.Parse(enumType, val.ToString()));
            }

            return enumValList;
        }
    }
}
