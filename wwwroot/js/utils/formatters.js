(function (baliseManager) {
    "use strict";

    function formatDate(dateValue) {
        const date = new Date(dateValue);

        if (Number.isNaN(date.getTime())) {
            return "Date inconnue";
        }

        return date.toLocaleString("fr-FR", {
            dateStyle: "short",
            timeStyle: "short"
        });
    }

    function formatBatteryLevel(batteryLevel) {
        return batteryLevel === null ? "Inconnue" : batteryLevel + "%";
    }

    function getBatteryStatusClass(batteryStatus) {
        const statusClasses = {
            Critical: "is-critical",
            Low: "is-low",
            Normal: "is-normal",
            Unknown: "is-unknown"
        };

        return statusClasses[batteryStatus] || statusClasses.Unknown;
    }

    baliseManager.formatters = {
        date: formatDate,
        batteryLevel: formatBatteryLevel,
        batteryStatusClass: getBatteryStatusClass
    };
}(window.baliseManager = window.baliseManager || {}));
