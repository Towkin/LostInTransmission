using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageOptionDropdown : MonoBehaviour
{
    public int m_SelectOptionIndex;
    public int m_OptionSetIndex;
    public RouterController m_Router;

    public void SelectOption()
    {
        m_Router.SetOptions(m_OptionSetIndex, m_SelectOptionIndex);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
