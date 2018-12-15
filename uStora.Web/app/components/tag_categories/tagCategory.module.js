(function () {
    angular.module('uStora.tag_categories', ['uStora.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('tag_categories', {
                url: "/tag_categories",
                parent: 'base',
                templateUrl: "/app/components/tag_categories/tagCategoryListView.html",
                controller: "tagCategoryListController"
            })
            .state('add_tag_category', {
                url: "/add_tag_category",
                parent: 'base',
                templateUrl: "/app/components/tag_categories/tagCategoryAddView.html",
                controller: "tagCategoryAddController"
            })
            .state('edit_tag_category', {
                url: "/edit_tag_category/:id",
                parent: 'base',
                templateUrl: "/app/components/tag_categories/tagCategoryEditView.html",
                controller: "tagCategoryEditController"
            });
    }
})();