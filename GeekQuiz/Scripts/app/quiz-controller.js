angular.module("QuizApp", []).controller("QuizCtrl", function ($scope, $http)
{
    $scope.options = [];
    $scope.title = "loading question...";
    $scope.answered = false;
    $scope.isCorrectAnswer = false;
    $scope.working = false;
    $scope.answer = function ()
    {
        return $scope.isCorrectAnswer ? "correct" : "incorrect";
    }

    $scope.sendAnswer = function (option)
    {
        $scope.answered = true;

        $http.post("/api/trivia", { "optionId": option.id, "questionId": option.questionId }).then(function success(response)
        {
            $scope.isCorrectAnswer = response.data;
        }, function error(response)
            {

            });
    };

    $scope.nextQuestion = function ()
    {
        $http.get("/api/trivia").then(function success(response)
        {
            $scope.answered = false;
            $scope.options = response.data.options;
            $scope.title = response.data.title;
        }, function error(response)
            {
                $scope.title = response.statusText;
            });
    };
});