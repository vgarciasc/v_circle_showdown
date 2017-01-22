using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Player : MonoBehaviour, ISmashable {
    /*Reference to objects in scene*/
    [Header("Prefabs and References")]
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
    [SerializeField]
    GameObject explosionRing;

    public GameObject cannonPosition;
    [HideInInspector]
    public PlayerData data; //playerdata to be modified by other classes
    public PlayerData originalData;
    public PlayerInstance instance;
    public Color color;
    public string playername;
    public bool isTriangle;
    
    #region EVENTS
    public delegate void VoidDelegate();
    public event VoidDelegate death_event,
                            bomb_event,
                            victory_event;
    public delegate void VisibleDelegate(bool value);
    public event VisibleDelegate visible_event;
    public delegate void IdentifiedVoidDelegate(PlayerInstance instance);
    public event IdentifiedVoidDelegate id_death_event;
    public delegate void BombTriangleEvent(Vector3 bomb_position);
    public event BombTriangleEvent bomb_triangle_event;
    public delegate void ItemDelegate(ItemData item_data);
    public event ItemDelegate get_item_event, 
                            use_item_event;    
    public delegate void ChargeDelegate(int currentCharge);
    public event ChargeDelegate charge_event;
    #endregion

    Rigidbody2D rb;
    Animator anim;
    SpecialCamera scamera;
    PolygonCollider2D triangleCollider;
    CircleCollider2D circleCollider;
    TrailRenderer trenderer;

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
    public float chargeBuildup;
    Vector3 lastVelocity;
    bool invincible = false,
        is_dead = false,
        almostExploding = false;
    public bool is_on_ground = false,
                should_be_visible = true;

    /*Blockers*/
    bool blockGrowth = false,
        blockCharge = false,
        blockInput = false;

    public void setPlayer(PlayerInstance instance) {
        this.ID = instance.playerID;
        this.joystick = instance.joystick;
        this.color = instance.color;
        this.playername = instance.name;
        this.instance = instance;
        reset_colors();
    }

    void Start() {
        /*References*/
        rb = GetComponentInChildren<Rigidbody2D>();
        anim = GetComponent<Animator>();
        scamera = Camera.main.GetComponent<SpecialCamera>();
        triangleCollider = GetComponent<PolygonCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        trenderer = GetComponent<TrailRenderer>();

        /*Init functions*/
        createJoystickInput();
        changeSprite(circleBorder, circleBackground);
        toggleTriangleDetection(false);
        reset_charge();
        StartCoroutine(handleAlmostMaxSize());
        StartCoroutine(grow());
        initPlayerData();
    }

    void FixedUpdate() {
        //checkGround();
        manage_charge();
        updateLastVelocity();
    }

    void Update() {
        //DEBUG
        forceField.SetActive(false);

        handleInput();
    }

    void initPlayerData() {
        data = (PlayerData) Instantiate(originalData);
    }

    #region Spritefest
    void changeSprite(Sprite border, Sprite background) {
        explosionRing.GetComponent<SpriteRenderer>().sprite = border;
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
            reset_charge();
        if (Input.GetButton(jsFire1))
            increaseChargeBuildup();
        if (Input.GetButtonUp(jsFire1) && Input.GetButton(jsJump))
            release_charge(1.5f);
        else if (Input.GetButtonUp(jsFire1))
            release_charge(1f);
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

    public void toggle_block_all_input(bool value) {
        blockCharge = value;
        blockInput = value;
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
    }

    public void OnTriggerEnter2D(Collider2D target) {
        switch (target.gameObject.tag) {
            case "BombExplosion":
                if (isTriangle) bomb_triangle_event(target.gameObject.transform.position);
                //bomb_event();
                hitSpikes();
                break;
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
                changeSize(0.005f + 0.01f * chargeBuildup / 100f);
                break;
            case "Inverse Nebula":
                changeSize(-0.005f - 0.01f * chargeBuildup / 100f);
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

    void checkGround() {
        Vector2 start = new Vector2(transform.position.x,
                                    transform.position.y - transform.localScale.x / 2f);

        is_on_ground = Physics2D.Raycast(start,
                                        Vector3.down * 0.2f,
                                        0.2f,
                                        1 << LayerMask.NameToLayer("CommonTerrain"));
    }

    void updateLastVelocity() {
        lastVelocity.z = lastVelocity.y;
        lastVelocity.y = lastVelocity.x;
        lastVelocity.x = rb.velocity.magnitude;
    }

    public float velocityHitMagnitude() {
        float aux = lastVelocity.x;
        if (lastVelocity.y > aux) aux = lastVelocity.y;
        if (lastVelocity.z > aux) aux = lastVelocity.z;

        return aux / 60;
    }

    void killPlayer() {
        if (is_dead) return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 2) {
            //this will be the last player to die
            StartCoroutine(slowmo(2.0f));
        }
        
        anim.enabled = true;
        circleCollider.enabled = false;
        triangleCollider.enabled = false;
        scamera.screenShake_(2f);
        is_dead = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (death_event != null) death_event();
        if (id_death_event != null) id_death_event(instance);
        anim.SetTrigger("explode");
    }

    static IEnumerator slowmo(float duration) {
        float timescale = 1f;
        float slow = 0.2f;

        Time.timeScale = slow;
        yield return new WaitForSeconds(duration * slow);

        int aux = 20;
        for (int i = 0; i < aux; i++) {
            if (Time.timeScale < timescale) {
                Time.timeScale += (timescale - slow) / aux;
            }
            yield return new WaitForEndOfFrame();
        }
        
        Time.timeScale = timescale;
    }

    bool isLookingAtObject(Transform target) {
        float angle = 50f;
        float angleBetweenPlayers = Mathf.Abs(Vector3.Angle(this.transform.up, transform.position - target.position) - 180f);
        return (angleBetweenPlayers < angle);
    }
    #endregion

    #region Animation
    //to be used only by animation
    void AnimationKillPlayer() {
        Destroy(this.gameObject);
    }
    #endregion

    #region Colors
    void reset_colors() {
        border_color = Color.black;
        spriteBackground.GetComponent<SpriteRenderer>().color = color;
        forceField.GetComponent<SpriteRenderer>().color = color;
        this.GetComponent<SpriteRenderer>().color = border_color;
        reset_charge_indicator();    
    }

    void reset_charge_indicator() {
        chargeIndicator.GetComponent<SpriteRenderer>().color = new Color(color.r - 0.4f, color.g - 0.4f, color.b - 0.4f, 0.5f);
    }

    public void toggle_visibility(bool value) {
        if (visible_event != null) {
            visible_event(value);
        }

        should_be_visible = value;
        this.GetComponent<SpriteRenderer>().enabled = value;
        spriteBackground.GetComponent<SpriteRenderer>().enabled = value;
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
            yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(data.timeBetweenSpurts);
            if (!blockGrowth) changeSize(data.timeSizeIncrement);
        }
    }

    public void changeSize(float sizeIncrement) {
        this.transform.localScale += new Vector3(sizeIncrement, sizeIncrement, sizeIncrement);
        checkSize();
    }

    void checkSize() {
        //manageAlmostExploding();
        almostExploding = this.transform.localScale.x > data.maxSize * 4 / 5;

        if (this.transform.localScale.x > data.maxSize)
            killPlayer();
        if (this.transform.localScale.x < data.minSize) {
            /*this.transform.localScale = new Vector2(minSize, minSize);*/
            Debug.Log(playername + " is too small.");
            // killPlayer();
        }
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

    #region Charge Bell
    public void reset_charge() {
        chargeBuildup = 0f;
        rb.mass = data.mass;
    }

    void toggleChargeIndicator(bool value) {
        chargeIndicator.SetActive(value);
    }

    void increaseChargeBuildup() {
        if (blockCharge) return;
        chargeBuildup += 1f;
    }

    float white = 0;
    bool whiteOut = false;

    Coroutine pulse;
    void manage_charge() {
        if (charge_event != null) {
            charge_event((int) chargeBuildup);
        }
        
        if (blockCharge) return;

        if (chargeBuildup >= data.maxChargeBuildup) {
            chargeBuildup = data.maxChargeBuildup;
            if (pulse == null) {
                //pulse = StartCoroutine(pulsate_charge());
            }
            return;
        }
        else {
            if (pulse != null) {
                reset_charge_indicator();
                StopCoroutine(pulse);
                pulse = null;
            }
        }

        float perc = chargeBuildup / data.maxChargeBuildup;
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

        rb.mass = data.mass + data.chargeWeight * perc;
    }

    IEnumerator pulsate_charge() {
        Vector3 initial = chargeIndicator.transform.localScale;
        Color initial_color = chargeIndicator.GetComponent<SpriteRenderer>().color;
        int i = 0;
        while (true) {
            Vector3 vec = initial;

            float multiplier = (- Mathf.Cos((i/20f * Mathf.PI) / 2) + 1) / 8f;
            Color col = Color.Lerp(initial_color,
                                    Color.white, 
                                    (- Mathf.Cos((i/5f * Mathf.PI) / 2) + 1) / 4f);

            vec += new Vector3(multiplier, multiplier, multiplier);

            chargeIndicator.GetComponent<SpriteRenderer>().color = col;
            chargeIndicator.transform.localScale = vec;
            i++;
            yield return new WaitForEndOfFrame();
        }
    }

    void release_charge(float power) {
        float perc = chargeBuildup / data.maxChargeBuildup;
        Vector2 direction = this.transform.up * data.chargeForce * perc * power;
        rb.velocity += direction;
        //rb.velocity += new Vector2(Mathf.Sign(rb.velocity.x) * 10f, 0);
        reset_charge();
    }
    #endregion

    #region Pause
    bool saveBlockInput, saveBlockCharge, saveBlockGrowth;

    public void OnPause() {
        saveBlockInput = blockInput;
        saveBlockCharge = blockCharge;        
        saveBlockGrowth = blockGrowth;
        
        blockInput = true;
        blockCharge = true;
        blockGrowth = true;
    }

    public void OnUnPause() {
        blockInput = saveBlockInput;
        blockCharge = saveBlockCharge;
        blockGrowth = saveBlockGrowth;
        
        if (Input.GetButtonUp(jsFire1))
            release_charge(1f);
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
        trenderer.enabled = !value;
        triangleCollider.enabled = value;
        reset_charge();
    }

    public void toggle_colliders(bool value) {
        if (isTriangle && value == true) {
            circleCollider.enabled = false;
            triangleCollider.enabled = true;
        }
        else if (!isTriangle && value == true) {
            circleCollider.enabled = true;
            triangleCollider.enabled = false;
        }
        else if (value == false) { // value == false
            circleCollider.enabled = false;
            triangleCollider.enabled = false;
        }
        
        toggleStuckDetection(value);
        toggleChargeIndicator(value);
        blockCharge = !value;
        blockGrowth = !value;
        reset_charge();
    }

    public IEnumerator start_blink() {
        Color original = color;
        Color original_border = this.GetComponent<SpriteRenderer>().color;
        Color transparent = HushPuppy.getColorWithOpacity(color, 0.5f);
        Color border_transparent = HushPuppy.getColorWithOpacity(original_border, 0.5f);

        bool toggle = true;
        while (true) {
            toggle = !toggle;
            if (toggle) { 
                spriteBackground.GetComponent<SpriteRenderer>().color = transparent;
                this.GetComponent<SpriteRenderer>().color = border_transparent; 
            }
            else {
                spriteBackground.GetComponent<SpriteRenderer>().color = original;
                this.GetComponent<SpriteRenderer>().color = original_border;
            }
            for (int i = 0; i < 5; i++)
                yield return new WaitForEndOfFrame();
        }
    }

    public void end_blink() {
        reset_colors();
    }
    #endregion

    #region victory_event
    public void get_victory() {
        if (victory_event != null) {
            victory_event();
        }
    }
    #endregion
}
