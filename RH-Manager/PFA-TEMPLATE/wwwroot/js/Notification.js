document.addEventListener('DOMContentLoaded', function () {
    // Get the current user's employee ID
    const employeeId = document.getElementById('current-employee-id')?.value;

    if (!employeeId) {
        console.warn('Employee ID not found, notifications will not work');
        return;
    }

    // Create connection to the hub
    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/notificationHub')
        .withAutomaticReconnect()
        .build();

    // Start connection and join employee group
    connection.start()
        .then(() => {
            console.log('Connected to notification hub');
            // Join group for this employee to receive notifications
            return connection.invoke('JoinEmployeeGroup', parseInt(employeeId));
        })
        .catch(err => console.error('Error connecting to notification hub:', err));

    // Listen for notifications
    connection.on('ReceiveNotification', (notification) => {
        console.log('Received notification:', notification);
        // Update notification counter in the navbar
        updateNotificationCounter();

        // Show a toast or notification popup
        showNotificationToast(notification);

        // Update notification dropdown if it's open
        if (document.getElementById('notificationsList').classList.contains('show')) {
            loadNotifications();
        }
    });

    // Function to update the notification counter
    function updateNotificationCounter() {
        fetch('/Notification/GetUnreadCount')
            .then(response => response.json())
            .then(count => {
                const badge = document.querySelector('#notificationsDropdown .badge');
                if (count > 0) {
                    if (!badge) {
                        const newBadge = document.createElement('span');
                        newBadge.className = 'badge badge-danger';
                        newBadge.textContent = count;
                        document.getElementById('notificationsDropdown').appendChild(newBadge);
                    } else {
                        badge.textContent = count;
                    }
                } else if (badge) {
                    badge.remove();
                }
            })
            .catch(error => console.error('Error updating notification counter:', error));
    }

    // Function to load notifications into dropdown
    function loadNotifications() {
        const container = document.getElementById('notifications-container');
        if (!container) return;

        fetch('/Notification/GetLatestNotifications')
            .then(response => response.json())
            .then(notifications => {
                if (notifications.length === 0) {
                    container.innerHTML = '<div class="text-center p-2">Pas de nouvelles notifications</div>';
                    return;
                }

                let html = '';
                notifications.forEach(notification => {
                    const isReadClass = notification.isRead ? '' : 'font-weight-bold bg-light';
                    html += `
                        <a class="dropdown-item ${isReadClass}" href="/Notification/GoToNotification/${notification.id}">
                            <div>${notification.message}</div>
                            <small class="text-muted">${new Date(notification.createdAt).toLocaleString()}</small>
                        </a>
                        <div class="dropdown-divider"></div>
                    `;
                });

                container.innerHTML = html;
            })
            .catch(error => {
                console.error('Error loading notifications:', error);
                container.innerHTML = '<div class="text-center p-2">Erreur de chargement</div>';
            });
    }

    // Load notifications when dropdown is opened
    document.getElementById('notificationsDropdown').addEventListener('click', function () {
        loadNotifications();
    });

    // Function to show a toast notification
    function showNotificationToast(notification) {
        // Check if we have a notification system like Toastr or use browser notifications
        if (typeof toastr !== 'undefined') {
            toastr.info(notification.message, 'Nouvelle notification');
        } else if ('Notification' in window && Notification.permission === 'granted') {
            new Notification('Nouvelle tâche', { body: notification.message });
        } else {
            // Fallback: create a simple toast
            const toast = document.createElement('div');
            toast.className = 'notification-toast';
            toast.innerHTML = `
                <div class="notification-toast-header">
                    <strong>Nouvelle notification</strong>
                    <button type="button" class="close">&times;</button>
                </div>
                <div class="notification-toast-body">
                    ${notification.message}
                </div>
            `;

            document.body.appendChild(toast);

            // Add styles if not already added
            if (!document.getElementById('notification-toast-styles')) {
                const style = document.createElement('style');
                style.id = 'notification-toast-styles';
                style.innerHTML = `
                    .notification-toast {
                        position: fixed;
                        bottom: 20px;
                        right: 20px;
                        min-width: 300px;
                        background-color: white;
                        border-radius: 4px;
                        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                        z-index: 9999;
                        overflow: hidden;
                        animation: toast-in 0.3s, toast-out 0.3s 5s forwards;
                    }
                    .notification-toast-header {
                        display: flex;
                        justify-content: space-between;
                        align-items: center;
                        padding: 10px 15px;
                        background-color: #f8f9fa;
                        border-bottom: 1px solid #dee2e6;
                    }
                    .notification-toast-body {
                        padding: 15px;
                    }
                    .notification-toast .close {
                        cursor: pointer;
                        background: none;
                        border: none;
                        font-size: 20px;
                    }
                    @keyframes toast-in {
                        from { transform: translateX(100%); }
                        to { transform: translateX(0); }
                    }
                    @keyframes toast-out {
                        from { transform: translateX(0); }
                        to { transform: translateX(100%); }
                    }
                `;
                document.head.appendChild(style);
            }

            // Auto remove toast after 5 seconds
            setTimeout(() => {
                toast.remove();
            }, 5300);

            // Close button
            toast.querySelector('.close').addEventListener('click', () => {
                toast.remove();
            });
        }
    }

    // Initial load of notification count
    updateNotificationCounter();
});