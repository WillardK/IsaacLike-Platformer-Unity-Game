using UnityEngine;
using System.Collections;

public class Background_Script : MonoBehaviour {

    Vector3 position;
    public GameObject player;  //set in Unity

    SpriteRenderer renderer;
    float size;

	// Use this for initialization
	void Start () {
        position = transform.position;
        renderer = GetComponent<SpriteRenderer>();
        size = renderer.bounds.size.x - .01f;
	}
	
	// Update is called once per frame
	void Update () {
	    
        //IF the player gets too far away, teleport to other side
        if (player.transform.position.x - position.x < -6)
        {
            position.x = position.x - (3 * size);
            transform.position = position;
        }
        else if (player.transform.position.x - position.x > 6)
        {
            position.x = position.x + (3 * size);
            transform.position = position;
        }
	}
}
