(function () {
    /**
     * "Main" method of the application
     */
    var kinspectorAppName = 'knlApp';
    angular.module(kinspectorAppName, ['ngRoute', 'knlServices', 'knlDirectives'])
        /**
         * The first visible page. User sets Kentico settings and then is able to redirect to the main menu.
         * Its route is set in the module config - is bound to '/target-setup' route
         */
        .controller('knlTargetSetupController', function ($scope, $location, $http, $rootScope, knlTargetConfigService) {
            $scope.model = $scope.model || {};
            $scope.model.config = knlTargetConfigService.getConfig();

            // This gets called when user pushes a button to connect
            $scope.connect = function () {

                // Save the config no matter if it's valid or not
                knlTargetConfigService.setConfig($scope.model.config);

                $scope.model.showLoader = true;

                $http.get("http://localhost:9000/api/modules/GetKenticoVersion/", { params: $scope.model.config })
                    .success(function (data) {

                        // Hack - clear the cached module results
                        localStorage.setItem('knlModuleResults', null);
                        localStorage.setItem('knlSetupModuleMetadata', null);
                        localStorage.setItem('knlModuleMetadata', null);
                        localStorage.setItem('knlModuleMetadataSecurity', null);

                        $scope.model.showLoader = false;
                        $location.path('/main-menu');
                    })
                    .error(function () {
                        $scope.model.showLoader = false;
                    });
            }
        })
        /**
         * Main menu page - allows the user to choose whether to perform instance setup, or analyse it.
         */
        .controller('knlMainMenuController', function ($scope, $location, $http, knlTargetConfigService, knlModuleService) {
            $scope.model = $scope.model || {};
            $scope.model.config = knlTargetConfigService.getConfig();
            $scope.model.showLoader = false;

            // This gets called when user pushes a button for analysis
            $scope.analyse = function (category) {
                if (angular.isUndefined(category)) {
                    category = '';
                }

                // Hack - clear the cached module results
                localStorage.setItem('knlModuleResults', null);
                localStorage.setItem('knlModuleMetadata' + category, null);
                
                // TODO: value encoding for query
                $location.path('/results/' + category);
            }
            
            // This gets called when user pushes a button for instance setup
            $scope.setupInstance = function () {
                // Hack - clear the cached module results
                localStorage.setItem('knlModuleResults', null);
                localStorage.setItem('knlSetupModuleMetadata', null);

                $location.path('/setup-instance');
            }
        })
        /**
         * Controller for displaying page with instance setup, is visible after the user selects instance setup in the main menu.
         * Its route is set in the module config - is bound to '/setup-instance' route
         */
        .controller('knlSetupInstanceController', function ($scope, $location, $http, knlTargetConfigService, knlModuleService, kiExportService) {
            $scope.model = $scope.model || {};
            $scope.model.modules = $scope.model.modules || [];
            knlModuleService.getAllSetupMetadata()
                .then(function (data) {
                    if (data) {
                        // This needs to be here for orderBy filter to work, since it works only with arrays
                        angular.forEach(data, function (elem) { $scope.model.modules.push(elem); });

                        // Reset module export selection
                        kiExportService.selectorsVisible = false;
                        kiExportService.selectedModules = [];
                    }
                })
                .catch(function () {
                    knlTargetConfigService.disconnect();
                });
        })
        /**
         * Controller for displaying page with results, is visible after the user selects analysis in the main menu. 
         * Its route is set in the module config - is bound to '/results' route
         */
        .controller('knlResultsController', function ($scope, $location, $http, $routeParams, knlTargetConfigService, knlModuleService, kiExportService) {
            $scope.model = $scope.model || {};
            $scope.model.modules = $scope.model.modules || [];
            knlModuleService.getAllMetadata($routeParams.category)
                .then(function (data) {
                    if (data) {
                        // This needs to be here for orderBy filter to work, since it works only with arrays
                        angular.forEach(data, function (elem) { $scope.model.modules.push(elem); });

                        // Reset module export selection
                        kiExportService.selectorsVisible = false;
                        kiExportService.selectedModules = [];
                    }
                })
                .catch(function () {
                    knlTargetConfigService.disconnect();
                });
        })
        /**
         * Interceptor that reads all the HTTP responses and pushes them to the knlErrorService (located in Services.js)
         */
        .factory('knlErrorMessageInterceptor', ['$q', 'knlErrorService', function ($q, errorService) {
            return {
                responseError: function (rejection) {
                    var errorMessage = rejection.status === 0
                        ? "Web API server is not running, run the executable KInspector.Web.exe first."
                        : rejection.data || "Server error.";
                    errorService.triggerError(errorMessage);
                    return $q.reject(rejection);
                },
            }
        }])
        /**
         * Web.config of the application
         */
        .config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {
            $routeProvider
                .when('/target-setup', {
                    templateUrl: 'partials/target-setup.html',
                    controller: 'knlTargetSetupController'
                })
                .when('/main-menu', {
                    templateUrl: 'partials/main-menu.html',
                    controller: 'knlMainMenuController'
                })
                .when('/setup-instance', {
                    templateUrl: 'partials/setup-instance.html',
                    controller: 'knlSetupInstanceController'
                })
                .when('/results/:category?', {
                    templateUrl: 'partials/results.html',
                    controller: 'knlResultsController'
                })
                .otherwise({
                    redirectTo: '/target-setup'
                });

            $httpProvider.interceptors.push('knlErrorMessageInterceptor');
        }])
        /**
         * Maps enumerable ModuleResultsType (found in KInspector.Core) to templates.
         */
        .constant('knlTemplateMapping', {
            0: "partials/module-item-string.html",
            1: "partials/module-item-list.html",
            2: "partials/module-item-table.html",
            3: "partials/module-item-tablelist.html",
            10: "partials/module-item-string-trusted.html",
            11: "partials/module-item-list-trusted.html"
        });

    angular.bootstrap(window.document, [kinspectorAppName]);
}());