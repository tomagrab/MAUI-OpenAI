/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./**/*.{razor,html,cs}"],
  theme: {
    extend: {
      colors: {
        backdrop: "#010409",
        darkblue: "#161b22",
        gray: "#6e7681",
        brightWhite: "#f0f6fc",
        blue: "#58a6ff",
        brightBlue: "#79c0ff",
        cyan: "#39c5cf",
        brightCyan: "#56d4dd",
        green: "#3fb950",
        brightGreen: "#56d364",
        purple: "#bc8cff",
        brightPurple: "#d2a8ff",
        red: "#ff7b72",
        brightRed: "#ffa198",
        yellow: "#d29922",
        brightYellow: "#e3b341",
        border: "hsl(var(--border))",
        input: "hsl(var(--input))",
        ring: "hsl(var(--ring))",
        background: "hsl(var(--background))",
        foreground: "hsl(var(--foreground))",
      },
      keyframes: {
        iconChange: {
          '0%': { opacity: '0', transform: 'scale(0.8)' },
          '100%': { opacity: '1', transform: 'scale(1)' },
        },
        fadeIn: {
          '0%': { opacity: 0 },
          '100%': { opacity: 1 },
        },
        fadeOut: {
          '0%': { opacity: 1 },
          '100%': { opacity: 0 },
        },
        slideIn: {
          '0%': { transform: 'translateY(-10px)' },
          '100%': { transform: 'translateY(0)' },
        },
        slideOut: {
          '0%': { transform: 'translateY(0)' },
          '100%': { transform: 'translateY(-10px)' },
        }
      },
      animation: {
        iconChange: 'iconChange 0.2s ease-in-out',
        fadeIn: 'fadeIn 0.3s ease-out forwards',
        fadeOut: 'fadeOut 0.3s ease-out forwards',
        slideIn: 'slideIn 0.3s ease-out forwards',
        slideOut: 'slideOut 0.3s ease-out forwards'
      }
    },
  },
  plugins: [],
};