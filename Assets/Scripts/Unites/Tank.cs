using UnityEngine;
using System.Collections;

public class Tank : Terrestre {

    public readonly static int COUT = 7;

    protected override void OnStart()
    {
        base.OnStart();
        this.transform.localScale = new Vector3(0.17f, 0.2f, 0.2f);
        angle = new Vector3(0, 180, 0);

        transform.eulerAngles = angle;
    }

    /// <summary>
    /// Applique material à l'unité
    /// </summary>
    /// <param name="material">Material Le material à appliquer.</param>
    public override void ApplyMaterial(Material material)
    {
        transform.GetComponentInChildren<Renderer>().material = material;
    }
}
