using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageOptionDropdown : MonoBehaviour {

    GameObject blocker = null;
    
    const string m_SelectAudioEvent = "event:/Effects/UI/Menu_Dropdown";

    public void Activate()
    {
        gameObject.SetActive(true);

        if(blocker == null)
        {
            blocker = new GameObject("Blocker - " + gameObject.name);
            blocker.transform.SetParent(transform.parent);
            var rect = blocker.AddComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;

            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            rect.localScale = Vector3.one * 1000.0f;
            
            var blockButton = blocker.AddComponent<UnityEngine.UI.Button>();
            blockButton.onClick.AddListener(Deactivate);

            var image = blocker.AddComponent<UnityEngine.UI.Image>();
            blockButton.targetGraphic = image;
            image.color = new Color(0, 0, 0, 0.0f);

            FMODUnity.RuntimeManager.PlayOneShot(m_SelectAudioEvent);
        }

        blocker.transform.SetSiblingIndex(1000);
        blocker.SetActive(true);

        // Put ourselves in front of the blocker.
        transform.SetSiblingIndex(1000);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
        Destroy(blocker);
        blocker = null;
    }

    private void OnDestroy()
    {
        Deactivate();
    }
}
