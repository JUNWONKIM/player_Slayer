using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System.Linq;
using System.IO;

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

    private static int globalEpisodeIndex = 0;

    private float currentSkullSpeed;
    private float currentSpawnInterval;
    private int currentMaxSkulls;

    public override void Initialize()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (AgentHp == null) AgentHp = GetComponent<AgentHp>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        previousHP = AgentHp.hp;

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


        //currentSkullSpeed = Academy.Instance.EnvironmentParameters.GetWithDefault("skullSpeed", 20f);
        //currentSpawnInterval = Academy.Instance.EnvironmentParameters.GetWithDefault("spawnInterval", 3f);
        //currentMaxSkulls = Mathf.FloorToInt(Academy.Instance.EnvironmentParameters.GetWithDefault("maxSkulls", 3));

        //if (creatureSpawner != null)
        //{
        //    creatureSpawner.SetCurriculum(currentSkullSpeed, currentSpawnInterval, currentMaxSkulls);
        //}

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
            .Where(b => b?.GetComponent<BulletInfo>()?.ownerAgent == this.transform).ToArray();

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
            var info = bullet?.GetComponent<BulletInfo>();
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

        GameObject nearestCreature = creatureSpawner.GetNearestCreature();
        float distToCreature = nearestCreature ? Vector3.Distance(transform.position, nearestCreature.transform.position) : float.MaxValue;
        float currentNearestDist = float.IsFinite(distToCreature) && distToCreature < 9999f ? distToCreature : -1f;

        if (currentNearestDist >= 0f && prevNearestDist >= 0f)
        {
            float delta = currentNearestDist - prevNearestDist;
            if (delta < -0.1f)
            {
                distPenaltySum += -delta;
            }
        }
        prevNearestDist = currentNearestDist;

        // ▶ Survival reward (increased)
        float survivalReward = 0.25f * Time.fixedDeltaTime;
        AddReward(survivalReward);
        totalSurvivalReward += survivalReward;

        float hpLoss = previousHP - AgentHp.hp;
        if (hpLoss > 0.01f)
        {
            accumulatedHpLoss += hpLoss;
            int hpLossInt = Mathf.FloorToInt(accumulatedHpLoss);
            if (hpLossInt >= 1)
            {
                float penalty = -1.0f * hpLossInt; // ▶ HP penalty reduced
                AddReward(penalty);
                totalHpPenalty += penalty;
                accumulatedHpLoss -= hpLossInt;
            }
        }
        previousHP = AgentHp.hp;

        if (AgentHp.hp <= 0f || episodeTimer >= maxEpisodeTime)
        {
            float survivalRatio = Mathf.Clamp01(episodeTimer / maxEpisodeTime);
            endReward = AgentHp.hp <= 0f ? -2f + survivalRatio * 2f : +2f; // ▶ Terminal reward scaled down
            AddReward(endReward);

            float survivalTime = Mathf.Max(episodeTimer, 1f);

            float threatPenalty = -1.5f * (threatPenaltySum / survivalTime); // ▶ Threat penalty reduced
            float distPenalty = -0.5f * (distPenaltySum / survivalTime);     // ▶ Distance penalty reduced

            AddReward(threatPenalty);
            AddReward(distPenalty);

            totalThreatPenalty += threatPenalty;
            totalDistPenalty += distPenalty;

            globalEpisodeIndex++;

            string logFolder = Application.dataPath + "/Logs";
            if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
            string path = logFolder + "/reward_log_full.txt";

            string summary = $"[Ep {globalEpisodeIndex}] Threat: {totalThreatPenalty:F2}, Dist: {totalDistPenalty:F2}, Survive: {totalSurvivalReward:F2}, HP: {totalHpPenalty:F2}, End: {endReward:F2}, Final: {GetCumulativeReward():F2}, skullSpeed: {currentSkullSpeed}, spawnInterval: {currentSpawnInterval}, maxSkulls: {currentMaxSkulls}\n";
            File.AppendAllText(path, summary);

            EndEpisode();
        }
    }

}
