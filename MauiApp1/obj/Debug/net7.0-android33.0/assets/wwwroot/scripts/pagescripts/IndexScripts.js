
function renderJqueryComponentsinIndex() {
    $("#feedbutton").on("click", function (e) {
        e.preventDefault();
        $(this).css("font-weight", "700");
        $("#dashboardbutton").css("font-weight", "400");
        $("#biodatabutton").css("font-weight", "400");
        var cssObj = {};
        cssObj.left = $(this).position().left;
        cssObj.width = $(this).outerWidth();
        $("#underline").css(cssObj);
    });
    $("#feedbutton").eq(0).trigger("click");

    $("#dashboardbutton").on("click", function (e) {
        e.preventDefault();
        $("#feedbutton").css("font-weight", "400");
        $(this).css("font-weight", "700");
        $("#biodatabutton").css("font-weight", "400");
        var cssObj = {};
        cssObj.left = $(this).position().left;
        cssObj.width = $(this).outerWidth();
        $("#underline").css(cssObj);
    });
    $("#dashboardbutton").eq(0).trigger("click");

    $("#biodatabutton").on("click", function (e) {
        e.preventDefault();
        $("#feedbutton").css("font-weight", "400");
        $("#dashboardbutton").css("font-weight", "400");
        $(this).css("font-weight", "700");
        var cssObj = {};
        cssObj.left = $(this).position().left;
        cssObj.width = $(this).outerWidth();
        $("#underline").css(cssObj);
    });
    $("#biodatabutton").eq(0).trigger("click");

    $("#date").datepicker()

    $(document).ready(function () {
        SetUnderlineOnInitialize();
    });

    
    //document.getElementById("content-group").addEventListener('swiped-up', function (e) {
    //    var contentgroup = document.getElementById("content-group");
    //    //CollapseContentGroup(contentgroup);
    //    ExpandContentGroup(contentgroup);
    //});

    //document.getElementById("content-group").addEventListener('swiped-down', function (e) {
    //    var contentgroup = document.getElementById("content-group");
    //    //ExpandContentGroup(contentgroup);
    //    CollapseContentGroup(contentgroup);
    //});
    ////$("#content-group").on("swipe", function (event) {
        
    ////});
}

function setDefaultNutrientRadioButton() {
    $("#NutrientDefaultRadioButton").prop("checked", true);
}

function SetUnderlineOnInitialize() {
    $("#feedbutton").css("font-weight", "700");
    $("#dashboardbutton").css("font-weight", "400");
    $("#biodatabutton").css("font-weight", "400");
    var cssObj = {};
    cssObj.left = 0;
    cssObj.width = $("#feedbutton").outerWidth();
    $("#underline").css(cssObj);
}

function ScrollToNow() {
   document.getElementById("NowFeedList").scrollIntoView(true);
}

window.JavaScriptInteropDatepicker = function () {
    $("#datepickerInput").datepicker({
        dateFormat: "dd/mm/yy",
        constrainInput: true,
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            var myElement = document.getElementById('datepickerInput');
            myElement.value = selectedDate;
            var event = new Event('change');
            myElement.dispatchEvent(event);

            $(this).datepicker("hide");
        },
    });

}

window.blazorjs = {
    dragable: function () {
        dragElement(document.getElementById("bottomSheetDiv"));

        var sheet = document.getElementById("sheet");

        function dragElement(elmnt) {
            var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
            if (document.getElementById(elmnt.id + "header")) {
                /* if present, the header is where you move the DIV from:*/
                document.getElementById(elmnt.id + "header").onmousedown = dragMouseDown;
            } else {
                /* otherwise, move the DIV from anywhere inside the DIV:*/
                elmnt.onmousedown = dragMouseDown;
            }

            function dragMouseDown(e) {
                e = e || window.event;
                e.preventDefault();
                // get the mouse cursor position at startup:
                pos3 = e.clientX;
                pos4 = e.clientY;
                document.onmouseup = closeDragElement;
                // call a function whenever the cursor moves:
                document.onmousemove = elementDrag;
            }

            function elementDrag(e) {
                e = e || window.event;
                e.preventDefault();
                //console.log(sheet.style.height);
                // calculate the new cursor position:
                var sheetHeight = (window.innerHeight - e.clientY);
                var maxSheetDragHeight = (window.innerHeight / 100) * 86;

                if (sheetHeight <= 492) {
                    sheetHeight = 492;
                }

                if (sheetHeight >= maxSheetDragHeight) {
                    sheetHeight = maxSheetDragHeight;
                }

                sheet.style.height = sheetHeight + "px";               
                
            }

            function closeDragElement() {
                /* stop moving when mouse button is released:*/
                document.onmouseup = null;
                document.onmousemove = null;
            }
        }
    }
    
}

window.blazorjsFeedItemContentGroupDrag = {
    dragable: function () {
        dragElement(document.getElementById("resizeSheetDiv"));

        var sheet = document.getElementById("content-group");
        var minSheetHeight = (window.innerHeight - 260);

        function dragElement(elmnt) {
            var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
            if (document.getElementById(elmnt.id + "header")) {
                /* if present, the header is where you move the DIV from:*/
                document.getElementById(elmnt.id + "header").onmousedown = dragMouseDown;
            } else {
                /* otherwise, move the DIV from anywhere inside the DIV:*/
                elmnt.onmousedown = dragMouseDown;
            }

            function dragMouseDown(e) {
                e = e || window.event;
                e.preventDefault();
                _feedItemListDragging = true;
                _scrollcounterExpand = 0;
                _scrollcounterCollapse = 0;
                // get the mouse cursor position at startup:
                pos3 = e.clientX;
                pos4 = e.clientY;
                document.onmouseup = closeDragElement;
                // call a function whenever the cursor moves:
                document.onmousemove = elementDrag;
            }

            function elementDrag(e) {
                e = e || window.event;
                e.preventDefault();
                _feedItemListDragging = true;
                _scrollcounterExpand = 0;
                _scrollcounterCollapse = 0;
                //console.log(sheet.style.height);
                // calculate the new cursor position:
                var sheetHeight = (window.innerHeight - e.clientY);
                var maxSheetDragHeight = window.innerHeight - 16;

                if (sheetHeight <= minSheetHeight) {
                    sheetHeight = minSheetHeight;
                }

                if (sheetHeight >= maxSheetDragHeight) {
                    sheetHeight = maxSheetDragHeight;
                }

                sheet.style.height = sheetHeight + "px";

            }

            function closeDragElement() {
                /* stop moving when mouse button is released:*/
                document.onmouseup = null;
                document.onmousemove = null;
                _feedItemListDragging = false;
                _scrollcounterExpand = 0;
                _scrollcounterCollapse = 0;
            }
        }
    }

    }

window.GetBrowserDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

var _scrollcounterExpand = 0;
var _scrollcounterCollapse = 0;
var _feedItemListDragging = false;
var _feedItemListLastScroll = 0;

function CheckScroll() {   

    var feeditemlist = document.getElementById("feed-item-list-div");
    var contentgroup = document.getElementById("content-group");
    var maxScrollHeight = (feeditemlist.scrollHeight - feeditemlist.offsetHeight);

    if (feeditemlist.scrollTop <= _feedItemListLastScroll) {

        if (_feedItemListDragging == false) {
            CollapseContentGroup(contentgroup);
            //ExpandContentGroup(contentgroup);
            SetFeedItemListLastScroll();
        }


    }
    else if (feeditemlist.scrollTop >= _feedItemListLastScroll - 1) {

        if (_feedItemListDragging == false) {
            ExpandContentGroup(contentgroup);
            //CollapseContentGroup(contentgroup);           
            SetFeedItemListLastScroll();
        }
    }
    else {
        //Do nothing just scroll
    }

    
    //if (feeditemlist.scrollTop <= 1) {

    //    if (_feedItemListDragging == false) {
    //        ExpandContentGroup(contentgroup);   
    //    }


    //}
    //else if (feeditemlist.scrollTop >= maxScrollHeight-1) {
        
    //    if (_feedItemListDragging == false) {
    //        CollapseContentGroup(contentgroup); 
    //    }
    //}
    //else
    //{
    //    //Do nothing just scroll
    //}
}

function ExpandContentGroup(contentgroup) {
    _scrollcounterCollapse = 0;
    if (_scrollcounterExpand > 2) {
        //Expand
        contentgroup.style.height = (window.innerHeight - 8) + "px";
    }
    else {
        _scrollcounterExpand = _scrollcounterExpand + 1;
    }
}

function CollapseContentGroup(contentgroup) {
    _scrollcounterExpand = 0;
    if (_scrollcounterCollapse > 2) {
        //Collapse
        contentgroup.style.height = (window.innerHeight - 260) + "px";

        
    }
    else {
        _scrollcounterCollapse = _scrollcounterCollapse + 1;
    }
}


function ResetScrollCounter() {
    _scrollcounterExpand = 0;
    _scrollcounterCollapse = 0;
}

function SetFeedItemListLastScroll() {
    var feeditemlist = document.getElementById("feed-item-list-div");
    _feedItemListLastScroll = feeditemlist.scrollTop;
}