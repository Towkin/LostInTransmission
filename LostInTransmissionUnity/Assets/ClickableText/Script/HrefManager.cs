/// Credit playemgames 
/// Sourced from - http://forum.unity3d.com/threads/sprite-icons-with-text-e-g-emoticons.265927/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;

public class HrefManager : MonoBehaviour
{
    
	public List<HyperLinkDetails> hyperLinkDetails = new List<HyperLinkDetails>();

	void Start() {
        GetComponent<TextPic>().onHrefClick.AddListener(OnHrefClick);
	}

    void OnDisable()
    {
		GetComponent<TextPic>().onHrefClick.RemoveListener(OnHrefClick);
    }

    private void OnHrefClick(string hrefName)
    {
		foreach (var link in hyperLinkDetails)
        {
			if (link.hyperlinkName == hrefName)
            {
				

				break;
			}
		}
    }

    private void OnHredEnter(string hrefName)
    {

    }

    private void OnHredExit(string hrefName)
    {

    }
}

[System.Serializable]
public class HyperLinkDetails {
	public string hyperlinkName;
}
