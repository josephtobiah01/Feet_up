(function () {

    var options;

    function init() {
        options = window["nutritioncharts_options"];
        for (let item of options.data) {
            drawChart(item);
        }
    }

    document.addEventListener("DOMContentLoaded", (evt) => {
        init();
    });

    function drawChart(item) {
        var Name = item.Name + ' (g)';
        var LabelTarget = 'Target ' + Name;
        var LabelActual = 'Actual ' + Name;
        var Target = item.Target;
        var Actual = item.Actual;
        var Label = item.Label;
        var Id = item.Name + "Chart";

        if (item.Name == "Calories") {
            Name = item.Name;
            LabelTarget = 'Target ' + item.Name;
            LabelActual = 'Actual ' + item.Name;
        }     

        new Chart(Id, {
            type: "line",
            data: {
                labels: Label,
                datasets: [{
                    label: LabelTarget,
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(255, 0, 0, 1.0)",
                    borderColor: "rgba(255, 0, 0, 0.1)",
                    data: Target
                },
                {
                    label: LabelActual,
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(0,0,255,1.0)",
                    borderColor: "rgba(0,0,255,0.1)",
                    data: Actual
                }]
            },
            options: {
                legend: { display: false },
                plugins: {
                    title: {
                        display: true,
                        text: Name
                    }
                }
            }
        });
    }
})();