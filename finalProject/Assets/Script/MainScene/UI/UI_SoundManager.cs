using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SoundManager : MonoBehaviour
{
    public AudioSource audioSource;  // 배경음악을 재생할 AudioSource
    public AudioClip defaultClip;    // 기본 배경음악 
    public AudioClip bossClip;       // 보스 배경음악 
    public bool loop = true;         // 배경음악 루프 여부
    [Range(0f, 1f)] public float volume = 0.5f; // 배경음악 볼륨 (0.0 ~ 1.0)

    private PlayerHP playerHP;       // PlayerHP 인스턴스
    private bool isBossMusicPlaying = false; // 보스 음악이 재생 중인지 여부

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
            PlayMusic(defaultClip);  // 기본 배경음악 재생
        }

        playerHP = FindObjectOfType<PlayerHP>();
    }

    void Update()
    {
        audioSource.volume = volume;
        audioSource.loop = loop;

        // 플레이어의 HP 상태에 따라 배경음악 변경
        if (playerHP != null)
        {
            if (playerHP.hp <= playerHP.max_hp * 0.3f && !isBossMusicPlaying) //플레이어 hp가 30% 남았을 시
            {
                PlayMusic(bossClip); //보스 음악 재생
                isBossMusicPlaying = true;
            }
        }
    }

    public void PlayMusic(AudioClip clip) // 배경음악 재생
    {
        if (clip == null) return;

        // 현재 재생 중인 음악을 멈추고 새로운 음악으로 교체
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

}