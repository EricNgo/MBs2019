(function (app) {
    app.controller('orderDetailController', orderDetailController);
    orderDetailController.$inject = ['apiService', '$scope', '$stateParams', 'notificationService', '$state'];

    function orderDetailController(apiService, $scope, $stateParams, notificationService, $state) {

        $scope.order = {};

        $scope.loading = true;

        $scope.getOrder = getOrder;
        $scope.exportProductToXsls = exportProductToXsls;
        $scope.total = 0;
        function getOrder() {
            apiService.get('/api/order/getbyid/' + $stateParams.id +'/true', null,
                function (result) {
                    $scope.order = result.data;
                    for (let i = 0; i <= result.data.OrderDetails.length; i++) {
                        $scope.total += result.data.OrderDetails[i].Product.Price * result.data.OrderDetails[i].Quantity;
                    }
                }, function (error) {
                    console.log(error);
                    notificationService.displayError('Load dữ liệu không thành công');
                });
        }

        function exportProductToXsls() {
            $scope.loading = true;
           
            apiService.get('/api/order/exporttoexcel/' + $stateParams.id, null, function (response) {
                if (response.status == 200) {
                    window.location.href = response.data.Message;
                }
            }, function (error) {
                notificationService.displayError(error);

            });
            $scope.loading = false;
        }

        getOrder();
    }
})(angular.module('uStora.orders'));