
using Microsoft.Azure.Kinect.Sensor;
using System.IO;
using System.Timers;
using Console = KinectImuRecorder.DateTimeConsole;

namespace KinectRecorder.Cli.Commands
{
    class Imu

    {

        private static System.Timers.Timer? triggerTimer;
        private static int captureFrequency = 50;
        private static string timestampFormat = @"hh\:mm\:ss\.fff";
        private static string floatFormat = @"0.00000";
        private static int captureTimeMs = 4000;

        static ImuSample? GetImuSample(Device inputDevice)
        {
            ImuSample? deviceSample = null;
            try
            {
                deviceSample = inputDevice.GetImuSample();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to get imu sample");
            }
            return deviceSample;
        }

        static void intializeTimedCapture(Device device, string filepath)
        {
            Console.WriteLine("Starting Timer");
            // Create a timer with a two second interval.
            triggerTimer = new System.Timers.Timer(captureFrequency);
            triggerTimer.Interval = captureFrequency;
            triggerTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e, device, filepath);

            triggerTimer.AutoReset = true;

            // Start the timer
            triggerTimer.Enabled = true;
        }

        static bool CreateFile(string filepath)
        {
            // This text is added only once to the file.
            try
            {
                if (!File.Exists(filepath))
                {
                    // Create a file to write to.
                    string createText = "TimeStamp,AccTimeStamp,AccX,AccY,AccZ,GyroTimeStamp,GyroX,GyroY,GyroZ" + Environment.NewLine;
                    File.WriteAllText(filepath, createText);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        static void AppendFile(string filepath, string content)
        {
            try
            {
                File.AppendAllText(filepath, String.Format("{0} {1}", content, Environment.NewLine));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to append sample to file");
            }
        }

        static string ImuToString(ImuSample? inputSample)
        {
            if (inputSample == null)
            {
                return String.Format("{0},,,,,,,,", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    inputSample.AccelerometerTimestamp.ToString(timestampFormat),
                    inputSample.AccelerometerSample.X.ToString(floatFormat),
                    inputSample.AccelerometerSample.Y.ToString(floatFormat),
                    inputSample.AccelerometerSample.Z.ToString(floatFormat),
                    inputSample.GyroTimestamp.ToString(timestampFormat),
                    inputSample.GyroSample.X.ToString(floatFormat),
                    inputSample.GyroSample.Y.ToString(floatFormat),
                    inputSample.GyroSample.Z.ToString(floatFormat)
                    );
            }
        }

        static void OnTimedEvent(object? sender, ElapsedEventArgs e, Device device, string filepath)
        {
            var imu = device.GetImuSample();

            if (imu == null)
            {
                Console.WriteLine("Failed to capture IMU sample");
            }
            var imuString = ImuToString(imu);
            Console.WriteLine(imuString);
            AppendFile(filepath, imuString);
        }

        public static async Task<int> ImuCapture(CaptureOptions captureOptions)
        {

            Console.WriteLine("Starting Capture");

            using (Device device = Device.Open(0))
            {
                Console.WriteLine("Device Opened Success");
                device.StartCameras(new DeviceConfiguration
                {
                    ColorFormat = ImageFormat.ColorBGRA32,
                    ColorResolution = ColorResolution.R3072p,
                    DepthMode = DepthMode.WFOV_2x2Binned,
                    SynchronizedImagesOnly = true,
                    CameraFPS = FPS.FPS5,
                });
                device.StartImu();

                // Create file
                var filename = captureOptions.Filename;
                if (filename == null)
                {
                    var datename = DateTime.Now.ToString(@"yyyyMMddHHmmss");
                    filename = String.Format("imu_{0}.csv", datename);
                }

                var success = CreateFile(filename);
                if (!success)
                {
                    Console.WriteLine("Failed to create file. Closing.");
                    device.Dispose();
                }

                var imu = device.GetImuSample();
                Console.WriteLine("Device IMU Started Success");

                intializeTimedCapture(device, filename);

                // Press Enter to stop capture
                Thread.Sleep(captureTimeMs);
                //Console.ReadLine();
                triggerTimer.Stop();
                triggerTimer.Dispose();

            }
            return 1;
        }
    }
}