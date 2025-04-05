using PusherServer;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace PFA_TEMPLATE.Services
{
    public class PusherService
    {
        private readonly Pusher _pusher;

        public PusherService(IConfiguration configuration)
        {
            var options = new PusherOptions
            {
                Cluster = configuration["Pusher:Cluster"],
                Encrypted = true
            };

            _pusher = new Pusher(
                configuration["Pusher:AppId"],
                configuration["Pusher:Key"],
                configuration["Pusher:Secret"],
                options
            );
        }

        public async Task TriggerAsync(string channelName, string eventName, object data)
        {
            await _pusher.TriggerAsync(channelName, eventName, data);
        }

        public string Authenticate(string channelName, string socketId)
        {
            return _pusher.Authenticate(channelName, socketId).ToJson();
        }
    }
}