using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiveAthan.Helpers;

namespace LiveAthan.Models
{
    public class LocationModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public String Description { get; set; }
        public Times PrayerTimes { get; set; }
    }
}