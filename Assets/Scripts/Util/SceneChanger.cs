using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : SingletonMonoBehaviour<SceneChanger>{

    public GameObject gameOverObj;

    protected override void Init()
    {
        base.Init();
        Debug.Log("シーンチェンジャー");
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ChangeScene(string sceneName,float fadeTime)
    {
        SceneNavigator.Instance.Change(sceneName, fadeTime);
    }
    public void AddScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
    
    public void ReStart()
    {
        Destroy(Instantiate(gameOverObj,GameObject.FindWithTag("Player").transform), 3.0f);

        ChangeScene(SceneManager.GetActiveScene().name,1.5f);
    }
}
