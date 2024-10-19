using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerExtend : Singleton<SceneManagerExtend>
{
    protected override void Init()
    {
        base.Init(); 
    }

    public void LoadScene(Define.Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
