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
        }
      },
      animation: {
        iconChange: 'iconChange 0.2s ease-in-out'
      }
    },
  },
  plugins: [],
};
