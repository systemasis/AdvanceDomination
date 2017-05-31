using UnityEngine;
using System.Collections;

public class Bombardier : Aerienne {
    
    public readonly static int COUT = 15;

    protected override void OnStart()
    {
        base.OnStart();
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        angle = new Vector3(0, 180, 0);

        // Replace l'unité à l'endroit
        transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, angle, 1);
    }

    /// <summary>
    /// Applique material aux sous-parties du bombardier.
    /// </summary>
    /// <param name="material">Material Le material à appliquer.</param>
    public override void ApplyMaterial(Material material)
    {
        for (int i = 0; i < 3; i++)
            if(transform.GetChild(i).GetComponent<Renderer>() != null)
                transform.GetChild(i).GetComponent<Renderer>().material = joueur.Material;
    }
}
