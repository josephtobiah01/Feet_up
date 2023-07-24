(function () {

    function init() {
        var chart = window["trainingcharts_datas"];
        for (let item of chart.data) {
            drawChart(item);
        }
    }

    document.addEventListener("DOMContentLoaded", (evt) => {
        init();
    });

    function drawChart(item) {
        var Name = item.Name;
        var LabelData1 = 'Not yet started';
        var LabelData2 = 'Completed';
        var LabelData3 = 'Skipped';
        var LabelData4 = 'Total';

        var Data_1 = item.InComplete;
        var Data_2 = item.Complete;
        var Data_3 = item.Skipped;
        var Data_4 = item.Total;
        var Label = item.Label;

        var Id = item.Id;

        new Chart(Id, {
            type: "bar",
            data: {
                labels: Label,
                datasets: [
                {
                    label: LabelData4,
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(0,0,255,1.0)", // Blue
                    borderColor: "rgba(0,0,255,0.1)", // Blue
                    type: 'line',
                    data: Data_4
                },
                {
                    label: LabelData2,
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(9,121,105,1.0)", // Green
                    borderColor: "rgba(9,121,105,0.3)", // Green
                    data: Data_2
                },
                {
                    label: LabelData3,
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(255,0,0,1.0)", // Red
                    borderColor: "rgba(255,0,0,0.3)", // Red
                    data: Data_3
                },
                {
                    label: LabelData1,
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(125, 125, 125, 1.0)", // Gray
                    borderColor: "rgba(125, 125, 125, 0.1)", //Gray
                    data: Data_1
                }]
                /*,
                {
                    label: LabelData5,
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(255,255,0,1.0)",
                    borderColor: "rgba(255,255,0,0.3)",
                    data: Data_5
                }
                */
            },
            options: {
                //indexAxis: 'x',
                indexAxis: 'y',
                legend: { display: false },
                plugins: {
                    title: {
                        display: true,
                        text: Name
                    }
                },
                scales: {
                    x: {
                        title: {
                            color: 'rgba(192,192,192,1.0)',
                            display: true,
                            text: 'Number of sets'
                        },
                        stacked: true
                    },
                    y: {
                        title: {
                            color: 'rgba(192,192,192,1.0)',
                            display: false,
                            text: 'Exercise Count'
                        },
                        stacked: true
                    }
                }
            }
        });
    }
})();