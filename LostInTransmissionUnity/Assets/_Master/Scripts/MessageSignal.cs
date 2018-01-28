using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSignal : MonoBehaviour {

    public MessageQuery m_Query;
    public GameObject m_Goal;

    [SerializeField]
    float m_Speed = 10.0f;
	
    void Start()
    {
    }
        
    // Update is called once per frame
	void Update () {
        transform.position += (m_Goal.transform.position - transform.position).normalized *
            Mathf.Min(m_Speed * Time.deltaTime, (m_Goal.transform.position - transform.position).magnitude);

        if((transform.position - m_Goal.transform.position).magnitude < 0.1f)
        {
            if(m_Query.Reciever == m_Goal)
            {
                m_Goal.GetComponent<Faction>().RecieveMessage(m_Query);
            }
            else
            {
                m_Goal.GetComponent<RouterController>().PushMessage(m_Query);
            }
            Destroy(gameObject);
        }
	}
}
