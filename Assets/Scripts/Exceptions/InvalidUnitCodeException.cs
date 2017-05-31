using UnityEngine;
using System.Collections;
using System;

public class InvalidUnitCodeException : Exception {

    public InvalidUnitCodeException(int code) : base("Un int allant de 0 à 6 était attendu, " + code + " reçu.")
    {

    }
}
