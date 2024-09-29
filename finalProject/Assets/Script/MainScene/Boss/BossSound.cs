using UnityEngine;
using System.Collections;

public class BossSound : MonoBehaviour
{
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip atk0Sound;
    public AudioClip atk1Sound;


    public float walkVolume = 1.0f;
    public float runVolume = 1.0f;
    public float atk0Volume = 1.0f;
    public float atk1Volume = 1.0f;


    public float walkPitch = 1.0f;
    public float runPitch = 1.0f;
    public float atk0Pitch = 1.0f;
    public float atk1Pitch = 1.0f;


    public float soundDelay = 0.1f;

    private AudioSource audioSource;
    private Animator animator;
    private string currentAnimationState;
    private bool isSoundPlaying;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        currentAnimationState = "";
        isSoundPlaying = false;
    }

    void Update()
    {
        HandleAnimationSound();
    }

    private void HandleAnimationSound() //애니메이션 실행 시 소리 실행
    {
        bool isRun = animator.GetBool("IsRun");
        bool isAtk0 = animator.GetBool("ATK0");
        bool isAtk1 = animator.GetBool("ATK1");
        bool isIdle = animator.GetBool("IsIdle");

        // Walk 상태는 다른 모든 상태가 false일 때
        bool isWalk = !isRun && !isAtk0 && !isAtk1 && !isIdle;

        if (isWalk)
        {
            ChangeSound("Walk", walkSound, walkVolume, walkPitch, true);
        }
        else if (isRun)
        {
            ChangeSound("Run", runSound, runVolume, runPitch, true);
        }
        else if (isAtk0)
        {
            StartCoroutine(PlaySoundWithDelay("Atk0", atk0Sound, atk0Volume, atk0Pitch, false, soundDelay));
        }
        else if (isAtk1)
        {
            StartCoroutine(PlaySoundWithDelay("Atk1", atk1Sound, atk1Volume, atk1Pitch, false, soundDelay));
        }
        else if (isIdle)
        {
            StopSound();
        }
    }

    private void ChangeSound(string newState, AudioClip newClip, float volume, float pitch, bool loop) //소리 설정 변경
    {
        if (currentAnimationState != newState || !isSoundPlaying)
        {
            audioSource.Stop();
            audioSource.clip = newClip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.loop = loop;
            audioSource.Play();
            currentAnimationState = newState;
            isSoundPlaying = true;
        }
    }

    private IEnumerator PlaySoundWithDelay(string newState, AudioClip newClip, float volume, float pitch, bool loop, float delay) //사운드 실행 전 딜레이
    {
        // 소리 재생 전에 대기
        yield return new WaitForSeconds(delay);

        // 소리 재생
        ChangeSound(newState, newClip, volume, pitch, loop);
    }

    private void StopSound() //소리 중지
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            isSoundPlaying = false;
        }
    }
}
