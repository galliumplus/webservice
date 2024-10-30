using System.Text.Json.Serialization;
using GalliumPlus.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Middleware.ErrorHandling
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
        /// <param name="errorData">Données additionnelles pour le déboguage</param>
        /// <returns></returns>
        public ErrorResult(
            ErrorCode errorCode,
            string errorMessage,
            int statusCode,
            object? errorData = null)
        : base(null)
        {
            this.Value = new Error { Code = errorCode.ToString(), Message = errorMessage, DebugInfo = errorData };
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Construit une réponse d'erreur à partir d'une GalliumException.
        /// </summary>
        /// <param name="exception">GalliumException sur laquelle l'erreur est basée</param>
        /// <param name="statusCode">Le statut HTTP de la réponse.</param>
        public ErrorResult(GalliumException exception, int statusCode) : base(null)
        {
            this.Value = new Error { Code = exception.ErrorCode.ToString(), Message = exception.Message };
            this.StatusCode = statusCode;
        }
    }
}
