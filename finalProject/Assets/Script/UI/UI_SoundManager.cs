using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SoundManager : MonoBehaviour
{
    public AudioSource audioSource;  // 배경음악을 재생할 AudioSource
    public AudioClip defaultClip;    // 기본 배경음악 클립
    public AudioClip bossClip;       // 보스 배경음악 클립
    public bool loop = true;         // 배경음악 루프 여부
    [Range(0f, 1f)] public float volume = 0.5f; // 배경음악 볼륨 (0.0 ~ 1.0)

    private PlayerHP playerHP;       // PlayerHP 인스턴스
    private bool isBossMusicPlaying = false; // 보스 음악이 재생 중인지 여부

    void Start()
    {
        // AudioSource가 설정되지 않은 경우, 컴포넌트를 가져옴
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 초기 설정
        audioSource.loop = loop;
        audioSource.volume = volume;

        // 기본 배경음악 재생
        if (defaultClip != null)
        {
            PlayMusic(defaultClip);
        }

        // PlayerHP 인스턴스를 찾아서 설정
        playerHP = FindObjectOfType<PlayerHP>();
    }

    void Update()
    {
        // 볼륨이나 루프 설정이 실시간으로 변경되었을 때 반영
        audioSource.volume = volume;
        audioSource.loop = loop;

        // 플레이어의 HP 상태에 따라 배경음악 변경
        if (playerHP != null)
        {
            if (playerHP.hp <= playerHP.max_hp * 0.3f && !isBossMusicPlaying)
            {
                // HP가 최대 HP의 30% 이하일 때 보스 음악으로 변경
                PlayMusic(bossClip);
                isBossMusicPlaying = true;
            }
            else if (playerHP.hp > playerHP.max_hp * 0.3f && isBossMusicPlaying)
            {
                // HP가 다시 30% 이상으로 돌아오면 기본 음악으로 변경
                PlayMusic(defaultClip);
                isBossMusicPlaying = false;
            }
        }
    }

    // 배경음악 재생 함수
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        // 현재 재생 중인 음악을 멈추고 새로운 음악으로 교체
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    // 배경음악 일시정지 함수
    public void PauseMusic()
    {
        audioSource.Pause();
    }

    // 배경음악 다시 재생 함수
    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    // 배경음악 정지 함수
    public void StopMusic()
    {
        audioSource.Stop();
    }
}
