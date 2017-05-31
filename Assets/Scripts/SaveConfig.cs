using UnityEngine;
using System.Collections;

public class SaveConfig : MonoBehaviour {

	public string dataPath;

	// Use this for initialization
	void Start () {
		dataPath = Application.dataPath;
		dataPath = dataPath.Replace ("Assets", "");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
