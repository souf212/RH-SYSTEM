document.addEventListener("DOMContentLoaded", function () {
    fetch('/Notification/GetUnreadCount')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Response data:', data); // Debugging

            // Update the sidebar badge
            const sidebarBadge = document.getElementById('notification-badge-sidebar');
            if (sidebarBadge) {
                if (data.count > 0) {
                    sidebarBadge.textContent = data.count;
                    sidebarBadge.style.display = 'inline-block';
                } else {
                    sidebarBadge.style.display = 'none';
                }
            }

            // Update the navbar badge
            const navbarBadge = document.getElementById('notification-badge-navbar');
            if (navbarBadge) {
                if (data.count > 0) {
                    navbarBadge.textContent = data.count;
                    navbarBadge.style.display = 'inline-block';
                } else {
                    navbarBadge.style.display = 'none';
                }
            }
        })
        .catch(error => {
            console.error('Error fetching unread notification count:', error);
        });
});
 
        function markAllAsRead(event) {
            event.preventDefault(); // Prevent the default form submission or navigation

        fetch('/Notification/MarkAllAsRead', {
            method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
        return response.json();
            })
            .then(data => {
                if (data.success) {
                    // Update the UI to reflect that all notifications are read
                    const notificationItems = document.querySelectorAll('.list-group-item-primary');
                    notificationItems.forEach(item => {
            item.classList.remove('list-group-item-primary');
                    });

        // Hide the "Mark All as Read" button
        const markAllButton = document.querySelector('button[onclick="markAllAsRead(event)"]');
        if (markAllButton) {
            markAllButton.style.display = 'none';
                    }

        // Update the notification badge count
        const sidebarBadge = document.getElementById('notification-badge-sidebar');
        const navbarBadge = document.getElementById('notification-badge-navbar');
        if (sidebarBadge) sidebarBadge.style.display = 'none';
        if (navbarBadge) navbarBadge.style.display = 'none';

        // Optionally, show a success message
        alert('All notifications have been marked as read.');
                } else {
            alert('Failed to mark all notifications as read.');
                }
            })
            .catch(error => {
            console.error('Error:', error);
        alert('An error occurred while marking notifications as read.');
            });
        }