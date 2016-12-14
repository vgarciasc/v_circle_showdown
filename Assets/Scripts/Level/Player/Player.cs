using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, ISmashable {
    /*Reference to objects in scene*/
    [Header("Prefabs and References")]
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
    Sprite circleBorderAlmostExploding;
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
    
    public GameObject cannonPosition;

    public PlayerData data;

    public Color color;
    public bool isTriangle;
    public delegate void UIDelegate();
    public event UIDelegate death_event;
    public delegate void ItemDelegate(ItemData item_data);
    public event ItemDelegate get_item_event, 
                            use_item_event;    

    Rigidbody2D rb;
    Animator anim;
    SpecialCamera scamera;
    PolygonCollider2D triangleCollider;
    CircleCollider2D circleCollider;

    /*Reset variables*/
    Color border_color;

    /*Joystick Input*/
    string jsHorizontal,
        jsVertical,
        jsFire1,
        jsFire2,
        jsFire3,
        jsJump;

    /*Other*/
    [Header("Other")]
    public int ID = -1;
    public string joystick;
    ItemData currentItem;
    public float tackleBuildup;
    Vector3 lastVelocity;
    bool invincible = false,
        ghostSwallow = false,
        almostExploding = false;

    /*Blockers*/
    bool blockGrowth = false,
        blockCharge = false,
        blockInput = false;

    public void setPlayer(PlayerInstance instance) {
        this.ID = instance.playerID;
        this.joystick = instance.joystick;
        this.color = instance.color;
        startColors();
    }

    void Start() {
        /*References*/
        rb = GetComponentInChildren<Rigidbody2D>();
        anim = GetComponent<Animator>();
        scamera = Camera.main.GetComponent<SpecialCamera>();
        triangleCollider = GetComponent<PolygonCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        /*Init functions*/
        startPsystem();
        startAnimator();
        createJoystickInput();
        changeSprite(circleBorder, circleBackground);
        toggleTriangleDetection(false);
        resetTackle();
        StartCoroutine(handleAlmostMaxSize());
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
    }

    #region Spritefest
    void changeSprite(Sprite border, Sprite background) {
        GetComponent<SpriteRenderer>().sprite = border;
        spriteBackground.GetComponent<SpriteRenderer>().sprite = background;
    }

    void changeBorderColor(Color cr) {
        GetComponent<SpriteRenderer>().color = cr;
    }
    #endregion

    public void timeOut() { killPlayer(); }

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

        float h_mov = Input.GetAxis(jsHorizontal) * data.speed;
        if (Mathf.Abs(rb.velocity.x) < data.maxVelocity)
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
        rb.AddForce(new Vector2(0, data.jumpForce));
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
            hitCharger(target.gameObject.GetComponent<Rigidbody2D>().velocity);

        if (target.gameObject.tag == "Player" && isLookingAtObject(target.transform)) {
            Player enemy = target.gameObject.GetComponent<Player>();
            if (enemy.isInvincible()) return;

            float hitStrength = velocityHitMagnitude();
            shakeScreen(hitStrength);
            
            this.giveHit(data.hitSizeIncrement + hitStrength);
            enemy.takeHit(data.hitSizeIncrement + hitStrength);
        }

        else if (ghostSwallow && target.gameObject.tag == "Player") {
            swallowPlayer(target.gameObject.GetComponent<Player>());
        }
    }

    public void OnTriggerEnter2D(Collider2D target) {
        switch (target.gameObject.tag) {
            case "Item":
                getItem(target.gameObject.GetComponent<Item>());
                break;
            case "Spikes":
                hitSpikes();
                break;
        }
    }

    public void OnTriggerStay2D(Collider2D target) {
        switch (target.gameObject.tag) {
            case "Item":
                getItem(target.gameObject.GetComponent<Item>());
                break;
            case "Nebula":
                changeSize(0.005f + 0.01f * tackleBuildup / 100f);
                break;
            case "Inverse Nebula":
                changeSize(-0.005f - 0.01f * tackleBuildup / 100f);
                break;
        }
    }

    public bool isInvincible() { return invincible; }

    void shakeScreen(float hitStrength) { scamera.screenShake_(hitStrength); }

    public void takeHit(float transferSize) {
        //Debug.Log("Transfer Size: " + transferSize);
        changeSize(transferSize);
        StartCoroutine(temporaryInvincibility(data.invincibleFrames));
    }

    void giveHit(float transferSize) {
        //this.changeSize(- transferSize);
    }

    void hitSpikes() {
        killPlayer();
    }

    void hitCharger(Vector2 velocity) {
        float aux = velocity.magnitude;
        changeSize(0.5f * aux / 20f);
        shakeScreen(0.5f * aux / 20f);
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

        death_event();
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
        Destroy(this.gameObject);
    }
    #endregion

    #region Colors
    void startColors() {
        border_color = Color.black;
        spriteBackground.GetComponent<SpriteRenderer>().color = color;
        forceField.GetComponent<SpriteRenderer>().color = color;
        this.GetComponent<SpriteRenderer>().color = border_color;
        chargeIndicator.GetComponent<SpriteRenderer>().color = new Color(color.r - 0.4f, color.g - 0.4f, color.b - 0.4f, 0.5f);
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
            yield return new WaitForSeconds(data.timeBetweenSpurts);
            if (!blockGrowth) changeSize(data.timeSizeIncrement);
        }
    }

    void changeSize(float sizeIncrement) {
        this.transform.localScale += new Vector3(sizeIncrement, sizeIncrement);
        checkSize();
    }

    void checkSize() {
        //manageAlmostExploding();
        almostExploding = this.transform.localScale.x > data.maxSize * 4 / 5;

        if (this.transform.localScale.x > data.maxSize)
            killPlayer();
        if (this.transform.localScale.x < data.minSize)
            /*this.transform.localScale = new Vector2(minSize, minSize);*/
            killPlayer();
    }

    IEnumerator handleAlmostMaxSize() {
        bool toggle = false;
        while (true) {
            changeBorderColor(border_color);
            yield return new WaitUntil(() => almostExploding);

            toggle = !toggle;
            if (toggle) changeBorderColor(border_color);
            else changeBorderColor(HushPuppy.getColorWithOpacity(border_color, 0.5f));

            for (int i = 0; i < 10; i++)
                    yield return new WaitForEndOfFrame();
        }
    }

    void manageAlmostExploding() {
        bool aux = this.transform.localScale.x > data.maxSize * 4 / 5;

        if (!isTriangle && aux != almostExploding) {
            if (aux) {
                changeSprite(circleBorderAlmostExploding, circleBackground);
            } else {
                changeSprite(circleBorder, circleBackground);
            }
        }

        almostExploding = aux;
    }
    #endregion

    #region Tackle Bell
    public void resetTackle() {
        tackleBuildup = 0f;
        rb.mass = data.mass;
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

        if (tackleBuildup >= data.maxTackleBuildup)
            tackleBuildup = data.maxTackleBuildup;

        float perc = tackleBuildup / data.maxTackleBuildup;
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

        rb.mass = data.mass + data.tackleWeight * perc;
    }

    void releaseTackle(float power) {
        float perc = tackleBuildup / data.maxTackleBuildup;
        Vector2 direction = this.transform.up * data.tackleForce * perc * power;
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
    //    //if (ID == 1) Debug.Log("corneredDetected[0]: " + corneredDetected[0]);
    //    //if (ID == 1) Debug.Log("corneredDetected[1]: " + corneredDetected[1]);
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
        Color aux = color;
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
        get_item_event(item.data);
        item.destroy();
    }

    void useItem(ItemData itemData) {
        if (itemData == null) return;

        use_item_event(itemData);
        currentItem = null;
    }

    void useHerbalife() {
        transform.localScale = data.scale;
    }

    public void toggleTriangle(bool value) {
        if (value)
            changeSprite(triangleBorder, triangleBackground);
        else
            changeSprite(circleBorder, circleBackground);

        isTriangle = !isTriangle;
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
        Coroutine cr = StartCoroutine(useGhostPotion_blink(color, transparent));
        yield return new WaitForSeconds(duration * 2 / 5);
        StopCoroutine(cr);

        spriteBackground.GetComponent<SpriteRenderer>().color = color;

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
    #endregion
}
