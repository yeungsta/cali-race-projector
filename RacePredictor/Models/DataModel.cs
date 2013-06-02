using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace RacePredictor.Models
{
    //Race object that corresponds to a row in the Races database table.
    public class Race
    {
        public int ID { get; set; }
        public string County { get; set; }
        public int Year { get; set; }
        public int Total { get; set; }
        public int White { get; set; }
        public int Hispanic { get; set; }
        public int Asian { get; set; }
        public int Pacific { get; set; }
        public int Black { get; set; }
        public int Indian { get; set; }
        public int Multirace { get; set; }
        public float WhitePer { get; set; }
        public float HispanicPer { get; set; }
        public float AsianPer { get; set; }
        public float PacificPer { get; set; }
        public float BlackPer { get; set; }
        public float IndianPer { get; set; }
        public float MultiracePer { get; set; }
    }

    public class RacePredictorDBContext : DbContext
    {
        public DbSet<Race> Races { get; set; }

#if Staging  //map to staging database tables
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Race>().ToTable("Races_raceprojector");
        }
#endif
    }

    //Use if just want to reinitialize every time
    //public class RaceInitializer : DropCreateDatabaseAlways<RacePredictorDBContext>
    public class RaceInitializer : DropCreateDatabaseIfModelChanges<RacePredictorDBContext>
    {
        protected override void Seed(RacePredictorDBContext context)
        {
            /*
            var races = new List<Race> {  
                 new Race { 
                     County = "Test",
                     Year = 2000,
                     Total = 100,
                     WhitePer = 57.75F
                 },
                 new Race { 
                     County = "Test2",
                     Year = 2001,
                     Total = 200,
                     WhitePer = 37.001F
                 },
             };

            races.ForEach(d => context.Races.Add(d));
            */ 
        }
    }

    public class DropdownListModel
    {
        public string SelectedItem { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
    }

    public class YearInfo
    {
        //total population
        public int Population { get; set; }

        //formatted and rounded for display
        public string PopulationDisplay { get; set; }

        //percentage of total population
        public float Percentage { get; set; }

        //formatted and rounded for display
        public string PercentageDisplay { get; set; }
    }

    public class DiffInfo
    {
        //difference (in total number) from start year to end year
        public int Diff { get; set; }

        //formatted and rounded for display
        public string DiffDisplay { get; set; }

        //difference (in percentage) from start year to end year
        public float PerDiff { get; set; }

        //formatted and rounded for display
        public string PerDiffDisplay { get; set; }
    }

    public class RaceDisplay
    {
        public string Text { get; set; }
        public string Color { get; set; }
    }

    public class DiffResult
    {
        public RaceDisplay RaceType { get; set; }
        public DiffInfo DiffInfo { get; set; }
    }

    public class YearResult
    {
        public RaceDisplay RaceType { get; set; }
        public YearInfo YearInfo { get; set; }
    }

    public class RaceQueryViewModel
    {
        public DropdownListModel County { get; set; }
        public DropdownListModel StartYear { get; set; }
        public DropdownListModel EndYear { get; set; }
        public string TotalStartYear { get; set; }
        public string TotalEndYear { get; set; }
        public DiffInfo TotalDiffResult { get; set; }
        public IOrderedEnumerable<YearResult> QueryStartYearResult { get; set; }
        public IOrderedEnumerable<YearResult> QueryEndYearResult { get; set; }
        public IOrderedEnumerable<DiffResult> QueryDiffResult { get; set; }
    } 
}