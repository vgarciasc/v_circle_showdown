using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour {
    /* Custom values for testing */
    [Header("Serialized Variables")]
    [SerializeField]
    [Range(0, 1f)]
    float speed = 0.3f;
    [SerializeField]
    float maxVelocity = 6f;
    [SerializeField]
    float maxTackleBuildup = 100f;
    [SerializeField]
    float maxSecondsOutOfScreen = 3f;
    [SerializeField]
    [Range(0, 300f)]
    float tackleForce = 50f;
    [SerializeField]
    float tackleWeight = 0.5f;
    [SerializeField]
    [Range(0, 900f)]
    float jumpForce = 300f;
    [SerializeField]
    [Range(0, 1f)]
    float hitSizeIncrement = 0.2f;
    [SerializeField]
    [Range(0, 0.2f)]
    float timeSizeIncrement = 0.02f;
    [SerializeField]
    [Range(0.8f, 10.0f)]
    float timeBetweenSpurts = 1.0f;
    [SerializeField]
    float maxSize = 15f;
    [SerializeField]
    float minSize = 0.6f;
    [SerializeField]
    [Range(1, 80f)]
    float hitTransferRatio = 40f;
    [SerializeField]
    [Range(0, 60)]
    int invincibleFrames = 20;

    /*Reference to objects in scene*/
    [Header("Prefabs and References")]
    [SerializeField]
    GameObject playerStatus_prefab;
    [SerializeField]
    GameObject playerMarker_prefab;
    [SerializeField]
    ParticleSystem explosion_psystem;
    [SerializeField]
    GameObject circleCollider;
    [SerializeField]
    GameObject triangleCollider;
    [SerializeField]
    Sprite circlePlayer;
    [SerializeField]
    Sprite trianglePlayer;

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
    Vector3 originalScale;

    /*Joystick Input*/
    string jsHorizontal,
        jsVertical,
        jsFire1,
        jsFire2,
        jsFire3,
        jsJump;

    /*Other*/
    [Header("Other")]
    public int playerID = -1;
    public string joystick;
    Item.Type currentItem;
    float tackleBuildup;
    Vector3 lastVelocity;
    bool invincible = false,
        inArena = true,
        isTriangle = false;

    public void setPlayer(int playerID, string joystick, Color color) {
        this.playerID = playerID;
        this.joystick = joystick;
        this.GetComponent<SpriteRenderer>().color = color;
    }

    void Start() {
        /*References*/
        originalColor = GetComponent<SpriteRenderer>().color;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gcontroller = HushPuppy.findGameObject("GameController").GetComponent<GameController>();
        scamera = Camera.main.GetComponent<SpecialCamera>();

        /*Default values*/
        originalMass = rb.mass;
        originalScale = transform.localScale;

        /*Init functions*/
        startUI();
        startPsystem();
        startAnimator();
        createJoystickInput();
        changeSprite(circlePlayer);
        resetTackle();
        StartCoroutine(checkOutOfScreen());
        StartCoroutine(grow());
    }

    void FixedUpdate() {
        manageCollisionType();
        manageTackle();
        updateLastVelocity();
    }

    void Update() {
        handleInput();
        updateMarker();
    }

    #region Spritefest
    void changeSprite(Sprite sprite) {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    #endregion

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

    #region Input
    void createJoystickInput() {
        if (joystick.Length == 0) joystick = "_J0";

        jsJump = "Jump" + joystick;
        jsFire1 = "Fire1" + joystick;
        jsFire2 = "Fire2" + joystick;
        jsFire3 = "Fire3" + joystick;
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
            increaseTackleBuildup();
        if (Input.GetButtonUp(jsFire1)) 
            releaseTackle();
        if (Input.GetButtonDown(jsFire2))
            useItem(currentItem);
    }

    void jump() {
        if (isTriangle) return;
        rb.AddForce(new Vector2(0, jumpForce));
    }

    void move(float movement) {
        if (isTriangle) return;
        rb.velocity += new Vector2(movement, 0);
    }
    #endregion

    #region Collision Treatment
    public void signalColliderEnter(Collision2D target) {
        if (target.gameObject.tag == "Player" && isLookingAtObject(target.transform)) {
            Player enemy = target.gameObject.GetComponent<Player>();
            if (enemy.isInvincible()) return;

            float hitStrength = velocityHitMagnitude();
            shakeScreen(hitStrength);
            this.giveHit(hitSizeIncrement + hitStrength);
            enemy.takeHit(hitSizeIncrement + hitStrength);
        }
    }

    public void signalTriggerEnter(Collider2D target) {
        switch (target.gameObject.tag) {
            case "Spikes":
                killPlayer();
                break;
            case "Arena":
                inArena = true;
                break;
            case "Portal":
                teleportTo(target.gameObject);
                break;
            case "Item":
                getItem(target.gameObject.GetComponent<Item>());
                break;
        }
    }

    public void signalTriggerExit(Collider2D target) {
        if (target.gameObject.tag == "Arena")
            inArena = false;
    }

    public bool isInvincible() { return invincible; }
    public bool isInArena() { return inArena; }

    void shakeScreen(float hitStrength) { scamera.screenShake_(hitStrength); }

    void manageCollisionType() {
        if (lastVelocity.x > 15f)
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        else
            rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
    }

    public void takeHit(float transferSize) {
        changeSize(transferSize);
        StartCoroutine(temporaryInvincibility(invincibleFrames));
    }

    void giveHit(float transferSize) {
        //this.changeSize(- transferSize);
    }

    IEnumerator temporaryInvincibility(int frames) {
        invincible = true;
        for (int i = 0; i < frames; i++)
            yield return new WaitForEndOfFrame();
        invincible = false;
    }

    float velocityHitMagnitude() {
        float aux = lastVelocity.x;
        if (lastVelocity.y > aux) aux = lastVelocity.y;
        if (lastVelocity.z > aux) aux = lastVelocity.z;

        return aux/60;
    }

    void updateLastVelocity() {
        lastVelocity.z = lastVelocity.y;
        lastVelocity.y = lastVelocity.x;
        lastVelocity.x = rb.velocity.magnitude;
    }

    void killPlayer() {
        anim.enabled = true;
        anim.SetTrigger("explode");
        playerStatus.playerKilled();
        playerMarker.playerKilled();
    }

    bool isLookingAtObject(Transform target) {
        float angle = 50f;
        float angleBetweenPlayers = Mathf.Abs(Vector3.Angle(this.transform.up, transform.position - target.position) - 180f);
        return (angleBetweenPlayers < angle);
    }
    #endregion

    #region Animation
    void startAnimator() {
        anim.enabled = false;
    }

    //to be used only by animation
    void AnimationKillPlayer() {
        gcontroller.checkGameOver();
        Destroy(this.gameObject);
    }
    #endregion

    #region Size Methods
    IEnumerator grow() {
        /* Gradual Growth */
        //while (true) {
        //    yield return new WaitForEndOfFrame();
        //    changeSize(timeSizeIncrement * Time.deltaTime);
        //}

        /*'Spurts' Growth */
        while (true) {
            yield return new WaitForSeconds(timeBetweenSpurts);
            changeSize(timeSizeIncrement);
        }
    }

    void changeSize(float sizeIncrement) {
        this.transform.localScale += new Vector3(sizeIncrement, sizeIncrement);
        checkSize();
    }

    void checkSize() {
        if (this.transform.localScale.x > maxSize)
            killPlayer();
        if (this.transform.localScale.x < minSize)
            this.transform.localScale = new Vector2(minSize, minSize);
    }

    #endregion

    #region Tackle Bell
    void resetTackle() {
        tackleBuildup = 0f;
        this.GetComponent<SpriteRenderer>().color = originalColor;
        rb.mass = originalMass;
    }

    void increaseTackleBuildup() {
        tackleBuildup += 1f;
    }
    
    void manageTackle() {
        if (tackleBuildup >= maxTackleBuildup)
            tackleBuildup = maxTackleBuildup;

        float perc = tackleBuildup / maxTackleBuildup;
        perc /= 2f;

        if (originalColor.r >= originalColor.g && originalColor.r >= originalColor.b)
            this.GetComponent<SpriteRenderer>().color = new Color(originalColor.r, originalColor.g - perc, originalColor.b - perc);
        else if (originalColor.g >= originalColor.b && originalColor.g >= originalColor.r)
            this.GetComponent<SpriteRenderer>().color = new Color(originalColor.r - perc, originalColor.g, originalColor.b - perc);
        else if (originalColor.b >= originalColor.g && originalColor.b >= originalColor.r)
            this.GetComponent<SpriteRenderer>().color = new Color(originalColor.r - perc, originalColor.g - perc, originalColor.b);
        else
            this.GetComponent<SpriteRenderer>().color = new Color(originalColor.r, originalColor.g - perc, originalColor.b - perc);

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
    //caso esse codigo gere erros no futuro, voce deve simplesmente alterar os colliders de detecçao de entalamento no prefab pra que eles
    //sejam menores. a precisao de detecçao de entalamento é sacrificada, mas pelo menos gera menos bugs
    bool[] corneredDetected = new bool[2];
    public void corneredDetection(int detectorID, bool value) {
        corneredDetected[detectorID] = value;
        //if (playerID == 1) Debug.Log("corneredDetected[0]: " + corneredDetected[0]);
        //if (playerID == 1) Debug.Log("corneredDetected[1]: " + corneredDetected[1]);
        if (corneredDetected[0] && corneredDetected[1]) {
            killPlayer();
        }
    }

    //int corneredDetected = 0;
    //public void corneredDetection(int detectorID, bool value) {
    //    if (value) corneredDetected++; else corneredDetected--;
    //    if (corneredDetected <= 0)
    //        corneredDetected = 0;
    //    if (corneredDetected >= 2)
    //        killPlayer();
    //}
    #endregion

    #region Particle System
    void startPsystem() {
        Color aux = originalColor;
        aux += new Color(0.3f, 0.3f, 0.3f);
        explosion_psystem.startColor = aux;
    }

    void startEmission() {
        float scale = this.transform.localScale.x * 0.15f + 0.15f;
        explosion_psystem.gameObject.transform.localScale = new Vector3(scale, scale, scale); 
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        scamera.screenShake_(2f);
        explosion_psystem.gameObject.SetActive(true);
    }
    #endregion

    #region Portal
    bool inPortalCooldown = false;
    void teleportTo(GameObject portal) {
        if (inPortalCooldown) return;
        Vector3 position = portal.GetComponent<Portal>().nextPortal().transform.position;
        this.transform.position = position;
        StartCoroutine(portalCooldown());
    }

    IEnumerator portalCooldown() {
        inPortalCooldown = true;
        yield return new WaitForSeconds(1.0f);
        inPortalCooldown = false;
    }
    #endregion

    #region Items
    void getItem(Item item) {
        currentItem = item.type;
        playerStatus.showItem(item);
        item.destroy();
    }

    void useItem(Item.Type itemType) {
        if (itemType == Item.Type.NONE) return;
        playerStatus.unshowItem();
        currentItem = Item.Type.NONE;

        switch (itemType) {
            case Item.Type.HERBALIFE:
                useHerbalife();
                break;
            case Item.Type.TRIANGLE:
                StartCoroutine(useTrianglePotion());
                break;
        }
    }

    void useHerbalife() {
        transform.localScale = originalScale;
    }

    IEnumerator useTrianglePotion() {
        isTriangle = true;
        changeSprite(trianglePlayer);
        circleCollider.SetActive(false);
        triangleCollider.SetActive(true);

        yield return new WaitForSeconds(5.0f);

        isTriangle = false;
        changeSprite(circlePlayer);
        circleCollider.SetActive(true);
        triangleCollider.SetActive(false);
    }
    #endregion
}
