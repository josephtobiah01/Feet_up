window.addEventListener("DOMContentLoaded", (event) => {
    var copyweek_modal = document.getElementById("copyweek_modal");
    var copyweek_form = document.getElementById("copyweek_form");
    var copyweek_alert_container = document.getElementById("alert_container");

    var copyweek_field_weekid = document.getElementById("CopyWeekModel_WeekId");
    var copyweek_field_startdate = document.getElementById("CopyWeekModel_StartDay");
    var copyweek_button_submit = document.getElementById("copyweek_submit");

    var copyweek_submit_url = "/api/data/copyweek";

    //add modal event to handle assignment of week id
    copyweek_modal.addEventListener("show.bs.modal", function (event) {
        var button = event.relatedTarget;
        var weekid = button.getAttribute("data-bs-weekid");

        copyweek_alert_container.innerHTML = null;
        copyweek_field_startdate.value = null;
        copyweek_field_weekid.value = weekid;
    });

    //add jquery validation
    $(copyweek_form).validate({
        errorClass: "is-invalid",
        validClass: "is-valid",
    });
    window.FitApp.AddCustomJQueryValidators();
    $(copyweek_field_startdate).rules("add", {
        check_ifmonday: true,
        messages: {
            check_ifmonday: "Please select a Monday"
        }
    });

    copyweek_button_submit.addEventListener("click", function (event) {
        if ($(copyweek_form).valid()) {
            copyweek_button_submit.setAttribute("disabled", "true");
            var payload = { WeekId: copyweek_field_weekid.value, StartDay: copyweek_field_startdate.value };
            fetch(copyweek_submit_url, {
                method: "POST",
                cache: "no-cache",
                credentials: "include",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            })
                .then((response) => {
                    if (!response.ok) {
                        copyweek_button_submit.removeAttribute("disabled");
                        setCopyWeekModalAlert("danger", "Error in copying week. Response returned " + response.status);
                    }
                    else {
                        return response.json();
                    }
                })
                .then((data) => {
                    copyweek_button_submit.removeAttribute("disabled");
                    if (data.success) {
                        setCopyWeekModalAlert("success", "Successfully copied week. New Week ID: " + data.weekId);
                        document.location.reload();
                    }
                    else {
                        setCopyWeekModalAlert("danger", "Error in copying week. Error: " + data.errorMessage);
                    }
                }).catch((error) => {
                    copyweek_button_submit.removeAttribute("disabled");
                    setCopyWeekModalAlert("danger", "Error in copying week. Error: " + error);
                });
        }
    });

    function setCopyWeekModalAlert(level, message) {
        var alertclass = "alert-" + level;
        var alertHtml = "<div role='alert' class='alert " + alertclass + "'>" + message + "</div>";

        copyweek_alert_container.innerHTML = alertHtml;
    }
});