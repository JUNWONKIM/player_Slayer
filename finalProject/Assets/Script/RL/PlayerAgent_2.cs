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

    private float accumulatedHpLoss = 0f;
    private float episodeStartReward = 0f;
    private float episodeTimer = 0f;
    private float previousHP;
    private float safeTimer = 0f;

    public int maxCreatures = 2;
    public int maxBullets = 3;

    public CreatureSpawner2 creatureSpawner;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // 보상 누적 변수
    float totalDistReward;
    float totalDotReward;
    float totalHpPenalty;
    float totalSurvivalReward;
    float endReward;

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
   

        totalDistReward = 0f;
        totalDotReward = 0f;
        totalSurvivalReward = 0f;
        totalHpPenalty = 0f;
        endReward = 0f;


        AgentHp.SetHp(5f, 5f); // hp와 max_hp를 동시에 설정
        previousHP = AgentHp.hp;

        episodeTimer = 0f;
        safeTimer = 0f;
        accumulatedHpLoss = 0f;


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

    // 1. agent 기준으로 이동 벡터 구성 (좌우, 앞뒤)
    Vector3 localMoveDir = new Vector3(move.x, 0, move.y);

    // 2. agent 기준 → 월드 기준으로 변환
    Vector3 worldMoveDir = transform.TransformDirection(localMoveDir);

    // 3. 실제 위치 이동 (월드 공간)
    rb.MovePosition(rb.position + worldMoveDir * moveSpeed * Time.fixedDeltaTime);
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
        episodeStartReward = GetCumulativeReward();

        Vector3 threatVector = Vector3.zero;
        int threatCount = 0;

        creatureSpawner.spawnedCreatures.RemoveAll(c => c == null);

        foreach (GameObject creature in creatureSpawner.spawnedCreatures)
        {
            Skull_RL skull = creature.GetComponent<Skull_RL>();
            if (skull == null) continue;

            Vector3 toSkull = creature.transform.position - transform.position;
            float dist = toSkull.magnitude;

            if (dist < 50f)
            {
                float normalizedDist = Mathf.Clamp01((dist - 3f) / 20f);
                float distReward = normalizedDist * 0.2f;
                AddReward(distReward);
                totalDistReward += distReward;
            }

            if (dist < 40f)
            {
                threatVector += -toSkull.normalized / dist;
                threatCount++;
            }
        }

        Debug.Log($"[DOT] threatCount: {threatCount}, velocity: {rb.velocity.magnitude:F2}");

        if (threatCount > 0)
        {
            if (rb.velocity.magnitude > 0.1f)
            {
                Vector3 fleeDir = threatVector.normalized;
                float dot = Vector3.Dot(fleeDir, rb.velocity.normalized);
                float dotReward = Mathf.Sign(dot) * dot * dot * 0.4f;
                AddReward(dotReward);
                totalDotReward += dotReward;
            }
            else
            {
                // 정지 상태 패널티
                AddReward(-0.05f);
                totalDotReward += -0.05f;
            }
        }

        float survivalReward = 0.5f * Time.fixedDeltaTime;
        AddReward(survivalReward);
        totalSurvivalReward += survivalReward;

        float hpLoss = previousHP - AgentHp.hp;
        if (hpLoss > 0.01f)
        {
            accumulatedHpLoss += hpLoss;

            int hpLossInt = Mathf.FloorToInt(accumulatedHpLoss);
            if (hpLossInt >= 1)
            {
                float hpPenalty = -3.0f * hpLossInt;
                AddReward(hpPenalty);
                totalHpPenalty += hpPenalty;
                accumulatedHpLoss -= hpLossInt;
            }

            safeTimer = 0f;
        }
        previousHP = AgentHp.hp;

        if (AgentHp.hp <= 0f)
        {
            endReward = -15.0f;
            AddReward(endReward);
            Debug.Log($"[CHECK] totalDotReward before summary: {totalDotReward:F5}");

            PrintRewardSummary();
            EndEpisode();
        }
        else if (episodeTimer >= maxEpisodeTime)
        {
            endReward = 10.0f;
            AddReward(endReward);
            Debug.Log($"[CHECK] totalDotReward before summary: {totalDotReward:F5}");

            PrintRewardSummary();
            EndEpisode();
        }
    }

    // ❄️ 클래스 레벨 함수로 분리
    void PrintRewardSummary()
    {
        Debug.Log($"[RewardSummary] " +
            $"DistTotal: {totalDistReward:F3}, " +
            $"DotTotal: {totalDotReward:F3}, " +
            $"Survival: {totalSurvivalReward:F3}, " +
            $"HpPenalty: {totalHpPenalty:F3}, " +
            $"EndReward: {endReward:F3}, " +
            $"FinalReward: {GetCumulativeReward():F3}");
    }
}
