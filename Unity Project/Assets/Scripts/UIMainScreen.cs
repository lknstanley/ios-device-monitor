using System;
using Plugins.iOS.TrackingUsage;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIMainScreen : MonoBehaviour
{
    private const string FormatStatStr = "{0}: {1}";

    [Header("Reference")]
    public GameObject statContainer;
    public Button startTrackingBtn;
    public Button stopTrackingBtn;
    
    [ Header( "CPU Usage UI" ) ]
    public TextMeshProUGUI cpuSystemLbl;
    public Slider cpuSystemSlider;
    public TextMeshProUGUI cpuUserLbl;
    public Slider cpuUserSlider;
    public TextMeshProUGUI cpuIdleLbl;
    public Slider cpuIdleSlider;
    public TextMeshProUGUI cpuNiceLbl;
    public Slider cpuNiceSlider;

    [ Header( "RAM Usage UI" ) ]
    public TextMeshProUGUI ramFreeLbl;
    public Slider ramFreeSlider;
    public TextMeshProUGUI ramWiredLbl;
    public Slider ramWiredSlider;
    public TextMeshProUGUI ramActiveLbl;
    public Slider ramActiveSlider;
    public TextMeshProUGUI ramInactiveLbl;
    public Slider ramInactiveSlider;
    public TextMeshProUGUI ramCompressedLbl;
    public Slider ramCompressedSlider;
    
    [Header("GPU Usage UI")]
    public TextMeshProUGUI gpuAllocatedLbl;
    public Slider gpuAllocatedSlider;

    private void Start()
    {
        UpdateUIState();
    }

    void UpdateUIState()
    {
        // Hide the dashboard at the beginning
        statContainer.SetActive( !DeviceTracker.Instance.IsNowTracking() );
        startTrackingBtn.interactable = DeviceTracker.Instance.IsNowTracking();
        stopTrackingBtn.interactable = !DeviceTracker.Instance.IsNowTracking();
    }
    
    #region Event Handlers

    public void OnStartTrackingClicked()
    {
        if ( DeviceTracker.Instance.StartTracking(OnStatReceived) )
        {
            statContainer.SetActive( true );
            UpdateUIState();
        }
    }

    public void OnStopTrackingClicked()
    {
        DeviceTracker.Instance.StopTracking();
        statContainer.SetActive( false );
        UpdateUIState();
    }

    public void OnStatReceived(Stat stat)
    {
        if ( stat != null )
        {
            // CPU Usage
            // - Set the maximum value of the sliders
            cpuSystemSlider.maxValue = cpuUserSlider.maxValue = cpuIdleSlider.maxValue = cpuNiceSlider.maxValue = 100;
            // - Mapping data to the UI
            cpuSystemLbl.text = String.Format( FormatStatStr, "System: ", $"{stat.cpuUsage.system:0.00}%" );
            cpuSystemSlider.value = stat.cpuUsage.system;
            cpuUserLbl.text = String.Format( FormatStatStr, "User: ", $"{stat.cpuUsage.user:0.00}%" );
            cpuUserSlider.value = stat.cpuUsage.user;
            cpuIdleLbl.text = String.Format( FormatStatStr, "Idle: ", $"{stat.cpuUsage.idle:0.00}%" );
            cpuIdleSlider.value = stat.cpuUsage.idle;
            cpuNiceLbl.text = String.Format( FormatStatStr, "Nice: ", $"{stat.cpuUsage.nice:0.00}%" );
            cpuNiceSlider.value = stat.cpuUsage.nice;
            
            // RAM Usage
            // - Set the maximum RAM value by summing all the RAM values
            float maxRam = stat.ramUsage.active + stat.ramUsage.wired + stat.ramUsage.compressed + stat.ramUsage.free + stat.ramUsage.inactive;
            ramFreeSlider.maxValue = ramWiredSlider.maxValue = ramActiveSlider.maxValue = ramInactiveSlider.maxValue = ramCompressedSlider.maxValue = maxRam;
            // - Mapping data to the UI
            ramFreeLbl.text = String.Format( FormatStatStr, "Free: ", $"{stat.ramUsage.free:0.00}GB" );
            ramFreeSlider.value = stat.ramUsage.free;
            ramWiredLbl.text = String.Format( FormatStatStr, "Wired: ", $"{stat.ramUsage.wired:0.00}GB" );
            ramWiredSlider.value = stat.ramUsage.wired;
            ramActiveLbl.text = String.Format( FormatStatStr, "Active: ", $"{stat.ramUsage.active:0.00}GB" );
            ramActiveSlider.value = stat.ramUsage.active;
            ramInactiveLbl.text = String.Format( FormatStatStr, "Inactive: ", $"{stat.ramUsage.inactive:0.00}GB" );
            ramInactiveSlider.value = stat.ramUsage.inactive;
            ramCompressedLbl.text = String.Format( FormatStatStr, "Compressed: ", $"{stat.ramUsage.compressed:0.00}GB" );
            ramCompressedSlider.value = stat.ramUsage.compressed;
            
            // GPU Usage
            // - Set the maximum GPU value
            gpuAllocatedSlider.maxValue = stat.gpuUsage.max;
            // - Mapping data to the UI
            gpuAllocatedLbl.text = String.Format( FormatStatStr, "Used: ", $"{stat.gpuUsage.allocated:0.00}MB / Max: {stat.gpuUsage.max}MB" );
            gpuAllocatedSlider.value = stat.gpuUsage.allocated;
        }
    }

    #endregion
}
