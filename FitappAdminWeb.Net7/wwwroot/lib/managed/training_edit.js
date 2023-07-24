(function () {
    function Exercise() {
        var self = this;
        self.Editable = ko.observable(true);
        self.ExerciseId = 0;
        self.ExerciseTypeId = ko.observable(0);
        self.IsComplete = ko.observable(false);
        self.IsSkipped = ko.observable(false);
        self.ShowHasSetDefaults = ko.observable(false);
        self.Sets = ko.observableArray([]);

        self.UseSetDefault = function () {       
            if (confirm("Replace current Set with the set defaults?")) {
                var setDefault = getSetDefaultByExerciseType(self.ExerciseTypeId());
                if (setDefault.length > 0) {                  
                    self.Sets([]);

                    setDefault.forEach((sd) => {
                        var newSet = new Set();
                        newSet.SetSequenceNumber(sd.setSequenceNumber);

                        sd.setMetricDefaults.forEach((setmetric) => {
                            var newSetMetric = new SetMetric();
                            newSetMetric.TargetCustomMetric(setmetric.defaultCustomMetric.toString());
                            newSetMetric.SetMetricTypeId(setmetric.fkSetMetricTypeId);
                            newSet.SetMetrics.push(newSetMetric);
                        });

                        self.Sets.push(newSet);
                    });                  
                }
            }
        }

        self.OnExerciseChange = function (data, event) {
            var value = event.target.value;
            var setDefault = getSetDefaultByExerciseType(value);
            //only enable set defaults for NEW exercises
            self.ShowHasSetDefaults(self.Editable() && setDefault.length > 0);
        }

        self.AddSet = function() {
            var newSet = new Set();
            newSet.SetSequenceNumber(self.Sets().length + 1);

            self.Sets.push(newSet);
        }

        self.RemoveSet = function (set) {
            if (confirm("Are you sure you want to remove this set?")) { 
                self.Sets.remove(set);
            }           
        }
    }

    function Set() {
        var self = this;
        self.SetId = 0;
        self.Editable = ko.observable(true);
        self.SetSequenceNumber = ko.observable();
        self.SetMetrics = ko.observableArray([]);
        self.IsComplete = ko.observable(false);
        self.IsSkipped = ko.observable(false);

        self.AddMetric = function() {
            var metric = new SetMetric();
            self.SetMetrics.push(metric);
        }

        self.RemoveMetric = function (metric) {
            if (confirm("Are you sure you want to remove this metric?")) {
                self.SetMetrics.remove(metric);
            }          
        }
    }

    function SetMetric() {
        var self = this;
        self.SetMetricId = 0;
        self.Editable = ko.observable(true);
        self.TargetCustomMetric = ko.observable(0);
        self.ActualCustomMetric = ko.observable();
        self.SetMetricTypeId = ko.observable();
    }

    function ViewModel() {
        var self = this;
        self.EditMode = ko.observable();

        self.UserId = 0;
        self.TrainingSessionId = 0;
        self.Eds12WeekPlanId = ko.observable(0);
        self.Name = ko.observable("");
        self.StartDateTime_TimeSpan = ko.observable("");
        self.StartDateTime = ko.observable("");
        self.EndDateTime_TimeSpan = ko.observable("");
        self.EndDateTime = ko.computed(() => {
            var startDate = self.StartDateTime();
            if (startDate != null && startDate != "") {
                return startDate;
            }
            return null;
        }, this);
        self.IsTemplate = ko.observable(false);
        self.TemplateId = ko.observable();
        self.Description = ko.observable();
        self.EnableSubmit = ko.observable(true);
        self.Exercises = ko.observableArray([]);

        self.AddExercise = function() {
            var newExercise = new Exercise();
            newExercise.Id = 0;
            newExercise.ExerciseTypeId(0);

            self.Exercises.push(newExercise);
            self.SetupChoices();
        }

        self.SetupChoices = function () {
            
        }

        self.RemoveExercise = function (exercise) {
            if (confirm("Are you sure you want to remove this exercise?")) {
                self.Exercises.remove(exercise);
            }         
        }

        self.DisplayDateTime = function (date) {
            if (date != null && date != "") {
                var date_time = (new Date(date)).toISOString().split('T')[0];
                //console.log(date_time);
                return date_time;
            }
        }

        self.LoadTemplate = function () {
            var tempId = self.TemplateId();

            if (tempId > 0) {
                var url = "/Training/EditSession/{id}?pid={pid}&copy=true&tmpl={tmpl}";
                url = url.replace("{id}", tempId);
                url = url.replace("{pid}", self.Eds12WeekPlanId());
                url = url.replace("{tmpl}", self.IsTemplate());

                var date_param = getQueryFromUrl("date");
                if (date_param != null) {
                    url += "&date=" + date_param;
                }

                if (confirm("Loading a template will discard all current changes in this form. Continue?")) {
                    document.location.href = url;
                }
            }          
        }

        self.LoadTrainingSessionData = function () {
            self.EditMode(document.getElementById("editmode").value);
            var elem = document.getElementById("Data_IsTemplate");
            if (elem.type && elem.type == "checkbox") {
                self.IsTemplate(elem.checked);
            }
            else {
                self.IsTemplate(elem.value == "true");
            }
         
            self.Eds12WeekPlanId(document.getElementById("Data_Eds12WeekProgramId").value);
            self.Name(document.getElementById("Data_Name").value);

            if (!self.IsTemplate()) {
                self.StartDateTime(document.getElementById("Data_StartDateTime").value);
                self.StartDateTime_TimeSpan(document.getElementById("Data_StartDateTime_TimeSpan").value);
                //self.EndDateTime(document.getElementById("Data_EndDateTime").value);
                self.EndDateTime_TimeSpan(document.getElementById("Data_EndDateTime_TimeSpan").value);
            }           
            self.Description(document.getElementById("Data_Description").value);
        }

        self.SetupValidation = function () {
            $("#exEditor").validate({
                errorClass: "is-invalid",
                validClass: "is-valid",
            });
        }

        self.ValidateSelf = function () {
            //place some validation here, if needed
            return $("#exEditor").valid();
        }

        self.SubmitModel = function() {
            var add_url = "/api/data/AddTrainingSession";
            var edit_url = "/api/data/edittrainingsession";

            var url = self.EditMode() ? edit_url : add_url;

            //validate the model here
            if (self.ValidateSelf()) {
                var model = ko.toJS(self);
                if (model.IsTemplate) {
                    model.StartDateTime = "2023-01-01";
                    model.EndDateTime = "2023-01-01";
                    model.StartDateTime_TimeSpan = "00:00";
                    model.EndDateTime_TimeSpan = "00:00";
                }

                self.EnableSubmit(false);
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
                        console.log(data);
                        //redirect to training list page on start date
                        self.EnableSubmit(true);
                        var redirectUrl = "";
                        if (self.IsTemplate()) {
                            redirectUrl = "/lookup/trainingsessiontemplates";
                        }
                        else {
                            redirectUrl = "/training/sessions/" + self.UserId + "?date=" + self.StartDateTime() + "&pid=" + self.Eds12WeekPlanId();                            
                        }
                        document.location.href = redirectUrl;
                    });
                }
                catch (e) {
                    self.EnableSubmit(true);
                }
            }
        }
    }

    var toastTmpl = document.getElementById("toast_tmpl");
    function DisplayToast(header, body, time) {

        var newToast = bootstrap.Toast.getOrCreateInstance(toastTmpl);
    }

    var url_getexerciselist = "/api/Data/GetExercisesbyTrainingSession";
    var url_dummygetexerciselist = "/api/Data/GetDummyExercisesByTrainingSession";
    function loadExerciseData(model, userId, sessionId, isCopy, useDummyData) {
        var url = url_getexerciselist + "?sessionid=" + sessionId + "&copy=" + isCopy;
        if (useDummyData) {
            url = url_dummygetexerciselist + "?userid=" + userId + "&sessionid=" + sessionId;
        }

        fetch(url)
            .then((response) => response.json())
            .then((data) => {
                console.log(data);
                var exerciselist = [];
                //exercise loop
                data.forEach((ex) => {
                    //set loop
                    var exercise_editable = true;
                    var setlist = [];
                    if (ex.sets !== undefined) {
                        ex.sets.forEach((set) => {
                            //metric loop
                            var set_editable = true;
                            var setmetriclist = [];                          

                            var currSet = new Set();
                            currSet.SetId = set.setID;
                            currSet.SetSequenceNumber(set.setSequenceNumber);
                            currSet.IsComplete(set.isComplete);
                            currSet.IsSkipped(set.isSkipped);
                            currSet.Editable(!(set.isComplete || set.isSkipped) && set_editable);
                            exercise_editable = currSet.Editable();

                            set.setMetrics.forEach((setmetric) => {
                                var currMetric = new SetMetric();
                                currMetric.SetMetricId = setmetric.setMetricID;
                                if (setmetric.targetCustomMetric != null) {
                                    currMetric.TargetCustomMetric(setmetric.targetCustomMetric.toString());
                                }
                                if (setmetric.actualCustomMetric != null) {
                                    currMetric.ActualCustomMetric(setmetric.actualCustomMetric.toString());
                                }
                                currMetric.SetMetricTypeId(setmetric.setMetricTypeID);
                                currMetric.Editable(setmetric.actualCustomMetric == null && currSet.Editable());
                                set_editable = currMetric.Editable();
                                setmetriclist.push(currMetric);
                            });

                            currSet.SetMetrics(setmetriclist);
                            setlist.push(currSet);
                        });
                    }

                    var currExercise = new Exercise();
                    currExercise.ExerciseId = ex.exerciseID
                    currExercise.IsComplete(ex.isComplete);
                    currExercise.IsSkipped(ex.isSkipped);
                    currExercise.ExerciseTypeId(ex.exerciseTypeID);
                    currExercise.Sets(setlist);
                    currExercise.Editable(!(ex.isComplete || ex.isSkipped) && exercise_editable);
                    exerciselist.push(currExercise);
                });

                model.Exercises(exerciselist);
            });
    }

    function getQueryFromUrl(key) {
        const params = new Proxy(new URLSearchParams(window.location.search), {
            get: (searchParams, prop) => searchParams.get(prop),
        });
        return params[key];
    }

    function observeExerciseList() {
        var exerlist = document.getElementById("exercise_list");
        var config = { attributes: false, childList: true, subtree: true };

        var callback = (mutationList, observer) => {
            for (const mutation of mutationList) {
                if (mutation.type === "childList") {
                    console.log("Exercise List Modified");
                }
            }
        };

        var observer = new MutationObserver(callback);
        observer.observe(exerList, config);
    }

    var exertype_choices;
    function setupChoices() {
        //initial setup for existing exercises
        var url = "/api/data/getexercisetypechoices";
        var targets = document.querySelectorAll(".exertype-select");
        
        //retrieve exercise list first
        fetch(url)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Failed to retrieve exercise types choices");
                }
                return response.json();
            }).then((data) => {
                exertype_choices = data;

                //initialize choices js for all dropdowns
                for (const dd of targets) {
                    const choices = new Choices(dd, {
                    //choices: exertype_choices
                    });
                }
            }).catch((error) => {
                console.log(error);
            })     
    }

    var url_getallsetdefaults = "/api/data/getallsetdefaults";
    var setdefaults = [];
    function loadExerciseSetDefaults() {
        fetch(url_getallsetdefaults)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Failed getting set defaults. Settling with none.")
                }
                return response.json();
            }).then((data) => {
                console.log(data);
                setdefaults = data;
            }).catch((error) => {
                //no set defaults if error
                console.log(error);
                setdefaults = []; 
            });
    }

    function getSetDefaultByExerciseType(exid) {
        if (exid === null) {
            return null;
        }          

        var setdefaultslist = [];
        setdefaults.forEach((sd) => {
            if (sd.fkExerciseTypeId == exid) {
                return setdefaultslist.push(sd);
            }
        })
        return setdefaultslist;
    }

    var vm;    
    function Initialize(options) {
        var user_id = options.UserId;
        var session_id = options.SessionId;
        var useDummyData = options.UseDummyData;
        var isCopy = options.IsCopy;

        if (user_id != null && session_id != null) {
            vm = new ViewModel();
            vm.TrainingSessionId = session_id;
            vm.UserId = user_id;
            vm.LoadTrainingSessionData();
            loadExerciseSetDefaults();
            loadExerciseData(vm, user_id, session_id, isCopy, useDummyData);

            if (isCopy) {
                //we need to set this to 0 after loading the exercises
                vm.TrainingSessionId = 0;
            }

            //setupChoices();
            vm.SetupValidation();
        }
        else {
            vm = new ViewModel();
        }    

        //binding
        console.log(ko.toJS(vm));
        ko.applyBindings(vm, document.getElementById("exEditor"));
    }

    window["fitapp"] = {
        TrainingEdit: { Initialize: Initialize }
    };
})();