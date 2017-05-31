using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Joueur : MonoBehaviour {

    /// <summary>
    /// L'argent que le joueur peut dépenser
    /// </summary>
    public int Credits
    {
        get
        {
            return credits;
        }
    }
    public int credits = 0;

    /// <summary>
    /// Liste des unités du joueur.
    /// </summary>
    public List<Unite> Unites
    {
        get
        {
            return unites;
        }
    }
    protected List<Unite> unites;

    /// <summary>
    /// Le nom du joueur
    /// </summary>
    public string Nom
    {
        get
        {
            return nom;
        }
        set
        {
            if (!ready)
                nom = value;
        }
    }
    protected string nom;

    /// <summary>
    /// Couleur du joueur
    /// </summary>
    public Utils.couleurs Couleur
    {
        get
        {
            return couleur;
        }
        set
        {
            if (!couleurSetted)
            {
                couleur = value;
                couleurSetted = true;
            }
        }
    }
    protected Utils.couleurs couleur;

    /// <summary>Variable d'état vérifiant que la couleur a bien été ajoutée</summary>
    private bool couleurSetted = false;

    /// <summary>Variable d'état vérifiant que le joueur est correctement initialisé</summary>
    public bool Ready
    {
        get
        {
            return ready;
        }
    }
    private bool ready = false;

    /// <summary>
    /// Indique si le joueur est un humain ou une IA
    /// </summary>
    public bool Humain
    {
        get
        {
            return humain;
        }
        set
        {
            if (!ready)
                humain = value;
        }
    }
    protected bool humain;

    /// <summary>
    /// Dictionnaire des territoires du joueur.
    /// </summary>
    public Dictionary<int, Territoire> Territoires
    {
        get
        {
            return territoires;
        }
    }
    protected Dictionary<int, Territoire> territoires;

    /// <summary>
    /// Liste des routes du joueur.
    /// </summary>
    public List<Route> Routes
    {
        get
        {
            return routes;
        }
    }
    protected List<Route> routes;

    /// <summary>
    /// Liste des continents du joueur.
    /// </summary>
    public List<Continent> Continents
    {
        get
        {
            return continents;
        }
    }
    protected List<Continent> continents;

    /// <summary>
    /// Material dont les unités, territoires et routes du joueur devront être dotés
    /// </summary>
    public Material Material
    {
        get
        {
            return material;
        }
    }
    protected Material material;

    /// <summary>
    /// Material à appliquer sur les unités qui ne peuvent plus jouer
    /// </summary>
    public Material DarkMaterial
    {
        get
        {
            return darkMaterial;
        }
    }
    protected Material darkMaterial;

    /// <summary>Le gestionnaire de l'interface.</summary>
    protected GameObject guiManager;

    /// <summary>La gestion de la partie.</summary>
	protected Partie partie;

    /// <summary>La gestion des territoites.</summary>
	protected ColoredTerritories coloredTerritories;

    /// <summary>Le magasin.</summary>
	protected BoutiqueManager boutique;

    /// <summary>Le gestionnaire de tours.</summary>
	protected TurnManager turnManager;

    /// <summary>Le gestionnaire de combat.</summary>
    protected BattleManager battleManager;

    //protected Mission mission;
    //protected List<Carte> cartes;

    private IEnumerator Start()
    {

        this.territoires = new Dictionary<int, Territoire>();
        this.routes = new List<Route>();
        this.continents = new List<Continent>();
        this.unites = new List<Unite>();

        // Couleur sera la dernière variable configurée et indique donc quand il est possible de procéder avec la classe joueur
        while (!couleurSetted)
            yield return null;

        ready = true;

        do
        {
            guiManager = GameObject.Find("GUIManager");
            yield return null;
        } while (guiManager == null);


        do
        {
            coloredTerritories = GameObject.Find("Territoires colorés").GetComponent<ColoredTerritories>();
            yield return null;
        } while (coloredTerritories == null);

        do
        {
            partie = GameObject.Find("Partie").GetComponent<Partie>();
            yield return null;
        } while (partie == null);

        do
        {
            boutique = GameObject.Find("Boutique button").GetComponent<BoutiqueManager>();
            yield return null;
        } while (boutique == null);

        do
        {
            turnManager = guiManager.GetComponent<TurnManager>();
            yield return null;
        } while (turnManager == null);

        do
        {
            battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
            yield return null;
        } while (battleManager == null);

        Materials materialsComponent = null;

        do
        {
            materialsComponent = GameObject.Find("Materials").GetComponent<Materials>();

            if (materialsComponent != null)
            {
                List<Material> materials = materialsComponent.GetMaterialFromColor(couleur);
                this.material = materials[0];
                this.darkMaterial = materials[1];
            }
            else
                yield return null;
        } while (materialsComponent == null);
    }

    /// <summary>
    /// Ajoute somme aux crédits du joueur. La somme peut être positive comme négative.
    /// </summary>
    /// <param name="somme">int La somme à ajouter</param>
    public void AjouterCredits(int somme)
    {
        credits += somme;
    }

    /// <summary>
    /// Ajoute un territoire conquis par le joueur.
    /// </summary>
    /// <param name="territoire">Territoire Le territoire ajouté par le joueur.</param>
    public void AddTerritoire(Territoire territoire)
    {

        if (!territoires.ContainsValue(territoire))
        {
            territoires.Add(territoire.Num, territoire);
        }
    }

    /// <summary>
    /// Retire un territoire perdu par le joueur.
    /// </summary>
    /// <param name="territoire">Territoire le territoire perdu par le joueur.</param>
    public void RemoveTerritoire(Territoire territoire)
    {
        if (territoires.ContainsValue(territoire))
        {
            territoires.Remove(territoire.Num);
        }
    }

    /// <summary>
    /// Retourne les gains estimés du joueur pour le prochain tour.
    /// Ceux-ci sont calculés en divisant par 3 le nombre de territoires que possède le joueur, arrondis à l'inférieur.
    /// Le minimum étant 3.
    /// </summary>
    /// <returns>int Les gains estimés du joueur au tour suivant.</returns>
    public int EstimatedEarnings()
    {
        if (territoires.Count < 6)
            return 3;
        else
        {
            return (int)Math.Floor((decimal)(territoires.Count / 3));
        }
    }

    /// <summary>
    /// Réactive les unités du joueur.
    /// </summary>
    public void MakeAllUnitsAvailable()
    {
        foreach (Unite unit in unites)
        {
            unit.Reset();
        }
    }

    /// <summary>
    /// Désactive les unités du joueur.
    /// </summary>
    public void MakeAllUnitsUnavailable()
    {
        foreach (Unite unit in unites)
        {
            unit.Disable();
        }
    }

    /// <summary>
    /// Désélectionne les unités du joueur
    /// </summary>
    public void UnselectAllUnits()
    {
        foreach (Unite unite in unites)
            unite.RemoveSelection();
    }

    public bool CanBuyNavalUnits()
    {
        bool canBuy = false;
        Dictionary<int, Territoire>.Enumerator territoireEnum = territoires.GetEnumerator();

        while (!canBuy && territoireEnum.MoveNext())
        {
            List<Route>.Enumerator routesEnum = territoireEnum.Current.Value.Routes.GetEnumerator();

            while (!canBuy && routesEnum.MoveNext())
                if (routesEnum.Current.joueur == null || routesEnum.Current.joueur.Equals(this))
                    canBuy = true;
        }

        return canBuy;
    }

    public List<Unite> GetUnitInTerritoire(Route cible)
    {
        if (this.routes.Contains(cible))
        {
            List<Unite> result = new List<Unite>();

            foreach (Unite unit in unites)
            {
                if (unit.GetType().Name == "Croiseur" || unit.GetType().Name == "Submarin")
                {
                    Maritime maritime = unit as Maritime;

                    if (maritime.route.Equals(cible))
                        result.Add(maritime);
                }
            }

            return result;
        }
        else
            throw new InvalidRouteException("Cette route n'appartient pas au joueur actuel.");
    }

    /*public void EchangerCartes(List<Cartes> Cartes)
    {

    }*/
}
