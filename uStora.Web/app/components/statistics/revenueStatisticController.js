(function (app) {

    app.controller('revenueStatisticController', revenueStatisticController);
    revenueStatisticController.$inject = ['apiService', '$scope', 'notificationService', '$filter', 'commonService'];
    function revenueStatisticController(apiService, $scope, notificationService, $filter, commonService) {
        $scope.tableData = [];
        $scope.getStatistic = getStatistic;
        $scope.getStatisticByQuaterly = getStatisticByQuaterly;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.labels = [];
        $scope.labels2 = [];
        $scope.series = ['Doanh thu'];
        $scope.chartData = [];
        $scope.chartData2 = [];
        $scope.loading = true;
        $scope.fromDate = '01/12/2016';
        $scope.toDate = '01/12/' + new Date().getFullYear();

        function getStatistic(page) {
            page = page || 0;
            var config = {
                params: {
                    fromDate: commonService.strToDate($scope.fromDate),
                    toDate: commonService.strToDate($scope.toDate),
                    page: page,
                    pageSize: 8
                }
            }
            apiService.get('/api/statistic/getrevenue', config, function (response) {
                var data = response.data.Items;
                $scope.tableData = data;
                $scope.page = response.data.Page;
                $scope.pagesCount = response.data.TotalPages;
                $scope.totalCount = response.data.TotalCount;
                var labels = [];
                var chartData = [];
                var revenues = [];
                var benefit = [];
                $.each(data, function (i, item) {
                    labels.push($filter('date')(item.Date, 'dd/MM/yyyy'));
                    revenues.push(item.Revenues);
                    benefit.push(item.Benefit);

                });
                chartData.push(revenues);
                //chartData.push(benefit);
                $scope.labels = labels;
                $scope.chartData = chartData;
                $scope.loading = false;
            }, function (response) {
                setTimeout(function () {
                    $scope.loading = false;
                }, 100);
            });
        }
        function getStatisticByQuaterly() {
            var config = {
                param: {
                    //mm/dd/yyyy
                    fromDate: '01/01/2017',
                    toDate: '01/01/2019'
                }
            }
            apiService.get('api/statistic/getrevenuebyquaterly?fromDate=' + config.param.fromDate + "&toDate=" + config.param.toDate, null, function (response) {
                $scope.tabledata2 = response.data;
                var labels2 = [];
                var chartData2 = [];
                var revenues2 = [];
                var benefits2 = [];
                $.each(response.data, function (i, item) {
                    //labels1.push($filter('Month')(item.Month));
                    labels2.push(item.Quarter + '-' + item.Year);
                    revenues2.push(item.Revenues);
                    benefits2.push(item.Benefit);
                });
                chartData2.push(revenues2);
                chartData2.push(benefits2);

                $scope.chartdata2 = chartData2;
                $scope.labels2 = labels2;
            }, function (response) {
                notificationService.displayError('Không thể tải dữ liệu');
            });
        }
        $('#fromDate').click(function () {
            jQuery('#fromDate').datetimepicker({
                format: 'd/m/Y',
                lang: 'vi',
                timepicker: false
            });
        });
        $('#toDate').click(function () {
            jQuery('#toDate').datetimepicker({
                format: 'd/m/Y',
                lang: 'vi',
                timepicker: false
            });
        });

        $scope.getStatistic();
        $scope.getStatisticByQuaterly();
    }
})(angular.module('uStora.statistics'));