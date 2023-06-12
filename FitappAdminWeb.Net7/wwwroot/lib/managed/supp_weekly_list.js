(function () {
    function CopyModalViewModal() {
        var self = this;
        self.UserId = ko.observable();
        self.PlanId = ko.observable();
        self.PlanList = ko.observableArray([]);

        self.LoadPlans = function (data, event) {
            console.log(data);
            console.log(event);
            var targetVal = event.target.value;

            self.PlanId(null);
            self.PlanList([]);

            var url_planload = "/api/data/getplanslistbyuser?userid=" + targetVal;
            fetch(url_planload)
                .then((response) => {
                    return response.json();
                })
                .then((data) => {
                    console.log(data);
                    self.PlanList(data);
                });
        };

        self.SubmitModel = function () {
            var url = "/Supplement/EditPlan/{id}?copy=true";
            url = url.replace("{id}", self.PlanId());
            document.location.href = url;
        };
    }

    var delete_url = "/api/data/deletesupplementplan";

    function callDelete(e) {
        if (window.confirm("Are you sure you want to delete this plan? This cannot be undone.")) {
            var tgt = e.currentTarget;
            if (tgt == null) {
                tgt = e.target;
            }

            var idToDelete = tgt.getAttribute("data-id");
            fetch(delete_url, {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(idToDelete)
            })
                .then((response) => {
                    if (!response.ok) {
                        throw new Error("Delete Failed.");
                    }
                    return response.json();
                })
                .then((data) => {
                    if (data == true) {
                        document.location.reload();
                    }
                    else {
                        throw new Error("Delete Failed.");
                    }
                })
                .catch((error) => {
                    window.FitApp.showToast("Error", "Failed to delete plan", "danger");
                });
        }
    }

    var deleteButtons = document.querySelectorAll("button[action='delete']");
    deleteButtons.forEach((button) => {
        button.addEventListener("click", callDelete);
    });

    var modalvm = new CopyModalViewModal();
    var modalform = document.getElementById("copymodal");
    ko.applyBindings(modalvm, modalform);
})();