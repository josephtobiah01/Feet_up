
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

    var nowFeedList = document.getElementById("NowFeedList");
    if (nowFeedList) {
        nowFeedList.scrollIntoView(true);
    }
   //document.getElementById("NowFeedList").scrollIntoView(true);
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
 


                var sheetHeight = (window.innerHeight - e.clientY);
                var maxSheetDragHeight = window.innerHeight - 34;

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

var _feedItemListDragging = false;
var _feedItemListLastScroll = 0;

var _doScroll = true;


const updateDebounceValue = () => {

}
let throttlePause;
let debounceTimer;

const debounce = (callback, time) => {
    window.clearTimeout(debounceTimer);
    debounceTimer = window.setTimeout(callback, time);
};
//const throttle = (callback, time) => {
//    if (throttlePause) return;

//    throttlePause = true;
//    setTimeout(() => {
//        callback();
//        throttlePause = false;
//    }, time);
//};


let touchstartY = 0
let touchendY = 0


const isScrollDisabled = false;

function preventDefault(e) {
    e.preventDefault();
}

function panHandler(e) {
    const DOCUMENT_POSITION_CONTAINED_BY = 16;
    const problemDiv = document.getElementById('problem-div');

    if (problemDiv.compareDocumentPosition(e.target) & DOCUMENT_POSITION_CONTAINED_BY) {
        // Dragging within the scrollable problem div
        if (!isScrollDisabled) {
            problemDiv.addEventListener('touchmove', preventDefault);
            isScrollDisabled = true;
        }
    }

    /** Do other panning things **/

    if (e.isFinal) {
        const restoreCondition = true; // In case you want to restore state

        if (restoreCondition) {
            problemDiv.removeEventListener('touchmove', preventDefault);
            isScrollDisabled = false;
        }
    }
}





function setupDebounce() {




    var feeditemlist = document.getElementById("feed-item-list-div");
    var contentgroup = document.getElementById("content-group");



    var ts;
    feeditemlist.addEventListener('touchstart', function (e) {
        ts = e.touches[0].clientY;
    });

    feeditemlist.addEventListener('touchmove', function (e) {
        var te = e.changedTouches[0].clientY;
        if (ts > te)
        {
            contentgroup.style.transition = "all 0.35s ease-out";
            ExpandContentGroup(contentgroup);

        }
        else if (ts < te)
        {
            contentgroup.style.transition = "all 0.35s ease-in";
            CollapseContentGroup(contentgroup);
        }
    });

    feeditemlist.addEventListener('touchend', function (e) {
        var te = e.changedTouches[0].clientY;
        if (ts > te) {
            contentgroup.style.transition = "all 0.35s ease-out";
            ExpandContentGroup(contentgroup);

        }
        else if (ts < te) {
            contentgroup.style.transition = "all 0.35s ease-in";
            CollapseContentGroup(contentgroup);
        }
    });
    feeditemlist.addEventListener('touchmove', function (e) {
        var te = e.changedTouches[0].clientY;
        if (ts > te) {
            contentgroup.style.transition = "all 0.35s ease-out";
            ExpandContentGroup(contentgroup);

        }
        else if (ts < te) {
            contentgroup.style.transition = "all 0.35s ease-in";
            CollapseContentGroup(contentgroup);
        }
    });
    feeditemlist.addEventListener('touchcancel', function (e) {
        var te = e.changedTouches[0].clientY;
        if (ts > te) {
            contentgroup.style.transition = "all 0.35s ease-out";
            ExpandContentGroup(contentgroup);

        }
        else if (ts < te) {
            contentgroup.style.transition = "all 0.35s ease-in";
            CollapseContentGroup(contentgroup);
        }
    });
}

function ExpandContentGroup(contentgroup) {
    contentgroup.style.height = (window.innerHeight) - 34 + "px";
}

function CollapseContentGroup(contentgroup) {
        contentgroup.style.height = (window.innerHeight - 260) + "px";
}



function SetFeedItemListLastScroll() {
    var feeditemlist = document.getElementById("feed-item-list-div");
    _feedItemListLastScroll = feeditemlist.scrollTop;
}