using UnityEngine;
using System;

public class InvalidColorException : Exception
{
    public InvalidColorException(Utils.couleurs couleur) : base("La couleur " + couleur + " n'est pas supportée.")
    {

    }
}