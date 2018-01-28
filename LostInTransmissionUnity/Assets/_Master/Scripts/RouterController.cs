using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class RouterController : MonoBehaviour {
    [SerializeField]
    UnityEngine.UI.Text m_Text;

    [SerializeField]
    GameObject m_OptionDropdownTemplate;
    

    List<string> m_History;
    List<MessageQuery> m_BackLog = new List<MessageQuery>();

    List<GameObject> m_Buttons = new List<GameObject>();

    public void SendMessage(TranslateMessageData messageData)
    {
        var query = new MessageQuery();
        query.SetData(messageData);
        m_BackLog.Add(query);
    }

    public void SetOptions(int optionSet, int optionIndex)
    {
        if (m_BackLog.Count == 0)
            return;

        m_BackLog[0].MessageOptions[optionSet].CurrentOption = optionIndex;
        BuildText(m_BackLog[0]);
    }

    // Use this for initialization
    void Start () {
        SendMessage(GetComponent<MessageDatabase>().Messages[5]);
        
        BuildText(m_BackLog[0]);
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
        for(int i = 0; i < optionSet.Options.Length; i++)
        {
            var dropdownItem = Instantiate(m_OptionDropdownTemplate);
            dropdownItem.GetComponentInChildren<UnityEngine.UI.Text>().text = optionSet.Options[i].Text;
            dropdownItem.GetComponent<MessageOptionDropdown>().m_OptionSetIndex = optionSetIndex;
            dropdownItem.GetComponent<MessageOptionDropdown>().m_SelectOptionIndex = i;
            dropdownItem.GetComponent<MessageOptionDropdown>().m_Router = this;
            dropdownItem.transform.position = parentRect.position + new Vector3(0, -dropdownItem.GetComponent<RectTransform>().rect.height * (i + 1));

            dropdownItem.transform.SetParent(parentRect);
            dropdownItem.SetActive(false);

            parentRect.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(dropdownItem.GetComponent<MessageOptionDropdown>().Activate);
        }
    }

    IEnumerator BuildButtons(List<OptionButtonBuilder> optionsBuilderList, TranslateMessageData.TranslateOptionSet[] optionSets)
    {
        yield return new WaitForEndOfFrame();

        var textGen = m_Text.cachedTextGenerator;
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
                buttonObject.transform.SetParent(m_Text.transform.parent);

                var rectTransform = buttonObject.AddComponent<RectTransform>();
                rectTransform.transform.position = m_Text.transform.position + new Vector3(
                    buttonSize.position.x + buttonSize.length / 2,
                    buttonSize.position.y - m_Text.fontSize / 2);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize.length);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Text.fontSize);

                var button = buttonObject.AddComponent<UnityEngine.UI.Button>();
                var image = buttonObject.AddComponent<UnityEngine.UI.Image>();
                image.color = Color.cyan;
                button.targetGraphic = image;
                
                m_Buttons.Add(buttonObject);

                CreateOptionsDropdown(rectTransform, optionSets[optionsBuilder.m_OptionSetIndex], optionsBuilder.m_OptionSetIndex);
            }
        }
        m_Text.transform.SetSiblingIndex(100);
    }

    void BuildText(MessageQuery query)
    {
        foreach (var button in m_Buttons)
            Destroy(button);
        m_Buttons.Clear();
        
        var builder = new StringBuilder();
        var messages = query.MessageText;

        var optionsBuilderList = new List<OptionButtonBuilder>();

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
            }
        }
        m_Text.text = builder.ToString();

        StartCoroutine(BuildButtons(optionsBuilderList, query.Data.OptionSet));
    }
}
