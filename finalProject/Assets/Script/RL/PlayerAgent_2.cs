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

    public int maxCreatures = 3;
    public int maxBullets = 3;

    public CreatureSpawner2 creatureSpawner;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public override void Initialize()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (AgentHp == null)
            AgentHp = GetComponent<AgentHp>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        previousHP = AgentHp.hp;
        episodeTimer = 0f;
        safeTimer = 0f;
    }

    public override void OnEpisodeBegin()
    {
        previousHP = AgentHp.hp = 6f; // ✅ 여유 있는 HP
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
        sensor.AddObservation(AgentHp.hp / 6f); // 정규화된 HP

        var Creatures = GameObject.FindGameObjectsWithTag("Creature");
        System.Array.Sort(Creatures, (a, b) => Vector3.Distance(transform.position, a.transform.position)
                                            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        for (int i = 0; i < maxCreatures; i++)
        {
            if (i < Creatures.Length)
            {
                Vector3 dir = (Creatures[i].transform.position - transform.position).normalized;
                float dist = Vector3.Distance(transform.position, Creatures[i].transform.position) / 100f;
                sensor.AddObservation(dir);  // 방향
                sensor.AddObservation(dist); // 거리
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(1f);  // 최대 거리
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

        // ✅ 움직임 보상 (매 프레임)
        AddReward(rb.velocity.magnitude * 0.01f);

        // ✅ 해골 거리 기반 보상 (가까우면 페널티, 멀면 보상)
        float closestDist = float.MaxValue;
        foreach (GameObject creature in GameObject.FindGameObjectsWithTag("Creature"))
        {
            Skull_RL skull = creature.GetComponent<Skull_RL>();
            if (skull != null && skull.ownerAgent == this.transform)
            {
                float dist = Vector3.Distance(transform.position, creature.transform.position);
                if (dist < closestDist)
                    closestDist = dist;
            }
        }
        if (closestDist < 5f)
        {
            AddReward(-0.02f * (5f - closestDist));  // 더 강한 페널티
        }
        else if (closestDist > 10f && closestDist < 40f)
        {
            AddReward(+0.01f * (closestDist - 10f));  // 더 강한 보상
        }

        // ✅ 생존 보상 (매 1초마다)
        safeTimer += Time.fixedDeltaTime;
        if (safeTimer >= 1f)
        {
            AddReward(+0.5f);
            safeTimer = 0f;
        }

        // ✅ 데미지 보상
        float hpLoss = previousHP - AgentHp.hp;
        if (hpLoss > 0f)
        {
            AddReward(-0.5f * hpLoss);  // 완화된 패널티
        }
        previousHP = AgentHp.hp;

        // ✅ 종료 조건
        if (AgentHp.hp <= 0f)
        {
            SetReward(-3.0f);
            EndEpisode();
        }
        else if (episodeTimer >= maxEpisodeTime)
        {
            SetReward(+5.0f);
            EndEpisode();
        }
    }
}
