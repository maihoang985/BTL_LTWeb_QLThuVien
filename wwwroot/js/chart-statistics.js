// File: wwwroot/js/chart-statistics.js

const ChartStatistics = (function () {
    // Private variables
    const chartColors = [
        '#696cff', '#71dd37', '#ff3e1d', '#03c3ec',
        '#ffab00', '#8592a3', '#ff6384', '#36a2eb',
        '#cc65fe', '#ffce56', '#4bc0c0', '#9966ff'
    ];
    const labelColor = '#6c757d';
    const borderColor = '#f1f1f1';

    let chart = null;
    let chartElement = null;
    let yearSelector = null;

    // Private methods
    function showLoading() {
        chartElement.innerHTML = `
            <div class="d-flex justify-content-center mt-3">
                <div class="spinner-border" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>`;
    }

    function showError(message) {
        chartElement.innerHTML = `
            <div class='alert alert-danger text-center mt-3'>
                ❌ ${message}
            </div>`;
    }

    function showNoData(year) {
        chartElement.innerHTML = `
            <div class='alert alert-warning text-center mt-3'>
                ⚠️ Không có dữ liệu thống kê cho năm ${year}.
            </div>`;
    }

    function destroyChart() {
        if (chart) {
            chart.destroy();
            chart = null;
        }
    }

    function createChartConfig(data, year) {
        const categories = data.map(x => 'Tháng ' + x.thang);
        const counts = data.map(x => x.soLuotMuon);
        const maxValue = Math.max(0, ...counts) + Math.ceil(Math.max(0, ...counts) * 0.1);

        return {
            series: [{
                name: 'Số lượt mượn',
                data: counts
            }],
            chart: {
                height: 400,
                type: 'bar',
                toolbar: { show: false }
            },
            title: {
                text: `Thống kê lượt mượn sách năm ${year}`,
                align: 'center',
                style: {
                    fontSize: '18px',
                    color: '#566a7f',
                    fontWeight: '600'
                }
            },
            xaxis: {
                min: 0,
                max: maxValue,
                tickAmount: 5,
                categories: categories,
                axisBorder: { show: false },
                labels: {
                    style: {
                        colors: labelColor,
                        fontSize: '13px'
                    }
                },
                title: {
                    text: 'Số Lượt Mượn',
                    style: {
                        color: labelColor,
                        fontSize: '14px',
                        fontWeight: 600
                    }
                }
            },
            plotOptions: {
                bar: {
                    horizontal: true,
                    barHeight: '30%',
                    borderRadius: 8,
                    borderRadiusApplication: 'end'
                }
            },
            grid: {
                borderColor: borderColor,
                xaxis: { lines: { show: false } },
                padding: { top: -20, bottom: -12 }
            },
            colors: chartColors,
            dataLabels: {
                enabled: true,
                style: {
                    fontSize: '13px',
                    fontWeight: 'bold',
                    colors: ['#fff']
                },
                formatter: val => (val > 0 ? val : '')
            },
            yaxis: {
                labels: {
                    style: {
                        colors: labelColor,
                        fontSize: '14px',
                        fontWeight: '500'
                    }
                }
            },
            legend: { show: false },
            tooltip: {
                y: {
                    formatter: val => `${val} lượt`
                }
            }
        };
    }

    async function fetchData(year) {
        const response = await fetch(`/ThongKe/GetLuotMuonTheoThang?year=${year}`);

        if (!response.ok) {
            throw new Error(`Lỗi HTTP ${response.status} - Không tìm thấy API hoặc có lỗi ở server.`);
        }

        return response.json();
    }

    function renderChart(year) {
        destroyChart();
        showLoading();

        fetchData(year)
            .then(data => {
                if (!data || data.length === 0) {
                    showNoData(year);
                    return;
                }

                chartElement.innerHTML = '';
                const chartConfig = createChartConfig(data, year);
                chart = new ApexCharts(chartElement, chartConfig);
                chart.render();
            })
            .catch(err => {
                showError(err.message);
            });
    }

    function setCurrentYear() {
        const currentYear = new Date().getFullYear();
        const options = [...yearSelector.options];

        if (options.some(o => o.value == currentYear)) {
            yearSelector.value = currentYear;
        }
    }

    function initEventListeners() {
        yearSelector.addEventListener('change', function () {
            renderChart(this.value);
        });
    }

    // Public API
    return {
        init: function (chartElementId, yearSelectorId) {
            chartElement = document.querySelector(chartElementId);
            yearSelector = document.querySelector(yearSelectorId);

            if (!chartElement || !yearSelector) {
                console.error('Chart element or year selector not found!');
                return;
            }

            setCurrentYear();
            renderChart(yearSelector.value);
            initEventListeners();
        },

        destroy: function () {
            destroyChart();
        },

        refresh: function (year) {
            renderChart(year || yearSelector.value);
        }
    };
})();

// Auto-init khi DOM ready
document.addEventListener('DOMContentLoaded', function () {
    ChartStatistics.init('#horizontalBarChart', '#yearSelector');
});