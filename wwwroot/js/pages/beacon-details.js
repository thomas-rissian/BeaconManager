(function ($, baliseManager) {
    "use strict";

    let beaconId = null;

    function readBeaconId() {
        const queryParameters = new URLSearchParams(window.location.search);
        const requestedId = Number(queryParameters.get("id"));

        return Number.isInteger(requestedId) && requestedId > 0
            ? requestedId
            : null;
    }

    function displayBeacon(beacon) {
        $("[data-beacon-title]").text(beacon.name);
        $("[data-beacon-name]").text(beacon.name);
        $("[data-beacon-code]").text(beacon.code);
        $("[data-beacon-equipment]").text(beacon.equipment || "—");
        $("[data-beacon-site]").text(beacon.site || "—");
        $("[data-beacon-battery]").text(
            baliseManager.formatters.batteryLevel(beacon.batteryLevel));
        $("[data-beacon-last-communication]").text(
            beacon.lastCommunicationUtc
                ? baliseManager.formatters.date(beacon.lastCommunicationUtc)
                : "Jamais");
        $("[data-beacon-status-badge]").replaceWith(
            $("<span>", {
                class: "status-badge status-" + beacon.status.toLowerCase(),
                text: beacon.status,
                "data-beacon-status-badge": ""
            })
        );
    }

    function displayEvents(events) {
        const eventsContainer = $("[data-beacon-events]");
        eventsContainer.empty();

        if (events.length === 0) {
            eventsContainer.append($("<p>", {
                class: "muted-text",
                text: "Aucun événement pour cette balise."
            }));
            return;
        }

        $.each(events, function (eventIndex, event) {
            const eventItem = $("<article>", { class: "event-item" });
            const eventHeader = $("<div>", { class: "event-item-header" });

            eventHeader.append(
                $("<strong>").text(event.eventType),
                $("<time>", {
                    datetime: event.occurredAtUtc,
                    text: baliseManager.formatters.date(event.occurredAtUtc)
                })
            );
            eventItem.append(eventHeader, $("<p>").text(event.message));

            if (!event.isRead) {
                eventItem.addClass("is-unread");
            }

            eventsContainer.append(eventItem);
        });
    }

    function displayError(message) {
        $("[data-detail-status]")
            .removeClass("muted-text")
            .addClass("dashboard-error")
            .text(message);
        $("[data-beacon-details]").prop("hidden", true);
    }

    function loadBeaconDetails() {
        if (beaconId === null) {
            displayError("L’identifiant de la balise est invalide.");
            return;
        }

        $.when(
            baliseManager.api.beacons.getById(beaconId),
            baliseManager.api.beacons.getEvents(beaconId)
        ).done(function (beaconResponse, eventsResponse) {
            const beacon = beaconResponse[0];
            const events = eventsResponse[0];

            if (!baliseManager.validators.isBeaconResponse(beacon)
                || !baliseManager.validators.isEventList(events)) {
                displayError("La réponse du serveur est invalide.");
                return;
            }

            displayBeacon(beacon);
            displayEvents(events);
            $("[data-beacon-details]").prop("hidden", false);
            $("[data-detail-status]").text("Données actualisées.");
        }).fail(function (xhr) {
            displayError(baliseManager.getApiErrorMessage(xhr));
        });
    }

    function submitSimulation(event) {
        event.preventDefault();

        const batteryValue = $("[name='batteryLevel']").val();
        const eventData = {
            eventType: $("[name='eventType']").val(),
            message: $("[name='message']").val(),
            batteryLevel: batteryValue === "" ? null : Number(batteryValue)
        };
        const submitButton = $("[data-simulation-form] button[type='submit']");

        submitButton.prop("disabled", true);
        $("[data-simulation-status]").removeClass("dashboard-error")
            .text("Enregistrement...");

        baliseManager.api.beacons.simulateEvent(beaconId, eventData)
            .done(function () {
                $("[data-simulation-status]").text("Événement enregistré.");
                $("[name='message']").val("");
                loadBeaconDetails();
            })
            .fail(function (xhr) {
                $("[data-simulation-status]")
                    .addClass("dashboard-error")
                    .text(baliseManager.getApiErrorMessage(xhr));
            })
            .always(function () {
                submitButton.prop("disabled", false);
            });
    }

    $(function () {
        beaconId = readBeaconId();
        $("[data-simulation-form]").on("submit", submitSimulation);
        loadBeaconDetails();
    });
}(jQuery, window.baliseManager));
