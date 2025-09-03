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

        // 저장된 볼륨값을 불러와 적용
        SetMasterVolume(GetMasterVolume());
        SetBGMVolume(GetBGMVolume());
        SetSFXVolume(GetSFXVolume());
    }

    public void SetMasterVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("Master", dbValue); // AudioMixer의 마스터 볼륨 조절
        PlayerPrefs.SetFloat("MasterVolume", volume); // PlayerPrefs에 저장
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

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 0.75f); // 기본값 0.75
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
