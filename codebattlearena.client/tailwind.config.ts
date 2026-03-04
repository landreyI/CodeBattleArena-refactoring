/** @type {import('tailwindcss').Config} */
export default {
    darkMode: 'class',
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}"
    ],
    safelist: [
        'scale-[1.1]',
        'scale-[1.15]',
        'scale-[1.2]',
        'scale-[1.25]',
        'scale-[1.3]',
        'scale-[1.35]',
        'scale-[1.4]',
        {
            pattern: /^rounded(-(sm|md|lg|xl|2xl|3xl))?$/, // округления
        },
        {
            pattern: /^shadow(-(sm|md|lg|xl))?$/, // тени
        },
        {
            pattern: /^animate-(fadeIn|spin)$/, // анимации
        },
    ],
    theme: {
        extend: {
            animation: {
                fadeIn: 'fadeIn 0.3s ease-out forwards',
            },
            keyframes: {
                fadeIn: {
                    '0%': { opacity: 0, transform: 'translateY(10px)' },
                    '100%': { opacity: 1, transform: 'translateY(0)' },
                },
            },
            colors: {
                background: 'var(--color-background)',
                foreground: 'var(--color-foreground)',
                primary: 'var(--color-primary)',
                "primary-pressed": 'var(--color-primary-pressed)',

                blue: 'var(--color-blue)',
                green: 'var(--color-green)',
                red: 'var(--color-red)',
                yellow: 'var(--color-yellow)',
                purple: 'var(--color-purple)',
                gray: 'var(--color-gray)',
                bronze: 'var(--color-bronze)',
                destructive: 'var(--color-red)',
            },
        },
    },
    plugins: [],
}
