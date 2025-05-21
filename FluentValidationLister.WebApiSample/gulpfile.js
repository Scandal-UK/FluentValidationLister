/// <binding AfterBuild='default' Clean='clean' />

import gulp from "gulp";
const { task, watch, series, src, dest } = gulp;
import { deleteAsync } from "del";
import terser from "gulp-terser";
import rename from "gulp-rename";

task("clean", function () {
    return deleteAsync(["wwwroot/js/**/*"]);
});

task("watch", function () {
    watch(["Scripts/**/*.ts"], series("default"));
});

task("default", function () {
    return src(["Scripts/**/*.js"])
        .pipe(terser())
        .pipe(rename({ suffix: ".min" }))
        .pipe(dest("wwwroot/js"));
});
