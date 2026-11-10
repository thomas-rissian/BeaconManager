(function ($, baliseManager) {
    "use strict";

    const webSocketPath = "/ws/events";
    const initialReconnectDelay = 1000;
    const maximumReconnectDelay = 30000;
    let webSocket = null;
    let reconnectTimer = null;
    let reconnectDelay = initialReconnectDelay;

    function getWebSocketUrl() {
        const protocol = window.location.protocol === "https:" ? "wss:" : "ws:";
        return protocol + "//" + window.location.host + webSocketPath;
    }

    function createNotificationContainer() {
        if ($("[data-notification-container]").length > 0) {
            return;
        }

        $("body").append($("<aside>", {
            class: "notification-container",
            "data-notification-container": "",
            "aria-live": "polite",
            "aria-label": "Notifications"
        }));
    }

    function displayNotification(beaconEvent) {
        const notification = $("<article>", {
            class: "notification-item"
        });
        const notificationHeader = $("<div>", {
            class: "notification-header"
        });
        const eventType = $("<strong>").text(beaconEvent.eventType);
        const closeButton = $("<button>", {
            class: "notification-close",
            type: "button",
            "aria-label": "Fermer la notification",
            text: "×"
        });
        const eventMessage = $("<p>").text(beaconEvent.message);
        const eventDate = $("<time>").text(
            formatNotificationDate(beaconEvent.occurredAtUtc));

        closeButton.on("click", function () {
            notification.remove();
        });
        notificationHeader.append(eventType, closeButton);
        notification.append(notificationHeader, eventMessage, eventDate);
        $("[data-notification-container]").prepend(notification);

        window.setTimeout(function () {
            notification.fadeOut(300, function () {
                notification.remove();
            });
        }, 8000);
    }

    function formatNotificationDate(dateValue) {
        const date = new Date(dateValue);

        return Number.isNaN(date.getTime())
            ? "Date inconnue"
            : date.toLocaleTimeString("fr-FR", {
                hour: "2-digit",
                minute: "2-digit"
            });
    }

    function isBeaconEvent(value) {
        return value !== null
            && typeof value === "object"
            && Number.isInteger(value.id)
            && typeof value.eventType === "string"
            && typeof value.message === "string"
            && typeof value.occurredAtUtc === "string";
    }

    function handleMessage(messageEvent) {
        let beaconEvent;

        try {
            beaconEvent = JSON.parse(messageEvent.data);
        } catch (error) {
            return;
        }

        if (!isBeaconEvent(beaconEvent)) {
            return;
        }

        displayNotification(beaconEvent);
        $(document).trigger("baliseManager:beacon-event", [beaconEvent]);
    }

    function scheduleReconnect() {
        if (reconnectTimer !== null) {
            return;
        }

        reconnectTimer = window.setTimeout(function () {
            reconnectTimer = null;
            connect();
        }, reconnectDelay);
        reconnectDelay = Math.min(reconnectDelay * 2, maximumReconnectDelay);
    }

    function connect() {
        if (webSocket !== null
            && (webSocket.readyState === WebSocket.OPEN
                || webSocket.readyState === WebSocket.CONNECTING)) {
            return;
        }

        webSocket = new WebSocket(getWebSocketUrl());

        webSocket.addEventListener("open", function () {
            reconnectDelay = initialReconnectDelay;
            baliseManager.setConnectionState(true);
        });

        webSocket.addEventListener("message", handleMessage);

        webSocket.addEventListener("error", function () {
            baliseManager.setConnectionState(false);
        });

        webSocket.addEventListener("close", function () {
            webSocket = null;
            baliseManager.setConnectionState(false);
            scheduleReconnect();
        });
    }

    $(function () {
        createNotificationContainer();
        connect();
    });
}(jQuery, window.baliseManager));
