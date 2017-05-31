using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Utils
{
    static Texture2D _whiteTexture;

    /// <summary>
    /// Énumération regroupant le code de chaque unité
    /// </summary>
    public enum unitCode
    {
        Infanterie,
        Tank,
        Artillerie,
        DCA,
        Croiseur,
        Submarin,
        Bombardier
    };

    /// <summary>
    /// Énumération regroupant le code de chaque unité
    /// </summary>
    [FlagsAttribute]
    public enum couleurs
    {
        rouge,
        bleue,
        jaune,
        vert,
        gris,
        violet
    };

    /// <summary>
    /// Retourne le nom de l'unité correspondant à code
    /// </summary>
    /// <param name="code">int Code de l'unité</param>
    /// <returns>string Le nom de l'unité</returns>
    public static string GetUnitsNameByCode(int code)
    {
        switch (code)
        {
            case ((int) unitCode.Infanterie):
                return "Infanterie";
            case ((int) unitCode.Tank):
                return "Tank";
            case ((int) unitCode.Artillerie):
                return "Artillerie";
            case ((int) unitCode.DCA):
                return "DCA";
            case ((int) unitCode.Croiseur):
                return "Croiseur";
            case ((int) unitCode.Submarin):
                return "Sous-marin";
            case ((int) unitCode.Bombardier):
                return "Bombardier";
            default:
                throw new InvalidUnitCodeException(code);
        }
    }

    /// <summary>
    /// Retourne le prix de l'unité référencée par code
    /// </summary>
    /// <param name="code">int Le code de l'unité</param>
    /// <returns>int Le prix de l'unité référencée par code</returns>
    public static int GetUnitPrice(int code)
    {
        switch (code)
        {
            case ((int) unitCode.Infanterie):
                return Infanterie.COUT;
            case ((int) unitCode.Tank):
                return Tank.COUT;
            case ((int) unitCode.Artillerie):
                return Artillerie.COUT;
            case ((int) unitCode.DCA):
                return Dca.COUT;
            case ((int) unitCode.Croiseur):
                return Croiseur.COUT;
            case ((int) unitCode.Submarin):
                return Submarin.COUT;
            case ((int) unitCode.Bombardier):
                return Bombardier.COUT;
            default:
                throw new InvalidUnitCodeException(code);
        }
    }

    public static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    public static Rect GetScreenRect( Vector3 screenPosition1, Vector3 screenPosition2 )
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min( screenPosition1, screenPosition2 );
        var bottomRight = Vector3.Max( screenPosition1, screenPosition2 );
        // Create Rect
        return Rect.MinMaxRect( topLeft.x, topLeft.y, bottomRight.x, bottomRight.y );
    }

    public static Bounds GetViewportBounds( Camera camera, Vector3 screenPosition1, Vector3 screenPosition2 )
    {
        var v1 = camera.ScreenToViewportPoint( screenPosition1 );
        var v2 = camera.ScreenToViewportPoint( screenPosition2 );
        var min = Vector3.Min( v1, v2 );
        var max = Vector3.Max( v1, v2 );
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;
        //min.z = 0.0f;
        //max.z = 1.0f;

        var bounds = new Bounds();
        bounds.SetMinMax( min, max );
        return bounds;
    }

    public static void DrawScreenRect( Rect rect, Color color )
    {
        GUI.color = color;
        GUI.DrawTexture( rect, WhiteTexture );
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder( Rect rect, float thickness, Color color )
    {
        // Top
        Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMin, rect.width, thickness ), color );
        // Left
        Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMin, thickness, rect.height ), color );
        // Right
        Utils.DrawScreenRect( new Rect( rect.xMax - thickness, rect.yMin, thickness, rect.height ), color );
        // Bottom
        Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMax - thickness, rect.width, thickness ), color );
    }

    public static Color32 GetColor(Utils.couleurs couleur)
    {
        if (couleur == Utils.couleurs.rouge)
            return new Color32(255, 0, 0, 255);
        else if (couleur == Utils.couleurs.bleue)
            return new Color32(0, 220, 255, 255);
        else if (couleur == Utils.couleurs.vert)
            return new Color32(0, 255, 0, 255);
        else if (couleur == Utils.couleurs.jaune)
            return new Color32(255, 255, 0, 255);
        else if (couleur == Utils.couleurs.violet)
            return new Color32(165, 0, 255, 255);
        else
            return new Color32(128, 128, 128, 255);
    }
}
