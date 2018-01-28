using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    [SerializeField]
    [FMODUnity.EventRef]
    string m_MusicPath;

    static MusicManager Instance = null;

    FMOD.Studio.EventInstance m_MusicInstance;

    // Use this for initialization
    void Start() {
        if (Instance != null)
        { 
            Destroy(gameObject);
            return;
        }
        Instance = this;

        m_MusicInstance = FMODUnity.RuntimeManager.CreateInstance(m_MusicPath);
        m_MusicInstance.start();

        for (int i = 0; i < 10; i++)
            StartCoroutine(MusicParameter());
    }

    float m_WaitSeconds = 20;
    IEnumerator MusicParameter()
    {
        yield return new WaitForSeconds(m_WaitSeconds);
        FMOD.Studio.ParameterInstance instance;
        m_MusicInstance.getParameter("MusicOverTime", out instance);
        float val;
        instance.getValue(out val);
        m_MusicInstance.setParameterValue("MusicOverTime", val + 1.0f);
    }
    
    // Update is called once per frame
    void Update () {

        

        
    }
}
