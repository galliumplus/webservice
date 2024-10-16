﻿namespace GalliumPlus.Core.Exceptions;

public abstract class GalliumException : Exception
{
    /// <summary>
    /// Code de l'erreur levée 
    /// </summary>
    public abstract ErrorCode ErrorCode { get; }

    /// <summary>
    /// Constructeur par défaut
    /// </summary>
    public GalliumException() : base() { }

    /// <summary>
    /// Constructeur d'une GalliumException avec un message
    /// </summary>
    /// <param name="message">String decrivant la raison </param>
    public GalliumException(string message) : base(message) { }
}