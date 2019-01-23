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
            this.triggerPin = controller.Pin27;
            this.triggerPin.PinMode = GpioPinDriveMode.Output;

            //initialize echo pin.
            this.echoPin = controller.Pin28;
            this.echoPin.PinMode = GpioPinDriveMode.Input;


        }

        private double LengthOfHighPulse
        {
            get
            {
                // The sensor is triggered by a logic 1 pulse of 10 or more microseconds.
                // We give a short logic 0 pulse first to ensure a clean logic 1.
                this.triggerPin.Write(GpioPinValue.Low);
                Sleep(5);
                this.triggerPin.Write(GpioPinValue.High);
                Sleep(10);
                this.triggerPin.Write(GpioPinValue.Low);

                // Read the signal from the sensor: a HIGH pulse whose
                // duration is the time (in microseconds) from the sending
                // of the ping to the reception of its echo off of an object.
                return GetTimeUntilNextEdge(echoPin, GpioPinValue.High, 100);


            }
        }

        public double Distance
        {
            get
            {
                // convert the time into a distance
                // duration of pulse * speed of sound (343m/s)
                // remember to divide by two because we're measuring the time for the signal to reach the object, and return.
                return (SPEED_OF_SOUND_METERS_PER_SECOND / 2) * LengthOfHighPulse;
            }
        }


        private static Stopwatch stopWatch = new Stopwatch();

        public static double GetTimeUntilNextEdge(GpioPin pin, GpioPinValue edgeToWaitFor, int maximumTimeToWaitInMilliseconds)
        {
            var t = Task.Run(() =>
            {
                stopWatch.Reset();

                while (pin.ReadValue() != edgeToWaitFor) { }

                stopWatch.Start();

                while (pin.ReadValue() == edgeToWaitFor) { }

                stopWatch.Stop();

                return stopWatch.Elapsed.TotalSeconds;
            });

            bool isCompleted = t.Wait(TimeSpan.FromMilliseconds(maximumTimeToWaitInMilliseconds));

            if (isCompleted)
            {
                return t.Result;
            }
            else
            {
                return -1d;
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
