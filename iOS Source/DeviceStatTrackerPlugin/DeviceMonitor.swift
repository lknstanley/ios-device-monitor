//
//  DeviceMonitor.swift
//  DeviceStatTrackerPlugin
//
//  Created by Stanley Lai on 30/3/2024.
//

import Foundation
import UIKit
import Metal


// Host Info Constants
private let HOST_BASIC_INFO_COUNT         : mach_msg_type_number_t =
UInt32(MemoryLayout<host_basic_info_data_t>.size / MemoryLayout<integer_t>.size)
private let HOST_LOAD_INFO_COUNT          : mach_msg_type_number_t =
UInt32(MemoryLayout<host_load_info_data_t>.size / MemoryLayout<integer_t>.size)
private let HOST_CPU_LOAD_INFO_COUNT      : mach_msg_type_number_t =
UInt32(MemoryLayout<host_cpu_load_info_data_t>.size / MemoryLayout<integer_t>.size)
private let HOST_VM_INFO64_COUNT          : mach_msg_type_number_t =
UInt32(MemoryLayout<vm_statistics64_data_t>.size / MemoryLayout<integer_t>.size)
private let HOST_SCHED_INFO_COUNT         : mach_msg_type_number_t =
UInt32(MemoryLayout<host_sched_info_data_t>.size / MemoryLayout<integer_t>.size)
private let PROCESSOR_SET_LOAD_INFO_COUNT : mach_msg_type_number_t =
UInt32(MemoryLayout<processor_set_load_info_data_t>.size / MemoryLayout<natural_t>.size)

// Declare the delegate
public typealias onUpdateStatHandler = @convention(c) (UnsafeMutablePointer<CChar>) -> Void

public class DeviceMonitor {
    public static let instance = DeviceMonitor()
    private var framesRendered = 0
    let machHost = mach_host_self()
    var loadPrevious = host_cpu_load_info()
    let mtlDevice: MTLDevice? = MTLCreateSystemDefaultDevice()
    let PAGE_SIZE = vm_kernel_page_size
    var callback: onUpdateStatHandler?
    var timer: Timer?
    
    init() {}
    
    func startTracking(handler: onUpdateStatHandler) {
        print("========== Start Tracking on Native Side ==========")
        self.callback = handler
        self.timer = Timer.scheduledTimer(timeInterval: 1, target: self, selector: #selector(doTracking), userInfo: nil, repeats: true)
    }
    
    func startTracking() {
        print("========== Start Tracking on Native Side ==========")
    }
    
    @objc func doTracking() {
        let cpuUsage = cpuUsage()
        let gpuUsage = gpuUsage()
        let ramUsage = ramUsage()
        let trackedData: [String: Any] = [
            "cpuUsage": [
                "system": cpuUsage.system,
                "user": cpuUsage.user,
                "idle": cpuUsage.idle,
                "nice": cpuUsage.nice
            ],
            "gpuUsage": [
                "max": gpuUsage.max,
                "allocated": gpuUsage.curr
            ],
            "ramUsage": [
                "free": ramUsage.free,
                "active": ramUsage.active,
                "inactive": ramUsage.inactive,
                "wired": ramUsage.wired,
                "compressed": ramUsage.compressed
            ]
        ]
        
        // Call handler and send data to Unity
        callback?(
            Utils.convertStringToCSString(
                text: Utils.convertDictionaryToJsonString(dictionary: trackedData)
            )
        )
    }
    
    func stopTrackingWithInterval() -> Void {
        self.timer?.invalidate()
        self.timer = nil
        self.callback = nil
    }
    
    func stopTracking() -> String {
        let cpuUsage = cpuUsage()
        let gpuUsage = gpuUsage()
        let ramUsage = ramUsage()
        let trackedData: [String: Any] = [
            "cpuUsage": [
                "system": cpuUsage.system,
                "user": cpuUsage.user,
                "idle": cpuUsage.idle,
                "nice": cpuUsage.nice
            ],
            "gpuUsage": [
                "max": gpuUsage.max,
                "allocated": gpuUsage.curr
            ],
            "ramUsage": [
                "free": ramUsage.free,
                "active": ramUsage.active,
                "inactive": ramUsage.inactive,
                "wired": ramUsage.wired,
                "compressed": ramUsage.compressed
            ]
        ]
        
        return Utils.convertDictionaryToJsonString(dictionary: trackedData)
    }
    
    func hostCPULoadInfo() -> host_cpu_load_info {
        var size     = HOST_CPU_LOAD_INFO_COUNT
        let hostInfo = host_cpu_load_info_t.allocate(capacity: 1)
        _ = hostInfo.withMemoryRebound(to: integer_t.self, capacity: Int(size)) {
            host_statistics(machHost, HOST_CPU_LOAD_INFO,
                            $0,
                            &size)
        }
        let data = hostInfo.move()
        hostInfo.deallocate()
        return data
    }
    
    func VMStatistics64() -> vm_statistics64 {
            var size     = HOST_VM_INFO64_COUNT
            let hostInfo = vm_statistics64_t.allocate(capacity: 1)
            let result = hostInfo.withMemoryRebound(to: integer_t.self, capacity: Int(size)) {
                host_statistics64(machHost,
                                  HOST_VM_INFO64,
                                  $0,
                                  &size)
            }
            let data = hostInfo.move()
            hostInfo.deallocate()
            return data
        }
    
    // Get CPU usage by host_cpu_load_info_t
    private func cpuUsage() -> (
        system: Double,
        user: Double,
        idle: Double,
        nice: Double) {
            let load = hostCPULoadInfo()
            
            let userDiff = Double(load.cpu_ticks.0 - loadPrevious.cpu_ticks.0)
            let sysDiff  = Double(load.cpu_ticks.1 - loadPrevious.cpu_ticks.1)
            let idleDiff = Double(load.cpu_ticks.2 - loadPrevious.cpu_ticks.2)
            let niceDiff = Double(load.cpu_ticks.3 - loadPrevious.cpu_ticks.3)
            
            let totalTicks = sysDiff + userDiff + niceDiff + idleDiff
            
            let sys  = sysDiff  / totalTicks * 100.0
            let user = userDiff / totalTicks * 100.0
            let idle = idleDiff / totalTicks * 100.0
            let nice = niceDiff / totalTicks * 100.0
            
            loadPrevious = load
            
            return (sys, user, idle, nice)
        }
    
    // Get RAM usage
    private func ramUsage() -> (
        free: Double,
        active: Double,
        inactive: Double,
        wired: Double,
        compressed: Double) {
            let stats = VMStatistics64()
            
            let free     = Utils.convertByte(
                value: Double(stats.free_count) * Double(PAGE_SIZE),
                target: Utils.UnitType.GB)
            let active   = Utils.convertByte(
                value: Double(stats.active_count) * Double(PAGE_SIZE),
                target: Utils.UnitType.GB)
            let inactive = Utils.convertByte(
                value: Double(stats.inactive_count) * Double(PAGE_SIZE),
                target: Utils.UnitType.GB)
            let wired    = Utils.convertByte(
                value: Double(stats.wire_count) * Double(PAGE_SIZE),
                target: Utils.UnitType.GB)
            
            // Result of the compression. This is what you see in Activity Monitor
            let compressed = Utils.convertByte(
                value: Double(stats.compressor_page_count) * Double(PAGE_SIZE),
                target: Utils.UnitType.GB)
            
            return (free, active, inactive, wired, compressed)
        }
    
    // Get the GPU usage by Metal API
    private func gpuUsage() -> (max: UInt64, curr: Int) {
        let maxGPUMem = mtlDevice?.recommendedMaxWorkingSetSize
        let currAllocatedGPUMem = mtlDevice?.currentAllocatedSize
        return (Utils.convertByte(value: maxGPUMem!, target: Utils.UnitType.MB),
                Utils.convertByte(value: currAllocatedGPUMem!, target: Utils.UnitType.MB))
    }
}


// Expose to Unity
@_cdecl("startTrackingWithInterval")
public func startTrackingWithInterval(handler: onUpdateStatHandler) -> Void {
    DeviceMonitor.instance.startTracking(handler: handler)
}

@_cdecl("stopTrackingWithInterval")
public func stopTrackingWithInterval() -> Void {
    DeviceMonitor.instance.stopTrackingWithInterval()
}

@_cdecl("startTracking")
public func startTracking() -> Void {
    DeviceMonitor.instance.startTracking()
}

@_cdecl("stopTracking")
public func stopTracking() -> UnsafeMutablePointer<CChar> {
    return Utils.convertStringToCSString(text: DeviceMonitor.instance.stopTracking())
}
