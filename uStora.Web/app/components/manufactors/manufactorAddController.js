(function (app) {
    app.controller('manufactorAddController', manufactorAddController);
    manufactorAddController.$inject = ['apiService', '$scope', 'notificationService', '$state'];
    function manufactorAddController(apiService, $scope, notificationService, $state) {
        $scope.manufactor = {}
        $scope.Addmanufactor = Addmanufactor;
        $scope.ckeditorOptions = {
            languague: 'vi',
            height: '200px'
        }
        
        $scope.chooseImage = function () {
            var finder = new CKFinder();

            finder.selectActionFunction = function (fileUrl) {
                $scope.$apply(function () {
                    $scope.manufactor.LogoUrl = fileUrl;
                })
            }
            finder.popup();
        }
        function Addmanufactor() {
            apiService.post('/api/manufactor/create', $scope.manufactor,
                function (result) {
                    notificationService.displaySuccess('Đã thêm ' + result.data.Name + ' thành công');
                    $state.go('manufactors');
                }, function (error) {
                    console.log(error);
                    notificationService.displayError('Thêm không thành công');
                });
        }
    }
})(angular.module('uStora.manufactors'));