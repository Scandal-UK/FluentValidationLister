/// <binding AfterBuild='default' Clean='clean' />

var gulp = require('gulp');
var del = require('del');

var paths = {
    scripts: ['Scripts/**/*.js']
};

gulp.task('clean', function () {
    return del(['wwwroot/js/**/*']);
});

gulp.task('default', function () {
    return gulp
        .src(paths.scripts)
        .pipe(gulp.dest('wwwroot/js'));
});
