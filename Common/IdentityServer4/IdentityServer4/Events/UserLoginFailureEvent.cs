﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using static KC.IdentityServer4.Constants;

namespace KC.IdentityServer4.Events
{
    /// <summary>
    /// Event for failed user authentication
    /// </summary>
    /// <seealso cref="KC.IdentityServer4.Events.Event" />
    public class UserLoginFailureEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:KC.IdentityServer4.Events.UserLoginFailureEvent" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="error">The error.</param>
        /// <param name="interactive">Specifies if login was interactive</param>
        /// <param name="clientId">The client id</param>
        public UserLoginFailureEvent(string username, string error, bool interactive = true, string clientId = null)
            : base(EventCategories.Authentication,
                  "User Login Failure",
                  EventTypes.Failure, 
                  EventIds.UserLoginFailure,
                  error)
        {
            Username = username;
            ClientId = clientId;

            if (interactive)
            {
                Endpoint = "UI";
            }
            else
            {
                Endpoint = EndpointNames.Token;
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public string ClientId { get; set; }
    }
}
