using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class PlayerCharacter : MonoBehaviour, IFeedable
{
    [field: SerializeField] public TeamDefinitions PlayerTeam;

    [Header("Resources")]
    public Transform ResourceHolder;
    public List<Resource> CollectedResources;

    [Header("Movement")]
    public float MaxVelocity;

    [Space] public float AccelerationTime;
    public AnimationCurve ForceAcceleration;

    [Space, Min(1f)] public float RewindOffset;

    [Space]
    public float DashForce = 15f;

    public float DashCooldown = 1f;

    [Header("Attack Options")]
    public float PushForce;

    public float ActivationTimer;

    [Header("Visuals")]
    //Temp Team Visuals Change this later I guess
    public SpriteRenderer MainSprite;
    [Space]
    public SpriteRenderer SecondarySprite;

    [Space] public TrailRenderer Trail;
    public ParticleSystem PushEffect;

    private Vector2 movementInput;
    private Rigidbody2D _rigidbody;

    #region Stats

    private float speedModifier;

    #endregion

    private float AttackTimer;
    private float DashTimer;
    private float AccelerationTimer;

    private Camera Cam;

    private PlayerCharacter _lastAttack;

    [ShowInInspector]
    private float _currentVelocity =>
        (MaxVelocity + speedModifier) * ForceAcceleration.Evaluate(AccelerationTimer / AccelerationTime);

    private TeamManager _teamManager;

    private void Awake()
    {
        Cam = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();
        _teamManager = FindObjectOfType<TeamManager>();
        //SetupInput(); // It is not working.. the input is successfully disabled at start, but it daoes not get enebled  
    }

    private void Start()
    {
        OnJoin();
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == GameConstants.RESOURCE_LAYER)
        {
            var re = col.gameObject.GetComponent<Resource>();
            if (!re.IsTaken)
            {
                TakeResource(re);
            }
        }

        if (col.gameObject.layer == GameConstants.WORM_LAYER)
        {
            var worm = col.gameObject.GetComponent<Worm>();
            foreach (var res in CollectedResources)
            {
                worm.ConsumeResource(res);
            }

            CollectedResources.Clear();
        }

        if (col.gameObject.layer == GameConstants.PLAYER_LAYER && AttackTimer != 0f)
        {
            var player = col.gameObject.GetComponent<PlayerCharacter>();

            if (_lastAttack == player) return;
            _lastAttack = player;

            player.Push(_rigidbody.velocity, PushForce);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == GameConstants.PLAYER_LAYER && _lastAttack != null)
            _lastAttack = null;
    }

    private void TakeResource(Resource resource)
    {
        CollectedResources.Add(resource);
        resource.IsTaken = true;
        resource.transform.SetParent(ResourceHolder);
        resource.transform.localPosition = Random.insideUnitCircle * 1.25f;
    }

    public void ConsumeResource(Resource res)
    {
        CollectedResources.Remove(res);
        res.onConsume();

        //Temp consumption effect add more later
        speedModifier += .5f;
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(movementInput * _currentVelocity, ForceMode2D.Force);
    }

    private void Update()
    {
        ResourceHolder.rotation = Quaternion.Euler(0.0f, 0.0f, Time.time * 360.0f);

        Rewind();

        AccelerationTiming();
        DashTiming();

        AttackTimer = Mathf.Clamp(AttackTimer - Time.deltaTime, 0, ActivationTimer);
    }

    private void Rewind()
    {
        var aspect = (float)Screen.width / Screen.height;

        var worldHeight = Cam.orthographicSize * 2;
        var worldWidth = worldHeight * aspect;

        worldHeight *= RewindOffset;
        worldWidth *= RewindOffset;

        void ClearTrail()
        {
            Trail.Clear();
            Trail.SetPositions(new Vector3[] { Vector3.zero });
        }

        if (transform.position.x >= worldWidth / 2)
        {
            transform.position -= new Vector3(worldWidth, 0);
            ClearTrail();
        }
        else if (transform.position.x <= -worldWidth / 2)
        {
            transform.position += new Vector3(worldWidth, 0);
            ClearTrail();
        }

        if (transform.position.y >= worldHeight / 2)
        {
            transform.position -= new Vector3(0, worldHeight);
            ClearTrail();
        }
        else if (transform.position.y <= -worldHeight / 2)
        {
            transform.position += new Vector3(0, worldHeight);
            ClearTrail();
        }
    }

    private void DashTiming()
    {
        if (DashTimer != 0)
            DashTimer = Mathf.Clamp(DashTimer - Time.deltaTime, 0, DashCooldown);
    }

    private void AccelerationTiming()
    {
        if (movementInput != Vector2.zero && AccelerationTimer <= AccelerationTime)
            AccelerationTimer = Mathf.Clamp(AccelerationTimer + Time.deltaTime, (float)0, (float)AccelerationTime);
        else
            AccelerationTimer = 0;
    }

    public void Push(Vector2 dir, float force)
    {
        _rigidbody.AddForce(dir.normalized * force, ForceMode2D.Impulse);
        PushEffect.Play();
        CameraShake.Instance.StartShake(.2f, 1f);
    }


    public void ProvideFeed(float amount) { }

    private bool CanDash() => CollectedResources.Count != 0;

    #region Actions

    public void OnMovement(InputValue value) =>
        movementInput = value.Get<Vector2>();


    public void OnDash(InputValue value)
    {
        if (DashTimer != 0 || !CanDash()) return;

        _rigidbody.AddForce(movementInput * DashForce, ForceMode2D.Impulse);
        DashTimer = DashCooldown;
        AttackTimer = ActivationTimer;

        var res = CollectedResources[0];
        CollectedResources.Remove(res);
        Destroy(res.gameObject);
    }

    public void OnConsume(InputValue value)
    {
        Debug.LogWarning("CONSUME!");
        if (CollectedResources.Count == 0) return;
        ConsumeResource(CollectedResources[0]);
    }

    #endregion

    #region Joing / Leave

    private void OnJoin()
    {
        _teamManager.AddPlayer(this);
        UpdatePlayerVisuals();

        transform.position = Random.insideUnitCircle * 8f;
    }

    #endregion

    private void UpdatePlayerVisuals()
    {
        MainSprite.color = PlayerTeam.PrimaryColor;
        SecondarySprite.color = PlayerTeam.SecondaryColor;
        Trail.colorGradient = PlayerTeam.TrailColor;
    }

    #region Input Management

    private PlayerInput _inputHandler;

    private void SetupInput()
    {
        _inputHandler = GetComponent<PlayerInput>();
        // _inputHandler.DeactivateInput();
        // GameController.Instance.onMatchStart += () => _inputHandler.ActivateInput();
        // GameController.Instance.onMatchEnd += (_winningTeam) => _inputHandler.DeactivateInput();
    }

    #endregion

}