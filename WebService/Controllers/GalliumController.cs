﻿using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace GalliumPlus.WebService.Controllers;

/// <summary>
/// Contrôleur de base.
/// </summary>
public class GalliumController : ControllerBase
{
    private T? FindContextItem<T>(string name)
    where T : class
    {
        T? result = null;

        if (this.HttpContext.Items.TryGetValue(name, out object? value))
        {
            if (value is T expected)
            {
                result = expected;
            }
            else if (value is not null)
            {
                throw new InvalidOperationException(
                    $"The \"{name}\" item must be of type {typeof(T).FullName}, "
                    + $"not {value.GetType().FullName}"
                );
            }
        }

        return result;
    }

    /// <summary>
    /// L'utilisateur qui a émis la requête actuelle.
    /// </summary>
    public new User? User => this.FindContextItem<User>("User");

    [NonAction]
    public User RequireUser() => this.User 
        ?? throw new MissingIdentificationException("Cette action requiert un utilisateur.");

    /// <summary>
    /// La session de l'utilisateur qui a émis la requête actuelle.
    /// </summary>
    public Session? Session => this.FindContextItem<Session>("Session");
    
    [NonAction]
    public Session RequireSession() => this.Session
        ?? throw new MissingIdentificationException("Cette action requiert une session active.");

    /// <summary>
    /// L'application depuis laquelle la requête actuelle a été émise.
    /// </summary>
    public Client? Client => this.FindContextItem<Client>("Client");
    
    [NonAction]
    public Client RequireClient() => this.Client
        ?? throw new MissingIdentificationException("Cette action requiert une session active.");

    /// <summary>
    /// Crée une réponse avec un corps JSON.
    /// </summary>
    /// <param name="value">L'objet à sérialiser.</param>
    /// <param name="statusCode">Le statut de la réponse.</param>
    [NonAction]
    public IActionResult Json(object? value, int statusCode = StatusCodes.Status200OK)
    {
        return new JsonResult(value) { StatusCode = statusCode };
    }

    /// <summary>
    /// Crée une réponse avec un statut de 201.
    /// </summary>
    /// <param name="route">La route de la ressource crée.</param>
    /// <param name="routeValues">Les paramètres de la route.</param>
    /// <param name="value">La ressource créée.</param>
    [NonAction]
    public IActionResult Created(string route, object routeValues, object? value = null)
    {
        return new CreatedAtRouteResult(route, routeValues, value);
    }

    /// <summary>
    /// Crée une réponse avec un statut de 201.
    /// </summary>
    /// <param name="route">La route de la ressource crée.</param>
    /// <param name="id">Le paramètre id de la route.</param>
    /// <param name="value">La ressource créée.</param>
    [NonAction]
    public IActionResult Created(string route, int id, object? value = null)
        => this.Created(route, new { id }, value);

    /// <summary>
    /// Crée une réponse avec un statut de 201.
    /// </summary>
    /// <param name="route">La route de la ressource crée.</param>
    /// <param name="id">Le paramètre id de la route.</param>
    /// <param name="value">La ressource créée.</param>
    [NonAction]
    public IActionResult Created(string route, string id, object? value = null)
        => this.Created(route, new { id }, value);

    [NonAction]
    public void RequirePermissions(Permission required)
    {
        if (!Permissions.Current.IsSupersetOf(this.Session!.Permissions, Permissions.Current.Maximum(required)))
        {
            throw new PermissionDeniedException(required);
        }
    }
}