(function (app) {
    app.controller('stockListController', stockListController);

    stockListController.$inject = ['$scope', '$sce', 'apiService', 'notificationService', '$ngBootbox', '$filter']

    function stockListController($scope, $sce, apiService, notificationService, $ngBootbox, $filter) {
        $scope.stockList = [];
        $scope.statusList = [
            { id: 0, name: "Còn hàng" },
            { id: 1, name: "Sắp hết hàng" },
            { id: 2, name: "Hết hàng" }
        ];

        $scope.products = [];
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;

        $scope.quantity = 0;

        $scope.keyword = '';
                               
        $scope.search = search;

        $scope.updateStock = function () {
            apiService.put('/api/product/updatestock', $scope.stockList, function () {
                notificationService.displaySuccess('Đã cập nhật thành công.');
                getstocks();
            }, function () {
                notificationService.displayError("Đã xảy ra lỗi.");
            });
        }

        function search() {
            getstocks();
        }

        $scope.getstocks = getstocks;

        function getstocks(page) {
            $scope.loading = true;
            page = page || 0;
            var config = {
                params: {
                    filter: $scope.keyword,
                    page: page,
                    pageSize: 20
                }
            }

            apiService.get('/api/product/getstock', config, function (result) {
                $scope.stockList = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
            }, function () {
                console.log('Không có stock nào!!!');
            });
        }
        $scope.getstocks();
    }
})(angular.module('uStora.stock'));