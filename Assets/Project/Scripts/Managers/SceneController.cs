using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    Lobby,
    Battle,
}

public class SceneController : Singleton<SceneController>
{
    public void LoadBattleScene() => SceneManager.LoadScene("Battle");// ������â ������

    public void GameExit() => Application.Quit(); 
}
