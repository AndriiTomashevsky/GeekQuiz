angular.module("QuizApp", []).controller("QuizCtrl", function ($scope, $http)
{
    $scope.answered = false;
    $scope.title = "loading question...";
})