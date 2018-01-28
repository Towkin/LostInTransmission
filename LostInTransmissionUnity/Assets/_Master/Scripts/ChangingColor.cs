using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangingColor : MonoBehaviour {

    float timeLeft;
    Color targetColor;

    Text m_TitleText;

    private void Start()
    {
        m_TitleText = GetComponent<Text>();
    }

    void Update()
    {
        if (timeLeft <= Time.deltaTime)
        {
            m_TitleText.color = targetColor;
            targetColor = new Color(Random.value, Random.value, Random.value);
            timeLeft = 1.0f;
        }
        else
        {
            m_TitleText.color = Color.Lerp(m_TitleText.color, targetColor, Time.deltaTime / timeLeft);
            timeLeft -= Time.deltaTime;
        }
    }
}
