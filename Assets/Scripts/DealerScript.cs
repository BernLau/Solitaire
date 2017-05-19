using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerScript : MonoBehaviour {


    private Stack<GameObject> deck = new Stack<GameObject>();
    private Stack<GameObject> dealt = new Stack<GameObject>();
    private GameManager controller;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void pass(GameManager temp)
    {
        controller = temp;
    }

    public void add(GameObject temp)
    {
        deck.Push(temp);
    }

    public void remove()
    {
        GameObject curr = dealt.Pop();
        curr.GetComponent<Cards>().setDeckStatus(false);
    }

    public void OnMouseUpAsButton()
    {
        if (Time.timeScale == 0)
            return;
        if(deck.Count == 0)
        {
            controller.score -= 75;
            while(dealt.Count != 0)
            {
                GameObject temp = dealt.Pop();
                temp.transform.position -= new Vector3(10, 0, 0);
                temp.GetComponent<Cards>().flip();
                controller.rearrange(temp.GetComponent<Cards>());
                deck.Push(temp);
            }
        }
        else
        {
            GameObject temp = deck.Pop();
            temp.transform.position += new Vector3(10, 0, 0);
            temp.GetComponent<Cards>().flip();
            controller.rearrange(temp.GetComponent<Cards>());
            dealt.Push(temp);
        }
        
    }
}
