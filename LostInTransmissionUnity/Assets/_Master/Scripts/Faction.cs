using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;






[RequireComponent(typeof(MessageDatabase))]
public class Faction : MonoBehaviour {

    [SerializeField]
    GameObject m_Router;

    

    [SerializeField]
    GameObject m_FirstContact;
    [SerializeField]
    string m_FirstContactMoodName;
    [SerializeField]
    float m_FirstContactMoodValue;

    [SerializeField]
    string m_FactionName;
    [SerializeField]
    Sprite m_FactionImage;

    public string FactionName { get { return m_FactionName; } }
    public Sprite FactionImage { get { return m_FactionImage; } }


    const float m_MoodRate = 0.5f;
    const float m_MoodScale = 1.5f;

    Dictionary<GameObject, Dictionary<string, float>> m_Moods = new Dictionary<GameObject, Dictionary<string, float>>();
    
    TranslateMessageData[] Messages { get { return GetComponent<MessageDatabase>().Messages; } }
    
    public void RecieveMessage(MessageQuery messageData)
    {
        if (!m_Moods.ContainsKey(messageData.Sender))
            m_Moods.Add(messageData.Sender, new Dictionary<string, float>());

        var senderMoods = m_Moods[messageData.Sender];

        // Standard modification from the message.
        foreach (var mood in messageData.Data.MoodSet)
        {
            if (!senderMoods.ContainsKey(mood.Name))
                senderMoods.Add(mood.Name, 0.0f);

            senderMoods[mood.Name] += mood.Modification;
        }
        foreach(var option in messageData.MessageOptions)
        {
            var optionMoodSet = messageData.Data.OptionSet[option.OptionsSetIndex].Options[option.CurrentOption].MoodSet;
            foreach (var mood in optionMoodSet)
            {
                if (!senderMoods.ContainsKey(mood.Name))
                    senderMoods.Add(mood.Name, 0.0f);

                senderMoods[mood.Name] += mood.Modification;
            }
        }

        EventUpdate();
    }

    void EventUpdate()
    {
        GameObject mostMoodyFaction = null;
        float mostMoodyValue = 0.0f;
        foreach(var factionMoods in m_Moods)
        {
            if (factionMoods.Key == gameObject)
                continue;

            float factionMoodValue = 0.0f;
            foreach (var mood in factionMoods.Value)
                factionMoodValue += Mathf.Abs(mood.Value);

            if (factionMoodValue > mostMoodyValue)
                mostMoodyFaction = factionMoods.Key;
        }

        if (mostMoodyFaction == null)
            return;

        var factionMood = GetTotalMood(mostMoodyFaction);
        
        TranslateMessageData bestMessage = null;
        float bestMessageValue = 0.0f;
        foreach (var message in Messages)
        {
            var postMessageMood = factionMood.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var mood in message.MoodSet)
            {
                if (!postMessageMood.ContainsKey(mood.Name))
                    postMessageMood.Add(mood.Name, 0.0f);

                postMessageMood[mood.Name] -= mood.Modification;
            }

            foreach (var option in message.OptionSet)
            {
                foreach (var mood in option.Options[0].MoodSet)
                {
                    if (!postMessageMood.ContainsKey(mood.Name))
                        postMessageMood.Add(mood.Name, 0.0f);

                    postMessageMood[mood.Name] -= mood.Modification;
                }
            }

            float messageValue = 0.0f;

            foreach (var mood in postMessageMood)
                messageValue += Mathf.Pow(m_MoodRate, Mathf.Abs(mood.Value)) * m_MoodScale;

            if(messageValue > bestMessageValue)
            {
                bestMessageValue = messageValue;
                bestMessage = message;
            }
        }

        if (bestMessage == null)
            return;

        m_Router.GetComponent<RouterController>().PushMessage(bestMessage, gameObject, mostMoodyFaction);

        // TODO: Decrease values here.
    }

    Dictionary<string, float> GetTotalMood(GameObject faction)
    {
        Dictionary<string, float> totalMoods = new Dictionary<string, float>();

        foreach(var f in new []{ gameObject, faction })
        {
            if (!m_Moods.ContainsKey(f))
                m_Moods.Add(f, new Dictionary<string, float>());

            foreach (var mood in m_Moods[f])
            {
                if (!totalMoods.ContainsKey(mood.Key))
                    totalMoods.Add(mood.Key, 0.0f);

                totalMoods[mood.Key] += mood.Value;
            }
        }

        return totalMoods;
    }



    // Use this for initialization
    void Start () {


        if (m_FirstContact == null || String.IsNullOrEmpty(m_FirstContactMoodName))
            return;

        if (!m_Moods.ContainsKey(m_FirstContact))
            m_Moods.Add(m_FirstContact, new Dictionary<string, float>());

        if (!m_Moods[m_FirstContact].ContainsKey(m_FirstContactMoodName))
            m_Moods[m_FirstContact].Add(m_FirstContactMoodName, 0.0f);

        m_Moods[m_FirstContact][m_FirstContactMoodName] += m_FirstContactMoodValue;
        EventUpdate();
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
