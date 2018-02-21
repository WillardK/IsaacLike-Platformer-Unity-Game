using UnityEngine;
using System.Collections;

public class MC_ControllerScript : MonoBehaviour {

    Animator animator;
    Rigidbody2D mc_rigidbody;
    SpriteRenderer spriterender;
    MC_UIScript uiscript;
    GameManagerScript GMScript;
    AudioSource audiosource;
    ChappieShopkeepScript csscript;

    bool OnGround = true;
    float walkspeed = 30f;
    float xvelocity_max = 2.5f;
    bool flipped = false;
    bool invincible = false;
    float yvelocity = 0;
    bool platformEnabled = true;
    public bool controlsEnabled = false;            //Controls start disabled and then are enabled by the introscreen object

    float xinput = 0;
    float yinput = 0;

    //Might replace this
    bool jumping = false;

    //new jumping code
    float minJumpHeight = .5f;
    float maxJumpHeight = 2;
    float minJumpVelocity;
    float maxJumpVelocity;

    float FrictionCoefficient = .1f;
    float timer;

    //Player Stats
    static public int maxHealth = 3;
    static public int health = 3;
    static public int coinCount = 0;
    static public int lifeCount = 3;

    //Item variables
    static public bool hasJetBoots = false;
    static public bool hasCloud9 = false;
    static public bool hasBibleThump = false;


    //Status Variables
    float dbljumpsleft = 1;
    public int tearsLeft = 2;




    public GameObject GameManager;

    public GameObject Tear;

    public AudioClip AudioJump;
    public AudioClip AudioDie;



    LayerMask Ground;
    LayerMask Platform;


    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        mc_rigidbody = GetComponent<Rigidbody2D>();
        spriterender = GetComponent<SpriteRenderer>();

        uiscript = GetComponent<MC_UIScript>();     //update UI immediately in case of scene change
        uiscript.updateCoins(coinCount);
        uiscript.updateHealth(health);
        uiscript.updateLives(lifeCount);



        GMScript = GameManager.GetComponent<GameManagerScript>();
        audiosource = GetComponent<AudioSource>();

        Ground = LayerMask.GetMask("Ground");
        Platform = LayerMask.GetMask("TransparentFX");

        //New jump physics
        minJumpVelocity = Mathf.Sqrt(2f*9.8f*minJumpHeight);
        maxJumpVelocity = Mathf.Sqrt(2f*9.8f*maxJumpHeight);

        //Initialize Item stats
        InitializeItemStats();
    }

    //Initialize all of the item stats
    void InitializeItemStats()
    {
        if(hasCloud9)
            GetComponent<Rigidbody2D>().gravityScale = .5f;
    }


    // Used Update For Input
    void Update () {

        //Get Horizontal input
        if(controlsEnabled)
        {
            xinput = Input.GetAxis("Horizontal") * walkspeed;
        }

        //Handle Jumping
        //Start jump
        if (Input.GetKeyDown(KeyCode.Space) && controlsEnabled)
        {
            //Fall through platforms
            if (Input.GetKey(KeyCode.DownArrow) && OnGround)
            {
                platformEnabled = false;
                timer = Time.time;
            }

            //Jump if on ground
            else if (Mathf.Abs(mc_rigidbody.velocity.y) < .5f || OnGround)
            {
                audiosource.Play();
                jumping = true;
                mc_rigidbody.velocity = (new Vector2(mc_rigidbody.velocity.x, maxJumpVelocity));
            }

            //Double Jumps
            else if (hasJetBoots && dbljumpsleft > 0)
            {
                dbljumpsleft--;
                audiosource.Play();
                jumping = true;
                mc_rigidbody.velocity = (new Vector2(mc_rigidbody.velocity.x, maxJumpVelocity));
            }
        }
        //If Space is released, set to min jump velocity
        else if (Input.GetKeyUp(KeyCode.Space) && jumping && controlsEnabled)
        {
            if (mc_rigidbody.velocity.y > minJumpVelocity)
            {
                mc_rigidbody.velocity = new Vector2(mc_rigidbody.velocity.x, minJumpVelocity);
            }
            jumping = false;
        }

        //Spawn at end of level for Debug
        else if (Input.GetKeyDown(KeyCode.A) && controlsEnabled)
        {
            Transform t = GameObject.Find("Enemy Controller/ChappieShopkeep(Clone)").transform;
            transform.position = new Vector2(t.position.x - 1f, t.position.y);
        }

        //Shoot tears
        else if (Input.GetKeyDown(KeyCode.LeftShift) && controlsEnabled && tearsLeft > 0)
        {
            ShootTear();
        }

        //Quit Application if ESC is pressed
        if (Input.GetKey("escape"))
            Application.Quit();
    }

    //Used Fixedupdate for applying physics
    void FixedUpdate()
    {
        float xvelocity = mc_rigidbody.velocity.x;
        float yvelocity = mc_rigidbody.velocity.y;
        Vector2 position = transform.position;

        //Enable Platforms if enabled
        if (platformEnabled)
        {
            //Ignore collisions with platforms on the way up
            Physics2D.IgnoreLayerCollision(2, 1, yvelocity > 0);
        }
        //Else count down until platforms are re-enabled
        else
        {
            //If timer is done reenable platforms
            if (Time.time - timer > .25f)
            {
                platformEnabled = true;
            }
            Physics2D.IgnoreLayerCollision(2, 1, true);
        }


        //Flip Character
        if (xinput > 0 && flipped)
        {
            spriterender.flipX = false;
            flipped = false;
        }
        else if (xinput < 0 && !flipped)
        {
            spriterender.flipX = true;
            flipped = true;
        }

        //Detect if character is grounded
        OnGround = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - .21f), 0.2f, Ground);
        OnGround = OnGround || Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - .21f), 0.15f, Platform);
        animator.SetBool("MC_OnGround", OnGround);


        //Allow only max speed
        if ((xvelocity > xvelocity_max && xinput > 0) || (xvelocity < xvelocity_max * -1 && xinput < 0))
        {
            xinput = 0;
        }

        //Apply Forces
        mc_rigidbody.AddForce(new Vector2(xinput, 0));

        //Apply Friction
        if (OnGround)
        {
            if (mc_rigidbody.velocity.x < -FrictionCoefficient)
            {
                mc_rigidbody.velocity = new Vector2(mc_rigidbody.velocity.x + FrictionCoefficient, mc_rigidbody.velocity.y);
            }
            else if (mc_rigidbody.velocity.x > FrictionCoefficient)
            {
                mc_rigidbody.velocity = new Vector2(mc_rigidbody.velocity.x - FrictionCoefficient, mc_rigidbody.velocity.y);
            }
            else
            {
                mc_rigidbody.velocity = new Vector2(0, mc_rigidbody.velocity.y);
            }

            //Reset Double Jumps
            dbljumpsleft = 1;
        }

        //Update Animations
        animator.SetFloat("MC_XVelocity", Mathf.Abs(xvelocity));
        animator.SetFloat("MC_YVelocity", yvelocity);
    }



    //Handle Collision events - used for taking damage
    void OnCollisionEnter2D(Collision2D col)
    {
        //If an enemy is hit, take damage
        if((col.gameObject.tag == "Bloop" || col.gameObject.tag == "Rudeabaga") && !invincible)
        {
            takeDamage(col);
        }
        else if (col.gameObject.tag == "DieKon" && !invincible)
        {
            takeDamage(col);
        }

        //Special handling for shopkeeps
        else if (col.gameObject.tag == "ChappieShopkeep" && !invincible)
        {
            if (csscript == null)
            {   //Get Shopkeeper object if jumped on
                csscript = col.gameObject.GetComponent<ChappieShopkeepScript>();
            }
            //Only take damage if the shopkeep isn't invincible
            if(csscript.invincible == false)
            {
                takeDamage(col);
            }

        }
    }


    //Handle Trigger events - used for jumping off stuff
    void OnTriggerEnter2D(Collider2D col)
    {
        //Bounce off Bloop and Diekon
        if (col.gameObject.tag == "Bloop" || col.gameObject.tag == "Rudeabaga" || col.gameObject.tag == "DieKon")
        {
            //Make jumping off of enemies possible
            if (Input.GetKey(KeyCode.Space))
            {
                mc_rigidbody.velocity = (new Vector2(mc_rigidbody.velocity.x, maxJumpVelocity));
                jumping = true;
            }
            else
            {
                mc_rigidbody.velocity = Vector2.zero;
                mc_rigidbody.AddForce(new Vector2(0, 200));
            }
        }

        //Shopkeeps get special handling
        else if (col.gameObject.tag == "ChappieShopkeep")
        {
            if (csscript == null)
            {   //Get Shopkeeper object if jumped on
                csscript = col.gameObject.GetComponent<ChappieShopkeepScript>();
            }
        }
        else if (col.gameObject.tag == "Coin")                  //Get Coin
        {
            getCoin();
        }
        else if (col.gameObject.tag == "Life")                  //Get Life
        {
            GetLife();
        }
        else if (col.gameObject.tag == "Death")                 //Die from death trigger
        {
            die();
        }
        else if (col.gameObject.tag == "Heart")
        {
            GetHeart();
        }

        //Handle hitting the end of the level
        else if(col.gameObject.tag == "Finish")
        {
            FinishLevel();
        }
    }

    //Register damage, start invincibility period and start damage animation
    void takeDamage(Collision2D col)
    {

        //Die if health is 0
        health--;
        if (health == 0)
        {
            die();
        }

        //otherwise start damaged stuff
        invincible = true;
        animator.SetBool("MC_Damaged", true);
        uiscript.updateHealth(health);


        // Move character backwards
        if (spriterender.flipX)
        {
            mc_rigidbody.velocity = Vector2.zero;
            mc_rigidbody.AddForce(new Vector2(100f, 100f));
        }
        else
        {
            mc_rigidbody.velocity = Vector2.zero;
            mc_rigidbody.AddForce(new Vector2(-100f, 199f));
        }
        

    }

    //Kills the player and starts the death animation
    void die()
    {
        controlsEnabled = false;

        //disable all colliders
        foreach (Collider2D c in GetComponents<Collider2D>())
        {
            c.enabled = false;
        }

        //Unfreeze rotation and send the character spinning
        mc_rigidbody.freezeRotation = false;
        mc_rigidbody.velocity = Vector2.zero;
        mc_rigidbody.AddForce(new Vector2(Random.Range(-150f, 150f), 150f));
        mc_rigidbody.AddTorque(-150f);

        //play death sound
        audiosource.clip = AudioDie;
        audiosource.Play();

        //Trigger animation
        animator.SetTrigger("MC_Dead");
    }

    //Triggered by end of dying animation - undo the dying animation and respawn the player
    public void undie()
    {
        
        //update lives
        lifeCount--;

        if (lifeCount == 0)
        {
            GMScript.RestartGame();
        }

        //Revive the player
        coinCount = 0;
        health = maxHealth;

        uiscript.updateCoins(coinCount);
        uiscript.updateLives(lifeCount);
        uiscript.updateHealth(health);

        //reenable all colliders
        foreach (Collider2D c in GetComponents<Collider2D>())
        {
            c.enabled = true;
        }

        //reset velocity, etc. to zero
        mc_rigidbody.freezeRotation = true;
        mc_rigidbody.velocity = Vector2.zero;
        mc_rigidbody.rotation = 0f;

        //reset jump audio clip
        audiosource.clip = AudioJump;

        //reenabled controls
        controlsEnabled = true;

        //Have the Game script move the player and camera back to spawn
        GMScript.PlayerDied();
    }

    void getCoin()
    {
        if (coinCount < 99)
        {
            coinCount++;
            uiscript.updateCoins(coinCount);
        }
    }

    void GetLife()
    {
        if (lifeCount < 99)
        {
            lifeCount++;
            uiscript.updateLives(lifeCount);
        }
    }

    void GetHeart()
    {
        if (health < 3)
        {
            health++;
            uiscript.updateHealth(health);
        }
    }

    void stopInvincibility()
    {
        animator.SetBool("MC_Damaged", false);
        invincible = false;
    }

    void ShootTear()
    {
        //Spawn tear slightly to the left or right dependent on where the MC is facing
        Vector2 spawnposition;
        float v;
        if(flipped)
        {
            spawnposition = new Vector2(transform.position.x - .2f, transform.position.y);
            v = -4;
        }
        else
        {
            spawnposition = new Vector2(transform.position.x + .2f, transform.position.y);
            v = 4;
        }

        //spawn tear and apply velocity 
        GameObject newtear = Instantiate(Tear, spawnposition, transform.rotation) as GameObject;
        newtear.GetComponent<Rigidbody2D>().velocity = new Vector2(v, 0);
        newtear.transform.parent = gameObject.transform;

        //Decrease tears remaining (They are increased back by the tear when it destructs)
        tearsLeft--;
    }



    //Finish the level
    void FinishLevel()
    {
        GMScript.NextScene();

        //Move the player to the beginning of the next level
        transform.position = new Vector2(1, 1);
    }
}
