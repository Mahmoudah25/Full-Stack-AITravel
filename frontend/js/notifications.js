function initializeNotifications(customerEmail) {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl('https://aitravel.runasp.net/hubs/notifications')
        .withAutomaticReconnect()
        .build();

    connection.on("BookingStatusChanged", (data) => {
        showNotification(`Booking status changed to: ${data.newStatus}`);

        // If the current page defines a handler (e.g. trip-details.js), let it
        // react to the status change — e.g. mark an activity's button as Paid.
        if (typeof window.handleBookingStatusChanged === 'function') {
            window.handleBookingStatusChanged(data);
        }
    });

    connection.start()
        .then(() => connection.invoke("SendNotification", customerEmail))
        .catch(err => console.error("SignalR connection error:", err));
}

function showNotification(message) {
    const notifDiv = document.getElementById('notificationBanner');
    notifDiv.textContent = message;
    notifDiv.classList.add('show');

    setTimeout(() => {
        notifDiv.classList.remove('show');
    }, 5000);
}