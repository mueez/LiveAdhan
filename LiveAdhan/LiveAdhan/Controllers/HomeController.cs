using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using LiveAthan.Models;
using LiveAthan.Helpers;

namespace LiveAthan.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index(double? latitude, double? longitude, int? timeOffset, String country = "", String city = "")
        {
            var result = new LocationModel();
            if (!latitude.HasValue || !longitude.HasValue)
            {
                var ip = Request.UserHostAddress;
                var client = new WebClient();
                Stream data = client.OpenRead(String.Format("http://freegeoip.net/xml/{0}", ip));
                var xmlNav = new XPathDocument(data).CreateNavigator();
                var lat = ReadNode(xmlNav, "/Response/Latitude");
                var lng = ReadNode(xmlNav, "/Response/Longitude");
                if (lat != "0" || lng != "0")
                {
                    latitude = Double.Parse(lat);
                    longitude = Double.Parse(lng);
                    city = ReadNode(xmlNav, "/Response/City");
                    country = ReadNode(xmlNav, "/Response/CountryName");
                }
            }

            Times prayerTimes = null;
            String description = "No location";

            if (latitude.HasValue && longitude.HasValue)
            {
                var times = new PrayerTimes();
                //times.setAsrMethod(JuristicMethod.Hanafi);
                //times.setCalcMethod(CalculationMethods.ISNA);
                // TODO: Is this client time zone?
                var today = DateTime.Today;
                if (!timeOffset.HasValue)
                {
                    timeOffset = -4; // TODO: Try to guess on server based on location?
                }
                //times.setTimeFormat(TimeFormat.Time12);
                prayerTimes = times.getDatePrayerTimes(today.Year, today.Month, today.Day, latitude.Value, longitude.Value, timeOffset.Value);
                description = String.Format("{0}, {1}", city, country);
            }

            return View(new LocationModel
            {
                Description = description,
                Latitude = latitude ?? 0,
                Longitude = longitude ?? 0,
                PrayerTimes = prayerTimes,
            });
        }

        private static String ReadNode(XPathNavigator xmlNav, String path)
        {
            var valueReader = xmlNav.Select(path);
            while (valueReader.MoveNext())
            {
                return valueReader.Current.Value;
                //System.Console.WriteLine(valueReader.Current.Name + " : " + valueReader.Current.Value);
            }
            return String.Empty; // TODO: Fail?
        }
    }
}
