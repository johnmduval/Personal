using System;

namespace Coordinates
{
    class Program
    {
        static void Main(string[] args)
        {

            //// Polaris
            //var raHour = 2.0;
            //var raMinute = 31.0;
            //var raSec = 0.0;

            //var decHour = 89.0;
            //var decMin = 15.0;
            //var decSec = 0.0;

            // andromeda
            var raHour = 0.0;
            var raMinute = 42.7;
            var raSec = 0.0;

            var decHour = 41.0;
            var decMin = 16.0;
            var decSec = 0.0;


            var ra = raHour + raMinute / 60.0 + raSec / 3600.0;
            var dec = decHour + decMin / 60.0 + decSec / 3600.0;

            var myLat = 42.592960;
            var myLong = -71.014687;

            var dt = DateTime.UtcNow;

            for (int i = 0; i < 12; i++)
            {
                dt = dt.AddHours(1);
                var altaz = RaDecToAltAz.Calculate(ra, dec, myLat, myLong, dt);
                Console.WriteLine($"alt={altaz.Alt:F1}\taz={altaz.Az:F1}");
            }

            Console.WriteLine("Press a key to exit");
            Console.ReadKey();
        }
    }
}
