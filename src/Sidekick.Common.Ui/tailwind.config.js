/** @type {import('tailwindcss').Config} */

const { fontFamily } = require("tailwindcss/defaultTheme");

module.exports = {
    content: ["../**/*.{razor,html,cshtml,cs}", "./**/*.js"],
    extract: {
        DEFAULT: (content) => {
            const matches = content.match(/(?:class|AdditionalClasses)="([^"]+)"/g) || [];
            return matches.map((match) => match.replace(/(?:class|AdditionalClasses)="|"/g, ''));
        },
    },
    darkMode: "selector",
    theme: {
        fontFamily: {
            sans: ["fontin", ...fontFamily.sans],
            caps: ["fontin-smallcaps", "fontin", ...fontFamily.sans],
        },
        fontSize: {
            xs: "0.625rem",
            sm: "0.75rem",
            base: "0.875rem",
            lg: "1rem",
            xl: "1.125rem",
            "2xl": "1.5rem",
            "3xl": "1.875rem",
        },
    },
};
