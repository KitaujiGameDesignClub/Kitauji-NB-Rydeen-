using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 
/// </summary>
public class StaticVideoPlayer : MonoBehaviour
{
    public static VideoPlayer videoPlayer;
    
    private AudioSource audioSource;


    private void Awake()
    {
       
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = Settings.SettingsContent.MusicVolume;
    }

    /// <summary>
    /// 获取当前视频播放多长时间了
    /// </summary>
    /// <returns></returns>
    public static float GetTime()
    {
        return videoPlayer.frame / 60f;
    }

    
  
    //音量控制，用事件组
}
