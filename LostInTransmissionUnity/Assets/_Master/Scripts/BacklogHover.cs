using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BacklogHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Animator m_Anim;

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
}
