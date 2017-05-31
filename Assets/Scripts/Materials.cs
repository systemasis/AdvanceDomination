using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Materials : MonoBehaviour {

    public Material bleu;
    public Material bleuSombre;
    public Material gris;
    public Material grisSombre;
    public Material jaune;
    public Material jauneSombre;
    public Material rouge;
    public Material rougeSombre;
    public Material vert;
    public Material vertSombre;
    public Material violet;
    public Material violetSombre;


    /// <summary>
    /// Retourne les materials à appliquer pour la couleur renseignée
    /// </summary>
    /// <param name="couleur">Utils.couleurs La couleur désirée</param>
    /// <returns>List Les materials à appliquer dans l'ordre suivant : clair et sombre.</returns>
    public List<Material> GetMaterialFromColor(Utils.couleurs couleur)
    {
        if (Utils.couleurs.bleue == couleur)
            return new List<Material>{
                bleu,
                bleuSombre
            };
        else if (Utils.couleurs.gris == couleur)
            return new List<Material>{
                gris,
                grisSombre
            };
        else if (Utils.couleurs.jaune == couleur)
            return new List<Material>{
                jaune,
                jauneSombre
            };
        else if (Utils.couleurs.rouge == couleur)
            return new List<Material>{
                rouge,
                rougeSombre
            };
        else if (Utils.couleurs.vert == couleur)
            return new List<Material>{
                vert,
                vertSombre
            };
        else if (Utils.couleurs.violet == couleur)
            return new List<Material>{
                violet,
                violetSombre
            };
        else
            throw new InvalidColorException(couleur);
    }
}
