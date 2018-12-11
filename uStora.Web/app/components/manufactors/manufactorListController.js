
(function (app) {
    app.controller('manufactorListController', slideListController);

    slideListController.$inject = ['$scope','$sce', 'apiService', 'notificationService', '$ngBootbox', '$filter']
    function slideListController($scope,$sce, apiService, notificationService, $ngBootbox, $filter) {
        $scope.manus = [];
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;

        $scope.keyword = '';

        $scope.search = search;

        $scope.deleteManufactor = deleteManufactor;

        $scope.selectAll = selectAll;

        $scope.deleteManuMulti = deleteManuMulti;
          

        function deleteManuMulti() {
            $ngBootbox.confirm('Tất cả dữ liệu đã chọn sẽ bị xóa. Bạn muốn tiếp tục?').then(function () {

                var listId = [];
                $.each($scope.selected, function (i, item) {
                    listId.push(item.Id);
                });
                var config = {
                    params: {
                        selectedManufactors: JSON.stringify(listId)
                    }
                }
                apiService.del('/api/manufactor/deletemulti', config, function (result) {
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
                angular.forEach($scope.manus, function (item) {
                    item.checked = true;
                });
                $scope.isAll = true;
            } else {
                angular.forEach($scope.manus, function (item) {
                    item.checked = false;
                });
                $scope.isAll = false;
            }
        }

        $scope.$watch('manus', function (n, o) {
            var checked = $filter('filter')(n, { checked: true });

            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
            }
            else {
                $('#btnDelete').attr('disabled', 'disabled');
            }
        }, true);



        function deleteManufactor(id) {
            $ngBootbox.confirm('Bạn chắc chắn muốn xóa bản ghi này?').then(function () {
                var config = {
                    params: {
                        id: id
                    }
                }
                apiService.del('/api/manufactor/delete', config, function () {
                    notificationService.displaySuccess('Đã xóa thành công.');
                    search();
                }, function () {
                    notificationService.displayWarning('Xóa không thành công!!!');
                })
            });
        }

        function search() {
            getmanus();
        }

        $scope.getmanus = getmanus;

        function getmanus(page) {
            $scope.loading = true;
            page = page || 0;
            var config = {
                params: {
                    keyword: $scope.keyword,
                    page: page,
                    pageSize: 20
                }
            }
            apiService.get('/api/manufactor/getall', config, function (result) {
                $scope.manus = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
            }, function () {
                console.log('Không có nhà cung cấp nào!!!');
            });
        }
        $scope.getmanus();
    }
})(angular.module('uStora.manufactors'));