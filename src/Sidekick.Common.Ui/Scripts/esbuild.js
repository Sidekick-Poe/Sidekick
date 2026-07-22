import { build } from 'esbuild';

await build({
    entryPoints: ['Scripts/index.js'],
    outfile: 'wwwroot/js/sidekick.js',
    bundle: true,
    minify: true,
    sourcemap: "inline",
});
