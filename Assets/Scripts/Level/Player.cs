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
    GameObject playerStatus_prefab;
    [SerializeField]
    GameObject playerMarker_prefab;
    [SerializeField]
    ParticleSystem explosion_psystem;

    GameObject playerStatus_container;

    Rigidbody2D rb;
    Animator anim;
    PlayerUIStatus playerStatus;
    PlayerUIMarker playerMarker;
    GameController gcontroller;
    SpecialCamera scamera;

    /*Reset variables*/
    Color originalColor;
    float originalMass;

    /*Joystick Input*/
    string jsHorizontal,
        jsVertical,
        jsFire1,
        jsJump;

    /*Other*/
    [Header("Other")]
    public int playerID = -1;
    public string joystick;
    float tackleForce,
        tackleWeight,
        tackleBuildup;
    float jumpForce;
    float hitSizeIncrement,
        timeSizeIncrement,
        maxSize,
        minSize;
    int corneredDetected = 0;

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
        gcontroller = HushPuppy.findGameObject("GameController").GetComponent<GameController>();
        scamera = Camera.main.GetComponent<SpecialCamera>();

        /*Default values*/
        speed = 0.3f;
        maxVelocity = 6f;
        maxTackleBuildup = 100f;
        originalMass = rb.mass;
        tackleWeight = 0.5f;
        tackleForce = 50f;
        jumpForce = 300f;
        hitSizeIncrement = 0.2f;
        timeSizeIncrement = 0.02f;
        maxSize = 15f;
        minSize = 0.6f;

        /*Init functions*/
        resetTackle();
        startUI();
        startPsystem();
        StartCoroutine(checkOutOfScreen());
        StartCoroutine(grow());
        createJoystickInput();
    }

    void Update () {
        handleInput();
        manageTackle();
        updateMarker();
    }

    #region UI Elements
    void startUI() {
        GameObject playerUI_container = HushPuppy.findGameObject("Player UI Container");

        playerStatus = Instantiate(playerStatus_prefab).GetComponent<PlayerUIStatus>();
        playerStatus.name = "Player " + (playerID + 1) + " Status";
        playerStatus.transform.SetParent(playerUI_container.transform.GetChild(0), false);
        playerStatus.setUI(playerID + 1, GetComponent<SpriteRenderer>());

        playerMarker = Instantiate(playerMarker_prefab).GetComponent<PlayerUIMarker>();
        playerMarker.name = "Player " + (playerID + 1) + " Marker";
        playerMarker.transform.SetParent(playerUI_container.transform.GetChild(1), false);
        playerMarker.setMarker(this.originalColor);
    }

    void updateMarker() {
        playerMarker.setPosition(this.transform.position);
    }
    #endregion

    #region Out of Screen
    IEnumerator checkOutOfScreen() {
        yield return new WaitForSeconds(1f);
        float timeLeft = maxSecondsOutOfScreen;
        while (SceneManager.GetActiveScene().name != "GameOver") {
            if (this.GetComponent<SpriteRenderer>().isVisible) {
                timeLeft = maxSecondsOutOfScreen;
                playerStatus.setTime(false); 
            } else {
                playerStatus.setTime(timeLeft--); }

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
        //if (Input.GetKeyDown(KeyCode.G))
        //    Camera.main.GetComponent<SpecialCamera>().screenShake_(0.1f);
        if (Input.GetKeyDown(KeyCode.K))
            killPlayer();

        float h_mov = Input.GetAxis(jsHorizontal) * speed;
        if (Mathf.Abs(rb.velocity.x) < maxVelocity)
            move(h_mov);
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

    void move(float movement) {
        rb.velocity += new Vector2(movement, 0);
    }

    void OnCollisionEnter2D(Collision2D target) {
        if (target.gameObject.tag == "Player" && isLookingAtObject(target.transform)) {
            float hitStrength = velocityHitMagnitude(rb.velocity);
            shakeScreen(hitStrength);
            target.gameObject.GetComponent<Player>().takeHit(hitSizeIncrement + hitStrength);
            giveHit(hitSizeIncrement + hitStrength);
            //float hitStrength = velocityHitMagnitude(rb.velocity);
            //target.gameObject.GetComponent<Player>().takeHit(hitStrength);
        }
    }

    void OnTriggerEnter2D(Collider2D target) {
        if (target.gameObject.tag == "Spikes")
            killPlayer();
    }

    void shakeScreen(float hitStrength) {
        scamera.screenShake_(hitStrength);
    }

    public void takeHit(float transferSize) {
        changeSize(transferSize);
    }

    void giveHit(float transferSize) {
        this.changeSize(- transferSize);
    }

    float velocityHitMagnitude(Vector2 velocity) {
        return velocity.magnitude / 40;
    }

    void killPlayer() {
        anim.SetTrigger("explode");
        playerStatus.playerKilled();
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
    IEnumerator grow() {
        while (true) {
            yield return new WaitForEndOfFrame();
            changeSize(timeSizeIncrement * Time.deltaTime);
        }
    }

    void changeSize(float sizeIncrement) {
        this.transform.localScale += new Vector3(sizeIncrement, sizeIncrement);
        checkIfExplodingSize();
        if (this.transform.localScale.x < minSize)
            this.transform.localScale = new Vector2(minSize, minSize);
    }

    void checkIfExplodingSize() {
        if (this.transform.localScale.magnitude > maxSize)
            killPlayer();
    }

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

    #region Cornered Detection
    public void corneredDetection(bool value) {
        corneredDetected++;
        if (corneredDetected >= 2)
            killPlayer();
    }
    #endregion

    #region Particle System
    void startPsystem() {
        Color aux = originalColor;
        aux += new Color(0.3f, 0.3f, 0.3f);
        explosion_psystem.startColor = aux;
    }

    void startEmission() {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        scamera.screenShake_(2f);
        explosion_psystem.gameObject.SetActive(true);
    }
    #endregion
}
