import { Link } from "react-router-dom";

export function InfoPage() {
    return (
        <div className="glow-box">
            <div className="max-w-4xl mx-auto text-center">
                <h1 className="text-5xl font-bold text-primary mb-8">
                    Welcome to Code Battle Arena!
                </h1>

                <p className="text-xl mb-6 leading-relaxed">
                    Code Battle Arena is the ultimate platform to challenge your friends and compete in fun, timed programming challenges!
                    Whether you're a beginner or a pro, there's something here for everyone.
                </p>

                <p className="text-xl mb-6">
                    <strong className="text-primary">Compete with Friends:</strong> You can create both
                    <span className="text-primary"> public</span> and
                    <span className="text-primary"> private</span> sessions.
                    Challenge your friends or meet new ones!
                </p>

                <p className="text-xl mb-6">
                    <strong className="text-primary">Join the Conversation:</strong> Chat with your friends or competitors in real-time
                    with our integrated messaging feature. Discuss strategies, share tips, and have fun!
                </p>

                <p className="text-xl mb-6">
                    <strong className="text-primary">Weekly Rankings:</strong> Every week, the top 3 players are selected and celebrated for their coding prowess.
                    Are you up for the challenge?
                </p>

                <div className="bg-gradient-to-r bg-primary p-6 rounded-lg shadow-lg text-black transform transition duration-300 hover:scale-105">
                    <p className="text-lg font-bold mb-4 text-center">Key Features:</p>
                    <ul className="list-disc pl-8 text-left text-lg">
                        <li>Compete in time-based coding challenges</li>
                        <li>Create both public and private sessions</li>
                        <li>Real-time chat to connect with other players</li>
                        <li>Weekly Top 3 players and leaderboard</li>
                    </ul>
                </div>

                <p className="text-xl mt-8 leading-relaxed">
                    Join now and start coding with friends or compete with other talented coders around the world!
                </p>

                <Link to="/session/create-session" className="mt-8 px-6 py-3 bg-primary text-black btn-animation">
                    Get Started
                </Link>
            </div>
        </div>
    );
}

export default InfoPage;
