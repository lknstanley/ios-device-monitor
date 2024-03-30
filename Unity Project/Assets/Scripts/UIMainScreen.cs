using System;
using Plugins.iOS.TrackingUsage;
using TMPro;
using UnityEngine;

public class UIMainScreen : MonoBehaviour
{
    private const string FormatStatStr = "{0}: {1}"; 
    public TextMeshProUGUI cpuUsageText;
    public TextMeshProUGUI memoryUsageText;
    public TextMeshProUGUI gpuUsageText;
    
    
    #region Event Handlers

    public void OnStartTrackingClicked()
    {
        if ( DeviceTracker.Instance.StartTracking() )
        {
            cpuUsageText.text = String.Format( FormatStatStr, "CPU", "Tracking..." );
            memoryUsageText.text = String.Format( FormatStatStr, "RAM", "Tracking..." );
            gpuUsageText.text = String.Format( FormatStatStr, "GPU", "Tracking..." );
        }
    }

    public void OnStopTrackingClicked()
    {
        var stat = DeviceTracker.Instance.StopTracking();
        if ( stat != null )
        {
            cpuUsageText.text = String.Format( FormatStatStr, "CPU", $"Idle: {stat.cpuUsage.idle:0.00}%, Nice: {stat.cpuUsage.nice:0.00}%, System: {stat.cpuUsage.system:0.00}%, User: {stat.cpuUsage.user:0.00}" );
            memoryUsageText.text = String.Format( FormatStatStr, "RAM", $"Compressed: {stat.ramUsage.compressed:0.00}GB, Free: {stat.ramUsage.free:0.00}GB, Active: {stat.ramUsage.active:0.00}GB, Inactive: {stat.ramUsage.inactive:0.00}GB, Wired: {stat.ramUsage.wired:0.00}GB" );
            gpuUsageText.text = String.Format( FormatStatStr, "GPU", $"Allocated: {stat.gpuUsage.allocated:0.00}MB, Max: {stat.gpuUsage.max:0.00}MB" );
        }
    }
    
    public void OnStatUpdate(double cpu, double memory, double gpu)
    {
        cpuUsageText.text = $"CPU: {cpu.ToString()}";
        memoryUsageText.text = $"CPU: {memory.ToString()}";
        gpuUsageText.text = $"CPU: {gpu.ToString()}";
    }

    #endregion
}
