import { build } from 'esbuild';

build({
    entryPoints: ['Scripts/index.js'],
    outfile: 'wwwroot/js/sidekick.js',
    bundle: true,
    minify: true,
    sourcemap: true,
});
