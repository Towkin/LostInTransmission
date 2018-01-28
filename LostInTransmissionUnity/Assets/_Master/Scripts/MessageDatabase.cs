using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;


[Serializable]
public class MoodModifier
{
    public string Name;
    public float Modification;
}

[Serializable]
public class TranslateMessageData
{
    [Serializable]
    public class TranslateOption
    {
        public string Text;
        public MoodModifier[] MoodSet;
    }

    [Serializable]
    public class TranslateOptionSet
    {
        public string Name;
        public TranslateOption[] Options;
    }
    
    public string Text;
    public MoodModifier[] MoodSet;
    public TranslateOptionSet[] OptionSet;
}

public class TranslateMessageDataLister
{
    [Serializable]
    public class TranslateOption
    {
        public TranslateOption()
        {
            MoodSet = new List<MoodModifier>();
        }

        public string Text;
        public List<MoodModifier> MoodSet;

        public static implicit operator TranslateMessageData.TranslateOption(TranslateOption fromOption)
        {
            return new TranslateMessageData.TranslateOption()
            {
                Text = fromOption.Text,
                MoodSet = fromOption.MoodSet.ToArray()
            };
        }
    }

    [Serializable]
    public class TranslateOptionSet
    {
        public TranslateOptionSet()
        {
            Options = new List<TranslateOption>();
        }

        public string Name;
        public List<TranslateOption> Options;
        public static implicit operator TranslateMessageData.TranslateOptionSet(TranslateOptionSet fromSet)
        {
            var set = new TranslateMessageData.TranslateOptionSet()
            {
                Name = fromSet.Name
            };
            set.Options = new TranslateMessageData.TranslateOption[fromSet.Options.Count];
            for(int i = 0; i < fromSet.Options.Count; i++)
            {
                // Note, implicit conversion:
                set.Options[i] = fromSet.Options[i];
            }
            return set;
        }
    }

    public TranslateMessageDataLister()
    {
        MoodSet = new List<MoodModifier>();
        OptionSet = new List<TranslateOptionSet>();
    }

    public string Text;
    public List<MoodModifier> MoodSet;
    public List<TranslateOptionSet> OptionSet;
}

public class MessageDatabase : MonoBehaviour
{
    [ContextMenuItem("Load Database", "LoadDb")]
    public UnityEngine.Object Database;
    [SerializeField]
    private TranslateMessageData[] m_Messages;
    public TranslateMessageData[] Messages { get { return m_Messages; } }

    private void Start()
    {
    }
    
    enum DatabaseColumn : int {
        MessageText = 0,
        OptionKey = 1,
        OptionText = 2,
        OptionMoodKey = 3,
        OptionMoodValue = 4
    }

    public void LoadDb()
    {
        List<TranslateMessageDataLister> messages = new List<TranslateMessageDataLister>();
        using (var reader = new StreamReader(AssetDatabase.GetAssetPath(Database)))
        {
            while (!reader.EndOfStream)
            {
                string[] columns = reader.ReadLine().Split('\t');
                if (columns.Length != 5)
                {
                    Debug.LogWarning("A line in the file '" + Database + "' didn't contain 5 columns!");
                    continue;
                }

                if (!String.IsNullOrEmpty(columns[(int)DatabaseColumn.MessageText]))
                {
                    // Skip the entire line if it has the column name.
                    if (columns[(int)DatabaseColumn.MessageText] == "MessageText")
                        continue;
                    
                    messages.Add(new TranslateMessageDataLister
                    {
                        Text = columns[(int)DatabaseColumn.MessageText]
                    });
                }

                var currentMessage = messages.LastOrDefault();
                if (currentMessage == null)
                    continue;

                if (!String.IsNullOrEmpty(columns[(int)DatabaseColumn.OptionKey]))
                {
                    currentMessage.OptionSet.Add(new TranslateMessageDataLister.TranslateOptionSet()
                    {
                        Name = columns[(int)DatabaseColumn.OptionKey]
                    });
                }

                var currentOptionSet = currentMessage.OptionSet.LastOrDefault();

                if (currentOptionSet != null && !String.IsNullOrEmpty(columns[(int)DatabaseColumn.OptionText]))
                {
                    currentOptionSet.Options.Add(new TranslateMessageDataLister.TranslateOption()
                    {
                        Text = columns[(int)DatabaseColumn.OptionText]
                    });
                }

                var currentMoodModifiers = currentOptionSet != null && currentOptionSet.Options.Count > 0 ?
                    currentOptionSet.Options[currentOptionSet.Options.Count - 1].MoodSet :
                    currentMessage.MoodSet;

                if (!String.IsNullOrEmpty(columns[(int)DatabaseColumn.OptionMoodKey]) &&
                    !String.IsNullOrEmpty(columns[(int)DatabaseColumn.OptionMoodValue]))
                {
                    currentMoodModifiers.Add(new MoodModifier()
                    {
                        Name = columns[(int)DatabaseColumn.OptionMoodKey],
                        Modification = float.Parse(columns[(int)DatabaseColumn.OptionMoodValue])
                    });
                }
            }
        }
        m_Messages = new TranslateMessageData[messages.Count];
        for (int i = 0; i < messages.Count; i++)
        {
            TranslateMessageDataLister output = messages[i];
            m_Messages[i] = new TranslateMessageData()
            {
                Text = output.Text,
                MoodSet = output.MoodSet.ToArray(),
                OptionSet = new TranslateMessageData.TranslateOptionSet[output.OptionSet.Count]
            };
            
            for (int j = 0; j < output.OptionSet.Count; j++)
            {
                // Note: Implicit conversion.
                m_Messages[i].OptionSet[j] = output.OptionSet[j];
            }
        }
    }
}
