function loadChart(chart) {

    for (let value of chart) {
        barChart(value);
    }

    function barChart(value) {

        console.log(value.ID);

        const labels = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
        const data = {
            labels: labels,
            datasets: [{
                axis: 'y',
                label: value.Name,
                data: [1,6,8,5,5,4,3,10],
                fill: false,
                backgroundColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(255, 159, 64, 0.2)',
                    'rgba(255, 205, 86, 0.2)',
                    'rgba(75, 192, 192, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(153, 102, 255, 0.2)',
                    'rgba(201, 203, 207, 0.2)',
                    'rgba(153, 102, 255, 0.2)',
                ],
                borderColor: [
                    'rgb(255, 99, 132)',
                    'rgb(255, 159, 64)',
                    'rgb(255, 205, 86)',
                    'rgb(75, 192, 192)',
                    'rgb(54, 162, 235)',
                    'rgb(153, 102, 255)',
                    'rgb(201, 203, 207)',
                    'rgb(153, 102, 255)',
                ],
                borderWidth: 1
            }]
        };

        new Chart(value.ID, {
            type: 'bar',
            data,
            options: {
                indexAxis: 'y',
            }
        });
    }

    function createChart(chartId, day, suppCount, actual) {
        new Chart(chartId, {
            type: "line",
            data: {
                labels: day,
                datasets: [
                {
                    label: 'Target',
                    fill: false,
                    lineTension: 0,
                    backgroundColor: "black",
                    borderColor: "rgb(0,0,255)",
                    data: suppCount
                },
                {
                    label: 'Actual',
                    fill: false,
                    lineTension: 0,
                    backgroundColor: "black",
                    borderColor: "yellow",
                    data: actual
                }]
            },
            options: {
                legend: { display: true },
            }
        });
    }
}