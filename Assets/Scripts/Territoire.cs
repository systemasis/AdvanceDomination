using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Territoire
{

	/// <summary>
	/// Le nom du territoire
	/// </summary>
	private string nom;
	public string Nom
	{
		get
		{
			return nom;
		}
	}

	/// <summary>
	/// Le numéro du territoire (temporaire le temps de nommer chaque territoire)
	/// </summary>
	private int num;
	public int Num
	{
		get
		{
			return num;
		}
    }

    /// <summary>
    /// Les routes connectées à ce territoire
    /// </summary>
    private List<Route> routes;
    public List<Route> Routes
    {
        get
        {
            return routes;
        }
    }

	/// <summary>
	/// Les territoires adjacents à celui-ci
	/// </summary>
	private List<Territoire> voisins;
	public List<Territoire> Voisins
	{
		get
		{
			return voisins;
		}
	}

	/// <summary>
	/// La position autour de laquelle les unités devront se placer
	/// </summary>
	public Vector3 Position
	{
		get
		{
			return position;
		}
	}
	public Vector3 position;

    /// <summary>
    /// Le joueur possédant le territoire
    /// </summary>
    public Joueur joueur;

    /// <summary>
    /// Le continent sur lequel se trouve le territoire
    /// </summary>
    public Continent continent;

	/// <summary>
	/// Couleur du territoire sur le quad Territoires colorés
	/// </summary> 
	public Color couleur;

    /// <summary>Liste des unités présentes sur le territoire</summary>
    public List<Unite> unites;

	public Territoire(Color couleur, string nom, int num)
	{
		this.nom = nom;
		routes = new List<Route>();
		voisins = new List<Territoire>();
		joueur = null;
		continent = null;
		this.couleur = couleur;
		this.num = num;
        unites = new List<Unite>();
	}

	public Territoire(Color couleur, string nom, int num, Vector3 position)
	{
        this.nom = nom;
        routes = new List<Route>();
        voisins = new List<Territoire>();
        joueur = null;
        continent = null;
        this.couleur = couleur;
        this.num = num;
        unites = new List<Unite>();
        this.position = position;
    }

    /// <summary>
    /// Ajoute un voisin dans la liste des voisins du territoire et inversement si nécessaire
    /// </summary>
    /// <param name="voisin">Territoire Le voisin à ajouter</param>
    public void AddVoisin(Territoire voisin)
	{
		// Ajout du voisin dans la liste des voisins
		if (!voisins.Contains(voisin))
			voisins.Add(voisin);

		// Ajout du territoire dans la liste des voisins du voisin
		if (!voisin.Voisins.Contains(this))
			voisin.AddVoisin(this);
	}
    
    /// <summary>
    /// Retourne les voisins au degre-ième degré du territoire.
    /// </summary>
    /// <param name="degre">int Le degré de voisinage.</param>
    /// <param name="withRoutes">bool True s'il faut considérer les routes, false sinon.</param>
    /// <param name="ranged">bool True s'il s'agit d'une attaque à distance.</param>
    public List<Territoire> GetVoisinsNDegree(int degre, bool withRoutes, bool ranged)
    {
        List<Territoire> voisinsARetourner = new List<Territoire> { this }; // Les voisins à retourner
        List<Territoire> voisinsAOuvrir = new List<Territoire>(); // Les voisins à regarder à ce degré
        List<Territoire> voisinsDegreeSuivant = new List<Territoire>(); // Les voisins à regarder au degré suivant
        int i = 0;

        foreach (Territoire territoire in Voisins)
            voisinsDegreeSuivant.Add(territoire);

        if (withRoutes)
        {
            foreach (Route route in routes)
            {
                // Si la route n'est pas bloquée par un autre joueur ou qu'il s'agit d'une attaque à distance
                if(route.joueur == null || route.joueur.Equals(joueur) || ranged)
                {
                    foreach (Territoire territoire in route.Territoires)
                        if(!territoire.Equals(this))
                            voisinsDegreeSuivant.Add(territoire);
                }
            }
        }
        
        // Tant que l'on a pas atteint la profondeur degre + 1.
        while(i <= degre)
        {
            // Réinitialisation
            voisinsAOuvrir.Clear();
            
            foreach (Territoire territoire in voisinsDegreeSuivant)
                voisinsAOuvrir.Add(territoire);

            voisinsDegreeSuivant.Clear();
            // Fin réinitialisation

            // Parcourt de la profondeur actuelle.
            foreach(Territoire voisin in voisinsAOuvrir)
            {
                voisinsARetourner.Add(voisin);

                foreach(Territoire voisinDuVoisin in voisin.Voisins)
                {
                    // Si le voisin n'est pas dans une des trois listes (et donc qu'il n'est pas déjà parcouru ou prévu d'être parcouru
                    if (!voisinsARetourner.Contains(voisinDuVoisin) && !voisinsAOuvrir.Contains(voisinDuVoisin) && !voisinsDegreeSuivant.Contains(voisinDuVoisin) && !voisinDuVoisin.Equals(this))
                        voisinsDegreeSuivant.Add(voisinDuVoisin);
                }

                // S'il faut également passer par les routes.
                if (withRoutes)
                {
                    foreach(Route route in voisin.Routes)
                    {
                    // Si la route n'est pas bloquée par un autre joueur ou qu'il s'agit d'une attaque à distance
                    if (route.joueur == null || route.joueur.Equals(joueur) || ranged)
                        {
                            foreach (Territoire territoire in route.Territoires)
                            {
                                // Si le territoire n'est pas dans une des trois listes (et donc qu'il n'est pas déjà parcouru ou prévu d'être parcouru
                                if (!voisinsARetourner.Contains(territoire) && !voisinsAOuvrir.Contains(territoire) && !voisinsDegreeSuivant.Contains(territoire) && !territoire.Equals(this))
                                    voisinsDegreeSuivant.Add(territoire);
                            }
                        }
                    }
                }
            }

            ++i;
        }

        return voisinsARetourner;
    }

    /// <summary>
    /// Vérifie la présence de DCA capable de défendre
    /// </summary>
    /// <returns>bool True si une DCA est à portée, false sinon.</returns>
    public bool DcaInRange()
    {
        List<Territoire> voisinsSecondDegre = GetVoisinsNDegree(2, false, true);

        foreach (Dca unit in joueur.Unites)
            if (voisinsSecondDegre.Contains(unit.territoire))
                return true;

        return false;
    }

    /// <summary>
    /// Renvoie la liste des DCAs à portée de tire.
    /// </summary>
    /// <returns>List(Dca) La liste des DCAs à portée</returns>
    public List<Dca> GetDcaInRange()
    {
        List<Territoire> voisinsSecondDegre = GetVoisinsNDegree(2, false, true);
        List<Dca> dcas = new List<Dca>();

        foreach (Unite unit in joueur.Unites)
        {
            if (unit.GetType().Name == "Dca")
            {
                Dca dca = unit as Dca;

                if (voisinsSecondDegre.Contains(dca.territoire))
                    dcas.Add(dca);
            }
        }

        return dcas;
    }
}
