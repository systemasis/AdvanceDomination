using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class MainMenuManager : MonoBehaviour {

    /// <summary>Liste les GameObject représentant les joueurs à créer.</summary>
    public List<GameObject> joueurs;

    /// <summary>Liste les valeurs des dropdown de couleur dans le cas où il serait nécessaire d'intervertir des couleurs.</summary>
    private int[] previousValue;

    /// <summary>Les états d'un joueur : humain ou contrôlé par une intelligence artificielle.</summary>
    private enum state
    {
        humain,
        ia,
    };

    /// <summary>Variable d'état vérifiant que les modifications apportées à un menu déroulant soit l'oeuvre d'un joueur ou d'un script.</summary>
    private bool autoModified;

    public void Start()
    {
        autoModified = true;
        previousValue = new int[joueurs.Count];

        for(int i = 0; i < joueurs.Count; i++)
        {
            Dropdown couleurs = joueurs[i].transform.FindChild("Couleur").FindChild("Couleurs").GetComponent<Dropdown>();
            couleurs.ClearOptions();
            int j = 0;

            foreach(Utils.couleurs couleur in Enum.GetValues(typeof(Utils.couleurs)))
            {
                couleurs.options.Add(new Dropdown.OptionData(FirstLetterToUpperCase(""+couleur)));

                if (j == i)
                {
                    joueurs[i].transform.FindChild("Couleur").FindChild("Image").GetComponent<Image>().color = Utils.GetColor(couleur);
                    couleurs.value = j; // Sélection de la bonne couleur dans le dropdown
                    previousValue[i] = j;
                }

                j++;
            }

            if (i > 1)
                joueurs[i].SetActive(false);

            couleurs.RefreshShownValue();
        }

        autoModified = false;
    }

    /// <summary>
    /// Appelée quand le menu déroulant de l'état d'un joueur est modifié. Active ou désactive le menu déroulant pour sélectionner
    /// la difficulté d'une intelligence artificielle rattachée au joueur correspondant.
    /// </summary>
    /// <param name="changed">Dropdown La dropdown correspondante.</param>
    public void StateChange(Dropdown changed)
    {
        if (!autoModified)
        {
            int etat = changed.value;

            Debug.Log((state) etat == state.humain);

            if ((state)etat == state.humain)
                changed.transform.parent.FindChild("Difficulté").gameObject.SetActive(false);
            else
                changed.transform.parent.FindChild("Difficulté").gameObject.SetActive(true);
            
        }
    }

    /// <summary>
    /// Appelée quand le menu déroulant de coulour d'un joueur est modifié. Change la couleur du joueur concerné et intervertie celle-ci
    /// avec celle du joueur l'ayant déjà sélectionné.
    /// </summary>
    /// <param name="changed">Dropdown Le menu déroulant modifié.</param>
    public void ColorChange(Dropdown changed)
    {
        if(!autoModified)
        {
            autoModified = true;

            for (int i = 0; i < joueurs.Count; i++)
            {
                // S'il ne s'agit pas du joueur dont on vient de modifier la couleur.
                if (i != changed.transform.parent.parent.GetSiblingIndex())
                {
                    Dropdown couleurs = joueurs[i].transform.FindChild("Couleur").FindChild("Couleurs").GetComponent<Dropdown>();

                    Debug.Log("couleurs = "+ couleurs.value +", changed = "+ changed.value + ", comparaison : "+(couleurs.value == changed.value));

                    // Si le joueur a la couleur sélectionnée par changed, on l'intervertie avec la précédente valeur de changed
                    if (couleurs.value == changed.value)
                    {
                        Debug.Log("coucou");

                        couleurs.value = previousValue[changed.transform.parent.parent.GetSiblingIndex()];
                        previousValue[i] = couleurs.value;
                        previousValue[changed.transform.parent.parent.GetSiblingIndex()] = changed.value;

                        ChangeColor(changed.transform.parent, changed.value);
                        ChangeColor(couleurs.transform.parent, couleurs.value);
                    }
                }
            }

            autoModified = false;
        }
    }

    /// <summary>
    /// Appelée lorsque le menu déroulant du nombre de joueur est modifié. Active et désactive les joueurs en fonction du nombre
    /// sélectionné.
    /// </summary>
    /// <param name="changed">Dropdown La dropdown du nombre de joueurs.</param>
    public void NombreChange(Dropdown changed)
    {
        if (!autoModified)
        {
            int nbr = Int32.Parse(changed.options[changed.value].text);

            // i = 2 parce que les joueurs 0 et 1 sont toujours actifs.
            for (int i = 2; i < nbr; i++)
                joueurs[i].SetActive(true);

            for (int i = nbr; i < 6; i++)
                joueurs[i].SetActive(false);

            changed.RefreshShownValue();
        }
    }

    /// <summary>Change la couleur du joueur.</summary>
    /// <param name="joueur">Transform Le transform du parent de la couleur qu'il faut changer.</param>
    /// <param name="value">int La valeur de la couleur à changer.</param>
    public void ChangeColor(Transform joueur, int value)
    {
        joueur.FindChild("Image").GetComponent<Image>().color = Utils.GetColor((Utils.couleurs) value);
        joueur.FindChild("Couleurs").GetComponent<Dropdown>().RefreshShownValue();
    }

    /// <summary>
    /// Returns the input string with the first character converted to uppercase
    /// </summary>
    private string FirstLetterToUpperCase(string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentException("There is no first letter");

        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }
}
