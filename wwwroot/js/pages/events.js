(function ($, baliseManager) {
    "use strict";

    function createEventRow(event) {
        const row = $("<tr>", {
            class: event.isRead ? "" : "event-row-unread"
        });
        const beaconLink = $("<a>", {
            class: "table-link",
            href: "/pages/beacon-details.html?id=" + event.beaconId,
            text: "Balise #" + event.beaconId
        });
        const actionCell = $("<td>");

        if (event.isRead) {
            actionCell.append($("<span>", {
                class: "event-read-label",
                text: "Lu"
            }));
        } else {
            actionCell.append($("<button>", {
                class: "button button-secondary mark-read-button",
                type: "button",
                text: "Marquer comme lu"
            }).data("event-id", event.id));
        }

        row.append(
            $("<td>").append($("<strong>").text(event.eventType)),
            $("<td>").append(beaconLink),
            $("<td>", { text: event.message }),
            $("<td>", {
                text: baliseManager.formatters.date(event.occurredAtUtc)
            }),
            $("<td>").append(createReadBadge(event.isRead)),
            actionCell
        );

        return row;
    }

    function createReadBadge(isRead) {
        return $("<span>", {
            class: isRead
                ? "status-badge status-active"
                : "status-badge status-maintenance",
            text: isRead ? "Lu" : "Non lu"
        });
    }

    function displayEvents(events) {
        const rowsContainer = $("[data-event-rows]");
        rowsContainer.empty();

        if (events.length === 0) {
            rowsContainer.append(
                $("<tr>").append(
                    $("<td>", {
                        class: "empty-table-message",
                        colspan: 6,
                        text: "Aucun événement récent."
                    })
                )
            );
            return;
        }

        $.each(events, function (eventIndex, event) {
            rowsContainer.append(createEventRow(event));
        });
    }

    function loadEvents() {
        $("[data-events-status]").text("Chargement des événements...");
        $("[data-refresh-events]").prop("disabled", true);

        baliseManager.api.events.getRecent()
            .done(function (events) {
                if (!baliseManager.validators.isEventList(events)) {
                    displayError("La réponse du serveur est invalide.");
                    return;
                }

                displayEvents(events);
                $("[data-events-status]").text(
                    events.length + " événement(s) récent(s).");
            })
            .fail(function (xhr) {
                displayError(baliseManager.getApiErrorMessage(xhr));
            })
            .always(function () {
                $("[data-refresh-events]").prop("disabled", false);
            });
    }

    function markEventAsRead(eventId, button) {
        button.prop("disabled", true);

        baliseManager.api.events.markAsRead(eventId)
            .done(function () {
                loadEvents();
            })
            .fail(function (xhr) {
                $("[data-events-status]")
                    .addClass("dashboard-error")
                    .text(baliseManager.getApiErrorMessage(xhr));
                button.prop("disabled", false);
            });
    }

    function displayError(message) {
        $("[data-event-rows]").empty().append(
            $("<tr>").append(
                $("<td>", {
                    class: "empty-table-message dashboard-error",
                    colspan: 6,
                    text: message
                })
            )
        );
        $("[data-events-status]").text("Le chargement a échoué.");
    }

    $(function () {
        $("[data-refresh-events]").on("click", loadEvents);
        $("[data-event-rows]").on("click", ".mark-read-button", function () {
            markEventAsRead($(this).data("event-id"), $(this));
        });
        loadEvents();
    });
}(jQuery, window.baliseManager));
