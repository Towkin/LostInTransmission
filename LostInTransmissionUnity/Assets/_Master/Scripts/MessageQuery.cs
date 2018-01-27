using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MessageQuery : MonoBehaviour {

    private TranslateMessageData m_Data;
    public TranslateMessageData Data { get { return m_Data; } }

    public void SetData(TranslateMessageData newData)
    {
        m_StringParts = newData.Text.Split(new char[] { '[', ']' });
        List<int> optionIndices = new List<int>();

        for (int i = 0; i < m_StringParts.Count(); i++)
        {
            for(int j = 0; j < newData.OptionSet.Count(); j++)
            {
                if(m_StringParts[i] == newData.OptionSet[j].Name)
                {
                    m_StringParts[i] = null;
                    optionIndices.Add(j);
                    break;
                }
            }
        }

        m_OptionSetIndices = optionIndices.ToArray();
        m_CurrentOptionIndices = new int[m_OptionSetIndices.Count()];
    }

    string[] m_StringParts;
    int[] m_OptionSetIndices;
    int[] m_CurrentOptionIndices;
    
    public string Text
    {
        get
        {
            StringBuilder builder = new StringBuilder();
            int partIndex = 0;
            foreach (var part in m_StringParts)
            {
                if (part == null)
                {
                    builder.Append(Data.OptionSet[m_OptionSetIndices[partIndex]].Options[m_CurrentOptionIndices[partIndex]].Text);
                    partIndex++;
                }
                else
                {
                    builder.Append(part);
                }
            }
            return builder.ToString();
        }
    }
}
