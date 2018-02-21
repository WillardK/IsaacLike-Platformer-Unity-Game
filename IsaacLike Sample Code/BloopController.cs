using UnityEngine;
using System.Collections;

public class BloopController : MonoBehaviour {

    GameObject player;
    GameObject explosion;
    GameObject heart;

    public int bloopSpeed = 0;

    bool active = false;
    Vector2 position;
    Rigidbody2D rigidbody;
    Animator animator;
    SpriteRenderer spriterender;
    Collider2D collider;
    AudioSource audio;


    // Use this for initialization
    void Start () {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriterender = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        player = gameObject.transform.parent.gameObject.GetComponent<EnemyController>().player;
        explosion = gameObject.transform.parent.gameObject.GetComponent<EnemyController>().explosion;
        heart = gameObject.transform.parent.gameObject.GetComponent<EnemyController>().heart;
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
	
	}

    void FixedUpdate()
    {
        //Stay inactive until player gets close enough
        if (!active)
        {
            if (player.transform.position.x > transform.position.x - 5)
            {
                active = true;
                rigidbody.velocity = new Vector2(bloopSpeed, rigidbody.velocity.y);
            }
        }
        else
        {
            //Flip Bloop if it hits a wall
            if (rigidbody.velocity.magnitude < .1)
            {
                flipBloop();
            }

            //Assign Velocity
            rigidbody.velocity = new Vector2(bloopSpeed, rigidbody.velocity.y);
        }
    }

    //Kill Bloop when jumped on
    void OnTriggerEnter2D(Collider2D col)
    {
        //Kill Bloop
        if (col.gameObject.tag == "Player")
        {
            die();
        }
        else if (col.gameObject.tag == "Death")
        {
            Destroy(gameObject);
        }
        else if(col.gameObject.tag == "Tear")
        {
            Destroy(gameObject);
        }
    }

    //Make bloops bounce off each other
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            flipBloop();
        }
        else if (col.gameObject.tag == "Bloop")
        {
            flipBloop();
        }
        else if (col.gameObject.tag == "Diekon")
        {
            flipBloop();
        }
        else if (col.gameObject.tag == "Rudeabaga")
        {
            flipBloop();
        }
    }

    void flipBloop()
    {
        bloopSpeed *= -1;
        spriterender.flipX = !spriterender.flipX;
    }


    //If hit, play animation, stop, and disable collisions
    void die()
    {
        audio.Play();
        rigidbody.velocity = Vector2.zero;
        rigidbody.gravityScale = 0;
        

        foreach (Collider c in gameObject.GetComponents<Collider>())
        {
            c.enabled = false;
        }

        animator.SetBool("IsDead", true);
    }

    public void destroy()
    {
        //make explosion
        Instantiate(explosion, transform.position, transform.rotation);

        //random chance to drop a heart
        if (Random.Range(0, 10) < 3)
        {
            Instantiate(heart, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
