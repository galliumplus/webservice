using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Middleware.ErrorHandling
{
    public class ErrorResult : JsonResult
    {
        /// <summary>
        /// Représente une erreur.
        /// </summary>
        private class Error
        {
            /// <summary>
            /// Un code identifiant le type d'erreur, composé de lettres majuscules et de soulignés.
            /// </summary>
            public string Code { get; set; } = "UNKNOWN";

            /// <summary>
            /// Une phrase expliquant l'erreur.
            /// </summary>
            public string Message { get; set; } = "Erreur inconnue.";

            /// <summary>
            /// Un objet additionel contenant des informations sur l'erreur
            /// (utile pour le déboguage).
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public object? DebugInfo { get; set; } = null;
        }

        /// <summary>
        /// Construit une réponse d'erreur.
        /// </summary>
        /// <param name="errorCode">Le code de l'erreur (voir <see cref="Error.Code"/>)</param>
        /// <param name="errorMessage">Le message d'erreur (voir <see cref="Error.Message"/>)</param>
        /// <param name="statusCode">Le statut HTTP de la réponse.</param>
        /// <param name="data">Données additionnelles pour le déboguage</param>
        /// <returns></returns>
        public ErrorResult(
            string errorCode,
            string errorMessage,
            int statusCode,
            object? errorData = null)
        : base(null)
        {
            Value = new Error { Code = errorCode, Message = errorMessage, DebugInfo = errorData };
            StatusCode = statusCode;
        }
    }
}
