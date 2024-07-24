using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Profiling;

public class DebugManager : MonoBehaviour
{

    public const float updateInterval = 1f;
    [SerializeField] TextMeshProUGUI playerPositionText;
    [SerializeField] TextMeshProUGUI CPUStatsText;
    [SerializeField] TextMeshProUGUI GPUStatsText;
    [SerializeField] TextMeshProUGUI gameVersionText;

    private void Start()
    {
        Debug.Log("sss");
        LoadDebugStats();
     //   StartCoroutine(UpdateStats());
    }

    private void LoadDebugStats()
    {
        gameVersionText.text = Application.productName + " " + Application.version;
    }
    private void OnEnable()
    {
        
    }

    private void Update()
    {
        UpdateDebugStats();
    }


    IEnumerator UpdateStats()
    {
        while(true)
        {
            UpdateDebugStats();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateDebugStats()
    {
        float cpuUsage = Profiler.GetTotalAllocatedMemoryLong() / (1024.0f * 1024.0f);
        CPUStatsText.text = string.Format("CPU Usage: {0:0.0} MB", cpuUsage);

        float gpuFrameTime = Time.deltaTime * 1000.0f;
        GPUStatsText.text = "GPU Frame Time: " + gpuFrameTime.ToString("F2") + " ms";

    }

}
