using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public interface SceneChangeTarget : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void ChangeScene(string s);
}

public class SceneChange : MonoBehaviour, SceneChangeTarget
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
