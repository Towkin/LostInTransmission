using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSmooth : MonoBehaviour {

    private Animator m_Anim;

    void Start()
    {
        m_Anim = GetComponent<Animator>();

        //m_Anim.SetBool("EnableSmooth", true);
    }


    //public void OnPointerEnter(PointerEventData eventData)
    //{


    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    m_Anim.SetBool("EnableSmooth", false);
    //}
}
