/// <binding AfterBuild='default' Clean='clean' />

var gulp = require('gulp');
var del = require('del');
var terser = require("gulp-terser");
var rename = require("gulp-rename");

var paths = {
    scripts: ['Scripts/**/*.js']
};

gulp.task('clean', function () {
    return del(['wwwroot/js/**/*']);
});

gulp.task('default', function () {
    return gulp
        .src(paths.scripts)
        .pipe(terser())
        .pipe(rename({ suffix: ".min" }))
        .pipe(gulp.dest("wwwroot/js"));
});
