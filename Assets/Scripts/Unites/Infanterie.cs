using UnityEngine;
using System.Collections;

public class Infanterie : Terrestre {

    public readonly static int COUT = 1;

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
        Material[] materials = transform.GetChild(transform.childCount - 1).GetComponent<Renderer>().materials;

        for (int i = 0; i < materials.Length; i++)
            materials[i] = material;

        transform.GetChild(transform.childCount - 1).GetComponent<Renderer>().materials = materials;
    }
}
