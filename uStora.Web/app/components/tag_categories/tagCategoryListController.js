﻿(function (app) {
    app.controller('tagCategoryListController', tagCategoryListController);

    tagCategoryListController.$inject = ['$scope', 'apiService', 'notificationService', '$ngBootbox', '$filter']
    function tagCategoryListController($scope, apiService, notificationService, $ngBootbox, $filter) {
        $scope.tagCategories = [];
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;

        $scope.keyword = '';

        $scope.search = search;

        $scope.deletetagCategory = deleteTagCategory;

        $scope.selectAll = selectAll;

        $scope.deleteTagCategoryMulti = deleteTagCategoryMulti;

        function deleteTagCategoryMulti() {
            $ngBootbox.confirm('Tất cả dữ liệu đã chọn sẽ bị xóa. Bạn muốn tiếp tục?').then(function () {

                var listId = [];
                $.each($scope.selected, function (i, item) {
                    listId.push(item.ID);
                });
                var config = {
                    params: {
                        selectedTagCategories: JSON.stringify(listId)
                    }
                }
                apiService.del('/api/tagcategory/deletemulti', config, function (result) {
                    notificationService.displaySuccess('Xóa thành công ' + result.data + ' bản ghi.');
                    search();
                }, function (error) {
                    notificationService.displayError('Xóa không thành công');
                });
            });
        }

        $scope.isAll = false;
        function selectAll() {
            if ($scope.isAll === false) {
                angular.forEach($scope.tagCategories, function (item) {
                    item.checked = true;
                });
                $scope.isAll = true;
            } else {
                angular.forEach($scope.tagCategories, function (item) {
                    item.checked = false;
                });
                $scope.isAll = false;
            }
        }

        $scope.$watch('tagCategories', function (n, o) {
            var checked = $filter('filter')(n, { checked: true });

            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
            }
            else {
                $('#btnDelete').attr('disabled', 'disabled');
            }
        }, true);



        function deleteTagCategory(id) {
            $ngBootbox.confirm('Bạn chắc chắn muốn xóa bản ghi này?').then(function () {
                var config = {
                    params: {
                        id: id
                    }
                }
                apiService.del('/api/tagcategory/delete', config, function () {
                    notificationService.displaySuccess('Đã xóa thành công.');
                    search();
                }, function () {
                    notificationService.displayWarning('Xóa không thành công!!!');
                })
            });
        }

        function search() {
            getTagCategories();
        }

        $scope.getTagCategories = getTagCategories;

        function getTagCategories(page) {
            $scope.loading = true;
            page = page || 0;
            var config = {
                params: {
                    keyword: $scope.keyword,
                    page: page,
                    pageSize: 15
                }
            }
            apiService.get('/api/tagcategory/getall', config, function (result) {
                $scope.tagCategories = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
            }, function () {
                console.log('Không có tag nào!!!');
            });
        }
        $scope.getTagCategories();
    }
})(angular.module('uStora.tag_categories'));