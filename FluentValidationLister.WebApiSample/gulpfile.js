/// <binding AfterBuild='default' Clean='clean' />

var gulp = require("gulp");
var del = require("del");
var terser = require("gulp-terser");
var rename = require("gulp-rename");

var paths = {
    source: ["Scripts/**/*.ts"],
    scripts: ["Scripts/**/*.js"],
    deployedScripts: ["wwwroot/js/**/*"],
    deployTarget: "wwwroot/js"
};

gulp.task("clean", function () {
    return del(paths.deployedScripts);
});

gulp.task("watch", function () {
    gulp.watch(paths.source, gulp.series("default"));
});

gulp.task("default", function () {
    return gulp
        .src(paths.scripts)
        .pipe(terser())
        .pipe(rename({ suffix: ".min" }))
        .pipe(gulp.dest(paths.deployTarget));
});
