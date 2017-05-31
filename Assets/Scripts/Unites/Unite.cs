using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Classe mère pour toutes les unités
/// 
/// Part of this code fetched from http://hyunkell.com/blog/rts-style-unit-selection-in-unity-5/
/// Credit goes to Jeff Zimmer
/// </summary>
public class Unite : MonoBehaviour
{
    /// <summary>
    /// Le cercle montrant que l'unité est sélectionnée
    /// </summary>
    public GameObject selectionCircle;

    public List<Unite>.Enumerator unitesEnum;

    /// <summary>
    /// Coordonnées du curseur dans le plan de l'écran
    /// </summary>
    private Vector3 screenPoint;

    /// <summary>
    /// Offsets de l'unité par rapport à la caméra lors d'un déplacement d'unités.
    /// </summary>
    private Vector3 offset;

    /// <summary>
    /// Liste les unités sélectionnées en plus de celle-ci
    /// </summary>
    public List<Unite> selectedList;

    /// <summary>
    /// Indique si l'unité est sélectionnée par le joueur
    /// </summary>
    public bool selected = false;

    /// <summary>
    /// Le joueur auquel appartient l'unité
    /// </summary>
    public Joueur Joueur
    {
        get
        {
            return joueur;
        }
        set
        {
            if (joueur == null)
                joueur = value;
        }
    }
    protected Joueur joueur;

    /// <summary>
    /// Material à appliquer quand l'unité peut être sélectionnée
    /// </summary>
    public Material lightMaterial;

    /// <summary>
    /// Material à appliquer quand l'unité ne peut plus être sélectionnée
    /// </summary>
    public Material darkMaterial;

    /// <summary>
    /// Indique si l'unité est déplacée
    /// </summary>
    private bool dragged = false;

    /// <summary>
    /// Variable d'état : 
    ///     - true : l'unité peut faire une action (attaquer/se déplacer)
    ///     - false : l'unité ne peut plus agir
    /// </summary>
    public bool canAct = true;

    /// <summary>
    /// Position originale avant un quelconque déplacement de l'unité.
    /// Permet d'annuler une translation si le déplacement n'et pas possible.
    /// </summary>
    public Vector3 OriginalPosition
    {
        get
        {
            return originalPosition;
        }
    }
    protected Vector3 originalPosition;

    /// <summary>
    /// Le gameobject en gérant l'affichage à l'écran.
    /// </summary>
    public GameObject guiManager;
    
    /// <summary>
    /// L'angle auquel l'unité est à l'endroit;
    /// </summary>
    public Vector3 Angle
    {
        get
        {
            return angle;
        }
    }
    protected Vector3 angle = new Vector3(-90, 0, 0);

    public BattleManager battleManager;

    public readonly float translationSpeed = 10.0f;  

    private IEnumerator Start()
    {
        guiManager = GameObject.Find("GUIManager");
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        TurnManager turnManager = guiManager.GetComponent<TurnManager>() as TurnManager;

        while(joueur == null)
        {
            yield return null;
        }

        lightMaterial = joueur.Material;
        darkMaterial = joueur.DarkMaterial;

        OnStart();
    }

    private void Update()
    {
        Vector3 position = gameObject.transform.position;

        // Si l'unité passe en-dessous du plateau on la replace au-dessus
        if (position.y < -0.1f)
            gameObject.transform.position = new Vector3(position.x, 0.1f, position.z);
    }

    // Part of the following code has been fetched from http://answers.unity3d.com/questions/12322/drag-gameobject-with-mouse.html
    // Credit goes to MarkGX http://answers.unity3d.com/users/7077/markgx.html
    void OnMouseDown()
    {
        if (!guiManager.GetComponent<GUIController>().dialogOpened) // S'il n'y a pas de fenêtre de dialogue d'ouverte
        {
            // Replace l'unité à l'endroit
            transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, angle, 1);

            if (selected)
            {
                screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

                // Listage des unités sélectionnées et enrigestrement de leurs décalages à la caméra
                foreach (Unite selectable in selectedList)
                {
                    if (selectable.selected)
                    {
                        selectable.offset = selectable.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                        selectable.originalPosition = selectable.transform.position;
                    }
                }
            }
        }
    }
    
    void OnMouseDrag()
    {
        if (!guiManager.GetComponent<GUIController>().dialogOpened) // S'il n'y a pas de fenêtre de dialogue d'ouverte
        {
            if (this.selected)
            {
                dragged = true;

                // Les coordonnées du curseur à la caméra
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

                // float step = translationSpeed * Time.deltaTime;

                // Application des changements aux autres unités sélectionnées
                foreach (Unite unit in selectedList)
                {
                    // Les coordonnées du curseur dans le "monde"
                    Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + unit.offset;

                    // Fixe la hauteur à 0.5 durant le déplacement pour éviter toute collision avec les autres pièces restées au sol
                    curPosition.y = 0.5f;

                    unit.transform.position = curPosition;

                    // selected.transform.position = Vector3.MoveTowards(selected.transform.position, curPosition, step);
                    // selected.GetComponent<Rigidbody>().MovePosition(curPosition);
                }

                // Replace l'unité à l'endroit
                transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, angle, 1);
            }
        }
    }


    void OnMouseUp()
    {
        if (!guiManager.GetComponent<GUIController>().dialogOpened) // S'il n'y a pas de fenêtre de dialogue d'ouverte
        {
            if (dragged)
            {
                dragged = false;
                TurnManager.phases phaseActive = guiManager.GetComponent<TurnManager>().PhaseActive;

                RaycastHit hit;

                // Le Raycast vérifiera si la droite perpendiculaire à la map passant par l'unité touche un territoire
                // et donc si l'unité tombera sur un territoire
                if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 4.0f, LayerMask.GetMask("BoardLayer")))
                {
                    System.Object zoneSelectionnee = GameObject.Find("Territoires colorés").GetComponent<ColoredTerritories>().GetHoveredTerritory(hit);

                    if (zoneSelectionnee != null)
                    {
                        if (zoneSelectionnee.GetType().Name == "Territoire")
                        {
                            Territoire territoireSelectionnee = zoneSelectionnee as Territoire;

                            // Terrestre terrestre = this as Terrestre;
                            // if(territoireSelectionnee.Equals(terrestre.territoire))

                            if (territoireSelectionnee.joueur == joueur)
                            {
                                // Les unités peuvent se déplacer pendant la phase de déploiement si elles viennent d'être achetée ou durant la 
                                // phase de mouvement
                                if (phaseActive != TurnManager.phases.Attaque)
                                    MoveMultipleUnits(territoireSelectionnee);
                                else
                                    InvalidPositioning("Vous ne pouvez (re)déployer vos unités en-dehors la phase de mouvement ou de déploiement.");
                            }
                            else
                            {
                                if (phaseActive == TurnManager.phases.Attaque)
                                    Attack(territoireSelectionnee);
                                else
                                    InvalidPositioning("Vous ne pouvez attaquer en-dehors de la phase d'attaque.");
                            }
                        }
                        else
                        {
                            Route routeSelectionnee = zoneSelectionnee as Route;

                            if (routeSelectionnee.joueur == joueur || routeSelectionnee.joueur == null)
                            {
                                // Les unités peuvent se déplacer pendant la phase de déploiement si elles viennent d'être achetée ou durant la 
                                // phase de mouvement
                                if (phaseActive != TurnManager.phases.Attaque)
                                    MoveMultipleUnits(routeSelectionnee);
                                else
                                    InvalidPositioning("Vous ne pouvez redéployer vos unités en-dehors la phase de mouvement.");
                            }
                            else
                            {

                                if (phaseActive == TurnManager.phases.Attaque)
                                    Attack(routeSelectionnee);
                                else
                                    InvalidPositioning("Vous ne pouvez attaquer en-dehors de la phase d'attaque");
                            }
                        }
                    }
                    else
                        InvalidPositioning("Cette unité ne peut être déployée ici.");
                }
                else
                {
                    foreach (Unite unit in selectedList)
                        unit.transform.position = unit.offset;
                }
            }
        }
    }

    // Tentative de routine déplaçant une unité vers le curseur
    private IEnumerator MoveToCursor(Vector3 curPosition)
    {
        while (!Input.GetMouseButtonUp(0) || transform.position != curPosition)
        {

            // Les coordonnées du curseur à la caméra
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

            // Les coordonnées du curseur dans le "monde"
            curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);

            // Fixe la hauteur à 0.5 durant le déplacement pour éviter toute collision avec les autres pièces restées au sol
            curPosition.y = 0.5f;

            Vector3 newPos = new Vector3();

            if (curPosition.x > transform.position.x)
                newPos.x = Math.Min(transform.position.x + 0.5f, curPosition.x);
            else if(curPosition.x < transform.position.x)
                newPos.x = Math.Max(transform.position.x - 0.5f, curPosition.x);

            if (curPosition.z > transform.position.z)
                newPos.z = Math.Min(transform.position.z + 0.5f, curPosition.z);
            else if(curPosition.z < transform.position.z)
                newPos.z = Math.Max(transform.position.z - 0.5f, curPosition.z);

            transform.position = newPos;

            yield return null;
        }
    }

    /// <summary>
    /// Vérifie qu'une unité peut être sélectionnée.
    /// </summary>
    /// <param name="joueurActif">Joueur Le joueur actif à ce tour</param>
    /// <param name="phaseActive">TurnManager.phases La phase en cour</param>
    /// <returns>bool true si l'unité peut être ajoutée, false sinon</returns>
    public virtual bool CanBeSelected(Joueur joueurActif, TurnManager.phases phaseActive)
    {
        return false;
    }

    /// <summary>
    /// Désactive l'unité qui ne sera plus sélectionnable.
    /// </summary>
    public virtual void Disable()
    {
        RemoveSelection();
        ApplyMaterial(darkMaterial);
        canAct = false;
    }

    /// <summary>
    /// Vérifie si les unités sélectionnées peuvent se déplacer sur moveTo et procède au déplacement le cas échéant.
    /// Replace les unités à leurs points de départs si ce n'est pas le cas.
    /// </summary>
    /// <param name="moveTo">Territoire Le territoire sur lequelle déplacer les unités.</param>
    public void MoveMultipleUnits(Territoire moveTo)
    {
        bool canMove = true;
        unitesEnum = selectedList.GetEnumerator();

        // Si une des unités sélectionnées ne peut pas se déplacer, on annule tout
        while(canMove && unitesEnum.MoveNext())
        {
            if(unitesEnum.Current.FindDestination(moveTo, false) == null)
                canMove = false;
        }

        if (canMove)
        {
            foreach (Unite unite in selectedList)
                unite.MoveUnit(moveTo, false);
        }
        else
            InvalidPositioning("Une ou plusieurs unité(s) ne peu(ven)t se déplacer sur le territoire sélectionné.");
    }

    /// <summary>
    /// Vérifie si les unités sélectionnées peuvent se déplacer sur moveTo et procède au déplacement le cas échéant.
    /// Replace les unités à leurs points de départs si ce n'est pas le cas.
    /// </summary>
    /// <param name="moveTo">Route La route sur laquelle déplacer les unités.</param>
    public void MoveMultipleUnits(Route moveTo)
    {
        bool canMove = true;
        List<Unite>.Enumerator unitesEnum = selectedList.GetEnumerator();

        while(canMove && unitesEnum.MoveNext())
        {
            canMove = unitesEnum.Current.CanMoveTo(moveTo);
        }

        if (canMove)
        {
            foreach (Unite unite in selectedList)
                unite.MoveUnit(moveTo);
        }
        else
            InvalidPositioning("Une ou plusieurs unité(s) ne peu(ven)t se déplacer sur la route sélectionnée.");
    }

    /// <summary>
    /// Désélectionne l'unité.
    /// </summary>
    public void RemoveSelection()
    {
        if (selectionCircle != null)
        {
            Destroy(selectionCircle.gameObject);
            selectionCircle = null;
        }
        selected = false;
    }

    /// <summary>
    /// Retire unitToRemove de la selection des unités
    /// </summary>
    /// <param name="uniteToRemove"></param>
    public void RemoveFromSelection(Unite uniteToRemove)
    {
        RemoveSelection();
        selectedList.Remove(uniteToRemove);
    }

    /// <summary>
    /// Réactive l'unité pour un nouveau tour.
    /// </summary>
    public void Reset()
    {
        ApplyMaterial(lightMaterial);
        canAct = true;
    }

    /// <summary>
    /// Fonction appellée à l'instantiation de l'unité. Permet une "surcharge" de Start()
    /// </summary>
    protected virtual void OnStart()
    {
        ApplyMaterial(joueur.Material);
    }

    /// <summary>
    /// Retourne moveTo s'il est possible à l'unité de s'y déplacer ou un territoire adjacent si voisinage = true.
    /// </summary>
    /// <param name="territoire">Territoire Le territoire sur lequel se déplacer</param>
    /// <param name="voisinage">bool true pour déplacer l'unité sur un territoire adjacent à moveTo, false pour déplacer l'unité sur moveTo</param>
    /// <returns>Territoire Le territoire sur lequel se déplacer si c'est possible, null si aucun ne correspond</returns>
    protected virtual Territoire FindDestination(Territoire moveTo, bool voisinage)
    {
        return null;
    }

    /// <summary>
    /// Vérifie que les unités peuvent se déplacer sur la route moveTo.
    /// </summary>
    /// <param name="moveTo">Route La route de destination.</param>
    /// <returns>bool True si les unités peuvent s'y rendre, false autrement.</returns>
    public virtual bool CanMoveTo(Route moveTo)
    {
        return false;
    }

    /// <summary>
    /// Fonction permettant de déplacer l'unité sur moveTo ou un territoire adjacent.
    /// </summary>
    /// <param name="territoire">Territoire Le territoire sur lequel se déplacer</param>
    /// <param name="voisinage">bool true pour déplacer l'unité sur un territoire adjacent à moveTo, false pour déplacer l'unité sur moveTo</param>
    public virtual void MoveUnit(Territoire moveTo, bool voisinage)
    {
    }

    /// <summary>
    /// Fonction permettant d'attaquer un territoire
    /// </summary>
    /// <param name="territoire">Territoire Le territoire à attaquer</param>
    public virtual void Attack(Territoire territoire)
    {

    }

    /// <summary>
    /// Fonction permettant de déplacer l'unité sur la route 
    /// </summary>
    /// <param name="route">Route La route sur laquelle se déplacer</param>
    public virtual void MoveUnit(Route route)
    {
    }

    /// <summary>
    /// Fonction permettant d'attaquer une route
    /// </summary>
    /// <param name="route">Route La route à attaquer</param>
    protected virtual void Attack(Route route)
    {

    }

    /// <summary>
    /// Affiche un message d'erreur à l'utilisateur et replace l'unité à son point de départ
    /// </summary>
    /// <param name="message">string Le message à afficher</param>
    protected void InvalidPositioning(string message)
    {
        ResetPosition();
        GameObject.Find("GUIManager").GetComponent<GUIController>().ErrorMessage(message);
    }

    /// <summary>
    /// Replace les unités à leurs positions originales.
    /// </summary>
    public void ResetPosition()
    {
        foreach(Unite unit in selectedList)
            unit.transform.position = unit.originalPosition;
    }

    /// <summary>
    /// Applique material à l'unité
    /// </summary>
    /// <param name="material">Material Le material à appliquer.</param>
    public virtual void ApplyMaterial(Material material)
    {
        transform.GetComponent<Renderer>().material = material;
    }
}