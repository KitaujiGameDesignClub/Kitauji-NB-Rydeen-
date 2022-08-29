using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Core : MonoBehaviour,IUpdate
{
    public  UnityEvent onGetButtonLeftCtrl = new UnityEvent();
    public UnityEvent onGetButtonRightCtrl = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        UpdateManager.RegisterUpdate(this);
    }

    // Update is called once per frame
   public void FastUpdate()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl)) onGetButtonLeftCtrl.Invoke();
        if(Input.GetKeyDown(KeyCode.RightControl)) onGetButtonRightCtrl.Invoke();
    }


   /// <summary>
   /// 保存
   /// </summary>
   [ContextMenu("保存")]
   public void Save()
   {
       var s = new YamlReadWrite.CymbalAction();
       s.time = new string[1];
       s.time[0] = "00:01:02:13";
       YamlReadWrite.Write(s,YamlReadWrite.FileName.Cymbal,"# 大镲时间设定");
   }
}
