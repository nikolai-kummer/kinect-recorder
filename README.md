# kinect-recorder

Simple command-line interface to grab information from the Microsoft Kinect.
Currently implemented methods:
* retrieve static image at highest resolution
* retrieve imu information

Usage:
* Default behaviour of KinectRecorderCli.exe is to capture image and save it with datetime timestamp.
* To capture imu data every 20 ms for 2 seconds and save it to imu_data.csv: 
```KinectRecorderCli.exe -c imu -f "imu_data.csv" -t 20 -d 4000```
* To capture image and save it to output_file.png: 
```KinectRecorderCli.exe -c image -f "output_file.csv"```
