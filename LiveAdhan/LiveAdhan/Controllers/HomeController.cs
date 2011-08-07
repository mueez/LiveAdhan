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
    static class SettingNames
    {
        public const String Latitude = "Latitude";
        public const String Longitude = "Longitude";
        public const String TimeOffset = "TimeOffset";
        public const String Country = "Country";
        public const String City = "City";
        public const String CalculationMethod = "CalculationMethod";
        public const String AsrMethod = "AsrMethod";
    }

    public class HomeController : Controller
    {
        const String CookieName = "Settings";
        //
        // GET: /Home/
        public ActionResult Index(SettingsModel settings)
        {
            ReadSettingsFromCookie(settings);
            SaveSettingsToCookie(settings);
            if (!settings.Latitude.HasValue || !settings.Longitude.HasValue)
            {
                var ip = Request.UserHostAddress;
                var url = String.Format("http://freegeoip.net/xml/{0}", ip);
                var ipLocation = DownloadXml(url);
                var lat = ReadNode(ipLocation, "/Response/Latitude");
                var lng = ReadNode(ipLocation, "/Response/Longitude");
                if (lat != "0" || lng != "0")
                {
                    settings.Latitude = Double.Parse(lat);
                    settings.Longitude = Double.Parse(lng);
                    settings.City = ReadNode(ipLocation, "/Response/City");
                    settings.Country = ReadNode(ipLocation, "/Response/CountryName");
                }
            }

            Times prayerTimes = null;
            String description = "Unknown location";
            var offset = settings.TimeOffset ?? -4; // TODO: Try to guess on server based on location?

            // Find out city and country if we don't have it
            if (settings.Latitude.HasValue && settings.Longitude.HasValue && (String.IsNullOrEmpty(settings.Country) || String.IsNullOrEmpty(settings.City)))
            {
                var url = String.Format("http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=true", settings.Latitude.Value, settings.Longitude.Value);
                var reverseGeocode = DownloadXml(url);
                var city = ReadNode(reverseGeocode, "/GeocodeResponse/result/address_component[type='locality']/long_name");
                if (!String.IsNullOrEmpty(city))
                {
                    settings.City = city;
                }
                var country = ReadNode(reverseGeocode, "/GeocodeResponse/result/address_component[type='country']/long_name");
                if (!String.IsNullOrEmpty(country))
                {
                    settings.Country = country;
                }
            }

            // Calculate prayer times if we have a location
            if (settings.Latitude.HasValue && settings.Longitude.HasValue)
            {
                var times = new PrayerTimes();
                if (settings.AsrMethod.HasValue) { times.setAsrMethod(settings.AsrMethod.Value); }
                if (settings.CalculationMethod.HasValue) { times.setCalcMethod(settings.CalculationMethod.Value); }
                // TODO: Is this client time zone?
                var today = DateTime.Today;

                //times.setTimeFormat(TimeFormat.Time12);
                prayerTimes = times.getDatePrayerTimes(today.Year, today.Month, today.Day, settings.Latitude.Value, settings.Longitude.Value, offset);
                if (!String.IsNullOrEmpty(settings.City) && !String.IsNullOrEmpty(settings.Country))
                {
                    description = String.Format("{0}, {1}", settings.City, settings.Country);
                }
            }

            return View(new ResponseModel
            {
                CalculationMethod = settings.CalculationMethod ?? CalculationMethod.ISNA,
                AsrMethod = settings.AsrMethod ?? JuristicMethod.Hanafi,
                Description = description,
                Latitude = settings.Latitude,
                Longitude = settings.Longitude,
                City = settings.City,
                Country = settings.Country,
                PrayerTimes = prayerTimes,
                TimeOffset = offset,
            });
        }

        private static XPathNavigator DownloadXml(string url)
        {
            var client = new WebClient();
            Stream data = client.OpenRead(url);
            var xmlNav = new XPathDocument(data).CreateNavigator();
            return xmlNav;
        }

        private void SaveSettingsToCookie(SettingsModel settings)
        {
            var cookie = Response.Cookies[CookieName];
            if (settings.AsrMethod != null) { cookie[SettingNames.AsrMethod] = settings.AsrMethod.ToString(); }
            if (settings.CalculationMethod != null) { cookie[SettingNames.CalculationMethod] = settings.CalculationMethod.ToString(); }
            if (!String.IsNullOrEmpty(settings.City)) { cookie[SettingNames.City] = settings.City; }
            if (!String.IsNullOrEmpty(settings.Country)) { cookie[SettingNames.Country] = settings.Country; }
            if (settings.Latitude != null) { cookie[SettingNames.Latitude] = settings.Latitude.ToString(); }
            if (settings.Longitude != null) { cookie[SettingNames.Longitude] = settings.Longitude.ToString(); }
            if (settings.TimeOffset != null) { cookie[SettingNames.TimeOffset] = settings.TimeOffset.ToString(); }
            cookie.Expires = DateTime.MaxValue;
        }

        private void ReadSettingsFromCookie(SettingsModel settings)
        {
            var cookie = Request.Cookies[CookieName];
            settings.AsrMethod = ReadValue(cookie, settings.AsrMethod, SettingNames.AsrMethod, t => (JuristicMethod)Enum.Parse(typeof(JuristicMethod), t));
            settings.CalculationMethod = ReadValue(cookie, settings.CalculationMethod, SettingNames.CalculationMethod, t => (CalculationMethod)Enum.Parse(typeof(CalculationMethod), t));
            settings.City = ReadValue(cookie, settings.City, SettingNames.City, t => t);
            settings.Country = ReadValue(cookie, settings.Country, SettingNames.Country, t => t);
            settings.Latitude = ReadValue(cookie, settings.Latitude, SettingNames.Latitude, t => Double.Parse(t));
            settings.Longitude = ReadValue(cookie, settings.Longitude, SettingNames.Longitude, t => Double.Parse(t));
            settings.TimeOffset = ReadValue(cookie, settings.TimeOffset, SettingNames.TimeOffset, t => Int32.Parse(t));
        }

        private T ReadValue<T>(HttpCookie cookie, T originalValue, String settingName, Func<String, T> convert)
        {
            if (originalValue == null)
            {
                if (cookie != null)
                {
                    var value = cookie[settingName];
                    if (value != null)
                    {
                        return convert(value);
                    }
                }
            }
            return originalValue;
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
