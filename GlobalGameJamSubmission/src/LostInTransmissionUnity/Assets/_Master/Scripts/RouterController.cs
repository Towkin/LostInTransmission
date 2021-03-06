﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;



public class RouterController : MonoBehaviour {
    [SerializeField]
    UnityEngine.UI.Text m_MessageText;
    [SerializeField]
    UnityEngine.UI.Image m_MessageImage;

    [SerializeField]
    UnityEngine.UI.Button m_SendButton;

    [SerializeField]
    UnityEngine.UI.Text m_SenderText;

    [SerializeField]
    UnityEngine.UI.Text m_RecieverText;

    [SerializeField]
    Sprite m_DefaultImage;

    [SerializeField]
    GameObject m_OptionDropdownTemplate;

    [SerializeField]
    GameObject m_SignalTemplate;

    [SerializeField]
    Color m_OptionTextColor = Color.grey;
    [SerializeField]
    float m_OptionTextPadding = 8;

    [SerializeField]
    UnityEngine.UI.Text m_BacklogText;

    List<MessageQuery> m_History = new List<MessageQuery>();
    List<MessageQuery> m_BackLog = new List<MessageQuery>();

    List<GameObject> m_Buttons = new List<GameObject>();
    List<GameObject> m_DropdownOptions = new List<GameObject>();

    public void Cleanup()
    {
        foreach (var item in m_DropdownOptions)
            Destroy(item);
        m_DropdownOptions.Clear();

        foreach (var button in m_Buttons)
            Destroy(button);
        m_Buttons.Clear();

        m_MessageText.text = "";
        m_MessageImage.sprite = m_DefaultImage;
        m_SenderText.text = "";
        m_RecieverText.text = "";
        m_SendButton.gameObject.SetActive(false);
        m_BacklogText.text = "";
    }

    public void SendMessage()
    {
        if (m_BackLog.Count == 0)
            return;

        MessageQuery sendQuery = m_BackLog[0];
        
        m_BackLog.RemoveAt(0);
        m_History.Add(sendQuery);
              

        Cleanup();

        var signalObject = Instantiate(m_SignalTemplate);
        signalObject.transform.position = gameObject.transform.position;

        var signal = signalObject.GetComponent<MessageSignal>();
        signal.m_Goal = sendQuery.Reciever;
        signal.m_Query = sendQuery;

        if (m_BackLog.Count > 0)
        {
            BuildMessage(m_BackLog[0]);

            FMODUnity.RuntimeManager.PlayOneShot(m_BackLog[0].Sender.GetComponent<Faction>().FactionAudioEvent);
        }
    }

    public void PushMessage(MessageQuery query)
    {
        m_BackLog.Add(query);

        if (m_BackLog.Count == 1)
        {
            BuildMessage(m_BackLog[0]);

            FMODUnity.RuntimeManager.PlayOneShot(m_BackLog[0].Sender.GetComponent<Faction>().FactionAudioEvent);
        }
            
    }

    public void SetOptions(int optionSet, int optionIndex)
    {
        if (m_BackLog.Count == 0)
            return;

        m_BackLog[0].MessageOptions[optionSet].CurrentOption = optionIndex;
        Cleanup();
        BuildMessage(m_BackLog[0]);
    }

    // Use this for initialization
    void Start () {
        Cleanup();
    }

    // Update is called once per frame
    void Update () {

    }

    class OptionButtonBuilder
    {
        public Vector2Int m_CharIndices;
        public int m_OptionSetIndex;
    }

    class UnderTextButton
    {
        public Vector2 position;
        public float length;
    }

    void CreateOptionsDropdown(RectTransform parentRect, TranslateMessageData.TranslateOptionSet optionSet, int optionSetIndex)
    {
        var optionSetList = new GameObject(optionSet.Name + " options");
        optionSetList.transform.SetParent(parentRect.parent);
        var optionSetTransform = optionSetList.AddComponent<RectTransform>();
        optionSetTransform.position = parentRect.position;
        parentRect.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(optionSetList.AddComponent<MessageOptionDropdown>().Activate);
        m_DropdownOptions.Add(optionSetList);

        for (int i = 0; i < optionSet.Options.Length; i++)
        {
            var dropdownItem = Instantiate(m_OptionDropdownTemplate);
            dropdownItem.GetComponentInChildren<UnityEngine.UI.Text>().text = optionSet.Options[i].Text;
            dropdownItem.GetComponent<MessageOptionDropdownItem>().m_OptionSetIndex = optionSetIndex;
            dropdownItem.GetComponent<MessageOptionDropdownItem>().m_SelectOptionIndex = i;
            dropdownItem.GetComponent<MessageOptionDropdownItem>().m_Router = this;
            dropdownItem.transform.position = parentRect.position + new Vector3(0, -dropdownItem.GetComponent<RectTransform>().rect.height * (i + 1));

            dropdownItem.transform.SetParent(optionSetList.transform);
        }
        optionSetList.SetActive(false);
    }

    IEnumerator BuildButtons(List<OptionButtonBuilder> optionsBuilderList, TranslateMessageData.TranslateOptionSet[] optionSets)
    {
        yield return new WaitForEndOfFrame();

        var textGen = m_MessageText.cachedTextGenerator;
        foreach (var optionsBuilder in optionsBuilderList)
        {
            int firstIndex = optionsBuilder.m_CharIndices.x, lastIndex = optionsBuilder.m_CharIndices.y;
            List<UnderTextButton> buttonSizes = new List<UnderTextButton>();
            buttonSizes.Add(new UnderTextButton()
            {
                position = textGen.characters[firstIndex].cursorPos,
                length = 0.0f
            });

            float lastY = textGen.characters[firstIndex].cursorPos.y;
            for (int i = firstIndex; i < lastIndex; i++)
            {
                if (Mathf.Abs(textGen.characters[i].cursorPos.y - lastY) > 1.0f)
                {
                    buttonSizes.Add(new UnderTextButton() {
                        position = textGen.characters[i].cursorPos
                    });
                    lastY = textGen.characters[i].cursorPos.y;
                }

                buttonSizes[buttonSizes.Count - 1].length += textGen.characters[i].charWidth;
            }

            foreach (var buttonSize in buttonSizes)
            {
                var buttonObject = new GameObject();
                buttonObject.transform.SetParent(m_MessageText.transform.parent);

                var rectTransform = buttonObject.AddComponent<RectTransform>();
                rectTransform.transform.position = m_MessageText.transform.position + new Vector3(
                    buttonSize.position.x + buttonSize.length / 2,
                    buttonSize.position.y - m_MessageText.fontSize / 2 - m_OptionTextPadding / 4);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize.length + m_OptionTextPadding);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_MessageText.fontSize + m_OptionTextPadding);

                var button = buttonObject.AddComponent<UnityEngine.UI.Button>();
                var image = buttonObject.AddComponent<UnityEngine.UI.Image>();
                image.color = m_OptionTextColor;
                button.targetGraphic = image;
                
                m_Buttons.Add(buttonObject);

                CreateOptionsDropdown(rectTransform, optionSets[optionsBuilder.m_OptionSetIndex], optionsBuilder.m_OptionSetIndex);
            }
        }
        m_MessageText.transform.SetSiblingIndex(1000);
    }

    void BuildMessage(MessageQuery query)
    {
        
        m_SenderText.text = query.Sender.GetComponent<Faction>().FactionName;
        m_RecieverText.text = query.Reciever.GetComponent<Faction>().FactionName;
        m_MessageImage.sprite = query.Sender.GetComponent<Faction>().FactionImage;

        var builder = new StringBuilder();
        var messages = query.MessageText;

        foreach (var msg in m_History)
        {
            m_BacklogText.text += "\n";
            m_BacklogText.text += msg.Sender.GetComponent<Faction>().FactionName + " to " + msg.Reciever.GetComponent<Faction>().FactionName + ": ";

            foreach (var part in msg.MessageText)
            {
                m_BacklogText.text += part.Text;
            }
        }

        var optionsBuilderList = new List<OptionButtonBuilder>();
        bool isDone = true;        
        foreach (var part in messages)
        {
            int partStart = builder.Length;
            
            builder.Append(part.Text);
            
            if (part.IsOption)
            {
                optionsBuilderList.Add(new OptionButtonBuilder()
                {
                    m_CharIndices = new Vector2Int(partStart, builder.Length),
                    m_OptionSetIndex = part.OptionsSetIndex
                });
                if (!part.HasBeenSet)
                    isDone = false;
            }            
        }
        if (isDone)
            m_SendButton.gameObject.SetActive(true);
        
        m_MessageText.text = builder.ToString();

        StartCoroutine(BuildButtons(optionsBuilderList, query.Data.OptionSet));
    }
}
