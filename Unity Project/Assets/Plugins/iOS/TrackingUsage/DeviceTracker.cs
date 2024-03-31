using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Plugins.iOS.TrackingUsage
{
    public class DeviceTracker : MonoBehaviour
    {
        #region Singleton
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
        #endregion
        
        #if UNITY_IOS
        
        [DllImport("__Internal")]
        private static extern void startTracking();

        [DllImport("__Internal")]
        private static extern string stopTracking(); // Return type is changed to IntPtr

        private bool isTracking = false;
        
        public bool StartTracking()
        {
            if ( isTracking ) return false;
            startTracking();
            isTracking = !isTracking;
            return true;
        }
        
        public Stat StopTracking()
        {
            if ( !isTracking ) return null;
            string rtn = stopTracking();
            Debug.Log( rtn );
            isTracking = !isTracking;
            return JsonUtility.FromJson< Stat >( rtn );
        }
        
        #endif
    }
}