'use strict';

var fs = require('fs');
var patterns = [
 { from: /return _observableOf\<Message\[\]\>\(null as any\);/g, to : 'return _observableOf(null as any);'}
];

if (process.argv.length < 3) {
    console.log('Usage: node nswag-connections.js <file-to-fix>');
} else {
    console.log('Starting to fix ' + process.argv[2]);
    var data = fs.readFileSync(process.argv[2], { encoding: 'utf8'});
    patterns.forEach(function(item) {
        console.log('Apply pattern:' + item.from);
        data = data.replace(item.from, item.to)
    });
    fs.writeFileSync(process.argv[2], data, { encoding: 'utf8'});
    console.log('Fixing is finished');
}