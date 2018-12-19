(function (app) {
    app.controller('tagCategoryEditController', tagCategoryEditController);
    tagCategoryEditController.$inject = ['apiService', '$scope', 'notificationService', '$state', '$stateParams','commonService'];
    function tagCategoryEditController(apiService, $scope, notificationService, $state, $stateParams, commonService) {
        $scope.tagCategory = {

        }


        $scope.loadTagCategoryDetail = loadTagCategoryDetail;
        $scope.UpdateTagCategory = UpdateTagCategory;
   

  
        function loadTagCategoryDetail() {
            apiService.get('/api/tagcategory/getbyid/' + $stateParams.id, null, function (result) {
                $scope.tagCategory = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function UpdateTagCategory() {
            apiService.put('/api/tagcategory/update', $scope.tagCategory,
                function (result) {
                    notificationService.displaySuccess('Đã cập nhật ' + result.data.Name + ' thành công');
                    $state.go('tag_categories');
                }, function (error) {
                    console.log(error);
                    notificationService.displayError('Cập nhật không thành công');
                });
        }

      

        loadTagCategoryDetail();
    }
})(angular.module('uStora.tag_categories'));