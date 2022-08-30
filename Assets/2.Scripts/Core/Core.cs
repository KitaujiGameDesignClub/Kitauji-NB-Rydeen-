using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Core : MonoBehaviour,IUpdate
{
    [Header("大镲部")]
    public Image clef;
    
    
    public  UnityEvent onGetButtonLeftCtrl = new UnityEvent();
    public UnityEvent onGetButtonRightCtrl = new UnityEvent();

    public UnityEvent inTime = new UnityEvent();
    public UnityEvent miss = new UnityEvent();
    
    public  UnityEvent onStart = new UnityEvent();
    public  UnityEvent onPrepare = new UnityEvent();
    
    /// <summary>
    /// 部分
    /// </summary>
    private int episode;
    
    /// <summary>
    /// 电脑可以识别的大镲时间（帧数）
    /// </summary>
    private List<int> cymbalAction;

    /// <summary>
    /// 到底几个大镲了（从0开始）0：高坂之前
    /// </summary>
    private int index;
    
    private void Awake()
    {
        Initialization();
        onStart.Invoke();

#if UNITY_EDITOR
        //编辑器模式下，永远允许跳过老师的话
        Settings.SettingsContent.hasPlayed = true;
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateManager.RegisterUpdate(this);
        StaticVideoPlayer.videoPlayer.Play();
    }

    // Update is called once per frame
   public void FastUpdate()
    {
        //游戏暂停预留
        
        
        
        if(!StaticVideoPlayer.videoPlayer.isPlaying) return;
        

        if(Input.GetKeyDown(KeyCode.LeftControl)) onGetButtonLeftCtrl.Invoke();
        if(Input.GetKeyDown(KeyCode.RightControl)) onGetButtonRightCtrl.Invoke();
        
        
        
        switch (StaticVideoPlayer.videoPlayer.frame)
        {
            //开始部分（用于制作跳过）
            //775:老师说出展现北宇治的实力之前的部分episode = 0
            case <= 775 when episode == 0:
                //空格允许跳过老师的话（在玩过之后）
                if (Input.GetKeyDown(KeyCode.Space) && Settings.SettingsContent.hasPlayed)
                {
                    StaticVideoPlayer.videoPlayer.frame = 775;
                    episode++;
                }

                break;
            
            case > 775 when episode == 1:
                onPrepare.Invoke();
                ClefFadeInAndKeyCheck();
                break;
                
        }

        
        
    }


   private void Initialization()
   {
       //读取大镲文件
       ReadYaml();
       //clef消失（不填充）
       clef.fillAmount = 0f;
   }

/// <summary>
/// 读取大镲文件
/// </summary>
   private void ReadYaml()
   {
       var yaml = YamlReadWrite.Read<YamlReadWrite.CymbalAction>(YamlReadWrite.FileName.Cymbal);

       cymbalAction = new List<int>();
      
       for (int i = 0; i < yaml.time.Length; i++)
       {
           //按照冒号分开。长度为4. 0 =1舍弃 =2有改动（相较于AS的版本） .1是分钟（1min=60s=1800)  2是秒（1s=30） 3则可以视为帧数
           string[] fix = yaml.time[i].Split(':');
          
           //lag对于整体的滞后性进行修复
           cymbalAction.Add(int.Parse(fix[3]) + int.Parse(fix[2]) * 30 + int.Parse(fix[1]) * 1800 +  Settings.SettingsContent.lag);
           
           
       }
       
       Debug.Log(cymbalAction[0]);
   }

   /// <summary>
   /// 音符（打击乐符号）淡入和按键检查
   /// </summary>
   private void ClefFadeInAndKeyCheck()
   {
       
       var frame = StaticVideoPlayer.videoPlayer.frame;
       
       //按照视频进度，对打击乐符号进行填充
       if (index == 0)
       {
           //775:老师说出展现北宇治的实力之前的部分episode = 0
           clef.fillAmount = (float)(frame - 775)  / (cymbalAction[index] - 775);
       }
       else
       {
           clef.fillAmount = (float)(frame - cymbalAction[index - 1])  / (cymbalAction[index] - cymbalAction[index - 1]);
       }
    
       
       

       //玩家按键判定
       if (frame >= cymbalAction[index] - 3 && frame <= cymbalAction[index] + 3 && Input.GetKeyDown(KeyCode.LeftControl))
       {
           if( index < cymbalAction.Count - 1)  index++;
           clef.fillAmount = 0f;
           inTime.Invoke();
       }
       //错过
       else if (frame > cymbalAction[index] + 3)
       {
           clef.fillAmount = 0f;
         if( index < cymbalAction.Count - 1)  index++;
           miss.Invoke();
       }

    
   }
   
   
#if UNITY_EDITOR
    [ContextMenu("间隔检查")]
    private void CheckInterval()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("间隔检查要在运行情况下进行");
            return;
            
        }
        
      
        for (int j = 1; j < cymbalAction.Count; j++)
        {
            if (cymbalAction[j] - cymbalAction[j - 1] <= 8)
            {
                Debug.LogError($"存在过短:{YamlReadWrite.Read<YamlReadWrite.CymbalAction>(YamlReadWrite.FileName.Cymbal).time[j]}与{YamlReadWrite.Read<YamlReadWrite.CymbalAction>(YamlReadWrite.FileName.Cymbal).time[j - 1]}");
            }
        }
    } 
#endif

   
}
