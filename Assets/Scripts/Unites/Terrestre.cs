using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Terrestre : Unite {

    public Territoire territoire;

    public override bool CanBeSelected(Joueur joueurActif, TurnManager.phases phaseActive)
    {
        if(phaseActive == TurnManager.phases.Deploiement)
            return joueurActif.Equals(joueur) && territoire == null && canAct;
        else
            return joueurActif.Equals(joueur) && canAct;
    }

    /// <summary>
    /// Retourne moveTo s'il est possible à l'unité de s'y déplacer, un territoire adjacent si voisinage = true ou null s'il n'est pas possible de déplacer l'unité.
    /// </summary>
    /// <param name="territoire">Territoire Le territoire sur lequel se déplacer</param>
    /// <param name="voisinage">bool true pour déplacer l'unité sur un territoire adjacent à moveTo, false pour déplacer l'unité sur moveTo</param>
    /// <returns>Territoire Le territoire sur lequel se déplacer si c'est possible, null si aucun ne correspond</returns>
    protected override Territoire FindDestination(Territoire moveTo, bool voisinage)
    {
        // L'unité peut encore se déplacer
        if (canAct)
        {
            // Le territoire est au joueur, on peut y stationner l'unité, ou c'est un voisin que l'on cherche
            if (moveTo.joueur == joueur || voisinage)
            {
                // L'unité vient d'être acheté, on la place sur le territoire sélectionné
                if (territoire == null)
                    return moveTo;
                else // L'unité a déjà été placée
                {
                    bool found = false;
                    List<Territoire>.Enumerator territoriesEnumerator = territoire.Voisins.GetEnumerator();

                    // On cherche si le territoire cible est dans le voisinage direct du territoire de départ
                    while (!found && territoriesEnumerator.MoveNext())
                    {
                        if (territoriesEnumerator.Current == moveTo)
                            found = true;
                    }

                    // On ne l'a pas trouvé alors on voit si la destination est connecté par une route au territoire de départ
                    if (!found)
                    {
                        List<Route>.Enumerator routesEnumerator = territoire.Routes.GetEnumerator();

                        while (!found && routesEnumerator.MoveNext())
                        {
                            List<Territoire> connecteds = routesEnumerator.Current.Territoires;
                            foreach (Territoire connected in connecteds)
                            {
                                // On a trouvé la destination et elle n'est pas bloquée par un barrage maritime, l'unité peut donc s'y déplacer
                                if (connected == moveTo || routesEnumerator.Current.joueur == joueur || routesEnumerator.Current.joueur == null)
                                    found = true;
                            }
                        }
                    }

                    // Toujours pas trouvé ?! Parcours de graphe time !
                    if (!found && voisinage)
                    {
                        // Les territoires à regarder
                        List<Territoire> toVisit = new List<Territoire> {
                            territoire
                        };
                        List<Territoire> visited = new List<Territoire>(); // Les territoires déjà vus
                        int i = -1;

                        // On parcourt maintenant les territoires alliés dans l'ordre de découverte en ajoutant ceux connectés par une route maritime
                        // qui ne soit pas occupée par un ennemie jusqu'à trouver le territoire ou ne plus pouvoir visiter d'autre territoire
                        while (!found && i++ < toVisit.Count)
                        {
                            // On a trouvé la destination, l'unité peut donc s'y déplacer
                            if (toVisit[i] == moveTo)
                            {
                                // On cherche cependant un territoire allié adjacent
                                if (voisinage)
                                {
                                    List<Territoire>.Enumerator enumerator = toVisit[i].Voisins.GetEnumerator();

                                    while (!found && enumerator.MoveNext())
                                    {
                                        if (enumerator.Current.joueur == joueur)
                                            return enumerator.Current;
                                    }

                                    InvalidPositioning("Il n'y a pas de territoire adjacent sur lequel placer cette unité.");
                                    return null;
                                }

                                found = true;
                            }
                            else
                            {

                                visited.Add(toVisit[i]);

                                // On ajoute les voisins direct du territoire visité s'ils appartiennent au joueur
                                foreach (Territoire ter in toVisit[i].Voisins)
                                {
                                    // Le territoire n'est pas celui de départ et n'est ni dans ceux déjà visités ou à visiter
                                    if (ter != territoire && !toVisit.Contains(ter) && !visited.Contains(ter) && ter.joueur == joueur)
                                        toVisit.Add(ter);
                                }

                                // On regarde également les territoires connectés par une route maritime
                                foreach (Route route in toVisit[i].Routes)
                                {
                                    // Si la route n'est pas bloquée par un barrage ennemie
                                    if (route.joueur == joueur || route.joueur == null)
                                    {
                                        foreach (Territoire ter in route.Territoires)
                                        {
                                            // Le territoire n'est pas celui de départ et n'est ni dans ceux déjà visités ou à visiter
                                            if (ter != territoire && !toVisit.Contains(ter) && !visited.Contains(ter))
                                                toVisit.Add(ter);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    if (found)
                        return moveTo;
                }
            }
            else
                InvalidPositioning("Vous ne pouvez placer une unité sur un territoire ennemie");
        }
        else
            InvalidPositioning("Une ou plusieurs unité(s) sélectionnée(s) ne peu(ven)t plus se déplacer");

        return null;
    }

    /// <summary>
    /// Fonction permettant de déplacer l'unité sur le territoire 
    /// </summary>
    /// <param name="moveTo">Territoire Le territoire sur lequel se déplacer</param>
    /// <returns>bool true si l'unité a pu se déplacer avec succès, false sinon</returns>
    public override void MoveUnit(Territoire moveTo, bool voisinage = false) // TODO À tester
    {
        Territoire destination = FindDestination(moveTo, voisinage);

        if(destination != null)
        {
            territoire = destination;
            territoire.unites.Add(this);

            // Les unités ne peuvent plus être jouées si elles sont déplacées en-dehors de la phase de déploiement.
            if (GameObject.Find("GUIManager").GetComponent<TurnManager>().PhaseActive != TurnManager.phases.Deploiement)
                Disable();
            else
                RemoveSelection();
        }
    }


    /// <summary>
    /// Fonction permettant d'attaquer un territoire.
    /// Cette fonction sera partagée par l'infanterie et le tank et réécrite pour l'artillerie.
    /// </summary>
    /// <param name="territoire">Territoire Le territoire à attaquer</param>
    public override void Attack(Territoire toAttack)
    {
        if (territoire == null)
            InvalidPositioning("Vous ne pouvez pas attaquer un territoire sans au préalable déployer votre unité.");
        else if (toAttack.joueur != joueur)
        {
            if (CanAttackTogether(toAttack))
            {
                battleManager.LaunchAttack(selectedList, toAttack);
            }
            else
                InvalidPositioning("Certaines unités ne peuvent attaquer la cible.");
        }
        else
            InvalidPositioning("Vous ne pouvez attaquer votre propre territoire.");
    }

    /// <summary>
    /// Indique si les unités sélectionnées peuvent attaquer ensemble. Pour cela, elles doivent être 
    /// </summary>
    /// <returns>bool true si les unités sont compatibles, false autrement.</returns>
    protected bool CanAttackTogether(Territoire toAttack)
    {
        if (this.GetType().Name == "Infanterie" || this.GetType().Name == "Tank")
        {
            // On vérifie pour chaque unité si elle est d'un type compatible (Tank ou Infanterie) et stationnée à la 
            // frontière de la cible
            foreach (Unite unit in selectedList)
            {
                
                if (unit.GetType().Name == "Infanterie" || unit.GetType().Name == "Tank")
                {
                    Terrestre terrestre = unit as Terrestre;

                    // Une des unités sélectionnées n'est pas stationnée à la frontière de la cible
                    // Si une route maritime qui ne soit pas bloquée ne connecte pas la cible à la base de l'unité
                    // Elle ne peut attaquer
                    if (!toAttack.GetVoisinsNDegree(1, true, false).Contains(terrestre.territoire))
                        return false;
                }
                else // Si l'unité n'est pas une infanterie ou un tank, le groupe ne peut attaquer de façon coordonnées
                    return false;
            }
        }
        else if (this.GetType().Name == "Artillerie")
        {
            if(toAttack.unites.Count > 1)
            {
                // On vérifie pour chaque unité si elle est du même type et stationnée à portée de tire de la cible.
                foreach (Unite unit in selectedList)
                {
                    if (unit.GetType().Name != "Artillerie")
                        return false;

                    Artillerie artie = unit as Artillerie;

                    // Si la cible n'est pas à moins de 3 territoires d'écart l'unité ne peut ouvrir le feu.
                    if (!toAttack.GetVoisinsNDegree(2, false, true).Contains(artie.territoire))
                        return false;
                }
            }
        }
        else // Une DCA ne peut pas attaquer
            return false;

        return true;
    }
}
