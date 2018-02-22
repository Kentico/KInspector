(function () {
    /**
     * ALL the directives in the application are in here.
     */
    angular.module('knlDirectives', [])
        /**
         * A directive that is containing the module. This is basically the black/red/orange/green/blue bar with a name
         * and a button to expand.
         */
        .directive('knlModuleContainer', ['kiExportService', function (exportService) {
            return {
                restrict: 'E',
                transclude: true,
                scope: true,
                templateUrl: "partials/module-item-container.html",
                link: function ($scope) {
                    $scope.model = $scope.model || {};

                    $scope.resultsVisible = false;

                    $scope.selectorsVisible = function () {
                        return exportService.selectorsVisible;
                    }

                    $scope.toggleModuleSelection = function(moduleName) {
                        var index = exportService.selectedModules.indexOf(moduleName);
                        if (index > 0) {
                            exportService.selectedModules.splice(index, 1);
                        } else {
                            exportService.selectedModules.push(moduleName);
                        }

                    }

                    $scope.toggleResultsVisibility = function (e) {
                        if ($scope.model.moduleLoaded) {
                            $scope.resultsVisible = !$scope.resultsVisible;
                            if ($scope.resultsVisible) {
                                $('table').stickyTableHeaders();
                            }
                        }
                    }

                    $scope.commentsVisible = false;
                    $scope.toggleCommentsVisibility = function (e) {
                    	$scope.commentsVisible = !$scope.commentsVisible;
                    }

                    $scope.$watchCollection('model.results', function (newResults) {
                        if (newResults) {
                            $scope.model.resultClass = 'result-status-' + newResults.Status;
                        }
                    });
                }
            }
        }])
        /**
         * This directive is the content of the module. Meaning it is wrapped in the knlModuleContainer directive. When you collapse
         * the red/green/orange/blue module, you will see this.
         */
		.directive('knlModule', ['$http', '$sce', 'knlErrorService', 'knlModuleService', 'knlTemplateMapping', function ($http, $sce, errorService, moduleService, knlTemplateMapping) {
		    return {
		        scope: {
		            moduleMetaData: '=knlModule'
		        },
		        template: '<div ng-include="model.templateUrl"></div>',
		        link: function ($scope, $element) {

		            $scope.model = $scope.model || {};

		            // Priority defines the order in which the modules will be displayed
		            // -1 priority for modules that weren't loaded yet
		            $scope.moduleMetaData.Priority = -1;

                    // The right result type will be chosen after loading the module
		            $scope.model.templateUrl = knlTemplateMapping[0];

		            $scope.loadModule = function (forced, keepPriority) {
		                $scope.model.moduleLoaded = false;
		                if (!$scope.model.moduleLoading) {
		                    $scope.model.moduleLoading = true;

		                    moduleService.loadModule($scope.moduleMetaData.Name, forced)
                                .then(function (result) {
                                    if (result) {
                                        $scope.model.results = result;
                                        // Skip results encoding for trusted results - supported only for String and List<String>
                                        if (result.Trusted) {
                                            if (result.ResultType == 0) {
                                                $scope.model.results.Result = $sce.trustAsHtml(result.Result);
                                            }
                                            else if (result.ResultType == 1) {
                                                for (var i = result.Result.length - 1; i >= 0; i -= 1) {
                                                    $scope.model.results.Result[i] = $sce.trustAsHtml(result.Result[i]);
                                                }
                                            }
                                        }
                                        $scope.model.moduleLoaded = true;

                                        if (!keepPriority) {
                                            if (result.Points === null) {
                                                // 101 is for modules that don't return anything. Put them even below the green ones
                                                $scope.moduleMetaData.Priority = 101;
                                            }
                                            else {
                                                $scope.moduleMetaData.Priority = result.Points;
                                            }
                                        }

                                        // Templates for unencoded results have offset 10
                                        var templateNo = result.Trusted ? result.ResultType + 10 : result.ResultType;
                                        if (!knlTemplateMapping[templateNo]) {
                                            errorService.triggerError("Template for module " + $scope.moduleMetaData.Name + " was not found.");
                                        } else {
                                            $scope.model.templateUrl = knlTemplateMapping[templateNo];
                                        }
                                    }
                                })
                                .finally(function () {
                                	$scope.model.moduleLoading = false;
                                });
		                }
		            };
		        }
		    };
		}])
        /**
         * The left menu with disconnect button only.
         */
        .directive('knlModulesSimpleSidebar', ['knlTargetConfigService', function (configService) {
            return {
                restrict: 'E',
                scope: {
                    modules: '='
                },
                templateUrl: "partials/modules-simple-sidebar.html",
                link: function ($scope) {
                    $scope.model = {};

                    var config = configService.getConfig();
                    $scope.model.serverName = config.Server;
                    $scope.model.databaseName = config.Database;

                    $scope.disconnect = function () {
                        configService.disconnect();
                    };
                }
            }
        }])
        /**
         * The left menu with disconnect button.
         */
        .directive('knlModulesSidebar', ['knlTargetConfigService', 'knlNavigationService', 'kiExportService', function (configService, navigationService, exportService) {
            return {
                restrict: 'E',
                scope: {
                    modules: '='
                },
                templateUrl: "partials/modules-sidebar.html",
                link: function ($scope) {
                    $scope.model = {};

                    var config = configService.getConfig();
                    $scope.model.serverName = config.Server;
                    $scope.model.databaseName = config.Database;

                    exportService.getExportModulesMetaData().then(function (exportModulesMetaData) {
                        $scope.model.exportModulesMetaData = exportModulesMetaData;
                        $scope.model.exportModuleSelection = exportModulesMetaData[0];
                    });

                    $scope.selectorsVisible = function() {
                        return exportService.selectorsVisible;
                    };

                    $scope.selectorsToggle = function () {
                        exportService.selectorsVisible = !exportService.selectorsVisible;
                    };

                    $scope.disconnect = function () {
                        configService.disconnect();
                    };

                    $scope.exportReport = function () {
                        exportService.exportReport($scope.model.exportModuleSelection.ModuleCodeName);
                    };

                    $scope.mainMenu = function () {
                        navigationService.mainMenu();
                    };
                }
            }
        }])
        /**
         * Error message displayed at the top of the page.
         */
        .directive('knlErrorMessage', ['knlErrorService', function (errorService) {
            return {
                template: "<div class='alert-box alert' ng-show='model.message'>{{model.message}}</div>",
                link: function ($scope) {
                    $scope.model = $scope.model || {};

                    errorService.listenToErrors(function (message) {
                        $scope.model.message = message;
                    });

                    // Clear the message on every location change
                    $scope.$on('$locationChangeStart', function (event) {
                        $scope.model.message = null;
                    });
                }
            };
        }])
        /**
         * A directive that will displaye a table when given 3 dimensional array-like objects. 
         * That means either array of arrays (that will be displayed without a header) or array of objects (that will be displayed with headers). 
         */
        .directive('knlTable', ['knlErrorService', function (errorService) {
            return {
                restrict: 'E',
                scope: {
                    data: '=tableData'
                },
                templateUrl: "partials/table.html",
                link: function ($scope) {
                    $scope.model = {};

                    $scope.$watch('data', function (data, d) {
                        if (data) {
                            var firstRow = $scope.data[0];
                            if (firstRow) {
                                $scope.model.tableHasHeaders = !angular.isArray(firstRow);

                                // Error handling
                                if (typeof firstRow === 'string') {
                                    errorService.triggerError("Loading invalid data into the table (list instead of table). Didn't you set wrong ResultType in ModuleMetadata?");
                                    return;
                                }

                                $scope.model.headers = Object.keys(firstRow);
                            }
                        }
                    });
                }
            }
        }])
        /**
         * 3-dot loader (used on the connect to target instance button and on module loads).
         */
        .directive('knlLoader', function () {
            return {
                restrict: 'E',
                replace: true,
                templateUrl: "partials/loader.html"
            }
        });
}());