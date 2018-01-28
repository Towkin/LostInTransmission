using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageOptionDropdownItem : MonoBehaviour
{
    public int m_SelectOptionIndex;
    public int m_OptionSetIndex;
    public RouterController m_Router;

    static string m_AudioSelectOptionEvent = "event:/Effects/UI/Button_Select";

    public void SelectOption()
    {
        m_Router.SetOptions(m_OptionSetIndex, m_SelectOptionIndex);
        FMODUnity.RuntimeManager.PlayOneShot(m_AudioSelectOptionEvent);
    }
}
