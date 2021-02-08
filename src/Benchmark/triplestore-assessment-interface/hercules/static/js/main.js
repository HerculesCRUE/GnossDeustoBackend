(function() {
    'use strict';

    var app = angular.module('HerculesApp', []);

    // We also have Jinja2, so change delimiters
    app.config(['$interpolateProvider', function($interpolateProvider) {
        $interpolateProvider.startSymbol('{a');
        $interpolateProvider.endSymbol('a}');
    }]);

    app.controller('HerculesController', ['$scope', '$log', '$http',
        function($scope, $log, $http) {
            $http.post(apidata.url, sessionStorage)
                .then(function(response) {
                	$scope.customrank = Object.entries(response.data.overrides).length !== 0
                    $scope.ranking = response.data.ranking
                })
        }
    ]);

}());