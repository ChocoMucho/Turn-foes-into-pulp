using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerExtend : Singleton<SceneManagerExtend>
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }
    public void LoadScene(Define.Scene scene)
    {
        CurrentScene.Clear();
        SceneManager.LoadScene(scene.ToString());
    }
}
