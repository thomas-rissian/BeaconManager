(function ($, baliseManager) {
    "use strict";

    const pageSize = 10;
    let currentPage = 1;
    let currentFilters = {};

    function createBeaconRow(beacon) {
        const row = $("<tr>");
        const detailsLink = $("<a>", {
            class: "table-link",
            href: "/pages/beacon-details.html?id=" + beacon.id,
            text: "Détails"
        });

        row.append(
            $("<td>", { text: beacon.code }),
            $("<td>", { text: beacon.name }),
            $("<td>", { text: beacon.equipment || "—" }),
            $("<td>", { text: beacon.site || "—" }),
            $("<td>").append(createStatusBadge(beacon.status)),
            $("<td>", {
                text: baliseManager.formatters.batteryLevel(beacon.batteryLevel)
            }),
            $("<td>", {
                text: beacon.lastCommunicationUtc
                    ? baliseManager.formatters.date(beacon.lastCommunicationUtc)
                    : "Jamais"
            }),
            $("<td>").append(detailsLink)
        );

        return row;
    }

    function createStatusBadge(status) {
        const statusClass = status.toLowerCase();
        return $("<span>", {
            class: "status-badge status-" + statusClass,
            text: status
        });
    }

    function displayBeacons(beaconPage) {
        const rowsContainer = $("[data-beacon-rows]");
        rowsContainer.empty();

        if (beaconPage.items.length === 0) {
            rowsContainer.append(
                $("<tr>").append(
                    $("<td>", {
                        class: "empty-table-message",
                        colspan: 8,
                        text: "Aucune balise ne correspond aux critères."
                    })
                )
            );
            return;
        }

        $.each(beaconPage.items, function (beaconIndex, beacon) {
            rowsContainer.append(createBeaconRow(beacon));
        });
    }

    function displayPagination(beaconPage) {
        const paginationContainer = $("[data-beacon-pagination]");
        paginationContainer.empty();

        if (beaconPage.totalPages <= 1) {
            return;
        }

        const previousButton = createPageButton(
            "Précédente",
            beaconPage.page - 1,
            beaconPage.page === 1);
        const nextButton = createPageButton(
            "Suivante",
            beaconPage.page + 1,
            beaconPage.page === beaconPage.totalPages);

        paginationContainer.append(previousButton);
        paginationContainer.append(
            $("<span>", {
                class: "pagination-label",
                text: "Page " + beaconPage.page + " sur " + beaconPage.totalPages
            })
        );
        paginationContainer.append(nextButton);
    }

    function createPageButton(label, page, isDisabled) {
        return $("<button>", {
            class: "button button-secondary pagination-button",
            type: "button",
            text: label,
            disabled: isDisabled
        }).data("page", page);
    }

    function readFilters() {
        return {
            search: $("[name='search']").val(),
            status: $("[name='status']").val(),
            site: $("[name='site']").val()
        };
    }

    function loadBeacons() {
        const requestParameters = $.extend({}, currentFilters, {
            page: currentPage,
            pageSize: pageSize
        });

        $("[data-beacon-status]").text("Chargement des balises...");

        baliseManager.api.beacons.getAll(requestParameters)
            .done(function (beaconPage) {
                if (!baliseManager.validators.isBeaconList(beaconPage)) {
                    displayError("La réponse du serveur est invalide.");
                    return;
                }

                displayBeacons(beaconPage);
                displayPagination(beaconPage);
                $("[data-beacon-status]").text(
                    beaconPage.totalCount + " balise(s) trouvée(s).");
            })
            .fail(function (xhr) {
                displayError(baliseManager.getApiErrorMessage(xhr));
            });
    }

    function displayError(message) {
        $("[data-beacon-rows]").empty().append(
            $("<tr>").append(
                $("<td>", {
                    class: "empty-table-message dashboard-error",
                    colspan: 8,
                    text: message
                })
            )
        );
        $("[data-beacon-status]").text("Le chargement a échoué.");
        $("[data-beacon-pagination]").empty();
    }

    $(function () {
        $("[data-beacon-filter-form]").on("submit", function (event) {
            event.preventDefault();
            currentFilters = readFilters();
            currentPage = 1;
            loadBeacons();
        });

        $("[data-beacon-pagination]").on("click", ".pagination-button", function () {
            currentPage = $(this).data("page");
            loadBeacons();
        });

        loadBeacons();
    });
}(jQuery, window.baliseManager));
