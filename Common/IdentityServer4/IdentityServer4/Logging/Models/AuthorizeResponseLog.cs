﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using KC.IdentityServer4.Extensions;
using KC.IdentityServer4.ResponseHandling;

namespace KC.IdentityServer4.Logging.Models
{
    internal class AuthorizeResponseLog
    {
        public string SubjectId { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string State { get; set; }

        public string Scope { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }


        public AuthorizeResponseLog(AuthorizeResponse response)
        {
            ClientId = response.Request?.Client?.ClientId;
            SubjectId = response.Request?.Subject?.GetSubjectId();
            RedirectUri = response.RedirectUri;
            State = response.State;
            Scope = response.Scope;
            Error = response.Error;
            ErrorDescription = response.ErrorDescription;
        }

        public override string ToString()
        {
            return LogSerializer.Serialize(this);
        }
    }
}