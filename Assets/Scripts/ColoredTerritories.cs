using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColoredTerritories : MonoBehaviour
{

    /// <summary>
    /// Dictionnaire des territoires de la partie.
    /// </summary>
    public Dictionary<int, Territoire> Territoires
    {
        get
        {
            return territoires;
        }
    }
    private Dictionary<int, Territoire> territoires = new Dictionary<int, Territoire>() {
        { 0, new Territoire(new Color32(0, 0, 128, 255), "1", 1, new Vector3(-27, 0, 6.75f)) },
        { 1, new Territoire(new Color32(0, 0, 255, 255), "2", 2, new Vector3(-21, 0, 7.5f)) },
        { 2, new Territoire(new Color32(0, 128, 0, 255), "3", 3, new Vector3(-29.5f, 0, 3)) },
        { 3, new Territoire(new Color32(0, 255, 0, 255), "4", 4, new Vector3(-24, 0, 5)) },
        { 4, new Territoire(new Color32(128, 0, 0, 255), "5", 5, new Vector3(-20, 0, 4)) },
        { 5, new Territoire(new Color32(255, 0, 0, 255), "6", 6, new Vector3(-15.5f, 0, 4.5f)) },
        { 6, new Territoire(new Color32(0, 128, 128, 255), "7", 7, new Vector3(-24, 0, 1.5f)) },
        { 7, new Territoire(new Color32(0, 128, 255, 255), "8", 8, new Vector3(-26, 0, -1.75f)) },
        { 8, new Territoire(new Color32(255, 0, 128, 255), "9", 9, new Vector3(-20, 0, -1.75f)) },
        { 9, new Territoire(new Color32(255, 0, 255, 255), "10", 10, new Vector3(-27.5f, 0, -4.5f)) },
        { 10, new Territoire(new Color32(255, 128, 0, 255), "11", 11, new Vector3(-30.5f, 0, -8.5f)) },
        { 11, new Territoire(new Color32(255, 128, 128, 255), "12", 12, new Vector3(-12, 0, -6)) },
        { 12, new Territoire(new Color32(255, 128, 255, 255), "13", 13, new Vector3(-21, 0, -9)) },
        { 13, new Territoire(new Color32(255, 255, 0, 255), "14", 14, new Vector3(-17, 0, -8)) },
        { 14, new Territoire(new Color32(255, 255, 128, 255), "15", 15, new Vector3(-11, 0, -10)) },
        { 15, new Territoire(new Color32(0, 255, 128, 255), "16", 16, new Vector3(-5, 0, -8)) },
        { 16, new Territoire(new Color32(0, 255, 255, 255), "17", 17, new Vector3(-19, 0, -13)) },
        { 17, new Territoire(new Color32(128, 0, 128, 255), "18", 18, new Vector3(-15.5f, 0, -13)) },
        { 18, new Territoire(new Color32(128, 0, 255, 255), "19", 19, new Vector3(-11.5f, 0, -13.5f)) },
        { 19, new Territoire(new Color32(128, 128, 0, 255), "20", 20, new Vector3(-6, 0, -14.5f)) },
        { 20, new Territoire(new Color32(128, 128, 128, 255), "21", 21, new Vector3(-9, 0, 15)) },
        { 21, new Territoire(new Color32(64, 64, 64, 255), "22", 22, new Vector3(-7, 0, 12)) },
        { 22, new Territoire(new Color32(128, 128, 255, 255), "23", 23, new Vector3(4.5f, 0, 9.5f)) },
        { 23, new Territoire(new Color32(128, 255, 0, 255), "24", 24, new Vector3(9.5f, 0, 8)) },
        { 24, new Territoire(new Color32(128, 255, 128, 255), "25", 25, new Vector3(1.5f, 0, 2.5f)) },
        { 25, new Territoire(new Color32(128, 255, 255, 255), "26", 26, new Vector3(5.5f, 0, 4)) },
        { 26, new Territoire(new Color32(0, 0, 64, 255), "27", 27, new Vector3(6.5f, 0, 0)) },
        { 27, new Territoire(new Color32(0, 0, 192, 255), "28", 28, new Vector3(5.5f, 0, -3.5f)) },
        { 28, new Territoire(new Color32(0, 64, 0, 255), "29", 29, new Vector3(8, 0, -6.5f)) },
        { 29, new Territoire(new Color32(0, 64, 64, 255), "30", 30, new Vector3(20, 0, 0)) },
        { 30, new Territoire(new Color32(0, 64, 192, 255), "31", 31, new Vector3(26.5f, 0, 2.5f)) },
        { 31, new Territoire(new Color32(0, 192, 0, 255), "32", 32, new Vector3(23, 0, -2.5f)) },
        { 32, new Territoire(new Color32(0, 192, 64, 255), "33", 33, new Vector3(29, 0, -2)) },
        { 33, new Territoire(new Color32(0, 192, 192, 255), "34", 34, new Vector3(21, 0, -6.5f)) },
        { 34, new Territoire(new Color32(64, 0, 0, 255), "35", 35, new Vector3(20, 0, -10.5f)) },
        { 35, new Territoire(new Color32(64, 0, 128, 255), "36", 36, new Vector3(23.5f, 0, -10.5f)) },
        { 36, new Territoire(new Color32(64, 0, 192, 255), "37", 37, new Vector3(20.5f, 0, -15.5f)) },
        { 37, new Territoire(new Color32(64, 64, 0, 255), "38", 38, new Vector3(0, 0, -16)) }
    };

    /// <summary>
    /// Les routes présentes sur la carte.
    /// </summary>
    public Dictionary<int, Route> Routes
    {
        get
        {
            return routes;
        }
    }
    private Dictionary<int, Route> routes;

    public void Start()
    {
        InitialiseVoisins();
        InitialiseRoutes();
    }

    /// <summary>
    /// Retourne le territoire ou la route au coordonnées de l'impact hit
    /// </summary>
    /// <param name="hit">RaycastHit Le point d'impact sur la carte colorée.</param>
    /// <returns>System.Object Le territoire ou la route aux coordonnées de l'impact, null s'il n'y en a pas.</returns>
    public System.Object GetHoveredTerritory(RaycastHit hit)
    {
        Texture2D tex = GetComponent<Renderer>().material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord; // Coordonnées (en pourcentages) de l'impact dans la texture
        pixelUV.x = tex.width - (pixelUV.x * tex.width);
        pixelUV.y = tex.height - (tex.height * pixelUV.y);
        Color pixel = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y); // Couleur du pixel touché dans la texture
        int i = -1;

        if (pixel != Color.white && pixel != Color.black)
        {
            while (++i < territoires.Count)
            {
                if (pixel.Equals(territoires[i].couleur)) // Si le pixel correspond, on retourne le territoire.
                {
                    return territoires[i];
                }
            }

            i = -1;

            while(++i < routes.Count)
            {
                if (pixel.Equals(routes[i].Couleur)) // Si le pixel correspond, on retourne la route.
                {
                    return routes[i];
                }
            }
        }

        return null;
    }

    public int GetRouteIndex(Route route)
    {
        foreach (KeyValuePair<int, Route> pair in routes)
            if(pair.Value.Equals(route))
                return pair.Key;
        
        return -1;
    }

    /// <summary>
    /// Initialise la liste des voisins de chaque territoire
    /// </summary>
    private void InitialiseVoisins()
    {
        territoires[0].AddVoisin(territoires[1]);
        territoires[0].AddVoisin(territoires[2]);
        territoires[0].AddVoisin(territoires[3]);

        territoires[1].AddVoisin(territoires[3]);
        territoires[1].AddVoisin(territoires[4]);
        territoires[1].AddVoisin(territoires[5]);

        territoires[2].AddVoisin(territoires[3]);
        territoires[2].AddVoisin(territoires[6]);

        territoires[3].AddVoisin(territoires[4]);
        territoires[3].AddVoisin(territoires[6]);

        territoires[4].AddVoisin(territoires[5]);
        territoires[4].AddVoisin(territoires[6]);
        territoires[4].AddVoisin(territoires[8]);

        territoires[5].AddVoisin(territoires[8]);

        territoires[6].AddVoisin(territoires[7]);
        territoires[6].AddVoisin(territoires[8]);

        territoires[7].AddVoisin(territoires[8]);
        territoires[7].AddVoisin(territoires[9]);

        territoires[8].AddVoisin(territoires[9]);
        territoires[8].AddVoisin(territoires[11]);

        territoires[9].AddVoisin(territoires[10]);

        territoires[11].AddVoisin(territoires[13]);
        territoires[11].AddVoisin(territoires[14]);
        territoires[11].AddVoisin(territoires[15]);

        territoires[12].AddVoisin(territoires[13]);
        territoires[12].AddVoisin(territoires[16]);

        territoires[13].AddVoisin(territoires[14]);
        territoires[13].AddVoisin(territoires[16]);
        territoires[13].AddVoisin(territoires[17]);

        territoires[14].AddVoisin(territoires[15]);
        territoires[14].AddVoisin(territoires[17]);
        territoires[14].AddVoisin(territoires[18]);
        territoires[14].AddVoisin(territoires[19]);

        territoires[15].AddVoisin(territoires[19]);

        territoires[16].AddVoisin(territoires[17]);

        territoires[17].AddVoisin(territoires[18]);
        territoires[17].AddVoisin(territoires[19]);

        territoires[18].AddVoisin(territoires[19]);

        territoires[20].AddVoisin(territoires[21]);

        territoires[22].AddVoisin(territoires[23]);
        territoires[22].AddVoisin(territoires[24]);
        territoires[22].AddVoisin(territoires[25]);

        territoires[24].AddVoisin(territoires[25]);
        territoires[24].AddVoisin(territoires[26]);

        territoires[25].AddVoisin(territoires[26]);

        territoires[26].AddVoisin(territoires[27]);

        territoires[27].AddVoisin(territoires[28]);

        territoires[29].AddVoisin(territoires[31]);

        territoires[30].AddVoisin(territoires[31]);
        territoires[30].AddVoisin(territoires[32]);

        territoires[31].AddVoisin(territoires[32]);
        territoires[31].AddVoisin(territoires[33]);

        territoires[33].AddVoisin(territoires[34]);
        territoires[33].AddVoisin(territoires[36]);
    }

    /// <summary>
    /// Initialise le dictionnaire des routes.
    /// </summary>
    private void InitialiseRoutes()
    {
        routes = new Dictionary<int, Route>();

        CreateRoute(0,  new int[] { 1, 20 }, new Color32(0, 0, 25, 255));
        CreateRoute(1, new[] { 2, 30 }, new Color32(0, 0, 50, 255));
        CreateRoute(2, new int[] { 5, 20 }, new Color32(0, 25, 0, 255));
        CreateRoute(3, new int[] { 5, 21 }, new Color32(0, 25, 25, 255));
        CreateRoute(4, new int[] { 8, 24 }, new Color32(0, 25, 50, 255));
        CreateRoute(5, new int[] { 9, 12 }, new Color32(0, 50, 0, 255));
        CreateRoute(6, new int[] { 10, 12 }, new Color32(0, 50, 25, 255));
        CreateRoute(7, new int[] { 10, 36 }, new Color32(0, 50, 50, 255));
        CreateRoute(8, new int[] { 15, 24 }, new Color32(25, 0, 0, 255));
        CreateRoute(9, new int[] { 15, 27 }, new Color32(25, 0, 25, 255));
        CreateRoute(10, new int[] { 19, 37 }, new Color32(25, 0, 50, 255));
        CreateRoute(11, new int[] { 21, 22 }, new Color32(25, 25, 0, 255));
        CreateRoute(12, new int[] { 21, 24 }, new Color32(25, 25, 25, 255));
        CreateRoute(13, new int[] { 23, 29 }, new Color32(25, 25, 50, 255));
        CreateRoute(14, new int[] { 26, 29 }, new Color32(25, 50, 0, 255));
        CreateRoute(15, new int[] { 27, 33 }, new Color32(25, 50, 25, 255));
        CreateRoute(16, new int[] { 28, 34 }, new Color32(25, 50, 50, 255));
        CreateRoute(17, new int[] { 34, 35 }, new Color32(50, 0, 0, 255));
        CreateRoute(18, new int[] { 35, 36 }, new Color32(50, 0, 25, 255));

        AddRoutesVoisines(0, new int[] { 1, 2, 11, 13 });
        AddRoutesVoisines(1, new int[] { 0, 7, 11, 13 });
        AddRoutesVoisines(2, new int[] { 0, 3 });
        AddRoutesVoisines(3, new int[] { 2, 4, 12 });
        AddRoutesVoisines(4, new int[] { 3, 8, 12 });
        AddRoutesVoisines(5, new int[] { 6 });
        AddRoutesVoisines(6, new int[] { 5, 7, 9, 10, 16, 17, 18 });
        AddRoutesVoisines(7, new int[] { 1, 6, 9, 10, 16, 17, 18 });
        AddRoutesVoisines(8, new int[] { 4, 9 });
        AddRoutesVoisines(9, new int[] { 6, 7, 8, 10, 16, 17, 18 });
        AddRoutesVoisines(10, new int[] { 6, 7, 9, 16, 17, 18 });
        AddRoutesVoisines(11, new int[] { 0, 2, 12, 13 });
        AddRoutesVoisines(12, new int[] { 3, 4, 11 });
        AddRoutesVoisines(13, new int[] { 0, 2, 11, 14 });
        AddRoutesVoisines(14, new int[] { 13, 15 });
        AddRoutesVoisines(15, new int[] { 14, 16 });
        AddRoutesVoisines(16, new int[] { 6, 7, 10, 15, 17, 18 });
        AddRoutesVoisines(17, new int[] { 6, 7, 10, 16, 18 });
        AddRoutesVoisines(18, new int[] { 6, 7, 10, 16, 17 });
    }

    /// <summary>
    /// Créé les routes et les associes aux territoires qu'elles connectes.
    /// </summary>
    /// <param name="indexRoute">int L'index de la nouvelle route dans la liste.</param>
    /// <param name="indexTerritoires">int[] Les index des territoires qu'elles connectent.</param>
    /// <param name="couleur">Color32 La couleur de la route sur la carte.</param>
    private void CreateRoute(int indexRoute, int[] indexTerritoires, Color32 couleur)
    {
        routes.Add(indexRoute, new Route(territoires[indexTerritoires[0]], territoires[indexTerritoires[1]], couleur));
        territoires[indexTerritoires[0]].Routes.Add(routes[indexRoute]);
        territoires[indexTerritoires[1]].Routes.Add(routes[indexRoute]);
    }

    /// <summary>
    /// Ajoute à une route celles auxquelles les unités qui y sont stationnées peuvent accéder.
    /// </summary>
    /// <param name="indexRoute">int L'index dans la liste des routes de celle que l'on veut modifier.</param>
    /// <param name="indexVoisines">int[] Les index dans la liste des routes des routes voisines à ajouter.</param>
    private void AddRoutesVoisines(int indexRoute, int[] indexVoisines)
    {
        List<Route> voisines = new List<Route>();

        for (int i = 0; i < indexVoisines.Length; i++)
            voisines.Add(routes[indexVoisines[i]]);

        routes[indexRoute].RoutesVoisines = voisines;
    }
}
