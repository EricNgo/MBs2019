(function (app) {
    app.controller('tagListController', tagListController);

    tagListController.$inject = ['$scope', 'apiService', 'notificationService', '$ngBootbox', '$filter']
    function tagListController($scope, apiService, notificationService, $ngBootbox, $filter) {
        $scope.tags = [];
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;

        $scope.keyword = '';

        $scope.search = search;

        $scope.deletetag = deleteTag;

        $scope.selectAll = selectAll;

        $scope.deleteTagMulti = deleteTagMulti;

        function deleteTagMulti() {
            $ngBootbox.confirm('Tất cả dữ liệu đã chọn sẽ bị xóa. Bạn muốn tiếp tục?').then(function () {

                var listId = [];
                $.each($scope.selected, function (i, item) {
                    listId.push(item.ID);
                });
                var config = {
                    params: {
                        selectedtags: JSON.stringify(listId)
                    }
                }
                apiService.del('/api/tag/deletemulti', config, function (result) {
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
                angular.forEach($scope.tags, function (item) {
                    item.checked = true;
                });
                $scope.isAll = true;
            } else {
                angular.forEach($scope.tags, function (item) {
                    item.checked = false;
                });
                $scope.isAll = false;
            }
        }

        $scope.$watch('tags', function (n, o) {
            var checked = $filter('filter')(n, { checked: true });

            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
            }
            else {
                $('#btnDelete').attr('disabled', 'disabled');
            }
        }, true);



        function deleteTag(id) {
            $ngBootbox.confirm('Bạn chắc chắn muốn xóa bản ghi này?').then(function () {
                var config = {
                    params: {
                        id: id
                    }
                }
                apiService.del('/api/tag/delete', config, function () {
                    notificationService.displaySuccess('Đã xóa thành công.');
                    search();
                }, function () {
                    notificationService.displayWarning('Xóa không thành công!!!');
                })
            });
        }

        function search() {
            getTags();
        }

        $scope.getTags = getTags;

        function getTags(page) {
            $scope.loading = true;
            page = page || 0;
            var config = {
                params: {
                    keyword: $scope.keyword,
                    page: page,
                    pageSize: 15
                }
            }
            apiService.get('/api/tag/getall', config, function (result) {
                $scope.tags = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
            }, function () {
                console.log('Không có tag nào!!!');
            });
        }
        $scope.getTags();
    }
})(angular.module('uStora.tags'));