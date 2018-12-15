(function () {
    angular.module('uStora.tags', ['uStora.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('tags', {
                url: "/tags",
                parent: 'base',
                templateUrl: "/app/components/tags/tagListView.html",
                controller: "tagListController"
            }).state('add_tag', {
                url: "/add_tag",
                parent: 'base',
                templateUrl: "/app/components/tags/tagAddView.html",
                controller: "tagAddController"
            }).state('edit_tag', {
                url: "/edit_tag/:id",
                parent: 'base',
                templateUrl: "/app/components/tags/tagEditView.html",
                controller: "tagEditController"
            }).state('tag_detail', {
                url: "/tags/:id",
                parent: 'base',
                templateUrl: "/app/components/tags/tagDetailView.html",
                controller: "tagDetailController"
            });
    }
})();