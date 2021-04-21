using System;
using System.Text;
using Windows.Devices.Gpio;

namespace Tedd.House1.Client.Esp32.Drivers
{
    public class Light
    {
        private GpioPin _light;
        private bool _status;
        public readonly int PinNumber;

        public bool Status
        {
            get => _status;
            set
            {
                _status = value;
                Update();
            }
        }

        
        public Light(int pinNumber)
        {
            PinNumber = pinNumber;
            _light = GpioController.GetDefault().OpenPin(pinNumber);
            _light.SetDriveMode(GpioPinDriveMode.Output);
            Off();
        }

        public void On()
        {
            Status = true;
        }

        public void Off()
        {
            Status = false;
        }
        private void Update()
        {
            if (Status)
                _light.Write(GpioPinValue.High);
            else
                _light.Write(GpioPinValue.Low);
        }

    }
}
