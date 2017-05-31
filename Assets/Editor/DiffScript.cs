using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

public class DiffScript : MonoBehaviour {

	Text difficultyText;
	Slider slider;
	string textBase = "Difficulté:";

	// Use this for initialization

	void Start() {
		difficultyText = GetComponent<Text> ();
		difficultyText.text = textBase + " Facile";
		slider = GameObject.Find ("DifficultySlider").GetComponent<Slider> ();
	}

	public void updateDifficulty(){
		switch ((int)slider.value) {
		case 0:
			difficultyText.text = textBase + " Facile";
			break;
		case 1:
			difficultyText.text = textBase + " Moyen";
			break;
		case 2:
			difficultyText.text = textBase + " Difficile";
			break;
		}
	}

}
