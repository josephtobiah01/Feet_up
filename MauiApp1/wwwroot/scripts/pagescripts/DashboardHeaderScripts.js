function renderJqueryComponentsinIndex() {

    $(document).ready(function () {
        SetUnderlineOnInitialize();
    });

}

function SetUnderlineOnInitialize() {
    $("#feedbutton").css("font-weight", "400");
    $("#dashboardbutton").css("font-weight", "700");
    $("#biodatabutton").css("font-weight", "400");
    var cssObj = {};
    cssObj.left = $("#dashboardbutton").position().left;
    cssObj.width = $("#dashboardbutton").outerWidth();
    $("#underline").css(cssObj);
}
