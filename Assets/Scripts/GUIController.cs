using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class GUIController : MonoBehaviour {

	/// <summary>
	/// La classe gérant le changement de tour.
	/// </summary>
	private TurnManager turnManager;

	/// <summary>
	/// Les fonds du joueur actif.
	/// </summary>
	private Text fonds;

	/// <summary>
	/// Le label dans le bandeau supérieur où doit apparaître le nom du joueur.
	/// </summary>
	public Image joueurLabel;

	/// <summary>
	/// Le panel affichant le message d'erreur.
	/// </summary>
	public GameObject errorDialog;

	/// <summary>
	/// Le panel affichant la demande de confirmation.
	/// </summary>
	public GameObject confirmationDialog;
    
    /// <summary>
    /// Le panel des paramètres.
    /// </summary>
    public GameObject parametersPanel;

    /// <summary>
    /// Le panel contenant le résumé d'un assaut.
    /// </summary>
    public GameObject assaultResultPanel;

    /// <summary>Le panel demandant d'attendre la fin du tour des IAs.</summary>
    public GameObject waitingPanel;

	public bool firstTurn = true;
	public bool iaplay = false;

    /// <summary>
    /// Le panel de sélection de cible lors d'une attaque maritime.
    /// </summary>
    public GameObject targetSelectionPanel;

    /// <summary>
    /// les sprites pour le dé rouge.
    /// </summary>
    public List<Sprite> redDice;

    /// <summary>
    /// les sprites pour le dé blanc.
    /// </summary>
    public List<Sprite> whiteDice;

    /// <summary>
    /// Les prefabs pour les entrées de chaque unité dans la liste d'unités des joueurs pour le résumé d'assaut.
    /// L'ordre des unité est le même que celui de Utils.unitCode
    /// </summary>
    public List<GameObject> listOfUnitsPrefabs;

    /// <summary>
    /// Les prefabs pour les boutons de sélection d'unités à cibler lors d'un assaut maritime.
    /// </summary>
    public List<GameObject> targetSelectionPrefabs;

    /// <summary>Chaque image formant l'animation de chargement.</summary>
    public List<Sprite> loadingFrames;

    /// <summary>Le nombre d'images à afficher par seconde pour l'animation de chargement</summary>
    private int framesPerSecond = 10;
    
    private bool isPaused = false;

    /// <summary>Variable d"état indiquant qu'une boite de dialogue est ouverte.</summary>
    public bool dialogOpened = false;

    /// <summary>Variable d'état indiquant que</summary>
    private bool assaultResumeOpened = false;

    void Start() {
        turnManager = this.gameObject.GetComponent<TurnManager>();
        fonds = GameObject.Find("Fonds").transform.GetChild(0).gameObject.GetComponent<Text>();
	}

    void Update() {
		if (turnManager.GetJoueurActif() != null)
		{
			Joueur joueurActif = turnManager.GetJoueurActif();
			TurnManager.phases phaseActive = turnManager.PhaseActive;
            
			joueurLabel.color = Utils.GetColor(joueurActif.Couleur);
			joueurLabel.transform.Find("Nom").gameObject.GetComponent<Text>().text = joueurActif.Nom;

            if (phaseActive == TurnManager.phases.Deploiement)
                joueurLabel.transform.Find("Phase Active").gameObject.GetComponent<Text>().text = "Déploiement";
            else
                joueurLabel.transform.Find("Phase Active").gameObject.GetComponent<Text>().text = phaseActive.ToString();


            if (joueurActif.Credits == 0)
                fonds.text = "Fonds : 0 (+" + joueurActif.EstimatedEarnings() + ") Cr";
            else
                fonds.text = "Fonds : " + joueurActif.Credits + "k (+" + joueurActif.EstimatedEarnings() + ") Cr";
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            if (Input.GetKeyDown(KeyCode.P)) {
                isPaused = !isPaused;
                parametersPanel.SetActive(!parametersPanel.activeSelf);
            }
            if (isPaused)
                Time.timeScale = 0f;
            else {
                Time.timeScale = 1.0f;
            }
        }
        if (waitingPanel.activeSelf)
        {
            int index = (int) (Time.time * framesPerSecond) % loadingFrames.Count;
            waitingPanel.transform.FindChild("Image").GetComponent<Image>().sprite = loadingFrames[index];
        }
    }
    
    /// <summary>
    /// Affiche un message d'erreur à l'utilisateur
    /// </summary>
    /// <param name="message">string Le message à afficher</param>
    public void ErrorMessage(string message)
    {
        errorDialog.transform.FindChild("Text").GetComponent<Text>().text = message;
        OpenDialog(errorDialog);
    }
    /// <summary>
    /// Ferme la boite de dialogue dialog et son parent.
    /// </summary>
    /// <param name="dialog">GameObject La boite de dialogue à fermer.</param>
    public void CloseDialog(GameObject dialog)
    {
        dialog.SetActive(false);
        bool toClose = true;
        Transform dialogParent = dialog.transform.parent;
        int i = -1, dialogsOpened = 0;

        // Si d'autres boites de dialogues sont ouvertes on ne ferme pas le parent
        while (++i < dialogParent.childCount && toClose)
            if (dialogParent.GetChild(i).gameObject.activeSelf)
            {
                dialogsOpened++;
                toClose = false;
            }

        if (!waitingPanel.activeSelf || dialog.name == "Waiting panel")
        {
            dialog.transform.parent.gameObject.SetActive(!toClose);
            dialogOpened = !toClose;
        }
        else if (dialogsOpened > 1)
            dialogOpened = true;
        else
            dialogOpened = false;
    }
    /// <summary>
    /// Ouvre la boite de dialogue dialog et son parent.
    /// </summary>
    /// <param name="dialog"></param>
    public void OpenDialog(GameObject dialog)
    {
        if(!dialog.activeSelf)
            dialog.SetActive(true);

        if(!dialog.transform.parent.gameObject.activeSelf)
            dialog.transform.parent.gameObject.SetActive(true);

        // Permet de mettre au premier plan cette nouvelle fenêtre
        dialog.transform.SetAsLastSibling();

        if(dialog.name != "Waiting panel")
            dialogOpened = true;
    }

    /// <summary>
    /// Demande confirmation d'une action au joueur
    /// </summary>
    /// <param name="message">string Le message à afficher</param>
    /// <param name="confirmation">string Le texte du bouton de confirmation</param>
    /// <param name="annulation">string Le texte du bouton d'annulation</param>
    public void ConfirmationQuestion(string message, string confirmation, string annulation = "Annuler")
    {
        confirmationDialog.transform.FindChild("Text").GetComponent<Text>().text = message;
        confirmationDialog.transform.FindChild("Confirmation").transform.FindChild("Text").GetComponent<Text>().text = confirmation;
        confirmationDialog.transform.FindChild("Annulation").transform.FindChild("Text").GetComponent<Text>().text = annulation;
        confirmationDialog.SetActive(true);
        errorDialog.transform.parent.gameObject.SetActive(true); // Activation du panel permettant de prévenir toute interaction avec le reste du jeu
    }

    /// <summary>
    /// Affiche la fenêtre de sélection de cibles pour les attaques maritimes.
    /// </summary>
    /// <param name="toAttack">Route La route maritime à attaquer.</param>
    public void TargetsSelection(Route toAttack)
    {
        Transform unitesTargetableList = targetSelectionPanel.transform.FindChild("Unités");
        targetSelectionPanel.transform.FindChild("Cible").GetComponent<Text>().text = ""+GameObject.Find("Territoires colorés").GetComponent<ColoredTerritories>().GetRouteIndex(toAttack);
        
        Dictionary<int, Maritime> maritimes = new Dictionary<int, Maritime>();

        // Récupération des unités défensives.
        foreach(Unite unit in toAttack.joueur.Unites)
        {

            if(unit.GetType().Name == "Croiseur" || unit.GetType().Name == "Submarin")
            {

                Maritime maritime = unit as Maritime;

                if (maritime.route.Equals(toAttack))
                    maritimes.Add(toAttack.joueur.Unites.IndexOf(maritime), maritime);
            }
        }

        int i = 0;
        int offset = maritimes.Count % 2 == 0 ? -55 : 0; // La coordonnées x du premier élément de la liste d'unité à afficher
        Dictionary<int, Maritime>.Enumerator maritimesEnum = maritimes.GetEnumerator();

        while(maritimesEnum.MoveNext())
        {
            GameObject prefab;

            // Le prefab de l'entrée pour le panel de sélection de cible
            if (maritimesEnum.Current.Value.GetType().Name == "Croiseur")
                prefab = targetSelectionPrefabs[0];
            else
                prefab = targetSelectionPrefabs[1];

            Instantiate(prefab, unitesTargetableList);

            Transform newEntry = unitesTargetableList.GetChild(unitesTargetableList.childCount - 1);

            // La coordonnées x est calculée en fonction de l'index de la boucle et de si le nombre d'unités est pair ou impair :
            // 1. Pour centrer les unités, on les place du centre vers les extérieurs en alternant gauche et droite,
            //    d'où le Math.Pow(-1, i) : on décale sur la droite (Math.Pow > 0) ou la gauche (Math.Pow < 0).
            // 2. Le décalage se fait par "cran" de 125. Pour connaitre le nombre de cran, on additionne le résultat
            //    de la division du rang de l'unité par 2 au reste de cette même division (i / 2 + i % 2).
            //    Ainsi, la première unité est le centre de la rangée (i/2 + i%2 = 0) et les unités suivantes apparaitront pour l'encadrer
            // 3. Enfin, ceci ne fonctionne que pour un nombre impair d'unités. Pour permettre le centrage d'un nombre pair
            //    on ajoute l'offset défini plus haut.
            newEntry.localPosition = new Vector3((float) Math.Pow(-1, i) * 125 * (i / 2 + i % 2) + offset, 0, 0);
            newEntry.rotation = new Quaternion();
            newEntry.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            
            // L'index permettra de supprimer l'unité visée lors de la résolution de l'attaque.
            newEntry.GetChild(newEntry.childCount - 1).gameObject.GetComponent<Text>().text = "" + maritimesEnum.Current.Key;

            ++i;
        }

        OpenDialog(targetSelectionPanel);
    }

    /// <summary>
    /// Action exécutée après avoir cliqué sur le bouton de confirmation du panel de sélection de cibles pour l'attaque maritime.
    /// Vérifie que suffisamment de cibles ont été sélectionnées et lance l'attaque le cas échéant ou renvoie une erreur à l'utilisateur.
    /// </summary>
    public void targetConfirmationButton()
    {
        Transform unites = targetSelectionPanel.transform.Find("Unités");
        List<GameObject> targetButtons = new List<GameObject>();
        List<Unite> unitAtt = new List<Unite>();
        List<Unite> attaquantUnites = turnManager.GetJoueurActif().Unites;
        int i = -1;

        // Récupération des unités attaquantes.
        while (attaquantUnites.Count > ++i && unitAtt.Count == 0)
            if (attaquantUnites[i].selected)
                unitAtt = attaquantUnites[i].selectedList;

        // Récupérations des boutons sélectionnés (les cibles).
        for (i = 0; i < unites.childCount; i++)
            if (unites.GetChild(i).GetComponent<TargetButton>().selected)
                targetButtons.Add(unites.GetChild(i).gameObject);

        // L'attaquant doit sélectionner un nombre de cibles en fonction du nombre de défenseur et du nombre d'unité à attaquer.
        // Ce nombre ne doit pas aller en-dessous de 1 et pas au dessus de 2.
        // On prend le minimum entre (A) le nombre d'unités à attaquer (une ou plus) et (B) le minimum entre le nombre d'unité à défendre (1 ou plus)
        // et 2.
        // (B) Renverra soit 1 (parce qu'il n'y aurait qu'une unité à la défense) soit 2.
        // (A) Renverra alors soit 1 (parce qu'il n'y aurait qu'une unité à l'attaque ou que (B) aurait renvoyé 1) soit 2.
        if (targetButtons.Count == Math.Min(unitAtt.Count, Math.Min(unites.childCount, 2)))
        {
            int[] targetsIds = new int[Math.Min(unitAtt.Count, Math.Min(unites.childCount, 2))];

            // Récupération de l'index dans la liste des unités du défenseur des cibles sélectionnées par l'attaquant.
            for (i = 0; i < targetButtons.Count; i++)
                targetsIds[i] = Int32.Parse(targetButtons[i].transform.GetChild(targetButtons[i].transform.childCount - 1).GetComponent<Text>().text);

            int index = Int32.Parse(targetSelectionPanel.transform.Find("Cible").GetComponent<Text>().text);

            List<Submarin> submarines = new List<Submarin>(unitAtt.Count);

            foreach (Unite unit in unitAtt)
                submarines.Add(unit as Submarin);

            // Lancement de l'attaque.
            GameObject.Find("BattleManager").GetComponent<BattleManager>().LaunchAttack(
                submarines,
                GameObject.Find("Territoires colorés").GetComponent<ColoredTerritories>().Routes[index],
                targetsIds
            );

            ResetTargetPanel();
        }
        else
            ErrorMessage("Vous devez sélectionner " + Math.Min(2, unitAtt.Count) + " cibles pour procéder à l'attaque.");
    }

    /// <summary>
    /// Action exécutée après avoir cliqué sur le bouton d'annulation du panel de sélection de cible pour les attaques maritimes.
    /// Replace les unités attaquantes et réinitialise le panel de sélection avant de le faire disparaitre.
    /// </summary>
    public void targetCancelButton()
    {
        List<Unite> unitAtt = new List<Unite>();
        List<Unite> attaquantUnites = turnManager.GetJoueurActif().Unites;
        int i = -1;

        // Récupération des unités attaquantes.
        while (attaquantUnites.Count > ++i && unitAtt.Count == 0)
            if (attaquantUnites[i].selected)
                unitAtt = attaquantUnites[i].selectedList;
            

        foreach (Unite unit in unitAtt)
            unit.transform.position = unit.OriginalPosition;

        ResetTargetPanel();
    }

    /// <summary>
    /// Réinitialise le panel de sélection de cible pour l'attaque maritime et le fait disparaitre.
    /// </summary>
    public void ResetTargetPanel()
    {
        Transform unites = targetSelectionPanel.transform.Find("Unités");
        int unitesChildCount = unites.childCount + 1;

        // Destruction des boutons pour chaque unités pouvant être ciblée
        while (--unitesChildCount > 0)
            Destroy(unites.GetChild(unitesChildCount - 1).gameObject);
        
        targetSelectionPanel.transform.Find("Cible").GetComponent<Text>().text = "";
        targetSelectionPanel.SetActive(false);
        targetSelectionPanel.transform.parent.gameObject.SetActive(false);

        dialogOpened = false;
    }

    /// <summary>
    /// Ouvre ou referme le menu des paramètres.
    /// </summary>
    public void ParameterButton()
    {
        parametersPanel.SetActive(!parametersPanel.activeSelf);
    }

    /// <summary>
    /// Affiche le résumé d'une attaque.
    /// </summary>
    /// <param name="diceAtt">int[] Valeurs des dés de l'attaquant.</param>
    /// <param name="diceDef">int[] Valeurs des dés du défenseur.</param>
    /// <param name="unitesDetruitesAtt">int[] Le nombre de chaque unité de l'attaquant détruites durant l'assaut.</param>
    /// <param name="unitesDetruitesDef">int[] Le nombre de chaque unité du défenseur détruites durant l'assaut.</param>
    /// <param name="unitAtt">List(Unite) Les unités restantes de l'attaquant.</param>
    /// <param name="unitDef">List(Unite) Les unités restantes du défenseur.</param>
    public IEnumerator ShowAssaultResult(int[] diceAtt, int[] diceDef, int[] unitesDetruitesAtt, int[] unitesDetruitesDef, List<Unite> unitAtt, List<Unite> unitDef)
    {
        //Debug.Log("Et ça, c'est l'donjon ?");
        GameObject diceGroupAtt;
        GameObject diceGroupDef;

        if (diceAtt.Length == 3)
            diceGroupAtt = assaultResultPanel.transform.FindChild("Attaque").FindChild("Three dice").gameObject;
        else if (diceAtt.Length == 2)
            diceGroupAtt = assaultResultPanel.transform.FindChild("Attaque").FindChild("Two dice").gameObject;
        else
            diceGroupAtt = assaultResultPanel.transform.FindChild("Attaque").FindChild("One die").gameObject;

        if (diceDef.Length == 2)
            diceGroupDef = assaultResultPanel.transform.FindChild("Défense").FindChild("Two dice").gameObject;
        else
            diceGroupDef = assaultResultPanel.transform.FindChild("Défense").FindChild("One die").gameObject;

        diceGroupAtt.SetActive(true);
        diceGroupDef.SetActive(true);

        // Tri dans l'ordre croissant
        Array.Sort(diceAtt);
        Array.Sort(diceDef);

        // Assignation des sprites pour chaque dé de l'attaque
        for (int i = 0; i < diceAtt.Length; i++)
        {
            GameObject die = diceGroupAtt.transform.GetChild(i).gameObject;
            die.GetComponent<Image>().sprite = redDice[diceAtt[i] - 1];
        }

        // Assignation des sprites pour chaque dé de la défense
        for (int i = 0; i < diceDef.Length; i++)
        {
            GameObject die = diceGroupDef.transform.GetChild(i).gameObject;
            die.GetComponent<Image>().sprite = whiteDice[diceDef[i] - 1];
        }

        // Listes d'unités de chaque joueur
        GameObject listeAtt = assaultResultPanel.transform.FindChild("Attaque").FindChild("Liste").gameObject;
        GameObject listeDef = assaultResultPanel.transform.FindChild("Défense").FindChild("Liste").gameObject;

        // Compte les unités ayant participer à la bataille pour les listes d'unités.
        int[] unitsToAtt = new int[7];
        int[] unitsToDef = new int[7];

        // Inclusion des unités détruites dans la liste des unités ayant participé à la bataille.
        unitesDetruitesAtt.CopyTo(unitsToAtt, 0);
        unitesDetruitesDef.CopyTo(unitsToDef, 0);

        /*
        Territoire territoireAtt = null, territoireDef = null;
        Route routeAtt = null, routeDef = null;
        */

        foreach (Unite unit in unitAtt)
        {
            if (unit.GetType().Name == "Infanterie")
                unitsToAtt[(int)Utils.unitCode.Infanterie]++;

            if (unit.GetType().Name == "Tank")
                unitsToAtt[(int)Utils.unitCode.Tank]++;

            if (unit.GetType().Name == "Artillerie")
                unitsToAtt[(int)Utils.unitCode.Artillerie]++;

            if (unit.GetType().Name == "Dca")
                unitsToAtt[(int)Utils.unitCode.DCA]++;

            if (unit.GetType().Name == "Croiseur")
                unitsToAtt[(int)Utils.unitCode.Croiseur]++;

            if (unit.GetType().Name == "Submarin")
                unitsToAtt[(int)Utils.unitCode.Submarin]++;

            if (unit.GetType().Name == "Bombardier")
                unitsToAtt[(int)Utils.unitCode.Bombardier]++;

            /*
            if (unit.GetType().Name == "Croiseur" && unit.GetType().Name == "Submarin")
            {
                Maritime maritime = unit as Maritime;
                routeAtt = maritime.route;
            }
            else if (unit.GetType().Name == "Bombardier")
            {
                Bombardier bombardier = unit as Bombardier;
                territoireAtt = bombardier.territoire;
            }
            else
            {
                Terrestre terrestre = unit as Terrestre;
                territoireAtt = terrestre.territoire;
            }
            */
        }

        foreach (Unite unit in unitDef)
        {
            if (unit.GetType().Name == "Infanterie")
                unitsToDef[(int)Utils.unitCode.Infanterie]++;

            if (unit.GetType().Name == "Tank")
                unitsToDef[(int)Utils.unitCode.Tank]++;

            if (unit.GetType().Name == "Artillerie")
                unitsToDef[(int)Utils.unitCode.Artillerie]++;

            if (unit.GetType().Name == "Dca")
                unitsToDef[(int)Utils.unitCode.DCA]++;

            if (unit.GetType().Name == "Croiseur")
                unitsToDef[(int)Utils.unitCode.Croiseur]++;

            if (unit.GetType().Name == "Submarin")
                unitsToDef[(int)Utils.unitCode.Submarin]++;

            if (unit.GetType().Name == "Bombardier")
                unitsToDef[(int)Utils.unitCode.Bombardier]++;

            /*
            if (unit.GetType().Name == "Croiseur" && unit.GetType().Name == "Submarin")
            {
                Maritime maritime = unit as Maritime;
                routeDef = maritime.route;
            }
            else if (unit.GetType().Name == "Bombardier")
            {
                Bombardier bombardier = unit as Bombardier;
                territoireDef = bombardier.territoire;
            }
            else
            {
                Terrestre terrestre = unit as Terrestre;
                territoireDef = terrestre.territoire;
            }
            */
        }

        int attEntries = 0;
        int defEntries = 0;
        int attNbrDetruites = 0;
        int defNbrDetruites = 0;

        // Ajout de chaque entrée à la liste
        for (int i = 0; i < 7; i++)
        {
            if (unitsToAtt[i] > 0)
            {
                Instantiate(listOfUnitsPrefabs[i], listeAtt.transform);

                GameObject newEntry = listeAtt.transform.GetChild(listeAtt.transform.childCount - 1).gameObject;

                newEntry.transform.localPosition = new Vector3(50, 130 - (20 * attEntries), 0);
                newEntry.transform.rotation = new Quaternion();
                newEntry.transform.localScale = new Vector3(1, 1, 1);
                newEntry.transform.FindChild("Nombre").GetComponent<Text>().text = unitsToAtt[i].ToString();

                ++attEntries;
            }

            if (unitsToDef[i] > 0)
            {
                Instantiate(listOfUnitsPrefabs[i], listeDef.transform);

                GameObject newEntry = listeDef.transform.GetChild(listeDef.transform.childCount - 1).gameObject;

                newEntry.transform.localPosition = new Vector3(50, 130 - (20 * defEntries), 0);
                newEntry.transform.rotation = new Quaternion();
                newEntry.transform.localScale = new Vector3(1, 1, 1);
                newEntry.transform.FindChild("Nombre").GetComponent<Text>().text = unitsToDef[i].ToString();

                ++defEntries;
            }
        }

        Text resume = assaultResultPanel.transform.FindChild("Résumé").GetComponent<Text>();
        bool attHasLostUnits = false, defHasLostUnits = false;
        string attUnitsLost = "", defUnitsLost = "";

        for (int i = 0; i < unitesDetruitesAtt.Length; i++)
        {
            if (unitesDetruitesAtt[i] > 0)
            {
                attHasLostUnits = true;
                string toWrite = unitesDetruitesAtt[i] + " " + Utils.GetUnitsNameByCode(i);

                attUnitsLost += attUnitsLost.Equals("") ? "" + toWrite : "," + toWrite;
                attNbrDetruites += unitesDetruitesAtt[i];
            }

            if (unitesDetruitesDef[i] > 0)
            {
                defHasLostUnits = true;
                string toWrite = unitesDetruitesDef[i] + " " + Utils.GetUnitsNameByCode(i);

                defUnitsLost += defUnitsLost.Equals("") ? "" + toWrite : "," + toWrite;
                defNbrDetruites += unitesDetruitesDef[i];
            }
        }

        List<Unite>.Enumerator unitAttEnum = unitAtt.GetEnumerator();

        if (unitAttEnum.MoveNext() || unitesDetruitesAtt[(int) Utils.unitCode.Bombardier] > 0)
        {
            // Si l'attaque est lancée par des bombardiers ajoute en soutien les DCA à proximité
            if ((unitAttEnum.Current != null && unitAttEnum.Current.GetType().Name == "Bombardier") || unitesDetruitesAtt[(int)Utils.unitCode.Bombardier] > 0)
            {
                Terrestre terrestre = unitDef[0] as Terrestre;
                List<Dca> dcasInRange = terrestre.territoire.GetDcaInRange();

                if (dcasInRange.Count > 0)
                {
                    Transform soutiens = assaultResultPanel.transform.FindChild("Défense").FindChild("Soutiens").FindChild("Liste");

                    Instantiate(listOfUnitsPrefabs[(int)Utils.unitCode.DCA], soutiens.transform);

                    GameObject newEntry = soutiens.transform.GetChild(listeDef.transform.childCount - 1).gameObject;

                    newEntry.transform.localPosition = new Vector3(-3, -30, 0);
                    newEntry.transform.rotation = new Quaternion();
                    newEntry.transform.localScale = new Vector3(1, 1, 1);
                    newEntry.transform.FindChild("Nombre").GetComponent<Text>().text = "" + dcasInRange.Count;
                }
            }
        }
        else
            assaultResultPanel.transform.FindChild("Défense").FindChild("Soutiens").gameObject.SetActive(false);

        resume.text = attHasLostUnits ? "L'attaquant a perdu " + attUnitsLost + ". " : "L'attaquant n'a perdu aucune unité. ";
        resume.text += defHasLostUnits ? "Le défenseur a perdu " + defUnitsLost + ". " : "Le défenseur n'a perdu aucune unité. ";

        if (attNbrDetruites > defNbrDetruites)
        {
            resume.text += "Le défenseur remporte une victoire ";
            resume.text += defNbrDetruites == 0 ? "majeure." : "mineure.";
        }
        else if (attNbrDetruites == defNbrDetruites)
        {
            resume.text += "Les pertes sont égales.";
        }
        else
        {
            resume.text += "L'attaquant remporte une victoire ";
            resume.text += attNbrDetruites == 0 ? "majeure" : "mineure";
            resume.text += unitDef.Count > 0 ? "." : " et le territoire.";
        }

        //Debug.Log("Effectivement, c'est le donjon de Naheulbeuk.");

        OpenDialog(assaultResultPanel);

        while (dialogOpened)
        {
            //Debug.Log("dialogOpened : " + dialogOpened);

            yield return null;
        }

        //Debug.Log("Il a pas l'air terrible ! "+ assaultResumeOpened);
    }

    /// <summary>
    /// Action exécutée après avoir cliqué sur le bouton de confirmation du panel de résumé d'assaut.
    /// Réinitialise le panel et le désactive.
    /// </summary>
    public void AssaultResumeConfirmationButton()
    {
        Transform attaque = assaultResultPanel.transform.FindChild("Attaque");
        Transform defense = assaultResultPanel.transform.FindChild("Défense");

        Transform listAtt = attaque.FindChild("Liste");
        Transform listDef = defense.FindChild("Liste");

        for (int i = 0; i < listAtt.childCount; i++)
            Destroy(listAtt.GetChild(i).gameObject);

        listAtt.DetachChildren();

        for (int i = 0; i < listDef.childCount; i++)
            Destroy(listDef.GetChild(i).gameObject);

        listDef.DetachChildren();

        attaque.FindChild("One die").gameObject.SetActive(false);
        attaque.FindChild("Two dice").gameObject.SetActive(false);
        attaque.FindChild("Three dice").gameObject.SetActive(false);

        defense.FindChild("One die").gameObject.SetActive(false);
        defense.FindChild("Two dice").gameObject.SetActive(false);

        assaultResultPanel.transform.FindChild("Défense").FindChild("Soutiens").gameObject.SetActive(true);

        CloseDialog(assaultResultPanel);
    }

    /// <summary>
    /// [temporaire] Ferme l'application
    /// </summary>
	public void quitGame(){
		Application.Quit ();
	}
}
