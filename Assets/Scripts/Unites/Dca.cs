using UnityEngine;
using System.Collections;

public class Dca : Terrestre {

    public readonly static int COUT = 10;

    protected override void OnStart()
    {
        base.OnStart();
        this.transform.localScale = new Vector3(0.15f, 0.2f, 0.075f);

        angle = new Vector3(-90f, 0.0f, 0.0f);
    }

    public override void Attack(Territoire toAttack)
    {
        InvalidPositioning("La DCA n'a qu'un rôle défensif et ne peux attaquer un territoire.");
    }
}
