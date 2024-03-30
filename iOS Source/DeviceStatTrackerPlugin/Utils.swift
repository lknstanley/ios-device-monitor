//
//  Utils.swift
//  DeviceStatTrackerPlugin
//
//  Created by Stanley Lai on 30/3/2024.
//

import Foundation


class Utils {
    
    public enum UnitType {
        case Byte
        case KB
        case MB
        case GB
    }
    
    
    // Convert Swift String to Unity String
    public static func convertStringToCSString(text: String) -> UnsafeMutablePointer<CChar>{
        let swiftString = text
        let stringInUTF8Ptr: UnsafePointer<CChar> = (swiftString as NSString).utf8String!
        return strdup(stringInUTF8Ptr)
    }
    
    // Convert dictionary to json string format
    public static func convertDictionaryToJsonString(dictionary: [String: Any]) -> String {
        do {
            let jsonData = try JSONSerialization.data(withJSONObject: dictionary, options: [])
            let jsonString = String(data: jsonData, encoding: .utf8)
            return jsonString!
        } catch {
            return ""
        }
    }
    
    // Convert byte in double value to certain unit
    public static func convertByte(value: Double, target: UnitType) -> Double {
        switch(target) {
        case .Byte:
            return value
        case .KB:
            return value / 1024
        case .MB:
            return value / 1024 / 1024
        case .GB:
            return value / 1024 / 1024 / 1024
        }
    }
    
    // Convert byte in UInt64 value to certain unit
    public static func convertByte(value: UInt64, target: UnitType) -> UInt64 {
        switch(target) {
        case .Byte:
            return value
        case .KB:
            return value / 1024
        case .MB:
            return value / 1024 / 1024
        case .GB:
            return value / 1024 / 1024 / 1024
        }
    }
    
    // Convert byte in Int value to certain unit
    public static func convertByte(value: Int, target: UnitType) -> Int {
        switch(target) {
        case .Byte:
            return value
        case .KB:
            return value / 1024
        case .MB:
            return value / 1024 / 1024
        case .GB:
            return value / 1024 / 1024 / 1024
        }
    }
}
