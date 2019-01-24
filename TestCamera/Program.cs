using System;
using System.IO;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace TestCamera
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World2!232323");
            TestCaptureImage();
        }
        static void TestCaptureImage()
        {
            var pictureBytes = Pi.Camera.CaptureImageJpeg(640, 480);
            var targetPath = "/home/pi/picture.jpg";
            if (File.Exists(targetPath))
                File.Delete(targetPath);

            File.WriteAllBytes(targetPath, pictureBytes);
            Console.WriteLine($"Took picture -- Byte count: {pictureBytes.Length}");

            var sensor = new HCSR04();
            for (var i = 0; i < 20; i++)
            {
                Console.WriteLine($"Obecny dystans : {sensor.Distance}");
                System.Threading.Thread.Sleep(2000);
            }
        }


        public static void TestLedBlinking()
        {
            // Get a reference to the pin you need to use.
            // All 3 methods below are exactly equivalent
            var blinkingPin = Pi.Gpio[0];
            blinkingPin = Pi.Gpio[WiringPiPin.Pin00];
            blinkingPin = Pi.Gpio.Pin00;

            // Configure the pin as an output
            blinkingPin.PinMode = GpioPinDriveMode.Output;

            // perform writes to the pin by toggling the isOn variable
            var isOn = false;
            for (var i = 0; i < 20; i++)
            {
                isOn = !isOn;
                blinkingPin.Write(isOn);
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
