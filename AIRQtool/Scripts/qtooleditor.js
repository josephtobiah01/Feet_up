function Save() {
    var versionInput = document.getElementById("VersionComments");
    if (versionInput.value.trim() == "") {
        alert("Please enter Version Comments");
        return;
    }
    else {
        $("#myForm").append("<input type=hidden name=IsSaveWithPublish value=false></input>");
        $("#myForm").submit();
    }
}
function SaveWithPublish() {
    var versionInput = document.getElementById("VersionComments");
    if (versionInput.value.trim() == "") {
        alert("Please enter Version Comments");
        return;
    }
    else {
        var result = confirm("Are you sure you want to publish this form?");
        if (result) {
            $("#myForm").append("<input type=hidden name=IsSaveWithPublish value=true></input>");
            $("#myForm").submit();
        }
    }
}

document.onreadystatechange = function () {
    if (document.readyState === "complete") {

        init_ACEEditor();
        init_DataTables();
        if ($('#dtp_input2').length > 0)
            $('#dtp_input2').datetimepicker();
        if ($(".alert.show").length > 0) {
            $('.alert.show').each(function () {
                $(this).appendTo($("#layout-output"));
                $("#layout-output").attr("aria-hidden", false).addClass("show");
            });
            $("#layout-output button").bind("click", hideAlert);
            setTimeout(hideAlert, 5e3);
            function hideAlert() {
                $("#layout-output").attr("aria-hidden", true).removeClass("show");
            }
        }

    }
}
function init_ACEEditor() {

    if ($('#Contents').length > 0) {
        $(".container").addClass("editor")
        var editor = ace.edit("editor");
        editor.setTheme("ace/theme/idle_fingers");
        if ($('#Name').length > 0)
            editor.getSession().setMode("ace/mode/javascript");
        else
            editor.getSession().setMode("ace/mode/html");
        editor.setHighlightSelectedWord(true);
        editor.setShowFoldWidgets(true);
        //editor.setShowGutter(false);
        editor.setBehavioursEnabled(false);
        editor.setHighlightActiveLine(true);
        editor.setHighlightGutterLine(false);
        editor.setHighlightSelectedWord(true);

        var textarea = document.getElementById("Contents");

        editor.getSession().on('change', function () {
            textarea.textContent = editor.getSession().getValue();
        });

        textarea.textContent = editor.getSession().getValue();
    }
}

function init_DataTables() {
    if ("undefined" != typeof $.fn.DataTable) {
        var initTable = function () {
            $("#datatable-buttons").length && $("#datatable-buttons").DataTable({
                dom: "Bfrtip",
                buttons: [{ extend: "copy", className: "btn-sm" },
                { extend: "csv", className: "btn-sm" },
                { extend: "excel", className: "btn-sm" },
                { extend: "pdfHtml5", className: "btn-sm" },
                { extend: "print", className: "btn-sm" }
                ], responsive: !0
            })
        };
        TableManageButtons = function () {
            "use strict";
            return {
                init: function () {
                    initTable()
                }
            }
        }

        $("#datatable").DataTable({
            "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        });

    }
}