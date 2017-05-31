using UnityEngine;
using System.Collections;
using UnityEditor;

public class Parameter : MonoBehaviour {

	private bool isPaused = false;
	public float slider;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P))
			isPaused = !isPaused;
		if (isPaused)
			Time.timeScale = 0f;
		else {
			Time.timeScale = 1.0f;
		}
	}

	void OnGUI(){
		//if (isPaused) {
			//Si le bouton est pressé alors isPaused devient faux donc le jeu reprend.
			slider = EditorGUILayout.Slider (slider, 6.0F, 10.0F);
				//isPaused = false;
			//}

		/*	if (GUI.Button (new Rect (Screen.width / 2 - 40, Screen.height / 2 + 40, 80, 40), "Paramètre")) {
				Application.LoadLevel ("Paramètre");
			}
			//Si le bouton est pressé alors on ferme completement le jeu ou charge le scene "Menu Principal
			//Dans le cas du boutton quitter il faut augmenter sa position Y pour qu'il soit plus bas.
			if(GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 + 100, 80, 40), "Quitter")){
				Application.Quit();
				//Application.LoadLevel("Menu Principal");
				print("Vous avez quitter");
			}
		}*/
	}
}
