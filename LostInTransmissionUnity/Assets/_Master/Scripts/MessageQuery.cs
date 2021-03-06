﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MessageQuery {

    private TranslateMessageData m_Data;
    public TranslateMessageData Data {
        get { return m_Data; }
    }

    public GameObject Sender { get; set; }
    public GameObject Reciever { get; set; }

    public void SetData(TranslateMessageData newData)
    {
        m_Data = newData;

        var strings = Data.Text.Split(new char[] { '[', ']' });
        m_MessageParts = new MessagePart[strings.Count()];
        for (int i = 0; i < strings.Count(); i++)
            m_MessageParts[i] = new MessagePart(strings[i]);
        
        for (int i = 0; i < m_MessageParts.Count(); i++)
        {
            for(int j = 0; j < Data.OptionSet.Count(); j++)
            {
                if(m_MessageParts[i].Text == Data.OptionSet[j].Name)
                {
                    if (Data.OptionSet[j].Options.Count() < 2)
                        break;

                    m_MessageParts[i] = new MessagePart(Data.OptionSet[j].Name, j);
                    break;
                }
            }
        }
    }
    
    public class MessagePart
    {
        public MessagePart(string text)
        {
            m_Text = text;
        }

        public MessagePart(string defaultText, int optionSet)
        {
            m_Text = defaultText;
            m_OptionSetIndex = optionSet;
            m_CurrentOption = -1;
            m_Dirty = true;
        }

        public void Update(ref TranslateMessageData.TranslateOptionSet[] optionSet)
        {
            if (m_Dirty && m_CurrentOption >= 0)
                m_Text = optionSet[m_OptionSetIndex].Options[CurrentOption].Text;
        }

        string m_Text;
        int m_OptionSetIndex = -1;
        int m_CurrentOption = -1;
        bool m_Dirty = false;

        public string Text
        {
            get { return m_Text; }
        }
        public int OptionsSetIndex { get { return m_OptionSetIndex; } }
        public int CurrentOption
        {
            get { return Mathf.Max(m_CurrentOption, 0); } 
            set
            {
                if (m_OptionSetIndex == -1)
                    return;

                m_Dirty = true;
                m_CurrentOption = value;
            }
        }

        public bool HasBeenSet
        {
            get { return m_CurrentOption >= 0; }
        }

        
        public bool IsOption { get { return m_OptionSetIndex >= 0; } }
    }
    
    MessagePart[] m_MessageParts;

    public TranslateMessageData.TranslateOptionSet GetOptionSet(MessagePart part)
    {
        return Data.OptionSet[part.OptionsSetIndex];
    }

    public MessagePart[] MessageText
    {
        get
        {
            foreach(var part in m_MessageParts)
                part.Update(ref m_Data.OptionSet);

            return m_MessageParts;
        }
    }
    public MessagePart[] MessageOptions
    {
        get
        {
            List<MessagePart> options = new List<MessagePart>();
            foreach (var part in m_MessageParts)
                if (part.IsOption)
                    options.Add(part);

            return options.ToArray();
        }
    }
}
