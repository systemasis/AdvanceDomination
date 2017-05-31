using UnityEngine;
using System.Collections;

public class Artillerie : Terrestre {

    public readonly static int COUT = 10;

    protected override void OnStart()
    {
        base.OnStart();
        this.transform.localScale = new Vector3(0.17f, 0.25f, 0.075f);

    }
}
