
using Microsoft.Azure.Kinect.Sensor;
using System.IO;
using System.Timers;
using Console = KinectImuRecorder.DateTimeConsole;

namespace KinectRecorder.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var app = new App();
            await app.ExecuteAsync(args);
        }
    }


}