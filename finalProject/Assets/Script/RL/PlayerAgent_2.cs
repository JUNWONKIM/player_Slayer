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
        sensor.AddObservation(AgentHp.hp / 6f);

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

        var bullets = creatureSpawner.GetNearestBullets(maxBullets).Where(b => b != null).ToArray();
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

        // Debug: 초록색 이동 방향
        if (rb.velocity.magnitude > 0.1f)
        {
            Debug.DrawLine(transform.position, transform.position + rb.velocity.normalized * 10f, Color.green, 0.1f, false);
        }

        // Debug: 가장 가까운 해골 방향 (빨강)
        GameObject nearest = creatureSpawner.GetNearestCreature();
        if (nearest != null)
        {
            Vector3 dirToCreature = (nearest.transform.position - transform.position).normalized;
            Debug.DrawLine(transform.position, transform.position + dirToCreature * 10f, Color.red, 0.1f, false);
        }

        float currentNearestDist = nearest ? Vector3.Distance(transform.position, nearest.transform.position) : 100f;

        // 1. 위협 밀도 기반 패널티
        float threatSum = 0f;
        foreach (var creature in creatureSpawner.spawnedCreatures)
        {
            if (creature == null) continue;
            float dist = Vector3.Distance(transform.position, creature.transform.position);
            if (dist < 40f)
                threatSum += 1f / Mathf.Max(dist, 1f);
        }
        float threatPenalty = -0.05f * threatSum;
        AddReward(threatPenalty);
        totalThreatPenalty += threatPenalty;

        // 2. 거리 감소 패널티
        if (prevNearestDist > 0f)
        {
            float delta = currentNearestDist - prevNearestDist;
            if (delta < -0.1f)
            {
                float distPenalty = Mathf.Clamp(delta * 0.1f, -0.1f, 0f);
                AddReward(distPenalty);
                totalDistPenalty += distPenalty;
            }
        }
        prevNearestDist = currentNearestDist;

        // 3. 생존 보상
        float survivalReward = 0.2f * Time.fixedDeltaTime;
        AddReward(survivalReward);
        totalSurvivalReward += survivalReward;

        // 4. HP 손실 페널티
        float hpLoss = previousHP - AgentHp.hp;
        if (hpLoss > 0.01f)
        {
            accumulatedHpLoss += hpLoss;
            int hpLossInt = Mathf.FloorToInt(accumulatedHpLoss);
            if (hpLossInt >= 1)
            {
                float penalty = -2.0f * hpLossInt;
                AddReward(penalty);
                totalHpPenalty += penalty;
                accumulatedHpLoss -= hpLossInt;
            }
        }
        previousHP = AgentHp.hp;

        // 5. 에피소드 종료 판단
        if (AgentHp.hp <= 0f)
        {
            float survivalRatio = Mathf.Clamp01(episodeTimer / maxEpisodeTime);
            endReward = -10f + survivalRatio * 5f;
            AddReward(endReward);
            PrintRewardSummary();
            EndEpisode();
        }
        else if (episodeTimer >= maxEpisodeTime)
        {
            endReward = +25f;
            AddReward(endReward);
            PrintRewardSummary();
            EndEpisode();
        }
    }

    void PrintRewardSummary()
    {
        Debug.Log($"[RewardSummary] ThreatPenalty: {totalThreatPenalty:F2}, DistPenalty: {totalDistPenalty:F2}, " +
                  $"Survive: {totalSurvivalReward:F2}, HP: {totalHpPenalty:F2}, End: {endReward:F2}, " +
                  $"Final: {GetCumulativeReward():F2}");
    }
}
