using System.Collections.Generic;
using TakedownTCG.cli.Views.Menus;

namespace TakedownTCG.cli.Services.Api
{
    /// <summary>
    /// Central registry that maps menu actions to API client factories.
    /// It keeps controller code simple by hiding client construction and
    /// lets you add new APIs by registering them in one place.
    /// </summary>
    public static class ApiRegistry
    {
        /// <summary>
        /// Stores factories for each menu action. Each factory returns a new client instance when resolved.
        /// </summary>
        private static readonly Dictionary<ApiMenu.Action, Func<IApiClient>> Factories = new();

        /// <summary>
        /// Registers an API client type against a menu action.
        /// </summary>
        /// <typeparam name="TClient">Concrete client type to construct.</typeparam>
        /// <param name="action">Menu action key to map to the client.</param>
        public static void Register<TClient>(ApiMenu.Action action)
            where TClient : IApiClient, new()
        {
            // Store a factory so callers get a fresh client per resolve.
            Factories[action] = static () => new TClient();
        }

        /// <summary>
        /// Registers an API client factory against a menu action.
        /// </summary>
        public static void Register(ApiMenu.Action action, Func<IApiClient> factory)
        {
            Factories[action] = factory;
        }

        /// <summary>
        /// Resolves a registered client for the given action.
        /// </summary>
        /// <param name="action">Menu action that identifies the API.</param>
        /// <returns>Client instance matching the requested types.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when no action is registered.</exception>
        public static IApiClient Resolve(ApiMenu.Action action)
        {
            if (!Factories.TryGetValue(action, out Func<IApiClient>? factory) || factory == null)
            {
                throw new KeyNotFoundException($"No API registered for action: {action}");
            }

            return factory();
        }
    }
}
