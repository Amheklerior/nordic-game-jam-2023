using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class PlayerCharacter : MonoBehaviour, IFeedable, IAttackable
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
    public float AttackViewRange = 3.25f;

    public float MaxAttackDistance = 2f;

    [Space]
    public float PushForce;

    public float ActivationTimer;

    [Space] public float HitSlowdownModifier = .5f;

    [Header("Visuals")]
    //Temp Team Visuals Change this later I guess
    public SpriteRenderer MainSprite;

    [Space]
    public SpriteRenderer SecondarySprite;

    public TrailRenderer Trail;
    [Space] public ParticleSystem TrailParticles;
    public ParticleSystem PushEffect;
    public ParticleSystem AttackEffect;
    [Space] public Transform ArrowPivot;

    private Vector2 movementInput;
    private Vector2 aimInput;

    private Rigidbody2D _rigidbody;

    #region Stats

    private float speedModifier;

    #endregion

    private float AttackTimer;
    private float DashTimer;
    private float AccelerationTimer;
    private float SlowdownTimer;

    private Camera Cam;

    private PlayerCharacter _lastAttack;

    private List<SpriteRenderer> renderers;

    [ShowInInspector]
    private float _currentVelocity => (MaxVelocity + speedModifier) * ForceAcceleration.Evaluate(AccelerationTimer / AccelerationTime);

    private TeamManager _teamManager;

    private void Awake()
    {
        Cam = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();
        _teamManager = FindObjectOfType<TeamManager>();

        renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    private void Start()
    {
        foreach (var rend in renderers)
        {
            ModifyVerticees(rend);
        }
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
        if (col.gameObject.layer == GameConstants.PLAYER_LAYER && _lastAttack != null) _lastAttack = null;
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

    public void RemoveAllResources()
    {
        for (int i = 0; i < CollectedResources.Count; i++)
        {
            CollectedResources[i]?.onConsume();
        }

        CollectedResources.Clear();
    }

    private void FixedUpdate()
    {
        var dir = movementInput * _currentVelocity;
        if (SlowdownTimer != 0f) dir *= HitSlowdownModifier;
        _rigidbody.AddForce(dir, ForceMode2D.Force);
    }

    private void Update()
    {
        ResourceHolder.rotation = Quaternion.Euler(0.0f, 0.0f, Time.time * 360.0f);

        Rewind();

        AccelerationTiming();
        DashTiming();

        void RotateTransform(Transform form, Vector2 dir)
        {
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            form.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

        RotateTransform(AttackEffect.transform, aimInput);
        RotateTransform(ArrowPivot.transform, aimInput);

        AttackTimer = Mathf.Clamp(AttackTimer - Time.deltaTime, 0, ActivationTimer);
        SlowdownTimer = Mathf.Clamp(SlowdownTimer - Time.deltaTime, 0, float.MaxValue);
        
        foreach (var rend in renderers)
        {
            rend.material.SetVector("_Velocity", -_rigidbody.velocity);
        }
    }

    private void Rewind()
    {
        var aspect = (float) Screen.width / Screen.height;

        var worldHeight = Cam.orthographicSize * 2;
        var worldWidth  = worldHeight * aspect;

        worldHeight *= RewindOffset;
        worldWidth *= RewindOffset;

        void ClearTrail()
        {
            Trail.Clear();
            Trail.SetPositions(new Vector3[] {Vector3.zero});
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
        if (DashTimer != 0) DashTimer = Mathf.Clamp(DashTimer - Time.deltaTime, 0, DashCooldown);
    }

    private void Slowed() =>
        SlowdownTimer = 5f;
    
    private void AccelerationTiming()
    {
        if (movementInput != Vector2.zero && AccelerationTimer <= AccelerationTime)
            AccelerationTimer = Mathf.Clamp(AccelerationTimer + Time.deltaTime, (float) 0, (float) AccelerationTime);
        else
            AccelerationTimer = 0;
    }

    public void Push(Vector2 dir, float force)
    {
        _rigidbody.AddForce(dir.normalized * force, ForceMode2D.Impulse);
        PushEffect.Play();
        RemoveAllResources();
        CameraShake.Instance.StartShake(.15f, .5f);
    }

    public void ProvideFeed(float amount) { }

    private bool CanDash() => CollectedResources.Count != 0;

    #region Actions

    public void OnMovement(InputValue value) => movementInput = value.Get<Vector2>();

    public void OnAim(InputValue value)
    {
        aimInput = value.Get<Vector2>();

        if (aimInput == Vector2.zero)
            ArrowPivot.gameObject.SetActive(false);
        else
            ArrowPivot.gameObject.SetActive(true);
    }

    public void OnAttack()
    {
        if (aimInput == Vector2.zero || CollectedResources.Count == 0) return;

        AttackEffect.Play();
        var attacked = GetAttackedTargets();

        attacked.ForEach(x => x.OnAttacked());
        
        var res = CollectedResources[0];
        CollectedResources.Remove(res);
        res.onConsume();

        Debug.LogWarning(attacked.Count);
    }

    public List<IAttackable> GetAttackedTargets()
    {
        var attacked = FindObjectsOfType<MonoBehaviour>().OfType<IAttackable>();
        var targets  = new List<IAttackable>();

        foreach (var target in attacked)
        {
            if (target == this) continue;
            var dot = Vector2.Dot(aimInput, transform.position - target.GetTransform().position);
            if (dot <= AttackViewRange && Vector2.Distance(transform.position, target.GetTransform().position) <= MaxAttackDistance) targets.Add(target);
        }

        return targets;
    }

    public void OnAttacked()
    {
        Slowed();
        CameraShake.Instance.StartShake(.05f, .75f);
        Debug.LogWarning($"{gameObject.name} HAS BEEN ATTACKED!");
    }

    public Transform GetTransform() => transform;

    public void OnDash(InputValue value)
    {
        if (DashTimer != 0 || !CanDash() || SlowdownTimer != 0) return;

        _rigidbody.AddForce(movementInput * DashForce, ForceMode2D.Impulse);
        DashTimer = DashCooldown;
        AttackTimer = ActivationTimer;

        var res = CollectedResources[0];
        CollectedResources.Remove(res);
        res.onConsume();
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

        void SetColor(ParticleSystem system, Color color)
        {
            var main = system.main;
            main.startColor = color;
        }
        
        SetColor(AttackEffect, PlayerTeam.PrimaryColor);
        SetColor(TrailParticles, PlayerTeam.SecondaryColor);
    }

    #region Testing

    [Button]
    public void TestSlowdown() => Slowed();

    #endregion
    
    #region Input Management

    private PlayerInput _inputHandler;

    private void SetupInput()
    {
        _inputHandler = GetComponent<PlayerInput>();
        // _inputHandler.DeactivateInput();
        // GameController.Instance.onMatchStart += () => _inputHandler.ActivateInput();
        // GameController.Instance.onMatchEnd += (_winningTeam) => _inputHandler.DeactivateInput();
    }


    void ModifyVerticees(SpriteRenderer rend)
    {
        Vector2[] newVerticees = new Vector2[64];
        ushort[]  indicees     = new ushort[62 * 3];
        
        
        ushort lastBig   = 63;
        ushort lastSmall = 3;
        
        ushort l1 = 0;
        ushort l2 = 1;
        ushort l3 = 2;
        
        for (int i = 0; i < 64; i++)
        {
            newVerticees[i] = new Vector2(Mathf.Sin(i / 64.0f * 2.0f * Mathf.PI) + 1.0f, Mathf.Cos(i / 64.0f * 2.0f * Mathf.PI) + 1.0f) * 128.0f;
        }
        
        for (int i = 0; i < 62; i++)
        {
            indicees[i * 3] = l1;
            indicees[i * 3 + 1] = l2;
            indicees[i * 3 + 2] = l3;
        
            if (i % 2 == 0)
            {
                l2 = l1;
                l1 = lastBig;
                lastBig--;
            }
            else
            {
                l2 = l3;
                l3 = lastSmall;
                lastSmall++;
            }
        }

        rend.sprite.OverrideGeometry(newVerticees, indicees);
    }

    #endregion
}