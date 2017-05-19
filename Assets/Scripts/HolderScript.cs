using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolderScript : MonoBehaviour {

    public List<GameObject> storage = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int getSize()
    {
        return storage.Count;
    }

    public bool add(GameObject temp)
    {
        
        Cards curr = temp.GetComponent<Cards>();
        if (curr.getNext() != null)
            return false;
        if(storage.Count == 0)
        {
            if (curr.number == 1)
            {
                storage.Add(temp);
                if(curr.getPrev() != null)
                {
                    if(!curr.getPrev().faceStatus())
                        curr.getPrev().flip();
                    curr.getPrev().setNext(null);
                    curr.setPrev(null);
                }
                if (curr.getDeckStatus())
                {
                    curr.getController().used();
                }

                return true;
            }
            else
                return false;
        }
        else
        {
            Cards pre = storage[storage.Count - 1].GetComponent<Cards>();
            if (pre.suite == curr.suite && pre.number == curr.number - 1)
            {
                storage.Add(temp);
                if (curr.getPrev() != null)
                {
                    if (!curr.getPrev().faceStatus())
                        curr.getPrev().flip();
                    curr.getPrev().setNext(null);
                    curr.setPrev(null);
                }
                if (curr.getDeckStatus())
                {
                    curr.getController().used();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
