const { build } = require('esbuild');

build({
  entryPoints: ['src/main.js'],
  outfile: 'SidekickScripts.razor.js',
  bundle: true,
  minify: false,
  sourcemap: true,
});
