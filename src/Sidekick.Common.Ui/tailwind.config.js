/** @type {import('tailwindcss').Config} */
const defaultTheme = require('tailwindcss/defaultTheme');

module.exports = {
    content: ["../**/*.{razor,html,cshtml,cs}", "./**/*.js"],
    darkMode: 'selector',
    theme: {
        fontFamily: {
            'sans': ['fontin', ...defaultTheme.fontFamily.sans],
            'caps': ['fontin-smallcaps', 'fontin', ...defaultTheme.fontFamily.sans],
        },
        fontSize: {
            xs: '0.625rem',
            sm: '0.75rem',
            base: '0.875rem',
            lg: '1rem',
            xl: '1.125rem',
            '2xl': '1.5rem',
            '3xl': '1.875rem'
        }
    }
}
