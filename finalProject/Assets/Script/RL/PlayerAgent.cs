using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class PlayerAgent : Agent
{
    public float moveSpeed = 5f;
    public Rigidbody rb;
    public AgentHp AgentHp;
    public float survivalRewardPerSecond = 0.1f;
    public float maxEpisodeTime = 60f;

    private float episodeTimer = 0f;
    private float previousHP;

    public Transform[] nearbyEnemies;
    public int maxTrackedEnemies = 3;

    public override void Initialize()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (AgentHp == null)
            AgentHp = GetComponent<AgentHp>();

        previousHP = AgentHp.hp;
        episodeTimer = 0f;
    }

    public override void OnEpisodeBegin()
    {
        previousHP = AgentHp.hp = AgentHp.max_hp;
        episodeTimer = 0f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(AgentHp.hp / AgentHp.max_hp);

        var enemies = GameObject.FindGameObjectsWithTag("Creature");
        System.Array.Sort(enemies, (a, b) => Vector3.Distance(transform.position, a.transform.position)
                                            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        for (int i = 0; i < maxTrackedEnemies; i++)
        {
            if (i < enemies.Length)
            {
                Vector3 dir = (enemies[i].transform.position - transform.position).normalized;
                float dist = Vector3.Distance(transform.position, enemies[i].transform.position) / 20f;
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

        // 생존 시간 보상
        AddReward(survivalRewardPerSecond * Time.fixedDeltaTime);

        // HP 감점 보상 (강화됨)
        float hpLoss = previousHP - AgentHp.hp;
        if (hpLoss > 0)
        {
            AddReward(-0.2f * hpLoss); // 체력 1당 -0.2
        }
        previousHP = AgentHp.hp;

        // 적 근접 패널티
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        foreach (GameObject creature in creatures)
        {
            float dist = Vector3.Distance(transform.position, creature.transform.position);
            if (dist < 5f)
            {
                AddReward(-0.02f * (5f - dist)); // 가까울수록 패널티
            }
        }

        // 종료 조건
        if (AgentHp.hp <= 0f)
        {
            SetReward(-2.0f);
            EndEpisode();
        }
        else if (episodeTimer >= maxEpisodeTime)
        {
            SetReward(+2.0f);
            EndEpisode();
        }
    }
}
