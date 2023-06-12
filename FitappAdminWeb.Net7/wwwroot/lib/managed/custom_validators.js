(function () {
    if (window.FitApp === undefined) {
        window.FitApp = {};
    }

    window.FitApp.AddCustomJQueryValidators = function () {
        //add custom validator for jquery validator
        jQuery.validator.addMethod("check_ifmonday", function (value, element) {
            var date = new Date(value);
            if (date != null && date.getDay() != 1) {
                return this.optional(element) || false;
            }
            return this.optional(element) || true;
        });
    };
})();

