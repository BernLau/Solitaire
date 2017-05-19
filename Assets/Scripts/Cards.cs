using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cards : MonoBehaviour{

    public int number;
    public int suite;
    private int zOrder;
    private int sortingOrder;
    private GameManager controller;
    public Sprite cardBack;
    private Sprite face;
    private bool isFaceUp;
    private Cards previous;
    private Cards next;
    private bool pickedUp;
    private Vector3 origLoc;
    private bool beingDealt;
    //1 = clubs, 2 = diamonds, 3 = hearts, 4 = spades

    public void setVal(int z, int order, GameManager control, bool faceUp, Cards prev, Cards nex, bool isDeal)
    {
        zOrder = z;
        sortingOrder = order;
        controller = control;
        setOrder(order);
        setZ(z);
        previous = prev;
        next = nex;
        pickedUp = false;
        isFaceUp = faceUp;
        beingDealt = isDeal;
        face = GetComponent<SpriteRenderer>().sprite;
        if (!faceUp)
        {
            GetComponent<SpriteRenderer>().sprite = cardBack;
            GetComponent<Transform>().localScale += new Vector3((float)-.02, 0, 0);
        }
            
    }

    public void flip()
    {
        if(isFaceUp)
        {
            GetComponent<SpriteRenderer>().sprite = cardBack;
            GetComponent<Transform>().localScale += new Vector3((float)-.02, 0, 0);
            isFaceUp = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = face;
            transform.localScale += new Vector3((float).02, 0, 0);
            isFaceUp = true;
        }
    }

    public void OnMouseDown()
    {
        if (Time.timeScale == 0)
            return;
        if(isFaceUp)
        {
            pickedUp = true;
            Cards temp = this;
            while (temp != null)
            {
                controller.rearrange(temp);
                temp = temp.next;
            }
            Cards curr = this;
            while (curr != null)
            {
              curr.origLoc = curr.transform.position;
              curr = curr.next;
            }
            
        }
    }

    public void OnMouseUp()
    {
        if (!pickedUp || Time.timeScale == 0)
            return;
        pickedUp = false;
        RaycastHit hit;
        BoxCollider collide = GetComponent<BoxCollider>() as BoxCollider;
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, -2), transform.forward);
        Ray ray2 = new Ray(transform.position + collide.center, transform.forward);
        bool fin = false;
        if (Physics.Raycast(ray, out hit))
        {
            HolderScript hold = hit.collider.gameObject.GetComponent<HolderScript>();
            
            if (hit.collider.gameObject.GetComponent<Cards>() != null)
            {
                fin = false;
            }
            else if(hold != null)
            {
                isHolder(hold);
                fin = true;
            }
        }
        if (!fin && Physics.Raycast(ray2, out hit))
        {
            Debug.Log(hit.collider.gameObject);
            Cards check = hit.collider.gameObject.GetComponent<Cards>();
            HolderScript hold = hit.collider.gameObject.GetComponent<HolderScript>();
            if (check != null)
            {
                if (!check.beingDealt)
                    fin = isCard(check);
                else
                    resetLocs();
            }
            else if (hold != null)
            {
                isHolder(hold);
            }
            else
            {
                if(number == 13)
                {
                    if (beingDealt)
                        controller.used();
                    setNewLocs(hit.collider.gameObject, false);
                    fin = true;
                }
                
            }
        }
        if(!fin)
            resetLocs();
    }

    public void isHolder(HolderScript hold)
    {
        if (hold.add(this.gameObject))
        {
            transform.position = hold.gameObject.transform.position;
            controller.score += 10;
        }
        else
            resetLocs();
    }

    public bool isCard(Cards check)
    {
        bool valid = false;
        if (number + 1 != check.getNumber())
        {
            return false;
        }
        else
        {
            if (suite == 1 || suite == 4)
            {
                if (check.getSuite() == 1 || check.getSuite() == 4)
                    return false;
                else
                {
                    valid = true;
                    if(beingDealt)
                    {
                        controller.used();
                        controller.score += 5;
                        beingDealt = false;
                    }
                }
            }
            else
            {
                if (check.getSuite() == 2 || check.getSuite() == 3)
                    return false;
                else
                {
                    valid = true;
                    if (beingDealt)
                    {
                        controller.used();
                        controller.score += 5;
                        beingDealt = false;
                    }
                }
            }
        }
        if (valid)
        {
            setNewLocs(check.gameObject, true);
            return true;
        }
        return false;
    }

    public void setNewLocs(GameObject check, bool start)
    {
        Cards curr = this;
        int count = 0;
        if(start)
        {
            count = 1;
        }
        while (curr != null)
        {
            curr.transform.position = check.transform.position + new Vector3(0, -2 * count, 0);
            count++;
            curr = curr.next;
        }

        if (previous != null)
        {
            if (!previous.isFaceUp)
            {
                previous.flip();
                controller.score += 5;
                previous.next = null;
            }
        }
        if (start)
        {
            check.GetComponent<Cards>().setNext(this);
            setPrev(check.GetComponent<Cards>());
        }
        else
            setPrev(null);
        
    }

    public void resetLocs()
    {
        Cards curr = this;
        int count = 0;
        while (curr != null)
        {
            curr.transform.position = curr.origLoc;
            count++;
            curr = curr.next;
        }
    }

    public void setOrder(int x)
    {
        sortingOrder = x;
        SpriteRenderer render = GetComponent<SpriteRenderer>() as SpriteRenderer;
        render.sortingOrder = sortingOrder;
    }

    public void setZ(int x)
    {
        zOrder = x;
        BoxCollider collide = GetComponent<BoxCollider>() as BoxCollider;
        collide.center = new Vector3(0, 0, zOrder);
    }

    public int getOrder()
    {
        return sortingOrder;
    }

    public int getZ()
    {
        return zOrder;
    }

    public int getNumber()
    {
        return number;
    }

    public int getSuite()
    {
        return suite;
    }

    public void setPrev(Cards use)
    {
        previous = use;
    }

    public Cards getPrev()
    {
        return previous;
    }

    public void setNext(Cards use)
    {
        next = use;
    }

    public Cards getNext()
    {
        return next;
    }

    public bool getDeckStatus()
    {
        return beingDealt;
    }

    public void setDeckStatus(bool stat)
    {
        beingDealt = stat;
    }

    public GameManager getController()
    {
        return controller;
    }

    public bool faceStatus()
    {
        return isFaceUp;
    }



    public void OnMouseDrag()
    {
        if (Time.timeScale == 0)
            return;
        if(isFaceUp)
        {
            Cards curr = this;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            int count = 0;
            while(curr != null)
            {
                curr.transform.position = mousePos + new Vector3(0, count * -2, 0);
                count++;
                curr = curr.next;
            }
        }
    }
}
