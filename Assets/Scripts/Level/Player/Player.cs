using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour, ISmashable {
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
    [SerializeField]
    float portalCooldown = 1.0f;

    /*Reference to objects in scene*/
    [Header("Prefabs and References")]
    [SerializeField]
    GameObject bombPrefab;
    [SerializeField]
    GameObject playerStatusPrefab;
    [SerializeField]
    GameObject playerMarkerPrefab;
    [SerializeField]
    GameObject playerVictoriesPrefab;
    [SerializeField]
    ParticleSystem explosionParticleSystem;
    [SerializeField]
    GameObject circleTrappedDetector;
    [SerializeField]
    GameObject triangleTrappedDetector;
    [SerializeField]
    GameObject triangleSpikes;
    [SerializeField]
    Sprite circleBorder;
    [SerializeField]
    Sprite circleBackground;
    [SerializeField]
    Sprite triangleBorder;
    [SerializeField]
    Sprite triangleBackground;
    [SerializeField]
    GameObject spriteBackground;
    [SerializeField]
    GameObject forceField;
    [SerializeField]
    GameObject chargeIndicator;
    [SerializeField]
    GameObject cannonPosition;

    GameObject playerStatusContainer;

    Subject subject = new Subject();
    Rigidbody2D rb;
    Animator anim;
    Color playerColor;
    PlayerUIStatus playerStatus;
    PlayerUIMarker playerMarker;
    SpecialCamera scamera;
    PolygonCollider2D triangleCollider;
    CircleCollider2D circleCollider;
    GameController gcontroller;

    /*Reset variables*/
    Color originalBorderColor;
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
    ItemData currentItem;
    float tackleBuildup;
    Vector3 lastVelocity;
    bool invincible = false,
        ghostSwallow = false;

    /*Blockers*/
    bool blockGrowth = false,
        blockCharge = false,
        blockInput = false;

    public void setPlayer(int playerID, string joystick, Color color) {
        this.playerID = playerID;
        this.joystick = joystick;
        playerColor = color;
    }

    void Start() {
        /*References*/
        rb = GetComponentInChildren<Rigidbody2D>();
        anim = GetComponent<Animator>();
        scamera = Camera.main.GetComponent<SpecialCamera>();
        triangleCollider = GetComponent<PolygonCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        gcontroller = (GameController)HushPuppy.safeFindComponent("GameController", "GameController");

        /*Default values*/
        originalMass = rb.mass;
        originalScale = transform.localScale;

        /*Init functions*/
        startColors();
        startUI();
        startPsystem();
        startAnimator();
        createJoystickInput();
        changeSprite(circleBorder, circleBackground);
        toggleTriangleDetection(false);
        resetTackle();
        StartCoroutine(checkOutOfScreen());
        StartCoroutine(grow());
    }

    void FixedUpdate() {
        manageTackle();
        updateLastVelocity();
    }

    void Update() {
        //DEBUG
        forceField.SetActive(false);

        handleInput();
        updateMarker();
    }

    #region Spritefest
    void changeSprite(Sprite border, Sprite background) {
        GetComponent<SpriteRenderer>().sprite = border;
        spriteBackground.GetComponent<SpriteRenderer>().sprite = background;
    }
    #endregion

    #region UI Elements
    void startUI() {
        GameObject playerUI_container = HushPuppy.safeFind("PlayerUIContainer");

        playerStatus = Instantiate(playerStatusPrefab).GetComponent<PlayerUIStatus>();
        playerStatus.name = "Player #" + (playerID + 1) + " Status";
        playerStatus.transform.SetParent(playerUI_container.transform.GetChild(0), false);
        playerStatus.setUI(playerID, this.playerColor);
        subject.addObserver(playerStatus);

        playerMarker = Instantiate(playerMarkerPrefab).GetComponent<PlayerUIMarker>();
        playerMarker.name = "Player #" + (playerID + 1) + " Marker";
        playerMarker.transform.SetParent(playerUI_container.transform.GetChild(1), false);
        playerMarker.setMarker(this.playerColor);
        subject.addObserver(playerMarker);
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
                playerStatus.setTime(timeLeft--);
            }

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
        if (Input.GetButtonUp(jsFire1) && Input.GetButton(jsJump))
            releaseTackle(1.5f);
        else if (Input.GetButtonUp(jsFire1))
            releaseTackle(1f);
        if (Input.GetButtonDown(jsFire2))
            useItem(currentItem);
    }

    void jump() {
        if (blockInput) return;
        rb.AddForce(new Vector2(0, jumpForce));
    }

    void move(float movement) {
        if (blockInput) return;
        rb.velocity += new Vector2(movement, 0);
    }
    #endregion

    #region Collision Treatment
    public void OnCollisionEnter2D(Collision2D target) {
        if (target.gameObject.tag == "Spikes")
            hitSpikes();

        if (target.gameObject.tag == "Charger")
            hitCharger();

        if (target.gameObject.tag == "Player" && isLookingAtObject(target.transform)) {
            Player enemy = target.gameObject.GetComponent<Player>();
            if (enemy.isInvincible()) return;

            float hitStrength = velocityHitMagnitude();
            shakeScreen(hitStrength);
            
            this.giveHit(hitSizeIncrement + hitStrength);
            enemy.takeHit(hitSizeIncrement + hitStrength);
        }

        else if (ghostSwallow && target.gameObject.tag == "Player") {
            swallowPlayer(target.gameObject.GetComponent<Player>());
        }
    }

    public void OnTriggerEnter2D(Collider2D target) {
        switch (target.gameObject.tag) {
            case "Spikes":
                hitSpikes();
                break;
            case "Portal":
                target.GetComponent<Portal>().teleport(this.gameObject);
                break;
            case "Item":
                getItem(target.gameObject.GetComponent<Item>());
                break;
        }
    }

    public void OnTriggerStay2D(Collider2D target) {
        if (target.gameObject.tag == "Nebula")
            changeSize(0.005f);

        if (target.gameObject.tag == "Inverse Nebula")
            changeSize(-0.005f);
    }

    public bool isInvincible() { return invincible; }

    void shakeScreen(float hitStrength) { scamera.screenShake_(hitStrength); }

    public void takeHit(float transferSize) {
        //Debug.Log("Transfer Size: " + transferSize);
        changeSize(transferSize);
        StartCoroutine(temporaryInvincibility(invincibleFrames));
    }

    void giveHit(float transferSize) {
        //this.changeSize(- transferSize);
    }

    void hitSpikes() {
        killPlayer();
    }

    void hitCharger() {
        changeSize(0.3f);
    }

    IEnumerator temporaryInvincibility(int frames) {
        invincible = true;
        for (int i = 0; i < frames; i++)
            yield return new WaitForEndOfFrame();
        invincible = false;
    }

    void updateLastVelocity() {
        lastVelocity.z = lastVelocity.y;
        lastVelocity.y = lastVelocity.x;
        lastVelocity.x = rb.velocity.magnitude;
    }

    float velocityHitMagnitude() {
        float aux = lastVelocity.x;
        if (lastVelocity.y > aux) aux = lastVelocity.y;
        if (lastVelocity.z > aux) aux = lastVelocity.z;

        return aux / 60;
    }

    void killPlayer() {
        anim.enabled = true;
        circleCollider.enabled = false;
        triangleCollider.enabled = false;

        subject.notify(Event.PLAYER_KILLED);
        anim.SetTrigger("explode");
    }

    void swallowPlayer(Player enemy) {
        changeSize(enemy.transform.localScale.x / 2f);
        enemy.swallowed();
    }

    void swallowed() {
        this.killPlayer();
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

    #region Colors
    void startColors() {
        originalBorderColor = Color.black;
        spriteBackground.GetComponent<SpriteRenderer>().color = playerColor;
        forceField.GetComponent<SpriteRenderer>().color = playerColor;
        this.GetComponent<SpriteRenderer>().color = originalBorderColor;
        chargeIndicator.GetComponent<SpriteRenderer>().color = new Color(playerColor.r - 0.4f, playerColor.g - 0.4f, playerColor.b - 0.4f, 0.5f);
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
            if (!blockGrowth) changeSize(timeSizeIncrement);
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
            /*this.transform.localScale = new Vector2(minSize, minSize);*/
            killPlayer();
    }

    #endregion

    #region Tackle Bell
    void resetTackle() {
        tackleBuildup = 0f;
        rb.mass = originalMass;
    }

    void toggleChargeIndicator(bool value) {
        chargeIndicator.SetActive(value);
    }

    void increaseTackleBuildup() {
        if (blockCharge) return;
        tackleBuildup += 1f;
    }

    float white = 0;
    bool whiteOut = false;

    void manageTackle() {
        if (blockCharge) return;

        if (tackleBuildup >= maxTackleBuildup)
            tackleBuildup = maxTackleBuildup;

        float perc = tackleBuildup / maxTackleBuildup;
        perc /= 1f;

        //if (originalColor.r >= originalColor.g && originalColor.r >= originalColor.b)
        //    this.GetComponent<SpriteRenderer>().color = new Color(originalColor.r, originalColor.g - perc, originalColor.b - perc, originalColor.a);
        //else if (originalColor.g >= originalColor.b && originalColor.g >= originalColor.r)
        //    this.GetComponent<SpriteRenderer>().color = new Color(originalColor.r - perc, originalColor.g, originalColor.b - perc, originalColor.a);
        //else if (originalColor.b >= originalColor.g && originalColor.b >= originalColor.r)
        //    this.GetComponent<SpriteRenderer>().color = new Color(originalColor.r - perc, originalColor.g - perc, originalColor.b);
        //else
        //    this.GetComponent<SpriteRenderer>().color = new Color(originalColor.r, originalColor.g - perc, originalColor.b - perc, originalColor.a);
        //this.GetComponent<SpriteRenderer>().color = new Color(originalColor.r - perc, originalColor.g - perc, originalColor.b - perc, originalColor.a);

        //Color aux = forceField.GetComponent<SpriteRenderer>().color;
        //forceField.GetComponent<SpriteRenderer>().color = new Color(aux.r, aux.g, aux.b, 0 + perc);

        float multiplier = Mathf.Sin(perc * Mathf.PI / 2);

        chargeIndicator.transform.localScale = new Vector3(multiplier, multiplier, multiplier);
        Color aux = chargeIndicator.GetComponent<SpriteRenderer>().color;
        chargeIndicator.GetComponent<SpriteRenderer>().color = new Color(aux.r, aux.g, aux.b, multiplier);

        rb.mass = originalMass + tackleWeight * perc;
    }

    void releaseTackle(float power) {
        float perc = tackleBuildup / maxTackleBuildup;
        Vector2 direction = this.transform.up * tackleForce * perc * power;
        rb.velocity += direction;
        //rb.velocity += new Vector2(Mathf.Sign(rb.velocity.x) * 10f, 0);
        resetTackle();
    }
    #endregion

    #region Cornered Detection
    //caso esse codigo gere erros no futuro, voce deve simplesmente alterar os colliders de detecçao de entalamento no prefab pra que eles
    //sejam menores. a precisao de detecçao de entalamento é sacrificada, mas pelo menos gera menos bugs
    //bool[] corneredDetected = new bool[2];
    //public void corneredDetection(int detectorID, bool value) {
    //    corneredDetected[detectorID] = value;
    //    //if (playerID == 1) Debug.Log("corneredDetected[0]: " + corneredDetected[0]);
    //    //if (playerID == 1) Debug.Log("corneredDetected[1]: " + corneredDetected[1]);
    //    if (corneredDetected[0] && corneredDetected[1]) {
    //        killPlayer();
    //    }
    //}

    void toggleTriangleDetection(bool value) {
        circleTrappedDetector.SetActive(!value);
        triangleTrappedDetector.SetActive(value);
    }

    void toggleCircleDetection(bool value) {
        toggleTriangleDetection(!value);
    }

    void toggleStuckDetection(bool value) {
        circleTrappedDetector.SetActive(value);
        triangleTrappedDetector.SetActive(value);
    }

    public void smashedDetected() {
        killPlayer();
    }
    #endregion

    #region Particle System
    void startPsystem() {
        Color aux = playerColor;
        aux += new Color(0.3f, 0.3f, 0.3f);
        explosionParticleSystem.startColor = aux;
    }

    void startEmission() {
        float scale = this.transform.localScale.x * 0.15f + 0.15f;
        explosionParticleSystem.gameObject.transform.localScale = new Vector3(scale, scale, scale);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        scamera.screenShake_(2f);
        explosionParticleSystem.gameObject.SetActive(true);
    }
    #endregion

    #region Item
    void getItem(Item item) {
        if (item.data.type == ItemType.BLACK_HOLE) {
            item.activateBlackHole();
            return;
        }

        currentItem = item.data;
        playerStatus.showItem(item);
        item.destroy();
    }

    void useItem(ItemData itemData) {
        playerStatus.unshowItem();
        currentItem = itemData;

        switch (itemData.type) {
            case ItemType.HERBALIFE:
                useHerbalife();
                break;
            case ItemType.TRIANGLE:
                StartCoroutine(useTrianglePotion(itemData.cooldown));
                break;
            case ItemType.BLACK_HOLE:
                break;
            case ItemType.GHOST:
                StartCoroutine(useGhostPotion(itemData.cooldown));
                break;
            case ItemType.BOMB:
                useBomb();
                break;
        }
    }

    void useHerbalife() {
        transform.localScale = originalScale;
    }

    void toggleTriangle(bool value) {
        if (value)
            changeSprite(triangleBorder, triangleBackground);
        else
            changeSprite(circleBorder, circleBackground);

        triangleSpikes.SetActive(value);
        blockInput = value;
        blockCharge = value;
        forceField.SetActive(!value);
        toggleChargeIndicator(!value);
        toggleTriangleDetection(value);
        circleCollider.enabled = !value;
        triangleCollider.enabled = value;
        resetTackle();
    }

    IEnumerator useTrianglePotion(float duration) {
        toggleTriangle(true);

        yield return new WaitForSeconds(duration);

        toggleTriangle(false);
    }

    IEnumerator useGhostPotion(float duration) {
        circleCollider.enabled = false;
        triangleCollider.enabled = false;
        toggleStuckDetection(false);
        toggleChargeIndicator(false);
        blockCharge = true;
        blockGrowth = true;
        resetTackle();

        Color transparent = HushPuppy.getColorWithOpacity(spriteBackground.GetComponent<SpriteRenderer>().color, 0.5f);
        spriteBackground.GetComponent<SpriteRenderer>().color = transparent;

        yield return new WaitForSeconds(duration * 3 / 5);
        Coroutine cr = StartCoroutine(useGhostPotion_blink(playerColor, transparent));
        yield return new WaitForSeconds(duration * 2 / 5);
        StopCoroutine(cr);

        spriteBackground.GetComponent<SpriteRenderer>().color = playerColor;

        circleCollider.enabled = true;
        toggleCircleDetection(true);
        toggleChargeIndicator(true);
        blockCharge = false;
        blockGrowth = false;

        StartCoroutine(ghostSwallowActivate());
    }

    IEnumerator ghostSwallowActivate() {
        ghostSwallow = true;

        for (int i = 0; i < 3; i++)
            yield return new WaitForEndOfFrame();

        ghostSwallow = false;
    }

    IEnumerator useGhostPotion_blink(Color original, Color transparent) {
        bool toggle = true;
        while (true) {
            toggle = !toggle;
            if (toggle) spriteBackground.GetComponent<SpriteRenderer>().color = transparent;
            else spriteBackground.GetComponent<SpriteRenderer>().color = original;
            for (int i = 0; i < 5; i++)
                yield return new WaitForEndOfFrame();
        }
    }

    void useBomb() {
        Bomb bomb = Instantiate(bombPrefab).GetComponent<Bomb>();
        bomb.transform.position = cannonPosition.transform.position;

        bomb.setBomb(this.transform.up, this.transform.localScale, tackleBuildup);
        resetTackle();
    }
    #endregion
}
