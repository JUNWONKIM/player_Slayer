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

        // ✅ 1. 생존 시간 보상 (초당 0.2점)
        AddReward(0.2f * Time.fixedDeltaTime);

        // ✅ 2. 해골과 거리 기반 보상
        float minDistanceRewardCooldown = 1.0f;
        float lastDistRewardTime = 0f;

        if (Time.time - lastDistRewardTime > minDistanceRewardCooldown && closestDist > 15f)
        {
            AddReward(+0.05f);
            lastDistRewardTime = Time.time;
        }


        // ✅ 3. 도망 방향 보상 (반대 방향으로 이동 시 보상)
        if (fleeDir != Vector3.zero && rb.velocity.magnitude > 1f)
        {
            float dot = Vector3.Dot(fleeDir, rb.velocity.normalized);
            if (dot > 0.7f)
                AddReward(+0.2f);
        }

        // ✅ 4. HP 감소 감지 → 큰 패널티
        float hpLoss = previousHP - AgentHp.hp;
        if (hpLoss > 0f)
        {
            AddReward(-3.0f * hpLoss); // 데미지 1당 -3점
            safeTimer = 0f;
        }
        previousHP = AgentHp.hp;

        // ✅ 5. 에피소드 종료 조건
        if (AgentHp.hp <= 0f)
        {
            SetReward(-15.0f); // 큰 패널티
            EndEpisode();
        }
        else if (episodeTimer >= maxEpisodeTime)
        {
            SetReward(+10.0f); // 성공 보상
            EndEpisode();
        }
    }


}
