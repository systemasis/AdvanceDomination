    using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Code fetched from http://hyunkell.com/blog/rts-style-unit-selection-in-unity-5/
/// </summary>
public class UnitSelectionComponent : MonoBehaviour
{
    /// <summary>
    /// Indique si le joueur est dans une sélection par rectangle
    /// </summary>
    private bool isSelecting = false;

    /// <summary>
    /// La position du curseur
    /// </summary>
    private Vector3 mousePosition1;

    /// <summary>
    /// Le prefab pour le cercle de sélection
    /// </summary>
    public GameObject selectionCirclePrefab;

    /// <summary>
    /// Le temps durant lequel le clic gauche a été maintenu pressé
    /// </summary>
    private float timeDown;

    /// <summary>
    /// Coordonnées du curseur dans le plan de la caméra
    /// </summary>
    private Vector3 screenPoint;

    /// <summary>
    /// Offset par rapport au curseur
    /// </summary>
    private Vector3 offset;

    /// <summary>
    /// Les unités sélectionnées
    /// </summary>
    private List<Unite> selectedObjects = new List<Unite>();
    public List<Unite> SelectedUnits
    {
        get
        {
            return selectedObjects;
        }
    }

    public Joueur joueurActif;

    public GUIController guiManager;

    void Update()
    {
        if (guiManager == null)
            guiManager = GameObject.Find("GUIManager").GetComponent<GUIController>();
        else if (!guiManager.dialogOpened) // Si une fenêtre de dialogue n'est pas ouverte
        {
            joueurActif = guiManager.GetComponent<TurnManager>().GetJoueurActif();

            // Réinitialisation du timer comptant le temps passé avec le bouton pressé
            if (Input.GetMouseButtonDown(1))
                timeDown = 0.0f;
            else if (Input.GetMouseButton(1))
            {
                timeDown += 0.1f;

                // 1/2 seconde c'est écoulé sans relâchement du bouton, le joueur veut sélectionner avec le triangle
                // !isSelecting permet d'éviter de réinitialiser la sélection
                if (timeDown > 0.5f && !isSelecting)
                {
                    isSelecting = true;
                    mousePosition1 = Input.mousePosition;

                    removeAllSelection(selectedObjects);
                    selectedObjects = new List<Unite>();
                }
            }

            // Lorsque le bouton est relâché, ajoute les unités sélectionnées s'il y en a
            if (Input.GetMouseButtonUp(1))
            {
                TurnManager.phases phaseActive = guiManager.GetComponent<TurnManager>().PhaseActive;

                // C'est un clic, on ajoute l'unité pointée par le curseur
                if (timeDown < 0.5f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 13.5f, LayerMask.GetMask("Unites")))
                    {
                        Unite unit = hit.collider.gameObject.GetComponent<Unite>();

                        if (unit.CanBeSelected(joueurActif, phaseActive))
                        {
                            AddSelection(hit.collider.gameObject.GetComponent<Unite>());
                        }
                    }
                    else
                    {
                        removeAllSelection(selectedObjects);
                    }
                }
                else // C'est une sélection multiple, on ajoute tout ce qui est dans le rectangle
                {
                    removeAllSelection(selectedObjects);
                    foreach (var selectableObject in FindObjectsOfType<Unite>())
                    {
                        if (IsWithinSelectionBounds(selectableObject.gameObject) && selectableObject.CanBeSelected(joueurActif, phaseActive))
                        {
                            AddSelection(selectableObject);
                        }
                    }

                    isSelecting = false;

                    /*
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Format("Selecting [{0}] Units", selectedObjects.Count));
                    foreach (var selectedObject in selectedObjects)
                        sb.AppendLine("-> " + selectedObject.gameObject.name);
                    Debug.Log(sb.ToString());
                    */
                }

                timeDown = 0.0f;
            }

            // Highlight all objects within the selection box
            if (isSelecting)
            {
                TurnManager.phases phaseActive = guiManager.GetComponent<TurnManager>().PhaseActive;

                foreach (var selectableObject in FindObjectsOfType<Unite>())
                {
                    if (IsWithinSelectionBounds(selectableObject.gameObject) && !selectedObjects.Contains(selectableObject) && selectableObject.CanBeSelected(joueurActif, phaseActive))
                        AddSelection(selectableObject);
                }
            }
        }
    }

    /// <summary>
    /// Vérifie la présence d'un GameObject dans le rectangle de sélection
    /// </summary>
    /// <param name="gameObject">GameObject Le GameObject à vérifié</param>
    /// <returns>true si le GameObject est dans le rectangle, false autrement</returns>
    /// <author>Jeff Zimmer</author>
    public bool IsWithinSelectionBounds( GameObject gameObject )
    {
        if( !isSelecting )
            return false;

        var camera = Camera.main;
        var viewportBounds = Utils.GetViewportBounds( camera, mousePosition1, Input.mousePosition );
        return viewportBounds.Contains( camera.WorldToViewportPoint( gameObject.transform.position ) );
    }

    void OnGUI()
    {
        if( isSelecting )
        {
            // Create a rect from both mouse positions
            var rect = Utils.GetScreenRect( mousePosition1, Input.mousePosition );
            Utils.DrawScreenRect( rect, new Color( 0.8f, 0.8f, 0.95f, 0.25f ) );
            Utils.DrawScreenRectBorder( rect, 2, new Color( 0.8f, 0.8f, 0.95f ) );
        }
    }

    /// <summary>
    /// Ajoute selectableObject à la liste des unités sélectionnées
    /// </summary>
    /// <param name="selectableObject">Unite Le selectableObject à ajouter à la liste d'unités sélectionnées</param>
    void AddSelection(Unite selectableObject)
    {
        if (selectableObject.selectionCircle == null)
        {
            selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
            selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
            selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);
        }
        selectableObject.selected = true;
        if (!selectedObjects.Contains(selectableObject))
            selectedObjects.Add(selectableObject);

        foreach (Unite unit in selectedObjects)
            unit.selectedList = selectedObjects;
    }

    /// <summary>
    /// Retire toutes les unités sélectionnées de la liste
    /// </summary>
    /// <param name="selectedObjects">List<Unite> La liste à vider</param>
    void removeAllSelection(List<Unite> selectedObjects)
    {
        foreach (Unite selectableObject in selectedObjects)
        {
            selectableObject.RemoveSelection();
        }
    }
}
 