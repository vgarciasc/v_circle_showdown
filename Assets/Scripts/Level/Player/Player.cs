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
    Sprite circleBorderSprite;
    [SerializeField]
    GameObject circleBorder;
    [SerializeField]
    Sprite circleBorderAlmostExploding;
    [SerializeField]
    Sprite circleBackground;
    [SerializeField]
    Sprite triangleBorderSprite;
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
    [SerializeField]
    GameObject aimbotTarget;

    public GameObject cannonPosition;
    [HideInInspector]
    public PlayerData data; //playerdata to be modified by other classes
    public PlayerData originalData;
    public PlayerInstance instance;
    public PlayerColor palette;
    public string playername;
    public bool isTriangle;
    
    #region EVENTS
    public delegate void VoidDelegate();
    public event VoidDelegate death_event,
                            bomb_event,
                            victory_event,
                            jump_event;
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

    /*Reset variables*/
    Color border_color;

    /*Joystick Input*/
    string jsHorizontal,
        jsHorizontalRight,
        jsVertical,
        jsVerticalRight,
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
                should_be_visible = true,
                aimbot_active = false;

    /*Blockers*/
    bool blockGrowth = false,
        blockCharge = false,
        blockInput = false;

    public void setPlayer(PlayerInstance instance) {
        this.ID = instance.playerID;
        this.joystick = instance.joystick;
        this.palette = instance.palette;
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

        /*Init functions*/
        createJoystickInput();
        changeSprite(circleBorderSprite, circleBackground);
        toggleTriangleDetection(false);
        reset_charge();
        StartCoroutine(handleAlmostMaxSize());
        StartCoroutine(grow());
        initPlayerData();
        initDelegates();
    }

    void FixedUpdate() {
        //checkGround();
        manage_charge();
        updateLastVelocity();
    }

    void Update() {
        //DEBUG
        forceField.SetActive(false);
        aimbot();

        handleInput();
    }

    void initPlayerData() {
        data = (PlayerData) Instantiate(originalData);
    }

    void initDelegates() {
        this.GetComponent<PlayerItemUser>().double_coffee += killPlayer;
    }

    #region Spritefest
    void changeSprite(Sprite border, Sprite background) {
        // explosionRing.GetComponent<SpriteRenderer>().sprite = border;
        circleBorder.GetComponent<SpriteRenderer>().sprite = border;
        spriteBackground.GetComponent<SpriteRenderer>().sprite = background;
    }

    void changeBorderColor(Color cr) {
        circleBorder.GetComponent<SpriteRenderer>().color = cr;
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
        jsHorizontalRight = "Horizontal2" + joystick;
        jsVertical = "Vertical" + joystick;
        jsVerticalRight = "Vertical2" + joystick;
    }

    bool alternate_input = false;

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

        if (Input.GetButtonDown(jsFire3)) {
            this.rb.angularDrag = 2f;
        }

        if (Input.GetKeyDown(KeyCode.Equals)) {
            alternate_input = !alternate_input;
        }

        if (alternate_input) {
            float h_mov2, v_mov2;
            h_mov2 = Input.GetAxis(jsHorizontalRight);
            v_mov2 = Input.GetAxis(jsVerticalRight);

            if (joystick == "_J0") {
                h_mov2 = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position).x;
                v_mov2 = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position).y;            
            }

            if (joystick == "_J0") {
                if (Input.GetButton("Fire1")) {
                    increaseChargeBuildup();
                }
                if (Input.GetButtonUp("Fire1")) {
                    release_charge(1f);
                }
                if (Input.GetButton("Fire2")) {
                    useItem(currentItem);
                }
            }

            if (Mathf.Abs(h_mov2 + v_mov2) > 0) {
                this.transform.up = new Vector2(h_mov2, v_mov2);
                rb.angularVelocity = 0f;
            }
        }
    }

    void jump() {
        if (blockInput) return;
        rb.AddForce(new Vector2(0, data.jumpForce));

        if (jump_event != null) {
            jump_event();
        }
    }

    void move(float movement) {
        if (blockInput) return;
        rb.velocity += new Vector2(movement * Mathf.Pow((this.transform.localScale.x / data.scale.x), 0.4f), 0);
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
                if (target.gameObject.GetComponentInParent<MushroomCloud>() == null ||
                    target.gameObject.GetComponentInParent<MushroomCloud>().playerID != ID) {
                    changeSize(0.005f + 0.01f * chargeBuildup / 100f);
                }
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
        shakeScreen(transferSize);
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

        end_receiving_aimbot();
        foreach (CircleCollider2D c in this.GetComponentsInChildren<CircleCollider2D>()) {
            c.enabled = false;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (/*players.Length == 2*/ true) {
            //this will be the last player to die
            StartCoroutine(slowmo(3.0f));
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
        float slow = 0.1f;

        Time.timeScale = slow;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        yield return new WaitForSeconds(duration * slow);

        int aux = 20;
        for (int i = 0; i < aux; i++) {
            if (Time.timeScale < timescale) {
                Time.timeScale += (timescale - slow) / aux;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
            }
            yield return new WaitForEndOfFrame();
        }
        
        Time.timeScale = timescale;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
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
        spriteBackground.GetComponent<SpriteRenderer>().color = palette.color;
        forceField.GetComponent<SpriteRenderer>().color = palette.color;
        circleBorder.GetComponent<SpriteRenderer>().color = border_color;
        reset_charge_indicator();    
    }

    void reset_charge_indicator() {
        chargeIndicator.GetComponent<SpriteRenderer>().color = new Color(palette.color.r - 0.4f,
            palette.color.g - 0.4f,
            palette.color.b - 0.4f, 0.5f);
    }

    public void toggle_visibility(bool value) {
        if (visible_event != null) {
            visible_event(value);
        }

        should_be_visible = value;
        circleBorder.GetComponent<SpriteRenderer>().enabled = value;
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

            toggle = !toggle;
            if (almostExploding) {
                if (toggle) {
                    changeBorderColor(border_color);
                }
                else {
                    changeBorderColor(new Color(palette.color.r - 0.7f,
                        palette.color.g - 0.7f,
                        palette.color.b - 0.7f));
                }

                int framesToWait = (int) ((data.maxSize - transform.localScale.x) * 10);
                framesToWait = Mathf.Clamp(framesToWait, 5, framesToWait);
                yield return HushPuppy.WaitForEndOfFrames(framesToWait);
            }
            else {
                yield return HushPuppy.WaitForEndOfFrames(10);
            }
        }
    }

    void manageAlmostExploding() {
        bool aux = this.transform.localScale.x > data.maxSize * 4 / 5;

        if (!isTriangle && aux != almostExploding) {
            if (aux) {
                changeSprite(circleBorderAlmostExploding, circleBackground);
            } else {
                changeSprite(circleBorderSprite, circleBackground);
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
                pulse = StartCoroutine(pulsate_charge());
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
            // Vector3 vec = initial;

            // float multiplier = (- Mathf.Cos((i/20f * Mathf.PI) / 2) + 1) / 8f;
            Color col = Color.Lerp(initial_color,
                                    new Color(initial_color.r, initial_color.g, initial_color.b, 0.3f), 
                                    (- Mathf.Cos((i/5f * Mathf.PI) / 2) + 1) / 4f);

            // vec += new Vector3(multiplier, multiplier, multiplier);

            chargeIndicator.GetComponent<SpriteRenderer>().color = col;
            // chargeIndicator.transform.localScale = vec;
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
        if (item.data == null) {
            return;
        }

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
            changeSprite(triangleBorderSprite, triangleBackground);
        else
            changeSprite(circleBorderSprite, circleBackground);

        isTriangle = !isTriangle;
        triangleSpikes.SetActive(value);
        blockInput = value;
        blockCharge = value;
        forceField.SetActive(!value);
        toggleChargeIndicator(!value);
        toggleTriangleDetection(value);
        circleCollider.enabled = !value;
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

    public IEnumerator start_blink(float timeOut) {
        Color original = palette.color;
        Color original_border = circleBorder.GetComponent<SpriteRenderer>().color;
        Color transparent = HushPuppy.getColorWithOpacity(palette.color, 0.5f);
        // Color border_transparent = HushPuppy.getColorWithOpacity(original_border, 0.3f);
        Color border_transparent = new Color(palette.color.r - 0.5f,
            palette.color.g - 0.5f,
            palette.color.b - 0.5f);

        bool toggle = true;
        while (true) {
            toggle = !toggle;
            if (toggle) { 
                // spriteBackground.GetComponent<SpriteRenderer>().color = transparent;
                circleBorder.GetComponent<SpriteRenderer>().color = border_transparent; 
            }
            else {
                // spriteBackground.GetComponent<SpriteRenderer>().color = original;
                circleBorder.GetComponent<SpriteRenderer>().color = original_border;
            }
            
            // float timeToWait = ((timeOut - Time.time)) / 60f;
            // Debug.Log(timeToWait);
            // yield return new WaitForSeconds(timeToWait);

            int framesToWait = (int) (Mathf.Pow((timeOut - Time.time) * 2f, 1.7f));
            framesToWait = (int) Mathf.Clamp(framesToWait, 3f, framesToWait);
            
            yield return HushPuppy.WaitForEndOfFrames(framesToWait);
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

    #region aimbot

    float aimbot_startTimestamp = 0f;
    float aimbot_duration = 0f;
    bool is_receiving_aimbot = false;
    Coroutine aimbot_blinking = null;

    public void start_emitting_aimbot(float duration) {
        if (aimbot_active) {
            GameObject[] players = Player.getAllPlayers();
            for (int i = 0; i < players.Length; i++) {
                players[i].GetComponent<Player>().end_receiving_aimbot();
            }
        }

        aimbot_duration = duration;
        aimbot_startTimestamp = Time.time;
        aimbot_active = true;
    }
    
    public void end_emitting_aimbot() {
        aimbot_active = false;
    }

    void start_receiving_aimbot(Player targeter, float startTimestamp, float duration) {
        if (is_receiving_aimbot || is_dead) {
            return;
        }

        is_receiving_aimbot = true;
        
        aimbotTarget.transform.rotation = Quaternion.identity;
        aimbotTarget.GetComponent<SpriteRenderer>().color = targeter.instance.palette.color;
        
        aimbot_blinking = StartCoroutine(start_blinking_aimbot(startTimestamp, duration));
        aimbotTarget.SetActive(true);
    }

    public void end_receiving_aimbot() {
        is_receiving_aimbot = false;
        aimbotTarget.SetActive(false);

        if (aimbot_blinking != null) {
            StopCoroutine(aimbot_blinking);
            aimbot_blinking = null;
        }
    }

    IEnumerator start_blinking_aimbot(float startTime, float duration) {
        bool toggle = false;
        
        while (true) {
            bool blinking = (Time.time - startTime) > ((1f/2f) * duration);

            if (blinking) {
                toggle = !toggle;
                aimbotTarget.SetActive(toggle);
                int framesToWait = (int) (2 * ((startTime + duration) - Time.time));
                framesToWait = Mathf.Clamp(framesToWait, 3, framesToWait);

                yield return HushPuppy.WaitForEndOfFrames(framesToWait);
            }
            else {
                yield return HushPuppy.WaitForEndOfFrames(10);
            }
        }
    }

    void aimbot() {
        if (!aimbot_active) return;
        int closest_player_index = -1;
        Vector2 closest_player_distance = new Vector2(100, 100);

        GameObject[] players = Player.getAllPlayers();
        if (players.Length <= 1) return;

        for (int i = 0; i < players.Length; i++) {
            Vector2 aux = players[i].transform.position - this.transform.position;
            if (aux.magnitude < closest_player_distance.magnitude &&
                aux.magnitude != 0) { //careful, can target itself
                closest_player_distance = aux;
                closest_player_index = i;
            }
        }

        players[closest_player_index].GetComponent<Player>().start_receiving_aimbot(this,
            aimbot_startTimestamp,
            aimbot_duration);

        for (int i = 0; i < players.Length; i++) {
            if (i != closest_player_index) {
                players[i].GetComponent<Player>().end_receiving_aimbot();
            }
        }

        this.transform.up = closest_player_distance;
    }
    #endregion

    public static GameObject[] getAllPlayers() {
        return GameObject.FindGameObjectsWithTag("Player");
    }
}
