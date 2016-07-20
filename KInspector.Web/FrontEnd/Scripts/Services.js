(function (localStorage) {
    /**
     * ALL the services in the application are in here.
     */
    angular.module('knlServices', [])
        /**
         * Handles saving and restoring config of the Kentico instance. Is also able to disconnect from the instance.
         */
		.factory('knlTargetConfigService', function ($rootScope, $location) {
		    return {
		        setConfig: function (c) {
		            localStorage.setItem('knlTargetConfig', JSON.stringify(c));
		        },
		        getConfig: function () {
		            return JSON.parse(localStorage.getItem('knlTargetConfig'));
		        },
		        disconnect: function () {
		            $rootScope.$emit('knlDisconnected');
		            $location.path('/target-setup');
		        }
		    }
		})
        /*
         * Handles navigation.
         */
        .factory('knlNavigationService', function ($location) {
            return {
                mainMenu: function () {
                    $location.path('/main-menu');
                }
            }
        })
        /**
         * Observer pattern for errors in the application.
         */
        .factory('knlErrorService', function () {
            var listeners = [];
            return {
                triggerError: function (msg) {
                    angular.forEach(listeners, function (listener) {
                        listener(msg);
                    });
                },
                listenToErrors: function (cb) {
                    listeners.push(cb);
                }
            }
        })
        /**
         * Handles loading and getting informations about modules. 
         * Caches everything and resets it only on disconnect from Kentico.
         */
        .factory('knlModuleService', ['$http', '$q', '$rootScope', 'knlTargetConfigService', 'knlErrorService', function ($http, $q, $rootScope, configService, errorService) {

            // Reset the cached results on disconnect
            $rootScope.$on('knlDisconnected', function () {
                localStorage.setItem('knlModuleResults', null);
                localStorage.setItem('knlSetupModuleMetadata', null);
                localStorage.setItem('knlModuleMetadata', null);
                localStorage.setItem('knlModuleMetadataSecurity', null);
            });

            // Export related elements visibility
            var mSelectorsVisible = false;

            return {
                /**
                 * Gets selector elements visibility
                 */
                selectorsVisible: function() {
                    return mSelectorsVisible;
                },             

                /**
                 * Toggles selector elements visibility
                 */
                selectorsToggle: function () {
                    mSelectorsVisible = !mSelectorsVisible;
                },
                /**
                 * Gets all the modules from local storage.
                 */
                getAllModules: function () {
                    return JSON.parse(localStorage.getItem('knlModuleResults')) || {};
                },

                /**
                 * Loads a module from server or from local storage.
                 */
                loadModule: function (moduleName, forced) {
                    var deferred = $q.defer();

                    // Try to get the data from local storage
                    var cachedResultArray = JSON.parse(localStorage.getItem('knlModuleResults')) || {};
                    if (!forced && cachedResultArray[moduleName]) {
                        // Return cached results
                        deferred.resolve(cachedResultArray[moduleName]);
                    }
                    else {
                        // Return results from server and cache them
                        var paramsWithModuleName = angular.extend({ moduleName: moduleName }, configService.getConfig());
                        $http.get("http://localhost:9000/api/modules/GetModuleResult", { params: paramsWithModuleName })
                            .success(function (data) {
                                // Get it the second time so there's no concurrent issues
                                var resultArray = JSON.parse(localStorage.getItem('knlModuleResults')) || {};
                                resultArray[moduleName] = data;
                                localStorage.setItem('knlModuleResults', JSON.stringify(resultArray));
                                deferred.resolve(data);
                            })
                            .error(deferred.error);
                    }

                    return deferred.promise;
                },

                /**
                 * Runs selected modules and returns module results as a file
                 */
                exportReportService: function (exportType, moduleNames) {
                    if (exportType == undefined) {
                        errorService.triggerError("No export type selected");
                        return;
                    }
                    
                    alert("Module selection not implemented. All modules except screenshotter selected.");
                    //return;

                    moduleNames = [
                        "Application restarts",
                        "Attachments by size",
                        "Cache items",
                        "Contact groups with manual macro",
                        "Database consistency check",
                        "Documents consistency issues",
                        "Duplicate Page Aliases",
                        "Event log errors",
                        "Expired tokens",
                        "Important scheduled tasks",
                        "Important settings",
                        "Inactive contact deletion settings",
                        "Kentico instance information",
                        "Maximum 100 files per folder in Azure Blob storage",
                        "Not found errors (404)",
                        "Number of children of TreeNode",
                        "Number of document aliases",
                        "Page type fields data type mismatch",
                        "Page type is assigned to a site",
                        "Performance demanding web parts in transformations",
                        "Site templates",
                        "Size of the online marketing tables",
                        "Top 25 tables by size",
                        "Unspecified 'columns' setting in web parts",
                        "Workflow consistency"
                    ];

                    // Fast test
                    //moduleNames = ["Event log errors", "Unspecified 'columns' setting in web parts", "Maximum 100 files per folder in Azure Blob storage"];

                    var paramsWithModuleNames = angular.extend({ moduleNames: moduleNames }, configService.getConfig(), { exportType: exportType });
                    var url = "http://localhost:9000/api/modules/GetModulesResults?" + $.param(paramsWithModuleNames);

                    window.open(url, '_blank');
                },

                /**
                 * Gets all the setup module metadata from the server. Uses local storage for caching.
                 * Setup modules are used for modifying the analysed instance in order to prevent it from staging, sending emails or synchronizing across web farm.
                 */
                getAllSetupMetadata: function () {
                    var deferred = $q.defer();
                    var cachedMetaData = JSON.parse(localStorage.getItem('knlSetupModuleMetadata'));

                    if (cachedMetaData) {
                        // Return cached results
                        deferred.resolve(cachedMetaData);
                    }
                    else {
                        // Return results from server and cache them
                        $http.get("http://localhost:9000/api/modules/GetSetupModulesMetadata", { cache: true, params: configService.getConfig() })
                            .success(function(data) {
                                var moduleMetaData = {};
                                angular.forEach(data, function(metaData) {
                                    moduleMetaData[metaData.Name] = metaData;
                                });

                                localStorage.setItem('knlSetupModuleMetadata', JSON.stringify(moduleMetaData));
                                deferred.resolve(moduleMetaData);
                            })
                            .error(deferred.error);
                    }
                    return deferred.promise;
                },

                /**
                 * Gets all the module metadata from the server. Uses local storage for caching.
                 * The category argument is optional.
                 */
                getAllMetadata: function (category) {
                    if (angular.isUndefined(category)) {
                        category = '';
                    }

                    var deferred = $q.defer();
                    var cachedMetaData = JSON.parse(localStorage.getItem('knlModuleMetadata' + category));

                    if (cachedMetaData) {
                        // Return cached results
                        deferred.resolve(cachedMetaData);
                    }
                    else {
                        // Return results from server and cache them
                        var params = configService.getConfig();
                        params.category = category;
                        $http.get("http://localhost:9000/api/modules/GetModulesMetadata", { cache: true, params: params })
                            .success(function(data) {
                                var moduleMetaData = {};
                                angular.forEach(data, function(metaData) {
                                    moduleMetaData[metaData.Name] = metaData;
                                });

                                localStorage.setItem('knlModuleMetadata' + category, JSON.stringify(moduleMetaData));
                                deferred.resolve(moduleMetaData);
                            })
                            .error(deferred.error);
                    }
                    return deferred.promise;
                }
            }
        }]);
}(window.localStorage));