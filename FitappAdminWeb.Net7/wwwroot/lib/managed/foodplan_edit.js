(function () {
    var mealtype_lookup = [];

    function Meal() {
        var self = this;
        self.Id = ko.observable(0);
        self.MealTypeId = ko.observable();
        self.HasTarget = ko.observable();
        self.Date = ko.observable();
        self.MealTypes = [];

        //Do not allow meal deletions when either of these flags are true
        //Editing targets should be fine, probably (for confirmation)
        self.IsComplete = ko.observable(false);
        self.IsOngoing = ko.observable(false);
        
        self.ScheduledTime_TimeSpan = ko.observable();

        self.MealCalorieTarget = ko.observable();
        self.MealCalorieMin = ko.observable();
        self.MealCalorieMax = ko.observable();
        self.ProteinGramsTarget = ko.observable();
        self.CrabsGramsTarget = ko.observable();
        self.SugarGramsTarget = ko.observable();
        self.FatGramsTarget = ko.observable();
        self.UnsaturatedFatGramsTarget = ko.observable();
        self.SaturatedFatGramsTarget = ko.observable();
        self.FiberGramsTarget = ko.observable();
        self.AlcoholGramsTarget = ko.observable();

        self.MealTypeId_OnChange = function (data, event) {
            var currValue = event.target.value;
            console.log("DETECTED MEAL TYPE ID: " + currValue);

            var defaultTime;
            data.MealTypes.forEach((mt) => {
                if (mt.id == currValue) {
                    console.log("DEFAULT TIME: " + mt.defaulttime);
                    defaultTime = truncateTime(mt.defaulttime);
                }
            });
            data.ScheduledTime_TimeSpan(defaultTime);
        }
    }

    function ViewModel() {
        var self = this;
        self.Id = ko.observable();
        self.FkUserId = ko.observable();
        self.Meals = ko.observableArray([]);
        self.CopyMode = ko.observable(false);
        self.Date = ko.observable();
        self.DaysToExtrapolate = ko.observable();
        self.IsSubmitEnabled = ko.observable(true);

        self.MealTypes = [];

        //computed information
        self.DayCalorieTarget = ko.computed(function () {
            return mealPropertySummate("MealCalorieTarget", this.Meals);
        }, self);
        self.DayCalorieTargetMin = ko.computed(function () {
            return mealPropertySummate("MealCalorieMin", this.Meals);
        }, self);
        self.DayCalorieTargetMax = ko.computed(function () {
            return mealPropertySummate("MealCalorieMax", this.Meals);
        }, self);
        self.DayCalorieDisplay = ko.computed(function () {
            var text = "{target} ({min} - {max})";
            text = text.replace("{target}", this.DayCalorieTarget());
            text = text.replace("{min}", this.DayCalorieTargetMin());
            text = text.replace("{max}", this.DayCalorieTargetMax());
            return text;
        }, self);
        self.ProteinGramsTarget = ko.computed(function () {
            return mealPropertySummate("ProteinGramsTarget", this.Meals);
        }, self);
        self.CrabsGramsTarget = ko.computed(function () {
            return mealPropertySummate("CrabsGramsTarget", this.Meals);
        }, self);
        self.SugarGramsTarget = ko.computed(function () {
            return mealPropertySummate("SugarGramsTarget", this.Meals);
        }, self);
        self.FatGramsTarget = ko.computed(function () {
            return mealPropertySummate("FatGramsTarget", this.Meals);
        }, self);
        self.SaturatedFatGramsTarget = ko.computed(function () {
            return mealPropertySummate("SaturatedFatGramsTarget", this.Meals);
        }, self);
        self.UnsaturatedFatGramsTarget = ko.computed(function () {
            return mealPropertySummate("UnsaturatedFatGramsTarget", this.Meals);
        }, self);
        self.FiberGramsTarget = ko.computed(function () {
            return mealPropertySummate("FiberGramsTarget", this.Meals);
        }, self);
        self.AlcoholGramsTarget = ko.computed(function () {
            return mealPropertySummate("AlcoholGramsTarget", this.Meals);
        }, self);

        self.AddMeal = function () {
            var meal = new Meal();
            meal.MealTypes = self.MealTypes;
            meal.Date(self.Date());

            var defaultTime;
            self.MealTypes.forEach((mt) => {
                if (mt.id == 1) {
                    defaultTime = truncateTime(mt.defaulttime);
                }
            });
            meal.ScheduledTime_TimeSpan(defaultTime);

            self.Meals.push(meal);
            scrollAndHighlightLastMealItem();
            addMealButtonEnableDisable();
        }

        self.RemoveMeal = function (meal) {
            self.Meals.remove(meal);
            addMealButtonEnableDisable();
        }

        self.CloneMeal = function (meal) {
            var newMeal = new Meal();
            newMeal.MealTypeId(meal.MealTypeId());
            newMeal.HasTarget(meal.HasTarget());
            newMeal.MealCalorieTarget(meal.MealCalorieTarget());
            newMeal.MealCalorieMin(meal.MealCalorieMin());
            newMeal.MealCalorieMax(meal.MealCalorieMax());
            newMeal.ProteinGramsTarget(meal.ProteinGramsTarget());
            newMeal.CrabsGramsTarget(meal.CrabsGramsTarget());
            newMeal.SugarGramsTarget(meal.SugarGramsTarget());
            newMeal.FatGramsTarget(meal.FatGramsTarget());
            newMeal.SaturatedFatGramsTarget(meal.SaturatedFatGramsTarget());
            newMeal.UnsaturatedFatGramsTarget(meal.UnsaturatedFatGramsTarget());
            newMeal.FiberGramsTarget(meal.FiberGramsTarget());
            newMeal.AlcoholGramsTarget(meal.AlcoholGramsTarget());
            newMeal.MealTypes = meal.MealTypes;
            self.Meals.push(newMeal);
            scrollAndHighlightLastMealItem();
            addMealButtonEnableDisable();
        }

        self.SubmitModel = function () {
            var add_url = "/api/data/adddailyfoodplan";
            var edit_url = "/api/data/editdailyfoodplan";

            var url = self.Id() == "0" ? add_url : edit_url;

            var model = ko.toJS(self);
            console.log(model);

            try {
                self.IsSubmitEnabled(false);
                fetch(url, {
                    method: "POST",
                    cache: "no-cache",
                    credentials: "include",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(model)
                }).then((response) => {
                    if (!response.ok) {
                        throw new Error("Submit Failed. Returned invalid response");
                    }
                    return response.json();
                }).then((data) => {
                    console.log(data);
                    self.IsSubmitEnabled(true);

                    var redirectUrl = "/nutrition/dailyplan?userid={id}&start={date}";
                    redirectUrl = redirectUrl.replace("{id}", self.FkUserId());
                    redirectUrl = redirectUrl.replace("{date}", self.Date());
                    //console.log(redirectUrl);
                    document.location.href = redirectUrl;
                });
            }
            catch (ex) {
                self.IsSubmitEnabled(true);
                throw ex;
            }
        }

        self.EnableDisableAddMeal = function () {
            addMealButtonEnableDisable();
        }

        function mealPropertySummate(mealProp, mealList) {
            var total = 0;

            mealList().forEach((meal) => {
                var property = meal[mealProp]();
                if (property != null && property != "" && !isNaN(property)) {
                    total += parseFloat(property);
                }
            });

            return total;
        }

        function scrollAndHighlightLastMealItem() {
            var mealCount = self.Meals().length;
            var targetElem = document.querySelector("#meal_" + mealCount);

            //scroll to elem, then flash it
            targetElem.scrollIntoView();

            $(targetElem).delay(500).fadeOut().fadeIn().fadeOut().fadeIn();
        }

        function addMealButtonEnableDisable() {
            var mealCount = self.Meals().length;

            $("#addmeal").prop('disabled', true);
            if (mealCount < 6) {
                $("#addmeal").prop('disabled', false);
            }
        }
    }

    function truncateTime(timeString) {
        if (timeString == null) {
            return null;
        }
        return timeString.substring(0, 5);
    }

    function loadMealsInDay(dayId, vm) {
        var url = "/api/data/getmealsinday?dayId=" + dayId;

        try {
            fetch(url)
                .then((response) => {
                    if (!response.ok) {
                        throw new Error("Cannot get meals in day " + dayId);
                    }
                    return response.json();
                })
                .then((data) => {
                    console.log(data);
                    data.forEach((meal) => {
                        var newMeal = new Meal();
                        if (vm.CopyMode()) {
                            meal.Id(0); //remove id for adding
                        }
                        newMeal.Date(vm.Date());
                        newMeal.Id(meal.id);
                        newMeal.MealTypeId(meal.mealTypeId);
                        newMeal.MealCalorieTarget(meal.mealCalorieTarget);
                        newMeal.MealCalorieMin(meal.mealCalorieMin);
                        newMeal.MealCalorieMax(meal.mealCalorieMax);
                        newMeal.ProteinGramsTarget(meal.proteinGramsTarget);
                        newMeal.CrabsGramsTarget(meal.crabsGramsTarget);
                        newMeal.SugarGramsTarget(meal.sugarGramsTarget);
                        newMeal.FatGramsTarget(meal.fatGramsTarget);
                        newMeal.UnsaturatedFatGramsTarget(meal.unsaturatedFatGramsTarget);
                        newMeal.SaturatedFatGramsTarget(meal.saturatedFatGramsTarget);
                        newMeal.FiberGramsTarget(meal.fiberGramsTarget);
                        newMeal.AlcoholGramsTarget(meal.alcoholGramsTarget);
                        newMeal.IsComplete(meal.isComplete);
                        newMeal.ScheduledTime_TimeSpan(truncateTime(meal.scheduledTime_TimeSpan));
                        newMeal.IsOngoing(meal.isOngoing);
                        newMeal.MealTypes = vm.MealTypes;
                        if (meal.mealTypeId != 4) {
                            vm.Meals.push(newMeal);
                            vm.EnableDisableAddMeal();
                        }
                    })
                });

        }
        catch (ex) {
            throw ex;
        }       
    }

    function getMealTypeScheduledTimeDefaults(vm) {
        var get_url = "/api/data/getmealtypes";

        try {
            fetch(get_url)
                .then((response) => {
                    if (!response.ok) {
                        throw new Error("GetMealTypes returned a not OK (200) response.");
                    }
                    return response.json();
                })
                .then((data) => {
                    vm.MealTypes = data;
                });
        }
        catch (ex) {
            console.log(ex);
            return null; //No defaults
        }
    }

    function init() {
        var options = window["editfoodplan_options"];

        var userId = options.UserId;
        var dayId = options.DayId;
        var isCopy = options.CopyMode;

        var vm = new ViewModel();
        vm.CopyMode(isCopy);
        vm.FkUserId(userId);
        getMealTypeScheduledTimeDefaults(vm);

        if (dayId != null) {
            vm.Id(isCopy ? 0 : dayId);
            vm.Date(document.getElementById("Date").value);
            loadMealsInDay(dayId, vm);
        }

        ko.applyBindings(vm, document.getElementById("dailyplan_editor"));
    }

    document.addEventListener("DOMContentLoaded", (evt) => {
        init();
    });
})();