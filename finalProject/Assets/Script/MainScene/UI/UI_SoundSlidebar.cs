using UnityEngine;
using UnityEngine.UI;

public class UI_SoundSlidebar : MonoBehaviour
{
    public Slider masterSlider; // 마스터 볼륨 슬라이더
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // AudioManager에서 볼륨 값 불러오기
        float masterVolume = AudioManager.instance.GetMasterVolume();
        float bgmVolume = AudioManager.instance.GetBGMVolume();
        float sfxVolume = AudioManager.instance.GetSFXVolume();

        // 슬라이더 값 동기화
        masterSlider.value = masterVolume;
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        // 슬라이더 값 변경 시 AudioManager를 통해 볼륨 조절
        masterSlider.onValueChanged.AddListener(delegate { AudioManager.instance.SetMasterVolume(masterSlider.value); });
        bgmSlider.onValueChanged.AddListener(delegate { AudioManager.instance.SetBGMVolume(bgmSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { AudioManager.instance.SetSFXVolume(sfxSlider.value); });
    }
}
