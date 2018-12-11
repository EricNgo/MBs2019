(function (app) {
    app.controller('manufactorEditController', manufactorEditController);
    manufactorEditController.$inject = ['apiService', '$scope', 'notificationService', '$state', '$stateParams', 'commonService'];
    function manufactorEditController(apiService, $scope, notificationService, $state, $stateParams, commonService) {
        $scope.manufactor = {};

        $scope.loadmanufactorDetail = loadmanufactorDetail;
        $scope.Updatemanufactor = Updatemanufactor;


        $scope.chooseImage = function () {
            var finder = new CKFinder();

            finder.selectActionFunction = function (fileUrl) {
                $scope.$apply(function () {
                    $scope.manufactor.LogoUrl = fileUrl;
                })
            }
            finder.popup();
        }

        function loadmanufactorDetail() {
            apiService.get('/api/manufactor/getbyid/' + $stateParams.id, null, function (result) {
                $scope.manufactor = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        function Updatemanufactor() {
            apiService.put('/api/manufactor/update', $scope.manufactor,
                function (result) {
                    notificationService.displaySuccess('Đã cập nhật ' + result.data.Name + ' thành công');
                    $state.go('manufactors');
                }, function (error) {
                    console.log(error);
                    notificationService.displayError('Cập nhật không thành công');
                });
        }
        loadmanufactorDetail();
    }
})(angular.module('uStora.manufactors'));