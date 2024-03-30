using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Plguins.iOS.TrackingUsage
{
    public class DeviceTracker : MonoBehaviour
    {
        // Singleton
        private static DeviceTracker _instance;
        public static DeviceTracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<DeviceTracker>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("DeviceTracker").AddComponent<DeviceTracker>();
                    }
                }
                return _instance;
            }
        }
        
        #if UNITY_IOS
        
        
        [DllImport("__Internal")]
        private static extern void startTracking();

        [DllImport("__Internal")]
        private static extern string stopTracking(); // Return type is changed to IntPtr
        
        public void StartTracking()
        {
            startTracking();
        }
        
        public void StopTracking()
        {
            string rtn = stopTracking();
            Debug.Log( rtn );
        }
        
        #endif
    }
}