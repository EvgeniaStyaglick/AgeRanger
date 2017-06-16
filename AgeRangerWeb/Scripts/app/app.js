var AgeRangerApp = angular.module('AgeRangerApp', ['ngRoute', 'AgeRangerControllers']);
AgeRangerApp.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/list',
    {
        templateUrl: 'Person/list.html',
        controller: 'ListController'
    }).
    when('/create',
    {
        templateUrl: 'Person/edit.html',
        controller: 'EditController'
    }).
    when('/edit/:id',
    {
        templateUrl: 'Person/edit.html',
        controller: 'EditController'
    }).
    when('/delete/:id',
    {
        templateUrl: 'Person/delete.html',
        controller: 'DeleteController'
    }).
    otherwise(
    {
        redirectTo: '/list'
    });
}]);