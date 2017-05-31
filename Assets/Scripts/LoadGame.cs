using UnityEngine;
using System.Collections;

public class LoadGame: MonoBehaviour {

	public void load1player()
	{
		PlayerPrefs.SetInt("jeu", 1);
		Application.LoadLevel(1);
	}

	public void load2players()
	{
		PlayerPrefs.SetInt("jeu", 2);
		Application.LoadLevel(1);
	}
}