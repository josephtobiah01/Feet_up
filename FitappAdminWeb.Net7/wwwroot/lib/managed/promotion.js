(function () {
    var field_imgurl = document.getElementById("UploadImageFile");
    var image_imgtag = document.getElementById("image_preview_img");
    var form = document.getElementById("promotion_form");
    var reader;
    var max_img_width = 343;
    var max_img_height = 140;

    document.addEventListener("DOMContentLoaded", function () {
        $(form).on("submit", function () {
            if ($(form).valid()) {
                if (image_imgtag.width != max_img_width ||
                    image_imgtag.height != max_img_height) {
                    return confirm("You are uploading an image that is not of the correct dimensions (343 x 140 px). Upon upload, the image will be resized to the correct dimensions which may cause distortion. Continue?");
                }
            }
        }).validate({
            errorClass: "is-invalid",
            validClass: "is-valid",
        });

        image_imgtag.onload = function () {
            var height = this.height;
            var width = this.width;
            console.log("Width: " + width + ", Height: " + height);
        };

        field_imgurl.onchange = (evt) => {
            if (FileReader !== undefined) {
                if (reader == null) {
                    reader = new FileReader();
                }
                reader.readAsDataURL(field_imgurl.files[0]);
                reader.onload = function (e) {
                    image_imgtag.src = e.target.result;
                };
            }
            else {
                console.log("[WARN] FileReader not available. Falling back to CreateObjectURL.");
                const [file] = field_imgurl.files;
                if (file) {
                    image_imgtag.src = URL.createObjectURL(file);
                }
            }
        };
    });
})();