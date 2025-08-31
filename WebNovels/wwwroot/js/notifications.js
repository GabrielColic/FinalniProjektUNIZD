window.webnovelsNotifications = (function () {
    let connection = null, dotNetRef = null;

    async function start() {
        if (connection) return;
        if (!window.signalR) {
            console.error("SignalR client not found. Load signalr.min.js before notifications.js");
            return;
        }
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/notifications")
            .withAutomaticReconnect()
            .build();

        connection.on("NewNotification", (payload) => {
            if (dotNetRef) dotNetRef.invokeMethodAsync("OnNewNotification", payload);
        });

        try { await connection.start(); }
        catch (err) { console.error("Notification hub connect failed:", err); }
    }

    function subscribe(ref) { dotNetRef = ref; }
    function unsubscribe() { dotNetRef = null; }

    return { start, subscribe, unsubscribe };
})();
