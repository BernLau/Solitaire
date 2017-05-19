using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private List<GameObject> deck = new List<GameObject>();
    public List<GameObject> pack = new List<GameObject>();
    public GameObject dealer;
    private GameObject createdDealer;
    public GameObject holder;
    public GameObject emptySpot;
    private GameObject[] theHolders;
    public Text winText;
    public Text scoreText;
    public Text highScore;
    public int score;
	// Use this for initialization
	void Start ()
    {
        if(PlayerPrefs.GetInt("highscore", 0) == 0)
            PlayerPrefs.SetInt("highscore", 0);
        score = 0;
        winText.text = "";
        scoreText.text = "Score: 0";
        highScore.text = "High Score: " + PlayerPrefs.GetInt("highscore");
        theHolders = new GameObject[4];
        int z = 51;
        int order = 0;
        Vector3 loc = new Vector3(-30, 5, 0);
        Cards prev = null;
        for(int x = 1; x <= 7; x++)
        {
            for(int y = 0; y < x; y++)
            {
                if (y == 0)
                {
                    Instantiate(emptySpot, loc, Quaternion.identity);
                }
                int rand = (int)(Random.value * pack.Count);
                GameObject curr = Instantiate(pack[rand], loc, Quaternion.identity) as GameObject;
                pack.RemoveAt(rand);
                Cards temp = curr.GetComponent<Cards>() as Cards;
                bool faceUp = y == x - 1;
                temp.setVal(z, order, this, faceUp, prev, null, false);
                if(prev != null)
                {
                    prev.setNext(temp);
                }
                prev = temp;
                z--;
                order++;
                loc += Vector3.down;
                deck.Add(curr);
            }
            loc = new Vector3(-30 + 10 * x, 5, 0);
            prev = null;
        }
        loc = new Vector3(-30, 15, 0);
        createdDealer = Instantiate(dealer, loc, Quaternion.identity) as GameObject;
        createdDealer.GetComponent<SpriteRenderer>().sortingOrder = -1;
        createdDealer.GetComponent<BoxCollider>().center = new Vector3(0, 0, -1);
        DealerScript set = createdDealer.GetComponent<DealerScript>();
        set.pass(this);
        for (int x = 0; x < 24; x++)
        {
            int rand = (int)(Random.value * pack.Count);
            GameObject curr = Instantiate(pack[rand], loc, Quaternion.identity) as GameObject;
            set.add(curr);
            pack.RemoveAt(rand);
            Cards temp = curr.GetComponent<Cards>() as Cards;
            temp.setVal(z, order, this, false, null, null, true);
            z--;
            order++;
            deck.Add(curr);
        }
        loc = new Vector3(0, 15, 0);
        for(int x = 0; x < 4; x++)
        {
            GameObject temp = Instantiate(holder, loc + new Vector3(10 * x, 0, 0), Quaternion.identity) as GameObject;
            temp.GetComponent<SpriteRenderer>().sortingOrder = -1;
            temp.GetComponent<BoxCollider>().center = new Vector3(0, 0, -1);
            theHolders[x] = temp;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Input.GetKey("escape"))
            Application.Quit();
        scoreText.text = "Score: " + score;
        bool done = true;
        for(int x = 0; x < 4; x++)
        {
            if (theHolders[x].GetComponent<HolderScript>().getSize() != 13)
                done = false;
        }
        if(done)
        {
            finish();
        }
    }

    public void finish()
    {
        winText.text = "You Win!";
        if (score > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", score);
            winText.text += "\nYou got a new high score!!!";
            highScore.text = "High Score: " + score;
        }
        Time.timeScale = 0;
    }

    public void rearrange(Cards check)
    {
        int prevOrder = check.getOrder();
        int prevZ = check.getZ();
        for (int x = 0; x < 52; x++)
        {
            Cards curr = deck[x].GetComponent<Cards>();
            if (curr.Equals(check))
            {
                curr.setOrder(51);
                curr.setZ(0);
            }
            else
            {
                if (curr.getOrder() > prevOrder)
                    curr.setOrder(curr.getOrder() - 1);
                if(curr.getZ() < prevZ)
                    curr.setZ(curr.getZ() + 1);
            }
            
        }
    }

    public void used()
    {
        createdDealer.GetComponent<DealerScript>().remove();
    }

    public void resetGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
