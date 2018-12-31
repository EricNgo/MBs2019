﻿(function (app) {
    app.controller('trackOrderPickupController', trackOrderPickupController);
    trackOrderPickupController.$inject = ['apiService', '$scope', 'notificationService', '$state', '$stateParams', 'commonService'];
    function trackOrderPickupController(apiService, $scope, notificationService, $state, $stateParams, commonService) {
        $scope.trackorder = {
            CreateDate: new Date()
        }
        $scope.orders = [];
        $scope.vehicles = [];
        $scope.users = [];
        //$scope.loadTrackOrderDetail = loadTrackOrderDetail;
        $scope.UpdateTrackOrder = UpdateTrackOrder;
        $scope.ckeditorOptions = {
            languague: 'vi',
            height: '200px'
        }
        function loadOrders() {
            apiService.get('/api/trackorder/getorders', null,
               function (result) {
                   $scope.orders = result.data;
               }, function () {
                   notificationService.displayError("Không có đơn hàng nào được tìm thấy.");
               })
        }

        function loadUsers() {
            apiService.get('/api/trackorder/getdriver', null,
               function (result) {
                   $scope.users = result.data;
               }, function () {
                   notificationService.displayError("Không có bản ghi nào được tìm thấy.");
               })
        }
        function loadTrackOrderDetail() {
            apiService.get('/api/trackorder/getbyid/' + $stateParams.id, null, function (result) {
                $scope.trackorder = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        function UpdateTrackOrder() {
            apiService.put('/api/trackorder/updatepickup', $scope.trackorder,
                function (result) {
                    notificationService.displaySuccess('Đơn hàng của ' + result.data.Order.CustomerName + ' đã được lấy');
                    $state.go('trackorders');
                }, function (error) {
                    console.log(error);
                    notificationService.displayError('Cập nhật không thành công');
                });
        }
        loadOrders();

        loadUsers();
        loadTrackOrderDetail();
    }
})(angular.module('uStora.trackorders'));