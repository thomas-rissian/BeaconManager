(function ($, baliseManager) {
    "use strict";

    /**
     * The API client centralizes HTTP calls so page scripts only handle
     * presentation and user interactions.
     */
    function getDashboardSummary() {
        return $.ajax({
            url: "/api/dashboard/summary",
            method: "GET",
            dataType: "json"
        });
    }

    function getRecentEvents() {
        return $.ajax({
            url: "/api/dashboard/recent-events",
            method: "GET",
            dataType: "json"
        });
    }

    function getBatteryStatus() {
        return $.ajax({
            url: "/api/dashboard/battery-status",
            method: "GET",
            dataType: "json"
        });
    }

    function getBeacons(queryParameters) {
        return $.ajax({
            url: "/api/beacons",
            method: "GET",
            data: queryParameters,
            dataType: "json"
        });
    }

    function getBeaconById(beaconId) {
        return $.ajax({
            url: "/api/beacons/" + encodeURIComponent(beaconId),
            method: "GET",
            dataType: "json"
        });
    }

    function getBeaconEvents(beaconId) {
        return $.ajax({
            url: "/api/beacons/" + encodeURIComponent(beaconId) + "/events",
            method: "GET",
            dataType: "json"
        });
    }

    function simulateBeaconEvent(beaconId, eventData) {
        return $.ajax({
            url: "/api/beacons/" + encodeURIComponent(beaconId) + "/simulate-event",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(eventData),
            dataType: "json"
        });
    }

    function markEventAsRead(eventId) {
        return $.ajax({
            url: "/api/beacon-events/" + encodeURIComponent(eventId) + "/read",
            method: "PATCH"
        });
    }

    const api = baliseManager.api = baliseManager.api || {};
    const dashboardApi = api.dashboard = api.dashboard || {};
    const beaconsApi = api.beacons = api.beacons || {};

    dashboardApi.getSummary = getDashboardSummary;
    dashboardApi.getRecentEvents = getRecentEvents;
    dashboardApi.getBatteryStatus = getBatteryStatus;
    beaconsApi.getAll = getBeacons;
    beaconsApi.getById = getBeaconById;
    beaconsApi.getEvents = getBeaconEvents;
    beaconsApi.simulateEvent = simulateBeaconEvent;
    const eventsApi = api.events = api.events || {};

    eventsApi.getRecent = getRecentEvents;
    eventsApi.markAsRead = markEventAsRead;
}(jQuery, window.baliseManager));
