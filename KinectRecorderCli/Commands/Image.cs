
using Microsoft.Azure.Kinect.Sensor;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Console = KinectImuRecorder.DateTimeConsole;

namespace KinectRecorder.Cli.Commands
{
    class Image
    {
        public static BitmapSource CreateBitmapSource(Microsoft.Azure.Kinect.Sensor.Image image, double dpiX = 300, double dpiY = 300)
        {
            PixelFormat pixelFormat;

            // Take a new reference on the image to ensure that the object
            // cannot be disposed by another thread while we have a copy of its UnsafeBufferPointer
            using (Microsoft.Azure.Kinect.Sensor.Image reference = image.Reference())
            {

                switch (reference.Format)
                {
                    case ImageFormat.ColorBGRA32:
                        pixelFormat = PixelFormats.Bgra32;
                        break;
                    case ImageFormat.Depth16:
                    case ImageFormat.IR16:
                        pixelFormat = PixelFormats.Gray16;
                        break;
                    default:
                        throw new AzureKinectException($"Pixel format {reference.Format} cannot be converted to a BitmapSource");
                }

                // BitmapSource.Create copies the unmanaged memory, so there is no need to keep
                // a reference after we have created the BitmapSource objects

                unsafe
                {
                    using (var pin = reference.Memory.Pin())
                    {
                        BitmapSource source = BitmapSource.Create(
                                    reference.WidthPixels,
                                    reference.HeightPixels,
                                    dpiX,
                                    dpiY,
                                    pixelFormat,
                                    /* palette: */ null,
                                    (IntPtr)pin.Pointer,
                                    checked((int)reference.Size),
                                    reference.StrideBytes);
                        return source;
                    }
                }
            }

        }
        public static async Task<int> ImageCapture(CaptureOptions captureOptions)
        {

            // See https://aka.ms/new-console-template for more information
            Console.WriteLine("Starting Capture");

            using (Device device = Device.Open(0))
            {
                device.StartCameras(new DeviceConfiguration
                {
                    ColorFormat = ImageFormat.ColorBGRA32,
                    ColorResolution = ColorResolution.R3072p,
                    DepthMode = DepthMode.WFOV_2x2Binned,
                    SynchronizedImagesOnly = true,
                    CameraFPS = FPS.FPS5,
                });

                int colorWidth = device.GetCalibration().ColorCameraCalibration.ResolutionWidth;
                int colorHeight = device.GetCalibration().ColorCameraCalibration.ResolutionHeight;


                // Allocate image buffers for us to manipulate
                using (var transformedDepth = new Microsoft.Azure.Kinect.Sensor.Image(ImageFormat.Depth16, colorWidth, colorHeight))
                using (var outputColorImage = new Microsoft.Azure.Kinect.Sensor.Image(ImageFormat.ColorBGRA32, colorWidth, colorHeight))
                using (Transformation transform = device.GetCalibration().CreateTransformation())
                {

                    if (!Environment.Is64BitProcess)
                    {
                        // In 32-bit the BitmapSource memory runs out quickly and we can hit OutOfMemoryException.
                        // Force garbage collection in each loop iteration to keep memory in check.
                        GC.Collect();
                    }

                    // Wait for a capture on a thread pool thread
                    Capture capture = device.GetCapture();

                    Console.WriteLine("Image Captured");

                    var bitmapSource = CreateBitmapSource(capture.Color);
                    bitmapSource.Freeze();

                    // Create file
                    var filename = captureOptions.Filename;
                    if (filename == null)
                    {
                        filename = String.Format("{0:yyyyMMdd-HHmmss}.png", DateTime.Now);
                    }

                    using (var fileStream = new FileStream(filename, FileMode.Create))
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                        encoder.Save(fileStream);
                    }
                    Console.WriteLine($"Capture Image written to {filename}");

                }
            }
            return 1;
        }
    }
}