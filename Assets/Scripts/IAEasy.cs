using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAEasy : IA {

    public int step = 0;
    private bool turnIsOver;

	public override IEnumerator IAPlay(){
        turnIsOver = false;

        while (!turnIsOver)
        {
            if (turnManager.PhaseActive == TurnManager.phases.Assignation)
                IAUniteDeploiement();
            else if (turnManager.PhaseActive == TurnManager.phases.Deploiement)
                IAUniteAchat();
            else if (turnManager.PhaseActive == TurnManager.phases.Attaque)
            {
                yield return StartCoroutine(IAAttack());
            }
            else if (turnManager.PhaseActive == TurnManager.phases.Mouvement)
            {
                IADefense();
                turnIsOver = true;
            }

            turnManager.PasserButton();

            yield return null;
        }
    }

    /// <summary>Déplacement des unités de l'IA lorsqu'il a la main sur les territoires non occupés par un ennemi.</summary>
    public void IAUniteDeploiement()
    {

        Dictionary<int, Territoire>.Enumerator territoiresEnum = coloredTerritories.Territoires.GetEnumerator();
        bool deploye = false;

        // Recherche d'un territoire libre pour le réclamer et y placer une infanterie
        while (!deploye && territoiresEnum.MoveNext())
        {
            if (territoiresEnum.Current.Value.joueur == null)
            {
                deploye = true;
                Territoire newTerritoire = territoiresEnum.Current.Value;

                newTerritoire.joueur = this;
                AddTerritoire(newTerritoire);

                Infanterie newUnit = boutique.CreateUnit(
                    (int)Utils.unitCode.Infanterie,
                    GameObject.Find("Unites").transform,
                    newTerritoire.position, this
                ).gameObject.GetComponent<Infanterie>();
                newUnit.territoire = newTerritoire;
                newTerritoire.unites.Add(newUnit);
            }
        }
    }

	/// <summary>
	/// Permet à l'IA d'acheter des unités selon les conditions.
	/// </summary>
	public void IAUniteAchat()
    {
        step = 1;
        // Le plus bas score et l'index du territoire correspondant.
        // 1000 nécessiterait par exemple 100 tanks sur le territoire, c'est donc une valeur à priori impossible à atteindre.
        int worstScore = 1000, worstIndex = 0;
        int[] worstCount = new int[] { 0, 0, 0, 0, 0 };

        foreach (KeyValuePair<int, Territoire> keyValue in territoires)
        {
            int score = 0;
            int[] unitesCount = new int[] { 0, 0, 0, 0, 0 };

            foreach(Unite unit in keyValue.Value.unites)
            {
                if (unit.GetType().Name == "Infanterie")
                {
                    score += 4;
                    unitesCount[(int)Utils.unitCode.Infanterie]++;
                }
                else if (unit.GetType().Name == "Tank")
                {
                    score += 10;
                    unitesCount[(int)Utils.unitCode.Tank]++;
                }
                else if (unit.GetType().Name == "Artillerie")
                {
                    score += 7;
                    unitesCount[(int)Utils.unitCode.Artillerie]++;
                }
                else if (unit.GetType().Name == "Dca")
                {
                    score += 4;
                    unitesCount[(int)Utils.unitCode.DCA]++;
                }
                else
                {
                    score += 5;
                    unitesCount[4]++;
                }
            }

            if(worstScore > score)
            {
                worstScore = score;
                worstIndex = keyValue.Value.Num;
                worstCount = unitesCount;
            }
        }

        Terrestre newUnit = null;

        if (worstCount[(int)Utils.unitCode.Tank] == 0)
        {

            if (credits >= Tank.COUT)
            {
                newUnit = boutique.CreateUnit(
                    (int)Utils.unitCode.Tank,
                    GameObject.Find("Unites").transform,
                    territoires[worstIndex].position,
                    this
                ).gameObject.GetComponent<Tank>();
            }
            else if (worstCount[(int)Utils.unitCode.Infanterie] < 5 && credits >= Infanterie.COUT)
            {
                newUnit = boutique.CreateUnit(
                    (int)Utils.unitCode.Infanterie,
                    GameObject.Find("Unites").transform,
                    territoires[worstIndex].position,
                    this
                ).gameObject.GetComponent<Infanterie>();
            }
        }
        else if (worstCount[(int)Utils.unitCode.Artillerie] == 0 && credits >= Artillerie.COUT)
        {
            newUnit = boutique.CreateUnit(
                (int)Utils.unitCode.Artillerie,
                GameObject.Find("Unites").transform,
                territoires[worstIndex].position,
                this
            ).gameObject.GetComponent<Artillerie>();
        }
        else if (credits >= Bombardier.COUT)
        {
            Aerienne newAerien = boutique.CreateUnit(
                (int)Utils.unitCode.Bombardier,
                GameObject.Find("Unites").transform,
                territoires[worstIndex].position,
                this
            ).gameObject.GetComponent<Bombardier>();

            newAerien.territoire = territoires[worstIndex];
            territoires[worstIndex].unites.Add(newAerien);
        }

        if(newUnit != null)
        {
            newUnit.territoire = territoires[worstIndex];
            territoires[worstIndex].unites.Add(newUnit);
        }

        if (credits >= Dca.COUT)
        {
            List<Territoire>.Enumerator voisins3DegreEnum = territoires[worstIndex].GetVoisinsNDegree(3, true, true).GetEnumerator();
            bool bombardierAPortee = false;

            while (!bombardierAPortee && voisins3DegreEnum.MoveNext())
            {
                List<Unite>.Enumerator unitesEnum = voisins3DegreEnum.Current.unites.GetEnumerator();

                while (!bombardierAPortee && unitesEnum.MoveNext())
                    if (unitesEnum.Current.GetType().Name == "Bombardier")
                        bombardierAPortee = true;
            }

            if (bombardierAPortee)
            {
                newUnit = boutique.CreateUnit(
                    (int)Utils.unitCode.DCA,
                    GameObject.Find("Unite").transform,
                    territoires[worstIndex].position,
                    this
                ).gameObject.GetComponent<Dca>();

                newUnit.territoire = territoires[worstIndex];
                territoires[worstIndex].unites.Add(newUnit);
            }
        }

        step = 2;
        //Si la route en question est coller au territoire du joueur
        /*	if (routes) {
                GameObject subMarinPrefab = boutique.GetRightPrefab ((int)Utils.unitCode.Submarin, true);
                Transform newUnitSubMarin = boutique.CreateUnit ((int)Utils.unitCode.Submarin, GameObject.Find ("Unite").transform, newTerritoire.position, rotation);
            }	*/
    }

	/// <summary>
	/// Attaque de l'IA lorsqu'il a la main contre les territoires enemies. 
	/// </summary>
	public IEnumerator IAAttack(){
        bool done = false;
        step = 3;

        // Tant qu'on en a pas terminé
        while (!done)
        {
            // Les territoires dont les unités peuvent attaquer
            List <Territoire> territoiresAttaquants = TerritoiresAttaquants();
            Territoire toAttack = null;

            if (territoiresAttaquants.Count > 0)
                 toAttack = ChoseTarget(territoiresAttaquants);

            if(toAttack != null)
            {
                bool attacking = false;
                List<Unite> unitToAttack = new List<Unite>();
                
                unitToAttack = GetUnitToAttack(toAttack);
                attacking = unitToAttack.Count > 0;

                foreach (Unite unite in unitToAttack)
                    unite.selectedList = unitToAttack;

                // Lancement de l'attaque
                if (attacking && unitToAttack.Count > 0)
                {
                    // Debug.Log("Salut ! Tu viens pour l'aventure ? uTA.Count : "+unitToAttack.Count);
                    yield return StartCoroutine(battleManager.LaunchAttack(unitToAttack, toAttack));
                    // Debug.Log("Mes amis, la porte est ouverte.");
                }
                else if (territoiresAttaquants.Count < 1)
                    done = true;
            }
            else
                done = true;

            yield return null;
        }
	}

	/// <summary>
	/// Renforcement des territoires en déplaçant les unités de l'IA d'un territoire à l'autre.
	/// </summary>
	public void IADefense(){
        /*
		unite.isMovable = false;
		monVoisin = voisins [0];
		//while (!unite.isMovable) {
			if (monVoisin.couleur == territoire.couleur) {
				for (int i = 0; i < voisins.Count; i++) {

					if (voisins [i].unity.Count <= voisins [i + 1].unity.Count) {
						monVoisin = voisins [i];
					} else {
						monVoisin = voisins [i + 1];
					}
				}
				unite.MoveUnit (monVoisin, voisinage);
			}
		//}
        */
	}

    protected List<Unite> GetUnitToAttack(Territoire toAttack)
    {
        List<Territoire>.Enumerator voisins3DegresEnum = toAttack.GetVoisinsNDegree(3, true, true).GetEnumerator();
        List<Territoire>.Enumerator voisins2DegresEnum = toAttack.GetVoisinsNDegree(2, false, true).GetEnumerator();
        List<Territoire>.Enumerator voisinsEnum = toAttack.GetVoisinsNDegree(1, true, false).GetEnumerator();
        bool attacking = false;

        List<Unite> unitToAttack = new List<Unite>();

        // S'il y a plus d'une unité sur la cible il est possible d'attaquer à distance
        if (toAttack.unites.Count > 1)
        {
            // S'il est possible d'attaquer avec au moins un bombardier on les sélectionne
            while (!attacking && voisins3DegresEnum.MoveNext())
            {
                if (voisins3DegresEnum.Current.joueur.Equals(this))
                {
                    List<Unite> unitesVoisin = voisins3DegresEnum.Current.unites;
                    attacking = true;

                    foreach (Unite unit in unitesVoisin)
                    {
                        if (unit.GetType().Name == "Bombardier" && unit.canAct)
                        {
                            unitToAttack.Add(unit);
                            unit.selected = true;
                        }

                    }
                }
            }

            // Il n'y a pas de bombardier à proximité alors il faut chercher les éventuelles artilleries
            if (!attacking)
            {
                while (!attacking && voisins2DegresEnum.MoveNext())
                {
                    List<Unite> unitesVoisin = voisins2DegresEnum.Current.unites;
                    attacking = true;

                    foreach (Unite unit in unitesVoisin)
                    {
                        if (unit.GetType().Name == "Artillerie" && unit.canAct)
                        {
                            unitToAttack.Add(unit);
                            unit.selected = true;
                        }
                    }
                }
            }
        }

        // Il n'y a pas non plus d'artillerie il faut se rabattre sur les unités d'assaut
        if (!attacking)
        {

            while (!attacking && voisinsEnum.MoveNext())
            {
                // Il s'agit d'un territoire de l'IA dont c'est le tour et plus d'une d'unité y est stationnée
                // ces dernières sont ajoutées à la liste de celles partant à l'assaut, sauf une.
                if (voisinsEnum.Current.joueur.Equals(this) && voisinsEnum.Current.unites.Count > 1)
                {
                    List<Tank> tanks = new List<Tank>();
                    List<Infanterie> infanteries = new List<Infanterie>();

                    foreach (Unite unit in voisinsEnum.Current.unites)
                    {
                        if (unit.GetType().Name == "Infanterie")
                            infanteries.Add(unit as Infanterie);
                        else if (unit.GetType().Name == "Tank")
                            tanks.Add(unit as Tank);
                    }

                    foreach (Tank tank in tanks)
                        if (unitToAttack.Count < voisinsEnum.Current.unites.Count - 1 && tank.canAct)
                        {
                            unitToAttack.Add(tank);
                            tank.selected = true;
                        }

                    foreach (Infanterie infanterie in infanteries)
                        if (unitToAttack.Count < voisinsEnum.Current.unites.Count - 1 && infanterie.canAct)
                        {
                            unitToAttack.Add(infanterie);
                            infanterie.selected = true;
                        }

                    if (unitToAttack.Count > 0)
                        attacking = true;
                }
            }
        }

        return unitToAttack;
    }

    /// <summary>
    /// Liste les territoires du joueurs qui peuvent donner l'assaut.
    /// </summary>
    /// <returns>List(Territoire) La liste des territoires pouvant donner l'assaut</returns>
    protected List<Territoire> TerritoiresAttaquants()
    {
        List<Territoire> result = new List<Territoire>();

        // Pour chaque territoire du joueur, il faut vérifier qu'il est possible d'attaquer
        foreach (KeyValuePair<int, Territoire> keyValue in territoires)
        {
            List<Unite>.Enumerator unitesEnum = keyValue.Value.unites.GetEnumerator();
            bool canAtt = false;

            while (!canAtt && unitesEnum.MoveNext())
            {

                if (unitesEnum.Current.canAct)
                {

                    // S'il s'agit d'une artillerie ou d'un bombardier, on vérifie qu'au moins un territoire ennemi possède au moins une unité
                    if (unitesEnum.Current.GetType().Name == "Artillerie" || unitesEnum.Current.GetType().Name == "Bombardier")
                    {
                        List<Territoire>.Enumerator voisinsEnum;

                        if (unitesEnum.Current.GetType().Name == "Artillerie")
                            voisinsEnum = keyValue.Value.GetVoisinsNDegree(2, false, true).GetEnumerator();
                        else
                            voisinsEnum = keyValue.Value.GetVoisinsNDegree(3, true, true).GetEnumerator();

                        // On peut attaquer si le territoire n'est pas au joueur et qu'il possède au moins une unité
                        while (!canAtt && voisinsEnum.MoveNext())
                            canAtt = !voisinsEnum.Current.joueur.Equals(this) && voisinsEnum.Current.unites.Count > 1;
                    }
                    else
                    {
                        // S'il y a plus d'une unité sur le territoire, l'assaut peut être donné
                        if (keyValue.Value.unites.Count > 1)
                        {
                            List<Territoire>.Enumerator voisinsEnum = keyValue.Value.GetVoisinsNDegree(1, true, false).GetEnumerator();

                            // L'assaut peut être donné s'il y a au moins un voisin qui appartienne à l'ennemi
                            while (!canAtt && voisinsEnum.MoveNext())
                            {
                                canAtt = !voisinsEnum.Current.joueur.Equals(this);
                            }
                        }
                    }
                }
            }

            if (canAtt)
                result.Add(keyValue.Value);
        }

        return result;
    }

    /// <summary>
    /// Choisi un territoire à attaquer.
    /// </summary>
    /// <param name="startingTerritories">List(Territoire) Les territoires desquels peuvent partir les attaques.</param>
    /// <returns>Territoire la cible choisie</returns>
    protected Territoire ChoseTarget(List<Territoire> startingTerritories)
    {
        Dictionary<int, int> poids = new Dictionary<int, int>();
        List<Territoire> ciblesPotentielles = new List<Territoire>();
        int worstIndex = -1;

        // Liste des cibles potentielles et de leurs poids.
        foreach (Territoire startingFrom in startingTerritories)
        {
            bool getAerien = false, getArti = false, getAssault = false;
            List<Unite>.Enumerator unitesEnum = startingFrom.unites.GetEnumerator();

            // Tant qu'il n'y a pas confirmation de la présence d'au moins un bombardier, une artillerie et une unité d'assaut
            // et que toutes les unités du territoires n'ont pas été scrutés...
            while ((!getAerien || !getArti || !getAssault) && unitesEnum.MoveNext())
            {
                if (unitesEnum.Current.canAct)
                {
                    if (unitesEnum.GetType().Name == "Bombardier")
                        getAerien = true;
                    else if (unitesEnum.GetType().Name == "Artillerie")
                        getArti = true;
                    else if (unites.GetType().Name != "Dca")
                        getAssault = true;
                }
            }

            List<Territoire> voisins;

            // S'il y a un bombardier il est possible d'attaquer jusqu'aux voisins du 3ème degré.
            if (getAerien)
            {
                voisins = startingFrom.GetVoisinsNDegree(3, true, true);

                foreach (Territoire voisin in voisins)
                    if (!voisin.joueur.Equals(this) && voisin.unites.Count > 1 && !ciblesPotentielles.Contains(voisin))
                    {
                        int score = 0;

                        foreach (Unite unit in voisin.unites)
                        {
                            if (unit.GetType().Name == "Infanterie")
                                score += 4;
                            else if (unit.GetType().Name == "Tank")
                                score += 10;
                            else if (unit.GetType().Name == "Artillerie")
                                score += 7;
                            else if (unit.GetType().Name == "Dca")
                                score += 4;
                            else
                                score += 5;
                        }

                        poids.Add(voisin.Num, score);
                        ciblesPotentielles.Add(voisin);
                    }
            }
            else if (getArti) // Et sinon, s'il y a une artillerie il est possible d'attaquer jusqu'aux voisins du 2nd degré.
            {
                voisins = startingFrom.GetVoisinsNDegree(2, true, true);

                foreach (Territoire voisin in voisins)
                    if (!voisin.joueur.Equals(this) && voisin.unites.Count > 1 && !ciblesPotentielles.Contains(voisin))
                    {
                        int score = 0;

                        foreach (Unite unit in voisin.unites)
                        {
                            if (unit.GetType().Name == "Infanterie")
                                score += 4;
                            else if (unit.GetType().Name == "Tank")
                                score += 10;
                            else if (unit.GetType().Name == "Artillerie")
                                score += 7;
                            else if (unit.GetType().Name == "Dca")
                                score += 4;
                            else
                                score += 5;
                        }

                        poids.Add(voisin.Num, score);
                        ciblesPotentielles.Add(voisin);
                    }
            }
            else if (getAssault) // Enfin, s'il y a une unité d'assaut il est possible d'attaquer le voisin.
            {
                voisins = startingFrom.GetVoisinsNDegree(1, true, false);

                foreach (Territoire voisin in voisins)
                    if (!voisin.joueur.Equals(this) && !ciblesPotentielles.Contains(voisin))
                    {
                        int score = 0;

                        foreach (Unite unit in voisin.unites)
                        {
                            if (unit.GetType().Name == "Infanterie")
                                score += 4;
                            else if (unit.GetType().Name == "Tank")
                                score += 10;
                            else if (unit.GetType().Name == "Artillerie")
                                score += 7;
                            else if (unit.GetType().Name == "Dca")
                                score += 4;
                            else
                                score += 5;
                        }

                        poids.Add(voisin.Num, score);
                        ciblesPotentielles.Add(voisin);
                    }
            }
        }

        if (ciblesPotentielles.Count > 0)
        {
            // Sélection du territoire à attaquer
            foreach (Territoire cible in ciblesPotentielles)
            {
                if (worstIndex == -1)
                    worstIndex = cible.Num;
                else if (poids[cible.Num] < poids[worstIndex])
                {
                    worstIndex = cible.Num;
                }
            }

            return coloredTerritories.Territoires[worstIndex - 1];
        }
        else
            return null;
    }
}
