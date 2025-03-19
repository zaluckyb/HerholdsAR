using UnityEngine;
using UnityEngine.Video;

public class QuestVideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string videoFileName = "Scene1.mp4";

    void Start()
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Videos/360/", videoFileName);
        
        #if UNITY_ANDROID && !UNITY_EDITOR
            filePath = filePath; // Android reads directly from streamingAssetsPath
        #else
            filePath = "file://" + filePath;
        #endif

        videoPlayer.url = filePath;
        videoPlayer.Play();
    }
}
