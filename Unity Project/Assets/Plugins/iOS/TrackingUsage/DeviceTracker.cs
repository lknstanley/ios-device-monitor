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

        [ MonoPInvokeCallback( typeof( Native_OnStatReceived ) ) ]
        private static void BrdigeToNative_OnStatReceived( string message )
        {
            Instance.Bridge_OnStatReceived( JsonUtility.FromJson< Stat >( message ) );
        }

        /// <summary>
        /// Native Method - Start Tracking the Device
        /// </summary>
        /// <param name="onStatReceived">Pointer to callback handler</param>
        [ DllImport( "__Internal" ) ]
        private static extern void startTracking(
            [ MarshalAs( UnmanagedType.FunctionPtr ) ] Native_OnStatReceived onStatReceived );
        
        /// <summary>
        /// Action to handle the received stat from native
        /// </summary>
        public Action<Stat> Bridge_OnStatReceived;
        

        /// <summary>
        /// Native Method - Stop Tracking the Device
        /// </summary>
        [DllImport("__Internal")]
        private static extern void stopTracking();

        /// <summary>
        /// Current status of tracking
        /// </summary>
        private bool isTracking = false;
        
        /// <summary>
        /// Start tracking the device
        /// </summary>
        /// <param name="onStatReceived">The handler that implements the logic when receiving stat from native.</param>
        /// <returns>Flag of succeed</returns>
        public bool StartTracking(Action<Stat> onStatReceived)
        {
            if ( isTracking ) return false;
            startTracking(BrdigeToNative_OnStatReceived);
            Bridge_OnStatReceived += onStatReceived;
            isTracking = !isTracking;
            return true;
        }
        
        /// <summary>
        /// Stop tracking the device
        /// </summary>
        public void StopTracking()
        {
            if ( !isTracking ) return;
            stopTracking();
            Bridge_OnStatReceived = null;
            isTracking = !isTracking;
        }

        /// <summary>
        /// Check the current device is under tracking or not
        /// </summary>
        /// <returns>Flag of tracking</returns>
        public bool IsNowTracking() => !isTracking;

        #endif
    }
}