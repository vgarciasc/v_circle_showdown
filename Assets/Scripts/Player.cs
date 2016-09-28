using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour {
    [Header("Serialized Variables")]
    [SerializeField]
    float speed;
    [SerializeField]
    float maxVelocity;
    [SerializeField]
    float maxTackleBuildup;
    [SerializeField]
    float maxSecondsOutOfScreen = 3f;

    [Header("Prefabs and References")]
    [SerializeField]
    GameObject playerUI_prefab;
    GameObject playerUI_layout;
    Rigidbody2D rb;
    Animator anim;
    PlayerUI playerUI;
    GameController gcontroller;

    /*Reset variables*/
    Color originalColor;
    float originalMass;

    /*Joystick Input*/
    string jsHorizontal,
        jsVertical,
        jsFire1,
        jsJump;

    /*Other*/
    public int playerID = -1;
    public string joystick;
    float tackleForce,
        tackleWeight,
        tackleBuildup;
    float jumpForce;
    float sizeIncrement;
    int maxSizeIncrements,
        currentSizeIncrements;

    public void setPlayer(int playerID, string joystick, Color color) {
        this.playerID = playerID;
        this.joystick = joystick;
        this.GetComponent<SpriteRenderer>().color = color;
    }

    void Start() {
        /*References*/
        rb = GetComponent<Rigidbody2D>();
        originalColor = GetComponent<SpriteRenderer>().color;
        anim = GetComponent<Animator>();
        playerUI_layout = HushPuppy.findGameObject("Player UI Layout");
        gcontroller = HushPuppy.findGameObject("GameController").GetComponent<GameController>();

        /*Default values*/
        speed = 0.3f;
        maxVelocity = 6f;
        maxTackleBuildup = 100f;
        originalMass = rb.mass;
        tackleWeight = 0.5f;
        tackleForce = 50f;
        jumpForce = 300f;
        sizeIncrement = 0.2f;
        maxSizeIncrements = 20;
        currentSizeIncrements = 0;
        playerID = choosePlayerID();

        /*Init functions*/
        resetTackle();
        startUI();
        StartCoroutine(checkOutOfScreen());
        createJoystickInput();
    }

    void Update () {
        handleInput();
        manageTackle();
    }

    #region UI Elements
    void startUI() {
        playerUI = Instantiate(playerUI_prefab).GetComponent<PlayerUI>();
        playerUI.transform.SetParent(playerUI_layout.transform, false);
        playerUI.setUI(playerID, GetComponent<SpriteRenderer>());
    }
    #endregion

    #region Out of Screen
    IEnumerator checkOutOfScreen() {
        yield return new WaitForSeconds(1f);
        float timeLeft = maxSecondsOutOfScreen;
        while (SceneManager.GetActiveScene().name != "GameOver") {
            if (this.GetComponent<SpriteRenderer>().isVisible) {
                timeLeft = maxSecondsOutOfScreen;
                playerUI.setTime(false); 
            } else {
                playerUI.setTime(timeLeft--); }

            if (timeLeft < 0) timeOut(); 

            yield return new WaitForSeconds(1f);
        }
    }

    public void timeOut() { killPlayer(); }
    #endregion

    #region Hit and Run
    void createJoystickInput() {
        if (joystick.Length == 0) joystick = "_J0";

        jsJump = "Jump" + joystick;
        jsFire1 = "Fire1" + joystick;
        jsHorizontal = "Horizontal" + joystick;
        jsVertical = "Vertical" + joystick;
    }

    void handleInput() {
        float h_mov = Input.GetAxis(jsHorizontal) * speed;
        if (Mathf.Abs(rb.velocity.x) < maxVelocity) 
            rb.velocity += new Vector2(h_mov, 0);
        if (Input.GetButtonDown(jsJump)) 
            jump();
        if (Input.GetButtonDown(jsFire1)) 
            resetTackle();
        if (Input.GetButton(jsFire1)) 
            tackleBuildup += 1f;
        if (Input.GetButtonUp(jsFire1)) 
            releaseTackle();
    }

    void jump() {
        rb.AddForce(new Vector2(0, jumpForce));
    }

    void OnCollisionEnter2D(Collision2D target) {
        if (target.gameObject.tag == "Player" && isLookingAtObject(target.transform))
            target.gameObject.GetComponent<Player>().takeHit();
        
        if (target.gameObject.tag == "Spikes")
            killPlayer();
    }

    public void takeHit() {
        this.transform.localScale += new Vector3(sizeIncrement, sizeIncrement);
        currentSizeIncrements++;
        if (currentSizeIncrements > maxSizeIncrements)
            killPlayer();
    }

    void killPlayer() {
        anim.SetTrigger("explode");
        playerUI.playerKilled();
    }

    //to be used only by animation
    void AnimationKillPlayer() {
        gcontroller.checkGameOver();
        Destroy(this.gameObject);
    }

    bool isLookingAtObject(Transform target) {
        float angle = 50f;
        float angleBetweenPlayers = Mathf.Abs(Vector3.Angle(this.transform.up, transform.position - target.position) - 180f);
        return (angleBetweenPlayers < angle);
    }
    #endregion

    #region Tackle Bell
    void resetTackle() {
        tackleBuildup = 0f;
        this.GetComponent<SpriteRenderer>().color = originalColor;
        rb.mass = originalMass;
    }
    
    void manageTackle() {
        if (tackleBuildup >= maxTackleBuildup)
            tackleBuildup = maxTackleBuildup;

        float perc = tackleBuildup / maxTackleBuildup;

        this.GetComponent<SpriteRenderer>().color = new Color(originalColor.r, 
                                                            originalColor.g - perc,
                                                            originalColor.b - perc);

        rb.mass = originalMass + tackleWeight * perc;
    }

    void releaseTackle() {
        float perc = tackleBuildup / maxTackleBuildup;
        Vector2 direction = this.transform.up * tackleForce * perc;
        rb.velocity += direction;
        //rb.velocity += new Vector2(Mathf.Sign(rb.velocity.x) * 10f, 0);
        resetTackle();
    }
    #endregion

    int choosePlayerID() {
        int id = 0;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            id++;
            if (player == this.gameObject)
                return id;
        }

        return -1;
    }
}
