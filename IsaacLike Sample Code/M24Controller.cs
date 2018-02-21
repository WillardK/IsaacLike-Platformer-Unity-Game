using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M24Controller : MonoBehaviour {

    GameObject player;
    GameObject explosion;
    GameObject heart;

    GameObject Gate;
    GameObject Gate2;

    Vector2 position;
    Rigidbody2D rigidbody;
    Animator animator;
    SpriteRenderer spriterender;
    Collider2D collider;

    float v = -5f;
    public int health = 3;
    public bool active;

    // Use this for initialization
    void Start () {

        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriterender = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        player = gameObject.transform.parent.gameObject.GetComponent<EnemyController>().player;
        explosion = gameObject.transform.parent.gameObject.GetComponent<EnemyController>().explosion;
        heart = gameObject.transform.parent.gameObject.GetComponent<EnemyController>().heart;
    }
	
	// Update is called once per frame
	void Update () {

        // Move if active
        if (active)
        {
            //turn around if stopped
            if (rigidbody.velocity.magnitude < .01f)
            {
                v *= -1;
            }

            //apply velocity every frame
            rigidbody.velocity = new Vector2(v, 0);
        }
        // Else check player position and activate if close
        else
        {       
            if (transform.position.x - player.transform.position.x < 2)
            {   //Activate, and move the gates down
                active = true;
                Gate = GameObject.Find("BossRoom(Clone)/Gate").gameObject;
                Gate2 = GameObject.Find("BossRoom(Clone)/Gate2").gameObject;
                Gate.transform.position = new Vector2(Gate.transform.position.x, Gate.transform.position.y - 1.56f);
                Gate2.transform.position = new Vector2(Gate.transform.position.x, Gate.transform.position.y - 1.56f);
            }
        }
	}

    // Take Damage
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Tear")
        {
            takeDamage();
        }
    }

    // Something here someday maybe
    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    //Handle taking damage, speeding up, and dying
    void takeDamage()
    {
        health--;

        if(health == 0)
        {
            die();
        }

        //speed up
        v *= 3;
    }

    void die()
    {
        Instantiate(explosion, transform.position, transform.rotation);

        //random chance to drop a heart
        if (Random.Range(0, 10) < 3)
        {
            Instantiate(heart, transform.position, transform.rotation);
        }
        Destroy(Gate);
        Destroy(Gate2);
        Destroy(gameObject);
    }
}
