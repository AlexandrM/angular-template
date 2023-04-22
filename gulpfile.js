// JavaScript source code

/// <binding Clean='clean' />
//"use strict";

var gulp = require('gulp');
var exec = require('child_process').exec;

gulp.task('test',
	function() {
		return new Promise(function(resolve, reject) {
			console.log('done');
			resolve();
		});
	});