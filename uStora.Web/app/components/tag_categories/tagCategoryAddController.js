(function (app) {
    app.controller('tagCategoryAddController', tagCategoryAddController);
    tagCategoryAddController.$inject = ['apiService', '$scope', 'notificationService','$state','commonService'];
    function tagCategoryAddController(apiService, $scope, notificationService, $state, commonService) {
        $scope.tagCategory = {
            //CreatedDate: new Date(),
            //Status: true
        }
        
        //$scope.flatFolders = [];
        //$scope.loadParentCategories = loadParentCategories;
        $scope.AddTagCategory = AddTagCategory;
        //$scope.GetSeoTitle = GetSeoTitle;

        //$scope.ckeditorOptions = {
        //    languague: 'vi',
        //    height: '200px'
        //}

        //$scope.chooseImage = function () {
        //    var finder = new CKFinder();

        //    finder.selectActionFunction = function (fileUrl) {

        //        $scope.$apply(function () {
        //            $scope.tagCategory.Image = fileUrl;
        //        })
        //    }
        //    finder.popup();
        //};

        //function GetSeoTitle() {
        //    $scope.productCategory.Alias = commonService.getSeoTitle($scope.productCategory.Name);
        //};

        function AddTagCategory() {
            apiService.post('/api/tagcategory/create', $scope.tagCategory,
                function (result) {
                    notificationService.displaySuccess('Đã thêm ' + result.data.Name + ' thành công');
                    $state.go('tag_categories');
                }, function (error) {
                    console.log(error);
                    notificationService.displayError('Thêm không thành công');
                });
        }

        //function loadParentCategories() {
        //    apiService.get('/api/productcategory/getallparents', null,
        //        function (result) {
        //            $scope.parentCategories = commonService.getTree(result.data, "ID", "ParentID");
        //            $scope.parentCategories.forEach(function (item) {
        //                recur(item, 0, $scope.flatFolders);
        //            });
        //        }, function () {
        //            console.log('Không có dữ liệu!!!');
        //        });
        //};

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

        //loadParentCategories();
    }
})(angular.module('uStora.tag_categories'));