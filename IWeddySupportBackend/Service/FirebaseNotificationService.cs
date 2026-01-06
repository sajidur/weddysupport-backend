using FirebaseAdmin.Messaging;

namespace IWeddySupport.Service
{


    public interface IFirebaseNotificationService
    {
        Task SendAsync(string deviceToken, string title, string body,
                       Dictionary<string, string>? data = null);
    }

    public class FirebaseNotificationService : IFirebaseNotificationService
    {
        public async Task SendAsync(string deviceToken, string title, string body,
                                    Dictionary<string, string>? data = null)
        {
            if (string.IsNullOrWhiteSpace(deviceToken))
                return;

            var message = new Message
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }

}
