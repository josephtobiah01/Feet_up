(function () {
    document.addEventListener("DOMContentLoaded", function (e) {
        var form_suppref = document.getElementById("suppref_editor");

        var ViewModel = function () {
            var self = this;
            var field_unitmetric = document.getElementById("Data_UnitMetric_Id");

            self.UnitMetricId = ko.observable(field_unitmetric.value);
            self.NewMetricVisible = ko.computed(function () {
                return this.UnitMetricId() === '0';
            }, self);
        }

        var vm = new ViewModel();
        window["debugvm"] = vm;
        ko.applyBindings(vm, form_suppref);

        $(form_suppref).validate({
            errorClass: "is-invalid",
            validClass: "is-valid",
        });
    });
})();