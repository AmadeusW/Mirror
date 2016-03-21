using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using Windows.UI.Xaml;

namespace AmadeusW.Mirror.GUI.Controllers
{
    public delegate void MeasurementEventHandler(int measurement1, int measurement2, ref bool shouldDebounce);

    public class ProximityController : IDisposable
    {
        public static ProximityController Instance { get; private set; }
        public event MeasurementEventHandler OnMeasurement;

        #region Private fields

        private ProximityController() { }

        SpiDevice ADC;
        DispatcherTimer timer;

        int average1 = 0;
        int average2 = 0;
        int oldAverage1 = 0;
        int oldAverage2 = 0;

        // The ADC response is 10 bits. We can fit it into 2 bytes. We add one byte padding. Hence, 
        byte[] responseBuffer = new byte[3];
        // In SPI communication, for every byte we want to receive, we send one byte
        // Therefore, the request buffers also are 3 bytes long
        byte[] range1Query = new byte[3] { 0x01, 0x80, 0 };
        byte[] range2Query = new byte[3] { 0x01, 0x90, 0 };

        TimeSpan interval = TimeSpan.FromSeconds(1);
        /// <summary>
        /// Responsible for falling back to slow polling after a period of inactivity
        /// </summary>
        private int inactivityCounter;
        /// <summary>
        /// Responsible for not issuing OnMeasurement event for a short while, to provide debouncing.
        /// </summary>
        private int skipCount;

        #endregion

        internal TimeSpan Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;

                if (timer != null)
                    timer.Interval = value;
            }
        }

        public static async Task<ProximityController> CreateNewAsync()
        {
            if (Instance != null)
            {
                Instance.Dispose();
            }

            var controller = new ProximityController();
            try
            {
                await controller.initSpi();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("InitSpi threw exception.", ex);
            }
            try
            {
                controller.initTimer();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("InitTimer threw exception.", ex);
            }
            Instance = controller;
            return Instance;
        }

        public void Dispose()
        {
            if (ADC != null)
            {
                ADC.Dispose();
            }
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
            }
        }

        #region Private methods

        private async Task initSpi()
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

        private void initTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = Interval;
            timer.Tick += Timer_Tick;
            timer.Start();
            System.Diagnostics.Debug.WriteLine("InitTimer successful");
        }

        private void Timer_Tick(object sender, object e)
        {
            if (ADC == null)
                return;

            ADC.TransferFullDuplex(range1Query, responseBuffer);
            var result1 = ConvertToInt(responseBuffer);

            ADC.TransferFullDuplex(range2Query, responseBuffer);
            var result2 = ConvertToInt(responseBuffer);

            if (result1 > 280 || result2 > 280)
            {
                Interval = TimeSpan.FromMilliseconds(50);
                inactivityCounter = 0;
            }
            else
            {
                inactivityCounter++;
            }
            if (inactivityCounter > 200)
            {
                Interval = TimeSpan.FromSeconds(1);
            }

            average1 = (result1 * 2 + average1 + oldAverage1) / 4;
            average2 = (result2 * 2 + average2 + oldAverage2) / 4;

            oldAverage1 = average1;
            oldAverage2 = average2;

            if (skipCount > 0)
            {
                skipCount--;
                return;
            }
            bool shouldDebounce = false;
            OnMeasurement?.Invoke(average1, average2, ref shouldDebounce);

            if (shouldDebounce)
            {
                skipCount = 20;
            }
        }

        /// <summary>
        /// Converts the array of 3 bytes into an integer.
        /// Uses the 10 least significant bits, and discards the rest
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int ConvertToInt(byte[] data)
        {
            int result = 0;
            result = data[1] & 0x03;
            result <<= 8;
            result += data[2];
            return result;
        }

        #endregion
    }
}
