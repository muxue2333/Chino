module.exports = function (grunt) {
    grunt.initConfig({
        clean :["wwwroot/lib/*", "temp/"],
    }); 

    grunt.loadNpmTasks("grunt-contrib-clean");
};