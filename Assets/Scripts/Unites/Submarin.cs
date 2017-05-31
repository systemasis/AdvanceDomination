using UnityEngine;
using System.Collections.Generic;

public class Submarin : Maritime {

    public readonly static int COUT = 10;

    protected override void OnStart()
    {
        base.OnStart();
        transform.localScale = new Vector3(0.3f, 0.5f, 0.5f);
        transform.localPosition = new Vector3(41.2f, transform.localPosition.y, transform.localPosition.z);
        angle = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public override void ApplyMaterial(Material material)
    {
        for (int i = 0; i < transform.childCount; i++)
            if(transform.GetChild(i).GetComponent<Renderer>() != null)
                transform.GetChild(i).GetComponent<Renderer>().material = material;
    }

    /// <summary>
    /// Lance l'attaque d'une route.
    /// </summary>
    /// <param name="route">Route La route à attaquer.</param>
    protected override void Attack(Route toAttack)
    {
        if (toAttack.joueur != joueur)
        {
            if (CanAttackTogether(toAttack))
            {
                guiManager.GetComponent<GUIController>().TargetsSelection(toAttack);

                //battleManager.LaunchAttack(submarines, toAttack);
            }
            else
                InvalidPositioning("Certaines unités ne peuvent attaquer la cible.");
        }
        else
            InvalidPositioning("Vous ne pouvez attaquer votre propre territoire.");
    }
}
