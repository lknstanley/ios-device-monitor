//
//  DeviceMonitor.swift
//  DeviceStatTrackerPlugin
//
//  Created by Stanley Lai on 30/3/2024.
//

import Foundation
import UIKit
import Metal


public class DeviceMonitor: NSObject {
    public static let instance = DeviceMonitor()
    public func startTracking() {
        print("========== Start Tracking ==========")
    }
    
    public func stopTracking() -> NSString {
        print("========== Stop Tracking ==========")
        return "Tracking is stopped"
    }
}


@_cdecl("startTracking")
public func startTracking() {
    DeviceMonitor.instance.startTracking()
}

@_cdecl("stopTracking")
public func stopTracking() -> NSString {
    return DeviceMonitor.instance.stopTracking()
}
