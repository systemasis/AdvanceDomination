using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Parametre : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnParam(){
		Application.LoadLevel (2);
	}

	public void OnNewPart(){
		Application.LoadLevel (1);
	}

	public void OnMainMenu(){
		SceneManager.LoadScene(0);
	}
		
	public void quitGame(){
		Application.Quit ();
		Debug.Log ("On clique et on quitte");
	}
}
