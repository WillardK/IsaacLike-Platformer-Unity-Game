using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {

    static GameManagerScript instance;
    string currentLevel = "Level1";

    public GameObject player;
    public GameObject camera;

    //items
    public GameObject JetBoots;
    public GameObject Cloud9;

    //bosses
    public GameObject M24Chappie;


    static ArrayList ItemList = new ArrayList();
    static ArrayList BossList = new ArrayList();


    void start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        //Initialize the item and boss lists
        InitializeItemList();
        InitializeBossList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            currentLevel = "Level1";
            SceneManager.LoadScene("Level1");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {

        }


    }

    public void PlayerDied()
    {
        //SceneManager.LoadScene(currentLevel);
        player.transform.position = new Vector2(1f, .6f);
        camera.transform.position = new Vector2(1f, .6f);
    }

    public void RestartGame()
    {

        SceneManager.LoadScene("Level1");
        

    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void NextScene()
    {
        Scene current = SceneManager.GetActiveScene();

        switch(current.name)
        {
            case "Level1":
                SceneManager.LoadScene("Level2");
                break;
            case "Level2":
                SceneManager.LoadScene("Level3");
                break;

        }
    }

    //Holds all of the items in the game
    void InitializeItemList()
    {
        //Add all items to the arraylist
        ItemList.Add(JetBoots);
        ItemList.Add(Cloud9);

        //shuffle the item order
        for (var i = ItemList.Count - 1; i > 0; i--)
        {
            var r = Random.Range(0, i);
            var tmp = ItemList[i];
            ItemList[i] = ItemList[r];
            ItemList[r] = tmp;
        }
    }

    //Holds all of the bosses in the game
    void InitializeBossList()
    {
        //Add all items to the arraylist
        BossList.Add(M24Chappie);

        //shuffle the item order
        for (var i = BossList.Count - 1; i > 0; i--)
        {
            var r = Random.Range(0, i);
            var tmp = BossList[i];
            BossList[i] = BossList[r];
            BossList[r] = tmp;
        }
    }

    //Return an item from the itemlist for level Generators
    public GameObject GetItem()
    {
        GameObject cur;

        //Check if there are items left
        if (ItemList.Count <= 0)
        {
            InitializeItemList();
        }

        cur = ItemList[0] as GameObject;            //just take the first one,
        ItemList.RemoveAt(0);                       //  Delete it from the list and return it
        return cur;
    }

    //Return an item from the itemlist for level Generators
    public GameObject GetBoss()
    {
        GameObject cur;

        //Check if there are items left
        if (BossList.Count <= 0)
        {
            InitializeBossList();
        }

        cur = BossList[0] as GameObject;            //just take the first one,
        BossList.RemoveAt(0);                       //  Delete it from the list and return it
        return cur;
    }

}
