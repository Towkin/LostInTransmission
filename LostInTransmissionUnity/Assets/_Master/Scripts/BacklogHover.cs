using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BacklogHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Animator m_Anim;
    List<GameObject> m_LogMessages = new List<GameObject>();
    [SerializeField]
    GameObject m_ObjectToCopy;

    void Start()
    {
        m_Anim = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Anim.SetBool("EnableHover", true);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_Anim.SetBool("EnableHover", false);
    }

    public void UpdateMessageLog(List<MessageQuery> history)
    {
       //foreach (GameObject obj in m_LogMessages)
       //    Destroy(obj);
       //m_LogMessages.Clear();
       //m_ObjectToCopy.SetActive(true);
       //
       //if(history.Count > 0)
       //{
       //    MessageQuery latest = history[history.Count - 1];
       //    foreach (var msg in history)
       //    {
       //        if ((msg.Sender == latest.Sender && msg.Reciever == latest.Reciever) || (msg.Sender == latest.Reciever && msg.Reciever == latest.Sender))
       //        {
       //            GameObject msgObj = GameObject.Instantiate(m_ObjectToCopy);
       //
       //            UnityEngine.UI.Text msgText = msgObj.GetComponentInChildren<UnityEngine.UI.Text>();
       //            foreach (var part in msg.MessageText)
       //            {
       //                msgText.text += part.Text;
       //            }
       //
       //            UnityEngine.UI.Image msgImage = msgObj.GetComponentInChildren<UnityEngine.UI.Image>();
       //            msgImage.sprite = msg.Sender.GetComponent<Faction>().FactionImage;
       //
       //            if (m_LogMessages.Count > 0)
       //                msgObj.transform.SetParent(m_LogMessages[m_LogMessages.Count - 1].transform);
       //            else
       //                msgObj.transform.SetParent(m_ObjectToCopy.transform);
       //            msgObj.transform.localPosition = new Vector3(0, -100, 0);
       //            m_LogMessages.Add(msgObj);
       //        }
       //    }
       //}        
    }
}
