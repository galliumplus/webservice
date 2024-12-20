﻿using GalliumPlus.Core.Exceptions;

namespace GalliumPlus.Core;

/// <summary>
/// Classe d'aide à la validation des valeurs monétaires.
/// </summary>
public static class MonetaryValue
{
    /// <summary>
    /// Vérifie qu'un nombre décimal n'as pas plus de deux chiffres après la virgule.
    /// </summary>
    /// <param name="euros">La valeur à vérifier.</param>
    /// <param name="description">
    /// Une description de la valeur à vérifier, utilisée pour générer des messages d'erreurs plus précis.
    /// </param>
    /// <returns>Exactement la valeur passée en entrée.</returns>
    /// <exception cref="InvalidResourceException"></exception>
    public static decimal Check(decimal euros, string description = "Une valeur en Euros")
    {
        decimal cents = euros * 100;
        if (cents % 1 != 0)
        {
            throw new InvalidResourceException($"{description} ne peux pas avoir des fractions de centimes.");
        }
        return euros;
    }

    /// <summary>
    /// Vérifie qu'un nombre décimal n'as pas plus de deux chiffres après la virgules et est positif.
    /// </summary>
    /// <param name="euros">La valeur à vérifier.</param>
    /// <param name="description">
    /// Une description de la valeur à vérifier, utilisée pour générer des messages d'erreurs plus précis.
    /// </param>
    /// <returns>Exactement la valeur passée en entrée.</returns>
    /// <exception cref="InvalidResourceException"></exception>
    public static decimal CheckNonNegative(decimal euros, string description)
    {
        if (euros < 0)
        {
            throw new InvalidResourceException($"{description} ne peux pas être négatif.");
        }
        return Check(euros, description);
    }
}