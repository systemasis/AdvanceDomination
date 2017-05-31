using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour {

    /// <summary>L'index du joueur actif.</summary>
	private int joueurActif = 0;

    /// <summary>Les joueurs de la partie.</summary>
	private Dictionary<int, Joueur> joueurs;

    /// <summary>Le temps maximum que peut durer le tour d'un joueur.</summary>
	public float maxTime;

    /// <summary>Le temps passé depuis le début du tour.</summary>
	private float timer;

    /// <summary>Variable d'état indiquant si les tours sont chronométrés.</summary>
	public bool timerActive;

	/// <summary>Les différentes phases d'un tour.</summary>
	public enum phases
	{
		Deploiement,
		Attaque,
		Mouvement,
        Assignation
	}
	/// <summary>
	/// La phase dans laquelle se trouve le joueur.
	/// </summary>
	public phases PhaseActive {
		get {
			return phaseActive;
		}
	}
	private phases phaseActive = phases.Deploiement;

	// Le bouton de changement de tour
	public Button button;

	// La classe Partie gérant le jeu
	public GameObject partie;

    public GUIController guiManager;

	void Start () {
		if (maxTime == 0)
			maxTime = 20.0f;
		timer = maxTime;
        if(joueurs == null)
            joueurs = partie.GetComponent<Partie>().Joueurs as Dictionary<int, Joueur>;
    }

	public void Update () {
		if (timerActive)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
				ChangeActivePlayer();
		}
	}

	/// <summary>
	/// Retourne le joueur dont c'est le tour
	/// </summary>
	/// <returns>Joueur Joueur dont c'est le tour.</returns>
	public Joueur GetJoueurActif()
	{
		Joueur player;

		if(joueurs.TryGetValue(joueurActif, out player))
		{
			return player;
		}

		joueurs = partie.GetComponent<Partie>().Joueurs;

		return null;
	}

    /// <summary>
    /// Change le joueur actif
    /// </summary>
    public void ChangeActivePlayer()
    {
        joueurs[joueurActif].MakeAllUnitsUnavailable();

        if (joueurActif == joueurs.Count - 1)
            joueurActif = 0;
        else
            joueurActif++;

        joueurs[joueurActif].MakeAllUnitsAvailable();
        joueurs[joueurActif].AjouterCredits(joueurs[joueurActif].EstimatedEarnings());
        timer = maxTime;

        phaseActive = phases.Deploiement;

        if (joueurs[joueurActif].Humain)
        {
            button.transform.GetChild(0).GetComponent<Text>().text = "Phase suivante";
            button.gameObject.SetActive(true);
            guiManager.CloseDialog(guiManager.waitingPanel);
        }
        else
        {
            button.gameObject.SetActive(false);
            IA joueurIA = joueurs[joueurActif] as IA;
            guiManager.OpenDialog(guiManager.waitingPanel);

            StartCoroutine(joueurIA.IAPlay());
        }
    }

    /// <summary>
    /// Active le changement de phase/tour
    /// </summary>
    public void PasserButton()
    {
        if (!GetComponent<GUIController>().dialogOpened) // S'il n'y a pas de fenêtre de dialogue d'ouverte
        {
            if (phaseActive == phases.Deploiement)
            {
                List<Unite>.Enumerator unitsEnum = joueurs[joueurActif].Unites.GetEnumerator();
                bool canPass = true;

                while (canPass && unitsEnum.MoveNext())
                {
                    if (unitsEnum.Current is Terrestre)
                    {
                        Terrestre unit = unitsEnum.Current as Terrestre;

                        if (unit.territoire == null)
                            canPass = false;
                    }
                    else if (unitsEnum.Current is Aerienne)
                    {
                        Aerienne unit = unitsEnum.Current as Aerienne;

                        if (unit.territoire == null)
                            canPass = false;
                    }
                    else
                    {
                        Maritime unit = unitsEnum.Current as Maritime;

                        if (unit.route == null)
                            canPass = false;
                    }
                }

                if (canPass)
                {
                    phaseActive = phases.Attaque;

                    if (!joueurs[joueurActif].Humain)
                    {
                        IAEasy joueurIA = joueurs[joueurActif] as IAEasy;

                        joueurIA.IAPlay();
                    }
                }
                else
                    InvalidAction("Vous ne pouvez passer votre tour sans déployer toutes les unités nouvellement achetées.");
            }
            else if (phaseActive == phases.Attaque)
            {
                phaseActive = phases.Mouvement;
                
                if (!joueurs[joueurActif].Humain)
                {
                    IAEasy joueurIA = joueurs[joueurActif] as IAEasy;

                    joueurIA.IAPlay();
                }
                else
                    button.transform.GetChild(0).GetComponent<Text>().text = "Fin du tour";
            }
            else if (phaseActive == phases.Mouvement)
                ChangeActivePlayer();
        }
    }

    /// <summary>
    /// Affiche un message d'erreur.
    /// </summary>
    /// <param name="message">string Le message d'erreur à afficher</param>
    public void InvalidAction(string message)
    {
        GetComponent<GUIController>().ErrorMessage(message);
    }

	public void OnGUI() {
		/*
		GUILayout.Label(((int) timer).ToString());
		GUILayout.Label ("Au tour de: "+joueurs[joueurActif].Nom);
        if(joueurActif == joueurs.Count - 1)
    		GUILayout.Label ("Joueur en attente: "+joueurs[0].Nom);
        else
            GUILayout.Label("Joueur en attente: " + joueurs[joueurActif+1].Nom);
        */
	}



}

