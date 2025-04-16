/// <binding AfterBuild='default' Clean='clean' />

import gulp from "gulp";
const { task, watch, series, src, dest } = gulp;
import { deleteAsync } from "del";
import terser from "gulp-terser";
import rename from "gulp-rename";

var paths = {
    source: ["Scripts/**/*.ts"],
    scripts: ["Scripts/**/*.js"],
    deployedScripts: ["wwwroot/js/**/*"],
    deployTarget: "wwwroot/js"
};

task("clean", function () {
    return deleteAsync(paths.deployedScripts);
});

task("watch", function () {
    watch(paths.source, series("default"));
});

task("default", function () {
    return src(paths.scripts)
        .pipe(terser())
        .pipe(rename({ suffix: ".min" }))
        .pipe(dest(paths.deployTarget));
});
