using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using Windows.UI.Xaml;

namespace AmadeusW.Mirror.GUI.Controllers
{
    internal class ProximityController
    {
        SpiDevice ADC;

        // The ADC response is 10 bits. We can fit it into 2 bytes. We add one byte padding. Hence, 
        byte[] responseBuffer = new byte[3];
        // In SPI communication, for every byte we want to receive, we send one byte
        // Therefore, the request buffers also are 3 bytes long
        byte[] range1Query = new byte[3] { 0x01, 0x80, 0 };
        byte[] range2Query = new byte[3] { 0x01, 0x90, 0 };


        private async void initSpi()
        {
            try
            {
                var settings = new SpiConnectionSettings(0)                         // Chip Select line 0
                {
                    ClockFrequency = 500 * 1000,                                    // Don't exceed 3.6 MHz
                    Mode = SpiMode.Mode0,
                };

                string spiAqs = SpiDevice.GetDeviceSelector("SPI0");                /* Find the selector string for the SPI bus controller          */
                var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);     /* Find the SPI bus controller device with our selector string  */
                ADC = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);     /* Create an SpiDevice with our bus controller and SPI settings */
                System.Diagnostics.Debug.WriteLine("InitSpi successful");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("InitSpi threw " + ex);
            }
        }

        private void initTimer()
        {
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(100),
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            ADC.TransferFullDuplex(range1Query, responseBuffer);
            var result1 = ConvertToInt(responseBuffer);

            ADC.TransferFullDuplex(range2Query, responseBuffer);
            var result2 = ConvertToInt(responseBuffer);
        }

        /// <summary>
        /// Converts the array of 3 bytes into an integer.
        /// Uses the 10 least significant bits, and discards the rest
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int ConvertToInt(byte[] data)
        {
            int result = 0;
            result = data[1] & 0x03;
            result <<= 8;
            result += data[2];
            return result;
        }
    }
}
