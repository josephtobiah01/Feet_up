window["dishbrowser_modal"] = (function () {
    var url_loadDish = "/api/data/getdishdetails?id=";
    var url_loadDishList = "/api/data/getdishlist?userid=";
    var elem_modal = null;
    var cache = {};

    function _loadDishData(dishid) {
        var url = url_loadDish + dishid;
        try {
            if (cache[dishid] != null) {
                return Promise.resolve(cache[dishid]);
            }

            //returns a promise to attach callbacks afterwards
            return fetch(url)
                .then((response) => {
                    if (!response.ok) {
                        throw new Error("Cannot retrieve dish details for id " + dishid);
                    }
                    return response.json();
                })
                .then((data) => {
                    cache[dishid] = data;
                    return data;
                });
        }
        catch (ex) {
            return Promise.reject(ex);
        }       
    }

    function _loadDishList(userid) {
        var url = url_loadDishList + userid;

        try {
            return fetch(url)
                .then((response) => {
                    if (!response.ok) {
                        throw new Error("Cannot get dish list from user id " + userid);
                    }
                    return response.json();
                })
        }
        catch (ex) {
            return Promise.reject(ex);
        }
    }

    function Dish() {
        var self = this;
        self.Id = ko.observable();
        self.FkDishType = ko.observable();
        self.CreationTimestamp = ko.observable();
        self.CompletionTimestamp = ko.observable();
        self.Name = ko.observable();
        self.Remarks = ko.observable();
        self.TranscriberRemarks = ko.observable();
        self.UploadPhotoReference = ko.observable();
        self.CalorieActual = ko.observable();
        self.ProteinActual = ko.observable();
        self.CrabsActual = ko.observable();
        self.SugarActual = ko.observable();
        self.FatActual = ko.observable();
        self.UnsaturatedFatActual = ko.observable();
        self.FiberGramsActual = ko.observable();
        self.SaturatedFatGramsActual = ko.observable();
        self.AlcoholGramsActual = ko.observable();
    }

    function ViewModel() {
        var self = this;
        self.UserId = ko.observable();
        self.SelectedDishId = ko.observable();
        self.SelectedDish = ko.observable();
        self.DishList = ko.observableArray([]);

        self.SubmitCallback = () => { };

        self.OnChangeSelected = function (data, evt) {
            var field_target = evt.currentTarget ?? evt.target;
            var dishId = field_target.value;

            _loadDishData(dishId).then((data) => {
                self.SelectedDish(data);
            }).catch((ex) => {
                throw ex;
            });
        };

        self.SubmitModal = function () {
            self.Callback(self.SelectedDish());
        };
    }

    function _init(options) {
        var userId = options.UserId;
        var callback = options.Callback;
        var modalid = options.ModalId ?? "dishbrowser_modal";

        var vm = new ViewModel();
        vm.UserId(userId)
        _loadDishList(userId).then((data) => {
            vm.DishList(data);
        })
        vm.Callback = callback;

        elem_modal = document.getElementById(modalid);
        ko.applyBindings(vm, elem_modal);
    };

    return {
        Initialize: _init
    };
})();