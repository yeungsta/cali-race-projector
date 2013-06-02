using System.ComponentModel;

namespace RacePredictor.Controllers
{
    public class Constants
    {
        public enum Counties { [Description("All of California")] California, Alameda, Alpine, Amador, Butte, Calaveras, Colusa, [Description("Contra Costa")] ContraCosta, [Description("Del Norte")] DelNorte, [Description("El Dorado")] ElDorado, Fresno, Glenn, Humboldt, Imperial, Inyo, Kern, Kings, Lake, Lassen, [Description("Los Angeles")] LosAngeles, Madera, Marin, Mariposa, Mendocino, Merced, Modoc, Mono, Monterey, Napa, Nevada, Orange, Placer, Plumas, Riverside, Sacramento, [Description("San Benito")] SanBenito, [Description("San Bernardino")] SanBernardino, [Description("San Diego")] SanDiego, [Description("San Francisco")] SanFrancisco, [Description("San Joaquin")] SanJoaquin, [Description("San Luis Obispo")] SanLuisObispo, [Description("San Mateo")] SanMateo, [Description("Santa Barbara")] SantaBarbara, [Description("Santa Clara")] SantaClara, [Description("Santa Cruz")] SantaCruz, Shasta, Sierra, Siskiyou, Solano, Sonoma, Stanislaus, Sutter, Tehama, Trinity, Tulare, Tuolumne, Ventura, Yolo, Yuba };
        public const int YearRangeStart = 2000;
        public const int YearRangeEnd = 2050;

        //Race Display Text Types
        public const string White = "White";
        public const string Hispanic = "Hispanic";
        public const string Asian = "Asian";
        public const string Pacific = "Pacific Islander";
        public const string Black = "Black";
        public const string Indian = "Indian";
        public const string Multirace = "Multi-race";
        public const string Total = "Total";

        //Race Display Colors
        public const string RoyalBlue = "RoyalBlue";
        public const string OrangeRed = "OrangeRed";
        public const string Orange = "Orange";
        public const string Green = "Green";
        public const string Purple = "Purple";
        public const string Cyan = "Cyan";
        public const string YellowGreen = "YellowGreen";
    }
}