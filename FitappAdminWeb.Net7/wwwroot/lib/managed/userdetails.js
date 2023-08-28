(function () {
    var url_genlink = "/api/data/getqtoollink?userid={uid}&url={url}";
    var url_passreset = "/api/data/getpasswordresetlink?userid=";

    var elem_userid = document.getElementById("uid");
    var elem_feduserid = document.getElementById("fuid");

    var elem_genlink = document.getElementById("generatelink");
    var elem_genlink_url = document.getElementById("genLinkModal_redirecturl");
    var elem_genlink_formdiv = document.getElementById("genLinkModalFormFields");

    var elem_genlink_submit_button = document.getElementById("genLink_submit");
    var elem_genlink_loader = document.getElementById("genlinkModal_loader");
    var elem_genlink_alertcontainer = document.getElementById("genlinkModal_linkdisplay");
    var elem_genlink_linktext = document.getElementById("linkdisplay");
    var elem_genlink_copylink_success = document.getElementById("copylink_success");
    var elem_genlink_copylink_button = document.getElementById("copylink_button");

    var elem_passreset_link = document.getElementById("genpassresetlink");
    var elem_passreset_linktext = document.getElementById("prm_linkdisplaytext");
    var elem_passreset_copylink_button = document.getElementById("prm_copylink_button");
    var elem_passreset_copylink_success = document.getElementById("prm_copylink_success");

    var elem_note_text = document.getElementById("AddNote_Note");
    var elem_note_count = document.getElementById("AddNote_Count");
    var elem_note_max_count = elem_note_text.getAttribute("maxlength");

    var c_dnone = "d-none";

    var ctrlKey = 17,
        cmdKey = 91,
        vKey = 86,
        bSpaceKey = 8,
        isCtrl = false;

    function initGenLinkModal() {
        InputNote();
        elem_genlink.addEventListener("click", function () {
            resetGenLinkModal();
        });

        elem_genlink_submit_button.addEventListener("click", function () {
            getQtoolLink();
        });

        elem_genlink_copylink_button.addEventListener("click", function () {
            var textToCopy = elem_genlink_linktext.innerText;

            navigator.clipboard.writeText(textToCopy);
            visibility(elem_genlink_copylink_success, true);
        });


    }

    function initPassResetModal() {
        elem_passreset_link.addEventListener("click", function () {
            elem_passreset_linktext.innerText = "Loading..";
            elem_passreset_copylink_button.disabled = true;
            getPasswordResetLink();
        });

        elem_passreset_copylink_button.addEventListener("click", function () {
            var textToCopy = elem_passreset_linktext.innerText;

            navigator.clipboard.writeText(textToCopy);
            visibility(elem_passreset_copylink_success, true);

            setTimeout(function () {
                visibility(elem_passreset_copylink_success, false);
            }, 3000);
        });  
    }

    function visibility(elem, isShown) {
        if (isShown) {
            elem.classList.remove(c_dnone);
        }
        else {
            elem.classList.add(c_dnone);
        }
    }

    function resetGenLinkModal() {
        visibility(elem_genlink_loader, false);
        visibility(elem_genlink_alertcontainer, false);
        visibility(elem_genlink_copylink_success, false);
        visibility(elem_genlink_formdiv, true);
        visibility(elem_genlink_copylink_button, false);
        visibility(elem_genlink_submit_button, true);

        elem_genlink_linktext.innerText = "";
        elem_genlink_url.value = "";
        elem_genlink_url.disabled = false;
        elem_genlink_submit_button.disabled = false;
    }

    function successGenLinkModal(linktext) {
        elem_genlink_alertcontainer.classList.remove("alert-danger");
        elem_genlink_alertcontainer.classList.add("alert-success");

        visibility(elem_genlink_loader, false);
        visibility(elem_genlink_alertcontainer, true);
        visibility(elem_genlink_copylink_success, false);
        visibility(elem_genlink_copylink_button, true);
        visibility(elem_genlink_submit_button, false);

        elem_genlink_linktext.innerText = linktext;
        elem_genlink_url.disabled = true;
    }

    function loadingGenLinkModal() {
        visibility(elem_genlink_loader, true);
        visibility(elem_genlink_alertcontainer, false);

        elem_genlink_url.disabled = true;
        elem_genlink_submit_button.disabled = true;
    }

    function errorGenLinkModal(errortext) {
        elem_genlink_alertcontainer.classList.remove("alert-success");
        elem_genlink_alertcontainer.classList.add("alert-danger");

        visibility(elem_genlink_loader, false);
        visibility(elem_genlink_alertcontainer, true);
        visibility(elem_genlink_copylink_success, false);
        visibility(elem_genlink_copylink_button, false);
        visibility(elem_genlink_submit_button, true);

        elem_genlink_submit_button.disabled = false;
        elem_genlink_url.disabled = false;
        
        elem_genlink_linktext.innerText = errortext;
    }

    function getQtoolLinkUrl(uid, url) {
        return url_genlink.replace("{uid}", uid).replace("{url}", encodeURI(url));
    }

    function getQtoolLink() {
        var uid = elem_userid.value;
        var url = elem_genlink_url.value;
        var apiurl = getQtoolLinkUrl(uid, url);
        var errorText = "Sorry, something went wrong. Please try again later.";

        try {
            loadingGenLinkModal();
            fetch(apiurl, {
                method: "GET",
                cache: "no-cache",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                }
            })
                .then((response) => {
                    if (response.ok !== true) {
                        return Promise.resolve("");
                    }
                    return response.text();
                })
                .then((data) => {
                    if (data == null || data == "") {
                        data = errorText;
                        errorGenLinkModal(data);
                    }
                    else {
                        successGenLinkModal(data);
                    }
                });
        }
        catch (ex) {
            console.log(ex);
            return errorGenLinkModal(errorText);
        }
    }

    function getPasswordResetLink() {
        var fuid = elem_feduserid.value;
        var url = url_passreset + fuid;
        var errorText = "Sorry, something went wrong. Please try again later.";

        try {
            fetch(url, {
                method: "GET",
                cache: "no-cache",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                }
            })
                .then((response) => {
                    if (response.ok !== true) {
                        return Promise.resolve("");
                    }
                    return response.text();
                })
                .then((data) => {
                    if (data == null || data == "") {
                        data = errorText;
                        elem_passreset_linktext.innerText = errorText;
                    }
                    else {
                        elem_passreset_copylink_button.disabled = false;
                        elem_passreset_linktext.innerText = data;
                    }                    
                });
        }
        catch (e) {
            console.log(ex);
        }
    }

    function InputNote() {
        elem_note_count.firstChild.textContent = elem_note_text.getAttribute("maxlength");

        "keydown,change,paste".split(",").forEach((eventName) => elem_note_text.addEventListener(eventName, async function (e) {

            elem_note_count.firstChild.textContent = elem_note_max_count - elem_note_text.value.length;

            if (eventName == "keydown" && e.keyCode == ctrlKey || e.keyCode == cmdKey)
                isCtrl = true;

            if (isCtrl && e.keyCode == vKey ||  e.keyCode == bSpaceKey) {
                var text = await navigator.clipboard.readText();
                console.log(text + " " + elem_note_text.value.length);
                elem_note_count.firstChild.textContent = elem_note_max_count - elem_note_text.value.length;
                isCtrl = false;
            }

            if (elem_note_text.value.length >= elem_note_max_count - 1) {
                elem_note_count.lastChild.textContent = "character left";
            } else {
                elem_note_count.lastChild.textContent = "characters left";
            }
        }));
    }
    document.addEventListener("DOMContentLoaded", function () {
        initGenLinkModal();
        initPassResetModal();
    });
})();