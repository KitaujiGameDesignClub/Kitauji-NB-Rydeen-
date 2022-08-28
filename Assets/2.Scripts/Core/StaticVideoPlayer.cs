using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 
/// </summary>
public class StaticVideoPlayer : MonoBehaviour
{
    private static VideoPlayer videoPlayer;

    //24:让左上角显示的帧数与PR中同步那个不
    public static int frame => (int)videoPlayer.frame + 20;

    public static bool isPlaying => videoPlayer.isPlaying;
    
    
    private AudioSource audioSource;

    public static void Play()
    {
        videoPlayer.Play();
    }
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
