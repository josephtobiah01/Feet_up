(function () {
    function Day() {
        var self = this;
        self.Id = ko.observable();
        self.DayOfWeek = ko.observable();
        self.DayName = ko.computed(function () {
            if (this.DayOfWeek() === 0) {
                return "Monday";
            }
            if (this.DayOfWeek() === 1) {
                return "Tuesday";
            }
            if (this.DayOfWeek() === 2) {
                return "Wednesday";
            }
            if (this.DayOfWeek() === 3) {
                return "Thursday";
            }
            if (this.DayOfWeek() === 4) {
                return "Friday";
            }
            if (this.DayOfWeek() === 5) {
                return "Saturday";
            }
            if (this.DayOfWeek() === 6) {
                return "Sunday";
            }
        }, self);
        
        self.Supplements = ko.observableArray([]);

        self.AddSupplement = function () {
            var newSupp = new Supplement();
            self.Supplements.push(newSupp);
        }

        self.DeleteSupplement = function (supp) {
            if (confirm("Are you sure you want to remove this supplement?")) {
                self.Supplements.remove(supp);
            }
        }
    }

    function Supplement() {
        var self = this;
        self.Id = ko.observable();
        self.Doses = ko.observableArray([]);
        self.FkSupplementReference = ko.observable();

        self.IsFreeEntry = ko.computed(function () {
            return this.FkSupplementReference() == 0;
        }, self);
        self.FreeEntryName = ko.observable();
        self.FkFreeEntryUnitMetric_string = ko.observable();
        self.FkFreeEntryUnitMetric = ko.computed(function () {
            if (this.FkFreeEntryUnitMetric_string() == null ||
                this.FkFreeEntryUnitMetric_string() == "") {
                return null;
            }
            return parseInt(this.FkFreeEntryUnitMetric_string());
        }, self);
        self.Remark = ko.observable();

        self.AddDose = function () {
            var newDose = new Dose();
            self.Doses.push(newDose);
        }

        self.DeleteDose = function (dose) {
            if (confirm("Are you sure you want to remove this dose?")) {
                self.Doses.remove(dose);
            }
        }
    }

    function Dose() {
        var self = this;
        self.Id = ko.observable();
        self.UnitCount = ko.observable();
        self.ScheduledTime = ko.observable();
        self.Remark = ko.observable();
        self.DoseWarningLimit = ko.observable();
        self.DoseHardCeilingLimit = ko.observable();
    }

    function CopyModal() {
        var self = this;
        
    }

    function ViewModel() {
        var self = this;
        self.Id = ko.observable();
        self.Days = ko.observableArray([]); //7 entries will be pre-added here per day
        self.EditMode = ko.observable(false);
        self.IsActive = ko.observable(false);
        self.IsTemplate = ko.observable(false);
        self.ForceScheduleSync = ko.observable(false);
        self.Remark = ko.observable();
        self.UserId = ko.observable();
        self.TemplateId = ko.observable();
        self.CopyMode = ko.observable(false);

        //copy modal
        self.Copy_Source = ko.observable();
        self.Copy_Dest_0 = ko.observable(false);
        self.Copy_Dest_1 = ko.observable(false);
        self.Copy_Dest_2 = ko.observable(false);
        self.Copy_Dest_3 = ko.observable(false);
        self.Copy_Dest_4 = ko.observable(false);
        self.Copy_Dest_5 = ko.observable(false);
        self.Copy_Dest_6 = ko.observable(false);

        self.CopyDaySubmit_Click = function () {
            var srcIndex = self.Copy_Source();
            if (self.Copy_Dest_0() == true) {
                CopyDay(srcIndex, 0);
            }
            if (self.Copy_Dest_1() == true) {
                CopyDay(srcIndex, 1);
            }
            if (self.Copy_Dest_2() == true) {
                CopyDay(srcIndex, 2);
            }
            if (self.Copy_Dest_3() == true) {
                CopyDay(srcIndex, 3);
            }
            if (self.Copy_Dest_4() == true) {
                CopyDay(srcIndex, 4);
            }
            if (self.Copy_Dest_5() == true) {
                CopyDay(srcIndex, 5);
            }
            if (self.Copy_Dest_6() == true) {
                CopyDay(srcIndex, 6);
            }

            self.Copy_Source(null);
            self.Copy_Dest_0(false);
            self.Copy_Dest_1(false);
            self.Copy_Dest_2(false);
            self.Copy_Dest_3(false);
            self.Copy_Dest_4(false);
            self.Copy_Dest_5(false);
            self.Copy_Dest_6(false);

            DismissCopyModal();
        }

        function DismissCopyModal() {
            var modal = document.getElementById("copyday_modal");
            var modalInstance = bootstrap.Modal.getInstance(modal);
            modalInstance.hide();
        }

        function CopyDay(srcIndex, destIndex) {
            if (srcIndex == destIndex) {
                //skip copy if same day
                return;
            }

            var dayToCopy = ko.toJS(self.Days()[srcIndex]);

            var newDay = new Day();
            newDay.DayOfWeek(dayToCopy.DayOfWeek);

            newDay.DayOfWeek(dayToCopy.DayOfWeek);
            dayToCopy.Supplements.forEach((supp) => {
                var newSupp = new Supplement();
                newSupp.FkSupplementReference(supp.FkSupplementReference);
                if (supp.IsFreeEntry) {
                    newSupp.FkFreeEntryUnitMetric_string(supp.FkFreeEntryUnitMetric);
                }

                supp.Doses.forEach((dose) => {
                    var newDose = new Dose();
                    newDose.UnitCount(dose.UnitCount);
                    newDose.DoseWarningLimit(dose.DoseWarningLimit);
                    newDose.DoseHardCeilingLimit(dose.DoseHardCeilingLimit);
                    newDose.ScheduledTime(truncateTime(dose.ScheduledTime));
                    newSupp.Doses.push(newDose);
                })
                newDay.Supplements.push(newSupp);
            })

            var dayToEdit = self.Days()[destIndex];
            dayToEdit.Supplements(newDay.Supplements());

            console.log(ko.toJS(self.Days));
        }

        for (var i = 0; i < 7; i++) {
            var newDay = new Day();
            newDay.DayOfWeek(i);
            self.Days.push(newDay);
        }

        self.loadDataFromFields = function () {
            self.Remark(document.getElementById("Data_Remark").value);
            self.IsActive(document.getElementById("Data_IsActive").checked);
            self.IsTemplate(document.getElementById("Data_IsTemplate").checked);
        }

        self.ValidateSelf = function () {
            return true;
        }

        self.SubmitModel = function () {
            var add_url = "/api/data/addsupplementplan";
            var edit_url = "/api/data/editsupplementplan";

            var url = (self.Id() != null && self.Id() > 0) ? edit_url : add_url;

            if (self.ValidateSelf()) {
                var model = ko.toJS(self);
                console.log("===POSTING===");
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
                        .then((response) => {
                            if (response.ok == false) {
                                throw new Error("Save Failed");
                            }

                            return response.json();
                        })
                        .then((data) => {
                            console.log(data);

                            var redirectUrl = "";
                            redirectUrl = "/Supplement/SupplementsWeekly/" + self.UserId();
                            document.location.href = redirectUrl;
                        })
                }
                catch (e) {
                    console.log(e);
                }
            }
        }
    }

    function getTimeFromISOString(dateString) {
        return dateString.substring(11, dateString.length - 3);
    }

    function truncateTime(timeString) {
        return timeString.substring(0, 5);
    }

    var url_planschedule = "/api/data/getsupplementplanschedulebyplanid";
    function loadSupplementSchedule(model, id) {
        var updateViewModelWithData = (data) => {
            console.log(data);
            data.forEach((day) => {
                var modelDay;

                //get relevant day item from model
                for (var i = 0; i < model.Days().length; i++) {
                    var currDay = model.Days()[i];
                    if (currDay.DayOfWeek() == day.dayOfWeek) {
                        modelDay = currDay;
                    }
                }

                day.supplements.forEach((supp) => {
                    var modelSupp = new Supplement();

                    modelSupp.FkSupplementReference(supp.fkSupplementReference);
                    if (modelSupp.IsFreeEntry()) {
                        modelSupp.FkFreeEntryUnitMetric_string(supp.fkFreeEntryUnitMetric);
                    }

                    supp.doses.forEach((dose) => {
                        var newDose = new Dose();
                        newDose.UnitCount(dose.unitCount);
                        newDose.DoseWarningLimit(dose.doseWarningLimit);
                        newDose.DoseHardCeilingLimit(dose.doseHardCeilingLimit);
                        newDose.ScheduledTime(truncateTime(dose.scheduledTime));
                        //newDose.ScheduledTime_TimeSpan(getTimeFromISOString(dose.scheduledTime));

                        modelSupp.Doses.push(newDose);
                    })

                    modelDay.Supplements.push(modelSupp);
                })
            })
        };

        var url = url_planschedule + "?planId=" + id;
        fetch(url)
            .then((response) => response.json())
            .then(updateViewModelWithData)
            .catch((e) => {
                console.log(e);
            });
    }

    var vm;
    function Initialize(options) {
        var user_id = options.UserId;
        var weeklyplan_id = options.WeeklyPlanId;
        var copymode = options.CopyMode;

        if (user_id != null && weeklyplan_id != null) {
            vm = new ViewModel();
            //load supplement plans here
            vm.Id(weeklyplan_id);
            vm.loadDataFromFields();
            loadSupplementSchedule(vm, weeklyplan_id);
            vm.EditMode(true);

            if (copymode) {
                vm.Id(0);
            }
        }
        else {
            vm = new ViewModel();
        }

        vm.UserId(user_id);
        vm.CopyMode(copymode);
        ko.applyBindings(vm, document.getElementById("suppsched_editor"));
    }

    window["fitapp"] = {
        SupplementWeeklyEdit: { Initialize: Initialize }
    };
})();