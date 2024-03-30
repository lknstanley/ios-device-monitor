using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Plugins.iOS.TrackingUsage
{
    [ Serializable ]
    public class Stat
    {
        public CPUUsage cpuUsage;
        [ FormerlySerializedAs( "memoryUsage" ) ]
        public RamUsage ramUsage;
        public GPUUsage gpuUsage;
    }

    [ Serializable ]
    public class CPUUsage
    {
        public float idle;
        public float nice;
        public float system;
        public float user;
    }
    
    [ Serializable ]
    public class RamUsage
    {
        public float active;
        public float wired;
        public float compressed;
        public float free;
        public float inactive;
    }
    
    [ Serializable ]
    public class GPUUsage
    {
        public float allocated;
        public float max;
    }
    
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