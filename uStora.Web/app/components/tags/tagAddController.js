(function (app) {
    app.controller('tagAddController', tagAddController);
    tagAddController.$inject = ['apiService', '$scope', 'notificationService', '$state', 'commonService'];
    function tagAddController(apiService, $scope, notificationService, $state, commonService) {
        $scope.tag = {
            //CreatedDate: new Date(),
            //Status: true
            Type:'product'
        }
        $scope.flatFolders = [];


        $scope.loadTagCategories = loadTagCategories;
        $scope.AddTag = AddTag;
        $scope.GetSeoTitle = GetSeoTitle;


        function GetSeoTitle() {
            $scope.tag.ID = commonService.getSeoTitle($scope.tag.Name);
        };

        function AddTag() {
            //$scope.product.MoreImages = JSON.stringify($scope.moreImages);
            apiService.post('/api/tag/create', $scope.tag,
                function (result) {
                    notificationService.displaySuccess('Đã thêm ' + result.data.Name + ' thành công');
                    $state.go('tags');
                }, function (error) {
                    console.log(error);
                    notificationService.displayError('Thêm không thành công');
                });
        };

        function loadTagCategories() {
            apiService.get('api/tagcategory/getallparents', null, function (result) {
                $scope.tagCategories = result.data;
            }, function () {
                console.log('Cannot get list parent');
            });
        };

        //function times(n, str) {
        //    var result = '';
        //    for (var i = 0; i < n; i++) {
        //        result += str;
        //    }
        //    return result;
        //};
        //function recur(item, level, arr) {
        //    arr.push({
        //        Name: times(level, '–') + ' ' + item.Name,
        //        ID: item.ID,
        //        Level: level,
        //        Indent: times(level, '–')
        //    });
        //    if (item.children) {
        //        item.children.forEach(function (item) {
        //            recur(item, level + 1, arr);
        //        });
        //    }
        //};

        loadTagCategories();

        //loadManus();
    }
})(angular.module('uStora.tags'));