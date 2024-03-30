using Plguins.iOS.TrackingUsage;
using TMPro;
using UnityEngine;

public class UIMainScreen : MonoBehaviour
{
    public TextMeshProUGUI cpuUsageText;
    public TextMeshProUGUI memoryUsageText;
    public TextMeshProUGUI gpuUsageText;
    
    
    #region Event Handlers

    public void OnStartTrackingClicked()
    {
        DeviceTracker.Instance.StartTracking();
    }

    public void OnStopTrackingClicked()
    {
        DeviceTracker.Instance.StopTracking();
    }
    
    public void OnStatUpdate(double cpu, double memory, double gpu)
    {
        cpuUsageText.text = $"CPU: {cpu.ToString()}";
        memoryUsageText.text = $"CPU: {memory.ToString()}";
        gpuUsageText.text = $"CPU: {gpu.ToString()}";
    }

    #endregion
}
