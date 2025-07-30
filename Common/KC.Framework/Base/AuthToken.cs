namespace KC.Framework.Base
{
    public class AuthToken 
    {
        public string Id { get; set; }

        public string Tenant { get; set; }

        public bool Internal { get; set; }

        public string DomainName { get; set; }

        public string ContainerName { get; set; }

        public string EncryptionKey { get; set; }
    }
}
