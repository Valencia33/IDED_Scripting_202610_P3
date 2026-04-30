using System.Collections;
using UnityEngine;

public interface IShootBehavior
{
    void ExecuteShoot();
}

public class BaseShoot : IShootBehavior
{
    private Transform spawnPoint;
    private float speed;
    private Transform upDirection;

    public BaseShoot(Transform spawnPoint, float speed, Transform upDirection)
    {
        this.spawnPoint = spawnPoint;
        this.speed = speed;
        this.upDirection = upDirection;
    }

    public void ExecuteShoot()
    {
        Bullet bullet = Pool.Instance.GetBullet();
        bullet.transform.position = spawnPoint.position;
        bullet.transform.rotation = spawnPoint.rotation;
        bullet.Rigidbody.AddForce(upDirection.up * speed, ForceMode.Impulse);
    }
}

public class ShootDecorator : IShootBehavior
{
    protected IShootBehavior wrappee;

    public ShootDecorator(IShootBehavior wrappee)
    {
        this.wrappee = wrappee;
    }

    public virtual void ExecuteShoot()
    {
        if (wrappee != null)
        {
            wrappee.ExecuteShoot();
        }
    }
}

public class TripleShootDecorator : ShootDecorator
{
    private MonoBehaviour coroutineRunner;
    private float delay;

    public TripleShootDecorator(IShootBehavior wrappee, MonoBehaviour runner, float delay) : base(wrappee)
    {
        this.coroutineRunner = runner;
        this.delay = delay;
    }

    public override void ExecuteShoot()
    {
        coroutineRunner.StartCoroutine(ShootSequenceRoutine());
    }

    private IEnumerator ShootSequenceRoutine()
    {
        base.ExecuteShoot();
        yield return new WaitForSeconds(delay);
        base.ExecuteShoot();
        yield return new WaitForSeconds(delay);
        base.ExecuteShoot();
    }
}

public class ShootCommand : MonoBehaviour, ICommand
{
    #region Bullet

    [Header("Bullet")]
    [SerializeField]
    private Rigidbody bullet;

    [SerializeField]
    private float bulletSpeed = 3F;

    #endregion Bullet

    [Header("Decorator Settings")]
    [SerializeField]
    private float powerUpDuration = 5F;
    
    [SerializeField]
    private float delayBetweenShots = 0.15F;

    private Transform BulletSpawnPoint => Player.Instance.BulletSpawnPoint;

    private bool CanShoot => BulletSpawnPoint != null && bullet != null;

    private IShootBehavior currentShootBehavior;
    private IShootBehavior defaultDecorator;

    private void Start()
    {
        IShootBehavior baseShoot = new BaseShoot(BulletSpawnPoint, bulletSpeed, transform);
        defaultDecorator = new ShootDecorator(baseShoot);
        currentShootBehavior = defaultDecorator;
    }

    public void Execute()
    {
        if (CanShoot)
        {
            if (currentShootBehavior != null)
            {
                currentShootBehavior.ExecuteShoot();
            }
        }
    }

    public void ActivateTripleShoot()
    {
        StartCoroutine(TripleShootRoutine());
    }

    private IEnumerator TripleShootRoutine()
    {
        currentShootBehavior = new TripleShootDecorator(defaultDecorator, this, delayBetweenShots);
        yield return new WaitForSeconds(powerUpDuration);
        currentShootBehavior = defaultDecorator;
    }
}