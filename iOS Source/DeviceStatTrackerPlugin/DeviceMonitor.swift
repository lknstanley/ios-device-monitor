//
//  DeviceMonitor.swift
//  DeviceStatTrackerPlugin
//
//  Created by Stanley Lai on 30/3/2024.
//

import Foundation
import UIKit
import Metal


public class DeviceMonitor {
    public static let instance = DeviceMonitor()
    public func startTracking() {
        print("========== Start Tracking ==========")
    }
    
    public func stopTracking() -> String {
        print("========== Stop Tracking ==========")
        return "Tracking is stopped"
    }
}


@_cdecl("startTracking")
public func startTracking() {
    DeviceMonitor.instance.startTracking()
}

@_cdecl("stopTracking")
public func stopTracking() -> UnsafeMutablePointer<CChar> {
    let nativeStr = DeviceMonitor.instance.stopTracking()
    let resNsStrPtr: UnsafePointer<CChar> = (nativeStr as NSString).utf8String!
    let resNsStrPtrDup: UnsafeMutablePointer<CChar> = strdup(resNsStrPtr)
    return resNsStrPtrDup
}
