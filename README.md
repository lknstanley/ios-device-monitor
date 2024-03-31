# iOS Device Monitor Native Library for Unity

This plugin with Swift supports Unity in reading CPU, RAM, and GPU usage on iOS devices.
This repository includes a demo Unity project to showcase how to access it.

# Development Environment

| Tools | Version     |
| ----- | ----------- |
| Xcode | 15.3        |
| Unity | 2022.3.19f1 |
| MacOS | 14.4.1      |

# Instruction

## 1. Build iOS Native Library

1. Clone the project
2. Run Terminal
3. Navigate to [iOS Source](https://github.com/lknstanley/ios-device-monitor/tree/master/iOS%20Source) folder
4. Run `./build.sh`, and the latest library named in `DeviceStatTrackerPlugin.framework` will be built and located in `Products` folder

PS1: Please make sure you have access right to run the build script

PS2: If you don't have access right, try running `chmod +x build.sh`

## 2. (Optional) Test with the demo Unity Project on iOS devices

1. Copy `DeviceStatTrackerPlugin.framework` from `iOS Source/Products` to `Unity Project/Assets/Plugins/iOS/TrackingUsage`
2. Open Unity Project and make sure it is on iOS platform
3. Build the project and run it on XCode

# References and Credits

To make this library, I have studied the following repositories, examples and documentation:

1. [SystemKit](https://github.com/beltex/SystemKit)
   - Understand how to access the CPU and RAM usage from Swift
2. [UnityPluginXcodeTemplate](https://github.com/fuziki/UnityPluginXcodeTemplate)
   - Data transition and format between Unity and native code
   - Delegate setup between Unity and iOS native plugin with Swift
3. [Metal API Documentation - MLTDevice](https://developer.apple.com/documentation/metal/mtldevice)
   - After understanding how to make a swift library for Unity, I found this Metal API documentation and was able to use it to implement the logic for getting GPU information.
4. [Interval in Swift](https://stackoverflow.com/a/40148293)

# Demo Video

<img src="https://i.imgur.com/fiTHo60.gif" width="400">
