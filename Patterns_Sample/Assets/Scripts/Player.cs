using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Player : MonoBehaviour
{
    public const int PLAYER_LIVES = 3;

    public const float PLAYER_RADIUS = 0.4F;

    private static Player instance;

    #region StatsProperties

    [SerializeField]
    private Transform bulletSpawnPoint;

    public int Score { get; set; }
    public int Lives { get; set; }

    public float HVal { get; private set; }

    public Transform BulletSpawnPoint => bulletSpawnPoint;

    #endregion StatsProperties

    private MovementCommand movementCommand;
    private ShootCommand shootCommand;

    public Action OnPlayerDied;
    public Action OnPlayerHit;

    public static Player Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Lives = PLAYER_LIVES;

        OnPlayerHit += PlayerHit;
        OnPlayerDied += PlayerDied;
        Target.onTargetDestroyed += AddScore;

        movementCommand = gameObject.GetComponent<MovementCommand>();
        shootCommand = gameObject.GetComponent<ShootCommand>();
    }

    private void PlayerHit()
    {
        Lives -= 1;

        if (Lives <= 0 && OnPlayerDied != null)
        {
            OnPlayerDied();
        }
    }

    private void AddScore(int scoreAdd) => Score += scoreAdd;

    private void PlayerDied()
    {
        //OnPlayerDied -= PlayerDied;
        OnPlayerDied = null;
        OnPlayerHit = null;
        Target.onTargetDestroyed -= AddScore;

        this.enabled = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        HVal = Input.GetAxis("Horizontal");

        if (HVal != 0F)
        {
            //if (movementCommand != null)
            //{
            //    movementCommand.Execute();
            //}
            movementCommand?.Execute();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            shootCommand?.Execute();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (shootCommand != null)
            {
                shootCommand.ActivateTripleShoot();
            }
        }
    }
}