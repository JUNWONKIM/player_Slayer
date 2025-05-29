using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System.Linq;

public class PlayerAgent_2 : Agent
{
    public float moveSpeed = 20f;
    public Rigidbody rb;
    public AgentHp AgentHp;
    public float maxEpisodeTime = 60f;

    public int maxCreatures = 2;
    public int maxBullets = 3;

    public CreatureSpawner2 creatureSpawner;

    private float episodeTimer = 0f;
    private float accumulatedHpLoss = 0f;
    private float previousHP;
    private float prevNearestDist = -1f;

    float threatPenaltySum = 0f;
    float distPenaltySum = 0f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    float totalThreatPenalty;
    float totalDistPenalty;
    float totalSurvivalReward;
    float totalHpPenalty;
    float endReward;

    private Vector3 lastActionMove = Vector3.zero;

    public override void Initialize()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (AgentHp == null) AgentHp = GetComponent<AgentHp>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        previousHP = AgentHp.hp;

        //float skullSpeed = Academy.Instance.EnvironmentParameters.GetWithDefault("skullSpeed", 20f);
        //float spawnInterval = Academy.Instance.EnvironmentParameters.GetWithDefault("spawnInterval", 3f);
        //int maxSkulls = Mathf.FloorToInt(Academy.Instance.EnvironmentParameters.GetWithDefault("maxSkulls", 3));

        //if (creatureSpawner != null)
        //{
        //    creatureSpawner.SetCurriculum(skullSpeed, spawnInterval, maxSkulls);
        //}
    }

    public override void OnEpisodeBegin()
    {
        totalThreatPenalty = 0f;
        totalDistPenalty = 0f;
        totalSurvivalReward = 0f;
        totalHpPenalty = 0f;
        endReward = 0f;
        accumulatedHpLoss = 0f;
        episodeTimer = 0f;
        prevNearestDist = -1f;
        threatPenaltySum = 0f;
        distPenaltySum = 0f;

        AgentHp.SetHp(5f, 5f);
        previousHP = AgentHp.hp;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (creatureSpawner != null)
        {
            creatureSpawner.ResetSpawner();
            creatureSpawner.SetTargetAgent(this.transform);
        }

        AddReward(+0.5f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(AgentHp.hp / 5f);

        var creatures = creatureSpawner.GetNearestCreatures(maxCreatures).Where(c => c != null).ToArray();
        foreach (var creature in creatures)
        {
            Vector3 dir = (creature.transform.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, creature.transform.position) / 100f;
            sensor.AddObservation(dir);
            sensor.AddObservation(dist);
        }
        for (int i = creatures.Length; i < maxCreatures; i++)
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(1f);
        }

        var bullets = creatureSpawner.GetNearestBullets(maxBullets)
            .Where(b =>
            {
                var info = b?.GetComponent<BulletInfo>();
                return info != null && info.ownerAgent == this.transform;
            }).ToArray();

        foreach (var bullet in bullets)
        {
            Vector3 dir = (bullet.transform.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, bullet.transform.position) / 50f;
            sensor.AddObservation(dir);
            sensor.AddObservation(dist);
        }
        for (int i = bullets.Length; i < maxBullets; i++)
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 move = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]);
        Vector3 worldMove = transform.TransformDirection(new Vector3(move.x, 0, move.y)).normalized;
        rb.velocity = worldMove * moveSpeed;
        lastActionMove = worldMove;
    }

    void FixedUpdate()
    {
        episodeTimer += Time.fixedDeltaTime;
        creatureSpawner.spawnedCreatures.RemoveAll(c => c == null);
        creatureSpawner.spawnedBullets.RemoveAll(b => b == null);

        // ▶ 위협도 계산 (Skull + Bullet 통합)
        float threatSum = 0f;
        int threatCount = 0;

        foreach (var creature in creatureSpawner.spawnedCreatures)
        {
            if (creature == null) continue;
            float dist = Vector3.Distance(transform.position, creature.transform.position);
            if (dist < 40f)
            {
                threatSum += 1f / Mathf.Max(dist, 1f);
                threatCount++;
            }
        }

        foreach (var bullet in creatureSpawner.spawnedBullets)
        {
            if (bullet == null) continue;
            var info = bullet.GetComponent<BulletInfo>();
            if (info == null || info.ownerAgent != this.transform) continue;

            float dist = Vector3.Distance(transform.position, bullet.transform.position);
            if (dist < 40f)
            {
                threatSum += 1f / Mathf.Max(dist, 1f);
                threatCount++;
            }
        }

        if (threatCount > 0)
        {
            float avgThreat = threatSum / threatCount;
            if (float.IsFinite(avgThreat))
                threatPenaltySum += avgThreat;
        }

        // ▶ 거리 변화 계산: Skull만 대상
        GameObject nearestCreature = creatureSpawner.GetNearestCreature();
        float distToCreature = nearestCreature ? Vector3.Distance(transform.position, nearestCreature.transform.position) : float.MaxValue;
        float currentNearestDist = distToCreature;

        if (!float.IsFinite(currentNearestDist) || currentNearestDist >= 9999f)
            currentNearestDist = -1f;

        if (currentNearestDist >= 0f && prevNearestDist >= 0f &&
            float.IsFinite(currentNearestDist) && float.IsFinite(prevNearestDist))
        {
            float delta = currentNearestDist - prevNearestDist;
            if (delta < -0.1f)
            {
                distPenaltySum += -delta;
            }
        }
        prevNearestDist = currentNearestDist;

        // ▶ 생존 보상
        float survivalReward = 0.4f * Time.fixedDeltaTime;
        AddReward(survivalReward);
        totalSurvivalReward += survivalReward;

        // ▶ HP 손실 페널티
        float hpLoss = previousHP - AgentHp.hp;
        if (hpLoss > 0.01f)
        {
            accumulatedHpLoss += hpLoss;
            int hpLossInt = Mathf.FloorToInt(accumulatedHpLoss);
            if (hpLossInt >= 1)
            {
                float penalty = -1.0f * hpLossInt;
                AddReward(penalty);
                totalHpPenalty += penalty;
                accumulatedHpLoss -= hpLossInt;
            }
        }
        previousHP = AgentHp.hp;

        // ▶ 종료 조건 및 최종 보상
        if (AgentHp.hp <= 0f || episodeTimer >= maxEpisodeTime)
        {
            float survivalRatio = Mathf.Clamp01(episodeTimer / maxEpisodeTime);
            endReward = AgentHp.hp <= 0f ? -10f + survivalRatio * 5f : +25f;
            AddReward(endReward);

            float survivalTime = episodeTimer;
            if (!float.IsFinite(survivalTime) || survivalTime <= 0f)
                survivalTime = 1f;

            float threatPenalty = -0.5f * (threatPenaltySum / survivalTime);
            float distPenalty = -0.2f * (distPenaltySum / survivalTime);

            if (!float.IsFinite(threatPenalty)) threatPenalty = 0f;
            if (!float.IsFinite(distPenalty)) distPenalty = 0f;

            if (Mathf.Abs(distPenaltySum) > 1000f)
            {
                Debug.LogWarning($"[WARN] distPenaltySum overflow: {distPenaltySum}");
                distPenaltySum = 0f;
            }

            AddReward(threatPenalty);
            AddReward(distPenalty);
            totalThreatPenalty += threatPenalty;
            totalDistPenalty += distPenalty;

            PrintRewardSummary();
            EndEpisode();
        }
    }


    void PrintRewardSummary()
    {
        string summary = $"[RewardSummary] Threat: {totalThreatPenalty:F2}, Dist: {totalDistPenalty:F2}, Survive: {totalSurvivalReward:F2}, HP: {totalHpPenalty:F2}, End: {endReward:F2}, Final: {GetCumulativeReward():F2}";
        Debug.Log(summary);

        string logFolder = Application.dataPath + "/Logs";
        if (!System.IO.Directory.Exists(logFolder))
            System.IO.Directory.CreateDirectory(logFolder);

        string logPath = logFolder + "/reward_log.txt";
        System.IO.File.AppendAllText(logPath, summary + "\n");
    }
}
