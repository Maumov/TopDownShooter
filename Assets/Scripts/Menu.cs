using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	public GameObject mainMenuHolder;
	public GameObject optionsMenuHolder;

	public Slider[] volumeSliders;
	public Toggle[] resolutionToggles;
	public Toggle fullScreenToggle;

	public int[] screenWidths;
	int activeScreenResIndex;

	void Start(){
		activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
		bool isFullScreen = (PlayerPrefs.GetInt("fullscreen") == 1) ? false : true;

		volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
		volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
		volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

		for(int i = 0; i< resolutionToggles.Length; i ++){
			resolutionToggles[i].isOn = i == activeScreenResIndex;
		}
		fullScreenToggle.isOn = isFullScreen;
	}

	public void Play(){
		SceneManager.LoadScene("Game");
	}
	public void Options(){
		mainMenuHolder.SetActive(false);
		optionsMenuHolder.SetActive(true);
	}

	public void MainMenu(){
		mainMenuHolder.SetActive(true);
		optionsMenuHolder.SetActive(false);
	}

	public void Quit(){
		Application.Quit();
	}

	public void SetScreenResolution(int i){
		if(resolutionToggles[i].isOn){
			activeScreenResIndex = i;
			float aspectRatio = 16f / 9f;
			Screen.SetResolution(screenWidths[i],(int)(screenWidths[i] / aspectRatio),false);
			PlayerPrefs.SetInt("screen res index",activeScreenResIndex);
			PlayerPrefs.Save();
		}
	}
	public void SetFullScreen(bool isFullscreen){
		for(int i = 0 ; i < resolutionToggles.Length; i++){
			resolutionToggles[i].interactable = !isFullscreen;
		}
		if(isFullscreen){
			Resolution[] allResolutions = Screen.resolutions;
			Resolution maxResolution = allResolutions[allResolutions.Length -1];
			Screen.SetResolution(maxResolution.width, maxResolution.height, true);

		}else{
			SetScreenResolution(activeScreenResIndex);
		}
		PlayerPrefs.SetInt("fullscreen",((isFullscreen)? 1: 0));
		PlayerPrefs.Save();
	}
	public void SetMasterVolume(float value){
		AudioManager.instance.SetVolume(value,AudioManager.AudioChannel.Master);
	}
	public void SetMusicVolume(float value){
		AudioManager.instance.SetVolume(value,AudioManager.AudioChannel.Music);
	}
	public void SetSFXVolume(float value){
		AudioManager.instance.SetVolume(value,AudioManager.AudioChannel.Sfx);
	}
}
