using Microsoft.Extensions.Logging;
using System;

namespace Com.WebTest.Multitenancy
{
    public interface IMessageService
    {
        string Id { get; }
        string Format(string message);
    }

    public class MessageService : IMessageService, IDisposable
    {
        ILogger<MessageService> log;

        public MessageService(ILogger<MessageService> log)
        {
            this.log = log;
        }

        public string Id { get; set; }

        public void Dispose()
        {
            log.LogInformation("Disposing MessageSerivce:{id}", Id);
        }

        public string Format(string message)
        {
            return $"{Id}: {message}";
        }
    }

    public class OtherMessageService : IMessageService
    {
        public string Id { get; set; } 

        public string Format(string message)
        {
            return "Other";
        }
    }
}
