using UnityEngine;
using UnityEngine.UI;

public class UI_SoundSlidebar : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // AudioManager에서 볼륨 값 불러오기
        float bgmVolume = AudioManager.instance.GetBGMVolume();
        float sfxVolume = AudioManager.instance.GetSFXVolume();

        // 슬라이더 값 동기화
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        // 슬라이더 값 변경 시 AudioManager를 통해 볼륨 조절
        bgmSlider.onValueChanged.AddListener(delegate { AudioManager.instance.SetBGMVolume(bgmSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { AudioManager.instance.SetSFXVolume(sfxSlider.value); });
    }
}
