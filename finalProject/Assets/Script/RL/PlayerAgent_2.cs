using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class PlayerAgent_2 : Agent
{
    public float moveSpeed = 20f;
    public Rigidbody rb;
    public AgentHp AgentHp;
    public float maxEpisodeTime = 60f;

    private float episodeTimer = 0f;
    private float previousHP;
    private float safeTimer = 0f;

    public int maxCreatures = 2;
    public int maxBullets = 3;

    public CreatureSpawner2 creatureSpawner;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public override void Initialize()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (AgentHp == null) AgentHp = GetComponent<AgentHp>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        previousHP = AgentHp.hp;
        episodeTimer = 0f;
        safeTimer = 0f;
    }

    public override void OnEpisodeBegin()
    {
        AgentHp.SetHp(5f, 5f); // hp와 max_hp를 동시에 설정
        previousHP = AgentHp.hp;

        episodeTimer = 0f;
        safeTimer = 0f;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        CleanupCreaturesAndBullets();

        if (creatureSpawner != null)
        {
            creatureSpawner.ResetSpawner();
            creatureSpawner.SetTargetAgent(this.transform);
        }

        AddReward(+0.5f); // 시작 보너스
    }


    void CleanupCreaturesAndBullets()
    {
        GameObject[] allCreatures = GameObject.FindGameObjectsWithTag("Creature");
        foreach (GameObject creature in allCreatures)
        {
            Skull_RL skull = creature.GetComponent<Skull_RL>();
            if (skull != null && skull.ownerAgent == this.transform)
            {
                Destroy(creature);
            }
        }

        GameObject[] allBullets = GameObject.FindGameObjectsWithTag("C_Bullet");
        foreach (GameObject bullet in allBullets)
        {
            Destroy(bullet);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(AgentHp.hp / 6f);

        var Creatures = GameObject.FindGameObjectsWithTag("Creature");
        System.Array.Sort(Creatures, (a, b) => Vector3.Distance(transform.position, a.transform.position)
                                            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        for (int i = 0; i < maxCreatures; i++)
        {
            if (i < Creatures.Length)
            {
                Vector3 dir = (Creatures[i].transform.position - transform.position).normalized;
                float dist = Vector3.Distance(transform.position, Creatures[i].transform.position) / 100f;
                sensor.AddObservation(dir);
                sensor.AddObservation(dist);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(1f);
            }
        }

        var bullets = GameObject.FindGameObjectsWithTag("C_Bullet");
        System.Array.Sort(bullets, (a, b) => Vector3.Distance(transform.position, a.transform.position)
                                            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        for (int i = 0; i < maxBullets; i++)
        {
            if (i < bullets.Length)
            {
                Vector3 dir = (bullets[i].transform.position - transform.position).normalized;
                float dist = Vector3.Distance(transform.position, bullets[i].transform.position) / 50f;
                sensor.AddObservation(dir);
                sensor.AddObservation(dist);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(1f);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 move = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]);
        Vector3 moveDir = new Vector3(move.x, 0, move.y);
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        episodeTimer += Time.fixedDeltaTime;
        safeTimer += Time.fixedDeltaTime;

        Vector3 fleeDir = Vector3.zero;
        float closestDist = float.MaxValue;

        foreach (GameObject creature in GameObject.FindGameObjectsWithTag("Creature"))
        {
            Skull_RL skull = creature.GetComponent<Skull_RL>();
            if (skull != null && skull.ownerAgent == this.transform)
            {
                Vector3 dirToSkull = (skull.transform.position - transform.position);
                float dist = dirToSkull.magnitude;

                if (dist < closestDist)
                {
                    closestDist = dist;
                    fleeDir = -dirToSkull.normalized;
                }
            }
        }

        // 해골 반대 방향으로 이동 중이면 보상
        if (closestDist < 30f && fleeDir != Vector3.zero)
        {
            float dot = Vector3.Dot(fleeDir, rb.velocity.normalized);
            if (dot > 0.7f && rb.velocity.magnitude > 1f)
                AddReward(+0.1f);
        }

        // 너무 가까우면 패널티
        if (closestDist < 5f)
            AddReward(-0.2f * (5f - closestDist));
        // 멀면 보상
        else if (closestDist > 10f)
            AddReward(+0.05f);

        // 생존 시간 보상 (0.05점/초)
        AddReward(0.05f * Time.fixedDeltaTime);

        // HP 감소 감지
        float hpLoss = previousHP - AgentHp.hp;
        if (hpLoss > 0f)
            AddReward(-1.0f * hpLoss);
        previousHP = AgentHp.hp;

        // 생존 보상 or 사망 패널티
        if (AgentHp.hp <= 0f)
        {
            SetReward(-5.0f);
            EndEpisode();
        }
        else if (episodeTimer >= maxEpisodeTime)
        {
            SetReward(+5.0f);
            EndEpisode();
        }
    }

}
