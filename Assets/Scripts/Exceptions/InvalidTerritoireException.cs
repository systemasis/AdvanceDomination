using System;

class InvalidTerritoireException : Exception
{
    public InvalidTerritoireException(string message) : base(message)
    {
    }
}