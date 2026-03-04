
export function Footer() {
    return (
        <footer className="footer bg-card text-center text-foreground">
            <p className="text-sm font-medium">2025 CodeBattleArena</p>
            <div className="flex justify-center gap-6 mt-4">
                {/* Instagram */}
                <a
                    href="https://www.instagram.com"
                    target="_blank"
                    rel="noopener noreferrer"
                    className="social-link text-primary hover:text-secondary transition-all duration-300"
                >
                    <img
                        src="https://upload.wikimedia.org/wikipedia/commons/thumb/9/95/Instagram_logo_2022.svg/800px-Instagram_logo_2022.svg.png"
                        alt="Instagram"
                        className="w-6 h-6"
                    />
                </a>

                {/* TikTok */}
                <a
                    href="https://www.tiktok.com"
                    target="_blank"
                    rel="noopener noreferrer"
                    className="social-link text-primary hover:text-secondary transition-all duration-300"
                >
                    <img
                        src="https://upload.wikimedia.org/wikipedia/commons/a/a6/Tiktok_icon.svg"
                        alt="TikTok"
                        className="w-6 h-6"
                    />
                </a>

                {/* Telegram */}
                <a
                    href="https://telegram.org"
                    target="_blank"
                    rel="noopener noreferrer"
                    className="social-link text-primary hover:text-secondary transition-all duration-300"
                >
                    <img
                        src="https://upload.wikimedia.org/wikipedia/commons/8/82/Telegram_logo.svg"
                        alt="Telegram"
                        className="w-6 h-6"
                    />
                </a>

                {/* YouTube */}
                <a
                    href="https://www.youtube.com"
                    target="_blank"
                    rel="noopener noreferrer"
                    className="social-link text-primary hover:text-secondary transition-all duration-300"
                >
                    <img
                        src="https://upload.wikimedia.org/wikipedia/commons/4/42/YouTube_icon_%282013-2017%29.png"
                        alt="YouTube"
                        className="w-6 h-6"
                    />
                </a>
            </div>
        </footer>
    );
}

export default Footer;