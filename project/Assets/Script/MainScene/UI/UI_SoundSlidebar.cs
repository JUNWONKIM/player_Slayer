using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_SoundSlidebar : MonoBehaviour
{
    public Slider masterSlider; // ������ ���� �����̴�
    public Slider bgmSlider; // BGM ���� �����̴�
    public Slider sfxSlider; //SFX ���� �����̴�

    private void Start()
    {
        // AudioManager���� ���� �� �ҷ�����
        float masterVolume = AudioManager.instance.GetMasterVolume();
        float bgmVolume = AudioManager.instance.GetBGMVolume();
        float sfxVolume = AudioManager.instance.GetSFXVolume();

        // �����̴� �� ����ȭ
        masterSlider.value = masterVolume;
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        // �����̴� �� ���� �� AudioManager�� ���� ���� ����
        masterSlider.onValueChanged.AddListener(delegate { AudioManager.instance.SetMasterVolume(masterSlider.value); });
        bgmSlider.onValueChanged.AddListener(delegate { AudioManager.instance.SetBGMVolume(bgmSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { AudioManager.instance.SetSFXVolume(sfxSlider.value); });
    }
}
