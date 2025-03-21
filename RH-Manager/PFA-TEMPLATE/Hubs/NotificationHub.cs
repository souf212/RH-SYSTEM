using Microsoft.AspNetCore.SignalR;

namespace PFA_TEMPLATE.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotificationToEmployee(int employeeId, object notificationData)
        {
            await Clients.Group(employeeId.ToString()).SendAsync("ReceiveNotification", notificationData);
        }

        public async Task JoinEmployeeGroup(int employeeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, employeeId.ToString());
        }
    }


}