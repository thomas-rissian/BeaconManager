(function (baliseManager) {
    "use strict";

    /**
     * These checks protect the rendering code from malformed API responses.
     * They do not replace backend validation.
     */
    function isDashboardSummary(value) {
        return isObject(value)
            && isNonNegativeInteger(value.totalBeacons)
            && isNonNegativeInteger(value.activeBeacons)
            && isNonNegativeInteger(value.inactiveBeacons)
            && isNonNegativeInteger(value.maintenanceBeacons)
            && isNonNegativeInteger(value.unreadEvents);
    }

    function isEventList(value) {
        return Array.isArray(value)
            && value.every(isDashboardEvent);
    }

    function isBatteryList(value) {
        return Array.isArray(value)
            && value.every(isBatteryStatus);
    }

    function isBeaconList(value) {
        return isObject(value)
            && Array.isArray(value.items)
            && isNonNegativeInteger(value.page)
            && isNonNegativeInteger(value.pageSize)
            && isNonNegativeInteger(value.totalCount)
            && isNonNegativeInteger(value.totalPages)
            && value.items.every(isBeacon);
    }

    function isBeaconResponse(value) {
        return isBeacon(value);
    }

    function isDashboardEvent(value) {
        return isObject(value)
            && typeof value.eventType === "string"
            && typeof value.message === "string"
            && typeof value.occurredAtUtc === "string"
            && typeof value.isRead === "boolean";
    }

    function isBatteryStatus(value) {
        return isObject(value)
            && typeof value.name === "string"
            && (value.batteryLevel === null || isNonNegativeInteger(value.batteryLevel))
            && typeof value.batteryStatus === "string";
    }

    function isBeacon(value) {
        return isObject(value)
            && isNonNegativeInteger(value.id)
            && typeof value.code === "string"
            && typeof value.name === "string"
            && typeof value.status === "string"
            && (value.batteryLevel === null || isNonNegativeInteger(value.batteryLevel));
    }

    function isObject(value) {
        return value !== null && typeof value === "object" && !Array.isArray(value);
    }

    function isNonNegativeInteger(value) {
        return Number.isInteger(value) && value >= 0;
    }

    baliseManager.validators = {
        isDashboardSummary: isDashboardSummary,
        isEventList: isEventList,
        isBatteryList: isBatteryList,
        isBeaconList: isBeaconList,
        isBeaconResponse: isBeaconResponse
    };
}(window.baliseManager = window.baliseManager || {}));
