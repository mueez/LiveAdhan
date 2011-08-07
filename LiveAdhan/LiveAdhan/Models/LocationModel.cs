using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiveAthan.Helpers;

namespace LiveAthan.Models
{
    public class ResponseModel
    {
        public CalculationMethod CalculationMethod { get; set; }
        public JuristicMethod AsrMethod { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public String Description { get; set; }
        public Times PrayerTimes { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int TimeOffset { get; set; }
    }
    public class SettingsModel
    {
        public CalculationMethod? CalculationMethod { get; set; }
        public JuristicMethod? AsrMethod { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? TimeOffset { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}