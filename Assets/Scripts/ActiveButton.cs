using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActiveButton : MonoBehaviour {

	//public Dropdown drop;
	GameObject drop;

	public void ToggleChanged(bool newValue){
		drop.SetActive (newValue);

	}

}
 