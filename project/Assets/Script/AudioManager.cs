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
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ������Ʈ ����
        }
        else
        {
            Destroy(gameObject);
        }

        // ����� �������� �ҷ��� ����
        SetMasterVolume(GetMasterVolume());
        SetBGMVolume(GetBGMVolume());
        SetSFXVolume(GetSFXVolume());
    }

    public void SetMasterVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("Master", dbValue); // AudioMixer�� ������ ���� ����
        PlayerPrefs.SetFloat("MasterVolume", volume); // PlayerPrefs�� ����
    }

    public void SetBGMVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("BGM", dbValue);
        PlayerPrefs.SetFloat("BGMVolume", volume); // PlayerPrefs�� ����
    }

    public void SetSFXVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("SFX", dbValue);
        PlayerPrefs.SetFloat("SFXVolume", volume); // PlayerPrefs�� ����
    }

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 0.75f); // �⺻�� 0.75
    }

    public float GetBGMVolume()
    {
        return PlayerPrefs.GetFloat("BGMVolume", 0.75f); // �⺻�� 0.75
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("SFXVolume", 0.75f); // �⺻�� 0.75
    }
}
