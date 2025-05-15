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
    public GhostSpawner ghostSpawner;
    private float episodeTimer = 0f;
    private float previousHP;
    public Transform field;  // 내 소속 필드 오브젝트


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

        // 필드가 연결되어 있을 때만 중심 위치로 이동
        if (field != null)
        {
            transform.position = field.position + new Vector3(0f, 0.5f, 0f);
        }

        if (ghostSpawner != null)
        {
            ghostSpawner.ResetGhosts();
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

        // ✅ 생존 시간 보상
        AddReward(0.1f * Time.fixedDeltaTime);

        // ✅ HP 감소 페널티
        float hpLoss = previousHP - AgentHp.hp;
        if (hpLoss > 0)
        {
            AddReward(-0.1f * hpLoss);
        }
        previousHP = AgentHp.hp;

        // ✅ 고스트 거리 기반 보상 및 페널티
        bool safeFromGhost = true;
        foreach (GameObject creature in GameObject.FindGameObjectsWithTag("Creature"))
        {
            float dist = Vector3.Distance(transform.position, creature.transform.position);
            if (dist < 10f)
                safeFromGhost = false;

            if (dist < 30f)
                AddReward(-0.005f * (30f - dist));
        }
        if (safeFromGhost)
            AddReward(0.02f * Time.fixedDeltaTime);

        // ✅ 총알 거리 기반 보상 및 페널티
        bool safeFromBullet = true;
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("C_Bullet"))
        {
            float dist = Vector3.Distance(transform.position, bullet.transform.position);
            if (dist < 5f)
                safeFromBullet = false;

            if (dist < 15f)
                AddReward(-0.01f * (15f - dist));
        }
        if (safeFromBullet)
            AddReward(0.03f * Time.fixedDeltaTime);

        // ✅ 종료 조건
        if (AgentHp.hp <= 0f)
        {
            SetReward(-3.0f); // 죽었을 때 큰 패널티
            EndEpisode();
        }
        else if (episodeTimer >= maxEpisodeTime)
        {
            SetReward(+5.0f); // 끝까지 살아남았을 때 보상
            EndEpisode();
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            SetReward(-2.0f);     // 벽에 닿았을 때 패널티
            EndEpisode();         // 에피소드 종료
        }
    }


}
