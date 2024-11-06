using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        Init();
    }

    void Init()
    {
        button.onClick.AddListener(() => SceneManagerExtend.Instance.LoadScene(Define.Scene.Battle));
    }
}
