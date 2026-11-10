(function ($) {
    "use strict";

    const baliseManager = window.baliseManager = window.baliseManager || {};

    function updateActiveNavigation() {
        const currentPath = window.location.pathname;
        const normalizedPath = currentPath === "/" ? "/" : currentPath.replace(/\/$/, "");

        $(".navigation-link").each(function () {
            const linkPath = new URL($(this).attr("href"), window.location.origin).pathname;
            const normalizedLinkPath = linkPath === "/" ? "/" : linkPath.replace(/\/$/, "");
            $(this).toggleClass("is-active", normalizedPath === normalizedLinkPath);
        });
    }

    function getApiErrorMessage(xhr) {
        if (xhr.responseJSON?.detail) {
            return xhr.responseJSON.detail;
        }

        if (xhr.responseJSON?.title) {
            return xhr.responseJSON.title;
        }

        return "Une erreur est survenue pendant la communication avec l'API.";
    }

    function setConnectionState(isConnected) {
        $("[data-connection-indicator]").toggleClass("is-offline", !isConnected);
        $("[data-connection-label]").text(
            isConnected
                ? "Notifications connectées"
                : "Notifications déconnectées");
    }

    baliseManager.getApiErrorMessage = getApiErrorMessage;
    baliseManager.setConnectionState = setConnectionState;

    $(function () {
        updateActiveNavigation();
        setConnectionState(false);
    });
}(jQuery));
