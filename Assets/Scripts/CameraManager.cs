using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    /// <summary>
    /// Index, dans le tableau bounds, de la valeur maximum que peut prendre la coordonnée x de la caméra
    /// </summary>
    private readonly int maxX=1;

    /// <summary>
    /// Index, dans le tableau bounds, de la valeur minimum que peut prendre la coordonnée x de la caméra
    /// </summary>
    private readonly int minX=0;

    /// <summary>
    /// Index, dans le tableau bounds, de la valeur maximum que peut prendre la coordonnée z de la caméra
    /// </summary>
    private readonly int maxZ=3;

    /// <summary>
    /// Index, dans le tableau bounds, de la valeur minimum que peut prendre la coordonnée z de la caméra
    /// </summary>
    private readonly int minZ=2;

    /// <summary>
    /// Les différentes limites que peut atteindre la caméra.
    /// Chaque sous-tableau correspond à un cran et les valeurs sont dans l'ordre suivant :
    /// minX, maxX, minZ, maxZ
    /// </summary>
    private readonly float[,] bounds = new float[,] {
        { -10.0f, 10.0f, -6.0f, 1.5f },
        { -7.5f, 7.5f, -4.5f, 2.0f },
        { -6.5f, 6.75f, -4.0f, 1.5f },
        { -5.0f, 5.0f, -3.5f, -0.0f },
        { -3.0f, 3.0f, -3.2f, -2.5f },
        { 0, 0, 0, 0}
    };
    
    /// <summary>
    /// Les différentes valeurs que peut prendre la coordonnée z de la caméra
    /// </summary>
    private readonly float[] cranTab = new float[] { 2.0f, 4.0f, 5.0f, 7.0f, 9.0f, 13.5f };

    /// <summary>
    /// Le cran actuel de la caméra
    /// </summary>
    private int cran;

	public GameObject minimap;
    
    void Start () {
        cran = cranTab.Length - 1;
    }
	
    void Update () {
		if (Input.GetKeyDown(KeyCode.M)) {
			minimap.SetActive (!minimap.activeSelf);
		}

        float rotation = Input.GetAxisRaw("Mouse ScrollWheel");
        // Mollette vers le haut
        if (rotation != 0)
            Zoom(rotation);

        // Replacement de la caméra si elle dépasse les limites
        if(transform.position.x < bounds[cran, minX] || transform.position.x > bounds[cran, maxX] || transform.position.z < bounds[cran, minZ] || transform.position.z > bounds[cran, maxZ])
        {
            Vector3 newPosition = transform.position;

            if (transform.position.x < bounds[cran, minX])
                newPosition.x = bounds[cran, minX];
            else if (transform.position.x > bounds[cran, maxX])
                newPosition.x = bounds[cran, maxX];

            if (transform.position.z < bounds[cran, minZ])
                newPosition.z = bounds[cran, minZ];
            else if (transform.position.z > bounds[cran, maxZ])
                newPosition.z = bounds[cran, maxZ];

            //Debug.Log("Position = " + transform.position + ", bounds = (" + bounds[cran, minX] + ", "+bounds[cran, maxX]+", "+bounds[cran, minZ]+", "+bounds[cran, maxZ]+"), newPosition = " + newPosition);

            transform.position = newPosition;
        }

        // Si la caméra n'est pas au cran le plus haut, le joueur peut se déplacer
        if (transform.position.y != 13.5f)
        {

            // Création d'un nouveau vecteur de déplacement
            Vector3 move = new Vector3(0.0f, 0.0f, 0.0f);

            // Récupération des touches haut et bas
            if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z)) && transform.position.z < bounds[cran, maxZ])
                move.z += 0.1f;
            if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && transform.position.z > bounds[cran, minZ])
                move.z -= 0.1f;

            // Récupération des touches gauche et droite
            if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q)) && transform.position.x > bounds[cran, minX])
                move.x -= 0.1f;
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && transform.position.x < bounds[cran, maxX])
                move.x += 0.1f;

            // On applique le mouvement à l'objet
            transform.position += move;
        }
        else
        {
            transform.position = new Vector3(0.0f, 13.5f, -4);
        }

    }

    /// <summary>
    /// Renvoie la nouvelle position de la caméra en fonction de la direction de la molette
    /// </summary>
    /// <param name="rotation">float La direction de rotation de la molette. rot > 0 : MWHEELUP, la caméra zoome; rot inférieur à 0 : MWHEELDOWN, la caméra dézoome.</param>
    public void Zoom(float rotation)
    {
        if (rotation < 0 && cran < cranTab.Length - 1)
        {
            cran++;
            if (cran == 1)
            {
                transform.position = new Vector3(transform.position.x, cranTab[cran], transform.position.z + 2.0f);
                transform.Rotate(new Vector3(35.0f, 0.0f, 0.0f));
            }
            transform.position = new Vector3(transform.position.x, cranTab[cran], transform.position.z);
        }
        else if (rotation > 0 && cran > 0)
        {
            cran--;
            if(cran == 0)
            {
                transform.position = new Vector3(transform.position.x, cranTab[cran], transform.position.z - 2.0f);
                transform.Rotate(new Vector3(-35.0f, 0.0f, 0.0f));
            }
            else
                transform.position = new Vector3(transform.position.x, cranTab[cran], transform.position.z);
        }
    }
}
