using System;
using System.Runtime.Serialization;

[Serializable]
internal class InvalidRouteException : Exception
{
    public InvalidRouteException(string message) : base(message)
    {
    }
}