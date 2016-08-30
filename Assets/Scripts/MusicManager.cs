using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MusicManager : MonoBehaviour {

	public AudioClip mainTheme;
	public AudioClip menuTheme;

	string sceneName;
	// Use this for initialization
	void Start () {
		OnLevelWasLoaded(0);
		//AudioManager.instance.PlayMusic(menuTheme,2);

	}
	
	void OnLevelWasLoaded(int scene){
		string newSceneName = SceneManager.GetActiveScene().name;
		if(newSceneName != sceneName){
			sceneName = newSceneName;
			Invoke("PlayMusic", .2f);
		}
	}
	void PlayMusic(){
		AudioClip clipToPlay = null;

		if(sceneName == "Menu"){
			clipToPlay = menuTheme;
		}else if(sceneName == "Game"){
			clipToPlay = mainTheme;
		}

		if(clipToPlay != null){
			AudioManager.instance.PlayMusic(clipToPlay,2f);
			Invoke("PlayMusic",clipToPlay.length);
		}
	}
}
