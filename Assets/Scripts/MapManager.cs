using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

    /*
    private const float TILE_OFFSET = 1.0f;
    private const float TILE_SIZE = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;
    */
    private void Update()
    {
        /*UpdateSelection();*/
        /*DrawGrid();*/
    }

    /* Useless but who knows, right ?*/
    /* Credits goes to https://www.youtube.com/watch?v=CzImJk7ZesI
        private void UpdateSelection()
        {
            if (!Camera.main)
                return;

            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("BoardLayer")))
            {
                Debug.Log(hit.point);
            }
        }

        private void DrawGrid()
        {
            Vector3 widthLine = new Vector3(20, 0, 0);
            Vector3 heigthLine = new Vector3(0, 0, 10);
            Color color = new Color(1, 1, 1);


            for(int i = 0; i < 1000; i++)
            {

                Vector3 start = new Vector3(-10, 0, (i-500)/100f);
                Debug.DrawLine(start, start + widthLine, color);

                for (int j = 0; j < 1500; j++)
                {
                    start = new Vector3((j - 500) / 100f, 0, -5);
                    Debug.DrawLine(start, start + heigthLine, color);

                }
            }
        }
    */
}
