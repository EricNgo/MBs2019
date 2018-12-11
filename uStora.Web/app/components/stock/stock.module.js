(function () {
    angular.module('uStora.stock', ['uStora.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('stock', {
                url: "/stock",
                parent: 'base',
                templateUrl: "/app/components/stock/stockListView.html",
                controller: "stockListController"
            });
    }
})();