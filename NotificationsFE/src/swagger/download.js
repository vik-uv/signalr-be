'use strict';

var fs = require('fs');
var https = require('http');

if (process.argv.length < 4) {
    console.log('Usage: node download.js <URL> <file-to-save>');
} else {
    var url = process.argv[2];
    var fileToSave = process.argv[3];
    
    https.get(url, function(res) {
        var filePath = fs.createWriteStream(fileToSave);
        res.pipe(filePath);
        filePath.on('finish',function() {
            filePath.close();
            console.log('Download Completed'); 
        })
    });
}