function ToggleCollapsible(ID) {
    var collapsible = document.getElementById("dropdown" + ID);
    var image = document.getElementById("img" + ID);
    if (collapsible != null) {
        if (collapsible.style.maxHeight == "1000px") {
            collapsible.style.maxHeight = "0";
            if (image != null) {
                image.style.content = "url(\"resources/public/icons/arrows/downarrow1.svg\")";
            }
        }
        else {
            collapsible.style.maxHeight = "1000px";
            if (image != null) {
                image.style.content = "url(\"resources/public/icons/arrows/uparrow1.svg\")";
            }
        }
    }


}