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

    [SerializeField] TextMeshProUGUI CPUStatsText;
    [SerializeField] TextMeshProUGUI GPUStatsText;
    [Space]
    [SerializeField] TextMeshProUGUI gameVersionText;
    [Space]
    [SerializeField] TextMeshProUGUI playerPositionText;
    [SerializeField] TextMeshProUGUI chunkStatsText;

    private void Start()
    {
        LoadDebugStats();
    }

    private void LoadDebugStats()
    {
        StartCoroutine(UpdateStats());
        GridVisualization.instance.onPlayerMove += UpdatePosition;

        gameVersionText.text = Application.productName + " " + Application.version;
        SetPlayerPosition(GridVisualization.instance.playerPosition);
        int chunkIndex = GridVisualization.instance.lastPlayerChunk;
        SetChunk(GridVisualization.instance.get,);

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
    private void UpdatePosition(object sender, PlayerPositionArgs e)
    {
        SetPlayerPosition(e.playerPosition);
        SetChunk(e.chunkCoordinates, e.chunkIndex);
    }

    private void SetPlayerPosition(Vector2 position)
    {
        playerPositionText.text = ($"Position: [ {(int)position.x} , {(int)position.y} ]");
    }

    private void SetChunk(Vector2 chunkCoordinates,int chunkIndex)
    {
        chunkStatsText.text = ($"Chunk: [ {(int)chunkCoordinates.x} , {(int)chunkCoordinates.y} ]  Index: {chunkIndex}");
    }

    private void OnDisable()
    {
        GridVisualization.instance.onPlayerMove -= UpdatePosition;
    }
}
