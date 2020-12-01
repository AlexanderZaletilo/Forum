using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Forum.Models;
using Microsoft.AspNetCore.Identity;

namespace Forum.Hubs
{
    public class DialogHub : Hub
    {
        private UserManager<User> _manager;
        public DialogHub(UserManager<User> manager)
        {
            _manager = manager;
        }
        public async Task Initialize(string receiverid)
        {
            User sender = await _manager.FindByNameAsync(Context.User.Identity.Name);
            User receiver = await _manager.FindByIdAsync(receiverid);
            Context.Items["senderName"] = sender.UserName;
            string first = sender.Id, second = receiver.Id;
            if (first.CompareTo(second) > 0)
            {
                var tmp = first;
                first = second;
                second = tmp;
            }
            Context.Items["group"] = $"{first}.{second}";
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.Items["group"].ToString());
        }
        public async Task Send(string message)
        {
            await Clients.Group(Context.Items["group"].ToString()).SendAsync(
                "Notify", Context.Items["senderName"], message, System.DateTime.UtcNow.ToString()
            );
        }
    }
}