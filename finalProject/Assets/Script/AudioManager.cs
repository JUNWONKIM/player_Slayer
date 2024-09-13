using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer audioMixer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 오브젝트 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBGMVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("BGM", dbValue);
        PlayerPrefs.SetFloat("BGMVolume", volume); // PlayerPrefs에 저장
    }

    public void SetSFXVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("SFX", dbValue);
        PlayerPrefs.SetFloat("SFXVolume", volume); // PlayerPrefs에 저장
    }

    public float GetBGMVolume()
    {
        return PlayerPrefs.GetFloat("BGMVolume", 0.75f); // 기본값 0.75
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("SFXVolume", 0.75f); // 기본값 0.75
    }
}
