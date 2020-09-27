using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinates
{
class RaDecToAltAz
    {
        double RA = 250.425; // 16 h 41.7 m * 15
        double Dec = 36.46667; // 35 ° 30 m
        double Lat = 51.76954; // normal decimal latitude
        double Long = 4.605606; // normal decimal latitude

        /// <summary>
        /// DateTime will be set to current UTC time
        /// </summary>
        /// <param name="RA">The right ascension in decimal value</param>
        /// <param name="Dec">The declination in decimal value</param>
        /// <param name="Lat">The latitude in decimal value</param>
        /// <param name="Long">The longitude in decimal value</param>
        /// <returns>The altitude and azimuth in decimal value</returns>
        public static AltAz Calculate(double RA, double Dec, double Lat, double Long)
        {
            return Calculate(RA, Dec, Lat, Long, DateTime.UtcNow);
        }

        /// <summary>
        /// </summary>
        /// <param name="RA">The right ascension in decimal value</param>
        /// <param name="Dec">The declination in decimal value</param>
        /// <param name="Lat">The latitude in decimal value</param>
        /// <param name="Long">The longitude in decimal value</param>
        /// <param name="Date">The date(time) in UTC</param>
        /// <returns>The altitude and azimuth in decimal value</returns>
        public static AltAz Calculate(double RA, double Dec, double Lat, double Long, DateTime Date)
        {
            // Day offset and Local Siderial Time
            double dayOffset = (Date - new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc)).TotalDays;
            double LST = (100.46 + 0.985647 * dayOffset + Long + 15 * (Date.Hour + Date.Minute / 60d) + 360) % 360;

            // Hour Angle
            double HA = (LST - RA + 360) % 360;

            // HA, DEC, Lat to Alt, AZ
            double x = Math.Cos(HA * (Math.PI / 180)) * Math.Cos(Dec * (Math.PI / 180));
            double y = Math.Sin(HA * (Math.PI / 180)) * Math.Cos(Dec * (Math.PI / 180));
            double z = Math.Sin(Dec * (Math.PI / 180));

            double xhor = x * Math.Cos((90 - Lat) * (Math.PI / 180)) - z * Math.Sin((90 - Lat) * (Math.PI / 180));
            double yhor = y;
            double zhor = x * Math.Sin((90 - Lat) * (Math.PI / 180)) + z * Math.Cos((90 - Lat) * (Math.PI / 180));

            double az = Math.Atan2(yhor, xhor) * (180 / Math.PI) + 180;
            double alt = Math.Asin(zhor) * (180 / Math.PI);

            return new AltAz() { Alt = alt, Az = az };
        }

        public class AltAz
        {
            public double Alt {get;set;}
            public double Az {get;set;}
        }
    }
}
