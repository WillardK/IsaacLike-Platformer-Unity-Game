using UnityEngine;
using System.Collections;


/// <summary>
/// 
/// Should be Under the EnemyController Object
/// 
/// </summary>
public class ChappieShopkeepScript : MonoBehaviour {

    GameObject player;
    GameObject explosion;
    GameObject heart;
    public GameObject Gate;
    public GameObject Gate2;

    float speed = 4;
    float jumpSpeed = 6;
    float health = 3;
    public bool invincible = false;

    float walltimer;
    bool onWall = false;

    public bool active = false;
    bool OnGround = false;
    Vector2 position;
    Rigidbody2D rigidbody;
    Animator animator;
    SpriteRenderer spriterender;
    Collider2D collider;
    AudioSource audio;

    LayerMask Ground;
    LayerMask Platform;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriterender = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        player = gameObject.transform.parent.gameObject.GetComponent<EnemyController>().player;
        explosion = gameObject.transform.parent.gameObject.GetComponent<EnemyController>().explosion;
        heart = gameObject.transform.parent.gameObject.GetComponent<EnemyController>().heart;
        audio = GetComponent<AudioSource>();

        Ground = LayerMask.GetMask("Ground");
        Platform = LayerMask.GetMask("TransparentFX");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        //Stay inactive until hit by player
        if (!active)
        {
          
        }

        //If active
        else
        {
            //Detect Ground
            OnGround = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - .13f), 0.05f, Ground);
            OnGround = OnGround || Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - .13f), 0.05f, Platform);

            animator.SetBool("IsGrounded", OnGround);

            //wait out invincibility
            if (invincible)
            {

            }
            else
            {
                Physics2D.IgnoreLayerCollision(1, 11, true);

                //Detect if a wall has been hit for a while, and move as needed
                if (rigidbody.velocity.x < .1f && !onWall)
                {
                    walltimer = Time.time;
                    onWall = true;

                }
                else if (rigidbody.velocity.x < .1f && onWall && Time.time - walltimer > .5f)
                {
                    onWall = false;
                    flip();
                }
                else if (rigidbody.velocity.x > .1f)
                {
                    onWall = false;
                }

                //Assign Velocity and jump if grounded
                if (OnGround && !onWall)
                {
                    rigidbody.velocity = new Vector2(speed, jumpSpeed);
                }
                else
                {
                    rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);
                }
            }
        }
    }

    //Handle Trigger events
    void OnTriggerEnter2D(Collider2D col)
    {
        //Take Damage when jumped on or shot
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Tear")
        {
            if (active)
            {
                takedamage(col);
            }
            else
            {
                active = true;
                animator.SetBool("IsActive", true);
                spriterender.flipX = !spriterender.flipX;

                //Move Gate down
                Gate.transform.position = new Vector2(Gate.transform.position.x, Gate.transform.position.y - 1.56f);
                Gate2.transform.position = new Vector2(Gate2.transform.position.x, Gate2.transform.position.y - 1.56f);
            }
        }
        else if (col.gameObject.tag == "Death")
        {
            Destroy(gameObject);
        }
    }

    //Make bloops bounce off each other
    void OnCollisionEnter2D(Collision2D col)
    {
        string side = SideWasHit(col);

        //Debug.Log(side);

        if (side == "side")
        {
            flip();
        }
    }


    void flip()
    {
        speed *= -1;
        spriterender.flipX = !spriterender.flipX;
    }

    //Detect which side was hit
    string SideWasHit(Collision2D col)
    {
        Vector3 contactpoint = col.contacts[0].point;
        Vector3 center = collider.bounds.center;

        if (contactpoint.x >= transform.position.x - .1f && contactpoint.x < transform.position.x + .1f)
        {
            return "bottom";
        }
        else if (contactpoint.y > transform.position.y + .1f)
        {
            return "top";
        }
        else
        {
            return "side";
        }
    }




    void takedamage(Collider2D col)
    {
        audio.Play();

        if (health == 0)
        {
            die();
        }
        else
        {
            health--;
            invincible = true;

            //Recoil away from the player
            if (transform.position.x - col.transform.position.x > 0)
            {
                rigidbody.velocity = new Vector2(5f, 5f);

            }
            else
            {
                rigidbody.velocity = new Vector2(-5f, 5f);

            }



            animator.SetBool("Damage", true);
        }
    }

    //If hit, play animation, stop, and disable collisions
    void die()
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.gravityScale = 0;

        //clear the gate out of the way
        Destroy(Gate);
        Destroy(Gate2);

        foreach (Collider c in gameObject.GetComponents<Collider>())
        {
            c.enabled = false;
        }

        animator.SetBool("IsDead", true);
    }

    public void invincibilityStop()
    {
        invincible = false;
        animator.SetBool("Damage", false);
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
