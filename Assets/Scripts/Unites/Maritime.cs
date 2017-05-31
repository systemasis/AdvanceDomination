using UnityEngine;
using System.Collections;

public class Maritime : Unite {

    /// <summary>La route maritime où stationne l'unité.</summary>
    public Route route;

    /// <summary>
    /// Vérifie qu'une unité peut être sélectionnée.
    /// </summary>
    /// <param name="joueurActif">Joueur Le joueur dont c'est le tour.</param>
    /// <param name="phaseActive">TurnManager.phases La phase actuelle.</param>
    /// <returns>bool True si l'unité peut être sélectionnée, false autrement.</returns>
    public override bool CanBeSelected(Joueur joueurActif, TurnManager.phases phaseActive)
    {
        if (phaseActive == TurnManager.phases.Deploiement)
            return joueurActif.Equals(joueur) && route == null && canAct;
        else
            return joueurActif.Equals(joueur) && canAct;
    }

    /// <summary>
    /// Vérifie que les unités sélectionnées peuvent toutes attaquer la cible.
    /// </summary>
    /// <param name="route">Route La route à attaquer.</param>
    /// <returns>bool True si les unités sont toutes en mesure d'attaquer, false autrement.</returns>
    protected bool CanAttackTogether(Route route)
    {
        foreach(Unite unit in selectedList)
        {
            // Seuls les sous-marins peuvent attaquer une route
            if (unit.GetType().Name != "Submarin")
                return false;

            Submarin sousmarin = unit as Submarin;

            // Si le sous-marin ne peut plus être joué ou que la cible n'est pas à sa portée, on ne peut lancer l'attaque
            if ( !(canAct && sousmarin.route.RoutesVoisines.Contains(route)))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Vérifie que l'unité peut se déplacer sur la route moveTo.
    /// </summary>
    /// <param name="moveTo">Route La destination de l'unité.</param>
    /// <returns>bool True si l'unité peut se déplacer sur la route, false autrement.</returns>
    public override bool CanMoveTo(Route moveTo)
    {
        return route == null || (route.RoutesVoisines.Contains(moveTo) && canAct && (moveTo.joueur == null || moveTo.joueur.Equals(joueur)));
    }

    /// <summary>
    /// Déplace l'unité sur moveTo.
    /// </summary>
    /// <param name="moveTo">Route La route sur laquelle placer l'unité.</param>
    public override void MoveUnit(Route moveTo) {

        route = moveTo;
        route.unites.Add(this);

        if (!joueur.Routes.Contains(moveTo))
        {
            moveTo.joueur = joueur;
            joueur.Routes.Add(moveTo);
        }

        // Les unités ne peuvent plus être jouées si elles sont déplacées en-dehors de la phase de déploiement.
        if (GameObject.Find("GUIManager").GetComponent<TurnManager>().PhaseActive != TurnManager.phases.Deploiement)
            Disable();
        else
            RemoveSelection();
    }
}
