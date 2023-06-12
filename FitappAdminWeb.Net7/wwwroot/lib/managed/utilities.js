(function () {
    if (window.FitApp === undefined) {
        window.FitApp = {};
    }

    window.FitApp.showToast = function (header, message, status) {
        var toastStatus = TOAST_STATUS.SUCCESS; //SUCCESS, DANGER, WARNING, INFO
        if (status.toLowerCase() === "success") {
            toastStatus = TOAST_STATUS.SUCCESS;
        }
        if (status.toLowerCase() === "danger") {
            toastStatus = TOAST_STATUS.DANGER;
        }
        if (status.toLowerCase() === "warning") {
            toastStatus = TOAST_STATUS.WARNING;
        }
        if (status.toLowerCase() === "info") {
            toastStatus = TOAST_STATUS.INFO;
        }

        var configs = {
            title: header,
            message: message,
            status: toastStatus,
            timeout: 5000
        };
        Toast.setPlacement(TOAST_PLACEMENT.BOTTOM_RIGHT);
        Toast.create(configs);
    }
})();