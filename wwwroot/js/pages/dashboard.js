(function ($, baliseManager) {
    "use strict";

    function displaySummary(summary) {
        $("[data-summary-value='totalBeacons']").text(summary.totalBeacons);
        $("[data-summary-value='activeBeacons']").text(summary.activeBeacons);
        $("[data-summary-value='inactiveBeacons']").text(summary.inactiveBeacons);
        $("[data-summary-value='maintenanceBeacons']").text(summary.maintenanceBeacons);
        $("[data-summary-value='unreadEvents']").text(summary.unreadEvents);
    }

    function createEventItem(event) {
        const eventItem = $("<article>", { class: "event-item" });
        const eventHeader = $("<div>", { class: "event-item-header" });
        const eventType = $("<strong>").text(event.eventType);
        const eventDate = $("<time>")
            .attr("datetime", event.occurredAtUtc)
            .text(baliseManager.formatters.date(event.occurredAtUtc));
        const eventMessage = $("<p>").text(event.message);

        eventHeader.append(eventType, eventDate);
        eventItem.append(eventHeader, eventMessage);

        if (!event.isRead) {
            eventItem.addClass("is-unread");
        }

        return eventItem;
    }

    function displayRecentEvents(events) {
        const eventContainer = $("[data-recent-events]");
        eventContainer.empty();

        if (events.length === 0) {
            eventContainer.append($("<p>", {
                class: "muted-text",
                text: "Aucun événement récent."
            }));
            return;
        }

        $.each(events, function (eventIndex, event) {
            eventContainer.append(createEventItem(event));
        });
    }

    function createBatteryItem(beacon) {
        const batteryItem = $("<article>", { class: "battery-item" });
        const batteryHeader = $("<div>", { class: "battery-item-header" });
        const beaconName = $("<strong>").text(beacon.name);
        const batteryLabel = $("<span>")
            .text(baliseManager.formatters.batteryLevel(beacon.batteryLevel));

        batteryHeader.append(beaconName, batteryLabel);
        batteryItem.append(batteryHeader);

        return batteryItem;
    }

    function displayBatteryStatus(beacons) {
        const batteryContainer = $("[data-battery-status]");
        batteryContainer.empty();

        if (beacons.length === 0) {
            batteryContainer.append($("<p>", {
                class: "muted-text",
                text: "Aucune balise enregistrée."
            }));
            return;
        }

        $.each(beacons, function (beaconIndex, beacon) {
            batteryContainer.append(createBatteryItem(beacon));
        });
    }

    function displayLoadingError(xhr) {
        const errorMessage = baliseManager.getApiErrorMessage(xhr);
        $("[data-dashboard-status]")
            .removeClass("muted-text")
            .addClass("dashboard-error")
            .text(errorMessage);
    }

    function loadDashboard() {
        $.when(
            baliseManager.api.dashboard.getSummary(),
            baliseManager.api.dashboard.getRecentEvents(),
            baliseManager.api.dashboard.getBatteryStatus()
        ).done(function (summaryResponse, eventsResponse, batteryResponse) {
            const summary = summaryResponse[0];
            const recentEvents = eventsResponse[0];
            const batteryStatus = batteryResponse[0];

            if (!baliseManager.validators.isDashboardSummary(summary)
                || !baliseManager.validators.isEventList(recentEvents)
                || !baliseManager.validators.isBatteryList(batteryStatus)) {
                displayLoadingError({
                    responseJSON: {
                        detail: "La réponse du serveur est invalide."
                    }
                });
                return;
            }

            displaySummary(summary);
            displayRecentEvents(recentEvents);
            displayBatteryStatus(batteryStatus);
            $("[data-dashboard-status]").text("Données actualisées.");
        }).fail(function (xhr) {
            displayLoadingError(xhr);
            $("[data-recent-events], [data-battery-status]").empty().append(
                $("<p>", {
                    class: "dashboard-error",
                    text: "Les données ne peuvent pas être chargées."
                })
            );
        });
    }

    $(function () {
        loadDashboard();
    });
}(jQuery, window.baliseManager));
