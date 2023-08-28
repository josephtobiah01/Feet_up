(function () {
    var namespace = "chat";
    var form_sendmessage = document.getElementById("form_sendmessage");
    var field_chathistory = document.getElementById("messagelist");
    var field_chatimage = document.getElementById("messageimage");
    var field_image = document.getElementById("formfile");
    var filereader = new FileReader();

    function ViewModel() {
        var self = this;
        self.TextMode = ko.observable(true);
        self.ImageBase64 = ko.observable();
        self.ImageContentType = ko.observable();
        self.Message = ko.observable();

        self.IsValid = ko.computed(function () {
            return this.ImageBase64() != null || this.Message() != null;
        }, self);

        self.SwitchMode = function () {
            self.ImageBase64(null);
            self.ImageContentType(null);
            self.Message(null);
            field_image.value = null;
            self.TextMode(!self.TextMode());
        }

        self.Image_OnChange = function (data, evt) {
            var elem = evt.target;

            if (filereader == null) {
                filereader = new FileReader();
            }

            var file = elem.files[0];
            self.ImageContentType(file.type);
            console.log(self.ImageContentType());

            filereader.readAsDataURL(file);
            filereader.onload = function (e) {
                self.ImageBase64(e.target.result);
            };
        }
    }

    function _init() {
        var vm = new ViewModel();
        window.addEventListener("load", function () {
            field_chathistory.scrollTop = field_chathistory.scrollHeight
        });
        ko.applyBindings(vm, form_sendmessage);
    }
    
    document.addEventListener("DOMContentLoaded", _init);
})();