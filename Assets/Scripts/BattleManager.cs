using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BattleManager : MonoBehaviour {

    /// <summary>
    /// Le territoire cible.
    /// </summary>
    private Territoire territoireCible = null;

    /// <summary>
    /// La route cible.
    /// </summary>
    private Route routeCible = null;

    ///<summary>Tableaux récapitulatif des unités perdues par l'attaquant.</summary>
    private int[] unitesDetruitesAtt = new int[] { 0, 0, 0, 0, 0, 0, 0 };

    ///<summary>Tableaux récapitulatif des unités perdues par le défenseur.</summary>
    private int[] unitesDetruitesDef = new int[] { 0, 0, 0, 0, 0, 0, 0 };

    /// <summary>Les unités pour une attaque terrestre ou aérienne.</summary>
    private List<Unite> unitAtt;

    /// <summary>Les unités du défenseur.</summary>
    private List<Unite> unitDef;

    /// <summary>Les sous-marins de l'attaquant</summary>
    private List<Submarin> subAtt;

    /// <summary>Le joueur qui attaque.</summary>
    private Joueur joueurAtt;

    /// <summary>Le joueur qui défend.</summary>
    private Joueur joueurDef;

    private void Start()
    {
        unitDef = new List<Unite>();
        unitAtt = new List<Unite>();
        subAtt = new List<Submarin>();
    }

    /// <summary>
    /// Lance l'attaque sur le territoire cible.
    /// </summary>
    /// <param name="unitAttacking">List(Unite) Les unités donnant l'assaut.</param>
    /// <param name="cible">Territoire Le territoire cible.</param>
    public IEnumerator LaunchAttack(List<Unite> unitAttacking, Territoire cible)
    {
        unitAtt = new List<Unite>();

        foreach (Unite unite in unitAttacking)
            unitAtt.Add(unite);

        //Debug.Log("unitAtt.Count : " + unitAtt.Count);

        territoireCible = cible;
        joueurAtt = unitAtt[0].Joueur;
        joueurDef = territoireCible.joueur;
        bool ranged = false;

        if (unitAtt[0].GetType().Name == "Artillerie" || unitAtt[0].GetType().Name == "Bombardier")
            ranged = true;

        // Récupération des unités présentes sur la cible
        foreach(Unite unit in cible.joueur.Unites)
        {
            if(unit.GetType().BaseType.Name == "Terrestre")
            {
                Terrestre terrestre = unit as Terrestre;

                if (terrestre.territoire.Equals(cible))
                    unitDef.Add(terrestre);
            }
            else if(unit.GetType().BaseType.Name == "Aerienne")
            {
                Aerienne aerienne = unit as Aerienne;

                if (aerienne.territoire.Equals(cible))
                    unitDef.Add(aerienne);
            }
        }

        int[] diceAtt = new int[Math.Min(3, unitAtt.Count)], diceDef = new int[Math.Min(2, unitDef.Count)];
        System.Random rand = new System.Random();

        // Jets de dés
        for(int i = 0; i < 3; i++)
        {
            if (i < Math.Min(3, unitAtt.Count))
                diceAtt[i] = rand.Next(1, 7); // Tire un chiffre entre 1 et 7 non inclus

            if (i < Math.Min(2, unitDef.Count))
                diceDef[i] = rand.Next(1, 7);
        }

        DestroyUnits(diceAtt, diceDef, ranged);
        
        // Si toutes les unités de la défense sont détruites, le territoire est conquit
        if (unitAtt.Count > 0 && unitDef.Count == 0)
        {
            territoireCible.joueur.Territoires.Remove(territoireCible.Num);
            territoireCible.joueur = joueurAtt;
            joueurAtt.Territoires.Add(territoireCible.Num, territoireCible);
            territoireCible.unites.Clear();

            if (!ranged)
            {
                foreach (Unite unit in unitAtt)
                {
                    if (unit.GetType().Name == "Infanterie" || unit.GetType().Name == "Tank")
                    {
                        Terrestre terrestre = unit as Terrestre;

                        terrestre.territoire.unites.Remove(terrestre);
                        terrestre.territoire = territoireCible;
                        territoireCible.unites.Add(unit);

                        if (joueurAtt.GetType().Name.Equals("IAEasy") || joueurAtt.GetType().Name.Equals("IAMedium") || joueurAtt.GetType().Name.Equals("IAHard"))
                            terrestre.transform.localPosition = territoireCible.Position;
                    }
                }
            }
        }
        else
        {
            foreach(Unite unit in unitAtt)
                unit.ResetPosition();

            if (ranged)
                foreach (Unite unit in unitAtt)
                    unit.Disable();
        }

        ////Debug.Log("Ça se voit.");

        // Affichage du résultat de l'assaut.
        yield return StartCoroutine(GameObject.Find("GUIManager").GetComponent<GUIController>().ShowAssaultResult(diceAtt, diceDef, unitesDetruitesAtt, unitesDetruitesDef, unitAtt, unitDef));

        //Debug.Log("Eh bien, je frappe pour qu'on vienne nous ouvrir.");

        // Réinitialisation des variables pour éviter toute erreur.
        Reset();
    }

    /// <summary>
    /// Lance l'attaque sur le territoire cible.
    /// </summary>
    /// <param name="unitAttacking">List(Submarin) Les sous-marins donnant l'assaut.</param>
    /// <param name="cible">Territoire Le territoire cible.</param>
    /// <param name="targetsIds">int[] Les index des unités ciblées dans la liste d'unités du défenseur.</param>
    public IEnumerator LaunchAttack(List<Submarin> unitAttacking, Route cible, int[] targetsIds)
    {
        routeCible = cible;
        subAtt = unitAttacking;
        joueurAtt = subAtt[0].Joueur;
        joueurDef = cible.joueur;

        // Récupération des unités présentes sur la cible
        foreach (Unite unit in cible.joueur.Unites)
        {
            if (unit.GetType().BaseType.Name == "Maritime")
            {
                Maritime maritime= unit as Maritime;

                if (maritime.route.Equals(cible))
                    unitDef.Add(maritime);
            }
        }

        int[] diceAtt = new int[Math.Min(3, subAtt.Count)], diceDef = new int[Math.Min(2, unitDef.Count)];
        System.Random rand = new System.Random();

        // Jets de dés
        for (int i = 0; i < 3; i++)
        {
            if (i < Math.Min(3, subAtt.Count))
                diceAtt[i] = rand.Next(1, 7); // Tire un chiffre entre 1 et 7 non inclus

            if (i < Math.Min(2, unitDef.Count))
                diceDef[i] = rand.Next(1, 7);
        }

        DestroyUnits(diceAtt, diceDef, targetsIds);

        foreach (Submarin unit in subAtt)
        {
            unit.Disable();
            unitAtt.Add(unit);
        }

        // Si toutes les unités de la défense sont détruites, la route est à nouveau libre.
        if (unitDef.Count == 0 && unitAtt.Count > 0)
        {
            joueurDef.Routes.Remove(cible);
            joueurAtt.Routes.Add(cible);

            foreach (Submarin unit in subAtt)
                unit.route = cible;
        }
        else
            unitAtt[0].ResetPosition();
        
        // Affichage du résultat de l'assaut.
        yield return StartCoroutine(GameObject.Find("GUIManager").GetComponent<GUIController>().ShowAssaultResult(diceAtt, diceDef, unitesDetruitesAtt, unitesDetruitesDef, unitAtt, unitDef));

        // Réinitialisation des variables pour éviter toute erreur.
        Reset();
    }

    /// <summary>
    /// Détruit les unités terrestres et aériennes en fonction du résultat des dés.
    /// </summary>
    /// <param name="diceAtt">int[] Les dés de l'attaquant.</param>
    /// <param name="diceDef">int[] Les dés du défenseur</param>
    /// <param name="ranged">bool Variable d'état définissant si l'attaque est à distance ou non.</param>
    private void DestroyUnits(int[] diceAtt, int[] diceDef, bool ranged)
    {
        int toDestroyAtt = 0, toDestroyDef = 0;
        int totalAtt = 0, totalDef = 0, i = 0;

        // Tri dans l'ordre croissant
        Array.Sort(diceAtt);
        Array.Sort(diceDef);

        // Comparaison des plus gros chiffres tirés par chaque joueur
        if (diceAtt[diceAtt.Length - 1] > diceDef[diceDef.Length - 1])
            toDestroyDef++;
        else
            toDestroyAtt++;


        // Comparaison des seconds plus gros chiffres tirés par chaque joueur
        if (diceAtt.Length > 1 && diceDef.Length > 1)
        {
            if (diceAtt[diceAtt.Length - 2] > diceDef[diceDef.Length - 2])
                toDestroyDef++;
            else
                toDestroyAtt++;
        }

        // Somme des dés de l'attaque
        for (i = 0; i < diceAtt.Length; i++)
            totalAtt += diceAtt[i];

        // Somme des dés de la défense
        for (i = 0; i < diceDef.Length; i++)
            totalDef += diceDef[i];

        // Si la somme de l'attaquant est supérieure au défenseur, on détruit un tank du défenseur s'il y en a
        if (totalAtt > totalDef)
            unitDef = DestroyTank(unitDef, true);
        else// Autrement, c'est un tank de l'attaquant
            unitAtt = DestroyTank(unitAtt, false);

        List<Dca> dcasInRange = territoireCible.GetDcaInRange();

        if (toDestroyAtt > 0 && unitAtt.Count > 0 && (!ranged || (unitAtt[0].GetType().Name == "Bombardier" && dcasInRange.Count > 0)))
        {
            List<Unite>.Enumerator unitesEnum = unitAtt.GetEnumerator();
            List<Unite> toDestroy = new List<Unite>();

            // On ne peut détruire plus de bombardier qu'il n'y a de DCA.
            // Si unitAtt[0] == null alors il s'agissait d'un tank donc la liste ne contient pas de bombardier
            if (unitAtt[0] != null && unitAtt[0].GetType().Name == "Bombardier")
                toDestroyAtt = Math.Min(toDestroyAtt, dcasInRange.Count);

            while (toDestroyAtt > 0 && unitesEnum.MoveNext())
            {
                if (unitesEnum.Current.GetType().Name != "Tank")
                {
                    if (unitesEnum.Current.GetType().Name == "Bombardier")
                        unitesDetruitesAtt[(int)Utils.unitCode.Bombardier]++;

                    if (unitesEnum.Current.GetType().Name == "Infanterie")
                        unitesDetruitesAtt[(int)Utils.unitCode.Infanterie]++;

                    foreach (Unite unit in unitAtt)
                        if (!unit.Equals(unitesEnum.Current))
                            unit.RemoveFromSelection(unitesEnum.Current);

                    toDestroy.Add(unitesEnum.Current);

                    --toDestroyAtt;
                }
            }

            foreach (Unite unit in toDestroy)
            {
                if(unit.GetType().Name == "Bombardier")
                {
                    Bombardier bomber = unit as Bombardier;

                    bomber.territoire.unites.Remove(bomber);
                }

                if(unit.GetType().Name == "Infanterie")
                {
                    Infanterie infanterie = unit as Infanterie;

                    infanterie.territoire.unites.Remove(infanterie);
                }

                unit.Joueur.Unites.Remove(unit);
                unitAtt.Remove(unit);
                Destroy(unit.gameObject);
            }

        }

        // Si c'est une attaque à distance, on laisse une unité en vie.
        int minUnitToLeaveAlive = ranged ? 1 : 0;

        if (toDestroyDef > minUnitToLeaveAlive)
        {
            List<Bombardier> bombardiers = new List<Bombardier>();
            List<Dca> dcas = new List<Dca>();
            List<Artillerie> artilleries = new List<Artillerie>();
            List<Infanterie> infanteries = new List<Infanterie>();

            foreach (Unite unit in unitDef)
            {
                if (unit.GetType().Name == "Bombardier")
                    bombardiers.Add(unit as Bombardier);

                if (unit.GetType().Name == "Dca")
                    dcas.Add(unit as Dca);

                if (unit.GetType().Name == "Artillerie")
                    artilleries.Add(unit as Artillerie);

                if (unit.GetType().Name == "Infanterie")
                    infanteries.Add(unit as Infanterie);
            }

            List<Bombardier>.Enumerator bombardiersEnum = bombardiers.GetEnumerator();
            List<Dca>.Enumerator dcasEnum = dcas.GetEnumerator();
            List<Artillerie>.Enumerator artilleriesEnum = artilleries.GetEnumerator();
            List<Infanterie>.Enumerator infanteriesEnum = infanteries.GetEnumerator();
            List<Unite> toDestroy = new List<Unite>();

            while (toDestroyDef > minUnitToLeaveAlive && bombardiersEnum.MoveNext())
            {
                unitesDetruitesDef[(int)Utils.unitCode.Bombardier]++;
                bombardiersEnum.Current.territoire.unites.Remove(bombardiersEnum.Current);
                toDestroy.Add(bombardiersEnum.Current);

                --toDestroyDef;
            }

            while (toDestroyDef > minUnitToLeaveAlive && dcasEnum.MoveNext())
            {
                unitesDetruitesDef[(int)Utils.unitCode.DCA]++;
                dcasEnum.Current.territoire.unites.Remove(dcasEnum.Current);
                toDestroy.Add(dcasEnum.Current);

                --toDestroyDef;
            }

            while (toDestroyDef > minUnitToLeaveAlive && artilleriesEnum.MoveNext())
            {
                unitesDetruitesDef[(int)Utils.unitCode.Artillerie]++;
                artilleriesEnum.Current.territoire.unites.Remove(artilleriesEnum.Current);
                toDestroy.Add(artilleriesEnum.Current);

                --toDestroyDef;
            }

            while (toDestroyDef > minUnitToLeaveAlive && infanteriesEnum.MoveNext())
            {
                unitesDetruitesDef[(int)Utils.unitCode.Infanterie]++;
                infanteriesEnum.Current.territoire.unites.Remove(infanteriesEnum.Current);
                toDestroy.Add(infanteriesEnum.Current);

                --toDestroyDef;
            }

            foreach (Unite unit in toDestroy)
            {
                unit.Joueur.Unites.Remove(unit);
                unitDef.Remove(unit);
                Destroy(unit.gameObject);
            }
        }
    }


    /// <summary>
    /// Détruit les unités maritimes en fonction du résultat des dés.
    /// </summary>
    /// <param name="diceAtt">int[] Les dés de l'attaquant.</param>
    /// <param name="diceDef">int[] Les dés du défenseur.</param>
    /// <param name="targetsIds">int[] les index des cibles dans la liste des unités du défenseur.</param>
    private void DestroyUnits(int[] diceAtt, int[] diceDef, int[] targetsIds)
    {
        int toDestroyAtt = 0, toDestroyDef = 0;

        // Tri dans l'ordre croissant
        Array.Sort(diceAtt);
        Array.Sort(diceDef);

        // Comparaison des plus gros chiffres tirés par chaque joueur
        if (diceAtt[diceAtt.Length - 1] > diceDef[diceDef.Length - 1])
            toDestroyDef++;
        else
            toDestroyAtt++;


        // Comparaison des seconds plus gros chiffres tirés par chaque joueur
        if (diceAtt.Length > 1 && diceDef.Length > 1)
        {
            if (diceAtt[diceAtt.Length - 2] > diceDef[diceDef.Length - 2])
                toDestroyDef++;
            else
                toDestroyAtt++;
        }

        List<Submarin> toDestroy = new List<Submarin>();
        int i = 0;

        while (i < toDestroyAtt)
        {
            unitesDetruitesAtt[(int)Utils.unitCode.Submarin]++;

            foreach (Submarin sub in subAtt)
                if (!sub.Equals(subAtt[i]))
                    sub.RemoveFromSelection(subAtt[i]);

            toDestroy.Add(subAtt[i]);
            i++;
        }

        foreach (Submarin sub in toDestroy)
        {
            sub.Joueur.Unites.Remove(sub);
            sub.route.unites.Remove(sub);
            subAtt.Remove(sub);
            Destroy(sub.gameObject);
        }

        i = 0;
        List<Maritime> maritimes = new List<Maritime>(toDestroyDef);
        
        while(i < toDestroyDef)
        {
            Unite unit = routeCible.joueur.Unites[targetsIds[i]];
            Maritime maritime = unit as Maritime;

            if (unit.GetType().Name == "Croiseur")
                unitesDetruitesDef[(int) Utils.unitCode.Croiseur]++;
            else
                unitesDetruitesDef[(int) Utils.unitCode.Submarin]++;
            
            maritimes.Add(maritime);
            i++;
        }

        foreach(Maritime current in maritimes)
        {
            current.route.unites.Remove(current);
            current.Joueur.Unites.Remove(current);
            unitDef.Remove(current);
            Destroy(current.gameObject);
        }
    }

    /// <summary>
    /// Détruit un tank s'il y en a dans la liste.
    /// </summary>
    /// <param name="unites">List(Unites) La liste des unités dans laquelle bouclée.</param>
    /// <param name="def">bool True si c'est une unité défensive, false sinon.</param>
    private List<Unite> DestroyTank(List<Unite> unites, bool def)
    {
        List<Unite>.Enumerator unitEnum = unites.GetEnumerator();
        Tank toDestroy = null;

        while(toDestroy == null && unitEnum.MoveNext())
        {
            if(unitEnum.Current.GetType().Name == "Tank")
            {
                Tank tank = unitEnum.Current as Tank;

                if (territoireCible.Equals(tank.territoire))
                    unitesDetruitesDef[(int)Utils.unitCode.Tank]++;
                else
                    unitesDetruitesAtt[(int)Utils.unitCode.Tank]++;

                toDestroy = tank;
            }
        }

        if (toDestroy != null)
        {
            if (def)
                unitDef.Remove(toDestroy);
            else
                unitAtt.Remove(toDestroy);

            // On retire le tank de la liste d'unités sélectionnées pour ne pas avoir d'erreur.
            if (def)
            {
                foreach (Unite unit in unitDef)
                    unit.RemoveFromSelection(toDestroy);
            }
            else
            {
                foreach (Unite unit in unitAtt)
                    unit.RemoveFromSelection(toDestroy);
            }

            toDestroy.territoire.unites.Remove(toDestroy);
            toDestroy.Joueur.Unites.Remove(toDestroy);
            unites.Remove(toDestroy);
            Destroy(toDestroy.gameObject);
        }

        return unites;
    }

    private void Reset()
    {
        territoireCible = null;
        unitAtt = new List<Unite>();
        routeCible = null;
        unitDef = new List<Unite>();
        unitesDetruitesAtt = new int[] { 0, 0, 0, 0, 0, 0, 0 };
        unitesDetruitesDef = new int[] { 0, 0, 0, 0, 0, 0, 0 };
        subAtt = new List<Submarin>();
        joueurAtt = null;
        joueurDef = null;
    }
}
