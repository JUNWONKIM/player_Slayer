using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SoundManager : MonoBehaviour
{
    public AudioSource audioSource;  // ��������� ����� AudioSource
    public AudioClip defaultClip;    // �⺻ ������� 
    public AudioClip bossClip;       // ���� ������� 
    public bool loop = true;         // ������� ���� ����
    [Range(0f, 1f)] public float volume = 0.5f; // ������� ���� (0.0 ~ 1.0)

    private PlayerHP playerHP;       // PlayerHP �ν��Ͻ�
    private bool isBossMusicPlaying = false; // ���� ������ ��� ������ ����

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = loop;
        audioSource.volume = volume;

        if (defaultClip != null)
        {
            PlayMusic(defaultClip);  // �⺻ ������� ���
        }

        playerHP = FindObjectOfType<PlayerHP>();
    }

    void Update()
    {
        audioSource.volume = volume;
        audioSource.loop = loop;

        // �÷��̾��� HP ���¿� ���� ������� ����
        if (playerHP != null)
        {
            if (playerHP.hp <= playerHP.max_hp * 0.3f && !isBossMusicPlaying) //�÷��̾� hp�� 30% ������ ��
            {
                PlayMusic(bossClip); //���� ���� ���
                isBossMusicPlaying = true;
            }
        }
    }

    public void PlayMusic(AudioClip clip) // ������� ���
    {
        if (clip == null) return;

        // ���� ��� ���� ������ ���߰� ���ο� �������� ��ü
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

}