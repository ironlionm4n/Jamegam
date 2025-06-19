using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnInterval = 2f;           // seconds
    [SerializeField] float offScreenPadding = 1f;        // world units past the frustum
    [SerializeField] EnemyMovementType movementType = EnemyMovementType.Straight;
    [SerializeField] Vector2 speedRange = new Vector2(3f, 5f);   // min / max
    [SerializeField] bool randomizeMovementType = true;

    enum EnemyMovementType { Straight, StraightBobbing, ZigZag, Circular }

    void OnEnable()  => StartCoroutine(SpawnLoop());
    void OnDisable() => StopAllCoroutines();

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        // 1. Where to spawn?
        Vector2 spawnPos = PickEdgeSpawn(offScreenPadding);

        // 2. Instantiate
        GameObject enemyGO = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // 3. Decide main travel direction (towards the opposite edge)
        Vector2 dir = (Camera.main.transform.position - (Vector3)spawnPos).normalized;
        // Optional: bias dir slightly toward screen centre so enemies actually cross the play area
        dir = Vector2.Lerp(dir, -spawnPos.normalized, 0.3f).normalized;

        // 4. Configure movement component
        float speed = Random.Range(speedRange.x, speedRange.y);
        BaseEnemyMover mover = enemyGO.AddComponent(GetMoverType(movementType)) as BaseEnemyMover;
        mover.Init(dir, speed);
        
        if(randomizeMovementType)
            movementType = (EnemyMovementType)Random.Range(0, System.Enum.GetValues(typeof(EnemyMovementType)).Length);
    }

    // --- helpers --------------------------------------------------------

    Vector2 PickEdgeSpawn(float pad = 1f)
    {
        Camera cam = Camera.main;
        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1, 1));

        float left   = bl.x - pad;
        float right  = tr.x + pad;
        float bottom = bl.y - pad;
        float top    = tr.y + pad;

        int edge = Random.Range(0, 4); // 0=L 1=R 2=T 3=B
        return edge switch
        {
            0 => new Vector2(left,  Random.Range(bottom, top)),
            1 => new Vector2(right, Random.Range(bottom, top)),
            2 => new Vector2(Random.Range(left, right),  top),
            _ => new Vector2(Random.Range(left, right), bottom)
        };
    }

    System.Type GetMoverType(EnemyMovementType t) => t switch
    {
        EnemyMovementType.Straight        => typeof(StraightMover),
        EnemyMovementType.StraightBobbing => typeof(StraightBobbingMover),
        EnemyMovementType.ZigZag          => typeof(ZigZagMover),
        EnemyMovementType.Circular        => typeof(CircleMover),
        _                                 => typeof(StraightMover)
    };
}

public abstract class BaseEnemyMover : MonoBehaviour
{
    protected Vector2 dir;
    protected float speed;

    // called by spawner right after AddComponent
    public void Init(Vector2 direction, float speed)
    {
        dir = direction.normalized;
        this.speed = speed;
    }

    protected virtual void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }
    
    protected virtual void RotateTowardsDirection(Vector2 targetDirection)
    {
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}

public class StraightMover : BaseEnemyMover
{
    protected override void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
        RotateTowardsDirection(dir);
    }
}

public class StraightBobbingMover : BaseEnemyMover
{
    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float frequency = 2f;
    float birthTime;

    void Start() => birthTime = Time.time;

    protected override void Update()
    {
        float offset = Mathf.Sin((Time.time - birthTime) * frequency) * amplitude;
        Vector2 perp = new Vector2(-dir.y, dir.x); // 90° to main dir
        Vector2 movement = dir * speed + perp * offset;
        transform.Translate(movement * Time.deltaTime, Space.World);
        RotateTowardsDirection(movement.normalized);
    }
}

public class ZigZagMover : BaseEnemyMover
{
    [SerializeField] float amplitude = 1f;
    [SerializeField] float period = 0.8f;

    protected override void Update()
    {
        float tri = 2f * Mathf.PingPong(Time.time / period, 1f) - 1f; // -1→1 saw
        Vector2 perp = new Vector2(-dir.y, dir.x);
        Vector2 movement = dir * speed + perp * (tri * amplitude);
        transform.Translate(movement * Time.deltaTime, Space.World);
        RotateTowardsDirection(movement.normalized);
    }
}

public class CircleMover : BaseEnemyMover
{
    [SerializeField] float radius = 2f;
    [SerializeField] float revPerSec = 0.5f;
    float angle;

    protected override void Update()
    {
        base.Update();

        angle += revPerSec * 2f * Mathf.PI * Time.deltaTime;
        Vector2 centreOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        Vector2 movement = dir * speed + centreOffset;
        transform.position += (Vector3)centreOffset * Time.deltaTime;
        RotateTowardsDirection(movement.normalized);
    }
}
