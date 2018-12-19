(function (app) {
    app.controller('productAddController', productAddController);
    productAddController.$inject = ['apiService', '$scope', 'notificationService', '$state', 'commonService'];
    function productAddController(apiService, $scope, notificationService, $state, commonService) {
        $scope.product = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.flatFolders = [];
        $scope.brands = [];
  
        $scope.loadBrands = loadBrands;
        $scope.loadTags = loadTags;
        $scope.loadProductCategories = loadProductCategories;
        $scope.AddProduct = AddProduct;
        $scope.GetSeoTitle = GetSeoTitle;
        $scope.moreImages = [];
        //$scope.availableTags = [{ name: 'ocho', description: null, id: 1 }, { name: 'manosc', description: null, id: 2 }, { name: 'vietnamo', description: null, id: 3 }];
        $scope.avaiTags = [];
        $scope.thisClose = thisClose;
        $scope.ckeditorOptions = {
            languague: 'vi',
            height: '200px'
        }
        function thisClose(img) {
            var listImage = $scope.moreImages;
            var index = listImage.indexOf(img);
            if (index > -1) {
                listImage.splice(index, 1);
            }
            console.log(index);
        }
        function loadBrands() {

            apiService.get('/api/product/listbrands', null,
               function (result) {
                   $scope.brands = result.data;
               }, function () {
                   notificationService.displayError("Không có nhãn hiệu nào được tìm thấy.");
               })
        }

        function loadManus(page) {
           
            apiService.get('/api/product/manufactors', null,
                function (result) {
                    $scope.manus = result.data;
                }, function () {
                    notificationService.displayError("Không có nhà cung cấp nào được tìm thấy.");
                })
        }

        $scope.chooseMoreImages = function () {
            var finder = new CKFinder();

            finder.selectActionFunction = function (fileUrl) {
                $scope.$apply(function () {
                    $scope.moreImages.push(fileUrl);
                })
            };
            finder.popup();
        }

        $scope.chooseImage = function () {
            var finder = new CKFinder();

            finder.selectActionFunction = function (fileUrl) {
                $scope.$apply(function () {
                    $scope.product.Image = fileUrl;
                })
            }
            finder.popup();
        };

        function GetSeoTitle() {
            $scope.product.Alias = commonService.getSeoTitle($scope.product.Name);
        };

        function AddProduct() {
            $scope.product.MoreImages = JSON.stringify($scope.moreImages);
            apiService.post('/api/product/create', $scope.product,
                function (result) {
                    notificationService.displaySuccess('Đã thêm ' + result.data.Name + ' thành công');
                    $state.go('products');
                }, function (error) {
                    console.log(error);
                    notificationService.displayError('Thêm không thành công');
                });
        };

        function loadProductCategories() {
            apiService.get('/api/productcategory/getallparents', null,
                function (result) {
                    $scope.parentCategories = commonService.getTree(result.data, "ID", "ParentID");
                    $scope.parentCategories.forEach(function (item) {
                        recur(item, 0, $scope.flatFolders);
                    });
                }, function () {
                    console.log('Không có dữ liệu!!!');
                });
        };

        function times(n, str) {
            var result = '';
            for (var i = 0; i < n; i++) {
                result += str;
            }
            return result;
        };
        function recur(item, level, arr) {
            arr.push({
                Name: times(level, '–') + ' ' + item.Name,
                ID: item.ID,
                Level: level,
                Indent: times(level, '–')
            });
            if (item.children) {
                item.children.forEach(function (item) {
                    recur(item, level + 1, arr);
                });
            }
        };

        function loadTags() {
            apiService.get('/api/product/listtags', null,
                function (result) {
                    $scope.avaiTags = result.data;
                    $scope.finalArray = $scope.avaiTags.map(function (obj) {
                        return obj.Name;
                    });
                }, function () {
                    notificationService.displayError("Không có tags nào được tìm thấy.");
                })

            function split(val) {
                return val.split(/,\s*/);
            }
            function extractLast(term) {
                return split(term).pop();
            }

            $("#tags")
                // don't navigate away from the field on tab when selecting an item
                .on("keydown", function (event) {
                    if (event.keyCode === $.ui.keyCode.TAB &&
                        $(this).autocomplete("instance").menu.active) {
                        event.preventDefault();
                    }
                })
                .autocomplete({
                    minLength: 0,
                    source: function (request, response) {
                        // delegate back to autocomplete, but extract the last term
                        response($.ui.autocomplete.filter(
                            $scope.finalArray, extractLast(request.term)));
                    },
                    focus: function () {
                        // prevent value inserted on focus
                        return false;
                    },
                    select: function (event, ui) {
                        var terms = split(this.value);
                        // remove the current input
                        terms.pop();
                        // add the selected item
                        terms.push(ui.item.value);
                        // add placeholder to get the comma-and-space at the end
                        terms.push("");
                        this.value = terms.join(", ");
                        return false;
                    }
                });
        };
  
        loadProductCategories();

        loadBrands();
     
        loadManus();
        loadTags();
    }
})(angular.module('uStora.products'));