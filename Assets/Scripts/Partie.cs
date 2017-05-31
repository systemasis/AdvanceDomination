using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Partie : MonoBehaviour {

    /// <summary>
    /// Dictionnaire des joueurs de la partie, indexés par ordre de passage.
    /// </summary>
    private Dictionary<int, Joueur> joueurs;
    public Dictionary<int, Joueur> Joueurs
    {
        get
        {
            return joueurs;
        }
    }

    /// <summary>
    /// Dictionnaire des routes de la partie.
    /// </summary>
    
    public Dictionary<int, Route> Routes
    {
        get
        {
            return routes;
        }
    }
	private Dictionary<int, Route> routes;

    /// <summary>
    /// Variable d'état indiquant si c'est le premier tour
    /// </summary>
    public bool FirstTurn
    {
        get
        {
            return firstTurn;
        }
    }
    private bool firstTurn = true;

    /// <summary>
    /// Dictionnaire des territoires de la partie.
    /// </summary>
    private Dictionary<int, Territoire> territoires = new Dictionary<int, Territoire>();
    public Dictionary<int, Territoire> Territoires
    {
        get
        {
            return territoires;
        }
    }

    /// <summary>
    /// La carte coloriée.
    /// </summary>
    public ColoredTerritories coloredTerritories;

    /// <summary>
    /// Les prefabs représentant le joueur et les différents niveaux d'IA.
    /// Les index sont les suivants :
    /// 0 : Joueur
    /// 1 : IAEasy
    /// 2 : IAMedium
    /// 3 : IAHard
    /// </summary>
    public List<GameObject> playerPrefabs;
    
	private IEnumerator Start ()
    {
        int nbrJoueurs = 1, nbrIas = 1, i = 0;
        joueurs = new Dictionary<int, Joueur>(nbrJoueurs);
        Transform joueursGameObject = GameObject.Find("Joueurs").transform;

        // Création des joueurs.
        for(i = 0; i < nbrJoueurs; i++)
        {
            Instantiate(playerPrefabs[0], joueursGameObject);

            Joueur joueur = joueursGameObject.GetChild(joueursGameObject.childCount - 1).GetComponent<Joueur>();
            InitialisePlayer(joueur, "Joueur" + (i + 1), true, (Utils.couleurs) i);

            joueurs.Add(i, joueur);
        }
        
        for(i = 0; i < nbrIas; i++)
        {
            Instantiate(playerPrefabs[1], joueursGameObject);

            IAEasy iae = joueursGameObject.GetChild(joueursGameObject.childCount - 1).GetComponent<IAEasy>();
            InitialisePlayer(iae, "IA" + (i + 1), false, (Utils.couleurs) nbrJoueurs + i);
            
            joueurs.Add(nbrJoueurs + i, iae);
        }
        
        territoires = coloredTerritories.Territoires;
        routes = coloredTerritories.Routes;
        List<Territoire> territoireAAttribuer = new List<Territoire>(territoires.Count);
        System.Random rand = new System.Random();

        for (i = 0; i < territoires.Count; i++)
            territoireAAttribuer.Add(territoires[i]);

        i = 0;
        BoutiqueManager boutique = GameObject.Find("Boutique button").GetComponent<BoutiqueManager>();

        // Ajout de territoires aux joueurs de façon arbitraire.
        while (territoireAAttribuer.Count > 0)
        {
            // Sélection aléatoire du territoire à attribuer.
            int index = rand.Next(0, territoireAAttribuer.Count);

            while (joueurs[i % joueurs.Count].Ready == false)
                yield return null;

            joueurs[i % joueurs.Count].Territoires.Add(territoireAAttribuer[index].Num, territoireAAttribuer[index]);

            // Ajout d'une infanterie pour marquer quel territoire appartient à qui.
            Infanterie newUnit = boutique.CreateUnit(
                (int) Utils.unitCode.Infanterie,
                GameObject.Find("Unites").transform,
                territoireAAttribuer[index].position,
                joueurs[i % joueurs.Count]
            ).GetComponent<Infanterie>();

            newUnit.territoire = territoireAAttribuer[index];

            territoireAAttribuer[index].unites.Add(newUnit);
            territoireAAttribuer[index].joueur = joueurs[i % joueurs.Count];
            territoireAAttribuer.RemoveAt(index);
            i++;

            // Au cas où ce serait un peu long.
            yield return null;
        }

        // Application des bonnes couleurs pour chaque unité.
        for(i = 0; i < joueurs.Count; i++)
        {
            foreach(Unite unit in joueurs[i].Unites)
            {
                unit.lightMaterial = joueurs[i].Material;
                unit.darkMaterial = joueurs[i].DarkMaterial;

                unit.ApplyMaterial(unit.darkMaterial);
            }
        }
        
        // Initialisation des fonds du premier joueur.
        joueurs[0].AjouterCredits(joueurs[0].EstimatedEarnings());
    }

    private void InitialisePlayer(Joueur joueur, string nom, bool humain, Utils.couleurs couleur)
    {
        joueur.Nom = nom;
        joueur.Humain = humain;
        joueur.Couleur = couleur;
    }
}
