using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;

namespace TestCamera
{
    public class HCSR04
    {
        private GpioPin triggerPin { get; set; }
        private GpioPin echoPin { get; set; }
        private const double SPEED_OF_SOUND_METERS_PER_SECOND = 343;

        public HCSR04()
        {
            GpioController controller = GpioController.Instance;

            //initialize trigger pin.
			triggerPin = controller.Pin18;
            triggerPin.PinMode = GpioPinDriveMode.Output;

            //initialize echo pin.
            this.echoPin = controller.Pin24;
            this.echoPin.PinMode = GpioPinDriveMode.Input;
        }

		private static Stopwatch stopWatch = new Stopwatch();
              
        public double Distance
        {
            get
            {              
				triggerPin.Write(GpioPinValue.High);
                Sleep(100000);
                this.triggerPin.Write(GpioPinValue.Low);

				var t = Task.Run(() =>
			   {
				   stopWatch.Reset();

				   while (!echoPin.Read())
				   {
					   stopWatch.Start();
				   }

				   while (echoPin.Read())
				   {
					   stopWatch.Stop();
				   }

				   return stopWatch.Elapsed.TotalSeconds;
				});
                
				bool isCompleted = t.Wait(TimeSpan.FromMilliseconds(200));

                if (isCompleted)
                {
					return (SPEED_OF_SOUND_METERS_PER_SECOND / 2) * t.Result;
                   
                }
                else
                {
					return -1d;
                }                     
            }
        }               

        private static ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        public static void Sleep(int delayMicroseconds)
        {
            manualResetEvent.WaitOne(
                TimeSpan.FromMilliseconds((double)delayMicroseconds / 1000d));
        }
    }
}
