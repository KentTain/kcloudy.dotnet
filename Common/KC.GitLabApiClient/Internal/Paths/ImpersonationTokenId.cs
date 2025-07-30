using System;
using KC.GitLabApiClient.Models.Users.Responses;

namespace KC.GitLabApiClient.Internal.Paths
{
    public class ImpersonationTokenId
    {
        private readonly string _id;

        private ImpersonationTokenId(string tokenId) => _id = tokenId;

        public static implicit operator ImpersonationTokenId(int tokenId)
        {
            return new ImpersonationTokenId(tokenId.ToString());
        }

        public static implicit operator ImpersonationTokenId(ImpersonationToken token)
        {
            int id = token.Id;
            if (id > 0)
                return new ImpersonationTokenId(id.ToString());

            throw new ArgumentException("Cannot determine token id from provided ImpersonationToken instance.");
        }

        public override string ToString()
        {
            return _id;
        }
    }
}
