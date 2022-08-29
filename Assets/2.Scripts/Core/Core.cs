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
        
        
    }


   private void Initialization()
   {
       //读取大镲文件
       ReadYaml();
       //clef消失（不填充）
       clef.fillAmount = 0f;
   }


   private void ReadYaml()
   {
       var yaml = YamlReadWrite.Read<YamlReadWrite.CymbalAction>(YamlReadWrite.FileName.Cymbal);

       cymbalAction = new List<int>();
      
       for (int i = 0; i < yaml.time.Length; i++)
       {
           //按照冒号分开。长度为4. 0 =1舍弃 =2有改动 .1是分钟（1min=60s=1800)  2是秒（1s=30） 3则可以视为帧数
           string[] fix = yaml.time[i].Split(':');
          
           //lag对于整体的滞后性进行修复
           cymbalAction.Add(int.Parse(fix[3]) + int.Parse(fix[2]) * 30 + int.Parse(fix[1]) * 1800 +  Settings.SettingsContent.lag);
           
           
       }
       
       Debug.Log(cymbalAction[0]);
   }

   /// <summary>
   /// 音符（打击乐符号）淡入
   /// </summary>
   private void ClefFadeIn()
   {
       //按照视频进度，对打击乐符号进行填充
       clef.fillAmount = (float)StaticVideoPlayer.videoPlayer.frame / cymbalAction[index];
     
       if (StaticVideoPlayer.videoPlayer.frame >= cymbalAction[index])
       {
           index++;
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
            if (cymbalAction[j] - cymbalAction[j - 1] <= 10)
            {
                Debug.LogError($"存在过短:{YamlReadWrite.Read<YamlReadWrite.CymbalAction>(YamlReadWrite.FileName.Cymbal).time[j]}与{YamlReadWrite.Read<YamlReadWrite.CymbalAction>(YamlReadWrite.FileName.Cymbal).time[j - 1]}");
            }
        }
    }
#endif

   
}
