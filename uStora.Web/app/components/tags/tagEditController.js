(function (app) {
    app.controller('tagEditController', tagEditController);
    tagEditController.$inject = ['apiService', '$scope', 'notificationService', '$state', '$stateParams', 'commonService'];
    function tagEditController(apiService, $scope, notificationService, $state, $stateParams, commonService) {
        $scope.tag = {
            //UpdatedDate: new Date()
        }
        $scope.loadTagDetail = loadTagDetail;
        $scope.loadTagCategories = loadTagCategories;
        $scope.GetSeoTitle = GetSeoTitle;
        $scope.UpdateTag = UpdateTag;

        function GetSeoTitle() {
            $scope.tag.ID = commonService.getSeoTitle($scope.tag.Name);
        };
        function loadTagDetail() {
            apiService.get('/api/tag/getbyid/' + $stateParams.id, null, function (result) {
                $scope.tag = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function loadTagCategories() {
            apiService.get('api/tagcategory/getallparents', null, function (result) {
                $scope.tagCategories = result.data;
            }, function () {
                console.log('Cannot get list parent');
            });
        };

        function UpdateTag() {
            apiService.put('/api/tag/update', $scope.tag,
                function (result) {
                    notificationService.displaySuccess('Đã cập nhật ' + result.data.Name + ' thành công');
                    $state.go('tags');
                }, function (error) {
                    console.log(error);
                    notificationService.displayError('Cập nhật không thành công');
                });
        }
        loadTagDetail();
        loadTagCategories()
    }
})(angular.module('uStora.tags'));