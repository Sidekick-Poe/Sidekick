/** @type {import('tailwindcss').Config} */
const defaultTheme = require('tailwindcss/defaultTheme');

module.exports = {
    content: ["../**/*.{razor,html,cshtml}", "./**/*.js"],
    darkMode: 'selector',
    theme: {
        fontFamily: {
            'sans': ['fontin', ...defaultTheme.fontFamily.sans],
            'caps': ['fontin-smallcaps', 'fontin', ...defaultTheme.fontFamily.sans],
        }
    }
}
