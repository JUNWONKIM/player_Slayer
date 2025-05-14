using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class PlayerAgent_2 : Agent
{
    public float moveSpeed = 5f;
    public Rigidbody rb;
    public AgentHp AgentHp;
    public float survivalRewardPerSecond = 0.1f;
    public float maxEpisodeTime = 60f;

    private float episodeTimer = 0f;
    private float previousHP;

    public int maxCreatures = 3;
    public int maxBullets = 3;

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

        AddReward(survivalRewardPerSecond * Time.fixedDeltaTime);

        float hpLoss = previousHP - AgentHp.hp;
        if (hpLoss > 0)
        {
            AddReward(-0.1f * hpLoss); // 완화된 페널티
        }
        previousHP = AgentHp.hp;

        foreach (GameObject Creature in GameObject.FindGameObjectsWithTag("Creature"))
        {
            float dist = Vector3.Distance(transform.position, Creature.transform.position);
            if (dist < 50f)
            {
                AddReward(-0.005f * (50f - dist)); // 고스트 거리 완화
            }
        }

        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("C_Bullet"))
        {
            float dist = Vector3.Distance(transform.position, bullet.transform.position);
            if (dist < 20f)
            {
                AddReward(-0.01f * (20f - dist)); // 투사체 거리 완화
            }
        }

        if (AgentHp.hp <= 0f)
        {
            SetReward(-1.0f); // 완화된 사망 패널티
            EndEpisode();
        }
        else if (episodeTimer >= maxEpisodeTime)
        {
            SetReward(+2.0f);
            EndEpisode();
        }
    }
}
