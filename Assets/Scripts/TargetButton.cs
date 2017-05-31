using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TargetButton : MonoBehaviour {

    public GameObject targetSelectionPanel;

    public List<TargetButton> targetButtons;

    public bool selected = false;

	// Use this for initialization
	void Start () {
        targetButtons = new List<TargetButton>();
        targetSelectionPanel = GameObject.Find("Canvas").transform.FindChild("Dialogs").FindChild("Target Selection Dialog").gameObject;
        Transform unites = targetSelectionPanel.transform.FindChild("Unités");

        // Récupération des boutons déjà existant et ajout de celui-ci à la liste des premiers.
        for (int i = 0; i < unites.childCount; i++)
        {
            TargetButton button = unites.GetChild(i).GetComponent<TargetButton>();

            if (!button.Equals(this))
            {
                targetButtons.Add(button);
                button.targetButtons.Add(this);
            }
        }
    }

    /// <summary>
    /// Action s'exécutant au clic de ce bouton.
    /// Sélectionne/désélectionne ce bouton respectivement si le maximum de boutons sélectionnés n'est pas atteint et qu'il n'est pas activé
    /// ou qu'il est activé.
    /// </summary>
    public void Click()
    {
        if (selected)
        {
            selected = false;
            transform.Find("Selected").gameObject.SetActive(false);
        }
        else
        {
            int selecteds = 0, i = -1;

            while(selecteds < 2 && ++i < targetButtons.Count)
            {
                if (targetButtons[i].selected)
                    selecteds++;
            }

            if (selecteds < 2)
            {
                selected = true;
                transform.Find("Selected").gameObject.SetActive(true);
            }
        }
    }
}
