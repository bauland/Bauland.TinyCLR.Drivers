using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;

namespace MAX6675
{
    /// <summary>
    /// MAX6675 Driver
    /// </summary>
    public class MAX6675
    {
        private readonly GpioPin _csPin;
        private readonly SpiDevice _device;
        private readonly byte[] buffer;
        private float _data;

        /// <summary>
        /// Constructor of MAX6675 sensor
        /// </summary>
        /// <param name="spiName">Name of Spi bus</param>
        /// <param name="csPin">Number of the pin</param>
        public MAX6675(string spiName, int csPin)
        {
            buffer = new byte[2];
            _csPin = GpioController.GetDefault().OpenPin(csPin);
            var settings = new SpiConnectionSettings()
            {
                ChipSelectType = SpiChipSelectType.Gpio,
                ChipSelectLine = _csPin,
                ChipSelectActiveState = false,
                Mode = SpiMode.Mode0,
                ClockFrequency = 4_000_000,
            };
            var _spiController = SpiController.FromName(spiName);
            _device = _spiController.GetDevice(settings);

            // Reset
            _csPin.SetDriveMode(GpioPinDriveMode.Output);
            _csPin.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Get temperature
        /// </summary>
        /// <returns>temperature in Celsius</returns>
        public float GetTemperatureInCelsius()
        {
            _device.Read(buffer);
            // Only bits from 4 to 15 are significant: they are a 12bit CAN
            _data = (buffer[0] << 5) + (buffer[1] >> 3);
            // Temperature's precision is 0.25 °C 
            return _data / 4.0f;
        }
    }
}
