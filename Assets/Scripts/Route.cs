using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Route {

    /// <summary>
    /// Le joueur possédant la route
    /// </summary>
    public Joueur joueur = null;

    /// <summary>
    /// Les territoires connectés par cette route
    /// </summary>
    private List<Territoire> territoires;
    public List<Territoire> Territoires
    {
        get
        {
            return territoires;
        }
    }

    /// <summary>
    /// La couleur de la route pour la reconnaissance.
    /// </summary>
    public Color Couleur
    {
        get
        {
            return couleur;
        }
    }
    private Color couleur;
    
    /// <summary>
    /// Les routes auxquelles peuvent accéder les unités stationnées sur celle-ci.
    /// </summary>
    public List<Route> RoutesVoisines
    {
        get
        {
            return routesVoisines;
        }
        set
        {
            if (routesVoisines == null)
                routesVoisines = value;
        }
    }
    private List<Route> routesVoisines;

    /// <summary>Les unités présentes sur la route</summary>
    public List<Unite> unites;

    /// <param name="territoires">Liste des territoires connectés par cette route.</param>
    /// <param name="color">Color La couleur de la route pour la reconnaissance.</param>
    public Route(List<Territoire> territoires, Color color)
    {
        this.territoires = territoires;
        couleur = color;
        unites = new List<Unite>();
    }
    
    /// <param name="territoire1">Territoire Un des territoires que la route connecte.</param>
    /// <param name="territoire2">Territoire Un des territoires que la route connecte.</param>
    /// <param name="color">Color La couleur de la route pour la reconnaissance.</param>
    public Route(Territoire territoire1, Territoire territoire2, Color color)
    {
        territoires = new List<Territoire>()
        {
            territoire1,
            territoire2
        };

        couleur = color;

        unites = new List<Unite>();
    }
}
