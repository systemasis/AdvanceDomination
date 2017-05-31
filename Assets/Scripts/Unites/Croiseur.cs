using UnityEngine;
using System.Collections;

public class Croiseur : Maritime {

    public readonly static int COUT = 15;

    protected override void OnStart()
    {
        base.OnStart();

        transform.localScale = new Vector3(0.01f, 0.01f, 0.02f);

        angle = new Vector3(-90.0f, -90.0f, 180.0f);
        transform.eulerAngles = angle;
    }
}
