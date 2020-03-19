using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject point;
    [SerializeField] private GameObject TrajectoryPt;

    private List<GameObject> TrajectoryPts = new List<GameObject>();
    private Vector2 BeganPos;
    private bool IsTouchDown;
    [SerializeField] private float Speed = 0.03f;

    private void Awake()
    {
        PlayerObserver.OnDamaged += Damaged;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            var obj = Instantiate(TrajectoryPt);
            TrajectoryPts.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                StartCoroutine(PlayerObserver.Damaged());
            }

            if (gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (IsGrounded && GameManager.Health > 0)
                {
                    // Input
#if UNITY_STANDALONE || UNITY_EDITOR
                    if (IsPointerOverGameObject)
                    {
                        HandleTouchingDown(Input.mousePosition);
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            //Mouse Button Down
                            BeganPos = Input.mousePosition;
                            point.transform.position = BeganPos;
                            point.gameObject.SetActive(true);
                            IsTouchDown = true;
                        }
                        else
                        {
                            HandleTouchingDown(Input.mousePosition);

                            if (Input.GetMouseButtonUp(0)) HandleTouchingUp(Input.mousePosition);
                        }
                    }
#elif UNITY_ANDROID
                    if (!IsPointerOverGameObject && Input.touchCount > 0)
                    {
                        var getTocuh = Input.GetTouch(0);
                        var fingerPos = getTocuh.position;
                        switch (getTocuh.phase)
                        {
                            case TouchPhase.Began:
                                BeganPos = fingerPos;
                                Point.transform.position = BeganPos;
                                Point.gameObject.SetActive(true);
                                IsTouchDown = true;
                                break;
                            case TouchPhase.Moved:
                            case TouchPhase.Stationary:
                                {
                                    HandleTouchingDown(fingerPos);
                                    break;
                                }
                            case TouchPhase.Ended:
                                HandleTouchingUp(fingerPos);
                                break;
                            case TouchPhase.Canceled:
                                break;
                        }
                    }
#endif
                }
                else
                {
                    // Handling when caught at the end of a cliff
                    var rigid2D = gameObject.GetComponent<Rigidbody2D>();
                    if (rigid2D.velocity.magnitude == 0)
                    {
                        rigid2D.velocity = gameObject.transform.localScale * Vector2.left * 2;
                    }
                }
            }

            // Flip
            var velocity = gameObject.GetComponent<Rigidbody2D>().velocity;
            if (velocity.x != 0.1)
            {
                if (Mathf.Abs(velocity.x) > 0.1)
                {
                    var directionX = 1;
                    if (velocity.x < 0.1) directionX = -1;

                    var localScale = gameObject.transform.localScale;

                    gameObject.transform.localScale = new Vector3(Mathf.Abs(localScale.x) * directionX, localScale.y, 1);
                }
            }

            // Animator
            var animator = gameObject.GetComponent<Animator>();
            animator.SetFloat("Speed", Mathf.Abs(velocity.x));
            animator.SetFloat("JumpSpeed", velocity.y);
            animator.SetBool("Grounded", IsGrounded);
        }
    }


    private void HandleTouchingDown(Vector2 touchingPos)
    {
        // Is Touching Down MouseButton
        if (IsTouchDown)
        {
            if ((BeganPos - touchingPos).magnitude > 50f)
            {
                // Calculate Velocity
                var velocity = CalculateVelocity(BeganPos, touchingPos);
                var damage = CalculateDamageByJump(velocity);
                PlayerObserver.LossHealthChanged(damage);
                HandleTrajectoryPoint(velocity);
            }
#if UNITY_STANDALONE || UNITY_EDITOR
            else
            {
                if (Input.GetMouseButton(0))
                {
                    HideTrajectoryPoints();
                }
                else
                {
                    InactivateTouchDown();
                }
            }
        }
        else
        {
            HandleTouchingUp(touchingPos);
        }
#endif
    }
    private void HandleTouchingUp(Vector2 touchingPos)
    {
        if (IsTouchDown && (BeganPos - touchingPos).magnitude > 50f)
        {
            GameManager.EffectAudio.PlayEffectSound("jump_06");
            var velocity = CalculateVelocity(BeganPos, Input.mousePosition);
            PlayerObserver.HealthChanged(CalculateDamageByJump(velocity));

            gameObject.GetComponent<Rigidbody2D>().velocity = velocity;
            gameObject.GetComponent<ParticleSystem>().Clear();
            gameObject.GetComponent<ParticleSystem>().Play();
        }

        InactivateTouchDown();
    }

    public void InactivateTouchDown()
    {
        IsTouchDown = false;
        point.SetActive(false);
        HideTrajectoryPoints();
    }

    private void HideTrajectoryPoints()
    {
        PlayerObserver.LossHealthChanged(0);
        HandleTrajectoryPoint(Vector2.zero);
    }

    private bool IsGrounded
    {
        get
        {
            var layerMask = 1 << LayerMask.NameToLayer("Cliff") | 1 << LayerMask.NameToLayer("Bottom");
            var ground = Physics2D.CircleCast(transform.position, 0.3f, Vector2.down, 0.4f, layerMask).rigidbody;
            return ground != null;
        }
    }

    private bool IsPointerOverGameObject
    {
        get
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return EventSystem.current.IsPointerOverGameObject();
#elif UNITY_ANDROID
            
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var getTocuh = Input.GetTouch(i);
                    
                    var isPointerOver = getTocuh.phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(getTocuh.fingerId);

                    if (isPointerOver) return true;
                }
            }
            else
            {
                return false;
            }
#endif
        }
    }

    private Vector2 CalculateVelocity(Vector2 beganPos, Vector2 endedPos)
    {
        return Vector2.ClampMagnitude((endedPos - beganPos) * -1 * Speed, 14.5f) * new Vector2(1, 1.25f);
    }

    private float CalculateDamageByJump(Vector2 velocity)
    {
        return (0.25f + velocity.magnitude * 0.2647f) * -1;
    }

    private void HandleTrajectoryPoint(Vector2 velocity)
    {
        if (velocity != Vector2.zero)
        {
            var rigid2D = gameObject.GetComponent<Rigidbody2D>();
            var position = rigid2D.position;
            var timeStep = Time.fixedDeltaTime * 15 / Physics2D.velocityIterations;
            var gravityAccel = rigid2D.gravityScale * timeStep * timeStep * Physics2D.gravity;

            var drag = 1 - rigid2D.drag * timeStep;
            var moveStep = velocity * timeStep;

            for (int i = 0; i < 5; i++)
            {
                moveStep += gravityAccel;
                moveStep *= drag;
                position += moveStep;
                TrajectoryPts[i].transform.position = position;
                TrajectoryPts[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                TrajectoryPts[i].SetActive(false);
            }
        }
    }

    private IEnumerator Damaged()
    {
        var reactVelocity = new Vector2(-7 * Mathf.Sign(transform.localScale.x), 7 * Mathf.Sign(transform.localScale.y));
        gameObject.GetComponent<Rigidbody2D>().AddForce(reactVelocity, ForceMode2D.Impulse);
        ChangeInvincibleState();

        yield return new WaitForSeconds(1.25f);

        ChangePlayerState();
    }

    private void ChangeInvincibleState()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.56f);
        gameObject.layer = LayerMask.NameToLayer("PlayerInvincible");
        IsTouchDown = false;
    }
    
    private void ChangePlayerState()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

        if (gameObject.layer == LayerMask.NameToLayer("PlayerInvincible"))
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }
}
