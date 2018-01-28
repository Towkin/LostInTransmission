using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    [SerializeField]
    [FMODUnity.EventRef]
    string m_MusicPath;

    FMOD.Studio.EventInstance m_MusicInstance;

    // Use this for initialization
    void Start () {
        m_MusicInstance = FMODUnity.RuntimeManager.CreateInstance(m_MusicPath);
        m_MusicInstance.start();
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
