var AgeRangerControllers = angular.module("AgeRangerControllers", []);

AgeRangerControllers.controller("ListController", ['$scope', '$http', '$location',
    function ($scope, $http, $location) {
        $http.get('/api/person').then(function (data) {
            $scope.persons = data.data;
            $scope.filteredPersons = $scope.persons;
            $scope.searchText = '';

            $scope.filterItems = function() {
                $scope.filteredPersons = $scope.persons.filter(function (item) {
                    var searchInUpper = $scope.searchText.toUpperCase();
                    return item.firstName.toUpperCase().indexOf(searchInUpper) !== -1
                        || item.lastName.toUpperCase().indexOf(searchInUpper) !== -1
                        || item.age.toString().indexOf(searchInUpper) !== -1
                        || item.ageGroup.toUpperCase().indexOf(searchInUpper) !== -1;
                });
            };

            $scope.create = function () {
                $location.path('/create');
            };

            $scope.edit = function (id) {
                $location.path('/edit/' + id);
            };

            $scope.delete = function (id) {
                if (confirm("Are you sure you want to delete the person with id = " + id + "?")) {
                    $http.delete('/api/person/' + id).then(function (data) {
                        $scope.persons = $scope.persons.filter(function (obj) {
                            return obj.id !== id;
                        });
                    }).catch(function (data) {
                        $scope.error = "An error has occured while deleting person!";
                    });
                }
            };
        }).catch(function (data) {
            $scope.error = "An error has occured while extracting person list!";
        });
    }
]);

AgeRangerControllers.controller("EditController", ['$scope', '$filter', '$http', '$routeParams', '$location',
    function ($scope, $filter, $http, $routeParams, $location) {
        $scope.id = -1;
        $scope.save = function () {
            var obj = {
                id: $scope.id,
                firstName: $scope.firstname,
                lastName: $scope.lastname,
                age: $scope.age
            };
            if ($scope.id == -1) {
                $http.post('/api/person/', obj).then(function (data) {
                    $location.path('/list');
                }).catch(function (data) {
                    $scope.error = "An error has occured while adding person!";
                });
            }
            else {
                $http.put('/api/person/', obj).then(function (data) {
                    $location.path('/list');
                }).catch(function (data) {
                    $scope.error = "An Error has occured while saving person!";
                });
            }
        }
        if ($routeParams.id) {
            $scope.id = $routeParams.id;
            $scope.title = "Edit Person";
            $http.get('/api/person/' + $routeParams.id).then(function (data) {
                $scope.firstname = data.data.firstName;
                $scope.lastname = data.data.lastName;
                $scope.age = data.data.age;
                $scope.agegroup = data.data.ageGroup;
            }).catch(function (data) {
                $scope.error = "An Error has occured while getting person info!";
            });
        }
        else {
            $scope.title = "Create New Person";
        }
    }
]);