using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;

/// <summary>
/// Handle everything related to controlling the character. Interact with both the Character (visual, animation) and CharacterCollider
/// </summary>
public class CharacterInputController : MonoBehaviour
{
    static int s_DeadHash = Animator.StringToHash("Dead");
    static int s_RunStartHash = Animator.StringToHash("runStart");
    static int s_MovingHash = Animator.StringToHash("Moving");
    static int s_JumpingHash = Animator.StringToHash("Jumping");
    static int s_JumpingSpeedHash = Animator.StringToHash("JumpSpeed");
    static int s_SlidingHash = Animator.StringToHash("Sliding");

    [Space(20)]
    [Header("------------------Public Refrences------------------")]
    public TrackManager trackManager;

    public Character character;
    public CharacterCollider characterCollider;
    public GameObject blobShadow;
    public float laneChangeSpeed = 1.0f;


    [Space(20)]
    [Header("------------------Assets List------------------")]
    [SerializeField] internal List<Consumable> magnetList = new List<Consumable>();
    [SerializeField] internal List<Consumable> extraLifeList = new List<Consumable>();
    [SerializeField] internal List<Consumable> invinciblityList = new List<Consumable>();
    [SerializeField] internal List<Consumable> scoreMultiplierList = new List<Consumable>();


    [Space(20)]
    [Header("------------------Coin Purchase Assets List------------------")]
    [SerializeField]
    internal List<Consumable> c_MagnetList = new List<Consumable>();
    [SerializeField] internal List<Consumable> c_ExtraLifeList = new List<Consumable>();
    [SerializeField] internal List<Consumable> c_InvinciblityList = new List<Consumable>();
    [SerializeField] internal List<Consumable> c_ScoreMultiplierList = new List<Consumable>();


    [Space(20)]
    [Header("------------------Asset use Buttons------------------")]
    [SerializeField]
    internal Button magnetPowerUp;

    [SerializeField] internal Button extraLifePowerUp;
    [SerializeField] internal Button scoreMultiplierPowerup;
    [SerializeField] internal Button invinciblityPowerUp;


    [Space(20)]
    [Header("------------------Asset Count Texts------------------")]
    [SerializeField]
    TMP_Text magCount;

    [SerializeField] TMP_Text ExtraLifeCount;
    [SerializeField] TMP_Text ScoreMultiCount;
    [SerializeField] TMP_Text InvincibleCount;

    [Space(20)]
    [Header("------------------Max Life Count------------------")]
    public int maxLife = 3;


    [Space(20)]
    [Header("------------------Current Consumable using------------------")]
    public Consumable inventory;

    public int coins
    {
        get { return m_Coins; }
        set { m_Coins = value; }
    }

    public int premium
    {
        get { return m_Premium; }
        set { m_Premium = value; }
    }

    public int currentLife
    {
        get { return m_CurrentLife; }
        set { m_CurrentLife = value; }
    }

    public List<Consumable> consumables
    {
        get { return m_ActiveConsumables; }
    }

    public bool isJumping
    {
        get { return m_Jumping; }
    }

    public bool isSliding
    {
        get { return m_Sliding; }
    }

    [Space(20)]
    [Header("------------------ Controls------------------")]
    public float jumpLength = 2.0f; // Distance jumped

    public float jumpHeight = 1.2f;
    public float slideLength = 2.0f;

    [Space(20)]
    [Header("------------------Sound------------------")]
    public AudioClip slideSound;

    public AudioClip powerUpUseSound;
    public AudioSource powerupSource;

    [HideInInspector] public int currentTutorialLevel;
    [HideInInspector] public bool tutorialWaitingForValidation;

    protected int m_Coins;
    protected int m_Premium;
    protected int m_CurrentLife;

    [Space(20)]
    [Header("------------------Active Consumeable------------------")]
    [SerializeField]
    protected List<Consumable> m_ActiveConsumables = new List<Consumable>();

    protected int m_ObstacleLayer;

    protected bool m_IsInvincible;
    protected bool m_IsRunning;

    protected float m_JumpStart;
    protected bool m_Jumping;

    protected bool m_Sliding;
    protected float m_SlideStart;

    protected AudioSource m_Audio;

    protected int m_CurrentLane = k_StartingLane;
    protected Vector3 m_TargetPosition = Vector3.zero;

    protected readonly Vector3 k_StartingPosition = Vector3.forward * 2f;

    protected const int k_StartingLane = 1;
    protected const float k_GroundingSpeed = 80f;
    protected const float k_ShadowRaycastDistance = 100f;
    protected const float k_ShadowGroundOffset = 0.01f;
    protected const float k_TrackSpeedToJumpAnimSpeedRatio = 0.6f;
    protected const float k_TrackSpeedToSlideAnimSpeedRatio = 0.9f;

    private void OnEnable()
    {
        ShopItemList.onCoinShop += GetCoinPurchasedInventories;
    }


    private void OnDisable()
    {
        ShopItemList.onCoinShop -= GetCoinPurchasedInventories;
    }

    private void Start()
    {
        InitButton();
        GetCoinPurchasedInventories();
    }

    void GetCoinPurchasedInventories()
    {
        c_InvinciblityList.Clear();
        c_MagnetList.Clear();
        c_ExtraLifeList.Clear();
        c_ScoreMultiplierList.Clear();

        foreach (var item in PlayerData.instance.consumables)
        {
            Consumable c = ConsumableDatabase.GetConsumbale(item.Key);
            for (int i = 0; i < item.Value; i++)
            {
                switch (c.GetConsumableType())
                {
                    case Consumable.ConsumableType.COIN_MAG:
                        c_MagnetList.Add(c);
                        break;
                    case Consumable.ConsumableType.EXTRALIFE:
                        c_ExtraLifeList.Add(c);
                        break;
                    case Consumable.ConsumableType.INVINCIBILITY:
                        c_InvinciblityList.Add(c);
                        break;
                    case Consumable.ConsumableType.SCORE_MULTIPLAYER:
                        c_ScoreMultiplierList.Add(c);
                        break;
                }
            }
        }

        magCount.text = c_MagnetList.Count.ToString();
        ExtraLifeCount.text = c_ExtraLifeList.Count.ToString();
        InvincibleCount.text = c_InvinciblityList.Count.ToString();
        ScoreMultiCount.text = c_ScoreMultiplierList.Count.ToString();
    }
    void InitButton()
    {
        magnetPowerUp.onClick.AddListener(delegate
        {
            if (c_MagnetList.Count > 0)
            {
                var c_mag = Instantiate(c_MagnetList[0], transform);
                UseConsumable(c_mag.GetComponent<Consumable>());
                PlayerData.instance.Consume(c_mag.GetComponent<Consumable>().GetConsumableType());
                c_MagnetList.RemoveAt(0);
            }

            magCount.text = c_MagnetList.Count.ToString();
        });

        extraLifePowerUp.onClick.AddListener(delegate
        {
            if (c_ExtraLifeList.Count > 0)
            {
                var c_extraLifeList = Instantiate(c_ExtraLifeList[0], transform);
                UseConsumable(c_extraLifeList.GetComponent<Consumable>());
                PlayerData.instance.Consume(c_extraLifeList.GetComponent<Consumable>().GetConsumableType());
                c_ExtraLifeList.RemoveAt(0);
            }

            ExtraLifeCount.text = c_ExtraLifeList.Count.ToString();
        });

        scoreMultiplierPowerup.onClick.AddListener(delegate
        {
            if (c_ScoreMultiplierList.Count > 0)
            {
                var c_scoreMulti = Instantiate(c_ScoreMultiplierList[0], transform);
                UseConsumable(c_scoreMulti.GetComponent<Consumable>());
                PlayerData.instance.Consume(c_scoreMulti.GetComponent<Consumable>().GetConsumableType());
                c_ScoreMultiplierList.RemoveAt(0);
            }

            ScoreMultiCount.text = c_ScoreMultiplierList.Count.ToString();
        });

        invinciblityPowerUp.onClick.AddListener(delegate
        {
            if (c_InvinciblityList.Count > 0)
            {
                var c_invinciblity = Instantiate(c_InvinciblityList[0], transform);
                UseConsumable(c_invinciblity.GetComponent<Consumable>());
                PlayerData.instance.Consume(c_invinciblity.GetComponent<Consumable>().GetConsumableType());
                c_InvinciblityList.RemoveAt(0);
            }

            InvincibleCount.text = c_InvinciblityList.Count.ToString();
        });
    }


    private void FixedUpdate()
    {
        if (c_MagnetList.Count != 0 && m_ActiveConsumables.Count == 0 && PlayerData.instance.tutorialDone == true)
        {
            magnetPowerUp.interactable = true;
        }
        else
        {
            magnetPowerUp.interactable = false;
        }

        if (c_ExtraLifeList.Count != 0 && m_ActiveConsumables.Count == 0 && PlayerData.instance.tutorialDone == true)
        {
            extraLifePowerUp.interactable = true;
        }
        else
        {
            extraLifePowerUp.interactable = false;
        }

        if (c_InvinciblityList.Count != 0 && m_ActiveConsumables.Count == 0 && PlayerData.instance.tutorialDone == true)
        {
            invinciblityPowerUp.interactable = true;
        }
        else
        {
            invinciblityPowerUp.interactable = false;
        }

        if (c_ScoreMultiplierList.Count != 0 && m_ActiveConsumables.Count == 0 &&
            PlayerData.instance.tutorialDone == true)
        {
            scoreMultiplierPowerup.interactable = true;
        }
        else
        {
            scoreMultiplierPowerup.interactable = false;
        }
    }

    protected void Awake()
    {
        m_Premium = 0;
        m_CurrentLife = 0;
        m_Sliding = false;
        m_SlideStart = 0.0f;
        m_IsRunning = false;
    }

#if !UNITY_STANDALONE
    protected Vector2 m_StartingTouch;
    protected bool m_IsSwiping = false;
#endif

    // Cheating functions, use for testing
    public void CheatInvincible(bool invincible)
    {
        m_IsInvincible = invincible;
    }

    public bool IsCheatInvincible()
    {
        return m_IsInvincible;
    }

    public void Init()
    {
        transform.position = k_StartingPosition;
        m_TargetPosition = Vector3.zero;

        m_CurrentLane = k_StartingLane;
        characterCollider.transform.localPosition = Vector3.zero;

        currentLife = maxLife;

        m_Audio = GetComponent<AudioSource>();

        m_ObstacleLayer = 1 << LayerMask.NameToLayer("Obstacle");
    }

    // Called at the beginning of a run or rerun
    public void Begin()
    {
        m_IsRunning = false;
        character.animator.SetBool(s_DeadHash, false);

        characterCollider.Init();

        m_ActiveConsumables.Clear();
    }

    public void End()
    {
        Debug.Log("End");
        CleanConsumable();
    }

    public void CleanConsumable()
    {
        for (int i = 0; i < m_ActiveConsumables.Count; ++i)
        {
            m_ActiveConsumables[i].Ended(this);
            Addressables.ReleaseInstance(m_ActiveConsumables[i].gameObject);
        }

        m_ActiveConsumables.Clear();
    }

    public void StartRunning()
    {
        StartMoving();
        if (character.animator)
        {
            character.animator.Play(s_RunStartHash);
            character.animator.SetBool(s_MovingHash, true);
        }
    }

    public void StartMoving()
    {
        m_IsRunning = true;
    }

    public void StopMoving()
    {
        m_IsRunning = false;
        trackManager.StopMove();
        if (character.animator)
        {
            character.animator.SetBool(s_MovingHash, false);
        }
    }

    protected bool TutorialMoveCheck(int tutorialLevel)
    {
        tutorialWaitingForValidation = currentTutorialLevel != tutorialLevel;

        return (!TrackManager.instance.isTutorial || currentTutorialLevel >= tutorialLevel);
    }

    private void ResolveSwipe(Vector2 diff)
    {
        if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
        {
            if (TutorialMoveCheck(2) && diff.y < 0)
            {
                if (!m_Sliding)
                    Slide();
            }
            else if (TutorialMoveCheck(1) && diff.y > 0)
            {
                Jump();
            }
        }
        else if (TutorialMoveCheck(0))
        {
            if (diff.x < 0)
            {
                ChangeLane(-1);
            }
            else
            {
                ChangeLane(1);
            }
        }
    }

    protected void Update()
    {
        // Keyboard arrow key inputs
        if (Input.GetKeyDown(KeyCode.LeftArrow) && TutorialMoveCheck(0))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && TutorialMoveCheck(0))
        {
            ChangeLane(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && TutorialMoveCheck(1))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && TutorialMoveCheck(2))
        {
            if (!m_Sliding)
                Slide();
        }

        // Swipe inputs (using touch inputs on mobile, mouse drag in editor / simulator / standalone)
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                m_StartingTouch = touch.position;
                m_IsSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && m_IsSwiping)
            {
                Vector2 diff = touch.position - m_StartingTouch;
                diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);

                if (diff.magnitude > 0.01f) // swipe distance threshold (1% of screen width)
                {
                    ResolveSwipe(diff);
                    m_IsSwiping = false;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                m_IsSwiping = false;
            }
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            m_StartingTouch = Input.mousePosition;
            m_IsSwiping = true;
        }
        else if (Input.GetMouseButton(0) && m_IsSwiping)
        {
            Vector2 diff = (Vector2)Input.mousePosition - m_StartingTouch;
            diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);

            if (diff.magnitude > 0.01f)
            {
                ResolveSwipe(diff);
                m_IsSwiping = false;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_IsSwiping = false;
        }
#endif

        Vector3 verticalTargetPosition = m_TargetPosition;

        if (m_Sliding)
        {
            // Slide time isn't constant but the slide length is (even if slightly modified by speed, to slide slightly further when faster).
            // This is for gameplay reason, we don't want the character to drasticly slide farther when at max speed.
            float correctSlideLength = slideLength * (1.0f + trackManager.speedRatio);
            float ratio = (trackManager.worldDistance - m_SlideStart) / correctSlideLength;
            if (ratio >= 1.0f)
            {
                // We slid to (or past) the required length, go back to running
                StopSliding();
            }
        }

        if (m_Jumping)
        {
            if (trackManager.isMoving)
            {
                // Same as with the sliding, we want a fixed jump LENGTH not fixed jump TIME. Also, just as with sliding,
                // we slightly modify length with speed to make it more playable.
                float correctJumpLength = jumpLength * (1.0f + trackManager.speedRatio);
                float ratio = (trackManager.worldDistance - m_JumpStart) / correctJumpLength;
                if (ratio >= 1.0f)
                {
                    m_Jumping = false;
                    character.animator.SetBool(s_JumpingHash, false);
                }
                else
                {
                    verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
                }
            }
            else if (!AudioListener.pause
            ) //use AudioListener.pause as it is an easily accessible singleton & it is set when the app is in pause too
            {
                verticalTargetPosition.y =
                    Mathf.MoveTowards(verticalTargetPosition.y, 0, k_GroundingSpeed * Time.deltaTime);
                if (Mathf.Approximately(verticalTargetPosition.y, 0f))
                {
                    character.animator.SetBool(s_JumpingHash, false);
                    m_Jumping = false;
                }
            }
        }

        characterCollider.transform.localPosition = Vector3.MoveTowards(characterCollider.transform.localPosition,
            verticalTargetPosition, laneChangeSpeed * Time.deltaTime);

        // Put blob shadow under the character.
        RaycastHit hit;
        if (Physics.Raycast(characterCollider.transform.position + Vector3.up, Vector3.down, out hit,
            k_ShadowRaycastDistance, m_ObstacleLayer))
        {
            blobShadow.transform.position = hit.point + Vector3.up * k_ShadowGroundOffset;
        }
        else
        {
            Vector3 shadowPosition = characterCollider.transform.position;
            shadowPosition.y = k_ShadowGroundOffset;
            blobShadow.transform.position = shadowPosition;
        }
    }

    public void Jump()
    {
        if (!m_IsRunning)
            return;

        if (!m_Jumping)
        {
            if (m_Sliding)
                StopSliding();

            float correctJumpLength = jumpLength * (1.0f + trackManager.speedRatio);
            m_JumpStart = trackManager.worldDistance;
            float animSpeed = k_TrackSpeedToJumpAnimSpeedRatio * (trackManager.speed / correctJumpLength);

            character.animator.SetFloat(s_JumpingSpeedHash, animSpeed);
            character.animator.SetBool(s_JumpingHash, true);
            m_Audio.PlayOneShot(character.jumpSound);
            m_Jumping = true;
        }
    }

    public void StopJumping()
    {
        if (m_Jumping)
        {
            character.animator.SetBool(s_JumpingHash, false);
            m_Jumping = false;
        }
    }

    public void Slide()
    {
        if (!m_IsRunning)
            return;

        if (!m_Sliding)
        {
            if (m_Jumping)
                StopJumping();

            float correctSlideLength = slideLength * (1.0f + trackManager.speedRatio);
            m_SlideStart = trackManager.worldDistance;
            float animSpeed = k_TrackSpeedToJumpAnimSpeedRatio * (trackManager.speed / correctSlideLength);

            character.animator.SetFloat(s_JumpingSpeedHash, animSpeed);
            character.animator.SetBool(s_SlidingHash, true);
            m_Audio.PlayOneShot(slideSound);
            m_Sliding = true;

            characterCollider.Slide(true);
        }
    }

    public void StopSliding()
    {
        if (m_Sliding)
        {
            character.animator.SetBool(s_SlidingHash, false);
            m_Sliding = false;

            characterCollider.Slide(false);
        }
    }

    public void ChangeLane(int direction)
    {
        if (!m_IsRunning)
            return;

        int targetLane = m_CurrentLane + direction;

        if (targetLane < 0 || targetLane > 2)
            // Ignore, we are on the borders.
            return;

        m_CurrentLane = targetLane;
        m_TargetPosition = new Vector3((m_CurrentLane - 1) * trackManager.laneOffset, 0, 0);
    }

    public void UseInventory()
    {
        if (inventory != null && inventory.CanBeUsed(this))
        {
            UseConsumable(inventory);
            inventory = null;
        }
    }

    public void UseConsumable(Consumable c)
    {
        characterCollider.audio.PlayOneShot(powerUpUseSound);

        for (int i = 0; i < m_ActiveConsumables.Count; ++i)
        {
            if (m_ActiveConsumables[i].GetType() == c.GetType())
            {
                // If we already have an active consumable of that type, we just reset the time
                m_ActiveConsumables[i].ResetTime();
                Addressables.ReleaseInstance(c.gameObject);
                return;
            }
        }

        // If we didn't had one, activate that one 
        c.transform.SetParent(transform, false);
        c.gameObject.SetActive(false);
        m_ActiveConsumables.Add(c);
        StartCoroutine(c.Started(this));
    }


    #region AssetUpdateRegion

    #region UpdateAssetData

    public class UpdateAsset
    {
        public string assetId;
        public bool Status;
    }

    #endregion


    #region Update Used Assets
    // Online assets update disabled.
    #endregion

    #endregion
}