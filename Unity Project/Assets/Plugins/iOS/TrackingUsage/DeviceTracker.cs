using System;
using System.Runtime.InteropServices;
using AOT;
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
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void Native_OnStatReceived([MarshalAs(UnmanagedType.LPStr), In] string stat );

        [ DllImport( "__Internal" ) ]
        private static extern void startTracking(
            [ MarshalAs( UnmanagedType.FunctionPtr ) ] Native_OnStatReceived onStatReceived );
        
        [MonoPInvokeCallback(typeof(Native_OnStatReceived))]
        private static void BrdigeToNative_OnStatReceived(string message)
        {
            Instance.Bridge_OnStatReceived(JsonUtility.FromJson<Stat>( message ));
        }
        
        public Action<Stat> Bridge_OnStatReceived;

        [DllImport("__Internal")]
        private static extern void stopTracking(); // Return type is changed to IntPtr

        private bool isTracking = false;
        
        public bool StartTracking(Action<Stat> onStatReceived)
        {
            if ( isTracking ) return false;
            startTracking(BrdigeToNative_OnStatReceived);
            Bridge_OnStatReceived += onStatReceived;
            isTracking = !isTracking;
            return true;
        }
        
        public void StopTracking()
        {
            if ( !isTracking ) return;
            stopTracking();
            Bridge_OnStatReceived = null;
            isTracking = !isTracking;
        }

        public bool IsNowTracking() => !isTracking;

#endif
    }
}