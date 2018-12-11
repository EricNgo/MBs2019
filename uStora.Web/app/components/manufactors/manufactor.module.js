(function () {
    angular.module('uStora.manufactors', ['uStora.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('manufactors', {
                url: "/manufactors",
                parent: 'base',
                templateUrl: "/app/components/manufactors/manufactorListView.html",
                controller: "manufactorListController"
            }).state('add_manufactor', {
                url: "/add_manufactor",
                parent: 'base',
                templateUrl: "/app/components/manufactors/manufactorAddView.html",
                controller: "manufactorAddController"
            }).state('edit_manufactor', {
                url: "/edit_manufactor/:id",
                parent: 'base',
                templateUrl: "/app/components/manufactors/manufactorEditView.html",
                controller: "manufactorEditController"
            });
    }
})();