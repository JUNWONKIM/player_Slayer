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

    public CreatureSpawner2 creatureSpawner; // Inspector에서 연결

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
    }


    public override void OnEpisodeBegin()
    {
        previousHP = AgentHp.hp = AgentHp.max_hp;
        episodeTimer = 0f;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // ✅ 초기 위치/회전 복원
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        CleanupCreaturesAndBullets();

        if (creatureSpawner != null)
        {
            creatureSpawner.ResetSpawner();
            creatureSpawner.SetTargetAgent(this.transform);
        }
    }


    void CleanupCreaturesAndBullets()
    {
        string[] tagsToClear = { "Creature", "C_Bullet" };

        foreach (string tag in tagsToClear)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
        }
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
            AddReward(-0.1f * hpLoss);
        }
        previousHP = AgentHp.hp;

        foreach (GameObject Creature in GameObject.FindGameObjectsWithTag("Creature"))
        {
            float dist = Vector3.Distance(transform.position, Creature.transform.position);
            if (dist < 50f)
            {
                AddReward(-0.005f * (50f - dist));
            }
        }

        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("C_Bullet"))
        {
            float dist = Vector3.Distance(transform.position, bullet.transform.position);
            if (dist < 20f)
            {
                AddReward(-0.01f * (20f - dist));
            }
        }

        if (AgentHp.hp <= 0f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
        else if (episodeTimer >= maxEpisodeTime)
        {
            SetReward(+2.0f);
            EndEpisode();
        }
    }
}
