(function () {
    var previewimg_elem = document.getElementById("image_preview_img");
    var field_imgurl = document.getElementById("extype_image");
    var reader;

    var max_img_width = 163;
    var max_img_height = 121;

    function SetDefault() {
        var self = this;

        self.Id = ko.observable();
        self.SetSequenceNumber = ko.observable();
        self.SetMetricDefaults = ko.observableArray([]);

        self.AddMetricDefault = function () {
            var metric = new SetMetricDefault();
            self.SetMetricDefaults.push(metric);
        }

        self.RemoveMetricDefault = function (metric) {
            self.SetMetricDefaults.remove(metric);
        }
    }

    function SetMetricDefault() {
        var self = this;
        self.Id = ko.observable();
        self.FkSetMetricTypeId = ko.observable();
        self.DefaultCustomMetric = ko.observable();
    }

    function ViewModel() {
        var self = this;
        self.Id = ko.observable();
        self.Name = ko.observable();
        self.EditMode = ko.observable(false);
        self.ExplainerTextFr = ko.observable();
        self.ExplainerVideoFr = ko.observable();
        self.FkLevelId = ko.observable();
        self.FkExerciseClassId = ko.observable();
        self.FkForceId = ko.observable();
        self.FkEquipmentId = ko.observable();
        self.FkSportId = ko.observable();
        self.FkMechanicsTypeId = ko.observable();
        self.FkMainMuscleWorkedId = ko.observable();
        self.FkOtherMuscleWorkedId = ko.observable();
        self.ExerciseImage = ko.observable();

        self.ExerciseClassFreeText = ko.observable();
        self.MainMuscleWorkedFreeText = ko.observable();
        self.OtherMuscleFreeText = ko.observable();
        self.EquipmentFreeText = ko.observable();
        self.MechanicsTypeFreeText = ko.observable();
        self.LevelFreeText = ko.observable();
        self.SportFreeText = ko.observable();
        self.ForceFreeText = ko.observable();

        self.SetDefaults = ko.observableArray([]);

        self.EnableLevelFreeText = ko.computed(function () {
            return this.FkLevelId() === "0";
        }, self);
        self.EnableExerciseClassFreeText = ko.computed(function () {
            return this.FkExerciseClassId() === "0";
        }, self)
        self.EnableForceFreeText = ko.computed(function () {
            return this.FkForceId() === "0";
        }, self)
        self.EnableEquipmentFreeText = ko.computed(function () {
            return this.FkEquipmentId() === "0";
        }, self)
        self.EnableSportFreeText = ko.computed(function () {
            return this.FkSportId() === "0";
        }, self)
        self.EnableMechanicsTypeFreeText = ko.computed(function () {
            return this.FkMechanicsTypeId() === "0";
        }, self)
        self.EnableMainMuscleWorkedFreeText = ko.computed(function () {
            return this.FkMainMuscleWorkedId() === "0";
        }, self)
        self.EnableOtherMainMuscleWorkedFreeText = ko.computed(function () {
            return this.FkOtherMuscleWorkedId() === "0";
        }, self)

        self.InitializeValues = function () {
            self.Id(document.getElementById("Data_Id").value);
            self.Name(document.getElementById("Data_Name").value);
            self.ExplainerTextFr(document.getElementById("Data_ExplainerTextFr").value);
            self.ExplainerVideoFr(document.getElementById("Data_ExplainerVideoFr").value);
            self.EditMode(document.getElementById("editmode").value);
            self.ExerciseImage(document.getElementById("Data_ExerciseImage").value);

            self.FkLevelId(document.getElementById("Data_FkLevelId").value);
            self.FkExerciseClassId(document.getElementById("Data_FkExerciseClassId").value);
            self.FkForceId(document.getElementById("Data_FkForceId").value);
            self.FkEquipmentId(document.getElementById("Data_FkEquipmentId").value);
            self.FkSportId(document.getElementById("Data_FkSportId").value);
            self.FkMechanicsTypeId(document.getElementById("Data_FkMechanicsTypeId").value);
            self.FkMainMuscleWorkedId(document.getElementById("Data_FkMainMuscleWorkedId").value);
            self.FkOtherMuscleWorkedId(document.getElementById("Data_FkOtherMuscleWorkedId").value);

            self.ExerciseClassFreeText(document.getElementById("Data_ExerciseClassFreeText").value);
            self.MainMuscleWorkedFreeText(document.getElementById("Data_MainMuscleWorkedFreeText").value);
            self.OtherMuscleFreeText(document.getElementById("Data_OtherMuscleFreeText").value);
            self.EquipmentFreeText(document.getElementById("Data_EquipmentFreeText").value);
            self.MechanicsTypeFreeText(document.getElementById("Data_MechanicsTypeFreeText").value);
            self.LevelFreeText(document.getElementById("Data_LevelFreeText").value);
            self.SportFreeText(document.getElementById("Data_SportFreeText").value);
            self.ForceFreeText(document.getElementById("Data_ForceFreeText").value);
        };

        self.SetupValidation = function () {
            $("#exertype_editor").validate({
                errorClass: "is-invalid",
                validClass: "is-valid",
            });
        };

        self.ValidateModel = function () {
            return $("#exertype_editor").valid();
        }

        self.AddSetDefault = function () {
            var newSet = new SetDefault();
            newSet.SetSequenceNumber(self.SetDefaults().length + 1);
            newSet.AddMetricDefault();

            self.SetDefaults.push(newSet);
        }

        self.RemoveSetDefault = function (set) {
            self.SetDefaults.remove(set);
        }

        self.SubmitModel = function () {
            var add_url = "/api/data/addexercisetype";
            var edit_url = "/api/data/editexercisetype";
            var uploadimage_url = "/lookup/uploadimage";

            var url = self.EditMode() ? edit_url : add_url;

            if (self.ValidateModel()) {
                var model = ko.toJS(self);
                var uploadedFile = field_imgurl.files[0];

                console.log("=====POSTING=====");
                console.log(uploadedFile);
                console.log(url);

                try {
                    
                    //NOTE: upload image first to retrieve blob url then save the edits/add afterwards
                    //If upload fails, set upload url to null and move on.
                    //NOTE: Use formdata to pass the file here..
                    //We might need to create the action not in WebAPI depending on support. In that case, create a seperate controller for this. /Image/Upload maybe

                    if (uploadedFile != null) {
                        var img_formdata = new FormData();
                        img_formdata.append("image", uploadedFile);

                        fetch(uploadimage_url, {
                            method: "POST",
                            cache: "no-cache",
                            credentials: "include",
                            body: img_formdata
                        })
                            .then((response) => response.json())
                            .then((data) => {
                                console.log(data);
                                model.ExerciseImage = data;

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
                                        var redirectUrl = "/lookup/exercisetype";
                                        document.location.href = redirectUrl;
                                    });
                            })
                    }
                    else {
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
                            var redirectUrl = "/lookup/exercisetype";
                            document.location.href = redirectUrl;
                        });
                    }
                }
                catch (ex) {
                    console.log(ex);
                }
            }         
        }

        self.InitializeValues();
    }

    var url_getsetdefaults = "/api/data/GetSetDefaults";
    function loadSetDefaults(model, exerciseTypeId) {
        var url = url_getsetdefaults + "?extypeid=" + exerciseTypeId;

        fetch(url)
            .then((response) => response.json())
            .then((data) => {
                console.log(data);
                var setList = [];

                data.forEach((set) => {
                    var metricList = [];

                    var currSet = new SetDefault();
                    currSet.Id(set.id);
                    currSet.SetSequenceNumber(set.setSequenceNumber);

                    if (set.edsSetMetricsDefault !== undefined) {
                        set.edsSetMetricsDefault.forEach((setmetric) => {
                            var currMetric = new SetMetricDefault();
                            currMetric.Id(setmetric.id);
                            currMetric.DefaultCustomMetric(setmetric.defaultCustomMetric);
                            currMetric.FkSetMetricTypeId(setmetric.fkSetMetricTypeId);

                            metricList.push(currMetric);
                        });
                    }

                    currSet.SetMetricDefaults(metricList);
                    setList.push(currSet);
                });

                model.SetDefaults(setList);
            });
    }

    function Initialize(options) {
        var vm = new ViewModel();
        if (vm.Id() > 0) {
            loadSetDefaults(vm, vm.Id());
        }
        vm.SetupValidation();

        previewimg_elem.onload = function () {
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
                    previewimg_elem.src = e.target.result;
                };
            }
            else {
                console.log("[WARN] FileReader not available. Falling back to CreateObjectURL.");
                const [file] = field_imgurl.files;
                if (file) {
                    previewimg_elem.src = URL.createObjectURL(file);
                }
            }
        };

        ko.applyBindings(vm, document.getElementById("exertype_editor"));
    }

    window["fitapp"] = {
        ExerTypeEdit: { Initialize: Initialize }
    }
})();