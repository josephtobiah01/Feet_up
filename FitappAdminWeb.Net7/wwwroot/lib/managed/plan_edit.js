(function () {
    function Day() {
        var self = this;
        self.StartDay = ko.observable();
        self.Id = ko.observable(0);
        self.Index = ko.observable(0);
        self.IsComplete = ko.observable(false);
        self.IsEditable = ko.observable(true);
    }

    function Week() {
        var self = this;
        self.Index = ko.observable(0);
        self.StartDate = ko.observable();
        self.EndDate = ko.observable();
        self.Id = ko.observable(0);
        self.IsEditable = ko.observable(true);
        self.DailyPlans = ko.observableArray([]);

        self.AddDay = function () {
            var day = new Day();
            day.Index(self.DailyPlans().length + 1);
            self.DailyPlans.push(day);
        }

        self.RemoveDay = function (day) {
            self.DailyPlans.remove(day);
        }
    }

    function ViewModel() {
        var self = this;
        self.EditMode = ko.observable();
        self.UserId = ko.observable(0);
        self.Id = ko.observable(0);
        self.Name = ko.observable("");
        self.StartDate = ko.observable("");
        self.IsCurrent = ko.observable(false);
        self.IsTemplate = ko.observable(false);
        self.WeeklyPlans = ko.observableArray([]);
        self.TemplateId = ko.observable(0);
        self.EndDate = ko.computed(() => {
            var startDate = self.StartDate();
            if (startDate != null && startDate != "") {
                var offset = 12 * 7;
                var date = new Date(startDate);
                date.setDate(date.getDate() + offset);
                return date;
            }
            return null;
        }, this);
        self.EnableSubmit = ko.observable(true);

        self.AddWeek = function () {
            var newWeek = new Week();
            newWeek.Index(self.WeeklyPlans().length + 1);
            self.WeeklyPlans.push(newWeek);
        }

        self.DisplayDate = function (date) {
            if (date != null && date != "") {
                //mm/dd/yyyy
                return  (date.getUTCMonth() + 1) + "/" + date.getUTCDate() + "/" + date.getUTCFullYear();
            }
        }

        self.RemoveWeek = function (week) {
            self.WeeklyPlans.remove(week);
        }

        self.InitializeViewModelData = function () {
            self.EditMode(document.getElementById("editmode").value);
            self.UserId(document.getElementById("Data_UserId").value);
            self.Name(document.getElementById("Data_Name").value);
            self.StartDate(document.getElementById("Data_StartDate").value);
            //self.EndDate(document.getElementById("Data_EndDate").value);
            self.IsCurrent(document.getElementById("Data_IsCurrent").checked);
            self.IsTemplate(document.getElementById("Data_IsTemplate").checked);
        }

        self.SetupValidation = function () {
            $("#progEditor").validate({
                errorClass: "is-invalid",
                validClass: "is-valid",
            });

            //add custom validator for jquery validator
            jQuery.validator.addMethod("check_ifmonday", function (value, element) {
                //console.log("check_ifmonday value:", value);
                //console.log("check_ifmonday element value", element.value);
                var date = new Date(value);
                if (date != null && date.getDay() != 1) {
                    return this.optional(element) || false;
                }
                return this.optional(element) || true;
            });

            $("#Data_StartDate").rules("add", {
                check_ifmonday: true,
                messages: {
                    check_ifmonday: "Please select a Monday"
                }
            });
        }

        self.ValidateSelf = function () {
            return $("#progEditor").valid();
        }

        self.SubmitModel = function () {
            var add_url = "/api/data/addplan";
            var edit_url = "/api/data/editplan";

            var url = self.EditMode() ? edit_url : add_url;

            if (self.ValidateSelf()) {
                self.EnableSubmit(false);
                var model = ko.toJS(self);
                console.log("====POSTING====");
                console.log(url);
                console.log(model);
                try {
                    fetch(url, {
                        method: "POST",
                        cache: "no-cache",
                        credentials: "include",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify(model)
                    })
                    .then((response) => response.json())
                    .then((data) => {
                        self.EnableSubmit(true);
                        var redirectUrl = "/training/index/" + self.UserId();
                        document.location.href = redirectUrl;
                    });
                }
                catch (e) {
                    console.log("Failed to add/edit plan");
                    self.EnableSubmit(true);
                }
            }
        }
    }

    var url_loadplanschedule = "/api/data/getplanschedulebyid";
    function loadPlanSchedule(model, planid) {
        var url = url_loadplanschedule + "?planid=" + planid;

        fetch(url)
            .then((response) => response.json())
            .then((data) => {
                console.log(data);
                var weekList = [];
                var weekIndex = 1;
                data.forEach((week) => {
                    var dayList = [];
                    var dayIndex = 1;
                    week.dailyPlans.forEach((day) => {
                        var currDay = new Day();
                        currDay.Id(day.id);
                        currDay.IsComplete(day.isComplete);
                        currDay.StartDay(getDateFromISOString(day.startDay));
                        currDay.IsEditable(!day.isComplete);
                        currDay.Index(dayIndex++);
                        dayList.push(currDay);
                    });

                    var currWeek = new Week();
                    currWeek.Id(week.id);
                    currWeek.StartDate(getDateFromISOString(week.startDate));
                    currWeek.EndDate(getDateFromISOString(week.endDate));
                    currWeek.DailyPlans(dayList);
                    currWeek.Index(weekIndex++);
                    weekList.push(currWeek);
                })
                model.WeeklyPlans(weekList);
                //console.log(ko.toJS(model));
            });
    }

    function getDateFromISOString(dateString) {
        return dateString.substring(0, 10);
    }

    var vm;
    function Initialize(options) {
        var user_id = options.UserId;
        var plan_id = options.PlanId;

        if (user_id != null && plan_id != null) {
            vm = new ViewModel();
            vm.Id(plan_id);
            vm.UserId(user_id);
            vm.InitializeViewModelData();
            loadPlanSchedule(vm, plan_id);
            vm.SetupValidation();
        }
        else {
            vm = new ViewModel();
        }

        //This is custom binding made to handle formatted dates using bootstrap-datepicker as input type "date" doesn't play well with custom formats.
        //Consider exporting this to other scripts with dates as this will be the de-facto binding for dates.
        ko.bindingHandlers.dpvalue = {
            init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                // This will be called when the binding is first applied to an element
                // Set up any initial state, event handlers, etc. here
                var jqelem = $(element);
                var value = valueAccessor();

                jqelem.datepicker({
                    format: "dd/mm/yyyy",
                    autoclose: true,
                    todayHighlight: true
                }).on("changeDate", function (e) {
                    value(e.date.toISOString());
                });
            },
            update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                // This will be called once when the binding is first applied to an element,
                // and again whenever any observables/computeds that are accessed change
                // Update the DOM element based on the supplied values here.
                console.log("UPDATE=============")

                var jq_elem = $(element);
                var parsedDate = jq_elem.datepicker("getUTCDate");
                console.log("dpvalue UPDATE:" + parsedDate.toISOString());

                valueAccessor(parsedDate.toISOString());
                console.log(ko.toJS(viewModel));
            }
        }

        //console.log(ko.toJS(vm));
        ko.applyBindings(vm, document.getElementById("progEditor"));
    }

    window["fitapp"] = {
        TrainingProgramEdit: { Initialize: Initialize }
    };
})();